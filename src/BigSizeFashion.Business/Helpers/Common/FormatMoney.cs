using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Common
{
    public static class FormatMoney
    {
        public static string FormatPrice(string price)
        {
            if (price.Length == 0)
                return "0";

            var newPrice = "";
            var count = 1;
            for(int i = price.Length - 1; i >= 0; i--)
            {
                newPrice = price[i] + newPrice;
                count++;
                if(count == 3)
                {
                    newPrice = "." + newPrice;
                    count = 1;
                }
            }
            return newPrice;
        }
    }
}
