using HighTech.Abstraction;
using HighTech.Common;
using HighTech.DTOs;
using HighTech.Exceptions;
using HighTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductsController : Controller
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly IFieldService fieldService;
        private readonly IReviewService reviewService;
        private readonly IImageService imageService;

        public ProductsController(IProductService _productService,
            ICategoryService _categoryService,
            IFieldService _fieldService,
            IReviewService _reviewService,
            IImageService _imageService)
        {
            productService = _productService;
            categoryService = _categoryService;
            fieldService = _fieldService;
            reviewService = _reviewService;
            imageService = _imageService;
        }

        public IActionResult GetMostSellers()
        {
            try
            {
                var products = productService.GetMostSellers();

                if (products.Count == 0)
                {
                    return Response.ResultSuccess<List<ProductDTO>>(new List<ProductDTO>());
                }

                var dtos = new List<ProductDTO>();

                foreach (var p in products)
                {
                    var categoryName = categoryService.GetCategoryByProduct(p.Id);
                    dtos.Add(ConvertToProductDTO(p, categoryName, false));
                }

                return Response.ResultSuccess(dtos);
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<List<ProductDTO>>("An unexpected error occurred.", 500);
            }
        }

        public IActionResult Get(string id)
        {
            if (id is null)
            {
                return Response.ResultFailure<ProductDTO>("Id cannot be null.", 400);
            }

            var product = productService.Get(id);

            if (product is null)
            {
                return Response.ResultFailure<ProductDTO>("Product not found.", 404);
            }

            var categoryName = categoryService.GetCategoryByProduct(product.Id);

            return Response.ResultSuccess(ConvertToProductDTO(product, categoryName, true));
        }

        public IActionResult GetAll()
        {
            var products = productService.GetAll();

            if (products.Count == 0)
            {
                return Response.ResultSuccess<List<ProductDTO>>(new List<ProductDTO>());
            }

            var dtos = new List<ProductDTO>();

            foreach (var p in products)
            {
                var categoryName = categoryService.GetCategoryByProduct(p.Id);
                dtos.Add(ConvertToProductDTO(p, categoryName, false));
            }

            return Response.ResultSuccess(dtos);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {
            try
            {
                var path = imageService.SaveProductImage(file);
                return Response.ResultSuccess(path);
            }
            catch (InvalidImageException ex)
            {
                return Response.ResultFailure<string>(ex.Message, 400);
            }
            catch (Exception)
            {
                return Response.ResultFailure<string>("An unexpected error occurred.", 500);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult Create(ProductDTO dto)
        {
            if(dto is null)
            {
                return Response.ResultFailure<ProductDTO>("Product is null!", 400);
            }

            var validationErrors = ValidateProduct(dto);
            validationErrors.AddRange(ValidateProductFields(dto));

            if (validationErrors.Count > 0)
            {
                return Response.ResultFailure<ProductDTO>(string.Join(", ", validationErrors), 400);
            }

            try
            {
                var product = productService.Create(dto.Manufacturer, dto.Model, dto.Warranty,
                dto.Price, dto.Discount, dto.Quantity, dto.Image);

                if (product is null || product.Id is null)
                {
                    return Response.ResultFailure<ProductDTO>("Failed to create product.", 400);
                }

                if (dto.Fields is not null)
                {
                    foreach (var field in dto.Fields)
                    {
                        var categoryField = categoryService.GetCategoryField(dto.CategoryName, field.Id);
                        fieldService.AddProductField(product.Id, categoryField.Id, field.Value);
                    }
                }
                dto.Id = product.Id;
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<ProductDTO>("An unexpected error occurred.", 500);
            }

            return Response.ResultSuccess(dto);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut]
        public IActionResult Edit(ProductDTO dto)
        {
            var validationErrors = ValidateProduct(dto);
            validationErrors.AddRange(ValidateProductFields(dto));

            if (validationErrors.Count > 0)
            {
                return Response.ResultFailure<ProductDTO>(string.Join(", ", validationErrors), 400);
            }

            var oldImage = productService.Get(dto.Id)?.Image;

            var product = productService.Edit(dto.Id, dto.Manufacturer, dto.Model, dto.Warranty,
                dto.Price, dto.Discount, dto.Quantity, dto.Image);

            if (product is null || product.Id is null)
            {
                return Response.ResultFailure<ProductDTO>("Failed to edit product.", 400);
            }

            if (!string.IsNullOrEmpty(oldImage) && oldImage != product.Image)
            {
                imageService.DeleteProductImage(oldImage);
            }

            if (dto.Fields.Count == 0)
            {
                return Response.ResultSuccess(dto);
            }

            try
            {
                fieldService.RemoveProductFieldsExceptCategory(dto.Id, dto.CategoryName);

                var productFields = fieldService.GetProductFields(dto.Id)
                    .Where(pf => pf.CategoryField.Category.Name == dto.CategoryName)
                    .ToList();

                foreach (var field in dto.Fields)
                {
                    var categoryField = categoryService.GetCategoryField(dto.CategoryName, field.Id);

                    if (categoryField is null)
                    {
                        return Response.ResultFailure<ProductDTO>("Product field does not belong to the selected category.", 400);
                    }

                    var productField = productFields
                        .FirstOrDefault(pf => pf.CategoryField.FieldId == field.Id);

                    if (productField is null)
                    {
                        fieldService.AddProductField(dto.Id, categoryField.Id, field.Value);
                    }
                    else
                    {
                        fieldService.EditProductFieldValue(productField.Id, categoryField.Id, field.Value);
                    }
                }

                return Response.ResultSuccess(dto);
            }
            catch(Exception ex)
            {
                return Response.ResultFailure<ProductDTO>("An unexpected error occurred.", 500);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Response.ResultFailure<bool>("Id cannot be null!", 400);
            }

            try
            {
                var removed = productService.Remove(id);
                return Response.ResultSuccess(removed);
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<bool>("An unexpected error occurred.", 500);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult MakeDiscount(DiscountDTO dto)
        {
            try
            {
                var product = productService.IncreaseDiscount(dto.Id, dto.Percentage);

                if (product is null)
                {
                    return Response.ResultFailure<ProductDTO>("Product not found.", 404);
                }

                return Response.ResultSuccess(ConvertToProductDTO(product, null, false));
            }
            catch(Exception ex)
            {
                return Response.ResultFailure<ProductDTO>("An unexpected error occurred.", 500);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult RemoveDiscount(DiscountDTO dto)
        {
            try
            {
                return Response.ResultSuccess(ConvertToProductDTO(productService.RemoveDiscount(dto.Id), null, false));
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<ProductDTO>("An unexpected error occurred.", 500);
            }
        }

        private ProductDTO ConvertToProductDTO(Product p, string categoryName, bool includeReviews)
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
                Fields = new List<FieldDTO>(),
                AverageRating = reviewService.GetAverageRating(p.Id),
                ReviewCount = reviewService.GetReviewCount(p.Id),
                Reviews = new List<ReviewDTO>()
            };

            var productFields = fieldService.GetProductFields(p.Id);
            var productFieldValues = productFields
                .GroupBy(pf => pf.CategoryField.FieldId)
                .ToDictionary(group => group.Key, group => group.First().Value);

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                var category = categoryService.GetByName(categoryName);

                if (category is not null)
                {
                    foreach (var categoryField in category.CategoryFields)
                    {
                        dto.Fields.Add(new FieldDTO()
                        {
                            Id = categoryField.FieldId,
                            Name = categoryField.Field.Name,
                            TypeCode = categoryField.Field.TypeCode,
                            Value = productFieldValues.TryGetValue(categoryField.FieldId, out var value)
                                ? value
                                : null
                        });
                    }
                }
            }

            if (dto.Fields.Count == 0 && productFields.Count > 0)
            {
                foreach (var pf in productFields)
                {
                    dto.Fields.Add(new FieldDTO()
                    {
                        Id = pf.CategoryField.FieldId,
                        Name = pf.CategoryField.Field.Name,
                        TypeCode = pf.CategoryField.Field.TypeCode,
                        Value = pf.Value
                    });
                }
            }

            if (includeReviews)
            {
                dto.Reviews = reviewService.GetByProduct(p.Id)
                    .Select(review => new ReviewDTO()
                    {
                        Id = review.Id,
                        ProductId = review.ProductId,
                        UserId = review.UserId,
                        Username = review.User?.UserName,
                        FirstName = review.User?.FirstName,
                        LastName = review.User?.LastName,
                        Rating = review.Rating,
                        Comment = review.Comment,
                        CreatedOn = review.CreatedOn,
                        UpdatedOn = review.UpdatedOn
                    })
                    .ToList();
            }

            return dto;
        }

        private static List<string> ValidateProduct(ProductDTO dto)
        {
            var errors = new List<string>();

            if (dto is null)
            {
                errors.Add("Product is required.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(dto.Manufacturer))
            {
                errors.Add("Manufacturer is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Model))
            {
                errors.Add("Model is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Image))
            {
                errors.Add("Image is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                errors.Add("Category name is required.");
            }

            if (dto.Price < 0)
            {
                errors.Add("Price cannot be negative.");
            }

            if (dto.Warranty < 0)
            {
                errors.Add("Warranty cannot be negative.");
            }

            if (dto.Quantity < 0)
            {
                errors.Add("Quantity cannot be negative.");
            }

            if (dto.Discount < 0)
            {
                errors.Add("Discount cannot be negative.");
            }

            StringLimits.AddMaxLengthError(errors, "Manufacturer", dto.Manufacturer, StringLimits.ProductManufacturer);
            StringLimits.AddMaxLengthError(errors, "Model", dto.Model, StringLimits.ProductModel);
            StringLimits.AddMaxLengthError(errors, "Image", dto.Image, StringLimits.ProductImage);
            StringLimits.AddMaxLengthError(errors, "Category name", dto.CategoryName, StringLimits.CategoryName);

            foreach (var field in dto.Fields ?? new List<FieldDTO>())
            {
                StringLimits.AddMaxLengthError(errors, "Product field value", field.Value, StringLimits.ProductFieldValue);
            }

            return errors;
        }

        // Validates each spec value against its field's authoritative TypeCode (from the DB,
        // not the client-supplied dto.TypeCode), and confirms the field belongs to the category.
        private List<string> ValidateProductFields(ProductDTO dto)
        {
            var errors = new List<string>();

            if (dto?.Fields is null)
            {
                return errors;
            }

            foreach (var field in dto.Fields)
            {
                var categoryField = categoryService.GetCategoryField(dto.CategoryName, field.Id);

                if (categoryField is null)
                {
                    errors.Add($"Field '{field.Name}' does not belong to category '{dto.CategoryName}'.");
                    continue;
                }

                FieldValueValidator.AddTypeError(errors, categoryField.Field.Name, categoryField.Field.TypeCode, field.Value);
            }

            return errors;
        }
    }
}
