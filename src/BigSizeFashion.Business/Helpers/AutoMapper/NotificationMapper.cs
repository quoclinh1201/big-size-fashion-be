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
    public class NotificationMapper : Profile
    {
        public NotificationMapper()
        {
            CreateMap<Notification, NotificationResponse>()
                .ForMember(d => d.CreateDate, s => s.MapFrom(s => ConvertDateTime.ConvertDateTimeToString(s.CreateDate)));

            CreateMap<CreateNotificationRequest, Notification>()
                .ForMember(d => d.AccountId, s => s.MapFrom(s => 0))
                .ForMember(d => d.CreateDate, s => s.MapFrom(s => DateTime.UtcNow.AddHours(7)))
                .ForMember(d => d.Status, s => s.MapFrom(s => true));
        }
    }
}
