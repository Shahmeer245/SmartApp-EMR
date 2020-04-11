using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class CheifComplaint:ICheifComplaint
    {
        public string Complaint { set; get; }             

        public async Task<List<CheifComplaint>> getCheifComplaint(string patientguid, string encounterId, long appointmentRecId, string caseId)
        {

            List<CheifComplaint> CheifComplaint = new List<CheifComplaint>();

            #region Patient CheifComplaint Query
            QueryExpression query = new QueryExpression("mzk_patientencounter");

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_cheifcomplaint");

            if (patientguid != null && patientguid != string.Empty)
            {
                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                childFilter.AddCondition("mzk_customer", ConditionOperator.Equal, new Guid(patientguid));

                FilterExpression childFilter1 = query.Criteria.AddFilter(LogicalOperator.And);
                childFilter1.AddCondition("mzk_cheifcomplaint", ConditionOperator.NotEqual, string.Empty);
            }
            else if (encounterId != null && encounterId != string.Empty)
            {
                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(encounterId));
            }


            if (!string.IsNullOrEmpty(caseId) || appointmentRecId > 0)
            {
                LinkEntity EntityEncounter = new LinkEntity("mzk_cheifcomplaint", "mzk_patientencounter", "mzk_patientencounterid", "mzk_patientencounterid", JoinOperator.Inner);

                if (!string.IsNullOrEmpty(caseId))
                {
                    EntityEncounter.LinkCriteria.AddCondition("mzk_caseid", ConditionOperator.Equal, new Guid(caseId));
                }

                if (appointmentRecId > 0)
                {
                    EntityEncounter.LinkCriteria.AddCondition("mzk_axrefappointmentrecid", ConditionOperator.Equal, Convert.ToDecimal(appointmentRecId));
                }

                query.LinkEntities.Add(EntityEncounter);
            }

            #endregion

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                CheifComplaint model = new CheifComplaint();

                if (entity.Attributes.Contains("mzk_cheifcomplaint"))
                {
                    model.Complaint = (entity.Attributes["mzk_cheifcomplaint"]).ToString();
                }

                CheifComplaint.Add(model);
            }
            return CheifComplaint;
        }

        public async Task<List<CheifComplaint>> getCheifComplaintFromClinicalTemplate(string caseId, string searchText, string appointmentId)
        {
            List<CheifComplaint> CheifComplaint = new List<CheifComplaint>();

            ClinicalTemplate clinicalTemplate = new ClinicalTemplate();

            string encounterId = "";
            PatientEncounter patEnc = new PatientEncounter();

            if (!string.IsNullOrEmpty(appointmentId))
            {
                patEnc = patEnc.encounterDetails((int)mzk_encountertype.Consultation, "", "" , appointmentId).Result;

                if (patEnc != null)
                {
                    encounterId = patEnc.EncounterId;
                }
            }
            else if (!string.IsNullOrEmpty(caseId))
            {
                patEnc.CaseId = caseId;
                patEnc.EncounterType = ((int)mzk_encountertype.PrimaryAssessment).ToString();
                List<PatientEncounter> listEnc = null;

                listEnc = patEnc.getEncounterDetails(patEnc).Result;

                if (listEnc != null && listEnc.FirstOrDefault() != null)
                {
                    encounterId = listEnc.First().EncounterId;
                }
            }

            List<ClinicalTemplateNarration> listNarration = clinicalTemplate.getPatientsClinicalTempalteNarration("", encounterId, "", false, 0 , false, searchText);

            foreach (ClinicalTemplateNarration narration in listNarration)
            {
                CheifComplaint model = new CheifComplaint();

                if (!string.IsNullOrEmpty(narration.comments))
                {
                    model.Complaint = narration.comments;
                }

                CheifComplaint.Add(model);
            }


            return CheifComplaint;
        }

        public async Task<bool> updateCheifComplaint(string encounterId, string CheifComplaint)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Entity encounterEntity = entityRepository.GetEntity("mzk_patientencounter", new Guid(encounterId), new ColumnSet("mzk_cheifcomplaint"));
                encounterEntity.Attributes["mzk_cheifcomplaint"] = CheifComplaint;

                entityRepository.UpdateEntity(encounterEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
 