namespace QuartzTest.Services.Configuration
{
    public interface IAppConfigurationProvider
    {
        string GetValue(string sectionName);
    }
}