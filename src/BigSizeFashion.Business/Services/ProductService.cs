using AutoMapper;
using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
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
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly ISizeService _sizeService;
        //private readonly IProductDetailService _productDetailService;
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
            IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetail> orderDetailRepository,
            IGenericRepository<Account> accountRepository,
            ISizeService sizeService,
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
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _sizeService = sizeService;
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
                    FilterProductByGender(ref query, param.Gender);
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

        private void FilterProductByGender(ref IQueryable<Product> query, string gender)
        {
            if (!query.Any() || String.IsNullOrEmpty(gender) || String.IsNullOrWhiteSpace(gender))
            {
                return;
            }
            if(gender.Equals("Male"))
            {
                query = query.Where(q => q.Gender == true);
            }
            else
            {
                query = query.Where(q => q.Gender == false);
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
                        model.PromotionId = pd.Promotion.PromotionId;
                        model.PromotionName = pd.Promotion.PromotionName;
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
                if (roduct.Status)
                {
                    roduct.Status = false;
                }
                else
                {
                    roduct.Status = true;
                }
                
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

        public async Task<PagedResult<GetListProductResponse>> GetListProductFitWithCustomer(string token, GetListProductFitWithCustomerParameters param)
        {
            try
            {
                var accountUId = DecodeToken.DecodeTokenToGetUid(token);
                var customer = await _customerRepository.FindAsync(c => c.Uid == accountUId);
                var height = customer.Height;
                var weight = customer.Weight;
                var s = new SearchProductsParameter {ProductName = param.ProductName, Category = param.CategoryName , PageNumber = param.PageNumber, PageSize = param.PageSize, Status = true };

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

        public async Task<Result<GetQuantityOfProductResponse>> GetQuantityOfProductInStore(string token, GetQuantityOfProductInStoreParameter param)
        {
            var result = new Result<GetQuantityOfProductResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var account = await _accountRepository.FindAsync(a => a.Uid == uid);
                var storeId = 0;

                if (account.RoleId == 1)
                {
                    //case chỉ 1 kho tổng
                    storeId = await _storeRepository.GetAllByIQueryable()
                                    .Where(s => s.IsMainWarehouse == true && s.Status == true)
                                    .Select(s => s.StoreId)
                                    .FirstOrDefaultAsync();
                }
                else
                {
                    storeId = await _staffRepository.GetAllByIQueryable()
                                 .Where(s => s.Uid == uid)
                                 .Select(s => s.StoreId)
                                 .FirstOrDefaultAsync();
                }

                if(storeId != 0)
                {
                    var productDetailId = await _productDetailRepository
                    .GetAllByIQueryable()
                    .Where(p => p.ProductId == param.ProductId && p.ColourId == param.ColourId && p.SizeId == param.SizeId)
                    .Select(p => p.ProductDetailId).FirstOrDefaultAsync();

                    var quantity = await _storeWarehouseRepository.GetAllByIQueryable()
                                    .Where(s => s.StoreId == storeId && s.ProductDetailId == productDetailId)
                                    .Select(s => s.Quantity)
                                    .FirstOrDefaultAsync();

                    result.Content = new GetQuantityOfProductResponse { ProductDetailId = productDetailId, StoreId = storeId, Quantity = quantity };
                    return result;
                }
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, "Cửa hàng không hợp lệ.");
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<IEnumerable<ColourResponse>>> GetAllColourOfProduct(int id)
        {
            var result = new Result<IEnumerable<ColourResponse>>();
            try
            {
                var listId = await _productDetailRepository.GetAllByIQueryable()
                    .Where(p => p.ProductId == id)
                    .Select(p => p.ColourId).Distinct().ToListAsync();
                var colours = new List<ColourResponse>();
                foreach (var i in listId)
                {
                    var c = await _colourRepository.FindAsync(cc => cc.ColourId == i && cc.Status == true);
                    colours.Add(_mapper.Map<ColourResponse>(c));
                }
                result.Content = colours;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<IEnumerable<SizeResponse>>> GetAllSizeOfProduct(int productId, int colourId)
        {
            var result = new Result<IEnumerable<SizeResponse>>();
            try
            {
                var listId = await _productDetailRepository.GetAllByIQueryable()
                    .Where(p => p.ProductId == productId && p.ColourId == colourId)
                    .Select(p => p.SizeId).Distinct().ToListAsync();
                var sizes = new List<SizeResponse>();
                foreach (var i in listId)
                {
                    var c = await _sizeRepository.FindAsync(cc => cc.SizeId == i && cc.Status == true);
                    sizes.Add(_mapper.Map<SizeResponse>(c));
                }
                result.Content = sizes;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<IEnumerable<GetListProductResponse>>> GetTopTenBestSeller()
        {
            var result = new Result<IEnumerable<GetListProductResponse>>();
            try
            {
                var month = DateTime.UtcNow.AddHours(7).Month;
                var year = DateTime.UtcNow.AddHours(7).Year;
                if(month == 1)
                {
                    month = 12;
                    year -= 1;
                }
                else
                {
                    month -= 1;
                }
                //var orders = await _orderRepository.GetAllByIQueryable()
                //    .Include(o => o.OrderDetails)
                //    .Where(o => o.CreateDate.Month == month && o.CreateDate.Year == year && o.Status == 5)
                //    .ToListAsync();

                var o = await _orderDetailRepository.GetAllByIQueryable().Include(o => o.Order)
                    .Where(o => o.Order.ApprovalDate.Value.Month == month 
                            && o.Order.ApprovalDate.Value.Year == year
                            && o.Order.Status != 0
                            && o.Order.Status != 1
                            && o.Order.Status != 6)
                    .ToListAsync();

                var listproductdetail = new Dictionary<int, int>();
                var listproduct = new Dictionary<int, int>();
                
                foreach (var item in o)
                {
                    if(listproductdetail.ContainsKey(item.ProductDetailId))
                    {
                        listproductdetail[item.ProductDetailId] += item.Quantity;
                    }
                    else
                    {
                        listproductdetail.Add(item.ProductDetailId, item.Quantity);
                    }
                }

                foreach (var item in listproductdetail)
                {
                    var pd = await _productDetailRepository.FindAsync(p => p.ProductDetailId == item.Key);

                    if(listproduct.ContainsKey(pd.ProductId))
                    {
                        listproduct[pd.ProductId] += item.Value;
                    }
                    else
                    {
                        listproduct.Add(pd.ProductId, item.Value);
                    }
                }
                //var cc = listproduct.OrderByDescending(s => s.Value);
                var cc = listproduct.ToList();
                cc.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                var list = new List<GetListProductResponse>();

                for (int i = 0; i < cc.Count(); i++)
                {
                    var product = await _productRepository.FindAsync(p => p.ProductId == cc.ElementAt(i).Key && p.Status == true);
                    if (product != null)
                    {
                        var pp = _mapper.Map<GetListProductResponse>(product);
                        var image = await _imageRepository.FindAsync(x => x.ProductId == pp.ProductId && x.IsMainImage == true);
                        if (image != null)
                        {
                            pp.ImageUrl = image.ImageUrl;
                        }
                        else
                        {
                            pp.ImageUrl = CommonConstants.NoImageUrl;
                        }

                        var now = DateTime.UtcNow.AddHours(7);
                        var pd = await _promotionDetailRepository.GetAllByIQueryable()
                                        .Include(p => p.Promotion)
                                        .Where(p => p.Promotion.ApplyDate <= now
                                                    && p.Promotion.ExpiredDate >= now
                                                    && p.Promotion.Status == true
                                                    && p.ProductId == pp.ProductId)
                                        .FirstOrDefaultAsync();
                        if (pd != null)
                        {
                            var unroundPrice = ((decimal)(100 - pd.Promotion.PromotionValue) / 100) * pp.Price;
                            pp.PromotionPrice = Math.Round(unroundPrice / 1000, 0) * 1000;
                            pp.PromotionValue = pd.Promotion.PromotionValue + "%";
                        }
                        list.Add(pp);
                    }
                }
                var response = list.Skip(0).Take(10);
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<IEnumerable<QuantityFitProductByCategoryResponse>>> GetQuantityFitProductByCategory(string token)
        {
            var result = new Result<IEnumerable<QuantityFitProductByCategoryResponse>>();
            try
            {
                var accountUId = DecodeToken.DecodeTokenToGetUid(token);
                var customer = await _customerRepository.FindAsync(c => c.Uid == accountUId);
                var height = customer.Height;
                var weight = customer.Weight;
                string size;
               

                if (customer.Gender == true)
                    {

                        if (height < 176 || weight < 76)
                        {
                        size = "";
                        }
                        else if (height >= 176 && weight >= 76 && height < 182 && weight < 86)
                        {
                            size = "XL";
                        }
                        else if (height >= 182 && weight >= 86 && height < 188 && weight < 96)
                        {
                           size = "XXL";
                        }
                        else if (height >= 188 && weight >= 96 && height < 194 && weight < 101)
                        {
                            size = "XXXL";
                        }
                        else if (height >= 188 && weight >= 101 && height < 195 && weight < 116)
                        {
                        size = "4XL";
                        }
                        else if (height >= 188 && weight >= 115 && height < 196 && weight < 121)
                        {
                        size = "5XL";
                        }
                        else
                        {
                        size = "6XL";
                        }
                    }
                    else
                    {
                        if (height < 168 || weight < 66)
                        {
                        size = "";
                        }
                        else if (height >= 168 && weight >= 66 && height < 176 && weight < 76)
                        {
                        size = "XL";
                        }
                        else if (height >= 176 && weight >= 76 && height < 182 && weight < 86)
                        {
                        size = "XXL";
                        }
                        else if (height >= 182 && weight >= 86 && height < 188 && weight < 91)
                        {
                        size = "XXXL";
                        }
                        else if (height >= 182 && weight >= 91 && height < 189 && weight < 106)
                        {
                        size = "4XL";
                        }
                        else if (height >= 182 && weight >= 106 && height < 190 && weight < 111)
                        {
                        size = "5XL";
                        }
                        else
                        {
                        size = "6XL";
                        }
                    }
                //getSizeId
               var sizeId = (await _sizeService.GetAllSize(new SearchSizeParameter()
                {
                    Size = size,
                    Status = true
                })).Content.FirstOrDefault().SizeId;

                var query =  _productRepository.GetAllByIQueryable()
                                   .Include(p => p.Category)
                                   .Where(p => p.Status == true)
                                   .Include(p => p.ProductDetails)
                                   .ThenInclude(p => p.Size)
                                   .Include(p => p.ProductDetails)
                                   .ThenInclude(p => p.Colour)
                                   .AsQueryable();


                FilterProductByColourAndSize(ref query, null, size);
                var listProduct = await query.Where(p => p.Gender == customer.Gender).ToListAsync();
                var groupforCategory = listProduct.GroupBy(p => p.CategoryId).Select(pd =>
                    new QuantityFitProductByCategoryResponse()
                    {
                        CategoryId = pd.Key,
                        QuantityFitProduct = pd.Count()
                    }
                    );
              
                

                result = new Result<IEnumerable<QuantityFitProductByCategoryResponse>>();
                result.Content = groupforCategory;
                return result;
                //getProductDetailId



            }
            catch(Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<GetDetailProductResponse>> GetProductByDetailID(int productId, int productDetailId)
        {
            var result = new Result<GetDetailProductResponse>();
            try
            {
                var product = await _productRepository.GetAllByIQueryable()
                                    .Include(p => p.Category)
                                    .Where(p => p.ProductId == productId)
                                    .Include(p => p.ProductDetails)
                                    .ThenInclude(p => p.Size)
                                    .Include(p => p.ProductDetails)
                                    .ThenInclude(p => p.Colour)
                                    .FirstOrDefaultAsync();

                if (product != null)
                {
                    var model = _mapper.Map<GetDetailProductResponse>(product);

                    var productDetail = await _productDetailRepository.GetAllByIQueryable()
                                        .Include(p => p.Size)
                                        .Include(p => p.Colour)
                                        .Where(p => p.ProductDetailId == productDetailId).FirstOrDefaultAsync();

                    var list = new List<ProductDetailResponse>();
                    var x = new ProductDetailResponse
                    {
                        ProductDetailId = productDetail.ProductDetailId,
                        ProductId = productDetail.ProductId,
                        Size = _mapper.Map<SizeResponse>(productDetail.Size),
                        Colour = _mapper.Map<ColourResponse>(productDetail.Colour)
                    };
                    list.Add(x);
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
                        model.PromotionId = pd.Promotion.PromotionId;
                        model.PromotionName = pd.Promotion.PromotionName;
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

        public async Task<Result<GetDetailProductResponse>> GetDetailFitProductWithCustomer(string token, int id)
        {
            var result = new Result<GetDetailProductResponse>();
            try
            {
                var accountUId = DecodeToken.DecodeTokenToGetUid(token);
                var customer = await _customerRepository.FindAsync(c => c.Uid == accountUId);
                var height = customer.Height;
                var weight = customer.Weight;
                var size = string.Empty;


                if (height == null || weight == null)
                {
                    return await GetProductByID(id);
                }

                if (customer.Gender != null)
                {
                    if (customer.Gender == true)
                    {
                        if (height < 176 || weight < 76)
                        {
                            return await GetProductByID(id);
                        }
                        else if (height >= 176 && weight >= 76 && height < 182 && weight < 86)
                        {
                            size = "XL";
                        }
                        else if (height >= 182 && weight >= 86 && height < 188 && weight < 96)
                        {
                            size = "XXL";
                        }
                        else if (height >= 188 && weight >= 96 && height < 194 && weight < 101)
                        {
                            size = "XXXL";
                        }
                        else if (height >= 188 && weight >= 101 && height < 195 && weight < 116)
                        {
                            size = "4XL";
                        }
                        else if (height >= 188 && weight >= 115 && height < 196 && weight < 121)
                        {
                            size = "5XL";
                        }
                        else
                        {
                            size = "6XL";
                        }
                    }
                    else
                    {
                        if (height < 168 || weight < 66)
                        {
                            return await GetProductByID(id);
                        }
                        else if (height >= 168 && weight >= 66 && height < 176 && weight < 76)
                        {
                            size = "XL";
                        }
                        else if (height >= 176 && weight >= 76 && height < 182 && weight < 86)
                        {
                            size = "XXL";
                        }
                        else if (height >= 182 && weight >= 86 && height < 188 && weight < 91)
                        {
                            size = "XXXL";
                        }
                        else if (height >= 182 && weight >= 91 && height < 189 && weight < 106)
                        {
                            size = "4XL";
                        }
                        else if (height >= 182 && weight >= 106 && height < 190 && weight < 111)
                        {
                            size = "5XL";
                        }
                        else
                        {
                            size = "6XL";
                        }
                    }
                }
                else
                {
                    return await GetProductByID(id);
                }

                var product = await _productRepository.GetAllByIQueryable()
                                    .Include(p => p.Category)
                                    .Where(p => p.ProductId == id && p.Status == true)
                                    .FirstOrDefaultAsync();

                if (product != null)
                {
                    var model = _mapper.Map<GetDetailProductResponse>(product);

                    var productDetails = await _productDetailRepository.GetAllByIQueryable()
                                        .Include(p => p.Size)
                                        .Include(p => p.Colour)
                                        .Where(p => p.ProductId == id && p.Size.SizeName == size).ToListAsync();

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
                        model.PromotionId = pd.Promotion.PromotionId;
                        model.PromotionName = pd.Promotion.PromotionName;
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

        public async Task<Result<IEnumerable<GetAllProductToImportResponse>>> GetAllProductToImport()
        {
            var result = new Result<IEnumerable<GetAllProductToImportResponse>>();
            var list = new List<GetAllProductToImportResponse>();
            try
            {
                var products = await _productDetailRepository
                    .GetAllByIQueryable()
                    .Include(p => p.Size)
                    .Include(p => p.Colour)
                    .Include(p => p.Product)
                    .Where(p => p.Product.Status == true)
                    .ToListAsync();

                foreach (var item in products)
                {
                    list.Add(new GetAllProductToImportResponse
                    {
                        ProductName = item.Product.ProductName,
                        SizeName = item.Size.SizeName,
                        ColourName = item.Colour.ColourName,
                        ProductId = item.ProductId,
                        ColourId = item.ColourId,
                        SizeId = item.SizeId,
                        ProductDeatailId = item.ProductDetailId
                    }) ;
                }
                result.Content = list;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<IEnumerable<GetAllProductToAddIntoPromotionResponse>>> GetAllProductToAddIntoPromotion()
        {
            var result = new Result<IEnumerable<GetAllProductToAddIntoPromotionResponse>>();
            var list = new List<GetAllProductToAddIntoPromotionResponse>();
            try
            {
                var products = await _productRepository
                    .GetAllByIQueryable()
                    .Where(p => p.Status == true)
                    .ToListAsync();

                foreach (var item in products)
                {
                    list.Add(new GetAllProductToAddIntoPromotionResponse
                    {
                        ProductName = item.ProductName,
                        ProductId = item.ProductId
                    });
                }
                result.Content = list;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<GetQuantityOfProductInAllStoreResponse>> GetQuantityOfProductInAllStore(int id)
        {
            var result = new Result<GetQuantityOfProductInAllStoreResponse>();
            try
            {
                var cc = await _storeWarehouseRepository
                    .GetAllByIQueryable()
                    .Include(s => s.Store)
                    .Where(s => s.ProductDetailId == id && s.Store.Status == true)
                    .ToListAsync();

                result.Content = new GetQuantityOfProductInAllStoreResponse { ProductDetailId = id, Quantity = cc.Select(c => c.Quantity).Sum() };
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
