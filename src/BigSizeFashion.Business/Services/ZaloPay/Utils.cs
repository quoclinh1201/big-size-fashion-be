using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services.ZaloPay
{
    public class Utils
    {
        public static long GetTimeStamp(DateTime date)
        {
            return (long)(date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }

        public static long GetTimeStamp()
        {
            //return GetTimeStamp(DateTime.UtcNow.AddHours(7));
            return GetTimeStamp(DateTime.Now);
        }
    }
}
