using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using QuartzTest.Services.Logging;

namespace QuartzTest.Ioc.Jobs
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider container;

        public JobFactory(IServiceProvider container)
        {
            this.container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                return container.GetService(bundle.JobDetail.JobType) as IJob;
            }
            catch (Exception exception)
            {
                var appLogger = container.GetRequiredService<IAppLogger>();
                appLogger.Error($"Registration for job was failed (Jobs are on another thread). Error message: {exception.Message}, Stack trace: {exception.StackTrace}");

                throw;
            }
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}