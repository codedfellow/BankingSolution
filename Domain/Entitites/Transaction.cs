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
        [ForeignKey(nameof(SenderRef))]
        public Guid SenderId { get; set; }
        [ForeignKey(nameof(ReceiverRef))]
        public Guid ReceiverId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        [MinLength(15), MaxLength(15)]
        public string TransactionRef { get; set; } = string.Empty;
        public User? SenderRef { get; set; }
        public User? ReceiverRef { get; set; }
    }
}
