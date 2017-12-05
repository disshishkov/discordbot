using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Bots.Converters
{
    public sealed class HeroTextConverter : IHeroTextConverter
    {
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

        public string Convert(IEnumerable<Hero> heroes)
        {
            var enumerable = heroes as Hero[] ?? heroes?.ToArray();

            if (!(enumerable?.Any() ?? false))
                return "No one hero was added";

            StringBuilder sb = new StringBuilder();
            foreach (var hero in enumerable)
            {
                var commands = (hero.Aliases ?? new string[0]).Concat(new []{hero.Name});
                
                sb.AppendLine($"`{hero.Name}`");
                sb.AppendLine($"Commands: {string.Join(", ", commands).ToLowerInvariant()}");
                sb.AppendLine($"Builds: {hero.Builds?.Count() ?? 0}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}