using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Transactions
{
    public record FundAccountDto(string? AccountNumber, decimal Amount, string? Narration);
}
