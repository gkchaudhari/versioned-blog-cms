using System;
using System.Collections.Generic;
using System.Text;

namespace CmsBackend.Domain.common
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
