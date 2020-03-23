using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCore.Web.Models
{
    //https://docs.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli   Database Providers
    //Part: 47,                 65.1
    public class AppDbContext : IdentityDbContext       //DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }

        //Part: 51
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Part:65.2
            base.OnModelCreating(modelBuilder);

            modelBuilder.Seed();
            //ModelBuilderExtensions class
        }
    }
}
