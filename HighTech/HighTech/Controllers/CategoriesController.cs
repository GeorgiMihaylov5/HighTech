using HighTech.Abstraction;
using HighTech.DTOs;
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
                .GroupBy(c => c.Id)
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Id cannot be a null!");
            }

            try
            {
                var removed = categoryService.RemoveCategories(id);

                return Json(removed);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
