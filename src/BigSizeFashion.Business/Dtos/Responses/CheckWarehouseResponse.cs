using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class CheckWarehouseResponse
    {
        //public string FromDate { get; set; }
        //public string ToDate { get; set; }
        public List<CheckWarehouseItem> ListProducts { get; set; } = new List<CheckWarehouseItem>();
    }
}
