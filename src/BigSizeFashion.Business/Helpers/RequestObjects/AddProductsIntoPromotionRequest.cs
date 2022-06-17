using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class AddProductsIntoPromotionRequest
    {
        [Required]
        public int PromotionId { get; set; }
        public List<int> ListProductId { get; set; }
    }
}
