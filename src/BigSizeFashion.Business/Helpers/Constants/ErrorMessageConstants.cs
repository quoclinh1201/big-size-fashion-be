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
    }
}
