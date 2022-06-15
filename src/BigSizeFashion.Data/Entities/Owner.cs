using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Owner
    {
        public int Uid { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Status { get; set; }

        public virtual Account UidNavigation { get; set; }
    }
}
