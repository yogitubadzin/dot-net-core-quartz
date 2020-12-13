using System;
using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzTest.Extensions;
using QuartzTest.Jobs;
using QuartzTest.Services.Configuration;
using QuartzTest.Services.Logging;

namespace QuartzTest.Services.Scheduling
{
    public class ScheduleService
    {
        private readonly IAppConfigurationProvider _appConfigurationProvider;
        private readonly IAppLogger _appLogger;
        private readonly IScheduler _scheduler;

        public ScheduleService(IServiceProvider serviceProvider)
        {
            _appConfigurationProvider = serviceProvider.GetRequiredService<IAppConfigurationProvider>();
            var jobFactory = serviceProvider.GetRequiredService<IJobFactory>();
            _appLogger = serviceProvider.GetRequiredService<IAppLogger>();

            var props = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"},
                {"quartz.scheduler.instanceName", "MyScheduler"},
                {"quartz.jobStore.type", "Quartz.Simpl.RAMJobStore, Quartz"},
                {"quartz.threadPool.threadCount", "10"}
            };
            var factory = new StdSchedulerFactory(props);
            _scheduler = factory.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
            _scheduler.JobFactory = jobFactory;
        }

        public void Start()
        {
            _appLogger.Info("Service event: Start");
            _scheduler.Start().ConfigureAwait(false).GetAwaiter().GetResult();
            ScheduleJobs();
        }

        private void ScheduleJobs()
        {
            ApplyPaymentsJob();
            ApplyInvoices();
        }

        private ITrigger CreateTrigger(string name, string group, string cronSchedule,
            RunJobScheduleOption runJobScheduleOption)
        {
            switch (runJobScheduleOption)
            {
                case RunJobScheduleOption.NotRun:
                    return TriggerBuilder.Create()
                        .WithIdentity(name, group)
                        .StartAt(DateTimeOffset.MaxValue)
                        .Build();
                case RunJobScheduleOption.CronExpression:
                    return TriggerBuilder.Create()
                        .WithIdentity(name, group)
                        .WithCronSchedule(cronSchedule)
                        .Build();
                case RunJobScheduleOption.RunNow:
                    return TriggerBuilder.Create()
                        .WithIdentity(name, group)
                        .StartNow()
                        .Build();
                default:
                    _appLogger.Error(
                        $"RunJobScheduleOption : {runJobScheduleOption} not found for Trigger Name: {name}, Trigger Group {group}.");
                    return TriggerBuilder.Create()
                        .WithIdentity(name, group)
                        .StartAt(DateTimeOffset.MaxValue)
                        .Build();
            }
        }

        private static IJobDetail CreateJobDetail<T>(string name, string group) where T : IJob
        {
            return JobBuilder.Create<T>()
                .WithIdentity(name, group)
                .Build();
        }

        public void Stop()
        {
            _appLogger.Info("Service event: Stop");
            _scheduler.Shutdown().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void ApplyInvoices()
        {
            var cronInvoices =
                _appConfigurationProvider.GetValue("Scheduling:CronExpression:InvoicesJob");
            var invoicesJobOption =
                _appConfigurationProvider.GetValue("Scheduling:StartProcessOption:StartInvoicesJobOption");
            var invoicesGroup = "InvoicesGroup";
            var invoicesJob =
                CreateJobDetail<InvoicesJob>("InvoicesJob", invoicesGroup);
            var invoicesTrigger = CreateTrigger("InvoicesTrigger", invoicesGroup,
                cronInvoices, (RunJobScheduleOption) invoicesJobOption.ParseToNumber());
            _scheduler.ScheduleJob(invoicesJob, invoicesTrigger).ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private void ApplyPaymentsJob()
        {
            var cronPayments =
                _appConfigurationProvider.GetValue("Scheduling:CronExpression:PaymentsJob");
            var paymentsJobOption =
                _appConfigurationProvider.GetValue("Scheduling:StartProcessOption:StartPaymentsJobOption");
            var paymentsGroup = "PaymentsGroup";
            var paymentsJob =
                CreateJobDetail<PaymentsJob>("PaymentsJob", paymentsGroup);
            var paymentsTrigger = CreateTrigger("PaymentsTrigger", paymentsGroup,
                cronPayments, (RunJobScheduleOption) paymentsJobOption.ParseToNumber());
            _scheduler.ScheduleJob(paymentsJob, paymentsTrigger).ConfigureAwait(false).GetAwaiter()
                .GetResult();
        }
    }
}