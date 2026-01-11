using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using HighTech.Mappers;
using HighTechServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class CategoriesController : Controller
	{
		private readonly ICategoryService categoryService;

		public CategoriesController(ICategoryService _categoryService)
		{
			categoryService = _categoryService;
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public IActionResult Create(CategoryDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Name))
			{
				return Response.Error("Category name is required!", ErrorCode.CategoryNameMissing, 400);
			}

			try
			{
				var category = categoryService.Create(dto.Name);

				if (category is null)
				{
					return Response.Error("Failed to create category.", ErrorCode.CategoryCreateError, 400);
				}

				// Add fields to the category if provided
				if (dto.Fields is not null && dto.Fields.Count > 0)
				{
					foreach (var field in dto.Fields)
					{
						categoryService.AddFieldToCategory(category.Id!, field.Id);
					}
				}

				dto.Id = category.Id;
				return Response.Success(dto, 201);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.CategoryCreateError, 400);
			}
		}

		[HttpPut]
		[Authorize(Roles = "Administrator")]
		public IActionResult Edit(CategoryDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Id))
			{
				return Response.Error("Category ID is required!", ErrorCode.CategoryIdMissing, 400);
			}

			if (string.IsNullOrEmpty(dto.Name))
			{
				return Response.Error("Category name is required!", ErrorCode.CategoryNameMissing, 400);
			}

			try
			{
				var category = categoryService.Edit(dto.Id, dto.Name);

				if (category is null)
				{
					return Response.Error($"Category with ID '{dto.Id}' not found.", ErrorCode.CategoryNotFound, 404);
				}

				// Update fields if provided
				if (dto.Fields is not null)
				{
					// Get current fields
					var currentFields = categoryService.GetCategoryFields(dto.Id);
					var currentFieldIds = currentFields.Select(cf => cf.FieldId).ToHashSet();
					var newFieldIds = dto.Fields.Select(f => f.Id).ToHashSet();

					// Remove fields that are no longer in the list
					var fieldsToRemove = currentFieldIds.Except(newFieldIds);
					foreach (var fieldId in fieldsToRemove)
					{
						categoryService.RemoveFieldFromCategory(dto.Id, fieldId);
					}

					// Add new fields
					var fieldsToAdd = newFieldIds.Except(currentFieldIds);
					foreach (var fieldId in fieldsToAdd)
					{
						categoryService.AddFieldToCategory(dto.Id, fieldId);
					}
				}

				return Response.Success(dto);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.CategoryUpdateError, 400);
			}
		}

		public IActionResult GetAll()
		{
			try
			{
				var categories = categoryService.GetAll();
				var dtos = CategoryMapper.ToDTOList(categories);

				return Response.Success(dtos);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.UnknownError, 400);
			}
		}

		[HttpGet("{id}")]
		public IActionResult Get(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return Response.Error("Category ID is required!", ErrorCode.CategoryIdMissing, 400);
			}

			try
			{
				var category = categoryService.Get(id);

				if (category is null)
				{
					return Response.Error($"Category with ID '{id}' not found.", ErrorCode.CategoryNotFound, 404);
				}

				var dto = CategoryMapper.ToDTO(category);

				return Response.Success(dto);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.UnknownError, 400);
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Administrator")]
		public IActionResult Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return Response.Error("ID cannot be null!", ErrorCode.CategoryIdMissing, 400);
			}

			try
			{
				var removed = categoryService.Remove(id);
				return Response.Success(removed);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.CategoryDeleteError, 400);
			}
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public IActionResult AddField(string categoryId, string fieldId)
		{
			if (string.IsNullOrEmpty(categoryId))
			{
				return Response.Error("Category ID is required!", ErrorCode.CategoryIdMissing, 400);
			}

			try
			{
				var categoryField = categoryService.AddFieldToCategory(categoryId, fieldId);
				return Response.Success(categoryField);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.CategoryUpdateError, 400);
			}
		}

		[HttpDelete]
		[Authorize(Roles = "Administrator")]
		public IActionResult RemoveField(string categoryId, string fieldId)
		{
			if (string.IsNullOrEmpty(categoryId))
			{
				return Response.Error("Category ID is required!", ErrorCode.CategoryIdMissing, 400);
			}

			try
			{
				var removed = categoryService.RemoveFieldFromCategory(categoryId, fieldId);
				return Response.Success(removed);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.CategoryUpdateError, 400);
			}
		}
	}
}
