using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;

namespace DSH.DiscordBot.Sources
{
    public interface ISource
    {
        IEnumerable<Hero> GetHeroes();
    }
}