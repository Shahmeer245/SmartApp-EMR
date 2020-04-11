using MazikCareService.AXRepository;
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
    public class PatientDisposition :IPatientDisposition
    {
        public string   Id                      { get; set; }
        public string   Asneeded                { get; set; }
        public string   FollowUpText            { get; set; }
        public string   Comments                { get; set; }
        public string   DescriptionofSickNotes  { get; set; }
        public string   EncounterId             { get; set; }
        public string   OutComeName             { get; set; }
        public string   Notes                   { get; set; }
        public int      FollowUp                { get; set; }
        public int      OutComeValue            { get; set; }
        public int      FollowUpNumber          { get; set; }
        public DateTime SickStartDate           { get; set; }
        public DateTime SickEndDate             { get; set; }
        public DateTime CreatedOn               { get; set; }
        public string PartnerHospitalId { get; set; }
        public string PartnerHospitalName { get; set; }

        public async Task<List<PatientDisposition>> getPatientDisposition(string patientEncounter, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate)
        {
            List<PatientDisposition> PatientDisposition = new List<PatientDisposition>();
            mzk_patientorder patientOrderEntity = new mzk_patientorder();
            #region Patient Procedure Query
            QueryExpression query = new QueryExpression(mzk_disposition.EntityLogicalName);
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);

            if (patientEncounter != string.Empty)
             childFilter.AddCondition("mzk_encounterid", ConditionOperator.Equal, new Guid(patientEncounter));

            //Search Filter
            if (!string.IsNullOrEmpty(SearchFilters))
            {
                if (SearchFilters == Convert.ToString(mzk_dispositionfilter.InConsultation))
                    childFilter.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_dispositionmzk_Status.InConsultation));
                if (SearchFilters == Convert.ToString(mzk_dispositionfilter.ConsultationComplete))
                    childFilter.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_dispositionmzk_Status.ConsultationComplete));
                if (SearchFilters == Convert.ToString(mzk_dispositionfilter.EncounterSignedOff))
                    childFilter.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_dispositionmzk_Status.EncounterSignedOff));
            }

            //Search Order
            if (!string.IsNullOrEmpty(searchOrder))
                // childFilter.AddCondition("mzk_productidname", ConditionOperator.Like, ("%" + searchOrder + "%"));

                //Search Date
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                childFilter.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });

            //Patient Order Type :: Disposition
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_followup",
                                                                    "mzk_comments", "mzk_asneeded",
                                                                    "mzk_descriptionofsicknotes",
                                                                    "mzk_sickstartdate",
                                                                    "mzk_sickenddate", "createdon", "modifiedon", "mzk_followupnumber","mzk_outcome","mzk_notes", "mzk_partnerhospitalid");

            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                if (Convert.ToDateTime(entity.Attributes["createdon"]) != Convert.ToDateTime(entity.Attributes["modifiedon"]))
                {
                    PatientDisposition model = new PatientDisposition();

                    if (entity.Attributes.Contains("mzk_asneeded") && Convert.ToBoolean(entity.Attributes["mzk_asneeded"]) == true)
                        model.Asneeded = "1";
                    else
                        model.Asneeded = "0";

                    if (entity.Attributes.Contains("mzk_followup"))
                    {
                        model.FollowUp = ((OptionSetValue)entity.Attributes["mzk_followup"]).Value;
                        model.FollowUpText = entity.FormattedValues["mzk_followup"].ToString();
                    }

                    if (entity.Attributes.Contains("mzk_followupnumber"))
                    {
                        model.FollowUpNumber = ((OptionSetValue)entity.Attributes["mzk_followupnumber"]).Value;
                    }

                    if (entity.Attributes.Contains("mzk_comments"))
                        model.Comments = entity.Attributes["mzk_comments"].ToString();

                    if (entity.Attributes.Contains("mzk_descriptionofsicknotes"))
                        model.DescriptionofSickNotes = entity.Attributes["mzk_descriptionofsicknotes"].ToString();

                    if (entity.Attributes.Contains("mzk_sickstartdate"))
                        model.SickStartDate = Convert.ToDateTime(entity.Attributes["mzk_sickstartdate"]);


                    if (entity.Attributes.Contains("mzk_sickenddate"))
                        model.SickEndDate = Convert.ToDateTime(entity.Attributes["mzk_sickenddate"]);

                    if (entity.Attributes.Contains("mzk_dispositionid"))
                        model.Id = entity.Id.ToString();

                    if (entity.Attributes.Contains("createdon"))
                        model.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);

                    if (entity.Attributes.Contains("mzk_outcome"))
                    {
                        model.OutComeValue = ((OptionSetValue)entity.Attributes["mzk_outcome"]).Value;
                        model.OutComeName = entity.FormattedValues["mzk_outcome"].ToString();
                    }

                    if (entity.Attributes.Contains("mzk_partnerhospitalid"))
                    {
                        model.PartnerHospitalId = ((EntityReference)entity["mzk_partnerhospitalid"]).Id.ToString();
                        model.PartnerHospitalName = ((EntityReference)entity["mzk_partnerhospitalid"]).Name.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_notes"))
                        model.Notes = entity.Attributes["mzk_notes"].ToString();

                    PatientDisposition.Add(model);
                }
            }

            return PatientDisposition;
        }
        public async Task<string> AddPatientDisposition(PatientDisposition patientDisposition)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            xrm.mzk_disposition patientDispositionEntity = new xrm.mzk_disposition();

            if (patientDisposition.Asneeded == "1")
                patientDispositionEntity.mzk_Asneeded = true; 

            if (patientDisposition.FollowUp != 0)
                patientDispositionEntity.mzk_FollowUp = new OptionSetValue( patientDisposition.FollowUp);
            if (!string.IsNullOrEmpty(patientDisposition.Comments))
                patientDispositionEntity.mzk_Comments = patientDisposition.Comments;
            if (!string.IsNullOrEmpty(patientDisposition.DescriptionofSickNotes))
                patientDispositionEntity.mzk_DescriptionofSickNotes = patientDisposition.DescriptionofSickNotes;

            if (!string.IsNullOrEmpty(patientDisposition.EncounterId))
            {
                patientDispositionEntity.mzk_EncounterId = new EntityReference(xrm.mzk_patientencounter.EntityLogicalName, new Guid(patientDisposition.EncounterId));

                PatientEncounter encounter = new PatientEncounter();
                encounter.EncounterId = patientDisposition.EncounterId;
                patientDispositionEntity.mzk_customer = new EntityReference("contact", new Guid(encounter.getEncounterDetails(encounter).Result.ToList().First<PatientEncounter>().PatientId));
            }

            if (patientDisposition.SickStartDate != DateTime.MinValue)
                patientDispositionEntity.mzk_SickStartDate = patientDisposition.SickStartDate;

            if (patientDisposition.SickEndDate != DateTime.MinValue)
                patientDispositionEntity.mzk_SickEndDate = patientDisposition.SickEndDate;

            if (!string.IsNullOrEmpty(patientDisposition.Notes))
                patientDispositionEntity.mzk_Notes = patientDisposition.Notes;

            if (patientDisposition.OutComeValue !=0)
                patientDispositionEntity.mzk_Outcome =new OptionSetValue(patientDisposition.OutComeValue);

            if (patientDisposition.PartnerHospitalId != null && patientDisposition.PartnerHospitalId != string.Empty && patientDisposition.PartnerHospitalId != "0")
                patientDispositionEntity.Attributes["mzk_partnerhospitalid"] = new EntityReference("mzk_hospital", new Guid(patientDisposition.PartnerHospitalId));
            Id = Convert.ToString(entityRepository.CreateEntity(patientDispositionEntity));

            return Id.ToString();
        }
        public async Task<bool> updatePatientDisposition(PatientDisposition patientDisposition)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_disposition.EntityLogicalName);
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                query.Criteria.AddCondition("mzk_encounterid", ConditionOperator.Equal, patientDisposition.EncounterId);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                if (entitycollection.Entities.Count > 0)
                {
                    Entity encounterEntity = entitycollection.Entities[0];

                    if (patientDisposition.Asneeded == "1")
                        encounterEntity.Attributes["mzk_asneeded"] = true;
                    else
                        encounterEntity.Attributes["mzk_asneeded"] = false;

                    mzk_dispositionmzk_FollowUp followUp = 0;

                    if (patientDisposition.FollowUp != 0)
                    {
                        encounterEntity.Attributes["mzk_followup"] = new OptionSetValue(patientDisposition.FollowUp);
                        followUp = (mzk_dispositionmzk_FollowUp)Convert.ToInt32(patientDisposition.FollowUp);
                    }

                    if (!string.IsNullOrEmpty(patientDisposition.Notes))
                        encounterEntity.Attributes["mzk_notes"] = patientDisposition.Notes;

                    if (patientDisposition.FollowUpNumber != 0)
                        encounterEntity.Attributes["mzk_followupnumber"] = new OptionSetValue(patientDisposition.FollowUpNumber);

                    if (!string.IsNullOrEmpty(patientDisposition.Comments))
                        encounterEntity.Attributes["mzk_comments"] = patientDisposition.Comments;
                    if (!string.IsNullOrEmpty(patientDisposition.DescriptionofSickNotes))
                        encounterEntity.Attributes["mzk_descriptionofsicknotes"] = patientDisposition.DescriptionofSickNotes;


                    if (patientDisposition.SickStartDate != DateTime.MinValue)
                        encounterEntity.Attributes["mzk_sickstartdate"] = patientDisposition.SickStartDate;
                    else
                        encounterEntity.Attributes["mzk_sickstartdate"] = null;

                    if (patientDisposition.SickEndDate != DateTime.MinValue)
                        encounterEntity.Attributes["mzk_sickenddate"] = patientDisposition.SickEndDate;
                    else
                        encounterEntity.Attributes["mzk_sickenddate"] = null;

                    if (patientDisposition.OutComeValue != 0)
                        encounterEntity.Attributes["mzk_outcome"] = new OptionSetValue(patientDisposition.OutComeValue);

                    if (patientDisposition.PartnerHospitalId != null && patientDisposition.PartnerHospitalId != string.Empty && patientDisposition.PartnerHospitalId!="0")
                        encounterEntity.Attributes["mzk_partnerhospitalid"] = new EntityReference("mzk_hospital", new Guid(patientDisposition.PartnerHospitalId));

                    entityRepository.UpdateEntity(encounterEntity);

                    AppointmentRepository appRepo = new AppointmentRepository();
                    PatientEncounter encounter = new PatientEncounter();
                    encounter.EncounterId = patientDisposition.EncounterId;
                    long appointmentrecid = new PatientEncounter().getEncounterDetails(encounter).Result.ToList().First<PatientEncounter>().AppointmentRefRecId;

                    if (appointmentrecid > 0)
                    {
                        appRepo.updateDispositionDetails(appointmentrecid, string.IsNullOrEmpty(patientDisposition.Comments) ? string.Empty : patientDisposition.Comments, patientDisposition.FollowUpNumber.ToString() + " " + followUp.ToString());
                    }
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
