using AutoMapper;
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
    public class GetListManagerAccountsMapper : Profile
    {
        public GetListManagerAccountsMapper()
        {
            CreateMap<Account, GetListAccountsResponse>()
                .ForMember(d => d.CreateAt, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.CreateAt)))
                .ForMember(d => d.Status, s => s.MapFrom(s => s.Status == true ? AccountStatusEnum.Active : AccountStatusEnum.Inactive));
        }
    }
}
