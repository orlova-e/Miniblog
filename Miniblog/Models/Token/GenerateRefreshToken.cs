using System;
using System.Security.Cryptography;

namespace Miniblog.Models.Token
{
    public class GenerateRefreshToken
    {
        public string Generate()
        {
            var randomNum = new byte[32];
            using(var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(randomNum);
                return Convert.ToBase64String(randomNum);
            }
        }
    }
}
