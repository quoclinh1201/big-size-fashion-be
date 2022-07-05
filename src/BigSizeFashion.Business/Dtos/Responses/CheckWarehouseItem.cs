using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class CheckWarehouseItem
    {
        public int productId { get; set; }
        public string ProductName { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public int ColourId { get; set; }
        public string ColourName { get; set; }
        public int BeginningQuantity { get; set; }
        public int EndingQuantityInSystem { get; set; }
        public int RealQuantity { get; set; }
        public int DifferenceQuantity { get; set; }
    }
}
