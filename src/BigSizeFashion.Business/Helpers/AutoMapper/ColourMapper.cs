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
    public class ColourMapper : Profile
    {
        public ColourMapper()
        {
            CreateMap<Colour, ColourResponse>()
                .ForMember(d => d.ColourName, s => s.MapFrom(s => s.ColourName));

            CreateMap<ColourRequest, Colour>()
                .ForMember(d => d.ColourName, s => s.MapFrom(s => s.ColourName))
                .ForMember(d => d.Status, s => s.MapFrom(s => true));
        }
    }
}
