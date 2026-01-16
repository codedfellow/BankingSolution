using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entitites
{
    public class BaseEntity
    {
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsDeleted { get; set; }
    }
}
