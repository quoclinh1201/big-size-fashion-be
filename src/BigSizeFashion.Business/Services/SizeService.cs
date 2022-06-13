using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Parameters;
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
    public class SizeService : ISizeService
    {
        private readonly IGenericRepository<Size> _genericRepository;
        private readonly IMapper _mapper;

        public SizeService(IGenericRepository<Size> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Result<SizeResponse>> CreateSize(SizeRequest request)
        {
            var result = new Result<SizeResponse>();
            try
            {
                var newSize = _mapper.Map<Size>(request);
                await _genericRepository.InsertAsync(newSize);
                await _genericRepository.SaveAsync();
                result.Content = _mapper.Map<SizeResponse>(newSize);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DeleteSize(int id)
        {
            var result = new Result<bool>();
            try
            {
                var size = await _genericRepository.FindAsync(s => s.SizeId == id);

                if(size is null)
                {
                    result.Content = false;
                    return result;
                }

                size.Status = false;
                await _genericRepository.UpdateAsync(size);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<IEnumerable<SizeResponse>>> GetAllSize(SearchSizeParameter param)
        {
            var result = new Result<IEnumerable<SizeResponse>>();
            try
            {
                var sizes = await _genericRepository.GetAllAsync();
                var query = sizes.AsQueryable();
                FilterSizeByName(ref query, param.Size);
                result.Content = _mapper.Map<List<SizeResponse>>(query.ToList());
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private static void FilterSizeByName(ref IQueryable<Size> query, string name)
        {
            if (!query.Any() || String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
            {
                return;
            }

            query = query.Where(q => q.Size1.ToLower().Contains(name.ToLower()));
        }

        public async Task<Result<SizeResponse>> GetSizeByID(int id)
        {
            var result = new Result<SizeResponse>();
            try
            {
                var size = await _genericRepository.FindAsync(s => s.SizeId == id && s.Status == true);
                result.Content = _mapper.Map<SizeResponse>(size);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<SizeResponse>> UpdateSize(int id, SizeRequest request)
        {
            var result = new Result<SizeResponse>();
            try
            {
                var size = await _genericRepository.FindAsync(s => s.SizeId == id);
                var model = _mapper.Map(request, size);
                await _genericRepository.UpdateAsync(model);
                result.Content = _mapper.Map<SizeResponse>(model);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }
    }
}
