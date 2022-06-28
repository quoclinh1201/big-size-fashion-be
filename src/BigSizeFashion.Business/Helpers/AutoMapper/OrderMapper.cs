using AutoMapper;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Enums;
using BigSizeFashion.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.AutoMapper
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<Order, GetOrderDetailResponse>()
                .ForMember(d => d.CustomerName, s => s.MapFrom(s => s.Customer.Fullname))
                .ForMember(d => d.StaffName, s => s.MapFrom(s => s.Staff.Fullname))
                .ForMember(d => d.OrderType, s => s.MapFrom(s => s.OrderType == true ? "Online" : "Offline"))
                //.ForMember(d => d.TotalPrice, s => s.MapFrom(s => FormatMoney.FormatPrice(s.TotalPrice)))
                //.ForMember(d => d.TotalPriceAfterDiscount, s => s.MapFrom(s => FormatMoney.FormatPrice(s.TotalPriceAfterDiscount)))
                .ForMember(d => d.CreateDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.CreateDate)))
                .ForMember(d => d.ApprovalDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.ApprovalDate)))
                .ForMember(d => d.PackagedDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.PackagedDate)))
                .ForMember(d => d.DeliveryDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.DeliveryDate)))
                .ForMember(d => d.ReceivedDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.ReceivedDate)))
                .ForMember(d => d.RejectedDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.RejectedDate)))
                .ForMember(d => d.Status, s => s.MapFrom(s => ConvertOrderStatus.ConvertOrderStatusToString(s.Status)));

            CreateMap<Order, ListOrderResponse>()
                .ForMember(d => d.OrderType, s => s.MapFrom(s => s.OrderType == true ? "Online" : "Offline"))
                .ForMember(d => d.Status, s => s.MapFrom(s => ConvertOrderStatus.ConvertOrderStatusToString(s.Status)));
            
            CreateMap<Order, ListOrderForStaffResponse>()
                .ForMember(d => d.OrderType, s => s.MapFrom(s => s.OrderType == true ? "Online" : "Offline"))
                .ForMember(d => d.CustomerName, s => s.MapFrom(s => s.Customer.Fullname))
                .ForMember(d => d.Status, s => s.MapFrom(s => ConvertOrderStatus.ConvertOrderStatusToString(s.Status)));
        }

        
    }
}
