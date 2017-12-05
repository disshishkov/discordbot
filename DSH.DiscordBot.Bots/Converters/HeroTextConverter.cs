using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Configuration;

namespace DSH.DiscordBot.Bots.Converters
{
    public sealed class HeroTextConverter : IHeroTextConverter
    {
        private readonly Lazy<IConfig> _config;
        
        public HeroTextConverter(Lazy<IConfig> config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string Convert(Hero hero)
        {
            if (hero == null)
                return "Hero is not exist";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"`{hero.Name}`");

            if (!(hero.Builds?.Any() ?? false))
            {
                sb.AppendLine("No one build was added");
            }
            else
            {
                foreach (var builds in hero.Builds.GroupBy(_ => _.Source))
                {
                    sb.AppendLine($"**{builds.Key?.ToLowerInvariant()}**");
                    foreach (var build in builds)
                    {
                        sb.AppendLine($"{build.Title} - {build.Url}");
                    }
                }
            }

            return sb.ToString();
        }

        public IEnumerable<string> Convert(IEnumerable<Hero> heroes)
        {
            var enumerable = heroes as Hero[] ?? heroes?.ToArray();

            if (!(enumerable?.Any() ?? false))
                yield return "No one hero was added";

            foreach (var chunckedHeroes in Split(enumerable, _config.Value.HeroesCountInList))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var hero in chunckedHeroes)
                {
                    var commands = (hero.Aliases ?? new string[0]).Concat(new []{hero.Name});
                
                    sb.AppendLine($"`{hero.Name}`");
                    sb.AppendLine($"Commands: {string.Join(", ", commands).ToLowerInvariant()}");
                    sb.AppendLine($"Builds: {hero.Builds?.Count() ?? 0}");
                    sb.AppendLine();
                }

                yield return sb.ToString();
            }
        }
        
        private static IEnumerable<IEnumerable<T>> Split<T>(IReadOnlyCollection<T> array, int size)
        {
            for (var i = 0; i < (float)array.Count / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}