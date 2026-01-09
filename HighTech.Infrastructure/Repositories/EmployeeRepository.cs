using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Infrastructure.Repositories
{
	public class EmployeeRepository : IEmployeeRepository
	{
		private readonly ApplicationDbContext context;

		public EmployeeRepository(ApplicationDbContext _context)
		{
			context = _context;
		}

		public Employee CreateEmployee(string? jobTitle, string? userId)
		{
			if (context.Employees.Any(x => x.UserId == userId))
			{
				throw new InvalidOperationException($"Employee for user ID '{userId}' already exists.");
			}

			// Validate user exists
			var userExists = context.Users.Any(u => u.Id == userId);
			if (!userExists)
			{
				throw new InvalidOperationException($"User with ID '{userId}' does not exist.");
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

		public Employee? GetEmployee(string? employeeId)
		{
			return context.Employees
				.Include(e => e.User)
				.FirstOrDefault(x => x.UserId == employeeId);
		}

		public Employee? GetEmployeeByUsername(string? username)
		{
			var user = context.Users.FirstOrDefault(x => x.UserName == username);
			if (user == null) return null;

			return context.Employees
				.Include(e => e.User)
				.FirstOrDefault(e => e.UserId == user.Id);
		}

		public ICollection<Employee> GetEmployees()
		{
			return context.Employees.Include(e => e.User).ToList();
		}

		public bool Update(string? id, string? firstName, string? lastName, string? phone)
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
