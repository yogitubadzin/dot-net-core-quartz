using System.IO;
using Microsoft.Extensions.Configuration;

namespace QuartzTest.Services.Configuration
{
    public class AppConfigurationProvider : IAppConfigurationProvider
    {
        private readonly string appSettingFileName = "appSettings.json";
        private IConfiguration _config;

        public AppConfigurationProvider()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile(appSettingFileName, false, true)
                .AddEnvironmentVariables()
                .SetBasePath(GetBasePath())
                .Build();
        }

        public string GetValue(string sectionName)
        {
            return _config.GetSection(sectionName).Value;
        }

        private string GetBasePath()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}
