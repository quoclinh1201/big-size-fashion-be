using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IFirebaseNotificationService
    {
        Task<string> SendNotification(string user, string title, string body, string channel);
    }
}
