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
    public class PatientObservation
    {
        public string ObservationID { get; set; }
        public Guid PatientID { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Externalemrid { get; set; }
        public DateTime IssuedDate { get; set; }

        public async Task<Guid> CreatePatientObservationCRM(PatientObservation obv)
        {
            try
            {

                Entity contact = new Entity("msemr_observation");

                if (obv.Externalemrid != "")
                {
                    contact["mzk_externalemrid"] = obv.Externalemrid;
                }
                if (obv.PatientID != Guid.Empty)
                {
                    contact["msemr_subjecttypepatient"] = new EntityReference("contact", obv.PatientID);
                }
                if (obv.Description != "")
                {
                    contact["msemr_description"] = obv.Description;
                }
                if (obv.CreatedOn != null)
                {
                    contact["createdon"] = Convert.ToDateTime(obv.CreatedOn);
                }
                if (obv.IssuedDate != null)
                {
                    contact["msemr_effectivestart"] = Convert.ToDateTime(obv.IssuedDate);
                }

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();


                QueryExpression queryExpression = new QueryExpression("msemr_observation");
                queryExpression.Criteria.AddCondition("mzk_externalemrid", ConditionOperator.Equal, obv.Externalemrid);

                queryExpression.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryExpression);

                if (entitycollection != null)
                {
                    if (entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities[0].Attributes.Contains("msemr_observationid"))
                        {
                            contact["msemr_observationid"] = new Guid(entitycollection.Entities[0].Attributes["msemr_observationid"].ToString());
                        }
                        entityRepository.UpdateEntity(contact);
                        contact.Id = new Guid(contact["msemr_observationid"].ToString());
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

        public async Task<List<PatientObservation>> getPatientObservationCRM(string patientGuid, DateTime startdate, DateTime enddate)
        {
            try
            {
                List<PatientObservation> list = new List<PatientObservation>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression("msemr_observation");

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
                        PatientObservation obj = new PatientObservation();
                        obj = getPatientObservationModelFilled(entitycollection[i], obj, "");
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

        public static PatientObservation getPatientObservationModelFilled(Entity entity, PatientObservation obs, string accountAlias)
        {
            if (entity.Attributes.Contains("msemr_description"))
            {
                obs.Description = (entity.Attributes["msemr_description"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_observationid"))
            {
                obs.ObservationID = (entity.Attributes["msemr_observationid"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_subjecttypepatient"))
            {
                obs.PatientID = new Guid((entity.Attributes["msemr_subjecttypepatient"] as EntityReference).Id.ToString());
            }

            if (entity.Attributes.Contains("createdon"))
            {
                obs.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);
            }

            if (entity.Attributes.Contains("msemr_effectivestart"))
            {
                obs.IssuedDate = Convert.ToDateTime(entity.Attributes["msemr_effectivestart"]);
            }

            return obs;
        }

    }
}
