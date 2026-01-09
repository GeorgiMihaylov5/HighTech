using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using HighTech.Mappers;
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
				return BadRequest("Category name is required!");
			}

			try
			{
				var category = categoryService.Create(dto.Name);

				if (category is null)
				{
					return BadRequest();
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
				return Json(dto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut]
		[Authorize(Roles = "Administrator")]
		public IActionResult Edit(CategoryDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Id))
			{
				return BadRequest("Category ID is required!");
			}

			if (string.IsNullOrEmpty(dto.Name))
			{
				return BadRequest("Category name is required!");
			}

			try
			{
				var category = categoryService.Edit(dto.Id, dto.Name);

				if (category is null)
				{
					return NotFound($"Category with ID '{dto.Id}' not found.");
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

				return Json(dto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		public IActionResult GetAll()
		{
			try
			{
				var categories = categoryService.GetAll();
				var dtos = CategoryMapper.ToDTOList(categories);

				return Json(dtos);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{id}")]
		public IActionResult Get(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return BadRequest("Category ID is required!");
			}

			try
			{
				var category = categoryService.Get(id);

				if (category is null)
				{
					return NotFound($"Category with ID '{id}' not found.");
				}

				var dto = CategoryMapper.ToDTO(category);

				return Json(dto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Administrator")]
		public IActionResult Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return BadRequest("ID cannot be null!");
			}

			try
			{
				var removed = categoryService.Remove(id);
				return Json(removed);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public IActionResult AddField(string categoryId, string fieldId)
		{
			if (string.IsNullOrEmpty(categoryId))
			{
				return BadRequest("Category ID is required!");
			}

			try
			{
				var categoryField = categoryService.AddFieldToCategory(categoryId, fieldId);
				return Json(categoryField);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete]
		[Authorize(Roles = "Administrator")]
		public IActionResult RemoveField(string categoryId, string fieldId)
		{
			if (string.IsNullOrEmpty(categoryId))
			{
				return BadRequest("Category ID is required!");
			}

			try
			{
				var removed = categoryService.RemoveFieldFromCategory(categoryId, fieldId);
				return Json(removed);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
