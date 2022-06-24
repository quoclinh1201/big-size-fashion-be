using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class ProductDetailRequest
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int ColourId { get; set; }
        [Required]
        public List<int> SizeIdList { get; set; }
    }
}
