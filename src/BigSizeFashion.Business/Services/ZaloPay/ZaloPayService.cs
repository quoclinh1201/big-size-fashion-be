using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.IServices.ZaloPay;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services.ZaloPay
{
    public class ZaloPayService : IZaloPayService
    {
        private readonly IGenericRepository<Order> _orderRepository;

        public ZaloPayService(IGenericRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }


        public async Task<Result<CreateZaloPayOrderResponse>> CreateOrder(int id)
        {
            var result = new Result<CreateZaloPayOrderResponse>();
            try
            {
                Random rnd = new Random();
                var embed_data = new Dictionary<string, string>();
                embed_data.Add("redirecturl", "https://bigsizefashion.page/payment-success");
                var items = new[] { new { } };
                var param = new Dictionary<string, string>();
                var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);

                var shippingFee = order.ShippingFee != null ? order.ShippingFee : 0;
                var amount = order.TotalPriceAfterDiscount + shippingFee;

                param.Add("app_id", ZaloPayConstants.AppId);
                param.Add("app_user", "user123");
                param.Add("app_time", Utils.GetTimeStamp().ToString());
                param.Add("amount", amount.ToString().Substring(0, amount.ToString().Length - 5));
                param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
                param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
                param.Add("item", JsonConvert.SerializeObject(items));
                param.Add("description", "BigSizeFashion - Thanh toán đơn hàng #" + id);
                param.Add("bank_code", "zalopayapp");

                var data = ZaloPayConstants.AppId + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
                param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, ZaloPayConstants.Key1, data));

                var response = await HttpHelper.PostFormAsync(ZaloPayConstants.CreateOrderUrl, param);

                var cc = new CreateZaloPayOrderResponse {
                    ReturnCode = Convert.ToInt32(response["return_code"]),
                    ReturnMessage = response["return_message"].ToString(),
                    SubReturnCode = Convert.ToInt32(response["sub_return_code"]),
                    SubReturnMessage = response["sub_return_message"].ToString(),
                    OrderUrl = response["order_url"].ToString(),
                    ZpTransToken = response["zp_trans_token"].ToString(),
                    ZpTransId = param["app_trans_id"].Trim()
                };

                //foreach (var entry in response)
                //{
                //    //Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
                //    cc += entry.Key + " = " + entry.Value + "\n";
                //}
                result.Content = cc;
                return result;
                
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<string>> CreateOrderString(int id)
        {
            var result = new Result<string>();
            try
            {
                Random rnd = new Random();
                var embed_data = new { };
                var items = new[] { new { } };
                var param = new Dictionary<string, string>();
                var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);

                var shippingFee = order.ShippingFee != null ? order.ShippingFee : 0;
                var amount = order.TotalPriceAfterDiscount + shippingFee;

                param.Add("app_id", ZaloPayConstants.AppId);
                param.Add("app_user", "user123");
                param.Add("app_time", Utils.GetTimeStamp().ToString());
                param.Add("amount", amount.ToString().Substring(0, amount.ToString().Length - 5));
                param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
                param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
                param.Add("item", JsonConvert.SerializeObject(items));
                param.Add("description", "BigSizeFashion - Thanh toán đơn hàng #" + app_trans_id);
                param.Add("bank_code", "zalopayapp");

                var data = ZaloPayConstants.AppId + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
                param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, ZaloPayConstants.Key1, data));

                var response = await HttpHelper.PostFormAsync(ZaloPayConstants.CreateOrderUrl, param);

                var cc = "";

                foreach (var entry in response)
                {
                    //Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
                    cc += entry.Key + " = " + entry.Value + "\n";
                }
                result.Content = cc;
                return result;

            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<CreateZaloPayOrderResponse>> CreateOrderWithMoney(CreateOrderWithMoneyRequest request)
        {
            var result = new Result<CreateZaloPayOrderResponse>();
            try
            {
                Random rnd = new Random();
                var embed_data = new { };
                var items = new[] { new { } };
                var param = new Dictionary<string, string>();
                var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.

                param.Add("app_id", ZaloPayConstants.AppId);
                param.Add("app_user", "user123");
                param.Add("app_time", Utils.GetTimeStamp().ToString());
                //param.Add("amount", request.TotalPrice.ToString().Substring(0, request.TotalPrice.ToString().Length - 5));
                param.Add("amount", request.TotalPrice);
                param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
                param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
                param.Add("item", JsonConvert.SerializeObject(items));
                param.Add("description", "BigSizeFashion - Thanh toán đơn hàng #" + app_trans_id);
                param.Add("bank_code", "zalopayapp");

                var data = ZaloPayConstants.AppId + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
                param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, ZaloPayConstants.Key1, data));

                var response = await HttpHelper.PostFormAsync(ZaloPayConstants.CreateOrderUrl, param);

                var cc = new CreateZaloPayOrderResponse
                {
                    ReturnCode = Convert.ToInt32(response["return_code"]),
                    ReturnMessage = response["return_message"].ToString(),
                    SubReturnCode = Convert.ToInt32(response["sub_return_code"]),
                    SubReturnMessage = response["sub_return_message"].ToString(),
                    OrderUrl = response["order_url"].ToString(),
                    ZpTransToken = response["zp_trans_token"].ToString(),
                    ZpTransId = param["app_trans_id"].Trim()
                };

                //foreach (var entry in response)
                //{
                //    //Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
                //    cc += entry.Key + " = " + entry.Value + "\n";
                //}
                result.Content = cc;
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
