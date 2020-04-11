using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Models.HL7;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class PatientCondition
    {
        public string ConditionID { get; set; }
        public Guid PatientID { get; set; }
        public string Title { get; set; }        
        public string Externalemrid { get; set; }
        public DateTime RecordedDate { get; set; }

        public async Task<Guid> CreatePatientConditionCRM(PatientCondition condition)
        {
            try
            {

                Entity contact = new Entity("msemr_condition");

                if (condition.Externalemrid != "")
                {
                    contact["mzk_externalemrid"] = condition.Externalemrid;
                }
                if (condition.PatientID != Guid.Empty)
                {
                    contact["msemr_subjecttypepatient"] = new EntityReference("contact", condition.PatientID);
                }
                if (condition.Title != "")
                {
                    contact["msemr_name"] = condition.Title;
                }
                if (condition.RecordedDate != null)
                {
                    contact["msemr_onsetdate"] = Convert.ToDateTime(condition.RecordedDate);
                }

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();


                QueryExpression queryExpression = new QueryExpression("msemr_condition");
                queryExpression.Criteria.AddCondition("mzk_externalemrid", ConditionOperator.Equal, condition.Externalemrid);

                queryExpression.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryExpression);

                if (entitycollection != null)
                {
                    if (entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities[0].Attributes.Contains("msemr_conditionid"))
                        {
                            contact["msemr_conditionid"] = new Guid(entitycollection.Entities[0].Attributes["msemr_conditionid"].ToString());
                        }
                        entityRepository.UpdateEntity(contact);
                        contact.Id = new Guid(contact["msemr_conditionid"].ToString());
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

        public async Task<List<PatientCondition>> getPatientConditionCRM(string patientGuid, DateTime startdate, DateTime enddate)
        {
            try
            {
                List<PatientCondition> list = new List<PatientCondition>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression("msemr_condition");

                if (string.IsNullOrEmpty(patientGuid))
                {
                    throw new ValidationException("Patient Id must be specified");
                }

                query.Criteria.AddCondition("msemr_subjecttypepatient", ConditionOperator.Equal, new Guid(patientGuid));
                //query.Criteria.AddCondition("msemr_planstartdate", ConditionOperator.GreaterEqual, Convert.ToDateTime(startdate));
                //query.Criteria.AddCondition("msemr_planenddate", ConditionOperator.LessEqual, Convert.ToDateTime(enddate));

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);


                if (entitycollection != null)
                {
                    for (int i = 0; i < entitycollection.Entities.Count; i++)
                    {
                        PatientCondition obj = new PatientCondition();
                        obj = getPatientConditionModelFilled(entitycollection[i], obj, "");
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

        public static PatientCondition getPatientConditionModelFilled(Entity entity, PatientCondition obs, string accountAlias)
        {
            if (entity.Attributes.Contains("msemr_name"))
            {
                obs.Title = (entity.Attributes["msemr_name"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_conditionid"))
            {
                obs.ConditionID = (entity.Attributes["msemr_conditionid"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_subjecttypepatient"))
            {
                obs.PatientID = new Guid((entity.Attributes["msemr_subjecttypepatient"] as EntityReference).Id.ToString());
            }

            if (entity.Attributes.Contains("msemr_onsetdate"))
            {
                obs.RecordedDate = Convert.ToDateTime(entity.Attributes["msemr_onsetdate"]);
            }

            return obs;
        }

    }
}
