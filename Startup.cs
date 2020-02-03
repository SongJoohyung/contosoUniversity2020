using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using contosoUniversity2020.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using contosoUniversity2020.Services;

namespace contosoUniversity2020
{
    public class Startup
    {
        //mwilliams: Part 11: IDENTITY FRAMEWORK    
        //Create private member for reading the secret key
        private string _adminUserPW = null;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //mwilliams: Part 3: Register the SchoolContext (database context) using Dependency Injection (DI) 
            services.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //end part 3

            /*  services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                  .AddEntityFrameworkStores<ApplicationDbContext>();
              services.AddControllersWithViews();
              services.AddRazorPages();*/
            //mwilliams: Part 11: IDENTITY FRAMEWORK 

            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI() //for routing identity pages (razoe pages, not mvc)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders(); //for email confirmation tokens

             //mwilliams: Part 15: - Register the NewsService using Dependency Injection
              services.AddTransient<INewsService, NewsService>();
              
              services.AddControllersWithViews();
              services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,//mwilliams: Part 5: Seeding the database (SchoolContext)
                              /*SchoolContext context*/
                              IServiceProvider serviceProvider//mwillaims: Part 11: IDENTITY FRAMEWORK
                              )
            {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //mwilliams: Part 10: Custom Error Pages (for 404 and 500 status codes)
            //Note: Must call the UseStatusCodePages before request handling middlewares
            //      like Static Files and MVXC middlewares
            app.UseStatusCodePagesWithReExecute("/Error/{0}"); //send to Error Controller passing status code arg

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //mwilliams: Part 5: Seeding the database (SchoolContext)
            //DbInitializer.Initialize(context);

            //mwilliams: Part 11: IDENTITY FRAMEWORK    
            //first we need to get our admin password
            /*_adminUserPW = Configuration["adminUserPW"];
            //now we can call our IdentityDbInitializer
            IdentityDbInitializer.Initialize(serviceProvider, _adminUserPW).Wait();
            */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
