using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Server
{


    public interface IUserClient
    {
        Task AddUser(string username);
    }


    public class ChatHub : Hub<IUserClient>
    {
         readonly ILogger<ChatHub> _logger;
        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"ConnectionId:{Context.ConnectionId} On line!!");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
             _logger.LogInformation($"ConnectionId:{Context.ConnectionId} Off line!!");
            return base.OnDisconnectedAsync(exception);
        }

        public Task OnAddUser(string username)
        {
            this.Clients.All.AddUser($"fzf-{username}");
            _logger.LogInformation($"新用户:{username}已加入!!!");
            return Task.CompletedTask;
        }

    }
}