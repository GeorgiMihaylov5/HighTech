﻿namespace HighTech.DTOs
{
    public class EmployeeDTO
    {
        public string Id { get; set; }
        public UserDTO User { get; set; }
        public string JobTitle { get; set; }
        public bool IsAdmin { get; set; }
    }
}
