using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{

    public class ServiceException : CustomException
    {
      
        public ServiceException(string Message): base(Message)
            {
                this.ResponceCode   = HttpStatusCode.InternalServerError;
                this.ExceptionType  = mzk_exceptioncategory.Service.ToString();
            }
    }
}
