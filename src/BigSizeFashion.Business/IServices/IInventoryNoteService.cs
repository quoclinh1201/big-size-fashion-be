using BigSizeFashion.Business.Dtos.Parameters;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IInventoryNoteService
    {
        Task<Result<InventoryNoteResponse>> CreateInventoryNote(string v, CreateInventoryNoteRequest request);
        Task<Result<InventoryNoteResponse>> GetInventoryNoteById(int id);
        Task<PagedResult<GetListInventoryNoteResponse>> GetListInventoryNote(string v, GetListInventoryNoteParameter param);
        Task<Result<bool>> DeleteInventoryNote(int id);
        Task<Result<Stream>> ExportExcel(int id);
    }
}
