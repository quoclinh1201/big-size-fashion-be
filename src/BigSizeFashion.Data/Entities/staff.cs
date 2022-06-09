using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class staff
    {
        public staff()
        {
            ImportInvoices = new HashSet<ImportInvoice>();
            Orders = new HashSet<Order>();
        }

        public int Uid { get; set; }
        public int StoreId { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Status { get; set; }

        public virtual Store Store { get; set; }
        public virtual Account UidNavigation { get; set; }
        public virtual ICollection<ImportInvoice> ImportInvoices { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
