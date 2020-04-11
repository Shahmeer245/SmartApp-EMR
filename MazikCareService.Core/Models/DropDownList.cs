using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Models;
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
    public class DropDownList
    {
        public string Type { get; set; }
        public List<DropDown> DropDownValues { get; set; }
        public async Task<List<DropDownList>> getDropDownList(string entityName, string encounterId, string SpecialityId = null, string Dosage = null, string patientId = null, List<string> dropDownName = null)
        {
            List<DropDownList> listValues = new List<DropDownList>();
            try
            {
                switch (entityName)
                {
                    case "templateDesigner":

                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "templateResponseType":
                                        listValues.Add(this.getOptionSetList("templateResponseType", "mzk_sonotesclinicaltemplate", "mzk_responsetype", "0"));
                                        break;
                                    case "gender":
                                        listValues.Add(this.getOptionSetList("gender", "mzk_sonotesclinicaltemplate", "mzk_gender", "0"));
                                        break;
                                    case "historyType":
                                        listValues.Add(this.getOptionSetList("historyType", "mzk_sonotesclinicaltemplatesection", "mzk_historytype", "0"));
                                        break;
                                    case "narrationResponseType":
                                        listValues.Add(this.getOptionSetList("narrationResponseType", "mzk_sonotesfindingnarration", "mzk_responsetype", "0"));
                                        break;
                                    case "narrationType":
                                        listValues.Add(this.getOptionSetList("narrationType", "mzk_sonotessectionfinding", "mzk_type", "0"));
                                        break;
                                    case "ageGroup":
                                        listValues.Add(this.getAgeGroupList("ageGroup"));
                                        break;
                                    case "speciality":
                                        listValues.Add(this.getSpecialityList("speciality", mzk_locationtype.Internal));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getOptionSetList("templateResponseType", "mzk_sonotesclinicaltemplate", "mzk_responsetype", "0"));
                            listValues.Add(this.getOptionSetList("gender", "mzk_sonotesclinicaltemplate", "mzk_gender", "0"));
                            listValues.Add(this.getOptionSetList("historyType", "mzk_sonotesclinicaltemplatesection", "mzk_historytype", "0"));
                            listValues.Add(this.getOptionSetList("narrationResponseType", "mzk_sonotesfindingnarration", "mzk_responsetype", "0"));
                            listValues.Add(this.getOptionSetList("narrationType", "mzk_sonotessectionfinding", "mzk_type", "0"));
                            listValues.Add(this.getAgeGroupList("ageGroup"));
                            listValues.Add(this.getSpecialityList("speciality", mzk_locationtype.Internal));
                        }
                        break;
                    case "statusManagerDesigner":

                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "caseType":
                                        listValues.Add(this.getOptionSetList("caseType", "", "mzk_casetype", "0"));
                                        break;
                                    case "entityType":
                                        listValues.Add(this.getOptionSetList("entityType", "", "mzk_entitytype", "0"));
                                        break;
                                    case "actionManager":
                                        listValues.Add(this.getActionManagerList("actionManager"));
                                        break;
                                    case "orderStatus":
                                        listValues.Add(this.getOptionSetList("orderStatus", "", "mzk_orderstatus", "0"));
                                        break;
                                    case "flipType":
                                        listValues.Add(this.getOptionSetList("flipType", "", "mzk_fliptype", "0"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getOptionSetList("caseType", "", "mzk_casetype", "0"));
                            listValues.Add(this.getOptionSetList("entityType", "", "mzk_entitytype", "0"));
                            listValues.Add(this.getActionManagerList("actionManager"));
                            listValues.Add(this.getOptionSetList("orderStatus", "", "mzk_orderstatus", "0"));
                            listValues.Add(this.getOptionSetList("flipType", "", "mzk_fliptype", "0"));
                        }
                        break;

                    case "agevalidation":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_agevalidation", "mzk_agegroup", "0"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_agevalidation", "mzk_agegroup", "0"));
                        }
                        break;
                    case "Relationships":
                        listValues.Add(this.getRelationshipList("Relationships"));
                        break;
                    case "ReasonCode":
                        listValues.Add(this.getReasonCodeList("Delivery Cancel Reason"));
                        listValues.Add(this.getMedicationSkipReasonList("Medication Skip Reason"));
                        break;
                    case "Contact":
                        listValues.Add(this.getOptionSetList("GenderCode", "contact", "gendercode", "0"));
                        break;
                    case "Concept":
                        listValues.Add(this.getConceptList("Allergies","1"));
                        listValues.Add(this.getConceptList("AllergyReaction", "2"));
                        listValues.Add(this.getOptionSetList("Allergy Status", "mzk_patientallergies", "mzk_status", "1"));
                        break;
                    case "Problem":
                        listValues.Add(this.getProblemList("Problem", "3"));
                        listValues.Add(this.getOptionSetList("Problem Type", "mzk_patientproblem", "mzk_problemtype", "1"));
                        listValues.Add(this.getOptionSetList("Problem Status", "mzk_patientproblem", "mzk_status", "1"));
                        break;
                    case "orderLab":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "Diagnosis":
                                        if (string.IsNullOrEmpty(encounterId))
                                            listValues.Add(this.getConceptList("Diagnosis", "3"));
                                        else
                                            listValues.Add(this.getEncounterDiagnosisList("Diagnosis", encounterId));
                                        break;
                                    case "Urgency":
                                        listValues.Add(this.getOptionSetList("Urgency", "mzk_patientorder", "mzk_urgency", "0"));
                                        break;
                                    case "Frequency":
                                        listValues.Add(this.getOrderSetupList("Frequency", "1", Dosage));
                                        break;
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_labfilter", "0"));
                                        break;
                                    case "SpecimenSource":
                                        listValues.Add(this.getOrderSetupList("SpecimenSource", "3", Dosage));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(encounterId))
                                listValues.Add(this.getConceptList("Diagnosis", "3"));
                            else
                                listValues.Add(this.getEncounterDiagnosisList("Diagnosis", encounterId));
                            listValues.Add(this.getOptionSetList("Urgency", "mzk_patientorder", "mzk_urgency", "0"));
                            listValues.Add(this.getOrderSetupList("Frequency", "1", Dosage));
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_labfilter", "0"));
                            listValues.Add(this.getOrderSetupList("SpecimenSource", "3", Dosage));

                        }
                        break;
                    case "orderRadiology":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "Diagnosis":
                                        if (string.IsNullOrEmpty(encounterId))
                                            listValues.Add(this.getConceptList("Diagnosis", "3"));
                                        else
                                            listValues.Add(this.getEncounterDiagnosisList("Diagnosis", encounterId));
                                        break;
                                    case "Frequency":
                                        listValues.Add(this.getOrderSetupList("Frequency", "1", Dosage));
                                        break;
                                    case "StudyDate":
                                        listValues.Add(this.getOptionSetList("StudyDate", "mzk_patientorder", "mzk_studydate", "2"));
                                        break;
                                    case "Location":
                                        listValues.Add(this.getLocationList("Location", 3));
                                        break;
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_radiologyfilter", "0"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(encounterId))
                                listValues.Add(this.getConceptList("Diagnosis", "3"));
                            else
                                listValues.Add(this.getEncounterDiagnosisList("Diagnosis", encounterId));
                            listValues.Add(this.getOrderSetupList("Frequency", "1", Dosage));
                            listValues.Add(this.getOptionSetList("StudyDate", "mzk_patientorder", "mzk_studydate", "2"));
                            listValues.Add(this.getLocationList("Location", 3));
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_radiologyfilter", "0"));
                        }
                        break;

                    case "specialTest":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "Diagnosis":
                                        if (string.IsNullOrEmpty(encounterId))
                                            listValues.Add(this.getConceptList("Diagnosis", "3"));
                                        else
                                            listValues.Add(this.getEncounterDiagnosisList("Diagnosis", encounterId));
                                        break;
                                    case "Frequency":
                                        listValues.Add(this.getOrderSetupList("Frequency", "1", Dosage));
                                        break;
                                    case "Urgency":
                                        listValues.Add(this.getOptionSetList("Urgency", "mzk_patientorder", "mzk_urgency", "1"));
                                        break;
                                    case "Location":
                                        listValues.Add(this.getLocationList("Location", 5));
                                        break;
                                    case "OrderStatus":
                                        listValues.Add(this.getOptionSetList("OrderStatus", "mzk_patientorder", "mzk_orderstatus", "1"));
                                        break;
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_specialtestfilter", "0"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(encounterId))
                                listValues.Add(this.getConceptList("Diagnosis", "3"));
                            else
                                listValues.Add(this.getEncounterDiagnosisList("Diagnosis", encounterId));
                            listValues.Add(this.getOrderSetupList("Frequency", "1", Dosage));
                            listValues.Add(this.getOptionSetList("Urgency", "mzk_patientorder", "mzk_urgency", "1"));
                            listValues.Add(this.getLocationList("Location", 5));
                            listValues.Add(this.getOptionSetList("OrderStatus", "mzk_patientorder", "mzk_orderstatus", "1"));
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_specialtestfilter", "0"));
                        }

                        break;
                    case "medication":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "Diagnosis":
                                        if (string.IsNullOrEmpty(encounterId))
                                            listValues.Add(this.getConceptList("Diagnosis", "3"));
                                        else
                                            listValues.Add(this.getEncounterDiagnosisList("Diagnosis", encounterId));
                                        break;
                                    case "Frequency":
                                        listValues.Add(this.getOrderSetupList("Frequency", "1", Dosage));
                                        break;
                                    case "Route":
                                        listValues.Add(this.getOrderSetupList("Route", "2", Dosage));
                                        break;
                                    case "Urgency":
                                        listValues.Add(this.getOptionSetList("Urgency", "mzk_patientorder", "mzk_urgency", "1"));
                                        break;
                                    case "Type":
                                        listValues.Add(this.getOptionSetList("Type", "mzk_patientorder", "mzk_medicationordertype", "1"));
                                        break;
                                    case "PRN_Indication":
                                        listValues.Add(this.getOptionSetList("PRN_Indication", "mzk_patientorder", "mzk_prnindication", "1"));
                                        break;
                                    case "Duration":
                                        listValues.Add(this.getOptionSetList("Duration", "mzk_dose", "mzk_duration", "1"));
                                        break;
                                    case "Dose":
                                        listValues.Add(this.getOptionSetList("Dose", "mzk_dose", "mzk_dose", "1"));
                                        break;
                                    case "Unit":
                                        listValues.Add(this.getUnitList("Unit", "1", Dosage));
                                        break;
                                    case "Location":
                                        listValues.Add(this.getLocationList("Location", 5));
                                        break;
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_medicationfilter", "1"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(encounterId))
                                listValues.Add(this.getConceptList("Diagnosis", "3"));
                            else
                                listValues.Add(this.getEncounterDiagnosisList("Diagnosis", encounterId));

                            listValues.Add(this.getOrderSetupList("Frequency", "1", Dosage));
                            listValues.Add(this.getOrderSetupList("Route", "2", Dosage));
                            listValues.Add(this.getOptionSetList("Urgency", "mzk_patientorder", "mzk_urgency", "1"));
                            listValues.Add(this.getOptionSetList("Type", "mzk_patientorder", "mzk_medicationordertype", "1"));
                            listValues.Add(this.getOptionSetList("PRN_Indication", "mzk_patientorder", "mzk_prnindication", "1"));
                            listValues.Add(this.getOptionSetList("Duration", "mzk_dose", "mzk_duration", "1"));
                            listValues.Add(this.getOptionSetList("Dose", "mzk_dose", "mzk_dose", "1"));
                            listValues.Add(this.getUnitList("Unit", "1", Dosage));
                            listValues.Add(this.getLocationList("Location", 5));
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_medicationfilter", "1"));
                        }
                        break;

                    case "signoff":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "EncounterStatus":
                                        listValues.Add(this.getOptionSetList("EncounterStatus", "mzk_patientencounter", "mzk_encounterstatus", "1"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getOptionSetList("EncounterStatus", "mzk_patientencounter", "mzk_encounterstatus", "1"));
                        }
                        break;

                    case "Allergy":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "AllergyReaction":
                                        listValues.Add(this.getConceptList("AllergyReaction", "2"));
                                        break;
                                    case "Allergy":
                                        listValues.Add(this.getOptionSetList("Allergy Status", "mzk_patientallergies", "mzk_status", "1"));
                                        break;
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientallergies", "mzk_allergyfilter", "1"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getConceptList("AllergyReaction", "2"));
                            listValues.Add(this.getOptionSetList("Allergy Status", "mzk_patientallergies", "mzk_status", "1"));
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientallergies", "mzk_allergyfilter", "1"));
                        }
                        break;

                    case "referral":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "ReferralCategoty":
                                        listValues.Add(this.getOptionSetList("ReferralCategoty", "mzk_patientorder", "mzk_referralcategory", "1"));
                                        break;
                                    case "Referral Type":
                                        listValues.Add(this.getOptionSetList("Referral Type", "mzk_patientorder", "mzk_referraltype", "1"));
                                        break;
                                    case "ReferralTo":
                                        listValues.Add(this.getUserList("ReferralTo", SpecialityId));
                                        break;
                                    case "ReferralToExternally":
                                        listValues.Add(this.getReferringPhysicianList("ReferralToExternally", SpecialityId));
                                        break;
                                    case "AppointmentRecommendation":
                                        listValues.Add(this.getOptionSetList("AppointmentRecommendation", "mzk_patientorder", "mzk_appointmentrecommendation", "1"));
                                        break;
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_referralsfilter", "0"));
                                        break;
                                    case "PartnerHospital":
                                        listValues.Add(this.getPartnerHospital("PartnerHospital"));
                                        break;
                                    case "Diagnosis":
                                        if (string.IsNullOrEmpty(encounterId))
                                            listValues.Add(this.getConceptList("Diagnosis", "3"));
                                        else
                                            listValues.Add(this.getEncounterDiagnosisList("Diagnosis", encounterId));
                                        break;
                                }
                            }
                        }
                        else
                        {

                            listValues.Add(this.getOptionSetList("ReferralCategoty", "mzk_patientorder", "mzk_referralcategory", "1"));
                            listValues.Add(this.getOptionSetList("Referral Type", "mzk_patientorder", "mzk_referraltype", "1"));

                            //Speciality
                            // listValues.Add(this.getSpecialityList("Speciality"));                       
                            //Internally
                            listValues.Add(this.getUserList("ReferralTo", SpecialityId));
                            //Externally
                            listValues.Add(this.getReferringPhysicianList("ReferralToExternally", SpecialityId));
                            //Appointment Recommendation
                            listValues.Add(this.getOptionSetList("AppointmentRecommendation", "mzk_patientorder", "mzk_appointmentrecommendation", "1"));
                            //SearchFilters
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_referralsfilter", "0"));
                            listValues.Add(this.getPartnerHospital("PartnerHospital"));

                            //Diagnosis
                            if (string.IsNullOrEmpty(encounterId))
                                listValues.Add(this.getConceptList("Diagnosis", "3"));
                            else
                                listValues.Add(this.getEncounterDiagnosisList("Diagnosis", encounterId));

                        }
                        break;
                    case "problem":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "Diagnosisstatus":
                                        listValues.Add(this.getOptionSetList("Diagnosisstatus", "mzk_patientproblem", "mzk_status", "1"));
                                        break;
                                    case "ProblemType":
                                        listValues.Add(this.getOptionSetList("ProblemType", "mzk_patientproblem", "mzk_problemtype", "1"));
                                        break;
                                    case "Chronicity":
                                        listValues.Add(this.getOptionSetList("Chronicity", "mzk_patientproblem", "mzk_chronicity", "1"));
                                        break;
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientproblem", "mzk_problemfilter", "0"));
                                        break;
                                }
                            }
                        }
                        else
                        {

                            listValues.Add(this.getOptionSetList("Diagnosisstatus", "mzk_patientproblem", "mzk_status", "1"));
                            listValues.Add(this.getOptionSetList("ProblemType", "mzk_patientproblem", "mzk_problemtype", "1"));
                            listValues.Add(this.getOptionSetList("Chronicity", "mzk_patientproblem", "mzk_chronicity", "1"));
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientproblem", "mzk_problemfilter", "0"));
                        }
                        break;
                    case "diagnosis":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "Diagnosisstatus":
                                        listValues.Add(this.getOptionSetList("Diagnosisstatus", "mzk_patientencounterdiagnosis", "mzk_status", "1"));
                                        break;
                                    case "ProblemType":
                                        listValues.Add(this.getOptionSetList("ProblemType", "mzk_patientencounterdiagnosis", "mzk_problemtype", "1"));
                                        break;
                                    case "Chronicity":
                                        listValues.Add(this.getOptionSetList("Chronicity", "mzk_patientencounterdiagnosis", "mzk_chronicity", "1"));
                                        break;
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientencounterdiagnosis", "mzk_diagnosisfilter", "0"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getOptionSetList("Diagnosisstatus", "mzk_patientencounterdiagnosis", "mzk_status", "1"));
                            listValues.Add(this.getOptionSetList("ProblemType", "mzk_patientencounterdiagnosis", "mzk_problemtype", "1"));
                            listValues.Add(this.getOptionSetList("Chronicity", "mzk_patientencounterdiagnosis", "mzk_chronicity", "1"));
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientencounterdiagnosis", "mzk_diagnosisfilter", "0"));
                        }
                        break;
                    case "procedure":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "ReferralTo":
                                        listValues.Add(this.getUserList("ReferralTo"));
                                        break;
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_procedurefilter", "0"));
                                        break;
                                    case "Location":
                                        listValues.Add(this.getLocationList("Location", 5));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getUserList("ReferralTo"));
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientorder", "mzk_procedurefilter", "0"));
                            listValues.Add(this.getLocationList("Location", 5));
                        }
                        break;
                    case "disposition":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "FollowUp":
                                        listValues.Add(this.getOptionSetList("FollowUp", "mzk_disposition", "mzk_followup", "0"));
                                        break;
                                    case "FollowUpNumber":
                                        listValues.Add(this.getOptionSetList("FollowUpNumber", "mzk_disposition", "mzk_followupnumber", "0"));
                                        break;
                                    case "Outcome":
                                        listValues.Add(this.getOptionSetList("Outcome", "mzk_disposition", "mzk_outcome", "0"));
                                        break;
                                    case "PartnerHospital":
                                        listValues.Add(this.getPartnerHospital("PartnerHospital"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getOptionSetList("FollowUp", "mzk_disposition", "mzk_followup", "0"));
                            listValues.Add(this.getOptionSetList("FollowUpNumber", "mzk_disposition", "mzk_followupnumber", "0"));
                            listValues.Add(this.getOptionSetList("Outcome", "mzk_disposition", "mzk_outcome", "0"));
                            listValues.Add(this.getPartnerHospital("PartnerHospital"));
                        }
                        break;
                    case "therapy":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "Frequency":
                                        listValues.Add(this.getOrderSetupList("Frequency", "1", Dosage));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getOrderSetupList("Frequency", "1", Dosage));
                        }
                        break;

                    case "quickRegistration":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "Gender":
                                        listValues.Add(this.getAXDropDown("Gender"));
                                        break;
                                    case "NationalIdType":
                                        listValues.Add(this.getAXDropDown("NationalIdType"));
                                        break;
                                    case "SearchPatientFilter":
                                        listValues.Add(this.getAXDropDown("SearchPatientFilter"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getAXDropDown("Gender"));
                            listValues.Add(this.getAXDropDown("NationalIdType"));
                            listValues.Add(this.getAXDropDown("SearchPatientFilter"));
                        }
                        break;

                    case "pastEncounter":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int i = 0; i < dropDownName.Count; i++)
                            {
                                switch (dropDownName[i])
                                {
                                    case "SearchFilters":
                                        listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientencounter", "mzk_encounterfilter", "0"));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            listValues.Add(this.getOptionSetList("SearchFilters", "mzk_patientencounter", "mzk_encounterfilter", "0"));
                        }
                        break;
                    case "medicalNotes":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int j = 0; j < dropDownName.Count; j++)
                            {
                                switch (dropDownName[j])
                                {
                                    case "SearchFilters":

                                        if (patientId != null)
                                        {

                                            Speciality speciality = new Speciality();
                                            List<string> specialityList = new List<string>();

                                            specialityList = speciality.getSpecialityList(patientId, (int)mzk_casetype.OutPatient, (int)mzk_encountertype.Consultation);

                                            if (specialityList.Count > 0)
                                            {
                                                listValues.Add(this.getSpecialityList("SearchFilters", mzk_locationtype.Internal, specialityList));
                                            }
                                        }

                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (patientId != null)
                            {
                                Speciality speciality = new Speciality();
                                List<string> specialityList = new List<string>();

                                specialityList = speciality.getSpecialityList(patientId, (int)mzk_casetype.OutPatient, (int)mzk_encountertype.Consultation);

                                if (specialityList.Count > 0)
                                {
                                    listValues.Add(this.getSpecialityList("SearchFilters", mzk_locationtype.Internal, specialityList));
                                }
                            }
                            break;
                        }
                        break;

                    case "clinicalNotes":
                        if (dropDownName != null && dropDownName.Count != 0)
                        {
                            for (int j = 0; j < dropDownName.Count; j++)
                            {
                                switch (dropDownName[j])
                                {
                                    case "SearchFilters":
                                        if (patientId != null)
                                        {
                                            Speciality speciality = new Speciality();
                                            List<string> specialityList = new List<string>();

                                            specialityList = speciality.getSpecialityList(patientId, (int)mzk_casetype.OutPatient, (int)mzk_encountertype.Consultation);

                                            if (specialityList.Count > 0)
                                            {
                                                listValues.Add(this.getSpecialityList("SearchFilters", mzk_locationtype.Internal, specialityList));
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (patientId != null)
                            {
                                Speciality speciality = new Speciality();
                                List<string> specialityList = new List<string>();

                                specialityList = speciality.getSpecialityList(patientId, (int)mzk_casetype.OutPatient, (int)mzk_encountertype.Consultation);

                                if (specialityList.Count > 0)
                                {
                                    listValues.Add(this.getSpecialityList("SearchFilters", mzk_locationtype.Internal, specialityList));
                                }
                            }
                            break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listValues;
        }

        public DropDownList getRelationshipList(string type)
        {
            try
            {
                DropDownList model = new DropDownList();
                QueryExpression query = new QueryExpression(xrm.mzk_masterdata.EntityLogicalName);
                query.Criteria.AddCondition("mzk_type", ConditionOperator.Equal, 19);
                query.ColumnSet = new ColumnSet("mzk_masterdataid", "mzk_code", "mzk_description");
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                model.Type = type;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (Entity entity in entitycollection.Entities)
                {
                    DropDown dropDown = new DropDown();
                    if (entity.Attributes.Contains("mzk_code"))
                        dropDown.text = entity["mzk_code"].ToString();
                    if(entity.Attributes.Contains("mzk_masterdataid"))
                        dropDown.value = entity["mzk_masterdataid"].ToString();
                    if(entity.Attributes.Contains("mzk_description"))
                        dropDown.IsDefault = entity["mzk_description"].ToString();
                    listDropDown.Add(dropDown);
                }
                model.DropDownValues = listDropDown;
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DropDownList getReasonCodeList(string type)
        {
            try
            {
                DropDownList model = new DropDownList();
                QueryExpression query = new QueryExpression(xrm.mzk_reasoncode.EntityLogicalName);
                query.Criteria.AddCondition("mzk_reasontype", ConditionOperator.Equal, 275380024);
                query.ColumnSet = new ColumnSet("mzk_reasoncodeid", "mzk_name");
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                model.Type = type;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (Entity entity in entitycollection.Entities)
                {
                    DropDown dropDown = new DropDown();
                    if (entity.Attributes.Contains("mzk_name"))
                        dropDown.text = entity["mzk_name"].ToString();
                    if (entity.Attributes.Contains("mzk_reasoncodeid"))
                        dropDown.value = entity["mzk_reasoncodeid"].ToString();
                    listDropDown.Add(dropDown);
                }
                model.DropDownValues = listDropDown;
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DropDownList getMedicationSkipReasonList(string type)
        {
            try
            {
                DropDownList model = new DropDownList();
                QueryExpression query = new QueryExpression(xrm.mzk_reasoncode.EntityLogicalName);
                query.Criteria.AddCondition("mzk_reasontype", ConditionOperator.Equal, 275380026);
                query.ColumnSet = new ColumnSet("mzk_reasoncodeid", "mzk_name");
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                model.Type = type;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (Entity entity in entitycollection.Entities)
                {
                    DropDown dropDown = new DropDown();
                    if (entity.Attributes.Contains("mzk_name"))
                        dropDown.text = entity["mzk_name"].ToString();
                    if (entity.Attributes.Contains("mzk_reasoncodeid"))
                        dropDown.value = entity["mzk_reasoncodeid"].ToString();
                    listDropDown.Add(dropDown);
                }
                model.DropDownValues = listDropDown;
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DropDownList getConceptList(string type, string typeValue)
        {
            DropDownList model = new DropDownList();

            try
            {
                Concept Concept = new Concept();

                List<Concept> listConcept = Concept.getConcept(typeValue);

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (Concept concept in listConcept)
                {
                    dropDown = new DropDown();

                    dropDown.text = concept.name;
                    dropDown.value = concept.conceptId;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getProblemList(string type, string typeValue)
        {
            DropDownList model = new DropDownList();

            try
            {
                Concept Concept = new Concept();

                List<Concept> listConcept = Concept.getConcept(typeValue);

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (Concept concept in listConcept)
                {
                    dropDown = new DropDown();

                    dropDown.text = concept.name;
                    dropDown.value = concept.conceptId;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getEncounterDiagnosisList(string type, string encounter)
        {
            DropDownList model = new DropDownList();

            try
            {
                PatientDiagnosis Diagnosis = new PatientDiagnosis();

                List<PatientDiagnosis> listDiagnosis = Diagnosis.getEncounterDiagnosis(encounter);

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (PatientDiagnosis diagnosis in listDiagnosis)
                {
                    dropDown = new DropDown();

                    dropDown.text = diagnosis.DiagnosisName;
                    dropDown.value = diagnosis.DiagnosisId;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getLocationList(string type, int typeValue)
        {
            DropDownList model = new DropDownList();
            try
            {

                Clinic modelObj = new Clinic();
                List<DropDown> listDropDown = new List<DropDown>();
                List<Clinic> modelList;
                DropDown dropDown;
                model.Type = type;

                modelList = modelObj.getClinicsList((mzk_orgunittype)typeValue);

                if (modelList != null)
                {
                    foreach (Clinic entity in modelList)
                    {
                        dropDown = new DropDown();

                        dropDown.text = entity.clinicName;
                        dropDown.value = entity.id;
                        listDropDown.Add(dropDown);
                    }

                    model.DropDownValues = listDropDown;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getOrderSetupList(string type, string typeValue, string Dosage)
        {
            DropDownList model = new DropDownList();

            try
            {
                OrderSetup ordersetup = new OrderSetup();
                List<OrderSetup> listConcept = ordersetup.getOrderSetup(typeValue, Dosage);

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (OrderSetup orderSetup in listConcept)
                {
                    dropDown = new DropDown();

                    dropDown.text = orderSetup.Description;
                    dropDown.value = orderSetup.Code;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getEncounterSetupList(string type, string typeValue)
        {
            DropDownList model = new DropDownList();

            try
            {
                EncounterSetup encountersetup = new EncounterSetup();
                List<EncounterSetup> listConcept = encountersetup.getEncounterSetup(typeValue);

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (EncounterSetup encounterSetup in listConcept)
                {
                    dropDown = new DropDown();

                    dropDown.text = encounterSetup.Description;
                    dropDown.value = encounterSetup.Code;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getSpecialityList(string type, mzk_locationtype _locationType, List<string> specialityIdList = null)
        {
            DropDownList model = new DropDownList();

            try
            {
                List<Speciality> listSpeciality;
                Speciality encountersetup = new Speciality();
                List<DropDown> listDropDown = new List<DropDown>();
                model.Type = type;
                DropDown dropDown;
                dropDown = new DropDown();
                dropDown.text = "All";
                dropDown.value = "0";
                dropDown.IsDefault = "True";
                listDropDown.Add(dropDown);

                listSpeciality = encountersetup.getSpecialityList(specialityIdList, _locationType);

                foreach (Speciality speciality in listSpeciality)
                {
                    dropDown = new DropDown();
                    dropDown.text = speciality.Description;
                    dropDown.value = speciality.SpecialityId;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }

        public DropDownList getAgeGroupList(string type)
        {
            DropDownList model = new DropDownList();

            try
            {
                List<AgeGroup> list = new AgeGroup().getList();

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (AgeGroup data in list)
                {
                    dropDown = new DropDown();

                    dropDown.text = data.Description;
                    dropDown.value = data.Id;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }

        public DropDownList getActionManagerList(string type)
        {
            DropDownList model = new DropDownList();

            try
            {
                List<ActionManager> list = new ActionManager().getList();

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (ActionManager data in list)
                {
                    dropDown = new DropDown();

                    dropDown.text = data.name;
                    dropDown.value = data.Id;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }

        public DropDownList getUserList(string type, string SpecialityId = null)
        {
            DropDownList model = new DropDownList();
            long SpecialityRefRecId = 0;

            try
            {
                User usersetup = new User();
                List<User> listUser = new List<User>();

                if (!string.IsNullOrEmpty(SpecialityId))
                {
                    Speciality sp = new Speciality();
                    List<string> specialityIdList = new List<string>();
                    specialityIdList.Add(SpecialityId);

                    if (sp.getSpecialityList(specialityIdList).Count > 0)
                    {
                        SpecialityRefRecId = sp.getSpecialityList(specialityIdList).First<Speciality>().SpecialityRefRecId;
                        listUser = usersetup.getUsers(SpecialityRefRecId, true, string.Empty, SpecialityId);

                        model.Type = type;
                        DropDown dropDown;
                        List<DropDown> listDropDown = new List<DropDown>();

                        foreach (User user in listUser)
                        {
                            dropDown = new DropDown();

                            dropDown.text = user.Name;
                            dropDown.value = user.userId;
                            dropDown.address = user.Address;
                            listDropDown.Add(dropDown);
                        }

                        model.DropDownValues = listDropDown;
                    }
                }
                else
                {
                    listUser = usersetup.getUser();
                    model.Type = type;
                    DropDown dropDown;
                    List<DropDown> listDropDown = new List<DropDown>();

                    foreach (User user in listUser)
                    {
                        dropDown = new DropDown();

                        dropDown.text = user.Name;
                        dropDown.value = user.userId;
                        dropDown.address = user.Address;
                        listDropDown.Add(dropDown);
                    }
                    model.DropDownValues = listDropDown;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getReferringPhysicianList(string type, string SpecialityId = null)
        {
            DropDownList model = new DropDownList();

            try
            {
                ReferringPhysician referringphysiciansetup = new ReferringPhysician();
                List<ReferringPhysician> listreferringphysician = referringphysiciansetup.getReferringPhysician(SpecialityId);

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (ReferringPhysician physician in listreferringphysician)
                {
                    dropDown = new DropDown();
                    dropDown.text = physician.name;
                    dropDown.value = physician.id;
                    dropDown.address = physician.address;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getUnitList(string type, string typeValue, string Dosage)
        {
            DropDownList model = new DropDownList();

            try
            {
                Unit units = new Unit();
                List<Unit> listUnit = units.getUnit(typeValue, Dosage);

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                foreach (Unit unit in listUnit)
                {
                    dropDown = new DropDown();
                    dropDown.text = unit.Description;
                    dropDown.value = unit.UnitId;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getOptionSetList(string type, string entityname, string attributename, string defaultVal)
        {
            DropDownList model = new DropDownList();

            try
            {
                OptionSet optionset = new OptionSet();
                List<OptionSet> listConcept = optionset.getOptionSetItems(entityname, attributename);

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();
                dropDown = new DropDown();


                if (type == "Outcome" || type == "FollowUp" || type == "FollowUpNumber")
                {
                    dropDown.text = "";
                    dropDown.value = "0";
                    dropDown.IsDefault = "True";
                    listDropDown.Add(dropDown);
                }

                foreach (OptionSet orderSetup in listConcept)
                {
                    dropDown = new DropDown();

                    dropDown.text = orderSetup.OptionSetName;
                    dropDown.value = orderSetup.OptionSetValue;
                    dropDown.color = orderSetup.OptionSetColor;
                    if (orderSetup.OptionSetValue == defaultVal)
                        dropDown.IsDefault = "True";
                    else
                        dropDown.IsDefault = "False";
                    listDropDown.Add(dropDown);

                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getAXDropDown(string type)
        {
            DropDownList model = new DropDownList();

            try
            {
                model.Type = type;

                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                DropDownRepository repo = new DropDownRepository();
                HMDropDownContract[] contractArray = repo.getDropDownList(type);

                foreach (HMDropDownContract contract in contractArray)
                {
                    dropDown = new DropDown();
                    dropDown.text = contract.parmDescription;
                    dropDown.value = contract.parmID;
                    dropDown.IsDefault = contract.parmIsDefault.ToString();
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getSpecialityListByResourceId(string type, string resourceId)
        {
            DropDownList model = new DropDownList();

            try
            {
                User encountersetup = new User();

                List<Speciality> listSpeciality = encountersetup.getSpecialities(resourceId);

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();

                dropDown = new DropDown();

                dropDown.text = "All";
                dropDown.value = "0";
                dropDown.IsDefault = "True";
                listDropDown.Add(dropDown);

                foreach (Speciality speciality in listSpeciality)
                {
                    dropDown = new DropDown();

                    dropDown.text = speciality.Description;
                    dropDown.value = speciality.SpecialityId;
                    dropDown.IsDefault = "False";
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
        public DropDownList getPartnerHospital(string type)
        {
            DropDownList model = new DropDownList();

            try
            {
                Hospital hospitalsetup = new Hospital();
                List<Hospital> listHospital = hospitalsetup.getHospital();

                model.Type = type;
                DropDown dropDown;
                List<DropDown> listDropDown = new List<DropDown>();
                dropDown = new DropDown();
                dropDown.text = "";
                dropDown.value = "0";
                dropDown.IsDefault = "True";
                listDropDown.Add(dropDown);
                foreach (Hospital hospital in listHospital)
                {
                    dropDown = new DropDown();
                    dropDown.text = hospital.Name;
                    dropDown.value = hospital.Id;
                    listDropDown.Add(dropDown);
                }

                model.DropDownValues = listDropDown;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }
    }
    public class Hospital
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<Hospital> getHospital()
        {
            List<Hospital> Hospital = new List<Hospital>();
            #region Hospital Query
            QueryExpression query = new QueryExpression("mzk_hospital");
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_name");

            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                Hospital model = new Hospital();
                model.Id = entity.Id.ToString();
                if (entity.Attributes.Contains("mzk_name"))
                    model.Name = entity.Attributes["mzk_name"].ToString();
                Hospital.Add(model);
            }
            return Hospital;
        }
    }
}
