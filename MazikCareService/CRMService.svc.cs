using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MazikCareService.Core.Models;
using System.ServiceModel.Web;
using MazikCareService.Core;
using Helper;

namespace MazikCareService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CRMService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CRMService.svc or CRMService.svc.cs at the Solution Explorer and start debugging.
    public class CRMService : ICRMService
    {
        [WebInvoke(
          Method = "GET",
          UriTemplate = "/GetData",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetData()
        {
            return "GetData";
        }


        [WebInvoke(
          Method = "GET",
          UriTemplate = "/GetRoles",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public async Task<List<Role>> GetRoles()
        {
            List<Role> data = new List<Role>();
           
            //if (AppSettings.GetByKey("AzureBus") == "true")
            //{
            //    using (var client = ServiceBusFactory.GetService())
            //    {
            //        data = await client.GetRoles();
            //    }
            //}
            //else
            //{
            //    IRoleService service = new RoleService();
            //    data = await service.GetRoles();
            //}

            return data;
        }

    }
}
