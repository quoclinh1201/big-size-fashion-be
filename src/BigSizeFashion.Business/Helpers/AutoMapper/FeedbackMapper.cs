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
    public class FeedbackMapper : Profile
    {
        public FeedbackMapper()
        {
            CreateMap<Feedback, FeedbackResponse>()
                .ForMember(d => d.CustomerName, s => s.MapFrom(s => s.Customer.Fullname))
                .ForMember(d => d.CreateDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.CreateDate)));

            CreateMap<FeedbackRequest, Feedback>()
                .ForMember(d => d.CreateDate, s => s.MapFrom(s => DateTime.UtcNow.AddHours(7)))
                .ForMember(d => d.Status, s => s.MapFrom(s => true));
        }
    }
}
