using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class AddressRequest
    {
        [Required]
        [MaxLength(50)]
        public string ReceiverName { get; set; }

        [Required]
        [MaxLength(200)]
        public string ReceiveAddress { get; set; }

        [Required]
        [MaxLength(10)]
        [MinLength(10)]
        public string ReceiverPhone { get; set; }
    }
}
