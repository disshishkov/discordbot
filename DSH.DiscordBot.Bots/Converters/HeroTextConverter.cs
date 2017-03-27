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
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{hero.Name}");

            foreach (var build in hero.Builds)
            {
                sb.AppendLine($"{build.Title} - {build.Url}");
            }

            return sb.ToString();
        }

        public string Convert(IEnumerable<Hero> heroes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var hero in heroes)
            {
                sb.AppendLine($"{hero.Name}");
                sb.AppendLine($"Commands: {hero.Name.ToLowerInvariant()}, {string.Join(", ", hero.Aliases).ToLowerInvariant()}");
                sb.AppendLine($"Builds: {hero.Builds.Count()}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}