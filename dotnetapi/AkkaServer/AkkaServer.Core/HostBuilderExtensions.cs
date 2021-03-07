using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;
using Petabridge.Cmd.Remote;
using System;
using ServiceProvider = Akka.DependencyInjection.ServiceProvider;

namespace AkkaServer.Core
{
    public static class HostBuilderExtensions
    {

        public static IHostBuilder UseAkkaServer(this IHostBuilder hostBuilder, string configfile, bool isdocker = false)
        {
            return hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddAkkaService(configfile,isdocker);
            });
        }
         static IServiceCollection AddAkkaService(this IServiceCollection services, string configfile, bool isdocker = false)
        {
            var config = HoconLoader.ParseConfig(configfile);
            if (isdocker)
                config = config.BootstrapFromDocker();
            
            services.AddSingleton(provider =>
            {
                var bootstrap = BootstrapSetup.Create().WithConfig(config);
                var di = ServiceProviderSetup.Create(provider);
                var actorSystemSetup = bootstrap.And(di);
                return ActorSystem.Create(config.GetString("akka.MaserServer"), actorSystemSetup).StartPbm();
            });

            return services;
        }

        public static ActorSystem StartPbm(this ActorSystem actorSystem)
        {
            var pbm = PetabridgeCmd.Get(actorSystem);
            pbm.RegisterCommandPalette(ClusterCommands.Instance);
            pbm.RegisterCommandPalette(RemoteCommands.Instance);
            pbm.RegisterCommandPalette(Petabridge.Cmd.Cluster.Sharding.ClusterShardingCommands.Instance);
            pbm.Start();
            return actorSystem;
        }

        public static IServiceCollection AddActorReference<TActor>(this IServiceCollection builder,string name=null) where TActor : ActorBase
        {
            builder.AddSingleton<ActorRefProvider<TActor>>(provider =>
            {
                var system = provider.GetService<ActorSystem>();
                var actorProps = ServiceProvider.For(system).Props<TActor>();
                var actorRef = new ActorRefProvider<TActor>(system.ActorOf(actorProps,name:name));
                return actorRef;
            });
            return builder;
        }


        public static IServiceCollection AddActorReference<TActor>(this IServiceCollection builder, Props actorProps,string name=null) where TActor : ActorBase
        {

            builder.AddSingleton<ActorRefProvider<TActor>>(provider =>
            {
                var system = provider.GetService<ActorSystem>();

                var actorRef = new ActorRefProvider<TActor>(system.ActorOf(actorProps,name));

                return actorRef;
            });

            return builder;
        }

        public static IServiceCollection AddActorReference<TActor>(this IServiceCollection builder, Func<IServiceProvider,Props>  actorProps, string name = null) where TActor : ActorBase
        {

            builder.AddSingleton<ActorRefProvider<TActor>>(provider =>
            {
                var system = provider.GetService<ActorSystem>();

                var actorRef = new ActorRefProvider<TActor>(system.ActorOf(actorProps(provider), name));

                return actorRef;
            });

            return builder;
        }



    }
}
