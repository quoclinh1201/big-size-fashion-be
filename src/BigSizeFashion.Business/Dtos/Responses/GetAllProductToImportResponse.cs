using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class GetAllProductToImportResponse
    {
        public string Product { get; set; }
        public int ProductId { get; set; }
        public int ColourId { get; set; }
        public int SizeId { get; set; }
    }
}
