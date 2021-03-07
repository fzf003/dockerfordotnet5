using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BedrockServer
{
    public class PrintConnectionHandler : ConnectionHandler
    {
        readonly ILogger<PrintConnectionHandler> _log;

        readonly CancellationToken cancellationToken;

        public PrintConnectionHandler(ILogger<PrintConnectionHandler> log)
        {
            _log = log;
            this.cancellationToken = new CancellationToken();
        }

        public override  async Task OnConnectedAsync(ConnectionContext connection)
        {
            var reader = connection.Transport.Input;
            var writer = connection.Transport.Output;
           
                _log.LogDebug("Receive connection on " + connection.ConnectionId);

              /*  Observable.Defer(() => Observable.FromAsync(() => reader.ReadAsync().AsTask()))
                           .Repeat()
                           .TakeWhile(p => p.IsCompleted)
                           .Select(p => Observable.FromAsync(() => WriteMessage(writer, reader, p.Buffer)))
                           .Finally(() => {
                               reader.Complete();
                               writer.Complete();
                           }).Subscribe();*/
            
               
                  try {         

                while (!cancellationToken.IsCancellationRequested)
                {
                    ReadResult result = await reader.ReadAsync();
                    ReadOnlySequence<byte> buffer = result.Buffer;

                    _log.LogDebug("Receiving data on " + connection.ConnectionId);

                   await WriteMessage(writer, reader, buffer);

                    if (result.IsCompleted)
                    {
                        _log.LogDebug("result.IsCompleted " + connection.ConnectionId);
                        break;
                    }

                    
                }
            }
            finally
            {
                reader.Complete();
                writer.Complete();
                _log.LogDebug($"Connection {connection.ConnectionId} disconnected");
            }
        }

        async Task WriteMessage(PipeWriter pipeWriter,PipeReader pipeReader,ReadOnlySequence<byte> buffer)
        {
            foreach (ReadOnlyMemory<byte> x in buffer)
            {
                await pipeWriter.WriteAsync(x);
            }

            pipeReader.AdvanceTo(buffer.End);
        }
    }
}