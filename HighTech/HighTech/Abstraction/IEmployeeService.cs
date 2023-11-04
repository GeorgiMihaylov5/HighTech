using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IEmployeeService
    {
        public List<Employee> GetEmployees();
        public Employee GetEmployee(string employeeId);
        public bool CreateEmployee(string jobTitle, string userId);
        public bool Remove(string employeeId);
        public string GetFullName(string employeeId);
        public bool Update(string id, string jobTitle);
    }
}
