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
namespace MazikCareWebApi.Controllers
{
    public class HL7_ORM_O01Controller : ApiController
    {
        //
        // GET: /HL7_ORM_O01/
        public Patient Get()
        {
            Patient result = new Patient();
            result.firstName = "firstName";
            result.lastName = "lastName";
            result.middleName = "niddleName";
            result.mobile = "999999999";
            result.mrn = "mrn";
            result.name = "name";
            result.age = "30";

            return result;
        }
        public async Task<HttpResponseMessage> Get(string patientId,string OrderId, string apptRecId, string caseId,string OrderStatus, string msgControlId)
        {
            HL7_ORM_O01 result;
            try
            {
                SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
                SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
                SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");
                var client = ServiceFactory.GetService(typeof(HL7_ORM_O01));
                result = await client.GetRIS_ORM_O01_Message(patientId,OrderId, apptRecId, caseId,OrderStatus);
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
                RIS risObj = new RIS();
                risObj.ReceiveORMO01Acknowledgment(ack.Acknowledgment);
            }
        }
        [HttpPost]
        async public void ORMOutBound(RISORMO01OutboundParam outbound)
        {
            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            if (outbound != null)
            {
                RIS risObj = new RIS();
                outbound.RISORMO01Outbound.MessageControlId = outbound.RISORMO01Outbound.OrderId;
                risObj.ORMOutBound(outbound.RISORMO01Outbound);
            }
        }
    }
}