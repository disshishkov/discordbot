using System;
using DSH.DiscordBot.Clients;
using DSH.DiscordBot.Infrastructure.Logging;

namespace DSH.DiscordBot.Host.Service
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class DiscordService : IService
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<IDiscordClient> _discordClient;

        public DiscordService(Lazy<ILog> log, Lazy<IDiscordClient> discordClient)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
        }

        public bool Start()
        {
            _log.Value.Info("Service Start");
            
            _discordClient.Value.Connect();

            return true;
        }

        public bool Stop()
        {
            _log.Value.Info("Service Stop");
            
            _discordClient.Value.Disconnect();

            return true;
        }
    }
}