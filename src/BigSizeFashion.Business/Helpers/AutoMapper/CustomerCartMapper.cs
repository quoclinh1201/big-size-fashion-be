using AutoMapper;
using BigSizeFashion.Business.Dtos.RequestObjects;
using BigSizeFashion.Business.Dtos.ResponseObjects;
using BigSizeFashion.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.AutoMapper
{
    public class CustomerCartMapper : Profile
    {
        public CustomerCartMapper()
        {
            CreateMap<CustomerCart, CartItemResponse>();
        }
    }
}
