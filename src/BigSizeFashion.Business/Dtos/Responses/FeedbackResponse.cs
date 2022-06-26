using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Responses
{
    public class FeedbackResponse
    {
        public int FeedbackId { get; set; }
        public string CustomerName { get; set; }
        public string Content { get; set; }
        public byte? Rate { get; set; }
        public string CreateDate { get; set; }
    }
}
