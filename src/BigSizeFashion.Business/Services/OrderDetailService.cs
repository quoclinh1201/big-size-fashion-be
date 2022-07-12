using AutoMapper;
using BigSizeFashion.Business.Dtos.Requests;
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
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IGenericRepository<Product> _productDetailRepository;
        private readonly IMapper _mapper;

        public OrderDetailService(IGenericRepository<OrderDetail> orderDetailRepository,
            IGenericRepository<Product> productDetailRepository,
            IMapper mapper)
        {
            _orderDetailRepository = orderDetailRepository;
            _productDetailRepository = productDetailRepository;
            _mapper = mapper;
        }
        public async Task<bool> createOrderDetail(List<OrderDetailRequest> request)
        {
            foreach(var orderDetailRequest in request)
            {
                var orderDetail = _mapper.Map<OrderDetail>(orderDetailRequest);
                await _orderDetailRepository.InsertAsync(orderDetail);
                await _orderDetailRepository.SaveAsync();
            }
            return true;
        }
    }
}
