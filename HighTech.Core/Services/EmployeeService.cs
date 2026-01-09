using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class EmployeeService : IEmployeeService
	{
		private readonly IEmployeeRepository employeeRepository;

		public EmployeeService(IEmployeeRepository _employeeRepository)
		{
			employeeRepository = _employeeRepository;
		}

		public Employee CreateEmployee(string? jobTitle, string? userId)
		{
			return employeeRepository.CreateEmployee(jobTitle, userId);
		}

		public Employee? GetEmployee(string? employeeId)
		{
			return employeeRepository.GetEmployee(employeeId);
		}

		public Employee? GetEmployeeByUsername(string? username)
		{
			return employeeRepository.GetEmployeeByUsername(username);
		}

		public ICollection<Employee> GetEmployees()
		{
			return employeeRepository.GetEmployees();
		}

		public bool Update(string? id, string? firstName, string? lastName, string? phone)
		{
			return employeeRepository.Update(id, firstName, lastName, phone);
		}
	}
}
