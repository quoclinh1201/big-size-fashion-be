using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Common
{
    public static class ErrorHelpers
    {
        public static Error PopulateError(int code, string type, string message)
        {
            return new Error
            {
                Code = code,
                Type = type,
                Message = message
            };
        }
    }
}
