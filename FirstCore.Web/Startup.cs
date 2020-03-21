using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstCore.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            //Part: 16,
            services.AddMvc().AddXmlSerializerFormatters();
            //Part:19,44, (49)
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
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
