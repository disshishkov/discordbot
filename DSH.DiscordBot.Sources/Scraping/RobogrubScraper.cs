using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;
using HtmlAgilityPack;

namespace DSH.DiscordBot.Sources.Scraping
{
    public sealed class RobogrubScraper : IScraper
    {
        public IEnumerable<Hero> ParseHeroes(HtmlNode node, string source)
        {
            return new List<Hero>();
        }
    }
}