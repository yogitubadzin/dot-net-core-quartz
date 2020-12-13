using System;

namespace QuartzTest.Services.Logging
{
    public class AppLogger : IAppLogger
    {
        public void Info(string text)
        {
            Console.WriteLine($"Info: {text}");
        }

        public void Error(string text)
        {
            Console.WriteLine($"Error: {text}");
        }

        public void Warn(string text)
        {
            Console.WriteLine($"Warn: {text}");
        }

        public void Debug(string text)
        {
            Console.WriteLine($"Debug: {text}");
        }

        public void Fatal(string text)
        {
            Console.WriteLine($"Fatal: {text}");
        }
    }
}