using HighTech.Abstraction;
using HighTech.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }

        public IActionResult Create(CategoryDTO dto)
        {
            var categoryCreated = categoryService.CreateCategoryField(dto.CategoryId, dto.Field.FieldName);

            if (!categoryCreated)
            {
                return BadRequest();
            }

            return Ok();
        }

        public IActionResult GetAll()
        {
            var categories = categoryService
                .GetAll()
                .Select(c => new CategoryDTO()
                {
                    CategoryId = c.CategoryId,
                    Field = new FieldDTO() 
                    { 
                        FieldName = c.Field.Id,
                        TypeCode = c.Field.TypeCode,
                        Value = null
                    },
                });

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
