using AutoMapper;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.AutoMapper
{
    public class OrderDetailMapper : Profile
    {
        public OrderDetailMapper()
        {
            CreateMap<OrderDetail, OrderDetailItem>();
                //.ForMember(d => d.Price, s => s.MapFrom(s => FormatMoney.FormatPrice(s.Price)))
                //.ForMember(d => d.DiscountPrice, s => s.MapFrom(s => FormatMoney.FormatPrice(s.DiscountPrice)));
        }
    }
}
