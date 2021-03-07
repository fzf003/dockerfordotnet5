using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Newtonsoft.Json;
using TinyService.ReactiveRabbit;
using System;

namespace OrderSercice.Services
{
    public static class EndpointExtensions
    {

        public static IEndpointRouteBuilder CreateOrderToPost(this IEndpointRouteBuilder endpoints, string prttern)
        {

            endpoints.MapPost(prttern, CreateOrderProcess);

            return endpoints;
        }

       static async Task CreateOrderProcess(HttpContext context)
        {
            var serviceEndpoint = context.RequestServices.GetService<IRabbmitServiceWorker>();
 
            var router = context.Request.RouteValues;

            var ordername = router["order"] as string;

            context.Response.ContentType = "application/json";
            try
            {

                var endpoint = serviceEndpoint.InputEndpoint;

                using (var streamReader = new StreamReader(context.Request.Body))
                {
                    var body = await streamReader.ReadToEndAsync();

                     endpoint.PushMessage(JsonConvert.DeserializeObject<User>(body));
 
                    await context.Response.WriteAsJsonAsync(new ReplyMessage(success: true, error: string.Empty, message: "成功！！")); ;

                }
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsJsonAsync(new ReplyMessage(success: false, error: "error", message: ex.Message));
            }
        }
    }
}
