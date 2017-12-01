using System;
using System.Collections.Generic;

namespace DSH.DiscordBot.Infrastructure.Configuration
{
    public interface IConfig
    {
        IEnumerable<Uri> Sources { get; }
        string Token { get; }
        string DbConnectionString { get; }
        string CommandPrefix { get; }
    }
}