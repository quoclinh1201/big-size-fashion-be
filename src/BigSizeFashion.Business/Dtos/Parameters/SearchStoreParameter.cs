using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Parameters
{
    public class SearchStoreParameter
    {
        public string StoreAddress { get; set; }
        public bool? IsMainWarehouse { get; set; }
        public bool? Status { get; set; }
    }
}
