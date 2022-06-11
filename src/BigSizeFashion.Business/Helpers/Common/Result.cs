using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Common
{
    public class Result<T>
    {
        public T Content { get; set; }
        public DateTime ResponseTime { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
