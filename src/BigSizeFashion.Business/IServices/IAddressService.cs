using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IAddressService
    {
        Task<Result<IEnumerable<AddressResponse>>> GetAllAddresses(string v);
        Task<Result<AddressResponse>> GetAddressById(string v, int id);
        Task<Result<AddressResponse>> CreateAddress(string v, AddressRequest request);
        Task<Result<AddressResponse>> UpdateAddress(string v, AddressRequest request, int id);
        Task<Result<bool>> DeleteAddress(string v, int id);
    }
}
