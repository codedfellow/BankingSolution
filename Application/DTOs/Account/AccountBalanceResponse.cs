using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Account
{
    public sealed record AccountBalanceResponse(string AccountNumber, decimal AccountBalance, string Address, string FullName);
}
