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
            ErrorMessage = "New password and confirmation new password do not match.")]
        public string ConfirmNewPassword{ get; set; }
    }
}
