using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class ImportProductRequest
    {
        [Required]
        public string DeliveryNoteName { get; set; }
        [Required]
        public int FromStoreId { get; set; }
        [Required]
        public List<ImportProductItem> ListProducts { get; set; } = new List<ImportProductItem>();
    }
}
