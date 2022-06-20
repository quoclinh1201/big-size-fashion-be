using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class PromotionRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string PromotionName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Range(1, 100)]
        public byte PromotionValue { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ApplyDate { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ExpiredDate { get; set; }
    }
}
