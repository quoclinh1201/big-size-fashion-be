using AutoMapper;
using BigSizeFashion.Business.Helpers.Enums;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.AutoMapper
{
    public class CreateCustomerAccountMapper : Profile
    {
        public CreateCustomerAccountMapper()
        {
            CreateMap<CreateCustomerAccountRequest, Account>()
                .ForMember(d => d.Username, s => s.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.CreateAt, s => s.MapFrom(s => DateTime.UtcNow.AddHours(7)))
                .ForMember(d => d.Status, s => s.MapFrom(s => true));

            CreateMap<CreateCustomerAccountRequest, Customer>()
                .ForMember(d => d.Fullname, s => s.MapFrom(s => s.Fullname))
                .ForMember(d => d.PhoneNumber, s => s.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.Gender, s => s.MapFrom(s => CustomerGender((int)s.Gender)))
                .ForMember(d => d.Weigth, s => s.MapFrom(s => s.Weigth))
                .ForMember(d => d.Heigth, s => s.MapFrom(s => s.Heigth))
                .ForMember(d => d.Status, s => s.MapFrom(s => true));
        }

        private static bool? CustomerGender(int gender)
        {
            return gender == 2 ? null : gender == 1 ? true : false;
        }
    }
}
