using System;
using System.Collections.Generic;
using System.IO;
using Identity.App.Extention;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Identity.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            webHost.Services.InitializeDb();
            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args){
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) => 
                {
                    var env = hostingContext.HostingEnvironment;
                    config.SetBasePath(env.ContentRootPath);
                    config.AddInMemoryCollection(new[] 
                    {
                        new KeyValuePair<string, string>("the-key", "the-value")
                    })
                    .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
                    .AddJsonFile($"appsettings.{env}.json", optional: true)
                    .AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging)=> {
                    logging.AddDebug();
                    logging.AddConsole();
                    // logging.AddDbLogger(); // هنوز تکمیل نشده
                })
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) => {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .UseStartup<Startup>();

            //WebHost.CreateDefaultBuilder(args)
            //    .UseStartup<Startup>();
        }
            
            

    }
}
