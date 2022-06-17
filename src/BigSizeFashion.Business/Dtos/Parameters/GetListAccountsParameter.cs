using BigSizeFashion.Business.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Parameters
{
    public class GetListAccountsParameter : QueryStringParameters
    {
        public string Fullname { get; set; }

        [Required]
        public AccountRoleEnum Role { get; set; }

        [Required]
        public StatusEnum Status { get; set; }
    }
}
