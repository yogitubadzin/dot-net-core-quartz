namespace QuartzTest.Services.Logging
{
    public interface IAppLogger
    {
        void Info(string text);

        void Error(string text);

        void Warn(string text);

        void Debug(string text);

        void Fatal(string text);
    }
}