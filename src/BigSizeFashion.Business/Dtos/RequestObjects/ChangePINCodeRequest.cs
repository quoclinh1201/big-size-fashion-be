using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class ChangePINCodeRequest
    {
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"[0-9]{6}")]
        [MaxLength(6)]
        [MinLength(6)]
        public string OldPinCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"[0-9]{6}")]
        [MaxLength(6)]
        [MinLength(6)]
        public string NewPinCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"[0-9]{6}")]
        [MaxLength(6)]
        [MinLength(6)]
        [Compare("NewPinCode",
            ErrorMessage = "New PIN code and confirmation new PIN code do not match.")]
        public string ConfirmNewPinCode { get; set; }
    }
}
