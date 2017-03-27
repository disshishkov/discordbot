using System;

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

        public string Token => this._settings.Value.Get()["Token"];
        public string DbConnectionString => this._settings.Value.Get()["DbConnectionString"];
        public char CommandPrefix => this._settings.Value.Get()["CommandPrefix"][0];
    }
}