using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class FeedbackRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Content { get; set; }

        [Required]
        [Range(1, 5)]
        public byte Rate { get; set; }
    }
}
