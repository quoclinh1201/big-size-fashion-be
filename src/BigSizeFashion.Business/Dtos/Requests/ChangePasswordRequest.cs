using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class ChangePasswordRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(20)]
        public string OldPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(20)]
        public string NewPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(20)]
        [Compare("NewPassword",
            ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmNewPassword{ get; set; }
    }
}
