using System;
using System.Threading.Tasks;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Bots.Converters;
using DSH.DiscordBot.Clients;
using DSH.DiscordBot.Contract.Dto;
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

        private static Task<string> ExecuteCommand(Action func, string successMessage)
        {
            return Task.Run(() =>
            {
                try
                {
                    func();
                }
                catch (Exception e)
                {
                    return $"Someting was wrong, see logs for details: {e.Message}";
                }

                return successMessage;
            });
        }

        private static Build ParseBuild(string buildStr)
        {
            var build = new Build();
            build.Title = "Default";

            if (string.IsNullOrWhiteSpace(buildStr))
                return build;

            var parts = buildStr.Split('|');

            // Just url is presented
            if (parts.Length == 1)
            {
                build.Url = new Uri(parts[0].ToLowerInvariant());
            }

            // Title and urls are presented
            if (parts.Length == 2)
            {
                build.Title = parts[0];
                build.Url = new Uri(parts[1].ToLowerInvariant());
            }

            return build;
        }

        private void AddCommands()
        {
            _discordClient.Value.AddAdminCommand("ку",
                (_, __) => Task.Run(() => "Ку!"));

            _discordClient.Value.AddAdminCommand("add_alias",
                (heroName, alias) => ExecuteCommand(
                    () => _hotsHeroesBot.Value.SaveAlias(heroName, alias),
                    $"'{alias}' alias was succesfully added to hero '{heroName}'"));

            _discordClient.Value.AddAdminCommand("add_build",
                (heroName, buildStr) =>
                {
                    var build = ParseBuild(buildStr);
                    return ExecuteCommand(
                        () => _hotsHeroesBot.Value.SaveBuild(heroName, build),
                        $"'{build.Title}' build was succesfully added to hero '{heroName}'");
                });

            _discordClient.Value.AddAdminCommand("update",
                (_, __) =>
                {
                    return Task.Run(() => "Done!");
                });

            var heroes = _hotsHeroesBot.Value.GetHeroes();

            _discordClient.Value.AddCommand("list", new [] {"l", "tierlist"},
                () => Task.Run(() => _heroesConverter.Value.Convert(heroes)));

            if (heroes == null)
                return;

            foreach (var hero in heroes)
            {
                var heroDetails = _hotsHeroesBot.Value.GetHero(hero.Name);

                _discordClient.Value.AddCommand(hero.Name, hero.Aliases,
                    () => Task.Run(() => _heroesConverter.Value.Convert(heroDetails)));
            }
        }
    }
}