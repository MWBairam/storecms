using admin.Data;
using admin.Models.CmsIdentity;
using admin.Services.Implementations;
using admin.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin
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
            services.AddControllersWithViews();

            //Add Identity Service:
            //Use CmsUser instead of the default IdentityUser
            services.AddIdentity<CmsUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            //the token provider is used because we used ResetPassword in AccountController.
            //Also, set the Identity cookie expiration timeout for 1 hour so the user will have to login in again.
            services.ConfigureApplicationCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(60));
            //and override password complexity options:
            //official class for password options https://github.com/dotnet/aspnetcore/blob/c925f99cddac0df90ed0bc4a07ecda6b054a0b02/src/Identity/Extensions.Core/src/PasswordOptions.cs
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;
            });


            //PostgresSql service for the Store tables:
            services.AddDbContext<ReversScaffoldedStoreContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            //and for store users tables:
            services.AddDbContext<ReversScaffoldedStoreIdentityContext>(options => options.UseNpgsql(Configuration.GetConnectionString("StoreIdentityConnection")));

            //Add Db service for the CMS app DB:
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("CmsDBConnection")));

            //add the UploadFile service:
            services.AddTransient<IUploadFile, UploadFile>();
        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
