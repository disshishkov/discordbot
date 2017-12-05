using System;
using System.Collections.Generic;
using Autofac.Features.Indexed;
using DSH.DiscordBot.Contract.Dto;
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
        private readonly Lazy<ISerializer> _serializer;
        private readonly Lazy<IClient> _client;
        private readonly IIndex<string, IScraper> _scraperFactory;

        public ScrapingSource(
            Lazy<ILog> log,
            Lazy<ISerializer> serializer,
            Lazy<IClient> client,
            IIndex<string, IScraper> scraperFactory)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _scraperFactory = scraperFactory ?? throw new ArgumentNullException(nameof(scraperFactory));
        }

        public IEnumerable<Hero> GetHeroes(IEnumerable<Source> sources)
        {
            var heroes = new List<Hero>();

            _log.Value.Info("Starting get heroes from the web sources");

            if (sources == null)
                return heroes;

            foreach (var source in sources)
            {
                if (source?.Type != SourceType.Scraping)
                    continue;
                
                var url = source.Url;
                if (url == null)
                    continue;

                _log.Value.Info("Get heroes from {0}", url);

                var html = new HtmlDocument();
                html.LoadHtml(_client.Value.GetString(url.ToString()).Result);

                var sourceName = url.Host.ToUpperInvariant();
                
                heroes.AddRange(_scraperFactory[sourceName].ParseHeroes(html.DocumentNode, sourceName));
            }

            _log.Value.Debug(
                "Scrapped heroes:{1}{0}",
                _serializer.Value.Serialize(heroes),
                Environment.NewLine);

            return heroes;
        }
    }
}