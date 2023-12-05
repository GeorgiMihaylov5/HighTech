using HighTech.Abstraction;
using HighTech.DTOs;
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

        public IActionResult Create(CategoryDTO dto)
        {
            foreach (var field in dto.Fields)
            {
                var categoryCreated = categoryService.CreateCategoryField(dto.CategoryId, field.FieldName);

                if (!categoryCreated)
                {
                    return BadRequest();
                }
            }

            return Ok();
        }

        public IActionResult GetAll()
        {
            var categories = categoryService
                .GetAll()
                .GroupBy(c => c.CategoryId)
                .Select(group => new CategoryDTO()
                    {
                        CategoryId = group.Key,
                        Fields = group.Select(c => new FieldDTO()
                        {
                            FieldName = c.Field.Id,
                            TypeCode = c.Field.TypeCode,
                            Value = null
                        })
                    }
                );

            return Json(categories);
        }

        public IActionResult Remove(string id)
        {
            if (id is null)
            {
                NotFound();
            }

            var removed = categoryService.RemoveCategories(id);
            //TODO Way to return boolean values
            return Ok(removed);
        }
    }
}
