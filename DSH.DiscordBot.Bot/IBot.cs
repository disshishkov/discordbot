using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Bot
{
    public interface IBot
    {
        Hero GetHero(string name);
        IEnumerable<Hero> GetHeroes();
    }
}