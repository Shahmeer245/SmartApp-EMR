using Helper;
using MazikCareService.Core.Models;
using MazikCareWebApi.ApiHelpers;
using MazikCareWebApi.Filters;
using MazikCareWebApi.Models;
using MazikLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MazikCareWebApi.Controllers
{
    [MzkAuthorize]
    public class OptionSetController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> getOptionSetItems(ApiModel inputmodel)
        {
            List<OptionSet> data = new List<OptionSet>();
            //int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            //if (CurrentPage < 0)
            //    return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<OptionSet>> model = new ApiResponseModel<List<OptionSet>>() { };
                var client = ServiceFactory.GetService(typeof(OptionSet));
                data = await client.getOptionSetItems(inputmodel.entityName,inputmodel.OptionSetAttributeValue);
                if (data != null)
                {
                    model.data.records = data;//.Take<OptionSet>(Convert.ToInt32(10)).Skip((CurrentPage - 1) * Convert.ToInt32(AppSettings.GetByKey("PageSize"))).ToList();
                }
                return Response.Success<List<OptionSet>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}
