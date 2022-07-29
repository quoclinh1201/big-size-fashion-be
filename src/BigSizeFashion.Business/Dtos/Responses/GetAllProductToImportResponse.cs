using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class GetAllProductToImportResponse
    {
        public string ProductName { get; set; }
        public string ColourName { get; set; }
        public string SizeName { get; set; }
        public int ProductId { get; set; }
        public int ColourId { get; set; }
        public int SizeId { get; set; }
        public int ProductDeatailId { get; set; }
    }
}
