using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;

namespace HighTech.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext context;

        public EmployeeService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public Employee CreateEmployee(string jobTitle, string userId)
        {
            if (context.Employees.Any(x => x.UserId == userId))
            {
                throw new InvalidOperationException("User already exist.");
            }

            var emp = new Employee()
            {
                JobTitle = jobTitle,
                UserId = userId,
            };

            context.Employees.Add(emp);
            context.SaveChanges();

            return emp;
        }

        public Employee GetEmployee(string employeeId)
        {
            return context.Employees.FirstOrDefault(x => x.UserId == employeeId);
        }

        public ICollection<Employee> GetEmployees()
        {
            return context.Employees.ToList();
        }

        //TODO
        public string GetFullName(string employeeId)
        {
            throw new NotImplementedException();
        }
        //TODO
        public bool Remove(string employeeId)
        {
            throw new NotImplementedException();
        }

        public bool Update(string id, string jobTitle)
        {
            var employees = GetEmployee(id);

            if (employees is null)
            {
                return false;
            }

            employees.JobTitle = jobTitle;

            context.Employees.Update(employees);
            return context.SaveChanges() != 0;
        }
    }
}
