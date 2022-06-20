using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class CreateStaffAccountRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(20)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string Fullname { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"[0]{1}[0-9]{9}")]
        [MaxLength(10)]
        [MinLength(10)]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int StoreId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string RoleAccount { get; set; }
    }
}
