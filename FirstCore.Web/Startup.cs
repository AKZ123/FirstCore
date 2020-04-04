using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstCore.Web.Models;
using FirstCore.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FirstCore.Web
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Part: 48
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            //Part: 65.3,        77.2.2
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
            //Part: 68.1
            //services.Configure<IdentityOptions>(option =>
            //{
            //    option.Password.RequiredLength = 10;
            //    option.Password.RequiredUniqueChars = 3;
            //});

            //Part: 16,
            services.AddMvc().AddXmlSerializerFormatters();  //Authorize set for whole application on this line p 71
            //Part:106.1
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "71904163316-djrdeocs3mk29a8gb10mud62e6vkl5ru.apps.googleusercontent.com";
                options.ClientSecret = "l80WOhAKmIAK4fpdMKETLc6R";
                //options.CallbackPath = "";  //Part:107.2
            })
             .AddFacebook(options =>             //Part:109
             {
                 options.AppId = "547600819507295";
                 options.AppSecret = "601f266bfde5a0eb66aefd7458f021c7";
             }) ;
            //Part: 97.1
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });
            //Part:94.1
            services.AddAuthorization(options => 
            {
                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role")
                                    //.RequireClaim("Create Role")
                                    );
                //Part: 95
                options.AddPolicy("AdminRolePolicy",
                  policy => policy.RequireRole("Admin"));
                //Part: 96.1
                //options.AddPolicy("EditRolePolicy",
                //   //policy => policy.RequireClaim("Edit Role"));
                //    policy => policy.RequireClaim("Edit Role","true"));  //Part: 98.4
                //Part: 99.1
                //options.AddPolicy("EditRolePolicy",
                //    policy => policy.RequireAssertion(context=>
                //    context.User.IsInRole("Admin") &&
                //    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value=="true") ||
                //    context.User.IsInRole("Super Admin")
                //    ));
                //Part: 101.3.1
                options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
            });

            //Part:19,44, (49)
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

            //Part:101.3.2
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            //Part:102.2
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Default 1 && Part: 11,12
            if (env.IsDevelopment())   //Production
            {
                app.UseDeveloperExceptionPage();
            }// Part: 58
            else
            {
                //Part: 60   Global exception
                app.UseExceptionHandler("/Error");

                //app.UseStatusCodePages();                           for URL misteck
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            //Part:12, & for access file/folder wwwroot(css,images,js,lib=>bootstrap,jquery)
            app.UseStaticFiles();
            //Default 2
            app.UseRouting();
            //app.UseMvcWithDefaultRoute();
            //Part: 65.4
            app.UseAuthentication();
            //Part: 71.1
            app.UseAuthorization();
            //Part: 32,36
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute( "default","Kader/{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.Run(async(context) => 
            //{
            //     await context.Response.WriteAsync("Hello Word 12!");
            //});

            //Default 3
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
        }
    }
}
