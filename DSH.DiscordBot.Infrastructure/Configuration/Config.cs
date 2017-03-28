﻿using System;

namespace DSH.DiscordBot.Infrastructure.Configuration
{
    public sealed class Config : IConfig
    {
        private readonly Lazy<IAppSettings> _settings;

        public Config(Lazy<IAppSettings> settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _settings = settings;
        }

        public string Token => _settings.Value.Get()["Token"];
        public string DbConnectionString => _settings.Value.Get()["DbConnectionString"];
        public string AdminName => _settings.Value.Get()["AdminName"];
        public char CommandPrefix => _settings.Value.Get()["CommandPrefix"][0];
    }
}