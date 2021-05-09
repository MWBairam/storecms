using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    //githib official page for CreateDefaultBuilder: https://github.com/dotnet/aspnetcore/blob/main/src/DefaultBuilder/src/WebHost.cs
                    //we want to add NLog logging provider to log to files.
                    //in order to add it, we should use the .ConfigureLogging,
                    //Console and Debug and EventSource logging providers were configured by default, so overriding .ConfigureLogging here will delete them,
                    //so write them again.
                    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                    loggingBuilder.AddEventSourceLogger();
                    loggingBuilder.AddNLog();
                });
    }
}
