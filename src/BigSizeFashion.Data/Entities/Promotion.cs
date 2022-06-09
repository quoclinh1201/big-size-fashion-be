using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Promotion
    {
        public Promotion()
        {
            PromotionDetails = new HashSet<PromotionDetail>();
        }

        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public byte PromotionValue { get; set; }
        public DateTime ApplyDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<PromotionDetail> PromotionDetails { get; set; }
    }
}
