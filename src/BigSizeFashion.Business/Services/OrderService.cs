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
using BigSizeFashion.Business.Services.ZaloPay;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private readonly IFirebaseNotificationService _firebaseNotificationService;


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
            IFirebaseNotificationService firebaseNotificationService,
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
            _firebaseNotificationService = firebaseNotificationService;
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

                    //var storeWarehouse = await _storeWarehouseRepository.FindAsync(s => s.StoreId == staff.StoreId && s.ProductDetailId == item.ProductDetailId);
                    //storeWarehouse.Quantity -= item.Quantity;
                    //await _storeWarehouseRepository.UpdateAsync(storeWarehouse);
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
                    //ApprovalDate = DateTime.UtcNow.AddHours(7),
                    //PackagedDate = DateTime.UtcNow.AddHours(7),
                    //DeliveryDate = DateTime.UtcNow.AddHours(7),
                    //ReceivedDate = DateTime.UtcNow.AddHours(7),
                    Status = (byte)OrderStatusEnum.Pending
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

                var username = await GetManagerUserName(staff.StoreId);
                await _firebaseNotificationService.SendNotification(username, "Đơn hàng offline #" + order.OrderId + " vừa được tạo", "Vui lòng xác thực đơn hàng.", "");

                result.Content = new OrderIdResponse { OrderId = order.OrderId };
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private async Task<string> GetManagerUserName(int storeId)
        {
            var username = await _staffRepository.GetAllByIQueryable()
                .Where(s => s.StoreId == storeId && s.Status == true && s.UidNavigation.RoleId == 2 && s.UidNavigation.Status == true)
                .Select(s => s.UidNavigation.Username)
                .FirstOrDefaultAsync();
            return username;
        }

        private async Task<string> GetStaffUserName(int uid)
        {
            var username = await _staffRepository.GetAllByIQueryable()
                .Where(s => s.Uid == uid && s.Status == true && s.UidNavigation.RoleId == 3 && s.UidNavigation.Status == true)
                .Select(s => s.UidNavigation.Username)
                .FirstOrDefaultAsync();
            return username;
        }

        private async Task<string> GetCustomerUserName(int uid)
        {
            var username = await _customerRepository.GetAllByIQueryable()
                .Where(s => s.Uid == uid && s.Status == true && s.UidNavigation.RoleId == 4 && s.UidNavigation.Status == true)
                .Select(s => s.UidNavigation.Username)
                .FirstOrDefaultAsync();
            return username;
        }

        public async Task<Result<IEnumerable<ListOrderResponse>>> GetListOrderForCustomer(string token, FilterOrderForStaffParameter param)
        {
            var result = new Result<IEnumerable<ListOrderResponse>>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var orders = await _orderRepository
                    .GetAllByIQueryable()
                    .Where(o => o.CustomerId == uid && o.CreateDate.Date == ConvertDateTime.ConvertStringToDate(param.CreateDate).Value.Date)
                    .OrderByDescending(o => o.CreateDate)
                    .ToListAsync();
                //var query = orders.AsQueryable();
                //FilterOrderByType(ref query, param.OrderType);
                //FilterOrderStatus(ref query, param.OrderStatus.ToString());
                //OrderByCreateDate(ref query, param.OrderByCreateDate);
                //var list = query.ToList();
                var response = _mapper.Map<List<ListOrderResponse>>(orders);

                result.Content = response;
                return result;
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

                var acc = await _accountRepository.GetAllByIQueryable()
                        .Include(a => a.staff)
                        .Where(a => a.RoleId == 2 && a.staff.StoreId == order.StoreId && a.Status == true && a.staff.Status == true)
                        .Select(a => a.staff.Fullname)
                        .FirstOrDefaultAsync();
                response.Store.ManagerName = acc;
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

        public async Task<Result<IEnumerable<NotEnoughProductResponse>>> ApproveOrder(int id)
        {
            var result = new Result<IEnumerable<NotEnoughProductResponse>>();
            try
            {
                var list = new List<NotEnoughProductResponse>();
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);
                var ods = await _orderDetailRepository.FindByAsync(o => o.OrderId == id);

                foreach (var item in ods)
                {
                    var storeWarehouse = await _storeWarehouseRepository.GetAllByIQueryable()
                        .Where(s => s.StoreId == order.StoreId && s.ProductDetailId == item.ProductDetailId)
                        .Include(s => s.ProductDetail)
                        .ThenInclude(s => s.Colour)
                        .Include(s => s.ProductDetail)
                        .ThenInclude(s => s.Product)
                        .Include(s => s.ProductDetail)
                        .ThenInclude(s => s.Size)
                        .FirstOrDefaultAsync();
                    if (storeWarehouse.Quantity < item.Quantity)
                    {
                        var cc = new NotEnoughProductResponse
                        {
                            ProductDetailId = storeWarehouse.ProductDetailId,
                            ColourId = storeWarehouse.ProductDetail.ColourId,
                            ColourName = storeWarehouse.ProductDetail.Colour.ColourName,
                            ProductId = storeWarehouse.ProductDetail.ProductId,
                            ProductName = storeWarehouse.ProductDetail.Product.ProductName,
                            SizeId = storeWarehouse.ProductDetail.SizeId,
                            SizeName = storeWarehouse.ProductDetail.Size.SizeName,
                            QuantityInStore = storeWarehouse.Quantity,
                            RequiredQuantity = item.Quantity
                        };
                        list.Add(cc);
                    }
                }

                if(list.Count == 0)
                {
                    order.ApprovalDate = DateTime.UtcNow.AddHours(7);
                    order.Status = (byte)OrderStatusEnum.Approved;
                    await _orderRepository.UpdateAsync(order);

                    foreach (var item in ods)
                    {
                        var storeWarehouse = await _storeWarehouseRepository.FindAsync(s => s.StoreId == order.StoreId && s.ProductDetailId == item.ProductDetailId);
                        storeWarehouse.Quantity -= item.Quantity;
                        await _storeWarehouseRepository.UpdateAsync(storeWarehouse);
                    }

                    var username = await GetCustomerUserName(order.CustomerId);
                    await _firebaseNotificationService.SendNotification(username, "Đơn hàng #" + order.OrderId + " đã được xác nhận", "Đơn hàng của bạn sẽ được giao trong thời gian sớm nhất.", "");
                    result.Content = null;
                    return result;
                }
                else
                {
                    result.Content = list;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<PagedResult<ListOrderForManagerResponse>> GetListOrderOfStoreForManager(string token, FilterOrderParameter param)
        {
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var storeId = await _staffRepository.GetAllByIQueryable().Where(s => s.Uid == uid).Select(s => s.StoreId).FirstOrDefaultAsync();
                var orders = await _orderRepository.FindByAsync(o => o.StoreId == storeId);
                var query = orders.AsQueryable();
                FilterOrderByType(ref query, param.OrderType);
                FilterOrderStatus(ref query, param.OrderStatus.ToString());
                OrderByStatus(ref query, param.OrderStatus);
                //OrderByCreateDate(ref query, param.OrderByCreateDate);
                var list = query.ToList();
                var response = _mapper.Map<List<ListOrderForManagerResponse>>(list);
                for (int i = 0; i < response.Count; i++)
                {
                    var date = GetDateOfOrder(query, response[i].OrderId, param.OrderStatus);
                    response[i].CreateDate = ConvertDateTime.ConvertDateTimeToString(date);
                }
                return PagedResult<ListOrderForManagerResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DateTime? GetDateOfOrder(IQueryable<Order> query, int orderId, OrderStatusEnum? orderStatus)
        {
            if (!query.Any())
            {
                return null;
            }
            
            if (OrderStatusEnum.Approved == orderStatus)
            {
                return query.Where(q => q.OrderId == orderId).Select(q => q.ApprovalDate).FirstOrDefault();
            }
            else if (OrderStatusEnum.Packaged == orderStatus)
            {
                return query.Where(q => q.OrderId == orderId).Select(q => q.PackagedDate).FirstOrDefault();
            }
            else if (OrderStatusEnum.Delivery == orderStatus)
            {
                return query.Where(q => q.OrderId == orderId).Select(q => q.DeliveryDate).FirstOrDefault();
            }
            else if (OrderStatusEnum.Received == orderStatus)
            {
                return query.Where(q => q.OrderId == orderId).Select(q => q.ReceivedDate).FirstOrDefault();
            }
            else if (OrderStatusEnum.Reject == orderStatus)
            {
                return query.Where(q => q.OrderId == orderId).Select(q => q.RejectedDate).FirstOrDefault();
            }
            return query.Where(q => q.OrderId == orderId).Select(q => q.CreateDate).FirstOrDefault();
        }

        private void OrderByStatus(ref IQueryable<Order> query, OrderStatusEnum? orderStatus)
        {
            if (!query.Any())
            {
                return;
            }

            if(OrderStatusEnum.Pending == orderStatus || OrderStatusEnum.Cancel == orderStatus || OrderStatusEnum.All  == orderStatus || orderStatus == null)
            {
                query = query.OrderByDescending(q => q.CreateDate);
            }
            else if(OrderStatusEnum.Approved == orderStatus)
            {
                query = query.OrderByDescending(q => q.ApprovalDate);
            }
            else if(OrderStatusEnum.Packaged == orderStatus)
            {
                query = query.OrderByDescending(q => q.PackagedDate);
            }
            else if (OrderStatusEnum.Delivery == orderStatus)
            {
                query = query.OrderByDescending(q => q.DeliveryDate);
            }
            else if (OrderStatusEnum.Received == orderStatus)
            {
                query = query.OrderByDescending(q => q.ReceivedDate);
            }
            else if (OrderStatusEnum.Reject == orderStatus)
            {
                query = query.OrderByDescending(q => q.RejectedDate);
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

                var username = await GetStaffUserName(order.StaffId.Value);
                var cc = await _firebaseNotificationService.SendNotification(username, "Phân công đơn hàng #" + order.OrderId, "Bạn được phân công đóng gói và xử lý đơn hàng #" + order.OrderId, "");
            
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<IEnumerable<ListOrderForStaffResponse>>> GetListAssignedOrder(string token, FilterOrderForStaffParameter param)
        {
            var result = new Result<IEnumerable<ListOrderForStaffResponse>>();
            try
            {
                var date = ConvertDateTime.ConvertStringToDate(param.CreateDate);
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var orders = await _orderRepository.FindByAsync(
                    o => o.StaffId == uid
                    && o.CreateDate.Day == date.Value.Day
                    && o.CreateDate.Month == date.Value.Month
                    && o.CreateDate.Year == date.Value.Year
                    && (o.Status == (byte)OrderStatusEnum.Approved
                        || o.Status == (byte)OrderStatusEnum.Packaged));
                var query = orders.AsQueryable();
                OrderByCreateDate(ref query, false);
                var list = _mapper.Map<List<ListOrderForStaffResponse>>(query.ToList());
                for (int i = 0; i < list.Count; i++)
                {
                    var totalQuantity = _orderDetailRepository.GetAllByIQueryable().Where(o => o.OrderId == list[i].OrderId).Select(o => o.Quantity).Sum();
                    list[i].TotalQuantity = totalQuantity;
                    var address = await _orderRepository.GetAllByIQueryable().Where(a => a.OrderId == list[i].OrderId).Include(a => a.DeliveryAddressNavigation).FirstOrDefaultAsync();
                    list[i].CustomerName = address.DeliveryAddressNavigation.ReceiverName;
                    list[i].CustomerPhone = address.DeliveryAddressNavigation.ReceiverPhone;
                }
                result.Content = list;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task<PagedResult<ListOrderForStaffResponse>> GetListOrderOfStoreForStaff(string token, FilterOrderParameter param)
        //{
        //    try
        //    {
        //        var uid = DecodeToken.DecodeTokenToGetUid(token);
        //        var orders = await _orderRepository.GetAllByIQueryable().Where(o => o.StaffId == uid).Include(o => o.Customer).ToListAsync();
        //        var query = orders.AsQueryable();
        //        FilterOrderByType(ref query, param.OrderType);
        //        FilterOrderStatus(ref query, param.OrderStatus.ToString());
        //        OrderByCreateDate(ref query, param.OrderByCreateDate);
        //        var response = _mapper.Map<List<ListOrderForStaffResponse>>(query.ToList());
        //        for (int i = 0; i < response.Count; i++)
        //        {
        //            var totalQuantity = _orderDetailRepository.GetAllByIQueryable().Where(o => o.OrderId == response[i].OrderId).Select(o => o.Quantity).Sum();
        //            response[i].TotalQuantity = totalQuantity;
        //        }
        //        return PagedResult<ListOrderForStaffResponse>.ToPagedList(response, param.PageNumber, param.PageSize);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public async Task<Result<IEnumerable<ListOrderForStaffResponse>>> GetListOrderOfStoreForStaff(string token, FilterOrderForStaffParameter param)
        {
            var result = new Result<IEnumerable<ListOrderForStaffResponse>>();
            try
            {
                var date = ConvertDateTime.ConvertStringToDate(param.CreateDate);
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var staff = await _staffRepository.FindAsync(s => s.Uid == uid);
                var orders = await _orderRepository
                    .GetAllByIQueryable()
                    .Where(o => o.StaffId == uid
                            && o.StoreId == staff.StoreId
                            && o.CreateDate.Day == date.Value.Day
                            && o.CreateDate.Month == date.Value.Month
                            && o.CreateDate.Year == date.Value.Year
                            && (o.OrderType == true && (o.Status == 4 || o.Status == 5) || o.OrderType == false))
                    .Include(o => o.Customer)
                    .OrderByDescending(o => o.CreateDate)
                    .ToListAsync();
                var response = _mapper.Map<List<ListOrderForStaffResponse>>(orders);

                for (int i = 0; i < response.Count; i++)
                {
                    var totalQuantity = _orderDetailRepository.GetAllByIQueryable().Where(o => o.OrderId == response[i].OrderId).Select(o => o.Quantity).Sum();
                    response[i].TotalQuantity = totalQuantity;
                    var address = await _orderRepository.GetAllByIQueryable().Where(a => a.OrderId == response[i].OrderId).Include(a => a.DeliveryAddressNavigation).FirstOrDefaultAsync();
                    if(address.DeliveryAddressNavigation != null)
                    {
                        response[i].CustomerName = address.DeliveryAddressNavigation.ReceiverName;
                        response[i].CustomerPhone = address.DeliveryAddressNavigation.ReceiverPhone;
                    }
                    else
                    {
                        response[i].CustomerName = orders.Where(o => o.OrderId == response[i].OrderId).Select(o => o.Customer.Fullname).FirstOrDefault();
                        response[i].CustomerPhone = orders.Where(o => o.OrderId == response[i].OrderId).Select(o => o.Customer.PhoneNumber).FirstOrDefault();
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

        public async Task<Result<bool>> PackagedOrder(int id)
        {
            var result = new Result<bool>();
            try
            {
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);
                order.PackagedDate = DateTime.UtcNow.AddHours(7);
                order.Status = (byte)OrderStatusEnum.Packaged;
                await _orderRepository.UpdateAsync(order);

                var username = await GetCustomerUserName(order.CustomerId);
                await _firebaseNotificationService.SendNotification(username, "Đơn hàng #" + order.OrderId + " đã được đóng gói", "Đơn hàng của bạn sẽ được giao trong thời gian sớm nhất.", "");

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

                //Call api cho bên thứ 3 để đăng ký giao hàng
                using (HttpClient hc = new HttpClient())
                {
                    var request = new DeliveryRequest
                    {
                        order_id = order.OrderId.ToString()
                    };
                    string json = JsonConvert.SerializeObject(request);
                    var response = await hc.PostAsync(ThirdPartyDeliverySimulationConstants.UrlServer, new StringContent(json, Encoding.UTF8, "application/json"));
                    //var responseString = await response.Content.ReadAsStringAsync();
                    //var sadsd = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);

                    var username = await GetCustomerUserName(order.CustomerId);
                    await _firebaseNotificationService.SendNotification(username, "Đơn hàng #" + order.OrderId + " đang được vận chuyển", "Đơn hàng của bạn sẽ được giao trong thời gian sớm nhất.", "");

                }
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

                var username = await GetCustomerUserName(order.CustomerId);
                await _firebaseNotificationService.SendNotification(username, "Đơn hàng #" + order.OrderId + " hoàn tất", "Cảm ơn bạn đã tin tưởng mua sắm tại Big-size Fashion.", "");

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

                var username = await GetCustomerUserName(order.CustomerId);
                await _firebaseNotificationService.SendNotification(username, "Đơn hàng #" + order.OrderId + " bị từ chối", "Đơn hàng của bạn bị từ chối", "");

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
                        if (order.Status != (byte)OrderStatusEnum.Pending
                            && order.Status != (byte)OrderStatusEnum.Reject
                            && order.Status != (byte)OrderStatusEnum.Cancel)
                        {
                            var ods = await _orderDetailRepository.FindByAsync(o => o.OrderId == id);
                            foreach (var item in ods)
                            {
                                var storeWarehouse = await _storeWarehouseRepository.FindAsync(s => s.StoreId == order.StoreId && s.ProductDetailId == item.ProductDetailId);
                                storeWarehouse.Quantity += item.Quantity;
                                await _storeWarehouseRepository.UpdateAsync(storeWarehouse);
                            }
                        }
                        order.Status = (byte)OrderStatusEnum.Pending;
                        order.StaffId = null;
                        order.ApprovalDate = null;
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

        public async Task<Result<OrderResponse>> AddOrder(string authorization, OrderRequest request)
        {
            var result = new Result<OrderResponse>();
            var uid = DecodeToken.DecodeTokenToGetUid(authorization);
            var listCart = await _cartService.getListCart(authorization);
            var listOrderDetailRequest = new List<OrderDetailRequest>();
            var addressResponse = await _addressService.GetAddressById(authorization, request.DeliveryAddress);
            var test = addressResponse.Content.ReceiveAddress;
            //var storeId = await _storeService.GetNearestStore(addressResponse.Content.ReceiveAddress);
            var orderResponse = new OrderResponse()
            {
                CreateDate = DateTime.UtcNow.AddHours(7),
                CustomerId = uid,
                DeliveryAddress = request.DeliveryAddress,
                PaymentMethod = request.PaymentMethod,
                Status = (byte)OrderStatusEnum.Pending,
              //  StoreId = storeId,
                OrderType = request.OrderType,
                TotalPrice = request.TotalPrice,
                TotalPriceAfterDiscount = request.TotalAfterDiscount,
                StoreId = request.StoreId,
                ShippingFee = request.ShippingFee,
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

            }
            try
            {
                await _orderDetailService.createOrderDetail(listOrderDetailRequest);
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
            
            //deleteCart
            await _cartService.AddToListCart(new List<AddToCartRequest>(), authorization);
            ///

            //var result = new Result<List<OrderResponse>>();
            //result.Content = new List<OrderResponse>();
            //result.Content = listOrderResponse;
            var username = await GetManagerUserName(order.StoreId);
            await _firebaseNotificationService.SendNotification(username, "Đơn hàng online #" + order.OrderId + " vừa được tạo", "Vui lòng xác thực đơn hàng.", "");

            result.Content = _mapper.Map<OrderResponse>(order);
            return result;
        }

        public async Task<Result<GetRevenueAndNumberOrdersResponse>> GetRevenueOfOwnStore(string token, GetRevenueParameter param)
        {
            var result = new Result<GetRevenueAndNumberOrdersResponse>();
            var listDate = new List<GetRevenueResponse>();
            var response = new GetRevenueAndNumberOrdersResponse();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var staff = await _staffRepository.FindAsync(s => s.Uid == uid);

                for (int i = 1; i <= DateTime.DaysInMonth(param.Year, param.Month); i++)
                {
                    var d = new GetRevenueResponse { Date = i, Value = 0 };
                    listDate.Add(d);
                }

                for (int i = 1; i <= listDate.Count; i++)
                {
                    var orders = await _orderRepository
                        .FindByAsync(o => o.StoreId == staff.StoreId
                        && o.ApprovalDate.Value.Day == listDate[i - 1].Date
                        && o.ApprovalDate.Value.Month == param.Month
                        && o.ApprovalDate.Value.Year == param.Year
                        && o.Status != 0
                        && o.Status != 1
                        && o.Status != 6);
                    var revenue = orders.Select(o => o.TotalPriceAfterDiscount).Sum();
                    listDate[i - 1].Value = revenue;
                }
                response.Revenues = listDate;

                var orderss = await _orderRepository.FindByAsync(o => o.StoreId == staff.StoreId && o.CreateDate.Month == param.Month && o.CreateDate.Year == param.Year);
                var cc = new StatisticOrderResponse
                {
                    TotalOrders = orderss.Select(o => o.OrderId).Count(),
                    PendingOrders = orderss.Where(o => o.Status == 1).Select(o => o.OrderId).Count(),
                    ProcessingOrders = orderss.Where(o => o.Status == 2 || o.Status == 3 || o.Status == 4).Select(o => o.OrderId).Count(),
                    ReceivedOrders = orderss.Where(o => o.Status == 5).Select(o => o.OrderId).Count(),
                    CanceledOrders = orderss.Where(o => o.Status == 0 || o.Status == 6).Select(o => o.OrderId).Count(),
                };
                response.NumberOrders = cc;
                result.Content = response; 
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<GetRevenueAndNumberOrdersResponse>> GetRevenueByStoreId(int id, GetRevenueParameter param)
        {
            var result = new Result<GetRevenueAndNumberOrdersResponse>();
            var listDate = new List<GetRevenueResponse>();
            var response = new GetRevenueAndNumberOrdersResponse();
            var cc = new StatisticOrderResponse();
            try
            {
                for (int i = 1; i <= DateTime.DaysInMonth(param.Year, param.Month); i++)
                {
                    var d = new GetRevenueResponse { Date = i, Value = 0 };
                    listDate.Add(d);
                }

                if (id == 0)
                {
                    for (int i = 1; i <= listDate.Count; i++)
                    {
                        var orders = await _orderRepository
                            .FindByAsync(o => o.ApprovalDate.Value.Day == listDate[i - 1].Date
                            && o.ApprovalDate.Value.Month == param.Month
                            && o.ApprovalDate.Value.Year == param.Year
                            && o.Status != 0
                            && o.Status != 1
                            && o.Status != 6);
                        var revenue = orders.Select(o => o.TotalPriceAfterDiscount).Sum();
                        listDate[i - 1].Value = revenue;
                    }
                    var orderss = await _orderRepository.FindByAsync(o => o.CreateDate.Month == param.Month && o.CreateDate.Year == param.Year);
                    cc.TotalOrders = orderss.Select(o => o.OrderId).Count();
                    cc.PendingOrders = orderss.Where(o => o.Status == 1).Select(o => o.OrderId).Count();
                    cc.ProcessingOrders = orderss.Where(o => o.Status == 2 || o.Status == 3 || o.Status == 4).Select(o => o.OrderId).Count();
                    cc.ReceivedOrders = orderss.Where(o => o.Status == 5).Select(o => o.OrderId).Count();
                    cc.CanceledOrders = orderss.Where(o => o.Status == 0 || o.Status == 6).Select(o => o.OrderId).Count();
                }
                else
                {
                    for (int i = 1; i <= listDate.Count; i++)
                    {
                        var orders = await _orderRepository
                            .FindByAsync(o => o.StoreId == id
                            && o.CreateDate.Day == listDate[i - 1].Date
                            && o.CreateDate.Month == param.Month
                            && o.CreateDate.Year == param.Year
                            && o.Status != 0
                            && o.Status != 1
                            && o.Status != 6);
                        var revenue = orders.Select(o => o.TotalPriceAfterDiscount).Sum();
                        listDate[i - 1].Value = revenue;
                    }
                    var orderss = await _orderRepository.FindByAsync(o => o.StoreId == id && o.CreateDate.Month == param.Month && o.CreateDate.Year == param.Year);
                    cc.TotalOrders = orderss.Select(o => o.OrderId).Count();
                    cc.PendingOrders = orderss.Where(o => o.Status == 1).Select(o => o.OrderId).Count();
                    cc.ProcessingOrders = orderss.Where(o => o.Status == 2 || o.Status == 3 || o.Status == 4).Select(o => o.OrderId).Count();
                    cc.ReceivedOrders = orderss.Where(o => o.Status == 5).Select(o => o.OrderId).Count();
                    cc.CanceledOrders = orderss.Where(o => o.Status == 0 || o.Status == 6).Select(o => o.OrderId).Count();
                }
                response.Revenues = listDate;
                response.NumberOrders = cc;

                result.Content = response;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<IEnumerable<NotEnoughProductResponse>>> ApproveOfflineOrder(int id)
        {
            var result = new Result<IEnumerable<NotEnoughProductResponse>>();
            try
            {
                var list = new List<NotEnoughProductResponse>();
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);

                var ods = await _orderDetailRepository.FindByAsync(o => o.OrderId == id);
                foreach (var item in ods)
                {
                    var storeWarehouse = await _storeWarehouseRepository.GetAllByIQueryable()
                        .Where(s => s.StoreId == order.StoreId && s.ProductDetailId == item.ProductDetailId)
                        .Include(s => s.ProductDetail)
                        .ThenInclude(s => s.Colour)
                        .Include(s => s.ProductDetail)
                        .ThenInclude(s => s.Product)
                        .Include(s => s.ProductDetail)
                        .ThenInclude(s => s.Size)
                        .FirstOrDefaultAsync();
                    if (storeWarehouse.Quantity < item.Quantity)
                    {
                        var cc = new NotEnoughProductResponse
                        {
                            ProductDetailId = storeWarehouse.ProductDetailId,
                            ColourId = storeWarehouse.ProductDetail.ColourId,
                            ColourName = storeWarehouse.ProductDetail.Colour.ColourName,
                            ProductId = storeWarehouse.ProductDetail.ProductId,
                            ProductName = storeWarehouse.ProductDetail.Product.ProductName,
                            SizeId = storeWarehouse.ProductDetail.SizeId,
                            SizeName = storeWarehouse.ProductDetail.Size.SizeName,
                            QuantityInStore = storeWarehouse.Quantity,
                            RequiredQuantity = item.Quantity
                        };
                        list.Add(cc);
                    }
                }

                if (list.Count == 0)
                {
                    foreach (var item in ods)
                    {
                        var storeWarehouse = await _storeWarehouseRepository.FindAsync(s => s.StoreId == order.StoreId && s.ProductDetailId == item.ProductDetailId);
                        storeWarehouse.Quantity -= item.Quantity;
                        await _storeWarehouseRepository.UpdateAsync(storeWarehouse);
                    }

                    var username = await GetStaffUserName(order.StaffId.Value);
                    var cc = await _firebaseNotificationService.SendNotification(username, "Đơn hàng #" + order.OrderId + " đã được duyệt", "Quản lý đã duyệt đơn hàng #" + order.OrderId + " thành công", "");

                    order.ApprovalDate = DateTime.UtcNow.AddHours(7);
                    order.PackagedDate = DateTime.UtcNow.AddHours(7);
                    order.DeliveryDate = DateTime.UtcNow.AddHours(7);
                    order.ReceivedDate = DateTime.UtcNow.AddHours(7);
                    order.Status = (byte)OrderStatusEnum.Received;
                    await _orderRepository.UpdateAsync(order);

                    result.Content = null;
                    return result;
                }
                else
                {
                    result.Content = list;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<IEnumerable<StaffPerformanceResponse>>> GetStaffPerformance(string token)
        {
            var result = new Result<IEnumerable<StaffPerformanceResponse>>();
            var list = new List<StaffPerformanceResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                DateTime[] last7Days = Enumerable.Range(0, 7)
                                        .Select(i => DateTime.Now.AddDays(-i))
                                        .ToArray();
                foreach (var item in last7Days)
                {
                    var orders = await _orderRepository
                                .FindByAsync(o => o.StaffId == uid
                                               && o.ApprovalDate.Value.Day == item.Day
                                               && o.ApprovalDate.Value.Month == item.Month
                                               && o.ApprovalDate.Value.Year == item.Year
                                               && o.Status != 0
                                               && o.Status != 1
                                               && o.Status != 6);
                    var value = orders.Select(o => o.TotalPriceAfterDiscount).Sum();
                    list.Add(new StaffPerformanceResponse { Date = ConvertDateTime.ConvertDateToString(item), Value = value });
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

        public async Task<Result<IEnumerable<StaffPerformanceOrderResponse>>> GetStaffPerformanceOrder(string token)
        {
            var result = new Result<IEnumerable<StaffPerformanceOrderResponse>>();
            var list = new List<StaffPerformanceOrderResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                DateTime[] last7Days = Enumerable.Range(0, 7)
                                        .Select(i => DateTime.Now.AddDays(-i))
                                        .ToArray();
                foreach (var item in last7Days)
                {
                    var orders = await _orderRepository
                                .FindByAsync(o => o.StaffId == uid
                                               && o.ApprovalDate.Value.Day == item.Day
                                               && o.ApprovalDate.Value.Month == item.Month
                                               && o.ApprovalDate.Value.Year == item.Year
                                               && o.Status != 0
                                               && o.Status != 1
                                               && o.Status != 6);
                    var value = orders.Select(o => o.OrderId).Count();
                    list.Add(new StaffPerformanceOrderResponse { Date = ConvertDateTime.ConvertDateToString(item), QuantityOfOrders = value });
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

        public async Task<Result<Stream>> ExportBill(int id)
        {
            var result = new Result<Stream>();
            try
            {
                result.Content = await CreateExcelFile(id);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private async Task<Stream> CreateExcelFile(int id, Stream stream = null)
        {
            try
            {
                var order = await _orderRepository
                                .GetAllByIQueryable()
                                .Where(o => o.OrderId == id)
                                .Include(o => o.Store)
                                .Include(o => o.Staff)
                                .FirstOrDefaultAsync();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
                {
                    // Tạo author cho file Excel
                    //excelPackage.Workbook.Properties.Author = "Hanker";
                    // Tạo title cho file Excel
                    //excelPackage.Workbook.Properties.Title = "EPP test background";
                    // thêm tí comments vào làm màu 
                    //excelPackage.Workbook.Properties.Comments = "This is my fucking generated Comments";
                    // Add Sheet vào file Excel
                    excelPackage.Workbook.Worksheets.Add("Hóa đơn #" + order.OrderId);
                    // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                    var workSheet = excelPackage.Workbook.Worksheets.First();
                    // Đổ data vào Excel file
                    //workSheet.Cells[1, 1].LoadFromCollection(list, true, TableStyles.Dark9);
                    await BindingFormatForExcel(workSheet, order);
                    excelPackage.Save();
                    return excelPackage.Stream;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task BindingFormatForExcel(ExcelWorksheet worksheet, Order order)
        {
            try
            {
                var orderDetail = await _orderDetailRepository
                                    .GetAllByIQueryable()
                                    .Where(o => o.OrderId == order.OrderId)
                                    .Include(o => o.ProductDetail)
                                    .ThenInclude(o => o.Product)
                                    .Include(o => o.ProductDetail)
                                    .ThenInclude(o => o.Size)
                                    .Include(o => o.ProductDetail)
                                    .ThenInclude(o => o.Colour)
                                    .ToListAsync();

                worksheet.DefaultColWidth = 10;
                worksheet.Cells.Style.WrapText = true;

                worksheet.Cells["A1:F1"].Value = "BIG-SIZE FASHION";
                worksheet.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:F1"].Style.Font.Bold = true;
                worksheet.Cells["A1:F1"].Merge = true;

                worksheet.Cells["A3:F3"].Value = order.Store.StoreAddress;
                worksheet.Cells["A3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:F3"].Merge = true;
                worksheet.Row(3).Height = 30.75;

                worksheet.Cells["A4:F4"].Value = "                                                                                                                                                            					";
                worksheet.Cells["A4:F4"].Merge = true;
                worksheet.Cells["A4:F4"].Style.Font.Strike = true;

                worksheet.Cells["A5:F5"].Value = "PHIẾU THANH TOÁN";
                worksheet.Cells["A5:F5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A5:F5"].Style.Font.Bold = true;
                worksheet.Cells["A5:F5"].Merge = true;

                worksheet.Cells["A6:B6"].Value = "Mã đơn hàng:";
                worksheet.Cells["A6:B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["A6:B6"].Merge = true;
                worksheet.Cells["C6:D6"].Value = "#" + order.OrderId;
                worksheet.Cells["C6:D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["C6:D6"].Merge = true;
                worksheet.Cells["A7:B7"].Value = "Ngày tạo:";
                worksheet.Cells["A7:B7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["A7:B7"].Merge = true;
                worksheet.Cells["C7:E7"].Value = ConvertDateTime.ConvertDateTimeToString(order.CreateDate);
                worksheet.Cells["C7:E7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["C7:E7"].Merge = true;
                worksheet.Cells["A8:B8"].Value = "Nhân viên:";
                worksheet.Cells["A8:B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["A8:B8"].Merge = true;
                worksheet.Cells["C8:E8"].Value = order.Staff.Fullname;
                worksheet.Cells["C8:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["C8:E8"].Merge = true;

                worksheet.Cells["A9:F9"].Value = "                                                                                                                                                            					";
                worksheet.Cells["A9:F9"].Merge = true;
                worksheet.Cells["A9:F9"].Style.Font.Strike = true;

                worksheet.Cells["A10:B10"].Value = "SL";
                worksheet.Cells["A10:B10"].Style.Font.Bold = true;
                worksheet.Cells["A10:B10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A10:B10"].Merge = true;

                worksheet.Cells["C10:D10"].Value = "Giá bán";
                worksheet.Cells["C10:D10"].Style.Font.Bold = true;
                worksheet.Cells["C10:D10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C10:D10"].Merge = true;

                worksheet.Cells["E10:F10"].Value = "T.Tiền";
                worksheet.Cells["E10:F10"].Style.Font.Bold = true;
                worksheet.Cells["E10:F10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E10:F10"].Merge = true;

                var index = 11;
                foreach (var item in orderDetail)
                {
                    var name = item.ProductDetail.Product.ProductName + "-Màu " + item.ProductDetail.Colour.ColourName + "-Size " + item.ProductDetail.Size.SizeName;
                    worksheet.Cells["A" + index + ":F" + index].Value = name;
                    worksheet.Cells["A" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":F" + index].Merge = true;
                    if(name.Length > 62)
                    {
                        worksheet.Row(index).Height = 30.75;
                    }
                    index += 1;

                    worksheet.Cells["A" + index + ":B" + index].Value = item.Quantity;
                    worksheet.Cells["A" + index + ":B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + index + ":B" + index].Merge = true;

                    worksheet.Cells["C" + index + ":D" + index].Value = FormatMoney.FormatPrice(item.PricePerOne);
                    worksheet.Cells["C" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["C" + index + ":D" + index].Merge = true;

                    worksheet.Cells["E" + index + ":F" + index].Value = FormatMoney.FormatPrice(item.Price);
                    worksheet.Cells["E" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["E" + index + ":F" + index].Merge = true;
                    index += 1;

                    if (item.DiscountPrice != null && item.DiscountPricePerOne != null)
                    {
                        worksheet.Cells["C" + (index - 1) + ":D" + (index -1)].Style.Font.Strike = true;
                        worksheet.Cells["E" + (index - 1) + ":F" + (index -1)].Style.Font.Strike = true;

                        worksheet.Cells["C" + index + ":D" + index].Value = FormatMoney.FormatPrice(item.DiscountPricePerOne);
                        worksheet.Cells["C" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["C" + index + ":D" + index].Merge = true;

                        worksheet.Cells["E" + index + ":F" + index].Value = FormatMoney.FormatPrice(item.DiscountPrice);
                        worksheet.Cells["E" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["E" + index + ":F" + index].Merge = true;
                    }
                    index += 1;
                }

                worksheet.Cells["A" + index + ":F" + index].Value = "                                                                                                                                                            					";
                worksheet.Cells["A" + index + ":F" + index].Merge = true;
                worksheet.Cells["A" + index + ":F" + index].Style.Font.Strike = true;
                index += 1;

                worksheet.Cells["C" + index + ":D" + index].Value = "Tổng tiền:";
                worksheet.Cells["C" + index + ":D" + index].Style.Font.Bold = true;
                worksheet.Cells["C" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C" + index + ":D" + index].Merge = true;

                worksheet.Cells["E" + index + ":F" + index].Value = FormatMoney.FormatPrice(order.TotalPriceAfterDiscount);
                worksheet.Cells["E" + index + ":F" + index].Style.Font.Bold = true;
                worksheet.Cells["E" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E" + index + ":F" + index].Merge = true;
                index += 2;

                worksheet.Cells["C" + index + ":D" + index].Value = "Thanh toán:";
                worksheet.Cells["C" + index + ":D" + index].Style.Font.Bold = true;
                worksheet.Cells["C" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C" + index + ":D" + index].Merge = true;

                worksheet.Cells["E" + index + ":F" + index].Value = FormatMoney.FormatPrice(order.TotalPriceAfterDiscount);
                worksheet.Cells["E" + index + ":F" + index].Style.Font.Bold = true;
                worksheet.Cells["E" + index + ":F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E" + index + ":F" + index].Merge = true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Result<IEnumerable<StaffPerformanceOfStoreResponse>>> GetPerformanceOfAllStaff(string token, GetRevenueParameter param)
        {
            var result = new Result<IEnumerable<StaffPerformanceOfStoreResponse>>();
            var list = new List<StaffPerformanceOfStoreResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var storeId = await _staffRepository.GetAllByIQueryable()
                                    .Where(s => s.Uid == uid)
                                    .Select(s => s.StoreId)
                                    .FirstOrDefaultAsync();

                var staffs = await _staffRepository.GetAllByIQueryable()
                                    .Include(s => s.UidNavigation)
                                    .Where(s => s.StoreId == storeId && s.UidNavigation.RoleId == 3 && s.Status == true)
                                    .ToListAsync();

                foreach (var staff in staffs)
                {
                    var orders = await _orderRepository
                                .FindByAsync(o => o.StaffId == staff.Uid
                                               && o.StoreId == storeId
                                               && o.ApprovalDate.Value.Month == param.Month
                                               && o.ApprovalDate.Value.Year == param.Year
                                               && o.Status != 0
                                               && o.Status != 1
                                               && o.Status != 6);
                    var revenue = orders.Select(o => o.TotalPriceAfterDiscount).Sum();
                    var quantity = orders.Select(o => o.OrderId).Count();

                    list.Add(new StaffPerformanceOfStoreResponse
                    {
                        Uid = staff.Uid,
                        Fullname = staff.Fullname,
                        Revenue = revenue,
                        QuantityOfOrders = quantity
                    });
                }

                var cc = list.OrderByDescending(l => l.Revenue);
                
                result.Content = cc.ToList();
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> UpdateReceivedOrder(TrackingOrderRequest request)
        {
            var result = new Result<bool>();
            try
            {
                var order = await _orderRepository.FindAsync(o => o.OrderId == request.OrderId);
                order.ReceivedDate = request.ReceivedDate;
                order.Status = (byte)OrderStatusEnum.Received;
                await _orderRepository.UpdateAsync(order);

                var username = await GetCustomerUserName(order.CustomerId);
                await _firebaseNotificationService.SendNotification(username, "Đơn hàng #" + order.OrderId + " hoàn tất", "Cảm ơn bạn đã tin tưởng mua sắm tại Big-size Fashion.", "");

                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<StatisticOrderResponse>> GetStatisticToday(string token)
        {
            var result = new Result<StatisticOrderResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var storeId = await _staffRepository
                    .GetAllByIQueryable()
                    .Where(s => s.Uid == uid && s.Status == true)
                    .Select(s => s.StoreId)
                    .FirstOrDefaultAsync();
                var now = DateTime.Now;
                var orders = await _orderRepository.FindByAsync(o => o.StoreId == storeId 
                                                               && o.CreateDate.Day == now.Day
                                                               && o.CreateDate.Month == now.Month
                                                               && o.CreateDate.Year == now.Year);
                var cc = new StatisticOrderResponse
                {
                    TotalOrders = orders.Select(o => o.OrderId).Count(),
                    PendingOrders = orders.Where(o => o.Status == 1).Select(o => o.OrderId).Count(),
                    ProcessingOrders = orders.Where(o => o.Status == 2 || o.Status == 3 || o.Status == 4).Select(o => o.OrderId).Count(),
                    ReceivedOrders = orders.Where(o => o.Status == 5).Select(o => o.OrderId).Count(),
                    CanceledOrders = orders.Where(o => o.Status == 0 || o.Status == 6).Select(o => o.OrderId).Count(),
                };
                result.Content = cc;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<StatisticOrderResponse>> GetStatisticLast30Days(string token)
        {
            var result = new Result<StatisticOrderResponse>();
            try
            {
                var uid = DecodeToken.DecodeTokenToGetUid(token);
                var storeId = await _staffRepository
                    .GetAllByIQueryable()
                    .Where(s => s.Uid == uid && s.Status == true)
                    .Select(s => s.StoreId)
                    .FirstOrDefaultAsync();
                var last30Days = DateTime.Now.AddDays(-30);
                var orders = await _orderRepository.FindByAsync(o => o.StoreId == storeId && o.CreateDate.Date >= last30Days.Date);
                var cc = new StatisticOrderResponse
                {
                    TotalOrders = orders.Select(o => o.OrderId).Count(),
                    PendingOrders = orders.Where(o => o.Status == 1).Select(o => o.OrderId).Count(),
                    ProcessingOrders = orders.Where(o => o.Status == 2 || o.Status == 3 || o.Status == 4).Select(o => o.OrderId).Count(),
                    ReceivedOrders = orders.Where(o => o.Status == 5).Select(o => o.OrderId).Count(),
                    CanceledOrders = orders.Where(o => o.Status == 0 || o.Status == 6).Select(o => o.OrderId).Count(),
                };
                result.Content = cc;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<StatisticOrderResponse>> GetStatisticTodayForOwner(int id)
        {
            var result = new Result<StatisticOrderResponse>();
            try
            {
                var now = DateTime.Now;
                if (id == 0)
                {
                    var orders = await _orderRepository.FindByAsync(o => o.CreateDate.Day == now.Day
                                                                      && o.CreateDate.Month == now.Month
                                                                      && o.CreateDate.Year == now.Year);
                    var cc = new StatisticOrderResponse
                    {
                        TotalOrders = orders.Select(o => o.OrderId).Count(),
                        PendingOrders = orders.Where(o => o.Status == 1).Select(o => o.OrderId).Count(),
                        ProcessingOrders = orders.Where(o => o.Status == 2 || o.Status == 3 || o.Status == 4).Select(o => o.OrderId).Count(),
                        ReceivedOrders = orders.Where(o => o.Status == 5).Select(o => o.OrderId).Count(),
                        CanceledOrders = orders.Where(o => o.Status == 0 || o.Status == 6).Select(o => o.OrderId).Count(),
                    };
                    result.Content = cc;
                    return result;
                }
                else
                {
                    var orders = await _orderRepository.FindByAsync(o => o.StoreId == id
                                                               && o.CreateDate.Day == now.Day
                                                               && o.CreateDate.Month == now.Month
                                                               && o.CreateDate.Year == now.Year);
                    var cc = new StatisticOrderResponse
                    {
                        TotalOrders = orders.Select(o => o.OrderId).Count(),
                        PendingOrders = orders.Where(o => o.Status == 1).Select(o => o.OrderId).Count(),
                        ProcessingOrders = orders.Where(o => o.Status == 2 || o.Status == 3 || o.Status == 4).Select(o => o.OrderId).Count(),
                        ReceivedOrders = orders.Where(o => o.Status == 5).Select(o => o.OrderId).Count(),
                        CanceledOrders = orders.Where(o => o.Status == 0 || o.Status == 6).Select(o => o.OrderId).Count(),
                    };
                    result.Content = cc;
                    return result;
                } 
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> ChangePaymentMethod(int id, string method)
        {
            var result = new Result<bool>();
            try
            {
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);
                order.PaymentMethod = method;
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
    }
}
