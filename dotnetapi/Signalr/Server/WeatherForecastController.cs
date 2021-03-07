using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Server
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        readonly IHubContext<ChatHub, IUserClient> _userClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHubContext<ChatHub, IUserClient> userClient)
        {
            _logger = logger;

            _userClient = userClient;
        }

        [HttpGet("Message/{username}")]
        public Task OnMessage(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                _userClient.Clients.All.AddUser(username);
                
                _logger.LogInformation("接受消息......");
            }
            return Task.CompletedTask;
        }
    }
}