using AutoMapper;
using BigSizeFashion.Business.Dtos.Requests;
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
    public class CustomerProfileMapper : Profile
    {
        public CustomerProfileMapper()
        {
            CreateMap<Customer, CustomerProfileResponse>()
                .ForMember(d => d.Weigth, s => s.MapFrom(s => s.Weight))
                .ForMember(d => d.Heigth, s => s.MapFrom(s => s.Height))
                .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertDateToString(s.Birthday)))
                .ForMember(d => d.Gender, s => s.MapFrom(s => s.Gender == true ? "Nam" : s.Gender == false ? "Nữ" : null));

            CreateMap<UpdateCustomerProfileRequest, Customer>()
                .ForMember(d => d.Weight, s => s.MapFrom(s => s.Weigth))
                .ForMember(d => d.Height, s => s.MapFrom(s => s.Heigth))
                .ForMember(d => d.Birthday, s => s.MapFrom(s => ConvertDateTime.ConvertStringToDate(s.Birthday)));

            CreateMap<AddNewCustomerRequest, Customer>()
                .ForMember(d => d.Status, s => s.MapFrom(s => true));
        }
    }
}
