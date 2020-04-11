using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MazikCareService.Core.Models.Integration;
using MazikCareWebApi.ApiHelpers;
using System.Dynamic;

namespace MazikCareWebApi.Controllers
{
    public class IntegrationController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> getMessage(MessageHeader messageHeader)
        {
            ExpandoObject data = new ExpandoObject();

            try
            {
                ApiResponseModel<ExpandoObject> model = new ApiResponseModel<ExpandoObject>() { };

                var client = ServiceFactory.GetService(typeof(MessageHeader));
                data = await client.getMessage(((EnumTriggerEvent)Convert.ToInt32(messageHeader.TriggerEvent)), messageHeader.fieldValues);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<ExpandoObject>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> receiveMessageFromCommunicationLayer(MessageHeader messageHeader)
        {
            ExpandoObject data = new ExpandoObject();

            try
            {
                ApiResponseModel<ExpandoObject> model = new ApiResponseModel<ExpandoObject>() { };

                var client = ServiceFactory.GetService(typeof(MessageHeader));
                data = await client.receiveMessage(messageHeader);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<ExpandoObject>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}
