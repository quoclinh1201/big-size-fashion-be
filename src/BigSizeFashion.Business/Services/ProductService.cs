using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly static string _noImage = "https://firebasestorage.googleapis.com/v0/b/big-size-fashion-chain.appspot.com/o/assets%2Fimages%2FNo-Photo-Available.jpg?alt=media&token=ef1bc098-c179-4cf8-907d-a98a3a274633";
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductImage> _imageRepository;
        private readonly IGenericRepository<StoreWarehouse> _storeWarehoustRepository;
        private readonly IGenericRepository<Store> _storeRepository;
        private readonly IGenericRepository<PromotionDetail> _promotionDetailRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductImage> imageRepository,
            IGenericRepository<StoreWarehouse> storeWarehoustRepository,
            IGenericRepository<PromotionDetail> promotionDetailRepository,
            IGenericRepository<Store> storeRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _storeWarehoustRepository = storeWarehoustRepository;
            _promotionDetailRepository = promotionDetailRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public async Task<Result<CreateProductResponse>> CreateProduct(CreateProductRequest request)
        {
            var result = new Result<CreateProductResponse>();
            try
            {
                var product = _mapper.Map<Product>(request);
                await _productRepository.InsertAsync(product);
                await _productRepository.SaveAsync();

                var model = await _productRepository.GetAllByIQueryable()
                    .Include(p => p.Category)
                    .Include(p => p.Colour)
                    .Include(p => p.Size)
                    .Where(p => p.ProductId == product.ProductId)
                    .FirstOrDefaultAsync();
                await AddNewProductIntoAllStore(model.ProductId);
                result.Content = _mapper.Map<CreateProductResponse>(model);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private async Task AddNewProductIntoAllStore(int productId)
        {
            var allStoreId = await _storeRepository.GetAllByIQueryable().Where(s => s.Status == true).Select(s => s.StoreId).ToListAsync();
            if(allStoreId.Count > 0)
            {
                foreach (var id in allStoreId)
                {
                    var storeWarehoust = new StoreWarehouse { ProductId = productId, StoreId = id, Quantity = 0 };
                    await _storeWarehoustRepository.InsertAsync(storeWarehoust);
                }
                await _storeWarehoustRepository.SaveAsync();
            }
        }

        public async Task<PagedResult<GetListProductResponse>> GetListProductsWithAllStatus(SearchProductsParameter param)
        {
            try
            {
                var products = new List<Product>();
                var response = new List<GetListProductResponse>();
                if (param.Status == true)
                {
                    products = await _productRepository.GetAllByIQueryable()
                                    .Include(p => p.Colour)
                                    .Include(p => p.Size)
                                    .Include(p => p.Category)
                                    .Where(p => p.Status == true).ToListAsync();
                }
                else if (param.Status == false)
                {
                    products = await _productRepository.GetAllByIQueryable()
                                    .Include(p => p.Colour)
                                    .Include(p => p.Size)
                                    .Include(p => p.Category)
                                    .Where(p => p.Status == false).ToListAsync();
                }
                else
                {
                    products = await _productRepository.GetAllByIQueryable()
                                    .Include(p => p.Colour)
                                    .Include(p => p.Size)
                                    .Include(p => p.Category).ToListAsync();
                }

                if(products.Count > 0)
                {
                    var query = products.AsQueryable();
                    FilterProductByName(ref query, param.ProductName);
                    FilterProductByCategory(ref query, param.Category);
                    FilterProductBySize(ref query, param.Size);
                    FilterProductByColour(ref query, param.Colour);
                    OrderByPrice(ref query, param.OrderByPrice);
                    response = _mapper.Map<List<GetListProductResponse>>(query.ToList());

                    if (response.Count > 0)
                    {
                        for (int i = 0; i < response.Count; i++)
                        {
                            var image = await _imageRepository.FindAsync(x => x.ProductId == response[i].ProductId && x.IsMainImage == true);
                            if (image != null)
                            {
                                response[i].ImageUrl = image.ImageUrl;
                            }
                            else
                            {
                                response[i].ImageUrl = _noImage;
                            }

                            var now = DateTime.UtcNow.AddHours(7);
                            var pd = await _promotionDetailRepository.GetAllByIQueryable()
                                            .Include(p => p.Promotion)
                                            .Where(p => p.Promotion.ApplyDate <= now
                                                        && p.Promotion.ExpiredDate >= now
                                                        && p.Promotion.Status == true
                                                        && p.PromotionId == response[i].ProductId)
                                            .FirstOrDefaultAsync();
                            if (pd != null)
                            {
                                var unroundPrice = ((decimal)(100 - pd.Promotion.PromotionValue) / 100) * response[i].Price;
                                response[i].PromotionPrice = Math.Round(unroundPrice / 1000, 0) * 1000;
                                response[i].PromotionValue = pd.Promotion.PromotionValue + "%";
                            }
                            else
                            {
                                response[i].PromotionPrice = response[i].Price;
                            }
                        }
                    }
                }
                return PagedResult<GetListProductResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void FilterProductByName(ref IQueryable<Product> query, string name)
        {
            if (!query.Any() || String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
            {
                return;
            }

            query = query.Where(q => q.ProductName.ToLower().Contains(name.ToLower()));
        }

        private static void FilterProductBySize(ref IQueryable<Product> query, string size)
        {
            if (!query.Any() || String.IsNullOrEmpty(size) || String.IsNullOrWhiteSpace(size))
            {
                return;
            }

            query = query.Where(q => q.Size.Size1.ToLower().Equals(size.ToLower()));
        }

        private static void FilterProductByColour(ref IQueryable<Product> query, string colour)
        {
            if (!query.Any() || String.IsNullOrEmpty(colour) || String.IsNullOrWhiteSpace(colour))
            {
                return;
            }

            query = query.Where(q => q.Colour.Colour1.ToLower().Equals(colour.ToLower()));
        }

        private static void FilterProductByCategory(ref IQueryable<Product> query, string category)
        {
            if (!query.Any() || String.IsNullOrEmpty(category) || String.IsNullOrWhiteSpace(category))
            {
                return;
            }

            query = query.Where(q => q.Category.Category1.ToLower().Equals(category.ToLower()));
        }

        private void OrderByPrice(ref IQueryable<Product> query, bool? orderByPrice)
        {
            if (!query.Any() || orderByPrice is null)
            {
                return;
            }

            if (orderByPrice is true)
            {
                query = query.OrderByDescending(x => x.Price);
            }
            else
            {
                query = query.OrderBy(x => x.Price);
            }
        }

        public async Task<Result<GetDetailProductResponse>> GetProductByID(int id)
        {
            var result = new Result<GetDetailProductResponse>();
            try
            {
                var product = await _productRepository.GetAllByIQueryable()
                    .Include(p => p.Category)
                    .Include(p => p.Colour)
                    .Include(p => p.Size)
                    .Where(p => p.ProductId == id)
                    .FirstOrDefaultAsync();

                if (product != null)
                {
                    var model = _mapper.Map<GetDetailProductResponse>(product);
                    var images = await _imageRepository.FindByAsync(i => i.ProductId == model.ProductId);

                    if (images.Count > 0)
                    {
                        model.Images = _mapper.Map<List<ProductImageResponse>>(images);
                    }
                    else
                    {
                        var image = new ProductImageResponse
                        {
                            ProductId = model.ProductId,
                            ImageUrl = _noImage,
                            IsMainImage = true
                        };
                        model.Images.Add(image);
                    }

                    var now = DateTime.UtcNow.AddHours(7);
                    var pd = await _promotionDetailRepository.GetAllByIQueryable()
                                    .Include(p => p.Promotion)
                                    .Where(p => p.Promotion.ApplyDate <= now
                                                && p.Promotion.ExpiredDate >= now
                                                && p.Promotion.Status == true
                                                && p.PromotionId == model.ProductId)
                                    .FirstOrDefaultAsync();
                    if(pd != null)
                    {
                        var unroundPrice = ((decimal)(100 - pd.Promotion.PromotionValue) / 100) * model.Price;
                        model.PromotionPrice = Math.Round(unroundPrice / 1000, 0) * 1000;
                        model.PromotionValue = pd.Promotion.PromotionValue + "%";
                    }
                    else
                    {
                        model.PromotionPrice = model.Price;
                    }
                    result.Content = model;
                    return result;
                }
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, "Sản phẩm không tồn tại");
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
