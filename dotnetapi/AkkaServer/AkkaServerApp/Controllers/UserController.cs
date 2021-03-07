using Akka.Client.Autogen.Grpc.v1;
using AkkaServer.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AkkaServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly ActorRefProvider<ResponseActor> _userServiceProvider;
        readonly ILogger _logger;

        readonly IDistributedCache _distributedCache;
        readonly IConfiguration configuration;
        public UserController(ActorRefProvider<ResponseActor> userServiceProvider, ILogger<UserController> logger, IDistributedCache distributedCache, IConfiguration configuration)
        {
            _userServiceProvider = userServiceProvider;
            _logger = logger;
            _distributedCache = distributedCache;
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("user/{name}/age/{age}")]
        public async Task<string> QueryUser([FromRoute] string name,string age)
        {
            var message = new RequestMessae {
                Message = $"{name}--{age}"
            };
            var  reply= await this._userServiceProvider.Ask<ReplyMessage>(message);

            return reply.Message;
        }

       // [HttpPost("query")]
       [HttpGet]
        [Route("query/{Id}/{Name}")]
        public async Task<string> Query([FromRoute]Myrequest myrequest)
        {
           
            return await Task.FromResult($"{myrequest.Name}--{myrequest.Id}");
        }

        [HttpGet(Name ="endpoint")]
        public async Task<string> GetEndpoint()
        {
            return this.configuration.GetSection("urls").Value;
        }
    }

    public class Myrequest
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
