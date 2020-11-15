using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;

namespace PipeChannelService.ReactiveSocket
{
    public static class PipeLineExtensions
    {
        public static async Task SendAsync(this PipeWriter writer, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        }


        public static async Task SendAsync(this PipeWriter writer, byte[] buffer, CancellationToken cancellationToken = default)
        {
            await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
         
        }

    }
}
