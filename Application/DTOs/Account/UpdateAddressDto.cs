using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Account
{
    public record UpdateAddressDto(string? AccountNumber, string? Address);
}
