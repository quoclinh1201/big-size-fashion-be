using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IStaffsService
    {
        Task<Result<StaffProfileResponse>> GetOwnProfile(string token);
        Task<Result<StaffProfileResponse>> UpdateProfile(string token, UpdateStaffProfileRequest request);
    }
}
