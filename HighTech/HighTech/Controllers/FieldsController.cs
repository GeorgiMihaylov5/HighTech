using HighTech.Abstraction;
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
            return Json(fieldService.GetFields().Select(f => new FieldDTO()
            {
                FieldName = f.Id,
                TypeCode = f.TypeCode,
            }));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(FieldDTO dto)
        {
            if (string.IsNullOrEmpty(dto.FieldName))
            {
                return BadRequest("Field name is required!");
            }

            try
            {
                var field = fieldService.CreateField(dto.FieldName, dto.TypeCode);

                return Json(new FieldDTO()
                {
                    FieldName = field.Id,
                    TypeCode = field.TypeCode
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
