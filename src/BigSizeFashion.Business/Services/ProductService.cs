using AutoMapper;
using BigSizeFashion.Business.Dtos.ResponseObjects;
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
        private readonly IGenericRepository<StoreWarehouse> _storeWarehoustRepository;
        private readonly IGenericRepository<Store> _storeRepository;
        private readonly IGenericRepository<PromotionDetail> _promotionDetailRepository;
        private readonly IGenericRepository<staff> _staffRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductImage> imageRepository,
            IGenericRepository<StoreWarehouse> storeWarehoustRepository,
            IGenericRepository<PromotionDetail> promotionDetailRepository,
            IGenericRepository<Store> storeRepository,
            IGenericRepository<staff> staffRepository,
            IGenericRepository<Customer> customerRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _storeWarehoustRepository = storeWarehoustRepository;
            _promotionDetailRepository = promotionDetailRepository;
            _storeRepository = storeRepository;
            _staffRepository = staffRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        //public async Task<Result<CreateProductResponse>> CreateProduct(CreateProductRequest request)
        //{
        //    var result = new Result<CreateProductResponse>();
        //    try
        //    {
        //        var product = _mapper.Map<Product>(request);
        //        await _productRepository.InsertAsync(product);
        //        await _productRepository.SaveAsync();

        //        var model = await _productRepository.GetAllByIQueryable()
        //            .Include(p => p.Category)
        //            .Include(p => p.Colour)
        //            .Include(p => p.Size)
        //            .Where(p => p.ProductId == product.ProductId)
        //            .FirstOrDefaultAsync();
        //        await AddNewProductIntoAllStore(model.ProductId);
        //        result.Content = _mapper.Map<CreateProductResponse>(model);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}

        //private async Task AddNewProductIntoAllStore(int productId)
        //{
        //    var allStoreId = await _storeRepository.GetAllByIQueryable().Where(s => s.Status == true).Select(s => s.StoreId).ToListAsync();
        //    if(allStoreId.Count > 0)
        //    {
        //        foreach (var id in allStoreId)
        //        {
        //            var storeWarehoust = new StoreWarehouse { ProductId = productId, StoreId = id, Quantity = 0 };
        //            await _storeWarehoustRepository.InsertAsync(storeWarehoust);
        //        }
        //        await _storeWarehoustRepository.SaveAsync();
        //    }
        //}

        //public async Task<PagedResult<GetListProductResponse>> GetListProductsWithAllStatus(SearchProductsParameter param)
        //{
        //    try
        //    {
        //        var products = new List<Product>();
        //        var response = new List<GetListProductResponse>();
        //        if (param.Status == true)
        //        {
        //            products =  _productRepository.GetAllByIQueryable()
        //                            .Include(p => p.Colour)
        //                            .Include(p => p.Size)
        //                            .Include(p => p.Category)
        //                            //.Where(p => p.Status == true).ToListAsync();
        //                            .DistinctBy(p => p.ProductName)
        //                            .Where(p => p.Status == false).ToList();
        //        }
        //        else if (param.Status == false)
        //        {
        //            products = _productRepository.GetAllByIQueryable()
        //                            .Include(p => p.Colour)
        //                            .Include(p => p.Size)
        //                            .Include(p => p.Category)
        //                            .DistinctBy(p => p.ProductName)
        //                            .Where(p => p.Status == false).ToList();
        //        }
        //        else
        //        {
        //            products = _productRepository.GetAllByIQueryable()
        //                            .Include(p => p.Colour)
        //                            .Include(p => p.Size)
        //                            .Include(p => p.Category)
        //                            //.ToListAsync();
        //                            //.DistinctBy(p => p.ProductName)
        //                            .Where(p => p.Status == false).ToList();
        //        }

        //        if(products.Count > 0)
        //        {
        //            var query = products.AsQueryable();
        //            FilterProductByName(ref query, param.ProductName);
        //            FilterProductByCategory(ref query, param.Category);
        //            FilterProductBySize(ref query, param.Size);
        //            FilterProductByColour(ref query, param.Colour);
        //            OrderByPrice(ref query, param.OrderByPrice);
        //            response = _mapper.Map<List<GetListProductResponse>>(query.ToList());

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
        //                }
        //            }
        //        }
        //        return PagedResult<GetListProductResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //private static void FilterProductByName(ref IQueryable<Product> query, string name)
        //{
        //    if (!query.Any() || String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
        //    {
        //        return;
        //    }

        //    query = query.Where(q => q.ProductName.ToLower().Contains(name.ToLower()));
        //}

        //private static void FilterProductBySize(ref IQueryable<Product> query, string size)
        //{
        //    if (!query.Any() || String.IsNullOrEmpty(size) || String.IsNullOrWhiteSpace(size))
        //    {
        //        return;
        //    }

        //    query = query.Where(q => q.Size.SizeName.ToLower().Equals(size.ToLower()));
        //}

        //private static void FilterProductByColour(ref IQueryable<Product> query, string colour)
        //{
        //    if (!query.Any() || String.IsNullOrEmpty(colour) || String.IsNullOrWhiteSpace(colour))
        //    {
        //        return;
        //    }

        //    query = query.Where(q => q.Colour.ColourName.ToLower().Equals(colour.ToLower()));
        //}

        //private static void FilterProductByCategory(ref IQueryable<Product> query, string category)
        //{
        //    if (!query.Any() || String.IsNullOrEmpty(category) || String.IsNullOrWhiteSpace(category))
        //    {
        //        return;
        //    }

        //    query = query.Where(q => q.Category.CategoryName.ToLower().Equals(category.ToLower()));
        //}

        //private void OrderByPrice(ref IQueryable<Product> query, bool? orderByPrice)
        //{
        //    if (!query.Any() || orderByPrice is null)
        //    {
        //        return;
        //    }

        //    if (orderByPrice is true)
        //    {
        //        query = query.OrderByDescending(x => x.Price);
        //    }
        //    else
        //    {
        //        query = query.OrderBy(x => x.Price);
        //    }
        //}

        //public async Task<Result<GetDetailProductResponse>> GetProductByID(int id)
        //{
        //    var result = new Result<GetDetailProductResponse>();
        //    try
        //    {
        //        var product = await _productRepository.GetAllByIQueryable()
        //            .Include(p => p.Category)
        //            .Include(p => p.Colour)
        //            .Include(p => p.Size)
        //            .Where(p => p.ProductId == id)
        //            .FirstOrDefaultAsync();

        //        if (product != null)
        //        {
        //            var model = _mapper.Map<GetDetailProductResponse>(product);
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
        //            if(pd != null)
        //            {
        //                var unroundPrice = ((decimal)(100 - pd.Promotion.PromotionValue) / 100) * model.Price;
        //                model.PromotionPrice = Math.Round(unroundPrice / 1000, 0) * 1000;
        //                model.PromotionValue = pd.Promotion.PromotionValue + "%";
        //            }
        //            else
        //            {
        //                model.PromotionPrice = model.Price;
        //            }
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

        //public async Task<PagedResult<GetListProductForStaffResponse>> GetListProductOfStore(string token, SearchProductsParameter param)
        //{
        //    try
        //    {
        //        var accountUId = DecodeToken.DecodeTokenToGetUid(token);
        //        var storeID = _staffRepository.GetAllByIQueryable().Where(s => s.Uid == accountUId).Select(s => s.StoreId).FirstOrDefault();
        //        var response = new List<GetListProductForStaffResponse>();

        //        var products = await _productRepository.GetAllByIQueryable()
        //                        .Where(p => p.Status == true)
        //                        .Include(p => p.Colour)
        //                        .Include(p => p.Size)
        //                        .Include(p => p.Category)
        //                        .ToListAsync();

        //        //var products = await _storeWarehoustRepository.GetAllByIQueryable()
        //        //                    .Where(s => s.StoreId == storeID && s.Product.Status == true)
        //        //                    .Include(p => p.Product)
        //        //                    .Include(p => p.Product.Colour)
        //        //                    .Include(p => p.Product.Category)
        //        //                    .Include(p => p.Product.Size)
        //        //                    .ToListAsync();

        //        if (products.Count > 0)
        //        {
        //            var query = products.AsQueryable();
        //            FilterProductByName(ref query, param.ProductName);
        //            FilterProductByCategory(ref query, param.Category);
        //            FilterProductBySize(ref query, param.Size);
        //            FilterProductByColour(ref query, param.Colour);
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

        //public async Task<Result<CreateProductResponse>> UpdateProduct(int id, CreateProductRequest request)
        //{
        //    var result = new Result<CreateProductResponse>();
        //    try
        //    {
        //        var product = await _productRepository.FindAsync(p => p.ProductId == id);
        //        var model = _mapper.Map(request, product);
        //        await _productRepository.UpdateAsync(model);
        //        result.Content = _mapper.Map<CreateProductResponse>(model);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}

        //public async Task<Result<bool>> DeleteProduct(int id)
        //{
        //    var result = new Result<bool>();
        //    try
        //    {
        //        var roduct = await _productRepository.FindAsync(s => s.ProductId == id);

        //        if (roduct is null)
        //        {
        //            result.Content = false;
        //            return result;
        //        }

        //        roduct.Status = false;
        //        await _productRepository.UpdateAsync(roduct);
        //        result.Content = true;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}

        //public async Task<PagedResult<GetListProductResponse>> GetListProductFitWithCustomer(string token, QueryStringParameters param)
        //{
        //    try
        //    {
        //        var accountUId = DecodeToken.DecodeTokenToGetUid(token);
        //        var customer = await _customerRepository.FindAsync(c => c.Uid == accountUId);
        //        var height = customer.Height;
        //        var weight = customer.Weight;
        //        var s = new SearchProductsParameter { PageNumber = param.PageNumber, PageSize = param.PageSize};

        //        if (height == null || weight == null)
        //        {
        //            return await GetListProductsWithAllStatus(s);
        //        }

        //        if(customer.Gender != null)
        //        {
        //            if (customer.Gender == true)
        //            {
        //                s.Gender = "Male";
        //                if (height < 176 || weight < 76)
        //                {
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else if(height >= 176 && weight >= 76 && height < 182 && weight < 86)
        //                {
        //                    s.Size = "XL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else if (height >= 182 && weight >= 86 && height < 188 && weight < 96)
        //                {
        //                    s.Size = "XXL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else if (height >= 188 && weight >= 96 && height < 194 && weight < 101)
        //                {
        //                    s.Size = "XXXL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else if (height >= 188 && weight >= 101 && height < 195 && weight < 116)
        //                {
        //                    s.Size = "4XL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else if (height >= 188 && weight >= 115 && height < 196 && weight < 121)
        //                {
        //                    s.Size = "5XL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else
        //                {
        //                    s.Size = "6XL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //            }
        //            else
        //            {
        //                s.Gender = "Female";
        //                if (height < 168 || weight < 66)
        //                {
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else if (height >= 168 && weight >= 66 && height < 176 && weight < 76)
        //                {
        //                    s.Size = "XL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else if (height >= 176 && weight >= 76 && height < 182 && weight < 86)
        //                {
        //                    s.Size = "XXL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else if (height >= 182 && weight >= 86 && height < 188 && weight < 91)
        //                {
        //                    s.Size = "XXXL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else if (height >= 182 && weight >= 91 && height < 189 && weight < 106)
        //                {
        //                    s.Size = "4XL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else if (height >= 182 && weight >= 106 && height < 190 && weight < 111)
        //                {
        //                    s.Size = "5XL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //                else
        //                {
        //                    s.Size = "6XL";
        //                    return await GetListProductsWithAllStatus(s);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ////////////////////////////////
        //            return await GetListProductsWithAllStatus(s);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

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
    }
}
