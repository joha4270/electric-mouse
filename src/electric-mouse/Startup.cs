using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using electric_mouse.Services.Interfaces;

namespace electric_mouse
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddEntityFramework()
                .AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options => options.Filters.Add(typeof(Filters.AddLanguageFilter)));
            services.Configure<MvcOptions>(options => { options.Filters.Add(new RequireHttpsAttribute()); });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>()
	            .AddTransient<ISmsSender, AuthMessageSender>()
	            .AddTransient<FacebookAPI>()
	            .AddTransient<AttachmentHandler>()
	            .AddTransient<IApiService, ApiService>()
	            .AddTransient<IDifficultyService, DifficultyService>()
	        	.AddTransient<ICommentService, CommentService>()
	        	.AddTransient<IRouteService, RouteService>()
	        	.AddTransient<IUserService, UserService>()
	        	.AddTransient<IAttachmentHandler, AttachmentHandler>()
	            .AddTransient<IHallService, HallService>()
	            .AddTransient<ISectionService, SectionService>();
            
            services.AddSingleton<LanguageCache>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            

            app.UseStaticFiles();

            app.UseIdentity();
            RoleHandler.AddRoles(serviceProvider, RoleHandler.Admin, RoleHandler.Post);

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            string[] conf = File.ReadAllLines("secrets.txt");
            
            app.UseFacebookAuthentication(new FacebookOptions()
            {
                AppId = conf[0],
                AppSecret = conf[1],
                SaveTokens = true
            });

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "default", // route
                    template: "{controller=Route}/{action=List}/{id?}");
                // Not sure if we'll need it though, nice as a test bench for now
                routes.MapRoute(
                    name: "home",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
