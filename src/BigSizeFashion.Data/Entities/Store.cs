using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Store
    {
        public Store()
        {
            CustomerCarts = new HashSet<CustomerCart>();
            Orders = new HashSet<Order>();
            StoreWarehouses = new HashSet<StoreWarehouse>();
            staff = new HashSet<staff>();
        }

        public int StoreId { get; set; }
        public string StoreAddress { get; set; }
        public string StorePhone { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<CustomerCart> CustomerCarts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<StoreWarehouse> StoreWarehouses { get; set; }
        public virtual ICollection<staff> staff { get; set; }
    }
}
