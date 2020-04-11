using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class PatientVisit
    {
        public string VisitReason { set; get; }


       

        //public async Task<List<PatientVisit>> getVisitReason(long patientRecId, string encounterId)
        //{
        //    PatientVisitRepository repo = new PatientVisitRepository();

        //    List<PatientVisit> patientVisit = new List<PatientVisit>();

        //    PatientEncounter patEnc = new PatientEncounter();
        //    patEnc.EncounterId = encounterId;

        //    patEnc = patEnc.encounterDetails(patEnc).Result.First();

        //    Guid caseGuid = Guid.Empty;
        //    long appointmentrecId = 0;

        //    if(patEnc != null)
        //    {
        //        if(patEnc.AppointmentRefRecId > 0)
        //        {
        //            appointmentrecId = patEnc.AppointmentRefRecId;
        //        }
        //        else if (!string.IsNullOrEmpty(patEnc.CaseId))
        //        {
        //            caseGuid = new Guid(patEnc.CaseId);
        //        }
        //    }
        //    HMPatientVisitContract[] contractList = null;
        //    //TODO: CRM implementation: Visit Reason
            
                
        //    if(contractList != null)
        //    {
        //        foreach (HMPatientVisitContract contract in contractList)
        //        {
        //            PatientVisit model = new PatientVisit();

        //            if (!string.IsNullOrEmpty(contract.parmVisitReason))
        //            {
        //                model.VisitReason = contract.parmVisitReason;
        //            }

        //            patientVisit.Add(model);
        //        }
        //    }
            
        //    return patientVisit;
        //}        

        //public async Task<bool> updateVisitReason(string encounterId, string visitReason)
        //{
        //    try
        //    {
        //        PatientVisitRepository repo = new PatientVisitRepository();

        //        PatientEncounter patEnc = new PatientEncounter();
        //        patEnc.EncounterId = encounterId;

        //        patEnc = patEnc.encounterDetails(patEnc).Result.First();

        //        Guid caseGuid = Guid.Empty;
        //        long appointmentrecId = 0;

        //        if (patEnc != null)
        //        {
        //            if (patEnc.AppointmentRefRecId > 0)
        //            {
        //                appointmentrecId = patEnc.AppointmentRefRecId;
        //            }
        //            else if (!string.IsNullOrEmpty(patEnc.CaseId))
        //            {
        //                caseGuid = new Guid(patEnc.CaseId);
        //            }
        //        }

        //        return repo.updateVisitReason(caseGuid, appointmentrecId, visitReason);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<List<PatientVisit>> getVisitReason(string customerId, string encounterId, string caseId = "", string appointmentId = "")
        {
            try
            {
                PatientVisit patientVisit;
                Guid caseGuid = Guid.Empty;
                List<PatientVisit> visitReasonList = new List<PatientVisit>();

                if (!string.IsNullOrEmpty(encounterId))
                {
                    PatientEncounter patEnc = new PatientEncounter();
                    patEnc.EncounterId = encounterId;

                    patEnc = patEnc.getEncounterDetails(patEnc).Result.First();                    

                    if (patEnc != null)
                    {
                        if (!string.IsNullOrEmpty(patEnc.AppointmentId))
                        {
                            appointmentId = patEnc.AppointmentId;
                        }
                        else if (!string.IsNullOrEmpty(patEnc.CaseId))
                        {
                            caseId = patEnc.CaseId;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(appointmentId) && !string.IsNullOrEmpty(caseId))
                    {
                        caseGuid = new Guid(caseId);
                    }
                }

                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(msdyn_workorder.EntityLogicalName);
                query.ColumnSet = new ColumnSet("msdyn_workordersummary");
                               
                if (!string.IsNullOrEmpty(customerId))
                {
                    query.Criteria.AddCondition("mzk_customer", ConditionOperator.Equal, new Guid(customerId));
                }

                if (caseGuid != Guid.Empty)
                {
                    query.Criteria.AddCondition("msdyn_servicerequest", ConditionOperator.Equal, caseGuid);
                }

                if (!string.IsNullOrEmpty(appointmentId))
                {
                    LinkEntity apptEntity = new LinkEntity(msdyn_workorder.EntityLogicalName, mzk_patientappointment.EntityLogicalName, "msdyn_workorderid", "mzk_workorderid", JoinOperator.Inner);

                    apptEntity.LinkCriteria.AddCondition("mzk_patientappointmentid", ConditionOperator.Equal, new Guid(appointmentId));

                    query.LinkEntities.Add(apptEntity);
                }

                EntityCollection entityCollection = repo.GetEntityCollection(query);                

                foreach (Entity entity in entityCollection.Entities)
                {
                    patientVisit = new PatientVisit();

                    if (entity.Attributes.Contains("msdyn_workordersummary"))
                    {
                        patientVisit.VisitReason = entity.Attributes["msdyn_workordersummary"].ToString();
                    }
                    visitReasonList.Add(patientVisit);
                }

                return visitReasonList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> updateVisitReason(string encounterId, string visitReason)
        {
            try
            {
                PatientEncounter patEnc = new PatientEncounter();
                patEnc.EncounterId = encounterId;

                patEnc = patEnc.getEncounterDetails(patEnc).Result.First();

                string caseId = string.Empty;
                string appointmentId = string.Empty;

                if (patEnc != null)
                {
                    if (!string.IsNullOrEmpty(patEnc.AppointmentId))
                    {
                        appointmentId = patEnc.AppointmentId;
                    }
                    else if (!string.IsNullOrEmpty(patEnc.CaseId))
                    {
                        caseId = patEnc.CaseId;
                    }
                }

                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(mzk_patientencounter.EntityLogicalName);
                query.ColumnSet = new ColumnSet(false);

                if (!string.IsNullOrEmpty(encounterId))
                {
                    query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(encounterId));
                }

                LinkEntity workOrder = new LinkEntity(mzk_patientencounter.EntityLogicalName, msdyn_workorder.EntityLogicalName, "mzk_workorder", "msdyn_workorderid", JoinOperator.Inner);
                
                if (!string.IsNullOrEmpty(caseId))
                {
                    workOrder.LinkCriteria.AddCondition("msdyn_servicerequest", ConditionOperator.Equal, new Guid(caseId));
                }
                if (!string.IsNullOrEmpty(appointmentId))
                {
                    workOrder.LinkCriteria.AddCondition("msdyn_workorderid", ConditionOperator.Equal, new Guid(appointmentId));
                }

                workOrder.Columns = new ColumnSet("msdyn_workordersummary");

                EntityCollection entityCollection = repo.GetEntityCollection(query);

                foreach (Entity entity in entityCollection.Entities)
                {
                    if (!string.IsNullOrEmpty(visitReason))
                    {
                        entity.Attributes["msdyn_workordersummary"] = visitReason;
                    }

                    repo.UpdateEntity(entity);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
    }
}
