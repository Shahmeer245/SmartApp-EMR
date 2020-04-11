using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models.HL7
{
    public class ADT_A08_Message
    {
        public HL7_Patient PID { set; get; }
        public HL7_PatientVIsit PV1 { set; get; }
        public HL7_EventType EVN { set; get; }
        public string MsgControlId { set; get; }
        public async Task<ADT_A08_Message> GetADT_A08_Message(string appointmentRecId, string caseId, string patientGuId)
        {
            try
            {
                ADT_A08_Message Adt_A08_Msg = new ADT_A08_Message();
                ADT_A04_Message _obj_A04_Message = new ADT_A04_Message();
                var result = await _obj_A04_Message.GetADT_A04_Message(appointmentRecId, caseId,patientGuId);
                Adt_A08_Msg.PID = result.PID;
                Adt_A08_Msg.PV1 = result.PV1;
                Adt_A08_Msg.EVN = result.EVN;
                return Adt_A08_Msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string getPatientRecIdFromGuid(string patientGuId)
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(Account.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_axrefrecid");

            query.Criteria.AddCondition("accountid", ConditionOperator.Equal, new Guid(patientGuId));

            EntityCollection entitycol = repo.GetEntityCollection(query);

            if (entitycol.Entities.Count > 0)
            {
                return entitycol.Entities[0]["mzk_axrefrecid"].ToString();
            }

            return string.Empty;
        }
    }

}
