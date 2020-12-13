using System;
using Microsoft.Extensions.DependencyInjection;
using QuartzTest.Extensions;
using QuartzTest.Ioc;
using QuartzTest.Services.Configuration;
using QuartzTest.Services.Logging;
using QuartzTest.Services.Scheduling;
using Topshelf;
using Topshelf.Runtime;

namespace QuartzTest
{
    public static class Program
    {
        private const string ServiceName = "Quartz";

        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var serviceProvider = Bootstrapper.GetServiceProvider();

            var appLogger = serviceProvider.GetRequiredService<IAppLogger>();
            var appConfigurationProvider = serviceProvider.GetRequiredService<IAppConfigurationProvider>();

            var intervalInSeconds = appConfigurationProvider
                .GetValue("WindowsServiceSettings:RecoverServiceDelayInSeconds").ParseToNumber();

            appLogger.Info("start");

            var rc = HostFactory.Run(x =>
            {
                x.Service<ScheduleService>(s =>
                {
                    s.ConstructUsing(sc => new ScheduleService(serviceProvider));
                    s.WhenStarted(sc => sc.Start());
                    s.WhenStopped(sc => sc.Stop());
                    s.BeforeStartingService(sc => { appLogger.Info("Service event: BeforeStartingService"); });
                    s.AfterStartingService(sc => { appLogger.Info("Service event: AfterStartingService"); });
                    s.BeforeStoppingService(sc => { appLogger.Info("Service event: BeforeStoppingService"); });
                    s.AfterStoppingService(sc => { appLogger.Info("Service event: AfterStoppingService"); });
                    s.WhenPaused(sc => { appLogger.Info("Service event: WhenPaused"); });
                    s.WhenContinued(sc => { appLogger.Info("Service event: WhenContinued"); });
                });
                x.RunAsLocalSystem();
                x.SetDescription(ServiceName);
                x.SetDisplayName(ServiceName);
                x.SetServiceName(ServiceName);
                x.StartAutomatically();
                x.OnException(exception =>
                {
                    Console.WriteLine($"Exception occured in Schedule. Message : {exception.Message}, StackTrace: {exception.StackTrace}");
                });

                x.UnhandledExceptionPolicy = UnhandledExceptionPolicyCode.LogErrorAndStopService;
                x.EnableServiceRecovery(r =>
                {
                    r.OnCrashOnly();
                    r.RestartService(TimeSpan.FromSeconds(intervalInSeconds));
                    r.RestartService(TimeSpan.FromSeconds(intervalInSeconds));
                    r.RestartService(TimeSpan.FromSeconds(intervalInSeconds));
                    r.SetResetPeriod(1);
                });
            });

            var exitCode = (int) Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"Unhandled exception: {e.ExceptionObject}");
        }
    }
}