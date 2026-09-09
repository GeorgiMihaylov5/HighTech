using HighTech.Abstraction;
using HighTech.Common;
using HighTech.DTOs;
using HighTech.Exceptions;
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
            return Response.ResultSuccess(fieldService.GetFields().Select(f => new FieldDTO()
            {
                Id = f.Id,
                Name = f.Name,
                TypeCode = f.TypeCode
            }));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(FieldDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
            {
                return Response.ResultFailure<FieldDTO>("Field name is required!", 400);
            }
            else if (dto.Name.Length > StringLimits.FieldName)
            {
                return Response.ResultFailure<FieldDTO>($"Field name cannot be longer than {StringLimits.FieldName} characters.", 400);
            }

            try
            {
                var field = fieldService.CreateField(dto.Name, dto.TypeCode);

                return Response.ResultSuccess(new FieldDTO()
                {
                    Id = field.Id,
                    Name = field.Name,
                    TypeCode = field.TypeCode
                });
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<FieldDTO>("An unexpected error occurred.", 500);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(FieldDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
            {
                return Response.ResultFailure<FieldDTO>("Field name is required!", 400);
            }
            else if (dto.Name.Length > StringLimits.FieldName)
            {
                return Response.ResultFailure<FieldDTO>($"Field name cannot be longer than {StringLimits.FieldName} characters.", 400);
            }

            try
            {
                var field = fieldService.EditField(dto.Id, dto.Name, dto.TypeCode);

                return Response.ResultSuccess(new FieldDTO()
                {
                    Id = field.Id,
                    Name = field.Name,
                    TypeCode = field.TypeCode
                });
            }
            catch (ConfiguratorMetadataException ex)
            {
                return Response.ResultFailure<FieldDTO>(ex.Message, 400);
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<FieldDTO>("An unexpected error occurred.", 500);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Response.ResultFailure<bool>("Id cannot be null!", 400);
            }

            try
            {
                var removed = fieldService.RemoveField(id);
                return Response.ResultSuccess(removed);
            }
            catch (ConfiguratorMetadataException ex)
            {
                return Response.ResultFailure<bool>(ex.Message, 400);
            }
            catch (FieldInUseException ex)
            {
                return Response.ResultFailure<bool>(ex.Message, 400);
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<bool>("An unexpected error occurred.", 500);
            }
        }
    }
}

