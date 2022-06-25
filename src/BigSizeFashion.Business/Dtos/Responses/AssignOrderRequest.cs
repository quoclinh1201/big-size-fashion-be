using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class AssignOrderRequest
    {
        [Required]
        public int StaffId { get; set; }
        [Required]
        public int OrderId { get; set; }
    }
}
