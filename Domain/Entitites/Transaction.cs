using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entitites
{
    public class Transaction : BaseEntity
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(DebitAccountRef))]
        public Guid DebitAccountId { get; set; }
        [ForeignKey(nameof(CreditAccountRef))]
        public Guid CreditAccountId { get; set; }
        public decimal Amount { get; set; }
        [MinLength(15), MaxLength(15)]
        public string TransactionRef { get; set; } = string.Empty;
        public CustomerAccount? DebitAccountRef { get; set; }
        public CustomerAccount? CreditAccountRef { get; set; }
    }
}
