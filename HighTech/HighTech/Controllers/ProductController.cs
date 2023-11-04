using HighTech.Abstraction;
using HighTech.DTOs;
using HighTech.Models;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;

        public ProductController(IProductService _productService,
            ICategoryService _categoryService)
        {
            productService = _productService;
            categoryService = _categoryService;
        }

        public IActionResult Get(string id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var product = productService.Get(id);

            if (product is null)
            {
                return Json(product);
            }

            var category = categoryService.GetCategoryByProduct(product.Id);

            return Json(ConvertToProductDTO(product, category.Id));
        }

        public IActionResult GetAll()
        {
            var products = productService.GetAll();

            if (products.Count == 0)
            {
                return Json(new ProductDTO[0]);
            }

            var category = categoryService.GetCategoryByProduct(products.First().Id);

            var dtos = new List<ProductDTO>();

            foreach (var p in products)
            {
                dtos.Add(ConvertToProductDTO(p, category.Id));
            }

            return Json(dtos);
        }

        [HttpPost]
        public IActionResult Create(ProductDTO dto)
        {
            var product = productService.Create(dto.Manufacturer, dto.Model, dto.Warranty,
                dto.Price, dto.Discount, dto.Quantity, dto.Image);

            if (product is null || product.Id is null)
            {
                return BadRequest();
            }

            if (dto.Fields is not null)
            {
                foreach (var field in dto.Fields)
                {
                    productService.AddProductField(product.Id, field.FieldName, field.Value);
                }
            }
            dto.Id = product.Id;

            return Json(dto);
        }

        [HttpPut]
        public IActionResult Edit(ProductDTO dto)
        {
            var product = productService.Edit(dto.Id, dto.Manufacturer, dto.Model, dto.Warranty,
                dto.Price, dto.Discount, dto.Quantity, dto.Image);

            if (product is null || product.Id is null)
            {
                return BadRequest();
            }

            if (dto.Fields.Count == 0)
            {
                return Json(dto);
            }

            var productFields = productService.GetProductFields(dto.Id).OrderBy(pf => pf.FieldId).ToList();
            var dtoValues = dto.Fields.OrderBy(pf => pf.FieldName).Select(f => f.Value).ToList();

            for (int i = 0; i < productFields.Count; i++)
            {
                if (productFields[i].Value != dtoValues[i])
                {
                    productService.EditProductFieldValue(productFields[i].Id, dtoValues[i]);
                }
            }

            return Json(dto);
        }

        [HttpDelete]
        public IActionResult Remove(ProductDTO product)
        {
            if (product is null || product.Id is null)
            {
                return BadRequest();
            }

            return Json(productService.Remove(product.Id));
        }

        private ProductDTO ConvertToProductDTO(Product p, string categoryName)
        {
            var dto = new ProductDTO()
            {
                Id = p.Id,
                Manufacturer = p.Manufacturer,
                Model = p.Model,
                Price = p.Price,
                Warranty = p.Warranty,
                Discount = p.Discount,
                Image = p.Image,
                Quantity = p.Quantity,
                CategoryName = categoryName,
                Fields = new List<FieldDTO>()
            };

            var productFields = productService.GetProductFields(p.Id);

            if (productFields.Count > 0)
            {
                foreach (var pf in productFields)
                {
                    dto.Fields.Add(new FieldDTO()
                    {
                        FieldName = pf.FieldId,
                        TypeCode = pf.Field.TypeCode,
                        Value = pf.Value
                    });
                }
            }

            return dto;
        }
    }
}
