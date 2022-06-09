using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Color
    {
        public Color()
        {
            Products = new HashSet<Product>();
        }

        public int ColorId { get; set; }
        public string Color1 { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
