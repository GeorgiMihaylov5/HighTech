using HighTech.Core.Entities;

namespace HighTech.Core.Services.Abstraction
{
	public interface IEmployeeService
	{
		public ICollection<Employee> GetEmployees();
		public Employee? GetEmployee(string? employeeId);
		public Employee? GetEmployeeByUsername(string? username);
		public Employee CreateEmployee(string? jobTitle, string? userId);
		public bool Update(string? id, string? firstName, string? lastName, string? phone);
	}
}
