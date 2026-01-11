using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using HighTech.Mappers;
using HighTechServer;
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
				return Response.Success(ProductMapper.ToDTOList(products));
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.UnknownError, 400);
			}
		}

		[HttpGet("{id}")]
		public IActionResult Get(string id)
		{
			if (id is null)
			{
				return Response.Error("Product ID is required!", ErrorCode.ProductIdMissing, 400);
			}

			try
			{
				var product = productService.Get(id);

				if (product is null)
				{
					return Response.Error($"Product with ID '{id}' not found.", ErrorCode.ProductNotFound, 404);
				}

				return Response.Success(ProductMapper.ToDTO(product));
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.UnknownError, 400);
			}
		}

		public IActionResult GetAll()
		{
			try
			{
				var products = productService.GetAll();
				return Response.Success(ProductMapper.ToDTOList(products));
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.UnknownError, 400);
			}
		}

		[HttpGet("{categoryId}")]
		public IActionResult GetByCategory(string categoryId)
		{
			try
			{
				var products = productService.GetByCategory(categoryId);
				return Response.Success(ProductMapper.ToDTOList(products));
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.UnknownError, 400);
			}
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public IActionResult Create(ProductDTO dto)
		{
			if (dto is null)
			{
				return Response.Error("Product is null!", ErrorCode.InvalidRequest, 400);
			}

			if (string.IsNullOrEmpty(dto.CategoryId))
			{
				return Response.Error("Valid Category ID is required!", ErrorCode.ProductCategoryIdMissing, 400);
			}

			try
			{
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
					return Response.Error("Failed to create product.", ErrorCode.ProductCreateError, 400);
				}

				// Set product field values if provided
				if (dto?.Fields is not null && dto.Fields.Count > 0)
				{
					var fieldValues = dto.Fields.ToDictionary(
						f => f.Id!,
						f => f.Value);

					productService.SetProductFieldValues(product.Id, fieldValues);
				}

				dto!.Id = product.Id;
				return Response.Success(dto, 201);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.ProductCreateError, 400);
			}
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut]
		public IActionResult Edit(ProductDTO dto)
		{
			if (dto is null || string.IsNullOrEmpty(dto.Id))
			{
				return Response.Error("Product ID is required!", ErrorCode.ProductIdMissing, 400);
			}

			if (string.IsNullOrEmpty(dto.CategoryId))
			{
				return Response.Error("Valid Category ID is required!", ErrorCode.ProductCategoryIdMissing, 400);
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
					return Response.Error($"Product with ID '{dto.Id}' not found.", ErrorCode.ProductNotFound, 404);
				}

				// Update product field values if provided
				if (dto.Fields is not null && dto.Fields.Count > 0)
				{
					var fieldValues = dto.Fields.ToDictionary(
						f => f.Id!,
						f => f.Value);

					productService.SetProductFieldValues(product.Id!, fieldValues);
				}

				return Response.Success(dto);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.ProductUpdateError, 400);
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Administrator")]
		public IActionResult Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return Response.Error("ID cannot be null!", ErrorCode.ProductIdMissing, 400);
			}

			try
			{
				var removed = productService.Remove(id);
				return Response.Success(removed);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.ProductDeleteError, 400);
			}
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public IActionResult MakeDiscount(DiscountDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Id))
			{
				return Response.Error("Product ID is required!", ErrorCode.ProductIdMissing, 400);
			}

			try
			{
				var product = productService.IncreaseDiscount(dto.Id, dto.Percentage);

				if (product is null)
				{
					return Response.Error($"Product with ID '{dto.Id}' not found.", ErrorCode.ProductNotFound, 404);
				}

				return Response.Success(ProductMapper.ToDTO(product));
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.ProductDiscountError, 400);
			}
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public IActionResult RemoveDiscount(DiscountDTO dto)
		{
			if (string.IsNullOrEmpty(dto.Id))
			{
				return Response.Error("Product ID is required!", ErrorCode.ProductIdMissing, 400);
			}

			try
			{
				var product = productService.RemoveDiscount(dto.Id);

				if (product is null)
				{
					return Response.Error($"Product with ID '{dto.Id}' not found.", ErrorCode.ProductNotFound, 404);
				}

				return Response.Success(ProductMapper.ToDTO(product));
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.ProductDiscountError, 400);
			}
		}
	}
}
