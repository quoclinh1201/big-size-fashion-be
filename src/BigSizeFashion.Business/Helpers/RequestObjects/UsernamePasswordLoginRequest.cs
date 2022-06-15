using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class UsernamePasswordLoginRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(20)]
        public string Password { get; set; }
    }
}
