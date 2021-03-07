using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderSercice.Services;
using TinyService.RequestResponse;
namespace OrderSercice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        readonly TinyService.Core.IActorFactory actorFactory;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, TinyService.Core.IActorFactory actorFactory)
        {
            _logger = logger;
            this.actorFactory = actorFactory;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> SayHello(string name)
        {
           var usersub= this.actorFactory.GetActor<UserActor>("fzf003");

           var reply=await usersub.RequestAsync<string>(new UserActor.UserInfo(name: "fzf003", body: $"{name}--{Guid.NewGuid().ToString("N")}").ToRequest());
 
            return Ok(new { message = reply });
        }

        [HttpPut()]
        public async Task<IActionResult> SayPost(User user)
        {
            _logger.LogInformation($"submit:{user}");
            var order1 = user with { name = "fzf0099" };
            return this.Ok(order1);
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
