using HighTech.Core.Entities;
using HighTech.DTOs;

namespace HighTech.Mappers
{
	public static class FieldMapper
	{
		public static FieldDTO ToDTO(Field field)
		{
			if (field is null)
			{
				return null!;
			}

			return new FieldDTO()
			{
				Id = field.Id,
				Name = field.Name,
				TypeCode = field.TypeCode,
			};
		}

		public static List<FieldDTO> ToDTOList(ICollection<Field> fields)
		{
			return fields?.Select(f => ToDTO(f)).ToList() ?? new List<FieldDTO>();
		}
	}
}
