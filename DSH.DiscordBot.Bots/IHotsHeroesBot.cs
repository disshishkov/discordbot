using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Bots
{
    public interface IHotsHeroesBot
    {
        Hero GetHero(string name);
        IEnumerable<Hero> GetHeroes();
        void SaveAlias(string heroName, string alias);
        void SaveBuild(string heroName, Build build);
    }
}