using AutoMapper;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.AutoMapper
{
    public class AddressMapper : Profile
    {
        public AddressMapper()
        {
            CreateMap<Address, DeliveryAddressResponse>();

            CreateMap<Address, AddressResponse>();
            CreateMap<AddressRequest, Address> ()
                .ForMember(d => d.Status, s => s.MapFrom(s => true));
        }
    }
}
