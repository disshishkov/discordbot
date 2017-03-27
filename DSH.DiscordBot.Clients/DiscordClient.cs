using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Commands;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Infrastructure.Logging;

namespace DSH.DiscordBot.Clients
{
    public sealed class DiscordClient : IDiscordClient
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<IConfig> _config;
        private readonly Discord.DiscordClient _client;
        private readonly CommandService _commandService;

        public DiscordClient(Lazy<ILog> log, Lazy<IConfig> config)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _log = log;
            _config = config;
            _client = new Discord.DiscordClient();

            _client.UsingCommands(_ => {
                _.PrefixChar = _config.Value.CommandPrefix;
                _.HelpMode = HelpMode.Public;
            });

            _commandService = _client.GetService<CommandService>();
        }

        public async void Connect()
        {
            _log.Value.Info("DiscordClient Connect");

            await _client.Connect(_config.Value.Token, TokenType.Bot);
        }

        public async void Disconnect()
        {
            _log.Value.Info("DiscordClient Disconnect");

            await _client.Disconnect();
        }

        public void AddCommand(string name, IEnumerable<string> aliases, string answer)
        {
            _log.Value.Info("DiscordClient AddCommand: {0}", name);

            _commandService.CreateCommand(name)
                .Alias(aliases.ToArray())
                .Do(async e => { await e.Channel.SendMessage(answer); });
        }
    }
}