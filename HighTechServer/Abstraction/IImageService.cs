using Microsoft.AspNetCore.Http;

namespace HighTech.Abstraction
{
    public interface IImageService
    {
        // Saves an uploaded product image to wwwroot and returns its public web path
        // (e.g. "/uploads/products/<guid>.jpg"). Throws InvalidImageException on a rule violation.
        public string SaveProductImage(IFormFile file);

        // Best-effort deletion of a previously uploaded product image given its public web path.
        // Only removes files inside the configured upload folder; external URLs are ignored.
        public void DeleteProductImage(string webPath);
    }
}
