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
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<CreateProductRequest, Product>()
                .ForMember(d => d.Status, s => s.MapFrom(s => true));

            //CreateMap<Product, CreateProductResponse>()
            //    .ForMember();
                
        }
    }
}
