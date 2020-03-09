using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCore.Web.Models
{
    //Part: 51
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
             modelBuilder.Entity<Employee>().HasData(
                    new Employee
                    {
                        Id = 1,
                        Name = "Kader",
                        Department = Dept.IT,
                        Email = "k@k.com"
                    },
                    new Employee
                    {
                        Id = 2,
                        Name = "Mark",
                        Department = Dept.HR,
                        Email = "m@m.com"
                    }
              );
        }
    }
}
