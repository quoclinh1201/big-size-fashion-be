using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface IColourService
    {
        Task<Result<IEnumerable<ColourResponse>>> GetAllColour(SearchColourParameter param);
        Task<Result<ColourResponse>> GetColourByID(int id);
        Task<Result<ColourResponse>> CreateColour(ColourRequest request);
        Task<Result<ColourResponse>> UpdateColour(int id, ColourRequest request);
        Task<Result<bool>> DeleteColour(int id);
    }
}
