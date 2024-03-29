﻿using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Address
    {
        public Address()
        {
            Orders = new HashSet<Order>();
        }

        public int AddressId { get; set; }
        public int CustomerId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiveAddress { get; set; }
        public string ReceiverPhone { get; set; }
        public bool Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
