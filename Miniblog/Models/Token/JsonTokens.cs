using System;

namespace Miniblog.Models.Token
{
    public class JsonTokens
    {
        public string access_token { get; set; }
        public DateTimeOffset access_token_expiration { get; set; }
        public string refresh_token { get; set; }
    }
}
