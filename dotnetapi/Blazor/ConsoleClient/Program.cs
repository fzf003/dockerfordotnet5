using Microsoft.Extensions.DependencyInjection;
using System;
using static GrpcService.Counter;
using Grpc.Core;
using System.Threading.Tasks;
using Grpc.Net.Client;
using System.Net.Http;
using System.Web;
using System.Threading;

namespace ConsoleClient
{
    class Program
    {
        static string token = string.Empty;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            token =
                //"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZnpmMDAzIiwiZXhwIjoxNjEwMTgwNTA0LCJpc3MiOiJFeGFtcGxlU2VydmVyIiwiYXVkIjoiRXhhbXBsZUNsaWVudHMifQ.KVXwrEwAE0vKz_lXCN5jNCmmXKnd1Ku1H5Yf4NF3n7k";
                await Authenticate();

            /* var services = new ServiceCollection();
             services.AddGrpcClient<CounterClient>(p =>
             {


                 p.Address = new Uri("https://localhost:8800");
             });

             var provider = services.BuildServiceProvider();
             */


          using var channel=  CreateAuthenticatedChannel(_token: token);

           var client = new CounterClient(channel);
                
 

            for (; ; )
            {
                var user = await client.QueryByUserIdAsync(new GrpcService.CounterRequest());
                Console.WriteLine(user.End);
                /*var response = client.StartCounter(new GrpcService.CounterRequest()
                {
                    Start = 9
                });
                await foreach (var item in response.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(item.End);
                }*/
                Console.ReadKey();
            }
        }



        static GrpcChannel CreateAuthenticatedChannel(string _token ,string address="https://localhost:8800")
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(_token))
                {
                    metadata.Add("Authorization", $"Bearer {_token}");
                }
                return Task.CompletedTask;
            });

            // SslCredentials is used here because this channel is using TLS.
            // Channels that aren't using TLS should use ChannelCredentials.Insecure instead.
            var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });
            return channel;
        }

        private static async Task<string> Authenticate()
        {
            Console.WriteLine($"Authenticating as {Environment.UserName}...");
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"https://localhost:8800/token?name={HttpUtility.UrlEncode(Environment.UserName)}"),
                Method = HttpMethod.Get,
                Version = new Version(2, 0)
            };
            var tokenResponse = await httpClient.SendAsync(request);
            tokenResponse.EnsureSuccessStatusCode();

            var token = await tokenResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Successfully authenticated.");

            return token;
        }
    }
}
