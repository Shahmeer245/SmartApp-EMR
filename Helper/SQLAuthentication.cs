using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class SQLAuthentication : IAuthentication
    {
        public TokenClass AuthenticateUser(string domain, string username, string pwd)
        {
            return new TokenClass();
        }
    }
}
