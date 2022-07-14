using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Dtos.Requests
{
    public class CreateNotificationRequest
    {
        public string Username { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ReferenceUrl { get; set; }
    }
}
