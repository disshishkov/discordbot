﻿namespace DSH.DiscordBot.Infrastructure.Configuration
{
    public interface IConfig
    {
        string Token { get; }
        string DbConnectionString { get; }
    }
}