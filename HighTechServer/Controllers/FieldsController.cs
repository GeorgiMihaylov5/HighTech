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
				return Response.Success(FieldMapper.ToDTOList(fieldService.GetFields()));
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.UnknownError, 400);
			}
		}

		[HttpGet("{categoryId}")]
		public IActionResult GetFieldsByCategory(string categoryId)
		{
			if (string.IsNullOrEmpty(categoryId))
			{
				return Response.Error("Category ID is required!", ErrorCode.CategoryIdMissing, 400);
			}

			try
			{
				var fields = fieldService.GetFieldsByCategory(categoryId);
				return Response.Success(FieldMapper.ToDTOList(fields));
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.UnknownError, 400);
			}
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public IActionResult Create(FieldDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Name))
			{
				return Response.Error("Field name is required!", ErrorCode.FieldNameMissing, 400);
			}

			try
			{
				var field = fieldService.CreateField(dto.Name, dto.TypeCode);

				return Response.Success(FieldMapper.ToDTO(field), 201);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.FieldCreateError, 400);
			}
		}

		[HttpPut]
		[Authorize(Roles = "Administrator")]
		public IActionResult Edit(FieldDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Id))
			{
				return Response.Error("Field ID is required!", ErrorCode.FieldIdMissing, 400);
			}

			if (string.IsNullOrEmpty(dto.Name))
			{
				return Response.Error("Field name is required!", ErrorCode.FieldNameMissing, 400);
			}

			try
			{
				var field = fieldService.EditField(dto.Id, dto.Name, dto.TypeCode);

				if (field is null)
				{
					return Response.Error($"Field with ID '{dto.Id}' not found.", ErrorCode.FieldNotFound, 404);
				}

				return Response.Success(FieldMapper.ToDTO(field));
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.FieldUpdateError, 400);
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Administrator")]
		public IActionResult Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return Response.Error("ID cannot be null!", ErrorCode.FieldIdMissing, 400);
			}

			try
			{
				var removed = fieldService.RemoveField(id);
				return Response.Success(removed);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.FieldDeleteError, 400);
			}
		}
	}
}
