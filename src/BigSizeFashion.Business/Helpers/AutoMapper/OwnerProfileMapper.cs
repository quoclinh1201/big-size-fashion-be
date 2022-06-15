using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
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
    public class OwnerProfileMapper : Profile
    {
        public OwnerProfileMapper()
        {
            CreateMap<Owner, OwnerProfileResponse>()
                .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.Birthday)));

            CreateMap<UpdateOwnerProfileRequest, Owner>()
                .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertStringToDate(s.Birthday)));
        }
    }
}
