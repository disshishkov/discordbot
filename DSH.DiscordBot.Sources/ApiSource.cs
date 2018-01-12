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
                        SaveHero(sourceName, heroesFromApi.Abathur, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Alarak, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Alexstrasza, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Ana, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Anubarak, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Artanis, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Arthas, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Auriel, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Azmodan, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Brightwing, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Butcher, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Cassia, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Chen, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Cho, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Chromie, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Dehaka, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Diablo, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Dva, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Etc, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Falstad, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Gall, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Garrosh, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Gazlowe, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Genji, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Greymane, ref heroes);
                        SaveHero(sourceName, heroesFromApi.GulDan, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Hammer, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Illidan, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Jaina, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Johanna, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Junkrat, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Kaelthas, ref heroes);
                        SaveHero(sourceName, heroesFromApi.KelThuzad, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Kerrigan, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Kharazim, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Leoric, ref heroes);
                        SaveHero(sourceName, heroesFromApi.LiLi, ref heroes);
                        SaveHero(sourceName, heroesFromApi.LiMing, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Lucio, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Lunara, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Malfurion, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Malthael, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Medivh, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Morales, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Muradin, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Murky, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Nazeebo, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Nova, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Probius, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Ragnaros, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Raynor, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Rehgar, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Rexxar, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Samuro, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Sonya, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Stitches, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Stukov, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Sylvanas, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Tassadar, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Thrall, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Tlv, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Tracer, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Tychus, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Tyrael, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Tyrande, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Uther, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Valeera, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Xul, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Zagara, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Zarya, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Zarya, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Zeratul, ref heroes);
                        SaveHero(sourceName, heroesFromApi.ZulJin, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Hanzo, ref heroes);
                        SaveHero(sourceName, heroesFromApi.Blaze, ref heroes);
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

        private static void SaveHero(string sourceName, Api.Entities.Hero hero, ref List<Hero> heroes)
        {
            if (hero != null && hero.Builds?.Length > 0
                && !string.IsNullOrWhiteSpace(hero.Name))
            {
                var name = NormalizeName(hero.Name);
                heroes.Add(new Hero()
                {
                    Name = name,
                    ImageUrl = new Uri($"http://www.heroesfire.com/images/wikibase/icon/heroes/{name}.png"),
                    Builds = hero.Builds.Select(_ => new Build()
                    {
                        Source = sourceName,
                        Title = _.Name,
                        Url = new Uri(_.Url)
                    })
                });
            }
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