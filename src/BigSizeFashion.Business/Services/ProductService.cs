using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _genericRepository;
        private readonly IMapper _mapper;

        public ProductService(IGenericRepository<Product> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        //public async Task<Result<CreateProductResponse>> CreateProduct(CreateProductRequest request)
        //{
        //    var result = new Result<CreateProductResponse>();
        //    try
        //    {
        //        var product = _mapper.Map<Product>(request);
        //        await _genericRepository.InsertAsync(product);
        //        await _genericRepository.SaveAsync();
        //        result.Content = _mapper.Map<CreateProductResponse>(product);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}
    }
}
