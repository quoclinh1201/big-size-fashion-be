using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IStaffService
    {
        Task<Result<StaffProfileResponse>> GetOwnProfile(string token);
        Task<Result<StaffProfileResponse>> UpdateProfile(string token, UpdateStaffProfileRequest request);
        Task<Result<IEnumerable<StaffOfStoreResponse>>> GetAllStaffOfStore(string v);
        Task<Result<string>> UploadAvatar(string v, IFormFile file);
        Task<Result<string>> GetAvatar(string v);
        Task<PagedResult<GetListManagerResponse>> GetListManager(GetListManagerParameter param);
    }
}
