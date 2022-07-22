using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Colour
    {
        public Colour()
        {
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int ColourId { get; set; }
        public string ColourName { get; set; }
        public string ColourCode { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
