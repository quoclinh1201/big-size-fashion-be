using AutoMapper;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services
{
    public class StaffService : IStaffService
    {
        private readonly IGenericRepository<staff> _staffRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        private string _apiKey;
        private string _bucket;
        private string _authEmail;
        private string _authPassword;

        public StaffService(
            IGenericRepository<staff> staffRepository,
            IGenericRepository<Account> accountRepository,
            IConfiguration config,
            IMapper mapper)
        {
            _staffRepository = staffRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _config = config;
            _apiKey = _config["FirebaseStorage:APIKey"];
            _bucket = _config["FirebaseStorage:Bucket"];
            _authEmail = _config["FirebaseStorage:AuthEmail"];
            _authPassword = _config["FirebaseStorage:AuthPassword"];
        }

        public async Task<Result<IEnumerable<StaffOfStoreResponse>>> GetAllStaffOfStore(string token)
        {
            var result = new Result<IEnumerable<StaffOfStoreResponse>>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var storeId = await _staffRepository.GetAllByIQueryable().Where(s => s.Uid == uid).Select(s => s.StoreId).FirstOrDefaultAsync();
                var staffs = await _staffRepository.GetAllByIQueryable().Include(s => s.UidNavigation).Where(s => s.StoreId == storeId && s.Status == true && s.UidNavigation.RoleId == 3).ToListAsync();
                result.Content = _mapper.Map<List<StaffOfStoreResponse>>(staffs);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<string>> GetAvatar(string token)
        {
            var result = new Result<string>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var avatar = await _staffRepository.GetAllByIQueryable().Where(s => s.Uid == uid).Select(s => s.AvatarUrl).FirstOrDefaultAsync();
                if(avatar == null)
                {
                    avatar = CommonConstants.BlankAvatar;
                }
                result.Content = avatar;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<StaffProfileResponse>> GetOwnProfile(string token)
        {
            var result = new Result<StaffProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var staff = await _staffRepository.FindAsync(s => s.Uid == uid && s.Status == true);

                if (staff is null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                var response = _mapper.Map<StaffProfileResponse>(staff);
                var account = await _accountRepository.GetAllByIQueryable().Include(a => a.Role).Where(s => s.Uid == uid).FirstOrDefaultAsync();
                response.Role = account.Role.RoleName;
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<StaffProfileResponse>> UpdateProfile(string token, UpdateStaffProfileRequest request)
        {
            var result = new Result<StaffProfileResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                if (uid == 0)
                {
                    result.Error = ErrorHelpers.PopulateError(401, APITypeConstants.Unauthorized_401, ErrorMessageConstants.Unauthenticate);
                    return result;
                }
                var staff = await _staffRepository.GetAllByIQueryable().Include(s => s.Store).Where(s => s.Uid == uid && s.Status == true).FirstOrDefaultAsync();

                if (staff is null)
                {
                    result.Error = ErrorHelpers.PopulateError(404, APITypeConstants.NotFound_404, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                var model = _mapper.Map(request, staff);
                await _staffRepository.UpdateAsync(model);

                var account = await _accountRepository.GetAllByIQueryable().Include(a => a.Role).Where(s => s.Uid == uid).FirstOrDefaultAsync();
                var response = _mapper.Map<StaffProfileResponse>(model);
                response.Role = account.Role.RoleName;

                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<string>> UploadAvatar(string token, IFormFile file)
        {
            var result = new Result<string>();
            try
            {
                if (file.Length > 0)
                {
                    var uid = DecodeToken.DecodeTokenToGetUid(token);
                    var staff = await _staffRepository.FindAsync(s => s.Uid == uid);

                    var auth = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
                    var a = await auth.SignInWithEmailAndPasswordAsync(_authEmail, _authPassword);
                    var cancellation = new CancellationTokenSource();
                    var ms = file.OpenReadStream();

                    if (staff.AvatarUrl != null)
                    {
                        var delete = new FirebaseStorage(
                            _bucket,
                            new FirebaseStorageOptions
                            {
                                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                                ThrowOnCancel = true
                            }
                        ).Child("assets")
                        .Child("images")
                        .Child("avt_" + uid)
                        .DeleteAsync();
                    }
                    
                    var upload = new FirebaseStorage(
                        _bucket,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true
                        }
                    ).Child("assets")
                    .Child("images")
                    .Child("avt_" + uid)
                    .PutAsync(ms, cancellation.Token);

                    var iamgeUrl = await upload;

                    staff.AvatarUrl = iamgeUrl;
                    await _staffRepository.UpdateAsync(staff);
                    result.Content = iamgeUrl;
                    return result;
                }
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.CannotUploadImage);
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
