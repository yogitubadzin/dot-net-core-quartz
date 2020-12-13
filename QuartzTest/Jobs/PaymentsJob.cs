using System.Threading.Tasks;
using Quartz;
using QuartzTest.Processors;
using QuartzTest.Services.Logging;

namespace QuartzTest.Jobs
{
    public class PaymentsJob : BaseJob
    {
        private readonly IPaymentsProcessor _paymentsProcessor;

        public PaymentsJob(ICorrelationIdGenerator correlationIdGenerator, IAppLogger appLogger, IPaymentsProcessor paymentsProcessor) : base(correlationIdGenerator, appLogger)
        {
            _paymentsProcessor = paymentsProcessor;
        }

        protected override string JobName { get; set; } = nameof(PaymentsJob);

        protected override async Task ExecuteJob(IJobExecutionContext context)
        {
            await _paymentsProcessor.Run();
        }
    }
}