using AutoMapper;
using BigSizeFashion.Business.Helpers.Common;
using BigSizeFashion.Business.Helpers.Constants;
using BigSizeFashion.Business.Helpers.Parameters;
using BigSizeFashion.Business.Helpers.RequestObjects;
using BigSizeFashion.Business.Helpers.ResponseObjects;
using BigSizeFashion.Business.IServices;
using BigSizeFashion.Data.Entities;
using BigSizeFashion.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services
{
    public class ColourService : IColourService
    {
        private readonly IGenericRepository<Colour> _genericRepository;
        private readonly IMapper _mapper;

        public ColourService(IGenericRepository<Colour> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<ColourResponse>>> GetAllColour(SearchColourParameter param)
        {
            var result = new Result<IEnumerable<ColourResponse>>();
            try
            {
                var colours = await _genericRepository.GetAllAsync();
                var query = colours.AsQueryable();
                FilterColourByName(ref query, param.Colour);
                FilterColourByStatus(ref query, param.Status);
                result.Content = _mapper.Map<List<ColourResponse>>(query.ToList());
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        private void FilterColourByStatus(ref IQueryable<Colour> query, bool? status)
        {
            if (!query.Any() || status is null)
            {
                return;
            }

            if (status is true)
            {
                query = query.Where(x => x.Status == true);
            }
            else
            {
                query = query.Where(x => x.Status == false);
            }
        }

        private static void FilterColourByName(ref IQueryable<Colour> query, string name)
        {
            if (!query.Any() || String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
            {
                return;
            }

            query = query.Where(q => q.ColourName.ToLower().Contains(name.ToLower()));
        }

        public async Task<Result<ColourResponse>> GetColourByID(int id)
        {
            var result = new Result<ColourResponse>();
            try
            {
                var colour = await _genericRepository.FindAsync(s => s.ColourId == id);
                result.Content = _mapper.Map<ColourResponse>(colour);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<ColourResponse>> CreateColour(ColourRequest request)
        {
            var result = new Result<ColourResponse>();
            try
            {
                var colour = _mapper.Map<Colour>(request);
                await _genericRepository.InsertAsync(colour);
                await _genericRepository.SaveAsync();
                result.Content = _mapper.Map<ColourResponse>(colour);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<ColourResponse>> UpdateColour(int id, ColourRequest request)
        {
            var result = new Result<ColourResponse>();
            try
            {
                var colour = await _genericRepository.FindAsync(s => s.ColourId == id);
                var model = _mapper.Map(request, colour);
                await _genericRepository.UpdateAsync(model);
                result.Content = _mapper.Map<ColourResponse>(model);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }

        public async Task<Result<bool>> DeleteColour(int id)
        {
            var result = new Result<bool>();
            try
            {
                var colour = await _genericRepository.FindAsync(s => s.ColourId == id);

                if (colour is null)
                {
                    result.Content = false;
                    return result;
                }

                if(colour.Status)
                {
                    colour.Status = false;
                }
                else
                {
                    colour.Status = true;
                }

                await _genericRepository.UpdateAsync(colour);
                result.Content = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ErrorHelpers.PopulateError(400, APITypeConstants.BadRequest_400, ex.Message);
                return result;
            }
        }
    }
}
