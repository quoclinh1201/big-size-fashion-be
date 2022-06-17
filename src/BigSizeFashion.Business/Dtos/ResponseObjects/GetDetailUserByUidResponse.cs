using BigSizeFashion.Business.Enums;
using BigSizeFashion.Business.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.ResponseObjects
{
    public class GetDetailUserByUidResponse
    {
        public int Uid { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Birthday { get; set; }
        public GenderEnum? Gender { get; set; }
        public string Role { get; set; }
        public StatusEnum Status { get; set; }
    }
}
