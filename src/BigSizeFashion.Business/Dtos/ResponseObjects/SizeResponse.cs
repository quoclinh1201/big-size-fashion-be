using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.ResponseObjects
{
    public class SizeResponse
    {
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public bool Status { get; set; }
    }
}
