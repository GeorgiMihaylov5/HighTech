using HighTech.Abstraction;
using HighTech.Common;
using HighTech.DTOs;
using HighTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class ConfiguratorController : Controller
    {
        private readonly IConfiguratorService configuratorService;

        public ConfiguratorController(IConfiguratorService _configuratorService)
        {
            configuratorService = _configuratorService;
        }

        public IActionResult GetCategories()
        {
            try
            {
                var categories = configuratorService.GetPcCategories();
                var dtos = categories.Select(ConvertToCategoryDTO).ToList();
                return Response.ResultSuccess(dtos);
            }
            catch
            {
                return Response.ResultFailure<List<CategoryDTO>>("An unexpected error occurred.", 500);
            }
        }

        public IActionResult GetParts(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return Response.ResultFailure<List<ProductDTO>>("Category name is required.", 400);
            }

            try
            {
                var products = configuratorService.GetPartsInCategory(categoryName);
                var dtos = products.Select(ConvertToProductDTO).ToList();
                return Response.ResultSuccess(dtos);
            }
            catch
            {
                return Response.ResultFailure<List<ProductDTO>>("An unexpected error occurred.", 500);
            }
        }

        [HttpPost]
        public IActionResult GetCompatibleParts([FromQuery] string categoryName, [FromBody] ConfiguratorSelectionDTO dto)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return Response.ResultFailure<List<ProductDTO>>("Category name is required.", 400);
            }

            try
            {
                var products = configuratorService.GetCompatibleParts(categoryName, dto?.Selection ?? new Dictionary<string, string>());
                var dtos = products.Select(ConvertToProductDTO).ToList();
                return Response.ResultSuccess(dtos);
            }
            catch
            {
                return Response.ResultFailure<List<ProductDTO>>("An unexpected error occurred.", 500);
            }
        }

        [HttpPost]
        public IActionResult Validate([FromBody] ConfiguratorSelectionDTO dto)
        {
            if (dto == null)
            {
                return Response.ResultFailure<ConfiguratorValidationResultDTO>("Selection is required.", 400);
            }

            try
            {
                var result = configuratorService.Validate(dto.Selection);
                return Response.ResultSuccess(result);
            }
            catch
            {
                return Response.ResultFailure<ConfiguratorValidationResultDTO>("An unexpected error occurred.", 500);
            }
        }

        private static CategoryDTO ConvertToCategoryDTO(Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Fields = category.CategoryFields?.Select(cf => new FieldDTO
                {
                    Id = cf.Field.Id,
                    Name = cf.Field.Name,
                    TypeCode = cf.Field.TypeCode
                }).ToList() ?? new List<FieldDTO>()
            };
        }

        private static ProductDTO ConvertToProductDTO(Product p)
        {
            var dto = new ProductDTO
            {
                Id = p.Id,
                Manufacturer = p.Manufacturer,
                Model = p.Model,
                Price = p.Price,
                Discount = p.Discount,
                Quantity = p.Quantity,
                Image = p.Image,
                Warranty = p.Warranty,
                Fields = new List<FieldDTO>()
            };

            if (p.ProductFields != null)
            {
                foreach (var pf in p.ProductFields)
                {
                    dto.Fields.Add(new FieldDTO
                    {
                        Id = pf.CategoryField?.Field?.Id,
                        Name = pf.CategoryField?.Field?.Name,
                        TypeCode = pf.CategoryField?.Field?.TypeCode ?? TypeCode.String,
                        Value = pf.Value
                    });
                }
            }

            return dto;
        }
    }
}
