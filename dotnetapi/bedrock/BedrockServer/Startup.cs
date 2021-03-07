using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace BedrockServer
{
    public class Startup
    {
        public  void Configure(IApplicationBuilder app,IWebHostEnvironment environment)
        {
            if(environment.IsDevelopment())
            {

            }

            //app.UseMiddleware<GreeterMiddleware>();

            app.UseRouting();

            app.UseEndpoints(route =>
            {
                //route.MapConnectionHandler<PrintConnectionHandler>("/");
               
                route.MapGet("/{name}", async context =>
                {
                    var router = context.Request.RouteValues;

                  // var endpoint= context.GetEndpoint();
                    var feature = context.Features.Get<IEndpointFeature>();
                    await context.Response.WriteAsync($"Endpoint Name {feature.Endpoint.DisplayName}");
                    await context.Response.WriteAsync("Hello world--"+ router["name"]);
                });
            });
        }

        public  void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddHostedService<TestHost>();
        }

        
    }
}
