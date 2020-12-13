using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using QuartzTest.Ioc.Jobs;
using QuartzTest.Jobs;
using QuartzTest.Processors;
using QuartzTest.Services.Configuration;
using QuartzTest.Services.Logging;

namespace QuartzTest.Ioc
{
    public static class Bootstrapper
    {
        public static IServiceProvider GetServiceProvider(Action<ServiceCollection> action = null)
        {
            var services = new ServiceCollection();

            RegisterLogging(services);
            RegisterConfiguration(services);
            RegisterJobs(services);
            RegisterProcessors(services);

            action?.Invoke(services);

            return services.BuildServiceProvider();
        }

        private static void RegisterProcessors(ServiceCollection services)
        {
            services.AddTransient<IPaymentsProcessor, PaymentsProcessor>();
            services.AddTransient<IInvoicesProcessor, InvoicesProcessor>();
        }

        private static void RegisterConfiguration(ServiceCollection services)
        {
            services.AddSingleton<IAppConfigurationProvider, AppConfigurationProvider>();
        }

        private static void RegisterLogging(IServiceCollection services)
        {
            services.AddSingleton<IAppLogger, AppLogger>();
        }

        private static void RegisterJobs(IServiceCollection services)
        {
            services.AddSingleton<IJobFactory>(provider =>
            {
                var jobFactory = new JobFactory(provider);
                return jobFactory;
            });

            services.AddSingleton<InvoicesJob>();
            services.AddSingleton<PaymentsJob>();

            services.AddSingleton<ICorrelationIdGenerator, CorrelationIdGenerator>();
        }
    }
}