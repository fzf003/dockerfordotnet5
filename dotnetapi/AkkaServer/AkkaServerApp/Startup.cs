using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AkkaServer.Core;
using Microsoft.OpenApi.Models;
using AkkaServerApp.GrpcService;
using System.IO.Compression;
using System.Net.Http;
using AkkaServerApp.Services;
using Microsoft.AspNetCore.Http;
using System;
using Steeltoe.Extensions.Logging;
using Steeltoe.Management.Tracing;

namespace AkkaServerApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(c=> {
                c.EnableDetailedErrors = true;
                c.ResponseCompressionLevel = CompressionLevel.Fastest;
            });
            services.AddLogging(l => l.AddConsole())
                    .AddActorReference<ResponseActor>(provider => ResponseActor.For(provider), "fzf003")
                    .AddHostedService<Worker>();
            services.AddControllers()
                    .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderSercice", Version = "v1" });
            });

            services.AddHttpClient("http",configureClient=> {
                configureClient.BaseAddress = new System.Uri("http://v1.jinrishici.com");
            });

            services.AddDistributedMemoryCache();
            services.AddDistributedTracing(Configuration,builder=> {
                builder.UseZipkinWithTraceOptions(services);
            });
             
            ///services.AddSingleton<ProjectService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderSercice v1"));


            app.UseHttpsRedirection();

            app.UseRouting();

          
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<QueryUserService>();
                endpoints.Map("/", async context => {
                  var config=  context.RequestServices.GetService<IConfiguration>();
                   await context.Response.WriteAsync(config["urls"]);
                });
            });
        }
    }
}
