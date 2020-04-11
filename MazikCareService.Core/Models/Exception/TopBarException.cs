using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class TopBarException:CustomException
    {
        public TopBarException(string Message): base(Message)
        {
            this.ResponceCode = HttpStatusCode.InternalServerError;
            this.ExceptionType = mzk_exceptioncategory.AppTopBar.ToString();
        }
    }
}
