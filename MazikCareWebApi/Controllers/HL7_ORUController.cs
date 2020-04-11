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
using System.Net;
using System.Text;

namespace MazikCareWebApi.Controllers
{
    public class HL7_ORUController : ApiController
    {
        //
        // GET: /HL7_ORM_O01/
       
        [HttpPost]
        async public void RIS_ORUResult(RISORUR01OutboundParam ack)
        {
            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            string url = System.Configuration.ConfigurationManager.AppSettings["RISImgurl"];
            var reportPath = System.Configuration.ConfigurationManager.AppSettings["RISReportsSharedFolder"];
            url = url.Replace("$", "&");
            if (ack != null)
            {
                url = url + ack.RISORUR01Outbound.PatientId + "&AccessionNumber=" + ack.RISORUR01Outbound.AccessionNumber;
                RIS risObj = new RIS();
                ack.RISORUR01Outbound.ImageUrl = url;
                ack.RISORUR01Outbound.ReportPath = reportPath + ack.RISORUR01Outbound.ReportPath;
                ack.RISORUR01Outbound.MessageControlId = ack.RISORUR01Outbound.OrderId;
                risObj.ORUResult(ack.RISORUR01Outbound);
            }
        }

        [HttpPost]
        public HttpResponseMessage LIS_ORUResult(LISORUR01OutboundParam ack)
        {
            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");
            string message = string.Empty;
            var reportPath = System.Configuration.ConfigurationManager.AppSettings["LISReportsSharedFolder"];
            if (ack != null)
            {
                LIS lisObj = new LIS();
               
                //ack.LISORUR01Outbound.MessageControlId = ack.LISORUR01Outbound.OrderId;
                ack.LISORUR01Outbound.ReportPath = reportPath + ack.LISORUR01Outbound.ReportPath;
                message = lisObj.ORUResult(ack.LISORUR01Outbound);
            }
            else
            {
                message = MazikCareService.Core.Models.HL7.Helper.generateACK("R01", "", "AE", "Invalid message format");
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StringContent(message, Encoding.UTF8, "application/json");

            return response;
        }
    }
}