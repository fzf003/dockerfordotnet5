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
using TestLib;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;

namespace ProductApi
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
            services.AddSingleton<UserService>();

            services.Configure<UserSetues>(Configuration.GetSection(nameof(UserSetues)));
            services.AddHostedService<HostService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductApi", Version = "v1" });
            });

            services.AddSingleton(serviceProvider =>
            {
                var server = serviceProvider.GetRequiredService<IServer>();
                return server.Features.Get<IServerAddressesFeature>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServerAddressesFeature server)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            var fearures = server;
                //app.ServerFeatures.Get<IServerAddressesFeature>();

       
     
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
               
               var user= endpoints.ServiceProvider.GetService<IOptionsMonitor<UserSetues>>();
                 
                endpoints.MapGet("/",async context=>{
                     await context.Response.WriteAsync(Guid.NewGuid().ToString("N")+"--"+user.CurrentValue.Name);
                });


            });
        }
    }
}
