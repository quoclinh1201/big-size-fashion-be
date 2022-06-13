using BigSizeFashion.Business.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.ResponseObjects
{
    public class GetListAccountsResponse
    {
        public int Uid { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string CreateAt { get; set; }
        public StatusEnum Status { get; set; }
    }
}
