using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Token
{
    public interface IJwtSigningEncodingKey
    {
        string SigningAlgorithm { get; }
        SecurityKey GetSecurityKey();
    }
}
