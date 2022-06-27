using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class IncreaseOrDesceaseProductRequest
    {
        [Required]
        public List<PairProductDetailIdAnQuantity> ListProduct { get; set; }
        [Required]
        public bool Action { get; set; }
    }
}
