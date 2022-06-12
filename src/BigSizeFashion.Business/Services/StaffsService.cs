using AutoMapper;
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
    public class StaffsService : IStaffsService
    {
        private readonly IGenericRepository<staff> _genericRepository;
        private readonly IMapper _mapper;

        public StaffsService(IGenericRepository<staff> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }
    }
}
