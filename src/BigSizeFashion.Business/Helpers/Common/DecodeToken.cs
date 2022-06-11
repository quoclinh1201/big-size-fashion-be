using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.Common
{
    public static class DecodeToken
    {
        public static int DecodeTokenToGetUid(string token)
        {
            var decode = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            var uid = Convert.ToInt32(decode.Claims.FirstOrDefault(claim => claim.Type == "Uid").Value);
            return uid;
        }
    }
}
