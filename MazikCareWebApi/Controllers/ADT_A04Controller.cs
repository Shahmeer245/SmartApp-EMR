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
    public class ADT_A04Controller : ApiController
    {
        //
        // GET: /ADT_A04_/
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
        public async Task<HttpResponseMessage> Get(string aptRecId, string caseId, string patientId,string msgControlId)
        {
            ADT_A04_Message result;
            try
            {

                SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
                SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
                SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

                var client = ServiceFactory.GetService(typeof(ADT_A04_Message));
                result = await client.GetADT_A04_Message(aptRecId, caseId, patientId);
                result.MsgControlId = msgControlId;
                return Request.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        async public void Get(string ackCode)
        {
            if (string.IsNullOrEmpty(ackCode))
            {
                var client = ServiceFactory.GetService(typeof(RIS));

                await client.ReceiveADTA04Acknowledgment(ackCode);
            }
        }
        [HttpPost]
        async public void SendAck(AckParam ack)
        {
            if (ack != null)
            {
                RIS risObj = new RIS();
                risObj.ReceiveADTA04Acknowledgment(ack.Acknowledgment);
            }
        }
    }
}