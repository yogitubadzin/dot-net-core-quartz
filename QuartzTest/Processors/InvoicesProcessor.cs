using System;
using System.Threading;
using System.Threading.Tasks;
using QuartzTest.Services.Logging;

namespace QuartzTest.Processors
{
    public class InvoicesProcessor : IInvoicesProcessor
    {
        private readonly IAppLogger _appLogger;

        public InvoicesProcessor(IAppLogger appLogger)
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
                    Thread.Sleep(200);
                    _appLogger.Info($"Hello world from Invoices Processor. Counter: {counter}");
                    Thread.Sleep(700);
                }
                catch (System.Exception)
                {
                    _appLogger.Error($"Error from Invoices Processor. Counter: {counter}");
                    if (counter == 20)
                    {
                        throw;
                    }
                }
            }

            throw new Exception("Error in InvoicesProcessor.");
        }
    }
}