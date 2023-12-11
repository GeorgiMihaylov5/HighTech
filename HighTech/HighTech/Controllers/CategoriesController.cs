using HighTech.Abstraction;
using HighTech.DTOs;
using HighTech.Models;
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
            if(string.IsNullOrEmpty(dto.CategoryId))
            {
                return BadRequest("Category name is required!");
            }
            else if(dto.Fields is null || dto.Fields.Count < 1)
            {
                return BadRequest("There must be at least one field!");
            }

            foreach (var field in dto.Fields)
            {
                try
                {
                    var category = categoryService.CreateCategoryField(dto.CategoryId, field.FieldName);

                    if (category is null)
                    {
                        return BadRequest();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Json(dto);
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
                        }).ToList()
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
