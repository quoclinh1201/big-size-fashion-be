using AutoMapper;
using BigSizeFashion.Business.Dtos.ResponseObjects;
using BigSizeFashion.Business.Dtos.Responses;
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
                .ForMember(d => d.Category, s => s.MapFrom(s => s.Category.CategoryName))
                .ForMember(d => d.Gender, s => s.MapFrom(s => s.Gender == true ? "Nam" : "Nữ"));

            CreateMap<Product, GetDetailProductResponse>()
                .ForMember(d => d.Category, s => s.MapFrom(s => s.Category.CategoryName))
                .ForMember(d => d.Gender, s => s.MapFrom(s => s.Gender == true ? "Nam" : "Nữ"));

            CreateMap<Product, GetListProductResponse>();

            CreateMap<ProductDetail, DeliveryNoteDetailItem>()
                .ForMember(d => d.Category, s => s.MapFrom(s => s.Product.Category.CategoryName))
                .ForMember(d => d.Size, s => s.MapFrom(s => s.Size.SizeName))
                .ForMember(d => d.Colour, s => s.MapFrom(s => s.Colour.ColourName));

                //.ForMember(d => d.ProductId, s => s.MapFrom(s => s.Product.ProductId))
                //.ForMember(d => d.ProductName, s => s.MapFrom(s => s.Product.ProductName))
                //.ForMember(d => d.Price, s => s.MapFrom(s => s.Product.Price))
                //.ForMember(d => d.Status, s => s.MapFrom(s => s.Product.Status));

            //CreateMap<Product, GetListProductForStaffResponse>();

            //CreateMap<Product, GetDetailProductForStaffResponse>()
            //    .ForMember(d => d.Category, s => s.MapFrom(s => s.Category.CategoryName))
            //    .ForMember(d => d.Size, s => s.MapFrom(s => s.Size.SizeName))
            //    .ForMember(d => d.Colour, s => s.MapFrom(s => s.Colour.ColourName))
            //    .ForMember(d => d.Gender, s => s.MapFrom(s => s.Gender == true ? "Male" : "Female"));

        }
    }
}
