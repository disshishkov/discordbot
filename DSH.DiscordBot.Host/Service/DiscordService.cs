using System;
using System.Threading.Tasks;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Bots.Converters;
using DSH.DiscordBot.Clients;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Sources;

namespace DSH.DiscordBot.Host.Service
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class DiscordService : IService
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<IDiscordClient> _discordClient;
        private readonly Lazy<IHotsHeroesBot> _hotsHeroesBot;
        private readonly Lazy<IHeroTextConverter> _heroesConverter;
        private readonly Lazy<ISource> _source;

        public DiscordService(
            Lazy<ILog> log,
            Lazy<IDiscordClient> discordClient,
            Lazy<IHotsHeroesBot> hotsHeroesBot,
            Lazy<IHeroTextConverter> heroesConverter,
            Lazy<ISource> source)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
            _hotsHeroesBot = hotsHeroesBot ?? throw new ArgumentNullException(nameof(hotsHeroesBot));
            _heroesConverter = heroesConverter ?? throw new ArgumentNullException(nameof(heroesConverter));
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public bool Start()
        {
            _log.Value.Info("Service Start");

            AddCommands();

            _discordClient.Value.Connect();

            return true;
        }

        public bool Stop()
        {
            _log.Value.Info("Service Stop");

            _discordClient.Value.Disconnect();

            return true;
        }

        private Task<string> ExecuteCommand(Action func, string successMessage)
        {
            return Task.Run(() =>
            {
                try
                {
                    func();
                }
                catch (Exception e)
                {
                    _log.Value.Error(e);

                    return $"Someting was wrong, see logs for details: {e.Message}";
                }

                return successMessage;
            });
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
                    var build = _hotsHeroesBot.Value.ParseBuild(buildStr);
                    return ExecuteCommand(
                        () => _hotsHeroesBot.Value.SaveBuild(heroName, build),
                        $"'{build.Title}' build was succesfully added to hero '{heroName}'");
                });

            _discordClient.Value.AddAdminCommand("update",
                (_, __) => ExecuteCommand(
                    () => _hotsHeroesBot.Value.SaveHeroes(_source.Value.GetHeroes()),
                    "Done!"));

            _discordClient.Value.AddCommand("list", new [] {"l", "tierlist"},
                () => Task.Run(() =>
                    _heroesConverter.Value.Convert(
                        _hotsHeroesBot.Value.GetHeroes())));

            var heroes = _hotsHeroesBot.Value.GetHeroes();
            if (heroes == null)
                return;

            foreach (var hero in heroes)
            {
                _discordClient.Value.AddCommand(hero.Name, hero.Aliases ?? new string[0],
                    () => Task.Run(() =>
                        _heroesConverter.Value.Convert(
                            _hotsHeroesBot.Value.GetHero(hero.Name))));
            }
        }
    }
}