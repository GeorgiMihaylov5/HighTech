using HighTech.Abstraction;
using HighTech.DTOs;
using HighTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductsController : Controller
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly IFieldService fieldService;

        public ProductsController(IProductService _productService,
            ICategoryService _categoryService,
            IFieldService _fieldService)
        {
            productService = _productService;
            categoryService = _categoryService;
            fieldService = _fieldService;
        }

        //public override JsonResult Json(object data)
        //{
        //    var settings = new JsonSerializerOptions
        //    {
        //    };

        //    return base.Json(data, settings);
        //}

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

            var categoryName = categoryService.GetCategoryByProduct(product.Id);

            return Json(ConvertToProductDTO(product, categoryName));
        }

        public IActionResult GetAll()
        {
            var products = productService.GetAll();

            if (products.Count == 0)
            {
                return Json(Array.Empty<ProductDTO>());
            }

            var dtos = new List<ProductDTO>();

            foreach (var p in products)
            {
                var categoryName = categoryService.GetCategoryByProduct(p.Id);

                dtos.Add(ConvertToProductDTO(p, categoryName));
            }

            return Json(dtos);
        }

        [HttpPost]
        public IActionResult Create(ProductDTO dto)
        {
            if(dto is null)
            {
                return BadRequest("Product is null!");
            }

            try
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
                        var category = categoryService.Get(dto.CategoryName, field.Id);

                        fieldService.AddProductField(product.Id, category.Id, field.Value);
                    }
                }
                dto.Id = product.Id;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            

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

            try
            {
                var productFields = fieldService.GetProductFields(dto.Id).OrderBy(pf => pf.Category.Field.Name).ToList();
                var dtoFileds = dto.Fields.OrderBy(pf => pf.Name).ToList();

                for (int i = 0; i < productFields.Count; i++)
                {
                    var category = categoryService.Get(dto.CategoryName, dtoFileds[i].Id);
                    fieldService.EditProductFieldValue(productFields[i].Id, category.Id, dtoFileds[i].Value);
                }

                var added = dtoFileds.Count - productFields.Count;
                if (added > 0)
                {
                    for (int i = 0; i < added; i++)
                    {
                        var category = categoryService.Get(dto.CategoryName, dtoFileds[i].Id);
                        fieldService.AddProductField(dto.Id, category.Id, dtoFileds[productFields.Count + i].Value);
                    }
                }

                return Json(dto);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
                var removed = productService.Remove(id);

                return Json(removed);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult MakeDiscount(DiscountDTO dto)
        {
            try
            {
                var product = productService.IncreaseDiscount(dto.Id, dto.Percentage);

                if (product is null)
                {
                    throw new InvalidOperationException();
                }

                return Json(ConvertToProductDTO(product, null));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult RemoveDiscount(DiscountDTO dto)
        {
            try
            {
                return Json(ConvertToProductDTO(productService.RemoveDiscount(dto.Id), null));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

            var productFields = fieldService.GetProductFields(p.Id);

            if (productFields.Count > 0)
            {
                foreach (var pf in productFields)
                {
                    dto.Fields.Add(new FieldDTO()
                    {
                        Id = pf.Category.FieldId,
                        Name = pf.Category.Field.Name,
                        TypeCode = pf.Category.Field.TypeCode,
                        Value = pf.Value
                    });
                }
            }

            return dto;
        }
    }
}
