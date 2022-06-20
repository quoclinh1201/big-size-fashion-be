using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.RequestObjects
{
    public class ManageProductInCartRequest
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int StoreId { get; set; }
    }
}
