﻿using BigSizeFashion.Business.Dtos.Parameters;
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
    public interface IDeliveryNoteService
    {
        Task<PagedResult<ListImportProductResponse>> GetListRequestImportProduct(string v, ImportProductParameter param);
        Task<PagedResult<ListImportProductResponse>> GetListExportProduct(string v, ImportProductParameter param);
        Task<Result<DeliveryNoteDetailResponse>> GetDeliveryNoteDetail(int id);
        Task<Result<bool>> CreateRequestImportProduct(string v, ImportProductRequest request);
        Task<Result<bool>> ApproveRequestImportProduct(string v, int id);
        Task<Result<bool>> RejectRequestImportProduct(int id);
        Task<PagedResult<ListImportProductResponse>> GetListExportProductForMainWarehouse(string v, ImportProductParameter param);
    }
}
