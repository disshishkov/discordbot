using System;
using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Logging;
using HtmlAgilityPack;

namespace DSH.DiscordBot.Sources.Scraping
{
    public sealed class HappyzergScraper : IScraper
    {
        private readonly Lazy<ILog> _log;
        
        public HappyzergScraper(Lazy<ILog> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IEnumerable<Hero> ParseHeroes(HtmlNode node, string source)
        {
            var heroes = new List<Hero>();
            
            var trNodes = node?.SelectNodes("//table/tbody/tr");

            if (trNodes == null)
                return heroes;

            foreach (var trNode in trNodes)
            {
                var name = GetHeroName(trNode);
                var builds = GetHeroBuilds(trNode, source);

                if (!string.IsNullOrWhiteSpace(name) && builds != null)
                {
                    _log.Value.Debug("Scrapped builds for hero '{0}'", name);

                    heroes.Add(new Hero()
                    {
                        Name = name,
                        Builds = builds
                    });
                }
            }

            return heroes;
        }
        
        private static string GetHeroName(HtmlNode node)
        {
            return node.SelectSingleNode("./td[1]/p[2]")?.InnerText;
        }

        private IEnumerable<Build> GetHeroBuilds(HtmlNode node, string sourceName)
        {
            var builds = new List<Build>();
            var aNodes = node?.SelectNodes("./td[2]/ul/li/span/a")
                         ?? node?.SelectNodes("./td[2]/ul/li/strong/a")
                         ?? node?.SelectNodes("./td[2]/ul/li/span/strong/a")
                         ?? node?.SelectNodes("./td[2]/ul/li/strong/span/a")
                         ?? node?.SelectNodes("./td[2]/ul/li/a");

            if (aNodes == null)
                return null;

            foreach (var aNode in aNodes)
            {
                try
                {
                    var title = aNode?.InnerText;
                    var url = aNode?.GetAttributeValue("href", string.Empty);

                    if (!string.IsNullOrWhiteSpace(title)
                        && !string.IsNullOrWhiteSpace(url))
                    {
                        builds.Add(new Build()
                        {
                            Source = sourceName,
                            Title = title,
                            Url = new Uri(url)
                        });
                    }
                }
                catch (Exception e)
                {
                    _log.Value.Error(e);
                }
            }

            return builds.Count > 0 ? builds : null;
        }
    }
}