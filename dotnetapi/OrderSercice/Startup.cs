using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using System.IO;
using TinyService.ReactiveRabbit;
using OrderSercice.Services;
using TinyService;
using TinyService.Core;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;

namespace OrderSercice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Debug));
             services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderSercice", Version = "v1" });
            });

            services.AddReactiveRabbit(setting =>
            {
                setting.HostName = "localhost";
                setting.Port = 5672;
                setting.VirtualHost = "fzf003";
                setting.UserName = "fzf003";
                setting.Password = "fzf003";
            });

            services.AddSingleton<IRabbmitServiceWorker, RabbmitServiceWorker>();

            services.UseTinyService();
 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderSercice v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/{name}/{sex}", async context => {
                    var router= context.Request.RouteValues;
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync($"{router["name"] as string}<br/>");
                    await context.Response.WriteAsync(router["sex"] as string);
                });

                endpoints.CreateOrderToPost("/{order}");
                
 
            });

   

        }
    }
    
}
