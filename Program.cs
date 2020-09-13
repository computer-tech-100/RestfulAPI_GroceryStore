using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyApp
{
    // Entry point of program
    public class Program
    {
        public static void Main(string[] args)
        {
            //Create Web Host then build it then Run it then it listens to HTTP requests
            CreateHostBuilder(args).Build().Run();
        }

        //Here we create "CreateHostBuilder" function
        //The CreateHostBuilder is a static method, which creates instance of IHostBuilder
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}