using System;
using System.Collections.Generic;
using System.Linq;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Infrastructure.Serialization;
using DSH.DiscordBot.Infrastructure.Web;
using DSH.DiscordBot.Sources.Api.Entities;
using Hero = DSH.DiscordBot.Contract.Dto.Hero;
using Build = DSH.DiscordBot.Contract.Dto.Build;

namespace DSH.DiscordBot.Sources
{
    public sealed class ApiSource : ISource
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<ISerializer> _serializer;
        private readonly Lazy<IClient> _client;
        
        public ApiSource(
            Lazy<ILog> log,
            Lazy<ISerializer> serializer,
            Lazy<IClient> client)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        
        public IEnumerable<Hero> GetHeroes(IEnumerable<Source> sources)
        {
            var heroes = new List<Hero>();

            _log.Value.Info("Starting get heroes from the api sources");

            if (sources == null)
                return heroes;

            foreach (var source in sources)
            {
                if (source?.Type != SourceType.Api)
                    continue;
                
                var url = source.Url;
                if (url == null)
                    continue;

                _log.Value.Info($"Get heroes from {url}");
                
                var sourceName = url.Host.ToUpperInvariant();

                try
                {
                    var heroesFromApi = _serializer.Value.Deserialize<Heroes>(
                        _client.Value.GetString(url.ToString()).Result);

                    if (heroesFromApi != null)
                    {
                        foreach (var property in heroesFromApi.GetType().GetProperties())
                        {
                            var hero = GetHero(sourceName, (Api.Entities.Hero)property.GetValue(heroesFromApi));
                            if (hero != null)
                                heroes.Add(hero);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Value.Error(e);
                }
            }

            _log.Value.Debug(
                $"Scrapped heroes:{Environment.NewLine}{_serializer.Value.Serialize(heroes)}");

            return heroes;
        }

        private static Hero GetHero(string sourceName, Api.Entities.Hero hero)
        {
            if (string.IsNullOrWhiteSpace(hero?.Name) || !(hero.Builds?.Length > 0)) 
                return null;
            
            var name = NormalizeName(hero.Name);
            return new Hero()
            {
                Name = name,
                ImageUrl = new Uri($"http://www.heroesfire.com/images/wikibase/icon/heroes/{name}.png"),
                Builds = hero.Builds.Select(_ => new Build()
                {
                    Source = sourceName,
                    Title = _.Name,
                    Url = new Uri(_.Url)
                })
            };
        }

        private static string NormalizeName(string name)
        {
            name = name.Replace("'", "").ToLowerInvariant();
            name = name.Replace(" ", "-");
            name = name.Replace(".", "");
            
            if (name == "butcher") 
                name = "the-butcher";
            if (name == "lost-vikings") 
                name = "the-lost-vikings";
  
            return name;
        }
    }
}