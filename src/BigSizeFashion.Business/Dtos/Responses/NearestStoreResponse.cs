﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class NearestStoreResponse
    {
        public int StoreId { get; set; }
        public decimal ShippingFee { get; set; }
    }
}
