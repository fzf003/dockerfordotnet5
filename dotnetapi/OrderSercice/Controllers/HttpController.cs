using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderSercice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HttpController : ControllerBase
    {
        // GET: api/<HttpController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //this.HttpContext.Session.SetString("");

            return new string[] { "value1", "value2" };
        }

        //[HttpGet("/api/query")]


       [HttpGet]
       [Route("query/{name}/{age}")]
        public Task<string> Query([FromHeader] string authorization,[FromRoute] string name,string age)
        {
            var upgradeFeature = HttpContext.Features.Get<IHttpUpgradeFeature>();
            /*if (upgradeFeature == null || !upgradeFeature.IsUpgradableRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status426UpgradeRequired;
                return Task.FromResult("error");
            }*/

            return Task.FromResult(name+age);
        }



        // GET api/<HttpController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HttpController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HttpController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HttpController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
