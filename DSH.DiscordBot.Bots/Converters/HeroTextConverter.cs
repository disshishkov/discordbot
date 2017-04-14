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
                return "А героя та нема";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{hero.Name}");

            if (!(hero.Builds?.Any() ?? false))
            {
                sb.AppendLine("Билдов нема");
            }
            else
            {
                foreach (var build in hero.Builds)
                {
                    sb.AppendLine($"{build.Title} - {build.Url}");
                }
            }

            return sb.ToString();
        }

        public string Convert(IEnumerable<Hero> heroes)
        {
            var enumerable = heroes as Hero[] ?? heroes.ToArray();

            if (!enumerable.Any())
                return "А героев та нема";

            StringBuilder sb = new StringBuilder();
            foreach (var hero in enumerable)
            {
                sb.AppendLine($"{hero.Name}");
                sb.AppendLine($"Commands: {hero.Name.ToLowerInvariant()}, {string.Join(", ", hero.Aliases ?? new string[0]).ToLowerInvariant()}");
                sb.AppendLine($"Builds: {hero.Builds?.Count() ?? 0}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}