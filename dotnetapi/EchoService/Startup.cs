using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using EchoService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace EchoService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddGrpc(o=> {
                o.ResponseCompressionLevel = CompressionLevel.Fastest;
                o.ResponseCompressionAlgorithm = "gzip";
            });
            services.AddTransient<IChangeStream, UserChangeStream>();
        }

        public IConfigurationRoot Configuration { get; set;}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();

                endpoints.MapGrpcService<UserInfoService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });

                endpoints.MapGet("/conf", async context =>
                {
                    await context.Response.WriteAsync(Configuration.GetDebugView());

                });

                endpoints.MapGet("/env",async context=>{
                    context.Response.Headers["Content-Type"]="text/html";
 
                   await context.Response.WriteAsync(env.EnvironmentName+"<br/>");

                    await context.Response.WriteAsync(env.ApplicationName+"<br/>");
                    await context.Response.WriteAsync(Environment.GetEnvironmentVariable("fzf003"));
                });

                

                 
            });
        }
    }
}
