using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MazikLogger
{
    public class AuthenticationException : CustomException
    {
        public AuthenticationException(string Message): base(Message)
        {
            this.ResponceCode = HttpStatusCode.Unauthorized;
            this.ExceptionType = "AppPopup";
        }

        public static AuthenticationException create(string message, string helpLink, string source)
        {
            AuthenticationException ex = new AuthenticationException(message);

            ex.HelpLink = helpLink;
            ex.Source = source;

            return ex;
        }
    }
}
