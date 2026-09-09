namespace HighTech.DTOs
{
    public class UpsertReviewDTO
    {
        public string ProductId { get; set; }
        public string Username { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
