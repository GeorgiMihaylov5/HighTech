namespace HighTech.Options
{
    public class ProductOptions
    {
        public int MostSellersCount { get; set; } = 10;

        // Image upload settings (relative to wwwroot).
        public string ImageUploadFolder { get; set; } = "uploads/products";

        public string[] AllowedImageExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

        public long MaxImageSizeBytes { get; set; } = 5_000_000;
    }
}
