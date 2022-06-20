using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.RequestObjects
{
    public class RemoveProductOutOfPromotionnRequest
    {
        public int ProductId { get; set; }
        public int PromotionId { get; set; }
    }
}
