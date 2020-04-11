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
    public class PatientDevice
    {
        public string DeviceID { get; set; }
        public Guid PatientID { get; set; }
        public string Externalemrid { get; set; }
        public DateTime RecordedDate { get; set; }
        public string Title { get; set; }


        public async Task<Guid> CreatePatientDeviceCRM(PatientDevice condition)
        {
            try
            {

                Entity contact = new Entity("msemr_device");

                if (condition.Externalemrid != "")
                {
                    contact["mzk_externalemrid"] = condition.Externalemrid;
                }
                if (condition.PatientID != Guid.Empty)
                {
                    contact["msemr_patient"] = new EntityReference("contact", condition.PatientID);
                }
                if (condition.Title != "")
                {
                    contact["msemr_name"] = condition.Title;
                }
                if (condition.RecordedDate != null)
                {
                    contact["msemr_manufacturerdate"] = Convert.ToDateTime(condition.RecordedDate);
                }

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();


                QueryExpression queryExpression = new QueryExpression("msemr_device");
                queryExpression.Criteria.AddCondition("mzk_externalemrid", ConditionOperator.Equal, condition.Externalemrid);

                queryExpression.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryExpression);

                if (entitycollection != null)
                {
                    if (entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities[0].Attributes.Contains("msemr_deviceid"))
                        {
                            contact["msemr_deviceid"] = new Guid(entitycollection.Entities[0].Attributes["msemr_deviceid"].ToString());
                        }
                        entityRepository.UpdateEntity(contact);
                        contact.Id = new Guid(contact["msemr_deviceid"].ToString());
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

        public async Task<List<PatientDevice>> getPatientDeviceCRM(string patientGuid, DateTime startdate, DateTime enddate)
        {
            try
            {
                List<PatientDevice> list = new List<PatientDevice>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression("msemr_device");

                if (string.IsNullOrEmpty(patientGuid))
                {
                    throw new ValidationException("Patient Id must be specified");
                }

                query.Criteria.AddCondition("msemr_patient", ConditionOperator.Equal, new Guid(patientGuid));
                //query.Criteria.AddCondition("msemr_planstartdate", ConditionOperator.GreaterEqual, Convert.ToDateTime(startdate));
                //query.Criteria.AddCondition("msemr_planenddate", ConditionOperator.LessEqual, Convert.ToDateTime(enddate));

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);


                if (entitycollection != null)
                {
                    for (int i = 0; i < entitycollection.Entities.Count; i++)
                    {
                        PatientDevice obj = new PatientDevice();
                        obj = getPatientDeviceModelFilled(entitycollection[i], obj, "");
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

        public static PatientDevice getPatientDeviceModelFilled(Entity entity, PatientDevice obs, string accountAlias)
        {
            if (entity.Attributes.Contains("msemr_name"))
            {
                obs.Title = (entity.Attributes["msemr_name"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_deviceid"))
            {
                obs.DeviceID = (entity.Attributes["msemr_deviceid"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_patient"))
            {
                obs.PatientID = new Guid((entity.Attributes["msemr_patient"] as EntityReference).Id.ToString());
            }

            if (entity.Attributes.Contains("msemr_manufacturerdate"))
            {
                obs.RecordedDate = Convert.ToDateTime(entity.Attributes["msemr_manufacturerdate"]);
            }

            return obs;
        }

    }
}
