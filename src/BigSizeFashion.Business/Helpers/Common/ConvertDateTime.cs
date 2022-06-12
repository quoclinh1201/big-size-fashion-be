using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Common
{
    public static class ConvertDateTime
    {
        public static string ConvertDateTimeToString(DateTime? dateTime)
        {
            if (dateTime != null) return dateTime?.ToString("dd/MM/yyyy");
            return null;
        }

        public static DateTime? ConvertStringToDatetime(string s)
        {
            if (s != null) return DateTime.ParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return null;
        }
    }
}
