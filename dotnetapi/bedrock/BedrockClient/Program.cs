using Microsoft.AspNetCore.Connections;
using System;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BedrockClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int port = 8087;
            Console.WriteLine($"Connecting to port {port}");
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            using var clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await clientSocket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, port));
            
            try
            {
                var stream = new NetworkStream(clientSocket);

                Console.Write("Ready for your input: ");
                var input = Console.ReadLine();
                PipeReader reader = PipeReader.Create(stream);
               
                while (true)
                {
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(input);
                    await stream.WriteAsync(data, 0, data.Length);

                    data = new Byte[256];
                    // Read the Tcp Server Response Bytes.
                    int bytes = stream.Read(data, 0, data.Length);
                    var response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    
                    Console.WriteLine("Received: {0}", response);
                    Console.Write("Ready for your input: ");
                    input = Guid.NewGuid().ToString("N");

                    Console.ReadKey();
                }
            }
            finally
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }
}
