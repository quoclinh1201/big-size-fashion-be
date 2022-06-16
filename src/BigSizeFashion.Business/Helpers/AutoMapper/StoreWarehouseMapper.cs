using AutoMapper;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.AutoMapper
{
    public class StoreWarehouseMapper : Profile
    {
        public StoreWarehouseMapper()
        {
            CreateMap<AddNewProductIntoStoreRequest, StoreWarehouse>();
        }
    }
}
