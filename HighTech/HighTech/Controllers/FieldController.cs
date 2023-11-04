using HighTech.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
    public class FieldController : Controller
    {
        private readonly IFieldService fieldService;
        private readonly ICategoryService categoryService;
        private readonly ICategoryFieldService categoryFieldService;


        public FieldController(IFieldService fieldService, ICategoryService categoryService, ICategoryFieldService categoryFieldService)
        {
            this.fieldService = fieldService;
            this.categoryService = categoryService;
            this.categoryFieldService = categoryFieldService;
        }

        public IActionResult Create(string categoryName, string fieldName, TypeCode typeCode)
        {
            fieldService.CreateField(fieldName, typeCode);
            categoryService.CreateCategory(categoryName);
            categoryFieldService.CreateCategoryField(categoryName, fieldName);

            return Json(fieldService.GetFieldsByCategory(categoryName));
        }

        public IActionResult GetFieldsByCategory(string categoryName)
        {
            return Json(fieldService.GetFieldsByCategory(categoryName));
        }
    }
}
