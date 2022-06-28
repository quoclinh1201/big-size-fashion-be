using BigSizeFashion.Business.Helpers.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Parameters
{
    public class ImportProductParameter : QueryStringParameters
    {
        public string DeliveryNoteName { get; set; }
    }
}
