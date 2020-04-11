using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    interface IAuthentication
    {
        TokenClass AuthenticateUser(string domain, string username, string pwd);
    }
}
