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
                return Response.ResultFailure<CategoryDTO>("Category name is required!", 400);
            }
            else if (dto.Name.Length > StringLimits.CategoryName)
            {
                return Response.ResultFailure<CategoryDTO>($"Category name cannot be longer than {StringLimits.CategoryName} characters.", 400);
            }
            else if(dto.Fields is null || dto.Fields.Count < 1)
            {
                return Response.ResultFailure<CategoryDTO>("There must be at least one field!", 400);
            }
            else if (HasDuplicateFields(dto))
            {
                return Response.ResultFailure<CategoryDTO>("Category cannot have duplicate fields.", 400);
            }
            else if (categoryService.ExistsByName(dto.Name))
            {
                return Response.ResultFailure<CategoryDTO>("Category name already exists.", 400);
            }

            try
            {
                var category = categoryService.Create(dto.Name, dto.Fields.Select(field => field.Id).ToList());

                if (category is null)
                {
                    return Response.ResultFailure<CategoryDTO>("Failed to create category.", 400);
                }
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<CategoryDTO>("An unexpected error occurred.", 500);
            }

            return Response.ResultSuccess(dto);
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(CategoryDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
            {
                return Response.ResultFailure<CategoryDTO>("Category name is required!", 400);
            }
            else if (dto.Name.Length > StringLimits.CategoryName)
            {
                return Response.ResultFailure<CategoryDTO>($"Category name cannot be longer than {StringLimits.CategoryName} characters.", 400);
            }
            else if (dto.Fields is null || dto.Fields.Count < 1)
            {
                return Response.ResultFailure<CategoryDTO>("There must be at least one field!", 400);
            }
            else if (HasDuplicateFields(dto))
            {
                return Response.ResultFailure<CategoryDTO>("Category cannot have duplicate fields.", 400);
            }
            else if (categoryService.ExistsByName(dto.Name, dto.Id))
            {
                return Response.ResultFailure<CategoryDTO>("Category name already exists.", 400);
            }

            try
            {
                var fieldIds = dto.Fields.Select(field => field.Id).ToList();

                var category = categoryService.Edit(dto.Id, dto.Name, fieldIds);

                if (category is null)
                {
                    return Response.ResultFailure<CategoryDTO>("Failed to edit category.", 400);
                }
            }
            catch (ConfiguratorMetadataException ex)
            {
                return Response.ResultFailure<CategoryDTO>(ex.Message, 400);
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<CategoryDTO>("An unexpected error occurred.", 500);
            }

            return Response.ResultSuccess(dto);
        }

        public IActionResult GetAll()
        {
            var categories = categoryService.GetAll()
                .Select(category => ConvertToCategoryDTO(category));

            return Response.ResultSuccess(categories);
        }

        public IActionResult Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Response.ResultFailure<CategoryDTO>("Id cannot be null.", 400);
            }

            var category = categoryService.Get(id);

            if (category is null)
            {
                return Response.ResultFailure<CategoryDTO>("Category not found.", 404);
            }

            return Response.ResultSuccess(ConvertToCategoryDTO(category));
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult PreviewCategoryRemoval(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Response.ResultFailure<CategoryRemovalPreviewDTO>("Id cannot be null.", 400);
            }

            try
            {
                var preview = categoryService.PreviewCategoryRemoval(id);

                if (preview is null)
                {
                    return Response.ResultFailure<CategoryRemovalPreviewDTO>("Category not found.", 404);
                }

                return Response.ResultSuccess(preview);
            }
            catch (Exception)
            {
                return Response.ResultFailure<CategoryRemovalPreviewDTO>("An unexpected error occurred.", 500);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult PreviewFieldRemoval(string id, [FromQuery] ICollection<string> fieldIds)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Response.ResultFailure<CategoryRemovalPreviewDTO>("Id cannot be null.", 400);
            }

            try
            {
                var removedFieldIds = fieldIds?
                    .Where(fieldId => !string.IsNullOrWhiteSpace(fieldId))
                    .ToList() ?? new List<string>();

                var preview = categoryService.PreviewFieldRemoval(id, removedFieldIds);

                if (preview is null)
                {
                    return Response.ResultFailure<CategoryRemovalPreviewDTO>("Category not found.", 404);
                }

                return Response.ResultSuccess(preview);
            }
            catch (Exception)
            {
                return Response.ResultFailure<CategoryRemovalPreviewDTO>("An unexpected error occurred.", 500);
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
                var removed = categoryService.Remove(id);
                return Response.ResultSuccess(removed);
            }
            catch (ConfiguratorMetadataException ex)
            {
                return Response.ResultFailure<bool>(ex.Message, 400);
            }
            catch (Exception ex)
            {
                return Response.ResultFailure<bool>("An unexpected error occurred.", 500);
            }
        }

        private static bool HasDuplicateFields(CategoryDTO dto)
        {
            return dto.Fields
                .Where(field => !string.IsNullOrWhiteSpace(field.Id))
                .GroupBy(field => field.Id)
                .Any(group => group.Count() > 1);
        }

        private CategoryDTO ConvertToCategoryDTO(Category category)
        {
            return new CategoryDTO()
            {
                Id = category.Id,
                Name = category.Name,
                Fields = category.CategoryFields.Select(categoryField => new FieldDTO()
                {
                    Id = categoryField.Field.Id,
                    Name = categoryField.Field.Name,
                    TypeCode = categoryField.Field.TypeCode,
                    Value = null
                }).ToList()
            };
        }
    }
}

