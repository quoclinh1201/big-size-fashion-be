using AutoMapper;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.AutoMapper
{
    public class StoreMapper : Profile
    {
        public StoreMapper()
        {
            CreateMap<Store, StoreResponse>();

            CreateMap<CreateStoreRequest, Store>()
                .ForMember(d => d.Status, s => s.MapFrom(s => true));
        }
    }
}
