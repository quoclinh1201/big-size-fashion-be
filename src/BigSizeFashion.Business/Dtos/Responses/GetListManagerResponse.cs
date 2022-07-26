using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class GetListManagerResponse
    {
        public int Uid { get; set; }
        public string Fullname { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
    }
}
