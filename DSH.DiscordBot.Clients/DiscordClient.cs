using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Infrastructure.Logging;

namespace DSH.DiscordBot.Clients
{
    public sealed class DiscordClient : IDiscordClient
    {
        // Discord has a 2000 lenght limit for the one message.
        private const int MaxMessageLenght = 2000;

        private readonly Lazy<ILog> _log;
        private readonly DSharpPlus.DiscordClient _client;
        private readonly CommandsNextModule _commandService;

        public DiscordClient(Lazy<ILog> log, Lazy<IConfig> config)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            var cfg = config ?? throw new ArgumentNullException(nameof(config));
            
            _client = new DSharpPlus.DiscordClient(new DiscordConfiguration()
            {
                LogLevel = LogLevel.Critical,
                TokenType = TokenType.Bot,
                Token = cfg.Value.Token
            });
            
            _commandService = _client.UseCommandsNext(new CommandsNextConfiguration()
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                StringPrefix = cfg.Value.CommandPrefix
            });
        }

        public async void Connect()
        {
            _log.Value.Info("DiscordClient Connect");

            await _client.ConnectAsync();
        }

        public async void Disconnect()
        {
            _log.Value.Info("DiscordClient Disconnect");

            await _client.DisconnectAsync();
        }

        public void AddCommand(string name, IEnumerable<string> aliases, Func<Task<string>> func)
        {
            _log.Value.Info("DiscordClient AddCommand: {0}", name);

            /*_commandService.CreateCommand(name)
                .Alias(aliases.ToArray())
                .Do(async e =>
                {
                    var answer = await func();
                    await SendMessage(answer, e);
                });*/
        }

        public void AddAdminCommand(string name, Func<string, string, Task<string>> func)
        {
            _log.Value.Info("DiscordClient AddAdminCommand: {0}", name);

            /*_commandService.CreateCommand(name)
                .AddCheck((_, user, __) => user.ToString() == _config.Value.AdminName)
                .Parameter("HeroName", ParameterType.Optional)
                .Parameter("EntityToAdd", ParameterType.Optional)
                .Do(async e =>
                {
                    var answer = await func(e.GetArg("HeroName"), e.GetArg("EntityToAdd"));
                    await SendMessage(answer, e);
                });*/
        }

        /*private static async Task SendMessage(string message, CommandEventArgs e)
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
        }*/
    }
}