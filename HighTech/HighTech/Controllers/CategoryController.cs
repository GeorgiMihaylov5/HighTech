using HighTech.Abstraction;
using HighTech.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }

        public IActionResult Create(CategoryDTO category)
        {
            return View();
        }
    }
}
