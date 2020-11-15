using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using static EchoService.Greeter;
using System.Linq;
using Polly;


namespace echoclient
{
    class Program
    {
        static void Main(string[] args)
        {
            var policy = Policy.Handle<Exception>()
                               .WaitAndRetryForeverAsync(retryAttempt =>
                                   TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                               );

            var client = GetGreeterClient();

            var grpccallrequest = Observable.FromAsync(() => ToGrpcRequest(client, policy, true))
                                            .Do(PrintLog, PrintError)
                                            .Repeat()
                                            .Catch<EchoService.HelloReply, Exception>(ex =>
                                            {
                                                PrintError(ex);
                                                return Observable.Empty<EchoService.HelloReply>();
                                            });

            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                      .Select(p => grpccallrequest)
                      .Concat()
                      .Do(PrintLog, PrintError)
                      .Subscribe();

            Console.ReadKey();
        }

        static Task<EchoService.HelloReply> ToGrpcRequest(GreeterClient client, Polly.Retry.AsyncRetryPolicy retryPolicy, bool isbackoff = true)
        {
            if (isbackoff)
            {
                return retryPolicy.ExecuteAsync(() => client.SayHelloAsync(new EchoService.HelloRequest { Name = DateTime.Now.ToString() }).ResponseAsync);
            }

            return client.SayHelloAsync(new EchoService.HelloRequest { Name = DateTime.Now.ToString() }).ResponseAsync;
        }

        static void PrintLog(EchoService.HelloReply reply)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(reply.Message);

            Console.ResetColor();
        }

        static void PrintError(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(exception.Message);

            Console.ResetColor();
        }

        static GreeterClient GetGreeterClient(string url = "https://localhost:6061", string pass = "fzf0031")
        {
            var rootpath = System.Environment.GetEnvironmentVariable("USERPROFILE");
            var certfile = Path.Combine($@"{rootpath}\.aspnet\https\EchoService.pfx");
            var cert = new X509Certificate2(certfile, pass);
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);
            return new GreeterClient(GrpcChannel.ForAddress(url, new GrpcChannelOptions
            {
                HttpClient = new HttpClient(handler)
            }));
        }
    }
  

}
