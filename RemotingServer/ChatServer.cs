using Proto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemotingServer
{
    internal class ChatServer : IActor
    {
        public Task ReceiveAsync(IContext context)
        {

            return Task.CompletedTask;
        }
    }
}
