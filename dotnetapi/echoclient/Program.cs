using EchoService;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using static EchoService.Greeter;
namespace echoclient
{
    class Program
    {
        static void Main(string[] args)
        {
            
                 var client =  GetGreeterClient();

                for (; ; )
                {
                   var repy= client.SayHello(new HelloRequest
                    {
                        Name = Guid.NewGuid().ToString("N")
                    });


                    Console.WriteLine(repy.Message);


                    Console.ReadKey();
                }
            }

            
        

        static GreeterClient GetGreeterClient(string url="https://localhost:6061",string pass="fzf0031")
        {
            var rootpath=System.Environment.GetEnvironmentVariable("USERPROFILE");
            var certfile= Path.Combine($@"{rootpath}\.aspnet\https\EchoService.pfx");
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
