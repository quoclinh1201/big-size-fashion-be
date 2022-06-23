using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Common
{
    public static class FormatMoney
    {
        public static string FormatPrice(decimal? price)
        {
            if (price == null)
                return null;

            if (price == 0)
                return "0 VNĐ";

            var priceString = price.ToString().Substring(0, price.ToString().Length - 5);
            var newPrice = "";
            var count = 0;
            for(int i = priceString.Length - 1; i >= 0; i--)
            {
                newPrice = priceString[i] + newPrice;
                count++;
                if(count == 3 && i != 0)
                {
                    newPrice = "." + newPrice;
                    count = 0;
                }
            }
            return newPrice + " VNĐ";
        }
    }
}
