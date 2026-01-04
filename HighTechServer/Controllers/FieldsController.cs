using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class FieldsController : Controller
	{
		private readonly IFieldService fieldService;

		public FieldsController(IFieldService fieldService)
		{
			this.fieldService = fieldService;
		}

		public IActionResult GetFields()
		{
			try
			{
				return Json(fieldService.GetFields().Select(f => new FieldDTO()
				{
					Id = f.Id,
					Name = f.Name,
					TypeCode = f.TypeCode,
				}));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{categoryId}")]
		public IActionResult GetFieldsByCategory(string categoryId)
		{
			if (string.IsNullOrEmpty(categoryId))
			{
				return BadRequest("Category ID is required!");
			}

			try
			{
				var fields = fieldService.GetFieldsByCategory(categoryId);
				return Json(fields.Select(f => new FieldDTO()
				{
					Id = f.Id,
					Name = f.Name,
					TypeCode = f.TypeCode,
				}));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public IActionResult Create(FieldDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Name))
			{
				return BadRequest("Field name is required!");
			}

			try
			{
				var field = fieldService.CreateField(dto.Name, dto.TypeCode);

				return Json(new FieldDTO()
				{
					Id = field.Id,
					Name = field.Name,
					TypeCode = field.TypeCode
				});
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut]
		[Authorize(Roles = "Administrator")]
		public IActionResult Edit(FieldDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Id))
			{
				return BadRequest("Field ID is required!");
			}

			if (string.IsNullOrEmpty(dto.Name))
			{
				return BadRequest("Field name is required!");
			}

			try
			{
				var field = fieldService.EditField(dto.Id, dto.Name, dto.TypeCode);

				if (field is null)
				{
					return NotFound($"Field with ID '{dto.Id}' not found.");
				}

				return Json(new FieldDTO()
				{
					Id = field.Id,
					Name = field.Name,
					TypeCode = field.TypeCode
				});
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
				var removed = fieldService.RemoveField(id);
				return Json(removed);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
