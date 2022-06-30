using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Constants
{
    public class ErrorMessageConstants
    {
        public const string ExistedManagerInStore = "Đã tồn tại vị trí Manager ở cửa hàng này.";
        public const string BlockedAccount = "Tài khoản của người dùng đã bị khóa.";
        public const string WrongUsernameOrPassword = "Sai thông tin tài khoản hoặc mật khẩu.";
        public const string Unauthenticate = "Vui lòng login để dùng chức năng này.";
        public const string WrongOldPassword = "Sai mật khẩu";
        public const string NotExistedUser = "Người dùng không tồn tại.";
        public const string ExistedPINCode = "Tài khoản đã có mã PIN.";
        public const string NotExistedPINCode = "Tài khoản chưa có mã PIN.";
        public const string WrongPINCode = "Sai mã PIN.";
        public const string ExpireDateLessThanApplyDate = "Ngày kết thúc khuyến mãi phải sau ngày bắt đầu áp dụng.";
        public const string ApplyDateLessThanNow = "Ngày bắt đầu áp dụng khuyến mãi phải là một ngày trong tương lai.";
        public const string InvalidPromotionDatetime = "Ngày áp dụng hoặc ngày kết thúc khuyến mãi đã trùng với khoảng thời gian diễn ra khuyến mãi khác.";
        public const string CannotUploadImage = "Đã có lỗi xảy ra khi upload hình ảnh.";
        public const string OutOfProduct = "Hiện tại sản phẩm tại cửa hàng này tạm thời hết hàng, vui lòng chọn lại ở cửa hàng khác.";
        public const string NotExistedOrder = "Đơn hàng không tồn tại.";
        public const string CannotCancelOrder = "Không thể hủy đơn hàng đã xác nhận.";
        public const string NotExistedProduct = "Sản phẩm không tồn tại.";
        public const string NotEnoughProduct = "Số lượng hàng hóa trong kho không đáp ứng đủ cho đơn hàng.";
    }
}
