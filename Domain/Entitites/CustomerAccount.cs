using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entitites
{
    public class CustomerAccount : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(UserRef))]
        public Guid UserId { get; set; }
        [MaxLength(10),MinLength(10)]
        public string AccountNumber { get; set; } = string.Empty;
        public decimal AccountBalance { get; set; }
        public string Address { get; set; } = string.Empty;
        public User? UserRef { get; set; }
    }
}
