using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IProductImageService
    {
        Task<Result<IEnumerable<ProductImageResponse>>> AddImagesForProduct(int id, IFormFileCollection files);
    }
}
