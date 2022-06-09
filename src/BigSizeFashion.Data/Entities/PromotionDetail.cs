using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class PromotionDetail
    {
        public int ProductId { get; set; }
        public int PromotionId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}
