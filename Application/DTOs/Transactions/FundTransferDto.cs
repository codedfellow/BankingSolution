using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Transactions
{
    public record FundTransferDto(string? DebitAccountNumber, string? CreditAccountNumber, decimal Amount);
}
