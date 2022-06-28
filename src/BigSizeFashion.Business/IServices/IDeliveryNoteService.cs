using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IDeliveryNoteService
    {
        Task<PagedResult<ListImportProductResponse>> GetListRequestImportProduct(string v, ImportProductParameter param);
        Task<PagedResult<ListImportProductResponse>> GetListExportProduct(string v, ImportProductParameter param);
        Task<Result<DeliveryNoteDetailResponse>> GetDeliveryNoteDetail(int id);
    }
}
