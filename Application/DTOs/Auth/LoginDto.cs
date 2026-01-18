using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public record LoginDto(string? Email, string? Password);
}
