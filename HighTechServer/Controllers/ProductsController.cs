using HighTech.Core.Entities;
using HighTech.Core.Services;
using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
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

		public ProductsController(IProductService _productService, ICategoryService _categoryService)
		{
			productService = _productService;
			categoryService = _categoryService;
		}

		public IActionResult GetMostSellers()
		{
			try
			{
				var products = productService.GetMostSellers();

				if (products.Count == 0)
				{
					return Json(Array.Empty<ProductDTO>());
				}

				var dtos = products.Select(p => ConvertToProductDTO(p)).ToList();

				return Json(dtos);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{id}")]
		public IActionResult Get(string id)
		{
			if (id is null)
			{
				return BadRequest("Product ID is required!");
			}

			try
			{
				var product = productService.Get(id);

				if (product is null)
				{
					return NotFound($"Product with ID '{id}' not found.");
				}

				return Json(ConvertToProductDTO(product));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		public IActionResult GetAll()
		{
			try
			{
				var products = productService.GetAll();

				if (products.Count == 0)
				{
					return Json(Array.Empty<ProductDTO>());
				}

				var dtos = products.Select(p => ConvertToProductDTO(p)).ToList();

				return Json(dtos);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{categoryId}")]
		public IActionResult GetByCategory(string categoryId)
		{
			try
			{
				var products = productService.GetByCategory(categoryId);

				if (products.Count == 0)
				{
					return Json(Array.Empty<ProductDTO>());
				}

				var dtos = products.Select(p => ConvertToProductDTO(p)).ToList();

				return Json(dtos);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public IActionResult Create(ProductDTO dto)
		{
			if (dto is null)
			{
				return BadRequest("Product is null!");
			}

			if (string.IsNullOrEmpty(dto.CategoryId))
			{
				return BadRequest("Valid Category ID is required!");
			}

			try
			{
				var product = productService.Create(
					dto.Manufacturer, 
					dto.Model, 
					dto.Warranty,
					dto.Price, 
					dto.Discount, 
					dto.Quantity, 
					dto.Image,
					dto.CategoryId);

				if (product is null || product.Id is null)
				{
					return BadRequest("Failed to create product.");
				}

				// Set product field values if provided
				if (dto.Fields is not null && dto.Fields.Count > 0)
				{
					var fieldValues = dto.Fields.ToDictionary(
						f => f.Id, 
						f => f.Value ?? string.Empty);

					productService.SetProductFieldValues(product.Id, fieldValues);
				}

				dto.Id = product.Id;
				return Json(dto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut]
		public IActionResult Edit(ProductDTO dto)
		{
			if (dto is null || string.IsNullOrEmpty(dto.Id))
			{
				return BadRequest("Product ID is required!");
			}

			if (string.IsNullOrEmpty(dto.CategoryId))
			{
				return BadRequest("Valid Category ID is required!");
			}

			try
			{
				var product = productService.Edit(
					dto.Id, 
					dto.Manufacturer, 
					dto.Model, 
					dto.Warranty,
					dto.Price, 
					dto.Discount, 
					dto.Quantity, 
					dto.Image,
					dto.CategoryId);

				if (product is null)
				{
					return NotFound($"Product with ID '{dto.Id}' not found.");
				}

				// Update product field values if provided
				if (dto.Fields is not null && dto.Fields.Count > 0)
				{
					var fieldValues = dto.Fields.ToDictionary(
						f => f.Id, 
						f => f.Value ?? string.Empty);

					productService.SetProductFieldValues(product.Id!, fieldValues);
				}

				return Json(dto);
			}
			catch (Exception ex)
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
				return BadRequest("ID cannot be null!");
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
		[Authorize(Roles = "Administrator")]
		public IActionResult MakeDiscount(DiscountDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Id))
			{
				return BadRequest("Product ID is required!");
			}

			try
			{
				var product = productService.IncreaseDiscount(dto.Id, dto.Percentage);

				if (product is null)
				{
					return NotFound($"Product with ID '{dto.Id}' not found.");
				}

				return Json(ConvertToProductDTO(product));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public IActionResult RemoveDiscount(DiscountDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Id))
			{
				return BadRequest("Product ID is required!");
			}

			try
			{
				var product = productService.RemoveDiscount(dto.Id);

				if (product is null)
				{
					return NotFound($"Product with ID '{dto.Id}' not found.");
				}

				return Json(ConvertToProductDTO(product));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		private ProductDTO ConvertToProductDTO(Product p)
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
				CategoryId = p.CategoryID,
				CategoryName = p.Category?.Name,
				Fields = new List<FieldDTO>()
			};

			// Add product field values
			if (p.ProductFieldValues is not null && p.ProductFieldValues.Count > 0)
			{
				foreach (var pf in p.ProductFieldValues)
				{
					dto.Fields.Add(new FieldDTO()
					{
						Id = pf.Field!.Id,
						Name = pf.Field.Name,
						TypeCode = pf.Field.TypeCode,
						Value = pf.Value
					});
				}
			}

			return dto;
		}
	}
}
