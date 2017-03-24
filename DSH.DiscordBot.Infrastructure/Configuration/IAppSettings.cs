using System.Collections.Specialized;

namespace DSH.DiscordBot.Infrastructure.Configuration
{
    public interface IAppSettings
    {
        NameValueCollection Get();
    }
}