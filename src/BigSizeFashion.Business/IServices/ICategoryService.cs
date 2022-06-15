using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<CategoryResponse>>> GetAllCategory(SearchCategoryParameter param);
        Task<Result<CategoryResponse>> GetCategoryByID(int id);
        Task<Result<CategoryResponse>> CreateCategory(CategoryRequest request);
        Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest request);
        Task<Result<bool>> DeleteCategory(int id);
    }
}
