using Proto;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Proto.DependencyInjection;


namespace RemotingServer
{
    internal class Echo : IActor
    {
        readonly IUserService userService;

        readonly List<PID> pidstore;
        public Echo(IUserService userService)
        {
            this.userService = userService;

            this.pidstore = new List<PID>();
        }

        public Task ReceiveAsync(IContext context)
        {
            switch(context.Message)
            {
                case Started started:
                    Handler(started,context);
                    break;
                case Stopped stopped:
                    Handler(stopped);
                    break;
                case TestMessage message:
                    Handler(message, context);
                    break;
                case ReceiveTimeout receiveTimeout:
                    Console.WriteLine("receive:{0}",receiveTimeout);
                    break;
            }

            return Task.CompletedTask;
        }

        void Handler(Started started,IContext context)
        {
            context.SetReceiveTimeout(TimeSpan.FromSeconds(1));
            Console.WriteLine("Start");
            pidstore.Add(context.SpawnNamed(context.System.DI().PropsFor<ChildActor>(), $"Product-{Guid.NewGuid().ToString("N")}"));
        }

        void Handler(Stopped stopped)
        {
            Console.WriteLine("Stop");
        }

        void Handler(TestMessage message,IContext context)
        {

            this.userService.Handle(message);

            pidstore.ForEach(context.Forward);

            pidstore.ForEach(p => context.Poison(p));







        }
    }

    internal class ChildActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            switch(context.Message)
            {
                case TestMessage testMessage:
                    Console.WriteLine("text mesage");
                    break;
                case Stopped stopped:
                    Console.WriteLine("停止{0}",context.Self.Address);
                    break;
                default:
                    Console.WriteLine("停止:{0}", context.Message);
                    break;
            }

            return Task.CompletedTask;
        }
    }

    internal interface IUserService
    {
        void Handle(TestMessage message);
    }

    internal class UserService : IUserService
    {
        public void Handle(TestMessage message)
        {
            Thread.Sleep(1500);
            Console.WriteLine("user Service:{0}", message);
        }
    }
}