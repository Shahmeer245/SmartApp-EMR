using MazikCareService.Core.Models;
using MazikCareService.Core.Models.HL7;
using MazikCareWebApi.ApiHelpers;
using MazikCareWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using MazikLogger;
using MazikCareService.CRMRepository;
using Helper;
using System.Web.Http.Results;
using System.Net;
using System.Text;

namespace MazikCareWebApi.Controllers
{
     public class LIS_ORMO01Controller : ApiController
    {
        //
        // GET: /HL7_ORM_O01/
      
        public async Task<HttpResponseMessage> Get(string patientId, string OrderId, string apptRecId, string caseId, string OrderStatus, string msgControlId)
        {
            HL7_ORM_O01 result;
            try
            {
                SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
                SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
                SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");
                var client = ServiceFactory.GetService(typeof(HL7_ORM_O01));
                result = await client.GetLIS_ORM_O01_Message(patientId, OrderId, apptRecId, caseId, OrderStatus);
                result.MsgControlId = msgControlId;
                return Request.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        async public void SendAck(AckParam ack)
        {
            if (ack != null)
            {
                LIS lisObj = new LIS();
                lisObj.ReceiveORMO01Acknowledgment(ack.Acknowledgment);
            }
        }

        [HttpPost]
        public HttpResponseMessage ORMOutBound(LISORMO01OutboundParam outbound)
        {
            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            string message = string.Empty;

            if (outbound != null)
            {
                LIS lisObj = new LIS();
                var nteCommentList = outbound.LISORMO01Outbound.Comment.Split
                    ('^');
                outbound.LISORMO01Outbound.NTE = new NTE
                {
                    Code = nteCommentList[0],
                    CommentText = nteCommentList[1],
                    CommentDescription=nteCommentList[2]
                };
                //orderstatus
                //dc
                //sc
                if (outbound.LISORMO01Outbound.OrderStatus == "SC")
                    outbound.LISORMO01Outbound.OrderStatus = outbound.LISORMO01Outbound.NTE.Code;
              
                //outbound.LISORMO01Outbound.MessageControlId = outbound.LISORMO01Outbound.OrderId;
                                
                message = lisObj.ORMOutBound(outbound.LISORMO01Outbound);
            }
            else
            {
                message = MazikCareService.Core.Models.HL7.Helper.generateACK("O01", "", "AE", "Invalid message format");
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StringContent(message, Encoding.UTF8, "application/json");

            return response;
        }
    }
}