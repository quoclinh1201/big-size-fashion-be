using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class ProductImage
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsReferenceImage { get; set; }

        public virtual Product Product { get; set; }
    }
}
