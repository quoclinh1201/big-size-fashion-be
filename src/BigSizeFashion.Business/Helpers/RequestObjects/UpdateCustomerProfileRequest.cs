using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class UpdateCustomerProfileRequest
    {
        public string Fullname { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Birthday { get; set; }

        [Required(AllowEmptyStrings = false)]
        public bool Gender { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Range(0, 255)]
        public byte Weigth { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Range(0, 255)]
        public byte Heigth { get; set; }
    }
}
