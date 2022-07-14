using BigSizeFashion.Business.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using BigSizeFashion.Business.Dtos.Requests;

namespace BigSizeFashion.Business.Services
{
    public class FirebaseNotificationService : IFirebaseNotificationService
    {
        private readonly INotificationService _service;

        public FirebaseNotificationService(INotificationService service)
        {
            _service = service;
        }

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

                var notify = new CreateNotificationRequest {Username = user, Title = title, Message = body };
                await _service.CreateNotification(notify);

                return await messaging.SendAsync(message);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
