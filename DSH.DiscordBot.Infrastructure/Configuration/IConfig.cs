using System.Collections.Generic;

namespace DSH.DiscordBot.Infrastructure.Configuration
{
    public interface IConfig
    {
        IEnumerable<string> Sources { get; }
        string Token { get; }
        string DbConnectionString { get; }
        string AdminName { get; }
        char CommandPrefix { get; }
    }
}