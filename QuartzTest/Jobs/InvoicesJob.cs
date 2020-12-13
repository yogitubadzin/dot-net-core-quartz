using System.Threading.Tasks;
using Quartz;
using QuartzTest.Processors;
using QuartzTest.Services.Logging;

namespace QuartzTest.Jobs
{
    public class InvoicesJob : BaseJob
    {
        private readonly IInvoicesProcessor _invoicesProcessor;

        public InvoicesJob(ICorrelationIdGenerator correlationIdGenerator, IAppLogger appLogger, IInvoicesProcessor invoicesProcessor) : base(correlationIdGenerator, appLogger)
        {
            _invoicesProcessor = invoicesProcessor;
        }

        protected override string JobName { get; set; } = nameof(InvoicesJob);

        protected override async Task ExecuteJob(IJobExecutionContext context)
        {
            await _invoicesProcessor.Run();
        }
    }
}