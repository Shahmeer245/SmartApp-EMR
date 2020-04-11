using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MazikLogger
{
    public class CustomException:Exception
    {
        public  string ExceptionType;
        public  System.Net.HttpStatusCode ResponceCode;
        public CustomException(string Message) : base(Message) {
            this.ResponceCode = System.Net.HttpStatusCode.InternalServerError;
            this.ExceptionType = "AppPopup";// mzk_exceptioncategory.Service.ToString();                      
        }
    }
}
