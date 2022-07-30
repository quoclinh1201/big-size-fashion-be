using AutoMapper;
using BigSizeFashion.Business.Dtos.Requests;
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
    public class InventoryNoteMapper : Profile
    {
        public InventoryNoteMapper()
        {
            CreateMap<CreateInventoryNoteRequest, InventoryNote>()
                .ForMember(d => d.InventoryNoteName, s => s.MapFrom(s => s.InventoryNoteName))
                .ForMember(d => d.ToDate, s => s.MapFrom(s => ConvertDateTime.ConvertStringToDate(s.ToDate)))
                .ForMember(d => d.FromDate, s => s.MapFrom(s => ConvertDateTime.ConvertStringToDate(s.FromDate)))
                .ForMember(d => d.Status, s => s.MapFrom(s => true));

            CreateMap<InventoryNote, GetListInventoryNoteResponse>()
                .ForMember(d => d.ToDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.ToDate)))
                .ForMember(d => d.FromDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.FromDate)))
                .ForMember(d => d.AdjustedDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.AdjustedDate)))
                .ForMember(d => d.StaffName, s => s.MapFrom(s => s.Staff.Fullname));
        }
    }
}
