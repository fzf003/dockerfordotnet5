using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Polly;
using Microsoft.Extensions.DependencyInjection;
using static Greet.Greeter;
using Greet;
using Echotell;
using Grpc.Core;

namespace echoclient
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var serviceprovider = ConfigServices(services =>
            {
                services.CreateGrpcClient();
            });

            var tellerservice= serviceprovider.GetService<Teller.TellerClient>();

             CancellationTokenSource cancellation = new CancellationTokenSource();

            var metadata = new Metadata();
            metadata.Add(key: "title", value: "fzf003");

            var userchangestream = tellerservice.TellResponse(new UserRequest
            {
                Age = 0,
                Name = string.Empty
            },metadata);

            //cancellation.CancelAfter(1000 * 20);

            try
            {

                await foreach (var item in userchangestream.ResponseStream.ReadAllAsync(cancellation.Token))
                {
                    Console.WriteLine(item.Body);
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("执行完毕.....");
            }

            Console.WriteLine(".............");
            Console.ReadKey();
        }

        static void RequestCall(IServiceProvider serviceprovider)
        {
            var client = serviceprovider.GetService<GreeterClient>();

            var policy = Policy.Handle<Exception>()
                             .WaitAndRetryForeverAsync(retryAttempt =>
                                 TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                             );

            var grpccallrequest = Observable.FromAsync(() => ToGrpcRequest(client, policy, true))
                                            .Do(PrintLog, PrintError)
                                            .Catch<HelloReply, Exception>(ex =>
                                            {
                                                PrintError(ex);
                                                return Observable.Empty<HelloReply>();
                                            });

            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                      .Select(p => grpccallrequest)
                      .Concat()
                      .Do(PrintLog, PrintError)
                      .Subscribe();
        }

        static Task<HelloReply> ToGrpcRequest(GreeterClient client, Polly.Retry.AsyncRetryPolicy retryPolicy, bool isbackoff = true)
        {
            if (isbackoff)
            {
                return retryPolicy.ExecuteAsync(() => client.SayHelloAsync(new HelloRequest { Name = DateTime.Now.ToString() }).ResponseAsync);
            }

            return client.SayHelloAsync(new HelloRequest { Name = DateTime.Now.ToString() }).ResponseAsync;
        }

        static void PrintLog(HelloReply reply)
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

        static IServiceProvider ConfigServices(Action<IServiceCollection> serviceaction)
        {
            var services = new ServiceCollection();
            serviceaction(services);
            return services.BuildServiceProvider();
        }

        static GreeterClient GetGreeterClient(string url = "https://localhost:5001", string pass = "fzf0031")
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

    public static class GrpcClientExtensions
    {
        public static IServiceCollection CreateGrpcClient(this IServiceCollection services, string url = "https://localhost:6001", string pass = "fzf0031")
        {
            services.AddGrpcClient<GreeterClient>(options =>
          {
              options.Address = new Uri(url);

          })
          .ConfigurePrimaryHttpMessageHandler(() =>
          {
              var rootpath = System.Environment.GetEnvironmentVariable("USERPROFILE");
              var certfile = Path.Combine($@"{rootpath}\.aspnet\https\EchoService.pfx");
              var cert = new X509Certificate2(certfile, pass);
              var handler = new HttpClientHandler();
              handler.ClientCertificates.Add(cert);
              return handler;
          });

                    services.AddGrpcClient<Teller.TellerClient>(options =>
                    {

                        options.Address = new Uri(url);

                    })
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        var rootpath = System.Environment.GetEnvironmentVariable("USERPROFILE");
                        var certfile = Path.Combine($@"{rootpath}\.aspnet\https\EchoService.pfx");
                        var cert = new X509Certificate2(certfile, pass);
                        var handler = new HttpClientHandler();
                        handler.ClientCertificates.Add(cert);
                        return handler;
                    });


            return services;

        }
    }


}
