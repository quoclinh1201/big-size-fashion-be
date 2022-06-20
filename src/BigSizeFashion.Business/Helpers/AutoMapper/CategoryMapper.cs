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
    public class CategoryMapper : Profile
    {
        public CategoryMapper()
        {
            CreateMap<Category, CategoryResponse>()
                .ForMember(d => d.CategoryName, s => s.MapFrom(s => s.CategoryName));

            CreateMap<CategoryRequest, Category>()
                .ForMember(d => d.CategoryName, s => s.MapFrom(s => s.Category))
                .ForMember(d => d.Status, s => s.MapFrom(s => true));
        }
    }
}
