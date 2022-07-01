using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices.VNPay
{
    public interface IVNPayService
    {
        Task<Result<string>> GetLink(int id, string clientIPAddr);
        Task<Result<bool>> GetIPNResponse(VNPayParameter param);
    }
}
