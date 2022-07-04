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
    public class PromotionMapper : Profile
    {
        public PromotionMapper()
        {
            CreateMap<Promotion, PromotionResponse>()
                .ForMember(d => d.PromotionValue, s => s.MapFrom(s => s.PromotionValue))
                .ForMember(d => d.ApplyDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.ApplyDate)))
                .ForMember(d => d.ExpiredDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.ExpiredDate)));

            CreateMap<PromotionRequest, Promotion>()
                .ForMember(d => d.ApplyDate, s => s.MapFrom(s => ConvertDateTime.ConvertStringToDate(s.ApplyDate)))
                .ForMember(d => d.ExpiredDate, s => s.MapFrom(s => ConvertDateTime.ConvertStringToDate(s.ExpiredDate)))
                .ForMember(d => d.Status, s => s.MapFrom(s => true));
        }
    }
}
