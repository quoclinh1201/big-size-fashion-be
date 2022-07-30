using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class CheckWarehouseRequest
    {
        //[Required]
        //public string FromDate { get; set; }
        //[Required]
        //public string ToDate { get; set; }
        [Required]
        public int InventoryNoteId { get; set; }
        [Required]
        public List<CheckWarehouseItem> ListProducts { get; set; }

    }
}
