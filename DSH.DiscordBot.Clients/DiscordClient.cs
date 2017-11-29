using System;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSH.DiscordBot.Clients.Commands;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Infrastructure.Logging;

namespace DSH.DiscordBot.Clients
{
    public sealed class DiscordClient : IDiscordClient
    {
        private readonly Lazy<ILog> _log;
        private readonly DSharpPlus.DiscordClient _client;

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
            
            var commandService = _client.UseCommandsNext(new CommandsNextConfiguration()
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                StringPrefix = cfg.Value.CommandPrefix
            });
            
            commandService.RegisterCommands<AdminCommands>();
            commandService.RegisterCommands<HeroesCommands>();
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
    }
}