using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ProductApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)=>
                      Host.CreateDefaultBuilder(args)
           
                        .ConfigureWebHostDefaults(webBuilder =>
                        {
                            //webBuilder.UseKestrel();
                            webBuilder.UseStartup<Startup>()
                           .ConfigureAppConfiguration((context, c) =>
                           {
                                /*c.SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddJsonFile("secrets.json", optional: false, reloadOnChange: true);*/
                           });

                            webBuilder.UseUrls("http://localhost:9911");

                        });
        
    }
}
