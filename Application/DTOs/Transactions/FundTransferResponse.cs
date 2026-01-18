using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Transactions
{
    public record FundTransferResponse(string Messaage, string TransactionReference);
}
