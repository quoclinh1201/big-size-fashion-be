using AutoMapper;
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
    public class SizeMapper : Profile
    {
        public SizeMapper()
        {
            CreateMap<Size, SizeResponse>()
                .ForMember(d => d.SizeName, s => s.MapFrom(s => s.Size1));

            CreateMap<SizeRequest, Size>()
                .ForMember(d => d.Size1, s => s.MapFrom(s => s.SizeName))
                .ForMember(d => d.Status, s => s.MapFrom(s => true));
        }
    }
}
