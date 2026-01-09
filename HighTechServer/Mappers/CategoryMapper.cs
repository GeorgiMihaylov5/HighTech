using HighTech.Core.Entities;
using HighTech.DTOs;

namespace HighTech.Mappers
{
	public static class CategoryMapper
	{
		public static CategoryDTO ToDTO(Category category)
		{
			if (category is null)
			{
				return null!;
			}

			return new CategoryDTO()
			{
				Id = category.Id,
				Name = category.Name,
				Fields = category.CategoryFields?.Select(cf => new FieldDTO()
				{
					Id = cf.Field!.Id,
					Name = cf.Field.Name,
					TypeCode = cf.Field.TypeCode,
					Value = null
				}).ToList() ?? new List<FieldDTO>()
			};
		}

		public static List<CategoryDTO> ToDTOList(ICollection<Category> categories)
		{
			return categories?.Select(ToDTO).ToList() ?? new List<CategoryDTO>();
		}
	}
}
