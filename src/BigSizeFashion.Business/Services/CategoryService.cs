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
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _genericRepository;
        private readonly IMapper _mapper;

        public CategoryService(IGenericRepository<Category> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<CategoryResponse>>> GetAllCategory(SearchCategoryParameter param)
        {
            var result = new Result<IEnumerable<CategoryResponse>>();
            try
            {
                var categories = await _genericRepository.GetAllAsync();
                var query = categories.AsQueryable();
                FilterCategoryByName(ref query, param.Category);
                FilterCategoryByStatus(ref query, param.Status);
                result.Content = _mapper.Map<List<CategoryResponse>>(query.ToList());
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private static void FilterCategoryByStatus(ref IQueryable<Category> query, bool? status)
        {
            if (!query.Any() || status == null)
            {
                return;
            }

            query = query.Where(q => q.Status == status);
        }

        private static void FilterCategoryByName(ref IQueryable<Category> query, string name)
        {
            if (!query.Any() || String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
            {
                return;
            }

            query = query.Where(q => q.CategoryName.ToLower().Contains(name.ToLower()));
        }

        public async Task<Result<CategoryResponse>> GetCategoryByID(int id)
        {
            var result = new Result<CategoryResponse>();
            try
            {
                var category = await _genericRepository.FindAsync(s => s.CategoryId == id && s.Status == true);
                result.Content = _mapper.Map<CategoryResponse>(category);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest request)
        {
            var result = new Result<CategoryResponse>();
            try
            {
                var category = _mapper.Map<Category>(request);
                await _genericRepository.InsertAsync(category);
                await _genericRepository.SaveAsync();
                result.Content = _mapper.Map<CategoryResponse>(category);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
        {
            var result = new Result<CategoryResponse>();
            try
            {
                var category = await _genericRepository.FindAsync(s => s.CategoryId == id);
                var model = _mapper.Map(request, category);
                await _genericRepository.UpdateAsync(model);
                result.Content = _mapper.Map<CategoryResponse>(model);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DeleteCategory(int id)
        {
            var result = new Result<bool>();
            try
            {
                var category = await _genericRepository.FindAsync(s => s.CategoryId == id);

                if (category is null)
                {
                    result.Content = false;
                    return result;
                }

                if(category.Status)
                {
                    category.Status = false;
                }
                else
                {
                    category.Status = true;
                }
                
                await _genericRepository.UpdateAsync(category);
                result.Content = true;
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
