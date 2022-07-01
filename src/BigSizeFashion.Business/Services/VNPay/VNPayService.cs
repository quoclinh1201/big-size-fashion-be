using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.IServices.VNPay;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services.VNPay
{
    public class VNPayService : IVNPayService
    {
        private readonly IGenericRepository<Order> _orderRepository;

        public VNPayService(IGenericRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<bool>> GetIPNResponse(VNPayParameter param)
        {
            var result = new Result<bool>();
            try
            {
                var pay = new PayLib();
                var vnp_HashSecret = VNPayConstants.HashSecret;
                var checkSignature = pay.ValidateSignature(param.vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    result.Content = true;
                    return result;
                }
                else
                {
                    result.Content = false;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<string>> GetLink(int id, string clientIPAddr)
        {
            var result = new Result<string>();
            try
            {
                var order = await _orderRepository.FindAsync(o => o.OrderId == id);
                var url = VNPayConstants.Url;
                var returnUrl = VNPayConstants.ReturnUrl;
                var tmnCode = VNPayConstants.TmnCode;
                var hashSecret = VNPayConstants.HashSecret;

                PayLib pay = new PayLib();
                pay.AddRequestData("vnp_Version", "2.0.1"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.0.0
                pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
                pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
                pay.AddRequestData("vnp_Amount", order.TotalPriceAfterDiscount.ToString().Substring(0, order.TotalPriceAfterDiscount.ToString().Length - 5)); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
                pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
                pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
                pay.AddRequestData("vnp_IpAddr", clientIPAddr); //Địa chỉ IP của khách hàng thực hiện giao dịch
                pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
                pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
                pay.AddRequestData("vnp_OrderType", "fashion"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
                pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
                pay.AddRequestData("vnp_TxnRef", order.OrderType.ToString()); //mã hóa đơn\

                string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
                result.Content = paymentUrl;
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
