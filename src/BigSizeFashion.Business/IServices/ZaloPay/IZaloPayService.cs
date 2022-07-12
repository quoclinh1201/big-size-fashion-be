using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices.ZaloPay
{
    public interface IZaloPayService
    {
        Task<Result<CreateZaloPayOrderResponse>> CreateOrder(int id);

        Task<Result<string>> CreateOrderString(int id);
        Task<Result<CreateZaloPayOrderResponse>> CreateOrderWithMoney(CreateOrderWithMoneyRequest request);
    }
}
