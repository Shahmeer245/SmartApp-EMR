using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class ValidationException : CustomException
    {
        public ValidationException(string Message): base(Message)
        {
            this.ResponceCode = HttpStatusCode.InternalServerError;
            this.ExceptionType = mzk_exceptioncategory.AppPopup.ToString();
        }
    }
}
