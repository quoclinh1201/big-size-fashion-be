using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class CreatePINCodeRequest
    {
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"[0-9]{6}")]
        [MaxLength(6)]
        [MinLength(6)]
        public string PinCode { get; set; }
    }
}
