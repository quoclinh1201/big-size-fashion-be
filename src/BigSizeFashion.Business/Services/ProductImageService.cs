using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services
{
    public class ProductImageService : IProductImageService
    {
        
        private readonly IGenericRepository<ProductImage> _genericRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        private string _apiKey;
        private string _bucket;
        private string _authEmail;
        private string _authPassword;

        public ProductImageService(IGenericRepository<ProductImage> genericRepository, IMapper mapper, IConfiguration config)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
            _config = config;
            _apiKey = _config["FirebaseStorage:APIKey"];
            _bucket = _config["FirebaseStorage:Bucket"];
            _authEmail = _config["FirebaseStorage:AuthEmail"];
            _authPassword = _config["FirebaseStorage:AuthPassword"];
        }

        public async Task<Result<IEnumerable<ProductImageResponse>>> AddImagesForProduct(int id, IFormFileCollection files)
        {
            var result = new Result<IEnumerable<ProductImageResponse>>();
            var listImage = new List<string>();
            //var response = new ProductImageResponse();
            try
            {
                if(files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            var auth = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
                            var a = await auth.SignInWithEmailAndPasswordAsync(_authEmail, _authPassword);
                            var cancellation = new CancellationTokenSource();
                            var ms = file.OpenReadStream();
                            
                            var upload = new FirebaseStorage(
                                _bucket,
                                new FirebaseStorageOptions
                                {
                                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                                    ThrowOnCancel = true
                                }
                            ).Child("assets")
                            .Child("images")
                            .Child(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "_" + $"{file.FileName}")
                            .PutAsync(ms, cancellation.Token);

                            var iamgeUrl = await upload;
                            listImage.Add(iamgeUrl);
                        }
                    }
                }

                if (listImage.Count > 0)
                {
                    var check = await _genericRepository.FindByAsync(c => c.ProductId == id && c.IsMainImage == true);
                    var isHaveMainImage = true;
                    if (check.Count == 0)
                    {
                        isHaveMainImage = false;
                    }
                    foreach (var imageUrl in listImage)
                    {
                        var model = new ProductImage();
                        model.ProductId = id;
                        model.ImageUrl = imageUrl;
                        if (!isHaveMainImage)
                        {
                            model.IsMainImage = true;
                            isHaveMainImage = true;
                        }
                        await _genericRepository.InsertAsync(model);
                    }
                    await _genericRepository.SaveAsync();
                    var images = await _genericRepository.FindByAsync(r => r.ProductId == id);
                    var response = _mapper.Map<List<ProductImageResponse>>(images);
                    result.Content = response;
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
