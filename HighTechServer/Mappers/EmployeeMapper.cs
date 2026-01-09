using HighTech.Core.Entities;
using HighTech.DTOs;

namespace HighTech.Mappers
{
	public static class EmployeeMapper
	{
		public static EmployeeDTO ToDTO(Employee employee)
		{
			if (employee is null)
			{
				return null!;
			}

			return new EmployeeDTO()
			{
				Id = employee.Id,
				JobTitle = employee.JobTitle,
				UserId = employee?.User?.Id,
				FirstName = employee?.User?.FirstName,
				LastName = employee?.User?.LastName,
				Email = employee?.User?.Email,
				PhoneNumber = employee?.User?.PhoneNumber,
				Username = employee?.User?.UserName,
			};
		}

		public static List<EmployeeDTO> ToDTOList(ICollection<Employee> employees)
		{
			return employees?.Select(e => ToDTO(e)).ToList() ?? new List<EmployeeDTO>();
		}
	}
}
