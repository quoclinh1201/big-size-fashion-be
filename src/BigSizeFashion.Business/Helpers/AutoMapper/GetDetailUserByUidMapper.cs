using AutoMapper;
using BigSizeFashion.Business.Enums;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Enums;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.AutoMapper
{
    public class GetDetailUserByUidMapper : Profile
    {
        public GetDetailUserByUidMapper()
        {
            CreateMap<Customer, GetDetailUserByUidResponse>()
                .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.Birthday)))
                .ForMember(d => d.Gender, s => s.MapFrom(s => CustomerGender(s.Gender).ToString()))
                .ForMember(d => d.Status, s => s.MapFrom(s => s.Status == true ? StatusEnum.Active : StatusEnum.Inactive));

            CreateMap<staff, GetDetailUserByUidResponse>()
                .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.Birthday)))
                .ForMember(d => d.Status, s => s.MapFrom(s => s.Status == true ? StatusEnum.Active : StatusEnum.Inactive));

            CreateMap<Admin, GetDetailUserByUidResponse>()
                .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.Birthday)))
                .ForMember(d => d.Status, s => s.MapFrom(s => s.Status == true ? StatusEnum.Active : StatusEnum.Inactive));

            CreateMap<Owner, GetDetailUserByUidResponse>()
                .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.Birthday)))
                .ForMember(d => d.Status, s => s.MapFrom(s => s.Status == true ? StatusEnum.Active : StatusEnum.Inactive));
        }

        private static GenderEnum CustomerGender(bool? gender)
        {
            return gender == true ? GenderEnum.Male : gender == false ? GenderEnum.Female : GenderEnum.Unknow;
        }
    }
}
