using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public record class LoginResponse(string Message, string Token);
}
