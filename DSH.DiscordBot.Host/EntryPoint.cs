using System;
using System.Collections.Generic;
using Autofac;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Bots.Converters;
using DSH.DiscordBot.Clients;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Host.Service;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Infrastructure.Serialization;
using DSH.DiscordBot.Infrastructure.Web;
using DSH.DiscordBot.Sources;
using DSH.DiscordBot.Sources.Scraping;
using DSH.DiscordBot.Storage;
using Topshelf;

namespace DSH.DiscordBot.Host
{
    internal static class EntryPoint
    {
        private static void Main()
        {
            var container = EntryPoint.RegisterDependencies();

            SetDiscordSharpDependencies(container);
            
            var log = container.Resolve<ILog>();

            Console.WriteLine("DiscordBot starting");

            try
            {
                HostFactory.Run(config =>
                {
                    config.Service<IService>(_ =>
                    {
                        _.ConstructUsing(host => container.Resolve<IService>());
                        _.WhenStarted(host => host.Start());
                        _.WhenStopped(host => host.Stop());

                        _.AfterStartingService(() => log.Info("DiscordBot Host started"));
                    });
                    config.StartAutomatically();
                    config.SetServiceName("DSH_DiscordBot_Host_System_Service");
                    config.SetDisplayName("DiscordBot Host System Service");
                    config.RunAsNetworkService();
                });
            }
            catch (Exception e)
            {
                log.Fatal(e);
                throw;
            }
        }

        //TODO: fix in 4.0 I need this fucked shit becayse DSharp+ has own DI container.
        private static void SetDiscordSharpDependencies(IComponentContext container)
        {
            var sources = new Dictionary<SourceType, ISource>();
            sources.Add(SourceType.Api, container.ResolveKeyed<ISource>(SourceType.Api));
            sources.Add(SourceType.Scraping, container.ResolveKeyed<ISource>(SourceType.Scraping));
            DependenciesResolver.Set(
                container.Resolve<IHeroTextConverter>(),
                sources,
                container.Resolve<IHotsHeroesBot>(),
                container.Resolve<IConfig>());
        }

        private static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<NLogLogger>()
                .As<ILog>()
                .InstancePerLifetimeScope();

            builder.RegisterType<JsonNetSerilizer>()
                .As<ISerializer>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AppSettings>()
                .As<IAppSettings>()
                .SingleInstance();

            builder.RegisterType<Config>()
                .As<IConfig>()
                .SingleInstance();

            builder.RegisterType<DiscordService>()
                .As<IService>()
                .SingleInstance();

            builder.RegisterType<HeroTextConverter>()
                .As<IHeroTextConverter>()
                .SingleInstance();

            builder.RegisterType<LiteDbStorage>()
                .As<IStorage>()
                .InstancePerLifetimeScope();

            builder.RegisterType<HotsHeroesBot>()
                .As<IHotsHeroesBot>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DiscordClient>()
                .As<IDiscordClient>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<HttpClient>()
                .As<IClient>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<SiteScreenshoter>()
                .As<IScreenshoter>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ScrapingSource>()
                .Keyed<ISource>(SourceType.Scraping)
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ApiSource>()
                .Keyed<ISource>(SourceType.Api)
                .InstancePerLifetimeScope();

            builder.RegisterType<HappyzergScraper>()
                .Named<IScraper>("HAPPYZERG.RU")
                .InstancePerLifetimeScope();
            
            builder.RegisterType<RobogrubScraper>()
                .Named<IScraper>("WWW.ROBOGRUB.COM")
                .InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}