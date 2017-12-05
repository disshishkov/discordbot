using System;
using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Logging;
using HtmlAgilityPack;

namespace DSH.DiscordBot.Sources.Scraping
{
    public sealed class RobogrubScraper : IScraper
    {
        private readonly Lazy<ILog> _log;
        
        public RobogrubScraper(Lazy<ILog> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }
        
        public IEnumerable<Hero> ParseHeroes(HtmlNode node, string source)
        {
            var heroes = new List<Hero>();
            
            var heroesNodes = node?.SelectNodes("//div[@id='builds']/div[@id='heroeslist']/a");
            var buildsNode = node?.SelectSingleNode("//div[@id='builds']/div[@id='buildslist']");

            if (heroesNodes == null || buildsNode == null)
                return heroes;

            foreach (var heroesNode in heroesNodes)
            {
                var name = GetHeroName(heroesNode);
                var imageUrl = GetHeroImageUrl(heroesNode);
                var dataId = GetDataId(heroesNode);
                var builds = GetHeroBuilds(buildsNode, dataId, source);
                
                if (!string.IsNullOrWhiteSpace(name) && builds != null)
                {
                    _log.Value.Debug("Scrapped builds for hero '{0}'", name);

                    heroes.Add(new Hero()
                    {
                        Name = name,
                        ImageUrl = imageUrl,
                        Builds = builds
                    });
                }
            }

            return heroes;
        }
        
        private static string GetHeroName(HtmlNode node)
        {
            return node.SelectSingleNode("./h2[1]")?.InnerText;
        }
        
        private static string GetDataId(HtmlNode node)
        {
            return node.SelectSingleNode("./img[1]")?.GetAttributeValue("data-id", string.Empty);
        }
        
        private Uri GetHeroImageUrl(HtmlNode node)
        {
            try
            {
                var url = node.SelectSingleNode("./img[1]")?.GetAttributeValue("src", string.Empty);
                return !string.IsNullOrWhiteSpace(url) ? new Uri(url) : null;
            }
            catch (Exception e)
            {
                _log.Value.Error(e);
                return null;
            }
        }

        private IEnumerable<Build> GetHeroBuilds(HtmlNode node, string dataId, string sourceName)
        {
            if (string.IsNullOrWhiteSpace(dataId))
                return null;
         
            var buildNames = node?.SelectNodes($"./div[@data-id='{dataId}']/a");
            if (buildNames == null)
                return null;
            
            var builds = new List<Build>();

            foreach (var buildName in buildNames)
            {
                try
                {
                    var title = buildName?.InnerText;
                    var buildDataId = buildName?.GetAttributeValue("data-id", string.Empty);

                    if (!string.IsNullOrWhiteSpace(title)
                        && !string.IsNullOrWhiteSpace(buildDataId))
                    {
                        var url = node.SelectSingleNode($"./div[@class='desclist']/p[@data-id='{buildDataId}']a[1]")
                            ?.GetAttributeValue("href", string.Empty);
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            builds.Add(new Build()
                            {
                                Source = sourceName,
                                Title = title,
                                Url = new Uri(url)
                            });
                        }
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