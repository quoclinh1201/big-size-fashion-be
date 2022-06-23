using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class CreateOrderForCustomerRequest
    {
        [Required]
        [RegularExpression(@"[0]{1}[0-9]{9}")]
        [MaxLength(10)]
        [MinLength(10)]
        public string CustomerPhoneNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        public List<ProductInOrder> ListProduct { get; set; }
    }
}
