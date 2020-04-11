using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MazikLogger
{
    public class ValidationException : CustomException
    {
        public ValidationException(string Message): base(Message)
        {
            this.ResponceCode = HttpStatusCode.OK;
            this.ExceptionType = "AppTopbar";// mzk_exceptioncategory.AppPopup.ToString();
        }

        public static ValidationException create(string message, string helpLink, string source)
        {
            ValidationException ex = new ValidationException(message);

            ex.HelpLink = helpLink;
            ex.Source = source;

            return ex;
        }
    }
}
