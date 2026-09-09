using HighTech.Abstraction;
using HighTech.Exceptions;
using HighTech.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HighTech.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment environment;
        private readonly ProductOptions options;

        public ImageService(IWebHostEnvironment _environment, IOptions<ProductOptions> _options)
        {
            environment = _environment;
            options = _options.Value;
        }

        public string SaveProductImage(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                throw new InvalidImageException("No image file was provided.");
            }

            if (file.Length > options.MaxImageSizeBytes)
            {
                var maxMb = options.MaxImageSizeBytes / 1_000_000.0;
                throw new InvalidImageException($"Image is too large. The maximum allowed size is {maxMb:0.#} MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) ||
                !options.AllowedImageExtensions.Contains(extension))
            {
                var allowed = string.Join(", ", options.AllowedImageExtensions);
                throw new InvalidImageException($"Unsupported image type. Allowed types: {allowed}.");
            }

            var webRoot = environment.WebRootPath
                ?? Path.Combine(environment.ContentRootPath, "wwwroot");

            var targetFolder = Path.Combine(webRoot, options.ImageUploadFolder);
            Directory.CreateDirectory(targetFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Public web path with forward slashes, regardless of OS path separator.
            return $"/{options.ImageUploadFolder.Replace('\\', '/')}/{fileName}";
        }

        public void DeleteProductImage(string webPath)
        {
            if (string.IsNullOrWhiteSpace(webPath))
            {
                return;
            }

            var folderPrefix = $"/{options.ImageUploadFolder.Replace('\\', '/')}/";

            // Only touch files we manage; leave external URLs / legacy values alone.
            if (!webPath.StartsWith(folderPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var webRoot = environment.WebRootPath
                ?? Path.Combine(environment.ContentRootPath, "wwwroot");

            var relativePath = webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRoot, relativePath);

            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
                // Best-effort cleanup: never fail the request because an old file could not be deleted.
            }
        }
    }
}
