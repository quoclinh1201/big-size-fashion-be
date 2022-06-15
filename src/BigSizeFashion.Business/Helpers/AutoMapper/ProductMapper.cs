﻿using AutoMapper;
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
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<CreateProductRequest, Product>()
                .ForMember(d => d.Status, s => s.MapFrom(s => true));

            CreateMap<Product, CreateProductResponse>()
                .ForMember(d => d.Category, s => s.MapFrom(s => s.Category.Category1))
                .ForMember(d => d.Size, s => s.MapFrom(s => s.Size.Size1))
                .ForMember(d => d.Colour, s => s.MapFrom(s => s.Colour.Colour1))
                .ForMember(d => d.Gender, s => s.MapFrom(s => s.Gender == true ? "Male" : "Female"));

            CreateMap<Product, GetDetailProductResponse>()
                .ForMember(d => d.Category, s => s.MapFrom(s => s.Category.Category1))
                .ForMember(d => d.Size, s => s.MapFrom(s => s.Size.Size1))
                .ForMember(d => d.Colour, s => s.MapFrom(s => s.Colour.Colour1))
                //.ForMember(d => d.Price, s => s.MapFrom(s => s.Price.ToString()))
                .ForMember(d => d.Gender, s => s.MapFrom(s => s.Gender == true ? "Male" : "Female"));

            CreateMap<Product, GetListProductResponse>();

        }
    }
}