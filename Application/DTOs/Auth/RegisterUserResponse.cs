using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public sealed record RegisterUserResponse(string Message, string Token);
}
