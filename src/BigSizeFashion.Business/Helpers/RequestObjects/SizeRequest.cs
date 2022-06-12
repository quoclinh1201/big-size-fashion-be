using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class SizeRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(10)]
        public string SizeName { get; set; }
    }
}
