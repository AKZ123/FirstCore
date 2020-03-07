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

        //Part:41
        Employee Add(Employee employee);

        //Part: 49
        Employee Update(Employee emploteeChanges);
        Employee Delete(int id);
    }
}
