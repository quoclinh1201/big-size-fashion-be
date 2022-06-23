using AutoMapper;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.AutoMapper
{
    public class ProductDetailMapper : Profile
    {
        public ProductDetailMapper()
        {
            CreateMap<ProductDetail, ProductDetailResponse>();
        }
    }
}
