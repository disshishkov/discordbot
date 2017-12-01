using System;
using System.Collections.Generic;
using System.Linq;

namespace DSH.DiscordBot.Infrastructure.Configuration
{
    public sealed class Config : IConfig
    {
        private readonly Lazy<IAppSettings> _settings;

        public Config(Lazy<IAppSettings> settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public string Token => _settings.Value.Get()["Token"];
        public string DbConnectionString => _settings.Value.Get()["DbConnectionString"];
        public string CommandPrefix => _settings.Value.Get()["CommandPrefix"];

        public IEnumerable<Uri> Sources
        {
            get
            {
                var sourcesStr = _settings.Value.Get()["Sources"];

                return string.IsNullOrWhiteSpace(sourcesStr)
                    ? null
                    : sourcesStr.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(url => new Uri(url));
            }
        }
    }
}