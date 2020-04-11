using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MazikCareWebApi.ApiHelpers
{
   public class Session
    {
        private static string sessionKey = "Authorization";

        public static bool ifSessionExists { get { return HttpContext.Current.Request.Headers["Authorization"] == null ? false : true; } }
    }
}
