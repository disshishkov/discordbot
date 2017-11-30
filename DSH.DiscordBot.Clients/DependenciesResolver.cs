using System;
using DSharpPlus.CommandsNext;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Bots.Converters;
using DSH.DiscordBot.Sources;

namespace DSH.DiscordBot.Clients
{
    public sealed class DependenciesResolver
    {
        private static readonly Lazy<DependenciesResolver> SingletonLazy 
            = new Lazy<DependenciesResolver>(() => new DependenciesResolver());

        private IHeroTextConverter _heroTextConverter;
        private ISource _source;
        private IHotsHeroesBot _hotsHeroesBot;

        public static void Set(
            IHeroTextConverter heroTextConverter,
            ISource source,
            IHotsHeroesBot hotsHeroesBot)
        {
            SingletonLazy.Value._heroTextConverter = heroTextConverter;
            SingletonLazy.Value._source = source;
            SingletonLazy.Value._hotsHeroesBot = hotsHeroesBot;
        }

        public static DependencyCollection GetDependencies()
        {
            return new DependencyCollectionBuilder()
                .AddInstance(new Lazy<IHeroTextConverter>(() => SingletonLazy.Value._heroTextConverter))
                .AddInstance(new Lazy<ISource>(() => SingletonLazy.Value._source))
                .AddInstance(new Lazy<IHotsHeroesBot>(() => SingletonLazy.Value._hotsHeroesBot))
                .Build();
        }
    }
}