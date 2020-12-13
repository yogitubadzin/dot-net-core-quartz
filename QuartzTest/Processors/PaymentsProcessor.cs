using System.Threading;
using System.Threading.Tasks;
using QuartzTest.Services.Logging;

namespace QuartzTest.Processors
{
    public class PaymentsProcessor  : IPaymentsProcessor
    {
        private readonly IAppLogger _appLogger;

        public PaymentsProcessor(IAppLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public Task Run()
        {
            int counter = 0;
            while (counter < 20)
            {
                try
                {
                    counter++;
                    Thread.Sleep(400);
                    _appLogger.Info($"Hello world from Payments Processor. Counter: {counter}");
                    Thread.Sleep(300);
                }
                catch (System.Exception)
                {
                    _appLogger.Error($"Error from Payments Processor. Counter: {counter}");
                    if (counter == 20)
                    {
                        throw;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}