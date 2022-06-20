using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Colour
    {
        public Colour()
        {
            Products = new HashSet<Product>();
        }

        public int ColourId { get; set; }
        public string ColourName { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
