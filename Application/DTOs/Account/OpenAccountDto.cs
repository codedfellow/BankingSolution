using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Account
{
    public record class OpenAccountDto(Guid? UserId, string? Address);
}
