using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class UpdateUserProfileRequest
    {
        [MaxLength(50)]
        public string Fullname { get; set; }

        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Birthday { get; set; }

        [RegularExpression(@"[0]{1}[0-9]{9}")]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
    }
}
