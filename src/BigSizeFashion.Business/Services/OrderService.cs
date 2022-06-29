using AutoMapper;
using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Enums;
using BigSizeFashion.Business.Helpers.Parameters;
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
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IGenericRepository<staff> _staffRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<StoreWarehouse> _storeWarehouseRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<ProductDetail> _productDetailRepository;
        private readonly IGenericRepository<ProductImage> _productImageRepository;
    
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IStoreService _storeService;
        private readonly IAddressService _addressService;
        private readonly IOrderDetailService _orderDetailService;



        private readonly IMapper _mapper;

        public OrderService(IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetail> orderDetailRepository,
            IGenericRepository<staff> staffRepository,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<StoreWarehouse> storeWarehouseRepository,
            IGenericRepository<Account> accountRepository,
            IGenericRepository<ProductImage> productImageRepository,
            IGenericRepository<ProductDetail> productDetailRepository,
            IProductService productService,
            ICartService cartService,
            IStoreService storeService,
            IAddressService addressService,
            IOrderDetailService orderDetailService,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _staffRepository = staffRepository;
            _customerRepository = customerRepository;
            _storeWarehouseRepository = storeWarehouseRepository;
            _accountRepository = accountRepository;
            _productDetailRepository = productDetailRepository;
            _productImageRepository = productImageRepository;
            _productService = productService;
            _cartService = cartService;
            _storeService = storeService;
            _addressService = addressService;
            _orderDetailService = orderDetailService;
            _mapper = mapper;
        }

        public async Task<Result<OrderIdResponse>> CreateOrderForCustomer(string token, CreateOrderForCustomerRequest request)
        {
            var result = new Result<OrderIdResponse>();
            try
            {
                var staffUid = DecodeToken.DecodeTokenToGetUid(token);
                var staff = await _staffRepository.FindAsync(s => s.Uid == staffUid);
                var customer = await _customerRepository.FindAsync(c => c.PhoneNumber.Equals(request.CustomerPhoneNumber));
                if (customer == null)
                {
                    result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.NotExistedUser);
                    return result;
                }

                decimal totalPrice = 0;
                decimal? totalDiscount = 0;

                foreach (var item in request.ListProduct)
                {
                    var ProductId = await _productDetailRepository
                        .GetAllByIQueryable()
                        .Where(p => p.ProductDetailId == item.ProductDetailId)
                        .Select(p => p.ProductId).FirstOrDefaultAsync();

                    var p = await _productService.GetProductPrice(ProductId);
                    var dp = await _productService.GetProductPromotionPrice(ProductId);

                    totalPrice += p * item.Quantity;

                    if (dp != null)
                    {
                        totalDiscount += dp * item.Quantity;
                    }
                    else
                    {
                        totalDiscount += p * item.Quantity;
                    }

                    var storeWarehouse = await _storeWarehouseRepository.FindAsync(s => s.StoreId == staff.StoreId && s.ProductDetailId == item.ProductDetailId);
                    storeWarehouse.Quantity -= item.Quantity;
                    await _storeWarehouseRepository.UpdateAsync(storeWarehouse);
                }

                var order = new Order
                {
                    CustomerId = customer.Uid,
                    StoreId = staff.StoreId,
                    StaffId = staff.Uid,
                    CreateDate = DateTime.UtcNow.AddHours(7),
                    TotalPrice = totalPrice,
                    TotalPriceAfterDiscount = totalDiscount,
                    OrderType = false,
                    PaymentMethod = request.PaymentMethod,
                    ApprovalDate = DateTime.UtcNow.AddHours(7),
                    PackagedDate = DateTime.UtcNow.AddHours(7),
                    DeliveryDate = DateTime.UtcNow.AddHours(7),
                    ReceivedDate = DateTime.UtcNow.AddHours(7),
                    Status = (byte)OrderStatusEnum.Received
                };
                await _orderRepository.InsertAsync(order);
                await _orderRepository.SaveAsync();

                foreach (var item in request.ListProduct)
                {
                    var ProductId = await _productDetailRepository
                        .GetAllByIQueryable()
                        .Where(p => p.ProductDetailId == item.ProductDetailId)
                        .Select(p => p.ProductId).FirstOrDefaultAsync();

                    var od = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductDetailId = item.ProductDetailId,
                        Quantity = item.Quantity,
                        PricePerOne = await _productService.GetProductPrice(ProductId),
                        Price = await _productService.GetProductPrice(ProductId) * item.Quantity,
                        DiscountPricePerOne = await _productService.GetProductPromotionPrice(ProductId),
                        DiscountPrice = await _productService.GetProductPromotionPrice(ProductId) * item.Quantity
                    };
                    await _orderDetailRepository.InsertAsync(od);
                }
                await _orderDetailRepository.SaveAsync();
                result.Content = new OrderIdResponse { OrderId = order.OrderId };
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<PagedResult<ListOrderResponse>> GetListOrderForCustomer(string token, FilterOrderParameter param)
        {
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var orders = await _orderRepository.FindByAsync(o => o.CustomerId == uid);
                var query = orders.AsQueryable();
                FilterOrderByType(ref query, param.OrderType);
                FilterOrderStatus(ref query, param.OrderStatus.ToString());
                OrderByCreateDate(ref query, param.OrderByCreateDate);
                var list = query.ToList();
                var response = _mapper.Map<List<ListOrderResponse>>(list);
                return PagedResult<ListOrderResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void FilterOrderStatus(ref IQueryable<Order> query, string status)
        {
            if (!query.Any() || String.IsNullOrEmpty(status) || String.IsNullOrWhiteSpace(status) || status.Equals(OrderStatusEnum.All.ToString()))
            {
                return;
            }

            var s = ConvertOrderStatus.ConvertStringToOrderStatus(status);
            query = query.Where(q => q.Status == s);
        }

        private static void FilterOrderByType(ref IQueryable<Order> query, bool? type)
        {
            if (!query.Any() || type is null)
            {
                return;
            }

            if(type is true)
            {
                query = query.Where(q => q.OrderType == true);
            }

            if (type is false)
            {
                query = query.Where(q => q.OrderType == false);
            }
        }

        private void OrderByCreateDate(ref IQueryable<Order> query, bool? orderByCreateDate)
        {
            if (!query.Any())
            {
                return;
            }

            if (orderByCreateDate is true || orderByCreateDate is null)
            {
                query = query.OrderByDescending(x => x.CreateDate);
            }
            else
            {
                query = query.OrderBy(x => x.CreateDate);
            }
        }

        public async Task<Result<GetOrderDetailResponse>> GetOrderDetailById(int id)
        {
            var result = new Result<GetOrderDetailResponse>();
            try
            {
                var order = await _orderRepository.GetAllByIQueryable()
                                .Where(o => o.OrderId == id)
                                .Include(o => o.Customer)
                                .Include(o => o.Staff)
                                .Include(o => o.DeliveryAddressNavigation)
                                .Include(o => o.Store)
                                .Include(o => o.OrderDetails)
                                .FirstOrDefaultAsync();
                if (order == null)
                {
                    result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.NotExistedOrder);
                    return result;
                }
                var response = _mapper.Map<GetOrderDetailResponse>(order);
                response.DeliveryAddress = _mapper.Map<DeliveryAddressResponse>(order.DeliveryAddressNavigation);
                response.Store = _mapper.Map<StoreResponse>(order.Store);
                response.ProductList = _mapper.Map<List<OrderDetailItem>>(order.OrderDetails);

                for (int i = 0; i < response.ProductList.Count; i++)
                {
                    //var product = await _productRepository.GetAllByIQueryable()
                    //                    .Where(p => p.ProductId == response.ProductList[i].ProductId)
                    //                    .Include(p => p.Category)
                    //                    .FirstOrDefaultAsync();

                    var product = await _productDetailRepository.GetAllByIQueryable()
                                          .Where(p => p.ProductDetailId == response.ProductList[i].ProductDetailId)
                                          .Include(p => p.Size)
                                          .Include(p => p.Colour)
                                          .Include(p => p.Product)
                                          .ThenInclude(p => p.Category)
                                          .FirstOrDefaultAsync();

                    response.ProductList[i].ProductId = product.Product.ProductId;
                    response.ProductList[i].ProductName = product.Product.ProductName;
                    response.ProductList[i].Category = product.Product.Category.CategoryName;
                    response.ProductList[i].Colour = product.Colour.ColourName;
                    response.ProductList[i].Size = product.Size.SizeName;
                    var imageUrl = await _productImageRepository.GetAllByIQueryable()
                        .Where(p => p.ProductId == product.Product.ProductId && p.IsMainImage == true)
                        .Select(p => p.ImageUrl)
                        .FirstOrDefaultAsync();
                    if (imageUrl == null)
                    {
                        response.ProductList[i].ProductImageUrl = CommonConstants.NoImageUrl;
                    }
                    else
                    {
                        response.ProductList[i].ProductImageUrl = imageUrl;
                    }
                }
                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> ApproveOrder(int id)
        {
            var result = new Result<bool>();
            try
            {
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);
                order.ApprovalDate = DateTime.UtcNow.AddHours(7);
                order.Status = (byte)OrderStatusEnum.Approved;
                await _orderRepository.UpdateAsync(order);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<PagedResult<ListOrderResponse>> GetListOrderOfStoreForManager(string token, FilterOrderParameter param)
        {
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var storeId = await _staffRepository.GetAllByIQueryable().Where(s => s.Uid == uid).Select(s => s.StoreId).FirstOrDefaultAsync();
                var orders = await _orderRepository.FindByAsync(o => o.StoreId == storeId);
                var query = orders.AsQueryable();
                FilterOrderByType(ref query, param.OrderType);
                FilterOrderStatus(ref query, param.OrderStatus.ToString());
                OrderByCreateDate(ref query, param.OrderByCreateDate);
                var list = query.ToList();
                var response = _mapper.Map<List<ListOrderResponse>>(list);
                return PagedResult<ListOrderResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<bool>> AssignOrder(AssignOrderRequest request)
        {
            var result = new Result<bool>();
            try
            {
                var order = await _orderRepository.FindAsync(o => o.OrderId == request.OrderId);
                order.StaffId = request.StaffId;
                await _orderRepository.UpdateAsync(order);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<PagedResult<ListOrderForStaffResponse>> GetListAssignedOrder(string token, QueryStringParameters param)
        {
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var orders = await _orderRepository.FindByAsync(o => o.StaffId == uid && o.Status == (byte)OrderStatusEnum.Approved);
                var query = orders.AsQueryable();
                OrderByCreateDate(ref query, false);
                var list = _mapper.Map<List<ListOrderForStaffResponse>>(query.ToList());
                for (int i = 0; i < list.Count; i++)
                {
                    var totalQuantity = _orderDetailRepository.GetAllByIQueryable().Where(o => o.OrderId == list[i].OrderId).Select(o => o.Quantity).Sum();
                    list[i].TotalQuantity = totalQuantity;
                }
                return PagedResult<ListOrderForStaffResponse>.ToPagedList(list, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedResult<ListOrderForStaffResponse>> GetListOrderOfStoreForStaff(string token, FilterOrderParameter param)
        {
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var orders = await _orderRepository.GetAllByIQueryable().Where(o => o.StaffId == uid).Include(o => o.Customer).ToListAsync();
                var query = orders.AsQueryable();
                FilterOrderByType(ref query, param.OrderType);
                FilterOrderStatus(ref query, param.OrderStatus.ToString());
                OrderByCreateDate(ref query, param.OrderByCreateDate);
                var response = _mapper.Map<List<ListOrderForStaffResponse>>(query.ToList());
                for (int i = 0; i < response.Count; i++)
                {
                    var totalQuantity = _orderDetailRepository.GetAllByIQueryable().Where(o => o.OrderId == response[i].OrderId).Select(o => o.Quantity).Sum();
                    response[i].TotalQuantity = totalQuantity;
                }
                return PagedResult<ListOrderForStaffResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result<bool>> PackagedOrder(int id)
        {
            var result = new Result<bool>();
            try
            {
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);
                order.PackagedDate = DateTime.UtcNow.AddHours(7);
                order.Status = (byte)OrderStatusEnum.Packaged;
                await _orderRepository.UpdateAsync(order);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DeliveryOrder(int id)
        {
            var result = new Result<bool>();
            try
            {
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);
                order.DeliveryDate = DateTime.UtcNow.AddHours(7);
                order.Status = (byte)OrderStatusEnum.Delivery;
                await _orderRepository.UpdateAsync(order);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> ReceivedOrder(int id)
        {
            var result = new Result<bool>();
            try
            {
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);
                order.ReceivedDate = DateTime.UtcNow.AddHours(7);
                order.Status = (byte)OrderStatusEnum.Received;
                await _orderRepository.UpdateAsync(order);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> RejectOrder(int id)
        {
            var result = new Result<bool>();
            try
            {
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);
                order.RejectedDate = DateTime.UtcNow.AddHours(7);
                order.Status = (byte)OrderStatusEnum.Reject;
                await _orderRepository.UpdateAsync(order);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> CancelOrder(string token, int id)
        {
            var result = new Result<bool>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var role = await _accountRepository.GetAllByIQueryable().Include(a => a.Role).Where(a => a.Uid == uid).Select(a => a.Role.RoleName).FirstOrDefaultAsync();

                var order = await _orderRepository.FindAsync(o => o.OrderId == id);
                if(order != null)
                {
                    if(role.Equals("Customer"))
                    {
                        if(order.Status == (byte)OrderStatusEnum.Pending)
                        {
                            order.Status = (byte)OrderStatusEnum.Cancel;
                            await _orderRepository.UpdateAsync(order);
                            result.Content = true;
                            return result;
                        }
                        else
                        {
                            result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.CannotCancelOrder);
                            return result;
                        }
                    }
                    else
                    {
                        order.Status = (byte)OrderStatusEnum.Cancel;
                        await _orderRepository.UpdateAsync(order);
                        result.Content = true;
                        return result;
                    }
                }
                else
                {
                    result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ErrorMessageConstants.NotExistedOrder);
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<bool> AddOrder(string authorization, OrderRequest request)
        {
            var uid = DecodeToken.DecodeTokenToGetUid(authorization);
            var listCart = await _cartService.getListCart(authorization);
            var listOrderDetailRequest = new List<OrderDetailRequest>();
            var addressResponse = await _addressService.GetAddressById(authorization, request.DeliveryAddress);
            var storeId = await _storeService.GetNearestStore(addressResponse.Content.ReceiveAddress);
            var orderResponse = new OrderResponse()
            {
                CreateDate = DateTime.UtcNow.AddHours(7),
                CustomerId = uid,
                DeliveryAddress = 1,
                PaymentMethod = request.PaymentMethod,
                Status = (byte)OrderStatusEnum.Pending,
                StoreId = storeId,
                OrderType = request.OrderType,
                TotalPrice = request.TotalPrice,
                TotalPriceAfterDiscount = request.PromotionPrice,
            };
            //saveOrder
            var order = _mapper.Map<Order>(orderResponse);
             await _orderRepository.InsertAsync(order);
             await _orderRepository.SaveAsync();

            foreach (var cart in listCart.Content)
            {

                //listOrderResponse.Add(orderResponse);
                var orderdetailRequest = new OrderDetailRequest()
                {
                    OrderId = order.OrderId,
                    DiscountPricePerOne = cart.ProductPromotion,
                    DiscountPrice = cart.ProductPromotion * cart.Quantity,
                    Price = cart.ProductPrice * cart.Quantity,
                    Quantity = cart.Quantity,
                    PricePerOne = cart.ProductPrice,
                    ProductDetailId = cart.ProductDetailId
                };
                listOrderDetailRequest.Add(orderdetailRequest);
                await _orderDetailService.createOrderDetail(listOrderDetailRequest);

            }
            //deleteCart
            await _cartService.AddToListCart(new List<AddToCartRequest>(), authorization);
            ///

            //var result = new Result<List<OrderResponse>>();
            //result.Content = new List<OrderResponse>();
            //result.Content = listOrderResponse;
            return true;
        }
    }
}
