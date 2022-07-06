using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        public string NewPassword { get; set; }
    }
}
