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
    public class PatientCarePlan
    {
        public Guid CarePlanID { get; set; }
        public Guid PatientID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime STartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Externalemrid { get; set; }

        public async Task<Guid> CreatePatientCarePlanCRM(PatientCarePlan careplan)
        {
            try
            {

                Entity contact = new Entity("msemr_careplan");

                if (careplan.Externalemrid != "")
                {
                    contact["mzk_externalemrid"] = careplan.Externalemrid;
                }
                if (careplan.PatientID != Guid.Empty)
                {
                    contact["msemr_patientidentifier"] = new EntityReference("contact", careplan.PatientID);
                }
                if (careplan.Title != "")
                {
                    contact["msemr_title"] = careplan.Title;
                }
                if (careplan.Description != "")
                {
                    contact["msemr_plandescription"] = careplan.Description;
                }
                if (careplan.STartDate != null)
                {
                    contact["msemr_planstartdate"] = Convert.ToDateTime(careplan.STartDate);
                }
                if (careplan.EndDate != null)
                {
                    contact["msemr_planenddate"] = Convert.ToDateTime(careplan.EndDate);
                }

                //contact["msemr_contacttype"] = new OptionSetValue(935000000); // Contact Type: Employee

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();


                QueryExpression queryExpression = new QueryExpression("msemr_careplan");
                queryExpression.Criteria.AddCondition("mzk_externalemrid", ConditionOperator.Equal, careplan.Externalemrid);

                queryExpression.ColumnSet = new ColumnSet(true);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryExpression);

                if (entitycollection != null)
                {
                    if (entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities[0].Attributes.Contains("msemr_careplanid"))
                        {
                            contact["msemr_careplanid"] = new Guid(entitycollection.Entities[0].Attributes["msemr_careplanid"].ToString());
                        }
                        entityRepository.UpdateEntity(contact);
                        contact.Id = new Guid(contact["msemr_careplanid"].ToString());
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

        public async Task<List<PatientCarePlan>> getPatientCareplans(string patientGuid, DateTime startdate, DateTime enddate)
        {
            try
            {
                List<PatientCarePlan> list = new List<PatientCarePlan>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression("msemr_careplan");

                if (string.IsNullOrEmpty(patientGuid))
                {
                    throw new ValidationException("Patient Id must be specified");
                }

                query.Criteria.AddCondition("msemr_patientidentifier", ConditionOperator.Equal, new Guid(patientGuid));
                query.Criteria.AddCondition("msemr_planstartdate", ConditionOperator.GreaterEqual, Convert.ToDateTime(startdate));
                query.Criteria.AddCondition("msemr_planenddate", ConditionOperator.LessEqual, Convert.ToDateTime(enddate));

                query.ColumnSet = new ColumnSet(true);

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                
                if (entitycollection != null)
                {                    
                    for (int i = 0; i < entitycollection.Entities.Count; i++)
                    {
                        PatientCarePlan obj = new PatientCarePlan();
                        obj = getPatientCareplanModelFilled(entitycollection[i], obj, "");
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


        public static PatientCarePlan getPatientCareplanModelFilled(Entity entity, PatientCarePlan patient, string accountAlias)
        {
            if (entity.Attributes.Contains("msemr_title"))
            {
                patient.Title = (entity.Attributes["msemr_title"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_patientidentifier"))
            {                
                patient.PatientID = new Guid((entity.Attributes["msemr_patientidentifier"] as EntityReference).Id.ToString());
            }

            if (entity.Attributes.Contains("msemr_plandescription"))
            {
                patient.Description = (entity.Attributes["msemr_plandescription"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_planstartdate"))
            {
                patient.STartDate = (DateTime)entity.Attributes["msemr_planstartdate"];
            }

            if (entity.Attributes.Contains("msemr_planenddate"))
            {
                patient.EndDate = (DateTime)entity.Attributes["msemr_planenddate"];
            }


            return patient;
        }
    }
}
