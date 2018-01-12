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
        string SaveBuild(string heroName, Build build);
        void SaveScreen(string heroName, string url);
        void SaveHeroes(IEnumerable<Hero> heroes);
        void DeleteAllHeroes();
        Build ParseBuild(string buildStr);
    }
}