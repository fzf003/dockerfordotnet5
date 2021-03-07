using Microsoft.Extensions.DependencyInjection;
using Proto;
using Proto.DependencyInjection;
using Proto.Remote;
using System;
using System.Threading.Tasks;
using ProtosReflection = Proto.Remote.ProtosReflection;

namespace RemotingServer
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var provider = ConfigureServices(services =>
            {
                services.AddSingleton(serviceProvider => new ActorSystem().WithServiceProvider(serviceProvider));
                services.AddTransient<Echo>();
                services.AddTransient<IUserService, UserService>();
                services.AddTransient<ChildActor>();
            });
            var system = provider.GetService<ActorSystem>();

            var serialization = new Serialization();

            serialization.RegisterFileDescriptor(ProtosReflection.Descriptor);

            // Proto.Remote.Remoting.Start("127.0.0.1", 12001);
            //var pid = Remote.SpawnNamedAsync("127.0.0.1:12000", "remote", "hello", TimeSpan.FromSeconds(5)).Result.Pid;

            /* serialization.RegisterFileDescriptor(JsonMessage Descriptor);
             var remote = Remoting.BindService(system, serialization);
             remote.Start("127.0.0.1", 8000);*/


           





             var pid = system.Root.Spawn(system.DI().PropsFor<Echo>());

            system.EventStream.Subscribe<DeadLetterEvent>(msg => Console.WriteLine($"Sender: {msg.Sender}, Pid: {msg.Pid}, Message: {msg.Message}"));

            for (; ; )
            {
                system.Root.Send(pid, new TestMessage());
                //await system.Root.PoisonAsync(pid);
                system.Root.Send(pid, new TestMessage());

                Console.ReadKey();
            }
        }

        static IServiceProvider ConfigureServices(Action<IServiceCollection> confservices)
        {
            var services = new ServiceCollection();
            confservices(services);
            return services.BuildServiceProvider();
        }
    }

    internal class TestMessage
    {
        public TestMessage()
        {
        }
    }
}
