using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Size
    {
        public Size()
        {
            Products = new HashSet<Product>();
        }

        public int SizeId { get; set; }
        public string Size1 { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
