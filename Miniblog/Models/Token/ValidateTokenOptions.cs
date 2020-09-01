using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Token
{
    public class ValidateTokenOptions
    {
        public const string ISSUER = "MiniblogServer";
        public const string AUDIENCE = "MiniblogClien";
        public const int LIFETIME = 5; // minutes
    }
}
