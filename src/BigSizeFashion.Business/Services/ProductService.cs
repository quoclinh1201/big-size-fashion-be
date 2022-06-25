using AutoMapper;
using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.ResponseObjects;
using BigSizeFashion.Business.Dtos.Responses;
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
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductImage> _imageRepository;
        private readonly IGenericRepository<StoreWarehouse> _storeWarehouseRepository;
        private readonly IGenericRepository<Store> _storeRepository;
        private readonly IGenericRepository<PromotionDetail> _promotionDetailRepository;
        private readonly IGenericRepository<staff> _staffRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Colour> _colourRepository;
        private readonly IGenericRepository<Size> _sizeRepository;
        private readonly IGenericRepository<ProductDetail> _productDetailRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductImage> imageRepository,
            IGenericRepository<StoreWarehouse> storeWarehouseRepository,
            IGenericRepository<PromotionDetail> promotionDetailRepository,
            IGenericRepository<Store> storeRepository,
            IGenericRepository<staff> staffRepository,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<Colour> colourRepository,
            IGenericRepository<Size> sizeRepository,
            IGenericRepository<ProductDetail> productDetailRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _storeWarehouseRepository = storeWarehouseRepository;
            _promotionDetailRepository = promotionDetailRepository;
            _storeRepository = storeRepository;
            _staffRepository = staffRepository;
            _customerRepository = customerRepository;
            _colourRepository = colourRepository;
            _sizeRepository = sizeRepository;
            _productDetailRepository = productDetailRepository;
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
                    .Where(p => p.ProductId == product.ProductId)
                    .FirstOrDefaultAsync();
                result.Content = _mapper.Map<CreateProductResponse>(model);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
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
                                    .Include(p => p.Category)
                                    .Where(p => p.Status == true)
                                    .Include(p => p.ProductDetails)
                                    .ThenInclude(p => p.Size)
                                    .Include(p => p.ProductDetails)
                                    .ThenInclude(p => p.Colour)
                                    .ToListAsync();
                }
                else if (param.Status == false)
                {
                    products = await _productRepository.GetAllByIQueryable()
                                    .Include(p => p.ProductDetails)
                                    .ThenInclude(p => p.Size)
                                    .Include(p => p.ProductDetails)
                                    .ThenInclude(p => p.Colour)
                                    .Include(p => p.Category)
                                    .Where(p => p.Status == false).ToListAsync();
                }
                else
                {
                    products = await _productRepository.GetAllByIQueryable()
                                    .Include(p => p.ProductDetails)
                                    .ThenInclude(p => p.Size)
                                    .Include(p => p.ProductDetails)
                                    .ThenInclude(p => p.Colour)
                                    .Include(p => p.Category)
                                    .ToListAsync();
                }

                if (products.Count > 0)
                {
                    var query = products.AsQueryable();
                    FilterProductByName(ref query, param.ProductName);
                    FilterProductByCategory(ref query, param.Category);
                    FilterProductByColourAndSize(ref query, param.Colour, param.Size);
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
                                response[i].ImageUrl = CommonConstants.NoImageUrl;
                            }

                            var now = DateTime.UtcNow.AddHours(7);
                            var pd = await _promotionDetailRepository.GetAllByIQueryable()
                                            .Include(p => p.Promotion)
                                            .Where(p => p.Promotion.ApplyDate <= now
                                                        && p.Promotion.ExpiredDate >= now
                                                        && p.Promotion.Status == true
                                                        && p.ProductId == response[i].ProductId)
                                            .FirstOrDefaultAsync();
                            if (pd != null)
                            {
                                var unroundPrice = ((decimal)(100 - pd.Promotion.PromotionValue) / 100) * response[i].Price;
                                response[i].PromotionPrice = Math.Round(unroundPrice / 1000, 0) * 1000;
                                response[i].PromotionValue = pd.Promotion.PromotionValue + "%";
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

        //private static void FilterProductBySize(ref IQueryable<Product> query, string size)
        //{
        //    if (!query.Any() || String.IsNullOrEmpty(size) || String.IsNullOrWhiteSpace(size))
        //    {
        //        return;
        //    }

        //    query = query.Where(q => q.ProductDetails.Any(p => p.Size.SizeName.ToLower().Equals(size.ToLower())));
        //}

        //private static void FilterProductByColour(ref IQueryable<Product> query, string colour)
        //{
        //    if (!query.Any() || String.IsNullOrEmpty(colour) || String.IsNullOrWhiteSpace(colour))
        //    {
        //        return;
        //    }

        //    query = query.Where(q => q.ProductDetails.Any(p => p.Colour.ColourName.ToLower().Equals(colour.ToLower())));
        //}

        private static void FilterProductByColourAndSize(ref IQueryable<Product> query, string colour, string size)
        {
            if (!query.Any() || ((String.IsNullOrEmpty(colour) || String.IsNullOrWhiteSpace(colour)) && (String.IsNullOrEmpty(size) || String.IsNullOrWhiteSpace(size))))
            {
                return;
            }

            if ((String.IsNullOrEmpty(colour) || String.IsNullOrWhiteSpace(colour)) && (!String.IsNullOrEmpty(size) || !String.IsNullOrWhiteSpace(size)))
            {
                query = query.Where(q => q.ProductDetails.Any(p => p.Size.SizeName.ToLower().Equals(size.ToLower())));
                return;
            }

            if ((!String.IsNullOrEmpty(colour) || !String.IsNullOrWhiteSpace(colour)) && (String.IsNullOrEmpty(size) || String.IsNullOrWhiteSpace(size)))
            {
                query = query.Where(q => q.ProductDetails.Any(p => p.Colour.ColourName.ToLower().Equals(colour.ToLower())));
                return;
            }

            if ((!String.IsNullOrEmpty(colour) || !String.IsNullOrWhiteSpace(colour)) && (!String.IsNullOrEmpty(size) || !String.IsNullOrWhiteSpace(size)))
            {
                query = query.Where(q => q.ProductDetails.Any(p => p.Colour.ColourName.ToLower().Equals(colour.ToLower()) && p.Size.SizeName.ToLower().Equals(size.ToLower())));
                return;
            }
        }

        private static void FilterProductByCategory(ref IQueryable<Product> query, string category)
        {
            if (!query.Any() || String.IsNullOrEmpty(category) || String.IsNullOrWhiteSpace(category))
            {
                return;
            }

            query = query.Where(q => q.Category.CategoryName.ToLower().Equals(category.ToLower()));
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
                                    .Where(p => p.ProductId == id)
                                    .Include(p => p.ProductDetails)
                                    .ThenInclude(p => p.Size)
                                    .Include(p => p.ProductDetails)
                                    .ThenInclude(p => p.Colour)
                                    .FirstOrDefaultAsync();

                if (product != null)
                {
                    var model = _mapper.Map<GetDetailProductResponse>(product);

                    var productDetails = await _productDetailRepository.GetAllByIQueryable()
                                        .Include(p => p.Size)
                                        .Include(p => p.Colour)
                                        .Where(p => p.ProductId == id).ToListAsync();

                    var list = new List<ProductDetailResponse>();

                    foreach (var item in productDetails)
                    {
                        var x = new ProductDetailResponse
                        {
                            ProductDetailId = item.ProductDetailId,
                            ProductId = item.ProductId,
                            Size = _mapper.Map<SizeResponse>(item.Size),
                            Colour = _mapper.Map<ColourResponse>(item.Colour)
                        };
                        list.Add(x);
                    }

                    model.ProductDetailList = list;

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
                            ImageUrl = CommonConstants.NoImageUrl,
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
                                                && p.ProductId == model.ProductId)
                                    .FirstOrDefaultAsync();
                    if (pd != null)
                    {
                        var unroundPrice = ((decimal)(100 - pd.Promotion.PromotionValue) / 100) * model.Price;
                        model.PromotionPrice = Math.Round(unroundPrice / 1000, 0) * 1000;
                        model.PromotionValue = pd.Promotion.PromotionValue + "%";
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

        //public async Task<PagedResult<GetListProductForStaffResponse>> GetListProductOfStore(string token, SearchProductsParameter param)
        //{
        //    try
        //    {
        //        var accountUId = DecodeToken.DecodeTokenToGetUid(token);
        //        var storeID = _staffRepository.GetAllByIQueryable().Where(s => s.Uid == accountUId).Select(s => s.StoreId).FirstOrDefault();
        //        var response = new List<GetListProductForStaffResponse>();

        //        var products = await _productRepository.GetAllByIQueryable()
        //                            .Include(p => p.Category)
        //                            .Where(p => p.Status == true)
        //                            .Include(p => p.ProductDetails)
        //                            .ThenInclude(p => p.Size)
        //                            .Include(p => p.ProductDetails)
        //                            .ThenInclude(p => p.Colour)
        //                            .ToListAsync();

        //        if (products.Count > 0)
        //        {
        //            var query = products.AsQueryable();
        //            FilterProductByName(ref query, param.ProductName);
        //            FilterProductByCategory(ref query, param.Category);
        //            FilterProductByColourAndSize(ref query, param.Colour, param.Size);
        //            OrderByPrice(ref query, param.OrderByPrice);
        //            response = _mapper.Map<List<GetListProductForStaffResponse>>(query.ToList());

        //            if (response.Count > 0)
        //            {
        //                for (int i = 0; i < response.Count; i++)
        //                {
        //                    var image = await _imageRepository.FindAsync(x => x.ProductId == response[i].ProductId && x.IsMainImage == true);
        //                    if (image != null)
        //                    {
        //                        response[i].ImageUrl = image.ImageUrl;
        //                    }
        //                    else
        //                    {
        //                        response[i].ImageUrl = CommonConstants.NoImageUrl;
        //                    }

        //                    var now = DateTime.UtcNow.AddHours(7);
        //                    var pd = await _promotionDetailRepository.GetAllByIQueryable()
        //                                    .Include(p => p.Promotion)
        //                                    .Where(p => p.Promotion.ApplyDate <= now
        //                                                && p.Promotion.ExpiredDate >= now
        //                                                && p.Promotion.Status == true
        //                                                && p.ProductId == response[i].ProductId)
        //                                    .FirstOrDefaultAsync();
        //                    if (pd != null)
        //                    {
        //                        var unroundPrice = ((decimal)(100 - pd.Promotion.PromotionValue) / 100) * response[i].Price;
        //                        response[i].PromotionPrice = Math.Round(unroundPrice / 1000, 0) * 1000;
        //                        response[i].PromotionValue = pd.Promotion.PromotionValue + "%";
        //                    }
        //                    else
        //                    {
        //                        response[i].PromotionPrice = response[i].Price;
        //                    }

        //                    var quantity = await _storeWarehoustRepository.GetAllByIQueryable()
        //                                        .Where(s => s.ProductId == response[i].ProductId && s.StoreId == storeID)
        //                                        .Select(s => s.Quantity)
        //                                        .FirstOrDefaultAsync();
        //                    response[i].Quantity = quantity > 0 ? quantity : 0;
        //                }
        //            }
        //        }
        //        return PagedResult<GetListProductForStaffResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public async Task<Result<GetDetailProductForStaffResponse>> GetProductOfStoreByID(string token, int id)
        //{
        //    var result = new Result<GetDetailProductForStaffResponse>();
        //    try
        //    {
        //        var accountUId = DecodeToken.DecodeTokenToGetUid(token);
        //        var storeID = _staffRepository.GetAllByIQueryable().Where(s => s.Uid == accountUId).Select(s => s.StoreId).FirstOrDefault();
        //        var product = await _productRepository.GetAllByIQueryable()
        //            .Include(p => p.Category)
        //            .Include(p => p.Colour)
        //            .Include(p => p.Size)
        //            .Where(p => p.ProductId == id)
        //            .FirstOrDefaultAsync();

        //        if (product != null)
        //        {
        //            var model = _mapper.Map<GetDetailProductForStaffResponse>(product);
        //            var images = await _imageRepository.FindByAsync(i => i.ProductId == model.ProductId);

        //            if (images.Count > 0)
        //            {
        //                model.Images = _mapper.Map<List<ProductImageResponse>>(images);
        //            }
        //            else
        //            {
        //                var image = new ProductImageResponse
        //                {
        //                    ProductId = model.ProductId,
        //                    ImageUrl = CommonConstants.NoImageUrl,
        //                    IsMainImage = true
        //                };
        //                model.Images.Add(image);
        //            }

        //            var now = DateTime.UtcNow.AddHours(7);
        //            var pd = await _promotionDetailRepository.GetAllByIQueryable()
        //                            .Include(p => p.Promotion)
        //                            .Where(p => p.Promotion.ApplyDate <= now
        //                                        && p.Promotion.ExpiredDate >= now
        //                                        && p.Promotion.Status == true
        //                                        && p.ProductId == model.ProductId)
        //                            .FirstOrDefaultAsync();
        //            if (pd != null)
        //            {
        //                var unroundPrice = ((decimal)(100 - pd.Promotion.PromotionValue) / 100) * model.Price;
        //                model.PromotionPrice = Math.Round(unroundPrice / 1000, 0) * 1000;
        //                model.PromotionValue = pd.Promotion.PromotionValue + "%";
        //            }
        //            else
        //            {
        //                model.PromotionPrice = model.Price;
        //            }

        //            var quantity = await _storeWarehoustRepository.GetAllByIQueryable()
        //                                        .Where(s => s.ProductId == model.ProductId && s.StoreId == storeID)
        //                                        .Select(s => s.Quantity)
        //                                        .FirstOrDefaultAsync();
        //            model.Quantity = quantity > 0 ? quantity : 0;

        //            result.Content = model;
        //            return result;
        //        }
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, "Sản phẩm không tồn tại");
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}

        public async Task<Result<CreateProductResponse>> UpdateProduct(int id, CreateProductRequest request)
        {
            var result = new Result<CreateProductResponse>();
            try
            {
                var product = await _productRepository.FindAsync(p => p.ProductId == id);
                var model = _mapper.Map(request, product);
                await _productRepository.UpdateAsync(model);
                var p = await _productRepository.GetAllByIQueryable().Include(p => p.Category).Where(p => p.ProductId == id).FirstOrDefaultAsync();
                result.Content = _mapper.Map<CreateProductResponse>(p);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DeleteProduct(int id)
        {
            var result = new Result<bool>();
            try
            {
                var roduct = await _productRepository.FindAsync(s => s.ProductId == id);

                if (roduct is null)
                {
                    result.Content = false;
                    return result;
                }

                roduct.Status = false;
                await _productRepository.UpdateAsync(roduct);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<PagedResult<GetListProductResponse>> GetListProductFitWithCustomer(string token, QueryStringParameters param)
        {
            try
            {
                var accountUId = DecodeToken.DecodeTokenToGetUid(token);
                var customer = await _customerRepository.FindAsync(c => c.Uid == accountUId);
                var height = customer.Height;
                var weight = customer.Weight;
                var s = new SearchProductsParameter { PageNumber = param.PageNumber, PageSize = param.PageSize };

                if (height == null || weight == null)
                {
                    return await GetListProductsWithAllStatus(s);
                }

                if (customer.Gender != null)
                {
                    if (customer.Gender == true)
                    {
                        s.Gender = "Male";
                        if (height < 176 || weight < 76)
                        {
                            return await GetListProductsWithAllStatus(s);
                        }
                        else if (height >= 176 && weight >= 76 && height < 182 && weight < 86)
                        {
                            s.Size = "XL";
                            return await GetListProductsWithAllStatus(s);
                        }
                        else if (height >= 182 && weight >= 86 && height < 188 && weight < 96)
                        {
                            s.Size = "XXL";
                            return await GetListProductsWithAllStatus(s);
                        }
                        else if (height >= 188 && weight >= 96 && height < 194 && weight < 101)
                        {
                            s.Size = "XXXL";
                            return await GetListProductsWithAllStatus(s);
                        }
                        else if (height >= 188 && weight >= 101 && height < 195 && weight < 116)
                        {
                            s.Size = "4XL";
                            return await GetListProductsWithAllStatus(s);
                        }
                        else if (height >= 188 && weight >= 115 && height < 196 && weight < 121)
                        {
                            s.Size = "5XL";
                            return await GetListProductsWithAllStatus(s);
                        }
                        else
                        {
                            s.Size = "6XL";
                            return await GetListProductsWithAllStatus(s);
                        }
                    }
                    else
                    {
                        s.Gender = "Female";
                        if (height < 168 || weight < 66)
                        {
                            return await GetListProductsWithAllStatus(s);
                        }
                        else if (height >= 168 && weight >= 66 && height < 176 && weight < 76)
                        {
                            s.Size = "XL";
                            return await GetListProductsWithAllStatus(s);
                        }
                        else if (height >= 176 && weight >= 76 && height < 182 && weight < 86)
                        {
                            s.Size = "XXL";
                            return await GetListProductsWithAllStatus(s);
                        }
                        else if (height >= 182 && weight >= 86 && height < 188 && weight < 91)
                        {
                            s.Size = "XXXL";
                            return await GetListProductsWithAllStatus(s);
                        }
                        else if (height >= 182 && weight >= 91 && height < 189 && weight < 106)
                        {
                            s.Size = "4XL";
                            return await GetListProductsWithAllStatus(s);
                        }
                        else if (height >= 182 && weight >= 106 && height < 190 && weight < 111)
                        {
                            s.Size = "5XL";
                            return await GetListProductsWithAllStatus(s);
                        }
                        else
                        {
                            s.Size = "6XL";
                            return await GetListProductsWithAllStatus(s);
                        }
                    }
                }
                else
                {
                    ////////////////////////////////
                    return await GetListProductsWithAllStatus(s);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<decimal> GetProductPrice(int id)
        {
            try
            {
                var price = await _productRepository.GetAllByIQueryable().Where(p => p.ProductId == id && p.Status == true).Select(p => p.Price).FirstOrDefaultAsync();
                return price;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<decimal?> GetProductPromotionPrice(int id)
        {
            try
            {
                var now = DateTime.UtcNow.AddHours(7);
                var price = await _productRepository.GetAllByIQueryable().Where(p => p.ProductId == id && p.Status == true).Select(p => p.Price).FirstOrDefaultAsync();
                var pd = await _promotionDetailRepository.GetAllByIQueryable()
                                .Include(p => p.Promotion)
                                .Where(p => p.Promotion.ApplyDate <= now
                                            && p.Promotion.ExpiredDate >= now
                                            && p.Promotion.Status == true
                                            && p.ProductId == id)
                                .FirstOrDefaultAsync();
                if (pd != null)
                {
                    var unroundPrice = ((decimal)(100 - pd.Promotion.PromotionValue) / 100) * price;
                    return Math.Round(unroundPrice / 1000, 0) * 1000;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<GetQuantityOfProductResponse>> GetQuantityOfProduct(GetQuantityOfProductParameter param)
        {
            var result = new Result<GetQuantityOfProductResponse>();
            try
            {
                var productDetailId = await _productDetailRepository
                    .GetAllByIQueryable()
                    .Where(p => p.ProductId == param.ProductId && p.ColourId == param.ColourId && p.SizeId == param.SizeId)
                    .Select(p => p.ProductDetailId).FirstOrDefaultAsync();

                var quantity = await _storeWarehouseRepository.GetAllByIQueryable()
                                .Where(s => s.StoreId == param.StoreId && s.ProductDetailId == productDetailId)
                                .Select(s => s.Quantity)
                                .FirstOrDefaultAsync();

                result.Content = new GetQuantityOfProductResponse { ProductDetailId = productDetailId, StoreId = param.StoreId, Quantity = quantity };
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
