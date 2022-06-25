using AutoMapper;
using BigSizeFashion.Business.Dtos.RequestObjects;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.ResponseObjects;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
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
    public class CartService : ICartService
    {
        private readonly IGenericRepository<CustomerCart> _genericRepository;
        private readonly IGenericRepository<StoreWarehouse> _storeWarehouseRepository;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public CartService(
            IGenericRepository<CustomerCart> genericRepository,
            IGenericRepository<StoreWarehouse> storeWarehouseRepository,
            IMapper mapper,
            IProductService productService
            )
        {
            _genericRepository = genericRepository;
            _storeWarehouseRepository = storeWarehouseRepository;
            _mapper = mapper;
            _productService = productService;
        }

        

        public async Task<Result<AddToCartResponse>> AddToCart(AddToCartRequest request, string token)
        {
            var uid = DecodeToken.DecodeTokenToGetUid(token);
            var result = new Result<AddToCartResponse>();
            try
            {
                var customerOrder = _mapper.Map<CustomerCart>(request);
                customerOrder.CustomerId = uid;
                //var price = customerOrder.ProductDetail.Product.Price;
                await _genericRepository.InsertAsync(customerOrder);
                await _genericRepository.SaveAsync();

                //var token = GenerateJSONWebToken(account.Uid.ToString(), customer.Fullname, account.Role.RoleName);
                result.Content = new AddToCartResponse
                {
                    CustomerId = uid,
                    ProductDetailId = request.ProductDetailId,
                    Quantity = request.Quantity,
                    StoreId = request.StoreId
                };
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<List<AddToCartResponse>>> AddToListCart(List<AddToCartRequest> request, string token)
        {
            var uid = DecodeToken.DecodeTokenToGetUid(token);
            var result = new Result<List<AddToCartResponse>>();
            result.Content = new List<AddToCartResponse>();
            try
            {
                await deleteCart(uid);
                foreach (var customerCart in request)
                {
                    var customerOrder = _mapper.Map<CustomerCart>(customerCart);
                    customerOrder.CustomerId = uid;

                    //var price = customerOrder.ProductDetail.Product.Price;
                    await _genericRepository.InsertAsync(customerOrder);
                    await _genericRepository.SaveAsync();

                    //var token = GenerateJSONWebToken(account.Uid.ToString(), customer.Fullname, account.Role.RoleName);
                    result.Content.Add(new AddToCartResponse
                    {
                        CustomerId = uid,
                        ProductDetailId = customerCart.ProductDetailId,
                        Quantity = customerCart.Quantity,
                        StoreId = customerCart.StoreId
                    });
                }
                //var token = GenerateJSONWebToken(account.Uid.ToString(), customer.Fullname, account.Role.RoleName);


                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task deleteCart(int uid)
        {
            await _genericRepository.DeleteSpecificFieldByAsync(p => p.CustomerId == uid);
            await _genericRepository.SaveAsync();

        }

        public async Task<Result<List<CartResponse>>> getListCart(string authorization)
        {
            var uid = DecodeToken.DecodeTokenToGetUid(authorization);
            var result = new Result<List<CartResponse>>();
            result.Content = new List<CartResponse>();
            try
            {
                var listCart = await _genericRepository.GetAllByIQueryable().Where(c => c.CustomerId == uid)
                .Include(c => c.ProductDetail)
                .ThenInclude(pd => pd.Product).
                ThenInclude(p => p.PromotionDetails).
                ThenInclude(pp => pp.Promotion).ToListAsync();
                
                foreach (var cart in listCart)
                {
                    var product = await _productService.
                    GetProductByID(cart.ProductDetail.Product.ProductId);

                  

                    var cartResponse = new CartResponse()
                    {
                        ProductDetailId = cart.ProductDetailId,
                        ProductImage = product.Content.Images.Where(image => image.IsMainImage == true).FirstOrDefault().ImageUrl,
                        ProductName = product.Content.ProductName,
                        ProductPrice = product.Content.Price,
                        ProductPromotion = product.Content.PromotionPrice,
                        StoreId = cart.StoreId
                    };

                    result.Content.Add(cartResponse);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }







        //public async Task<Result<bool>> AddProductToCart(string token, ManageProductInCartRequest request)
        //{
        //    var result = new Result<bool>();
        //    try
        //    {
        //        var uid = DecodeToken.DecodeTokenToGetUid(token);
        //        var product = await _storeWarehouseRepository.GetAllByIQueryable()
        //            .Where(s => s.ProductId == request.ProductId && s.StoreId == request.StoreId)
        //            .Include(s => s.Product)
        //            .FirstOrDefaultAsync();

        //        if(product == null)
        //        {
        //            result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.OutOfProduct);
        //            return result;
        //        }

        //        if(product.Quantity > 0)
        //        {
        //            var cart = await _genericRepository.FindAsync(s => s.ProductId == request.ProductId && s.StoreId == request.StoreId && s.CustomerId == uid);
        //            if(cart != null)
        //            {
        //                cart.Quantity = cart.Quantity + 1;
        //                cart.Price = cart.Quantity * product.Product.Price;
        //                await _genericRepository.UpdateAsync(cart);
        //                await _genericRepository.SaveAsync();
        //                result.Content = true;
        //                return result;
        //            }
        //            else
        //            {
        //                var newItem = new CustomerCart
        //                {
        //                    CustomerId = uid,
        //                    ProductId = request.ProductId,
        //                    StoreId = request.StoreId,
        //                    Quantity = 1,
        //                    Price = product.Product.Price
        //                };
        //                await _genericRepository.InsertAsync(newItem);
        //                await _genericRepository.SaveAsync();
        //                result.Content = true;
        //                return result;
        //            }
        //        }
        //        else
        //        {
        //            result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.OutOfProduct);
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}

        //public async Task<Result<IEnumerable<CartItemResponse>>> DecreaseProductInCart(string token, ManageProductInCartRequest request)
        //{
        //    var result = new Result<IEnumerable<CartItemResponse>>();
        //    try
        //    {
        //        var uid = DecodeToken.DecodeTokenToGetUid(token);
        //        var product = await _storeWarehouseRepository.GetAllByIQueryable()
        //            .Where(s => s.ProductId == request.ProductId)
        //            .Include(s => s.Product)
        //            .FirstOrDefaultAsync();

        //        var cart = await _genericRepository.FindAsync(s => s.ProductId == request.ProductId && s.StoreId == request.StoreId && s.CustomerId == uid);
        //        cart.Quantity = cart.Quantity - 1;

        //        if(cart.Quantity > 0)
        //        {
        //            cart.Price = cart.Quantity * product.Product.Price;
        //            await _genericRepository.UpdateAsync(cart);
        //            await _genericRepository.SaveAsync();
        //            return await GetAllProductInCartByCustomerID(token);
        //        }
        //        else
        //        {
        //            await _genericRepository.DeleteSpecificFieldByAsync(s => s.ProductId == request.ProductId && s.StoreId == request.StoreId && s.CustomerId == uid);
        //            await _genericRepository.SaveAsync();
        //            return await GetAllProductInCartByCustomerID(token);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}

        //public async Task<Result<IEnumerable<CartItemResponse>>> GetAllProductInCartByCustomerID(string token)
        //{
        //    var result = new Result<IEnumerable<CartItemResponse>>();
        //    try
        //    {
        //        var uid = DecodeToken.DecodeTokenToGetUid(token);
        //        var cartItems = await _genericRepository.FindByAsync(c => c.CustomerId == uid);
        //        result.Content = _mapper.Map<List<CartItemResponse>>(cartItems);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}

        //public async Task<Result<IEnumerable<CartItemResponse>>> IncreaseProductInCart(string token, ManageProductInCartRequest request)
        //{
        //    var result = new Result<IEnumerable<CartItemResponse>>();
        //    try
        //    {
        //        var uid = DecodeToken.DecodeTokenToGetUid(token);
        //        var product = await _storeWarehouseRepository.GetAllByIQueryable()
        //            .Where(s => s.ProductId == request.ProductId && s.StoreId == request.StoreId)
        //            .Include(s => s.Product)
        //            .FirstOrDefaultAsync();

        //        if (product == null)
        //        {
        //            result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.OutOfProduct);
        //            return result;
        //        }

        //        if (product.Quantity > 0)
        //        {
        //            var cart = await _genericRepository.FindAsync(s => s.ProductId == request.ProductId && s.StoreId == request.StoreId && s.CustomerId == uid);
        //            cart.Quantity = cart.Quantity + 1;
        //            cart.Price = cart.Quantity * product.Product.Price;
        //            await _genericRepository.UpdateAsync(cart);
        //            await _genericRepository.SaveAsync();
        //            return await GetAllProductInCartByCustomerID(token);
        //        }
        //        else
        //        {
        //            result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.OutOfProduct);
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}

        //public async Task<Result<IEnumerable<CartItemResponse>>> RemoveProductInCart(string token, ManageProductInCartRequest request)
        //{
        //    var result = new Result<IEnumerable<CartItemResponse>>();
        //    try
        //    {
        //        var uid = DecodeToken.DecodeTokenToGetUid(token);
        //        var cart = await _genericRepository.FindAsync(s => s.ProductId == request.ProductId && s.StoreId == request.StoreId && s.CustomerId == uid);

        //        await _genericRepository.DeleteSpecificFieldByAsync(s => s.ProductId == request.ProductId && s.StoreId == request.StoreId && s.CustomerId == uid);
        //        await _genericRepository.SaveAsync();
        //        return await GetAllProductInCartByCustomerID(token);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
        //        return result;
        //    }
        //}
    }
}
