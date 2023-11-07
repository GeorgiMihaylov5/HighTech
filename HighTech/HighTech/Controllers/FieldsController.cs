using HighTech.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FieldsController : Controller
    {
        private readonly IFieldService fieldService;
        private readonly ICategoryService categoryService;


        public FieldsController(IFieldService fieldService, ICategoryService categoryService)
        {
            this.fieldService = fieldService;
            this.categoryService = categoryService;
        }

        public IActionResult Create(string categoryName, string fieldName, TypeCode typeCode)
        {
            fieldService.CreateField(fieldName, typeCode);
            categoryService.CreateCategoryField(categoryName, fieldName);

            return Json(fieldService.GetFieldsByCategory(categoryName));
        }

        public IActionResult GetFieldsByCategory(string categoryName)
        {
            return Json(fieldService.GetFieldsByCategory(categoryName));
        }
    }
}
