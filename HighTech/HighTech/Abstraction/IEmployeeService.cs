using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IEmployeeService
    {
        public ICollection<Employee> GetEmployees();
        public Employee GetEmployee(string employeeId);
        public Employee CreateEmployee(string jobTitle, string userId);
        public bool Remove(string employeeId);
        public string GetFullName(string employeeId);
        public bool Update(string id, string jobTitle);
    }
}
