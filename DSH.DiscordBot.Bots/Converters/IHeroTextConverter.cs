using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Bots.Converters
{
    public interface IHeroTextConverter
    {
        string Convert(Hero hero);
        string Convert(IEnumerable<Hero> heroes);
    }
}