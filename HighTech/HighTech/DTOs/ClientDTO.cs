namespace HighTech.DTOs
{
    public class ClientDTO
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public UserDTO User { get; set; }
        public string JWT { get; set; }
    }
}
