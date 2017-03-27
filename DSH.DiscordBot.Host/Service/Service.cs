using System;
using System.Linq;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Bots.Converters;
using DSH.DiscordBot.Clients;
using DSH.DiscordBot.Infrastructure.Logging;

namespace DSH.DiscordBot.Host.Service
{
    public sealed class Service : IService
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<IDiscordClient> _discordClient;
        private readonly Lazy<IHotsHeroesBot> _hotsHeroesBot;
        private readonly Lazy<IHeroTextConverter> _heroesConverter;

        public Service(
            Lazy<ILog> log,
            Lazy<IDiscordClient> discordClient,
            Lazy<IHotsHeroesBot> hotsHeroesBot,
            Lazy<IHeroTextConverter> heroesConverter)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (discordClient == null)
                throw new ArgumentNullException(nameof(discordClient));
            if (hotsHeroesBot == null)
                throw new ArgumentNullException(nameof(hotsHeroesBot));
            if (heroesConverter == null)
                throw new ArgumentNullException(nameof(heroesConverter));

            _log = log;
            _discordClient = discordClient;
            _hotsHeroesBot = hotsHeroesBot;
            _heroesConverter = heroesConverter;
        }

        public void Start()
        {
            _log.Value.Info("Service Start");

            AddCommands();

            _discordClient.Value.Connect();
        }

        public void Stop()
        {
            _log.Value.Info("Service Stop");

            _discordClient.Value.Disconnect();
        }

        private void AddCommands()
        {
            var heroes = _hotsHeroesBot.Value.GetHeroes().ToArray();

            _discordClient.Value.AddCommand(
                "list",
                new [] {"l", "tierlist"},
                _heroesConverter.Value.Convert(heroes));

            foreach (var hero in heroes)
            {
                _discordClient.Value.AddCommand(
                    hero.Name,
                    hero.Aliases,
                    _heroesConverter.Value.Convert(hero));
            }
        }
    }
}