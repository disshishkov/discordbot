using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Infrastructure.Logging;

namespace DSH.DiscordBot.Clients
{
    public sealed class DiscordClient : IDiscordClient
    {
        // Discord has a 2000 lenght limit for the one message.
        private const int MaxMessageLenght = 2000;

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
                _.HelpMode = HelpMode.Disabled;
            });

            _commandService = _client.GetService<CommandService>();
        }

        public void Connect()
        {
            _log.Value.Info("DiscordClient Connect");

            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(_config.Value.Token, TokenType.Bot);
            });
        }

        public void Disconnect()
        {
            _log.Value.Info("DiscordClient Disconnect");

            _client.ExecuteAndWait(async () =>
            {
                await _client.Disconnect();
            });
        }

        public void AddCommand(string name, IEnumerable<string> aliases, Func<Task<string>> func)
        {
            _log.Value.Info("DiscordClient AddCommand: {0}", name);

            _commandService.CreateCommand(name)
                .Alias(aliases.ToArray())
                .Do(async e =>
                {
                    var answer = await func();
                    await SendMessage(answer, e);
                });
        }

        public void AddAdminCommand(string name, Func<string, string, Task<string>> func)
        {
            _log.Value.Info("DiscordClient AddAdminCommand: {0}", name);

            _commandService.CreateCommand(name)
                .AddCheck((_, user, __) => user.ToString() == _config.Value.AdminName)
                .Parameter("HeroName", ParameterType.Optional)
                .Parameter("EntityToAdd", ParameterType.Optional)
                .Do(async e =>
                {
                    var answer = await func(e.GetArg("HeroName"), e.GetArg("EntityToAdd"));
                    await SendMessage(answer, e);
                });
        }

        private static async Task SendMessage(string message, CommandEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            if (message.Length <= MaxMessageLenght)
            {
                await e.Channel.SendMessage(message);
            }
            else
            {
                foreach (var msg in ChunksUpto(message, MaxMessageLenght))
                {
                    await e.Channel.SendMessage(msg);
                }
            }
        }

        private static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length-i));
        }
    }
}