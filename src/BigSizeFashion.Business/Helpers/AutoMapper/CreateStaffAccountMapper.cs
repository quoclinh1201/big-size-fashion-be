using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Enums;
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
    public class CreateStaffAccountMapper : Profile
    {
        public CreateStaffAccountMapper()
        {
            CreateMap<CreateStaffAccountRequest, Account>()
                .ForMember(d => d.CreateAt, s => s.MapFrom(s => DateTime.UtcNow.AddHours(7)))
                .ForMember(d => d.Status, s => s.MapFrom(s => true));

            CreateMap<CreateStaffAccountRequest, staff>()
                .ForMember(d => d.Status, s => s.MapFrom(s => true));

            CreateMap<Account, AccountResponse>()
                .ForMember(d => d.CreateAt, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.CreateAt)))
                .ForMember(d => d.Status, s => s.MapFrom(s => s.Status == true ? StatusEnum.Active : StatusEnum.Inactive));
        }
    }
}
