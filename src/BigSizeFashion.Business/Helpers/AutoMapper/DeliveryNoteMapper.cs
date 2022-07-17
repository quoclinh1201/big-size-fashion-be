using AutoMapper;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.AutoMapper
{
    public class DeliveryNoteMapper : Profile
    {
        public DeliveryNoteMapper()
        {
            CreateMap<DeliveryNote, ListImportProductResponse>()
                .ForMember(d => d.RequestStore, s => s.MapFrom(s => s.FromStoreNavigation.StoreName))
                .ForMember(d => d.StoreAddress, s => s.MapFrom(s => s.FromStoreNavigation.StoreAddress))
                .ForMember(d => d.CreateDate, s => s.MapFrom(s => ""))
                .ForMember(d => d.Status, s => s.MapFrom(s => s.Status == 0 ? "Đã hủy" : s.Status == 1 ? "Chờ xác nhận" : "Đã xác nhận"));

            CreateMap<DeliveryNote, ListExportProductResponse>()
                .ForMember(d => d.RequestStore, s => s.MapFrom(s => s.ToStoreNavigation.StoreName))
                .ForMember(d => d.StoreAddress, s => s.MapFrom(s => s.ToStoreNavigation.StoreAddress))
                .ForMember(d => d.CreateDate, s => s.MapFrom(s => ""))
                .ForMember(d => d.Status, s => s.MapFrom(s => s.Status == 0 ? "Đã hủy" : s.Status == 1 ? "Chờ xác nhận" : "Đã xác nhận"));

            CreateMap<DeliveryNote, DeliveryNoteDetailResponse>()
                .ForMember(d => d.ReceiveStaffId, s => s.MapFrom(s => s.StaffId))
                .ForMember(d => d.ReceiveStaffName, s => s.MapFrom(s => s.Staff.Fullname))
                .ForMember(d => d.FromStore, s => s.MapFrom(s => new StoreResponse()))
                .ForMember(d => d.ToStore, s => s.MapFrom(s => new StoreResponse()))
                .ForMember(d => d.CreateDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.CreateDate)))
                .ForMember(d => d.ApprovalDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.ApprovalDate)))
                .ForMember(d => d.Status, s => s.MapFrom(s => s.Status == 0 ? "Đã hủy" : s.Status == 1 ? "Chờ xác nhận" : "Đã xác nhận"));
        }
    }
}
