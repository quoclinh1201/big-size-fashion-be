using BigSizeFashion.Business.Dtos.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IOrderDetailService
    {
        Task<bool> createOrderDetail(List<OrderDetailRequest> request);
    }
}
