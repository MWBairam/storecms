using admin.Models.CmsIdentity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using admin.Data.SeedData;
using admin.Data;
using Microsoft.EntityFrameworkCore;

namespace admin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Instead of the only below command, we modified this method to update-database (commit the migration) 
            //and seed data we want to the DB 
            //then run:
            
            //CreateHostBuilder(args).Build().Run();
            
            //create a host builder instance, ;ater at the end we will run it .Run()
            var host = CreateHostBuilder(args).Build();

            //we are out of startup.cs domain, so we do not have controll over the lifetime of our instances,
            //so we car wrap up our code between using{ } which will discard the instances after the job is done,
            //or we can write "using" statement with the new syntax of as below by creating a scope, then scope is used to bring Serviceprovider,
            //then ServiceProvider is used to bring the services instances we want:
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            //bring the logger service to log exceptions in case appeared:
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            //since we are out of our startup.cs domain, there is no global exception handeling using the app.UseExceptionHandler,
            //so we will do the below using try&catch:
            try
            {
                //bring the AppDbContext to migrate it to the DB to create the DB tables if those are not created in the DB:
                var identityContext = services.GetRequiredService<AppDbContext>();
                await identityContext.Database.MigrateAsync();

                //then bring the UserManager and RoleMananeger services, and call the SeedAsync method while passing those services to it
                //in order to seed the user, roles, user's roles we want to the DB:
                var userManager = services.GetRequiredService<UserManager<CmsUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();                
                await SeedData.SeedAsync(userManager, roleManager, loggerFactory);

            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during migration");
            }

            host.Run();

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
