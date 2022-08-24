using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class GetAvailableStoreResponse
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string Distance { get; set; }
    }
}
