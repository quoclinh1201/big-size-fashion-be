using System;
using System.Collections.Generic;

#nullable disable

namespace BigSizeFashion.Data.Entities
{
    public partial class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        public int RoleId { get; set; }
        public string Role1 { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
