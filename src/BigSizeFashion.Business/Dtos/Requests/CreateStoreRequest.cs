using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class CreateStoreRequest
    {
        [Required]
        [MaxLength(200)]
        public string StoreName { get; set; }

        [Required]
        [MaxLength(200)]
        public string StoreAddress { get; set; }

        [Required]
        [RegularExpression(@"[0]{1}[0-9]{9}")]
        [MaxLength(10)]
        [MinLength(10)]
        public string StorePhone { get; set; }

        [Required]
        public bool IsMainWarehouse { get; set; }
    }
}
