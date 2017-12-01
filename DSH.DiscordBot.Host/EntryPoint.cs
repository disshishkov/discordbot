using System;
using Autofac;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Bots.Converters;
using DSH.DiscordBot.Clients;
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
            DependenciesResolver.Set(
                container.Resolve<IHeroTextConverter>(),
                container.Resolve<ISource>(),
                container.Resolve<IHotsHeroesBot>());

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

            builder.RegisterType<ScrapingSource>()
                .As<ISource>()
                .InstancePerLifetimeScope();

            builder.RegisterType<HttpClient>()
                .As<IClient>()
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