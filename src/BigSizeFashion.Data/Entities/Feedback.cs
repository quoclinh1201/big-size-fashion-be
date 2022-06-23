using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        public byte? Rate { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}
