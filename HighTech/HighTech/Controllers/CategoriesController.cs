using HighTech.Abstraction;
using HighTech.DTOs;
using HighTech.Models;
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

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(CategoryDTO dto)
        {
            if(string.IsNullOrEmpty(dto.Name))
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
                    var category = categoryService.CreateCategoryField(dto.Name, field.Id);

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

        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(CategoryDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
            {
                return BadRequest("Category name is required!");
            }
            else if (dto.Fields is null || dto.Fields.Count < 1)
            {
                return BadRequest("There must be at least one field!");
            }

            var category = categoryService.EditCategoryName(dto.Id, dto.Name);

            //skip the foreach, if fields are not changed
            var oldNames = category.Select(x => x.Field.Name).ToArray();
            var names = dto.Fields.Select(x => x.Name).ToArray();

            if (oldNames.OrderBy(x => x).SequenceEqual(names.OrderBy(x => x)))
            {
                return Json(dto);
            }

            var removed = categoryService.RemoveCategoryByName(dto.Name);

            foreach (var field in dto.Fields)
            {
                try
                {
                    categoryService.CreateCategoryField(dto.Name, field.Id);
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
                .GroupBy(c => new { c.Id, c.Name })
                .Select(group => new CategoryDTO()
                    {
                        Id = group.Key.Id,
                        Name = group.Key.Name,
                        Fields = group.Select(c => new FieldDTO()
                        {
                            Id = c.Field.Id,
                            Name = c.Field.Name,
                            TypeCode = c.Field.TypeCode,
                            Value = null
                        }).ToList()
                    }
                );

            return Json(categories);
        }

        [HttpDelete("{name}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Id cannot be a null!");
            }

            try
            {
                var removed = categoryService.RemoveCategoryByName(name);

                return Json(removed);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
