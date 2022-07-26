using AutoMapper;
using BigSizeFashion.Business.Dtos.Responses;
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
    public class StaffProfileMapper : Profile
    {
        public StaffProfileMapper()
        {
            CreateMap<staff, StaffProfileResponse>()
                .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.Birthday)));

            CreateMap<UpdateStaffProfileRequest, staff>()
                .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertStringToDate(s.Birthday)));

            CreateMap<staff, StaffOfStoreResponse>()
               .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.Birthday)));

            CreateMap<staff, GetListManagerResponse>()
                .ForMember(d => d.StoreName, s => s.MapFrom(s => s.Store.StoreName));
        }
    }
}
