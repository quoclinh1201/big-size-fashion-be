﻿using BigSizeFashion.Business.Helpers.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Parameters
{
    public class GetListProductInPromotionParameter : QueryStringParameters
    {
        public int PromotionId { get; set; }
        public string ProductName { get; set; }
    }
}
