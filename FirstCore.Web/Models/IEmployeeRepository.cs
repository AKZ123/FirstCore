using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCore.Web.Models
{
    //part: 18
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int Id);

        //Part: 27
        IEnumerable<Employee> GetAllEmployee();
    }
}
