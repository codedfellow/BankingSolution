using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Account
{
    public record UpdateAccountDto(string? AccountNumber, decimal AccountBalance, string? Address);
}
