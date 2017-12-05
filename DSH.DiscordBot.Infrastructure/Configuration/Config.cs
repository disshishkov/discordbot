using System;
using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

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
        public int HeroesCountInList => int.Parse(_settings.Value.Get()["HeroesCountInList"]);

        public IEnumerable<Source> Sources
        {
            get
            {
                var result = new List<Source>();
                var sourcesStr = _settings.Value.Get()["Sources"];

                if (string.IsNullOrWhiteSpace(sourcesStr))
                    return null;

                foreach (var sourceStr in sourcesStr.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = sourceStr.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        result.Add(new Source()
                        {
                            Type = (SourceType)Enum.Parse(typeof(SourceType), parts[0], true),
                            Url = new Uri(parts[1])
                        });
                    }
                }

                return result;
            }
        }
    }
}