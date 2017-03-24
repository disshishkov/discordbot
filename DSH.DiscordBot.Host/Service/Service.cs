using System;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Infrastructure.Logging;

namespace DSH.DiscordBot.Host.Service
{
    public sealed class Service : IService
    {
        private readonly Lazy<ILog> _log;
        private readonly Lazy<IConfig> _config;

        public Service(Lazy<ILog> log, Lazy<IConfig> config)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _log = log;
            _config = config;
        }

        public void Start()
        {
            _log.Value.Trace("Bot start with token {0}", _config.Value.Token);
        }

        public void Stop()
        {
            _log.Value.Trace("Bot was stopped");
        }
    }
}