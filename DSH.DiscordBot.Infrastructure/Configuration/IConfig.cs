using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Infrastructure.Configuration
{
    public interface IConfig
    {
        IEnumerable<Source> Sources { get; }
        string Token { get; }
        string DbConnectionString { get; }
        string CommandPrefix { get; }
    }
}