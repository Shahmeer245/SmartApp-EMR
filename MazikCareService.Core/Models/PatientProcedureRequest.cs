using Helper;
using MazikCareService.AXRepository;
using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class PatientProcedureRequest
    {

        public string Title { get; set; }
        public string ProcedureRequestId { get; set; }
        public Guid PatientID { get; set; }
        public string Externalemrid { get; set; }
        public DateTime RecordedDate { get; set; }

        public async Task<Guid> CreatePatientProcedureCRM(PatientProcedureRequest condition)
        {
            try
            {

                Entity contact = new Entity("msemr_procedurerequest");

                if (condition.Externalemrid != "")
                {
                    contact["mzk_externalemrid"] = condition.Externalemrid;
                }
                if (condition.PatientID != Guid.Empty)
                {
                    contact["msemr_subjectpatient"] = new EntityReference("contact", condition.PatientID);
                }
                if (condition.Title != "")
                {
                    contact["msemr_name"] = condition.Title;
                }
                if (condition.RecordedDate != null)
                {
                    contact["msemr_authoredon"] = Convert.ToDateTime(condition.RecordedDate);
                }

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();


                QueryExpression queryExpression = new QueryExpression("msemr_procedurerequest");
                queryExpression.Criteria.AddCondition("mzk_externalemrid", ConditionOperator.Equal, condition.Externalemrid);

                queryExpression.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryExpression);

                if (entitycollection != null)
                {
                    if (entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities[0].Attributes.Contains("msemr_procedurerequestid"))
                        {
                            contact["msemr_procedurerequestid"] = new Guid(entitycollection.Entities[0].Attributes["msemr_procedurerequestid"].ToString());
                        }
                        entityRepository.UpdateEntity(contact);
                        contact.Id = new Guid(contact["msemr_procedurerequestid"].ToString());
                    }
                    else
                    {
                        contact.Id = entityRepository.CreateEntity(contact);
                    }
                }
                else
                {
                    contact.Id = entityRepository.CreateEntity(contact);
                }

                return contact.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<PatientProcedureRequest>> getPatientProcedureCRM(string patientGuid, DateTime startdate, DateTime enddate)
        {
            try
            {
                List<PatientProcedureRequest> list = new List<PatientProcedureRequest>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression("msemr_procedurerequest");

                if (string.IsNullOrEmpty(patientGuid))
                {
                    throw new ValidationException("Patient Id must be specified");
                }

                query.Criteria.AddCondition("msemr_subjectpatient", ConditionOperator.Equal, new Guid(patientGuid));
                //query.Criteria.AddCondition("msemr_planstartdate", ConditionOperator.GreaterEqual, Convert.ToDateTime(startdate));
                //query.Criteria.AddCondition("msemr_planenddate", ConditionOperator.LessEqual, Convert.ToDateTime(enddate));

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);


                if (entitycollection != null)
                {
                    for (int i = 0; i < entitycollection.Entities.Count; i++)
                    {
                        PatientProcedureRequest obj = new PatientProcedureRequest();
                        obj = getPatientProcedureRequestModelFilled(entitycollection[i], obj, "");
                        list.Add(obj);
                    }
                }



                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static PatientProcedureRequest getPatientProcedureRequestModelFilled(Entity entity, PatientProcedureRequest obs, string accountAlias)
        {
            if (entity.Attributes.Contains("msemr_name"))
            {
                obs.Title = (entity.Attributes["msemr_name"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_procedurerequestid"))
            {
                obs.ProcedureRequestId = (entity.Attributes["msemr_procedurerequestid"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_subjectpatient"))
            {
                obs.PatientID = new Guid((entity.Attributes["msemr_subjectpatient"] as EntityReference).Id.ToString());
            }

            if (entity.Attributes.Contains("msemr_authoredon"))
            {
                obs.RecordedDate = Convert.ToDateTime(entity.Attributes["msemr_authoredon"]);
            }

            return obs;
        }

    }
}
