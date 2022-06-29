using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.ResponseObjects
{
    public class StoreResponse
    {
        public int StoreId { get; set; }
        public string ManagerName { get; set; }
        public string StoreAddress { get; set; }
        public string StorePhone { get; set; }
        public bool Status { get; set; }
    }
}
