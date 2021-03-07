using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetaPoco;
using PetaPoco.Providers;
using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ProxyGenerator = Castle.DynamicProxy.ProxyGenerator;
using System.Threading.Channels;
using System.Threading;
using Microsoft.Extensions.Hosting;
using PetaPocoClient.AkkaService;
namespace PetaPocoClient
{
    class Program
    {
        const string productconnection = "server=.,14330;Initial Catalog=fzf003;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true";

        static Channel<string> _acceptQueue = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        static async Task Reader(ChannelReader<string> channelReader,CancellationToken cancellationToken=default)
        {
           while( await channelReader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
               if( channelReader.TryRead(out string message))

                Console.WriteLine(message);
            }
        }
        static async Task Main(string[] args)
        {

            var host = new HostBuilder();
            host.UseAkkaServer();

           await host.StartAsync();
               
            Console.ReadKey();
        }

        static void PetaPocoClient()
        {
            var provider = BuildServiceCollection(cfg =>
            {
                cfg.UsingConnectionString(productconnection)
                   .UsingProvider<SqlSererMsDataDatabaseProvider>()
                   .UsingConnectionOpened(new EventHandler<DbConnectionEventArgs>((s, e) =>
                   {
                       Console.WriteLine("打开:{0}", e.Connection.State);
                   })).UsingConnectionClosing(new EventHandler<DbConnectionEventArgs>((s, e) =>
                   {
                       Console.WriteLine("关闭:{0}", e.Connection.State);
                   }))
                   .UsingCommandExecuting(new EventHandler<DbCommandEventArgs>((s, e) =>
                   {
                       Console.WriteLine("执行sql:{0}", e.Command.CommandText);
                   }))
                   .UsingTransactionStarted(new EventHandler<DbTransactionEventArgs>((s, e) => {
                       //Console.WriteLine(e.Transaction.);
                       Console.WriteLine("开始事务....");
                   }))
                   .UsingTransactionEnding(new EventHandler<DbTransactionEventArgs>((s, e) => {
                       Console.WriteLine("结束事务....");
                   }));


            }).BuildServiceProvider();

            var proxy = provider.GetService<IProxyGenerator>();

            var database = proxy.CreateInterfaceProxyWithTargetInterface(provider.GetService<ISampleService>(), new StandardInterceptor());
        }

        static IServiceCollection BuildServiceCollection(Action<IDatabaseBuildConfiguration> optionsAction)
        {
            var services = new ServiceCollection();
            services.AddLogging(c => c.SetMinimumLevel(LogLevel.Debug)
                                      .AddFilter("Microsoft", LogLevel.Information)
                                      .AddFilter("System", LogLevel.Information)
                                      .AddFilter("Program", LogLevel.Debug)
                                      .AddConsole());

            services.AddTransient<ISampleService, SampleService>();
            services.AddTransient<ILogger, ConsoleLogger>();
            var generator = new ProxyGenerator(new DefaultProxyBuilder());
            services.AddSingleton<IProxyGenerator>(generator);
            services.AddTransient<IDatabase>(sp =>
            {
                 var builder = DatabaseConfiguration.Build();

                optionsAction(builder);

                return 
                //new Database(productconnection,new SqlSererMsDataDatabaseProvider());
                builder.Create();
            });

            return services;
        }
    }

    
}
