using System;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using QuartzTest.Services.Logging;

namespace QuartzTest.Jobs
{
    public abstract class BaseJob : IJob
    {
        private readonly ICorrelationIdGenerator _correlationIdGenerator;
        protected readonly IAppLogger _appLogger;

        protected BaseJob(ICorrelationIdGenerator correlationIdGenerator, IAppLogger appLogger)
        {
            _correlationIdGenerator = correlationIdGenerator;
            _appLogger = appLogger;
        }

        protected abstract string JobName { set; get; }

        protected abstract Task ExecuteJob(IJobExecutionContext context);

        public async Task Execute(IJobExecutionContext context)
        {
            var correlationId = _correlationIdGenerator.Generate();
            try
            {
                JobNameGuard();

                _appLogger.Info($"{JobName} Job with correlationId {correlationId} is starting.");

                await ExecuteJob(context);

                _appLogger.Info($"{JobName} Job with correlationId {correlationId} is finished.");
            }
            catch (Exception exception)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"{JobName} with correlationId {correlationId} is failed.");
                stringBuilder.AppendLine();
                stringBuilder.Append($"Message:{exception.Message}");
                stringBuilder.AppendLine();
                stringBuilder.Append($"StackTrace:{exception.StackTrace}");
                stringBuilder.AppendLine();

                _appLogger.Error(stringBuilder.ToString());
            }
        }

        private void JobNameGuard()
        {
            if (string.IsNullOrWhiteSpace(JobName))
            {
                throw new Exception("JobName is not provided");
            }
        }
    }
}