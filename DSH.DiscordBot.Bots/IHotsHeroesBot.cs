using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Bots
{
    public interface IHotsHeroesBot
    {
        Hero GetHero(string name);
        Hero GetHeroByAlias(string alias);
        IEnumerable<Hero> GetHeroes();
        void DeleteHero(string name);
        void SaveAlias(string heroName, string alias);
        void SaveBuild(string heroName, Build build);
        void SaveHeroes(IEnumerable<Hero> heroes);
        void DeleteAllHeroes();
        Build ParseBuild(string buildStr);
    }
}