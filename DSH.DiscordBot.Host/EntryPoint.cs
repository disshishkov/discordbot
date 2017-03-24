using System;
using Autofac;
using DSH.DiscordBot.Host.Service;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Infrastructure.Logging;
using Topshelf;

namespace DSH.DiscordBot.Host
{
    internal static class EntryPoint
    {
        private static void Main()
        {
            var container = EntryPoint.RegisterDependencies();

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

                        _.AfterStartingService(() => Console.WriteLine(@"DiscordBot Host started"));
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
                .SingleInstance();

            builder.RegisterType<AppSettings>()
                .As<IAppSettings>()
                .SingleInstance();

            builder.RegisterType<Config>()
                .As<IConfig>()
                .SingleInstance();

            builder.RegisterType<Service.Service>()
                .As<IService>()
                .SingleInstance();

            return builder.Build();
        }
    }
}