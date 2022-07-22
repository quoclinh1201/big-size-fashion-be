using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Customer
    {
        public Customer()
        {
            Addresses = new HashSet<Address>();
            CustomerCarts = new HashSet<CustomerCart>();
            Feedbacks = new HashSet<Feedback>();
            Orders = new HashSet<Order>();
        }

        public int Uid { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public bool? Gender { get; set; }
        public byte? Weight { get; set; }
        public byte? Height { get; set; }
        public bool Status { get; set; }
        public string AvatarUrl { get; set; }

        public virtual Account UidNavigation { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<CustomerCart> CustomerCarts { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
