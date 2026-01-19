using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Transactions
{
    public record TransactionHistoryDto
    {
        public string DebitAccountNumber { get; set; } = string.Empty;
        public string CreditAccountNumber { get; set; } = string.Empty;
        public string DebitAccountOwner { get; set; } = string.Empty;
        public string CreditAccountOwner { get; set; } = string.Empty;
        public DateTime TransactionDateUtc { get; set; }
        public decimal Amount { get; set; }
        public string TransactionRef { get; set; } = string.Empty;
        public string Narration { get; set; } = string.Empty;
    }
}
