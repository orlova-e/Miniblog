using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Token
{
    public interface IJwtSigningDecodingKey
    {
        SecurityKey GetSecurityKey();
    }
}
