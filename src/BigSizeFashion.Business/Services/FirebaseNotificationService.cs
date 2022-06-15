using BigSizeFashion.Business.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;

namespace BigSizeFashion.Business.Services
{
    public class FirebaseNotificationService : IFirebaseNotificationService
    {
        public async Task<string> SendNotification(string user, string title, string body, string channel)
        {
            try
            {
                var message = new Message()
                {
                    Data = new Dictionary<string, string>()
                    {
                        ["channel"] = channel
                    },
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },

                    Topic = user
                };
                var messaging = FirebaseMessaging.DefaultInstance;
                return await messaging.SendAsync(message);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
