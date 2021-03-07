using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserServiceController : ControllerBase
    {
         private readonly ILogger<UserServiceController> _logger;

        readonly HubConnection _hubConnection;

        public UserServiceController(ILogger<UserServiceController> logger, HubConnection hubConnection)
        {
            _logger = logger;

            this._hubConnection = hubConnection;
        }

        [HttpGet]
        public Task  Publish(string message)
        {
            this._hubConnection.InvokeAsync("OnAddUser",message);
             return Task.CompletedTask;
        }
    }
}
