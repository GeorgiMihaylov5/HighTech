using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using HighTech.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class ProductsController : Controller
	{
		private readonly IProductService productService;

		public ProductsController(IProductService _productService)
		{
			productService = _productService;
		}

		public IActionResult GetMostSellers()
		{
			try
			{
				var products = productService.GetMostSellers();
				return Json(ProductMapper.ToDTOList(products));
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

				return Json(ProductMapper.ToDTO(product));
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
				return Json(ProductMapper.ToDTOList(products));
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
				return Json(ProductMapper.ToDTOList(products));
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
				//TODO i dont think be best option is to set a 0 when it is null. Better return a message
				//Use Result pattern for return it will be the best
				var product = productService.Create(
					dto?.Manufacturer,
					dto?.Model,
					dto?.Warranty ?? 0,
					dto?.Price ?? 0,
					dto?.Discount ?? 0,
					dto?.Quantity ?? 0,
					dto?.Image,
					dto?.CategoryId);

				if (product is null || product.Id is null)
				{
					return BadRequest("Failed to create product.");
				}

				// Set product field values if provided
				if (dto?.Fields is not null && dto.Fields.Count > 0)
				{
					var fieldValues = dto.Fields.ToDictionary(
						f => f.Id!,
						f => f.Value);

					productService.SetProductFieldValues(product.Id, fieldValues);
				}

				dto?.Id = product.Id;
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
						f => f.Id!,
						f => f.Value);

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

				return Json(ProductMapper.ToDTO(product));
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

				return Json(ProductMapper.ToDTO(product));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


	}
}
