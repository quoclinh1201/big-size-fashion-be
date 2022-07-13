using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.IServices;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services.ScheduleJob
{
    public class OrderTrackingJob : IJob
    {
        private readonly IOrderService _service;

        public OrderTrackingJob(IOrderService service)
        {
            _service = service;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    var response = await hc.GetAsync(ThirdPartyDeliverySimulationConstants.UrlServer);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseString);

                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            var request = new TrackingOrderRequest
                            {
                                OrderId = Convert.ToInt32(item["order_id"]),
                                ReceivedDate = (DateTime)item["received_date"]
                            };

                            await _service.UpdateReceivedOrder(request);
                        }
                    }
                }
                //return Task.CompletedTask;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
