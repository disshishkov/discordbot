using System.Collections.Specialized;
using System.Configuration;

namespace DSH.DiscordBot.Infrastructure.Configuration
{
    public sealed class AppSettings : IAppSettings
    {
        public NameValueCollection Get()
        {
            return ConfigurationManager.AppSettings;
        }
    }
}