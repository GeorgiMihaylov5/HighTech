using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using Microsoft.EntityFrameworkCore;
using static Duende.IdentityServer.Models.IdentityResources;

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
            return context.Employees.Include(c => c.User).FirstOrDefault(x => x.UserId == employeeId);
        }

        public Employee GetEmployeeByUsername(string username)
        {
            return context.Employees.Include(c => c.User).FirstOrDefault(x => x.User.UserName == username);
        }

        public ICollection<Employee> GetEmployees()
        {
            return context.Employees.Include(c => c.User).ToList();
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

        public bool Update(string id, string firstName, string lastName, string phone)
        {
            var employee = context.Employees.Find(id);

            if (employee is null)
            {
                return false;
            }

            var user = context.Users.FirstOrDefault(x => x.Id == employee.UserId);

            if (user is null)
            {
                return false;
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phone;

            context.Employees.Update(employee);
            context.Users.Update(user);

            return context.SaveChanges() != 0;
        }
    }
}
