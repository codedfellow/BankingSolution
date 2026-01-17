using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class RegisterUserDto
    {
        public string? FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
    }
}
