using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class CategoryRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string Category { get; set; }
    }
}
