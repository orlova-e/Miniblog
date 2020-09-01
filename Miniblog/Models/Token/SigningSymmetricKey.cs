using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miniblog.Token
{
    public class SigningSymmetricKey : IJwtSigningEncodingKey, IJwtSigningDecodingKey
    {
        private readonly SymmetricSecurityKey _secretKey;
        public string SigningAlgorithm { get; } = SecurityAlgorithms.HmacSha256;
        public SigningSymmetricKey(string key)
        {
            this._secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }
        public SecurityKey GetSecurityKey()
            => this._secretKey;
    }
}
