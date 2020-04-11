using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikLogger
{
    public class CustomFormatException : FormatException
    {

        public string ExceptionType;
        public System.Net.HttpStatusCode ResponceCode;

        public CustomFormatException(string Message) : base(Message) {
            this.ResponceCode = System.Net.HttpStatusCode.InternalServerError;
            this.ExceptionType = "Service";// mzk_exceptioncategory.Service.ToString();
        }
    }
}
