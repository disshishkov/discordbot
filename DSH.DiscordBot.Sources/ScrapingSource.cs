using System;
using System.Collections.Generic;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Infrastructure.Serialization;
using DSH.DiscordBot.Infrastructure.Web;
using HtmlAgilityPack;

namespace DSH.DiscordBot.Sources
{
    public sealed class ScrapingSource : ISource
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<IConfig> _config;
        private readonly Lazy<ISerializer> _serializer;
        private readonly Lazy<IClient> _client;

        public ScrapingSource(
            Lazy<ILog> log,
            Lazy<IConfig> config,
            Lazy<ISerializer> serializer,
            Lazy<IClient> client)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            _log = log;
            _config = config;
            _serializer = serializer;
            _client = client;
        }

        public IEnumerable<Hero> GetHeroes()
        {
            var heroes = new List<Hero>();

            _log.Value.Info("Starting get heroes from the web sources");

            var sourceUrls = _config.Value.Sources;

            if (sourceUrls == null)
                return heroes;

            foreach (var url in sourceUrls)
            {
                _log.Value.Info("Get heroes from {0}", url);

                var html = new HtmlDocument();
                html.LoadHtml(_client.Value.GetString(url).Result);

                var trNodes = html.DocumentNode?.SelectNodes("//table/tbody/tr");

                if (trNodes == null)
                    continue;

                foreach (var trNode in trNodes)
                {
                    var name = GetHeroName(trNode);
                    var builds = GetHeroBuilds(trNode, url);

                    if (!string.IsNullOrWhiteSpace(name)
                        && builds != null)
                    {
                        _log.Value.Debug("Scrapped builds for hero '{0}'", name);

                        heroes.Add(new Hero()
                        {
                            Name = name,
                            Builds = builds
                        });
                    }
                }
            }

            _log.Value.Debug(
                "Scrapped heroes:{1}{0}",
                _serializer.Value.Serialize(heroes),
                Environment.NewLine);

            return heroes;
        }

        private static string GetHeroName(HtmlNode node)
        {
            return node.SelectSingleNode("./td[1]/p[2]")?.InnerText;
        }

        private IEnumerable<Build> GetHeroBuilds(HtmlNode node, string sourceName)
        {
            var builds = new List<Build>();
            var aNodes = node?.SelectNodes("./td[2]/ul/li/a");

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