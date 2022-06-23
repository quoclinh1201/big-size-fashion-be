using BigSizeFashion.Business.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.ResponseObjects
{
    public class GetDetailProductResponse
    {
        public int ProductId { get; set; }
        public int ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
        public string PromotionValue { get; set; }
        public string Category { get; set; }
        //public List<SizeResponse> Size { get; set; } = new List<SizeResponse>();
        //public List<ColourResponse> Colour { get; set; } = new List<ColourResponse>();
        public List<ProductDetailResponse> ProductDetailList { get; set; } = new List<ProductDetailResponse>();
        public string Gender { get; set; }
        public List<ProductImageResponse> Images { get; set; } = new List<ProductImageResponse>();
        public string Brand { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
