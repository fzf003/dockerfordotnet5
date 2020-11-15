using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using System.Buffers;
using Microsoft.Extensions.Logging;

namespace PipeChannelService.ReactiveSocket
{
    public class SocketServer : IDisposable
    {
        readonly Socket _serversocket;

        readonly Subject<SocketAcceptClient> socketobserver = new Subject<SocketAcceptClient>();

        readonly CompositeDisposable socketDisposable;

        readonly List<SocketAcceptClient> connections = new List<SocketAcceptClient>();

        readonly ILogger<SocketServer> _logger;

        readonly ILoggerFactory _loggerfactory;
        public SocketServer(IPEndPoint endPoint, ILoggerFactory loggerfactory)
        {
            this.socketDisposable = new CompositeDisposable();

            this._loggerfactory = loggerfactory;

            this._logger = loggerfactory.CreateLogger<SocketServer>();

            _serversocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };

            _serversocket.Bind(endPoint);
 
            _serversocket.Listen();

            _logger.LogInformation($"Listening on port {endPoint.Port}");

        }

        public IObservable<SocketAcceptClient> AcceptClientObservable => socketobserver.AsObservable();

        public void Start()
        {
            var disp = Observable.Defer(() => Observable.FromAsync(() => _serversocket.AcceptAsync()))
                      .Repeat()
                      .Do(PrintLog, PrintError)
                      .Subscribe(ProcessLinesAsync);

            socketDisposable.Add(disp);
        }



         void PrintLog(Socket socket)
        {
            Console.ForegroundColor = ConsoleColor.Green;
 
            _logger.LogInformation($"远程连接:{socket.RemoteEndPoint}已接入->本地:{socket.LocalEndPoint}");

            Console.ResetColor();
        }

         void PrintError(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            _logger.LogError(exception.Message);

            Console.ResetColor();
        }

        public void ProcessLinesAsync(Socket socket)
        {
            var acceptclient= new SocketAcceptClient(socket,this, _loggerfactory);

            socketobserver.OnNext(acceptclient);

            connections.Add(acceptclient);

            _logger.LogInformation("客户端:{0}已加入连接管理", socket.RemoteEndPoint);
        }

       internal void RemoveConnection(SocketAcceptClient socketclient)
        {
            _logger.LogInformation("移除前客户端:{0}",connections.Count);
             this.connections.Remove(socketclient);
            _logger.LogInformation("移除后客户端:{0}", connections.Count);
        }
 

        public void Dispose()
        {
            this.socketDisposable?.Dispose();
            this._serversocket?.Shutdown(SocketShutdown.Both);
            this._serversocket?.Close(1000);
        }
 
    }


   
    


   











}
