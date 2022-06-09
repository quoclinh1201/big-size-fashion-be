using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Account
    {
        public Account()
        {
            Notifications = new HashSet<Notification>();
        }

        public int Uid { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreateAt { get; set; }
        public bool Status { get; set; }

        public virtual Role Role { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual staff staff { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
