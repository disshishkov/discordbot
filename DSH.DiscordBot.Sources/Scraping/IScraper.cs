using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;
using HtmlAgilityPack;

namespace DSH.DiscordBot.Sources.Scraping
{
    public interface IScraper
    {
        IEnumerable<Hero> ParseHeroes(HtmlNode node, string source);
    }
}