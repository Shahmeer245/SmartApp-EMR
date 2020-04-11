using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;
using MazikLogger;
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
    public class PatientAppointment
    {
        public string AppointmentID { get; set; }
        public Guid PatientID { get; set; }
        public string Externalemrid { get; set; }
        public DateTime RecordedDate { get; set; }
        public string Title { get; set; }


        public async Task<Guid> CreatePatientAppointmentCRM(PatientAppointment condition)
        {
            try
            {

                Entity contact = new Entity("msemr_appointmentemr");

                if (condition.Externalemrid != "")
                {
                    contact["mzk_externalemrid"] = condition.Externalemrid;
                }
                if (condition.PatientID != Guid.Empty)
                {
                    contact["msemr_actorpatient"] = new EntityReference("contact", condition.PatientID);
                }
                if (condition.Title != "")
                {
                    contact["subject"] = condition.Title;
                }
                if (condition.RecordedDate != null)
                {
                    contact["msemr_starttime"] = Convert.ToDateTime(condition.RecordedDate);
                }

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();


                QueryExpression queryExpression = new QueryExpression("msemr_appointmentemr");
                queryExpression.Criteria.AddCondition("mzk_externalemrid", ConditionOperator.Equal, condition.Externalemrid);

                queryExpression.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryExpression);

                if (entitycollection != null)
                {
                    if (entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities[0].Attributes.Contains("msemr_appointmentemrid"))
                        {
                            contact["msemr_appointmentemrid"] = new Guid(entitycollection.Entities[0].Attributes["msemr_appointmentemrid"].ToString());
                        }
                        entityRepository.UpdateEntity(contact);
                        contact.Id = new Guid(contact["msemr_appointmentemrid"].ToString());
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

        public async Task<List<PatientAppointment>> getPatientDeviceCRM(string patientGuid, DateTime startdate, DateTime enddate)
        {
            try
            {
                List<PatientAppointment> list = new List<PatientAppointment>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression("msemr_appointmentemr");

                if (string.IsNullOrEmpty(patientGuid))
                {
                    throw new ValidationException("Patient Id must be specified");
                }

                query.Criteria.AddCondition("msemr_actorpatient", ConditionOperator.Equal, new Guid(patientGuid));
                //query.Criteria.AddCondition("msemr_planstartdate", ConditionOperator.GreaterEqual, Convert.ToDateTime(startdate));
                //query.Criteria.AddCondition("msemr_planenddate", ConditionOperator.LessEqual, Convert.ToDateTime(enddate));

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);


                if (entitycollection != null)
                {
                    for (int i = 0; i < entitycollection.Entities.Count; i++)
                    {
                        PatientAppointment obj = new PatientAppointment();
                        obj = getPatientAppointmentModelFilled(entitycollection[i], obj, "");
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

        public static PatientAppointment getPatientAppointmentModelFilled(Entity entity, PatientAppointment obs, string accountAlias)
        {
            if (entity.Attributes.Contains("subject"))
            {
                obs.Title = (entity.Attributes["subject"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_appointmentemrid"))
            {
                obs.AppointmentID = (entity.Attributes["msemr_appointmentemrid"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_actorpatient"))
            {
                obs.PatientID = new Guid((entity.Attributes["msemr_actorpatient"] as EntityReference).Id.ToString());
            }

            if (entity.Attributes.Contains("msemr_starttime"))
            {
                obs.RecordedDate = Convert.ToDateTime(entity.Attributes["msemr_starttime"]);
            }

            return obs;
        }
    }
}
