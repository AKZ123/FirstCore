using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCore.Web.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        //part:18
        private List<Employee> _employees;
        public MockEmployeeRepository()
        {
            _employees = new List<Employee>()
            {
                new Employee() { Id=1,Name="Kaer", Email="a@k.com", Department="HR"},
                new Employee() { Id=2,Name="Kaer2", Email="a@k2.com", Department="HR2" },
                new Employee() { Id=3,Name="Kaer3", Email="a@k3.com", Department="HR3"}
            };
        }

        //Part:27
        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employees;
        }

        public Employee GetEmployee(int Id)
        {
            return _employees.FirstOrDefault(x => x.Id == Id);
        }
    }
}
