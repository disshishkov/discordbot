using System;
using System.Collections.Generic;
using Autofac.Features.Indexed;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Infrastructure.Serialization;
using DSH.DiscordBot.Infrastructure.Web;
using DSH.DiscordBot.Sources.Scraping;
using HtmlAgilityPack;

namespace DSH.DiscordBot.Sources
{
    public sealed class ScrapingSource : ISource
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<IConfig> _config;
        private readonly Lazy<ISerializer> _serializer;
        private readonly Lazy<IClient> _client;
        private readonly IIndex<string, IScraper> _scraperFactory;

        public ScrapingSource(
            Lazy<ILog> log,
            Lazy<IConfig> config,
            Lazy<ISerializer> serializer,
            Lazy<IClient> client,
            IIndex<string, IScraper> scraperFactory)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _scraperFactory = scraperFactory ?? throw new ArgumentNullException(nameof(scraperFactory));
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
                if (url == null)
                    continue;

                _log.Value.Info("Get heroes from {0}", url);

                var html = new HtmlDocument();
                html.LoadHtml(_client.Value.GetString(url.ToString()).Result);

                var source = url.Host.ToUpperInvariant();
                
                heroes.AddRange(_scraperFactory[source].ParseHeroes(html.DocumentNode, source));
            }

            _log.Value.Debug(
                "Scrapped heroes:{1}{0}",
                _serializer.Value.Serialize(heroes),
                Environment.NewLine);

            return heroes;
        }
    }
}