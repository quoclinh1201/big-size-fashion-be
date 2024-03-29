﻿using BigSizeFashion.Business.Dtos.RequestObjects;
using BigSizeFashion.Business.Dtos.Requests;
using BigSizeFashion.Business.Dtos.ResponseObjects;
using BigSizeFashion.Business.Dtos.Responses;
using BigSizeFashion.Business.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.IServices
{
    public interface ICartService
    {
        //Task<Result<IEnumerable<CartItemResponse>>> GetAllProductInCartByCustomerID(string token);
        //Task<Result<bool>> AddProductToCart(string token, ManageProductInCartRequest request);
        //Task<Result<IEnumerable<CartItemResponse>>> IncreaseProductInCart(string token, ManageProductInCartRequest request);
        //Task<Result<IEnumerable<CartItemResponse>>> DecreaseProductInCart(string token, ManageProductInCartRequest request);
        //Task<Result<IEnumerable<CartItemResponse>>> RemoveProductInCart(string token, ManageProductInCartRequest request);
        Task<Result<AddToCartResponse>> AddToCart(AddToCartRequest request, string authorization);
        Task<Result<List<AddToCartResponse>>> AddToListCart(List<AddToCartRequest> request, string authorization);
        Task<Result<List<CartResponse>>> getListCart(string authorization);
        
    }
}
