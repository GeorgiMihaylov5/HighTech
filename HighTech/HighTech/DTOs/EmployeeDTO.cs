namespace HighTech.DTOs
{
    public class EmployeeDTO: UserDTO
    {
        public string Id { get; set; }
        public string JobTitle { get; set; }
        public bool IsAdmin { get; set; }
    }
}
