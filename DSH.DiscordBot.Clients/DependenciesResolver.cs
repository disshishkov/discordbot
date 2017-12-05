using System;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Bots.Converters;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Sources;

namespace DSH.DiscordBot.Clients
{
    public sealed class DependenciesResolver
    {
        private static readonly Lazy<DependenciesResolver> SingletonLazy 
            = new Lazy<DependenciesResolver>(() => new DependenciesResolver());

        private IHeroTextConverter _heroTextConverter;
        private IDictionary<SourceType, ISource> _source;
        private IHotsHeroesBot _hotsHeroesBot;
        private IConfig _config;

        public static void Set(
            IHeroTextConverter heroTextConverter,
            IDictionary<SourceType, ISource> source,
            IHotsHeroesBot hotsHeroesBot,
            IConfig config)
        {
            SingletonLazy.Value._heroTextConverter = heroTextConverter;
            SingletonLazy.Value._source = source;
            SingletonLazy.Value._hotsHeroesBot = hotsHeroesBot;
            SingletonLazy.Value._config = config;
        }

        public static DependencyCollection GetDependencies()
        {
            return new DependencyCollectionBuilder()
                .AddInstance(new Lazy<IHeroTextConverter>(() => SingletonLazy.Value._heroTextConverter))
                .AddInstance(new Lazy<IDictionary<SourceType, ISource>>(() => SingletonLazy.Value._source))
                .AddInstance(new Lazy<IHotsHeroesBot>(() => SingletonLazy.Value._hotsHeroesBot))
                .AddInstance(new Lazy<IConfig>(() => SingletonLazy.Value._config))
                .Build();
        }
    }
}