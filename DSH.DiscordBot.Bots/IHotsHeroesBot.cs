using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Bots
{
    public interface IHotsHeroesBot
    {
        Hero GetHero(string name);
        IEnumerable<Hero> GetHeroes();
    }
}