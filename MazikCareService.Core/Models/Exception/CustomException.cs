using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class CustomException:Exception
    {
        public  string ExceptionType;
        public  System.Net.HttpStatusCode ResponceCode;
        public CustomException(string Message) : base(Message) {
            this.ResponceCode = System.Net.HttpStatusCode.InternalServerError;
            this.ExceptionType = mzk_exceptioncategory.Service.ToString();
        }
    }
}
