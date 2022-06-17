using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class ValidateTimeOfPromotionRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string ApplyDate { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ExpiredDate { get; set; }
    }
}
