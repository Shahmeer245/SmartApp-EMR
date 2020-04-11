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
    public class PatientEncounter : IPatientEncounter
    {
        private EntityRepository<Mzk_patientencounter> _entityRep;

        public string EncounterNo { get; set; }
        public string EncounterTemplateId { get; set; }
        public string CheifComplaint { get; set; }
        public string EncounterId { get; set; }
        public string EncounterStatus { get; set; }
        public string AllergyReviewStatus { get; set; }
        public string AllergyReviewedById { get; set; }
        public string AllergyReviewedByName { get; set; }
        public string userId { get; set; }
        public string EncounterType { get; set; }
        public string EncounterTypeName { get; set; }
        public string CaseId { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public string SignedBy { get; set; }
        public string RxNumber { get; set; }
        public string searchEncounter { get; set; }
        public string Instruction { get; set; }
        public string SummaryJson { get; set; }
        public string CaseNumber { get; set; }
        public string CaseType { get; set; }
        public string SearchFilters { get; set; }
        public int EncounterStatusValue { get; set; }
        public DateTime AllergyReviewedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime SignedOn { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isConsultation { get; set; }
        public bool diagnosisRequired { get; set; }
        public decimal AXRefrecid { get; set; }
        public long AppointmentRefRecId { get; set; }
        public bool isAutoSignOff { get; set; }
        public string AppointmentId { get; set; }

        public string PatientId { get; set; }
        public mzk_casetype caseTypeValue { get; set; }

        string CurrentEncounterId = string.Empty;

        public Guid PatientID { get; set; }
        public string Externalemrid { get; set; }
        public DateTime RecordedDate { get; set; }
        public string Title { get; set; }

        public async Task<string> addPatientEncounter(string CaseId, int encounterType, string worklistTypeID, long resourceRecId = 0, string cpsaWorkflowId = "", string resourceId = null, string patientId = null, string appointmentId = null)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            Entity encounterEntity = new Entity("mzk_patientencounter");

            if (!string.IsNullOrEmpty(appointmentId))
            {
                encounterEntity.Attributes["mzk_appointment"] = new EntityReference(mzk_patientappointment.EntityLogicalName, new Guid(appointmentId));
            }
            
            encounterEntity.Attributes["mzk_caseid"] = new EntityReference(xrm.Incident.EntityLogicalName, new Guid(CaseId));
            encounterEntity.Attributes["mzk_customer"] = new EntityReference(xrm.Contact.EntityLogicalName, new Guid(patientId));
            encounterEntity.Attributes["mzk_encountertype"] = new OptionSetValue(encounterType);
            encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue((int)mzk_encounterstatus.Open);

            if (!string.IsNullOrEmpty(cpsaWorkflowId))
            {
                encounterEntity.Attributes["mzk_casepathwaystateactivityworkflow"] = new EntityReference(mzk_casepathwaystateactivityworkflow.EntityLogicalName, new Guid(cpsaWorkflowId));
            }
            string encounterTemplateId = this.getEncounterTemplateId(worklistTypeID, cpsaWorkflowId, resourceId, patientId, appointmentId);

            if (!string.IsNullOrEmpty(encounterTemplateId))
            {
                encounterEntity.Attributes["mzk_encountertemplate"] = new EntityReference(mzk_uitemplate.EntityLogicalName, new Guid(encounterTemplateId));
            }
            else
            {
                throw new ValidationException("Encounter template not set. Failed to create encounter");
            }

            EncounterNo = entityRepository.CreateEntity(encounterEntity).ToString();

            if (EncounterNo != null && EncounterNo.ToString() != string.Empty)
            {
                PatientDisposition disposition = new PatientDisposition();
                disposition.EncounterId = EncounterNo;
                await disposition.AddPatientDisposition(disposition);

                if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                {
                    if ((int)mzk_encountertype.Consultation == encounterType && (!string.IsNullOrEmpty(CaseId)))
                    {
                        CaseRepository caseRepo = new CaseRepository();

                        caseRepo.updateCaseTransStatus(0, HMServiceStatus.Started, false, 0, "", CaseId);
                    }
                    else if ((int)mzk_encountertype.PrimaryAssessment == encounterType && (!string.IsNullOrEmpty(CaseId)))
                    {
                        CaseRepository caseRepo = new CaseRepository();

                        caseRepo.updateCaseTransStatus(0, HMServiceStatus.Started, false, 0, "", CaseId, resourceRecId);
                    }
                    else if ((int)mzk_encountertype.Assessment == encounterType && resourceRecId > 0)
                    {
                        User userObj = new User().getUser(resourceRecId).FirstOrDefault();

                        if (userObj != null)
                        {
                            PatientReferralOrder referral = new PatientReferralOrder().referralExist(CaseId, userObj.userId);

                            if (referral != null && !string.IsNullOrEmpty(referral.CaseTransRecId))
                            {
                                CaseRepository caseRepo = new CaseRepository();
                                long caseTransRecId = (long)Convert.ToDecimal(referral.CaseTransRecId);

                                caseRepo.updateCaseTransStatus(caseTransRecId, HMServiceStatus.Started, false, 0, "", "");
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(cpsaWorkflowId))
                {
                    SoapEntityRepository repo = SoapEntityRepository.GetService();

                    QueryExpression query = null;

                    EntityCollection entitycollection = null;

                    PatientCase patientCase = new PatientCase();
                    PatientCase patientCaseDetail = await patientCase.getCaseDetails(CaseId);

                    query = new QueryExpression(mzk_casepathwaystateactivityworkflow.EntityLogicalName);
                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                    query.Criteria.AddCondition("mzk_casepathwaystateactivityworkflowid", ConditionOperator.Equal, cpsaWorkflowId);

                    LinkEntity cpsOutcome = new LinkEntity()
                    {
                        Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true),
                        EntityAlias = "CPSO",
                        LinkFromEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                        LinkToEntityName = mzk_casepathwaystateoutcome.EntityLogicalName,
                        LinkFromAttributeName = "mzk_casepathwaystate",
                        LinkToAttributeName = "mzk_casepathwaystate",
                        JoinOperator = JoinOperator.Inner
                    };

                    LinkEntity mmtCode = new LinkEntity()
                    {
                        Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true),
                        EntityAlias = "MMTCode",
                        LinkFromEntityName = mzk_casepathwaystateoutcome.EntityLogicalName,
                        LinkToEntityName = mzk_mmtcode.EntityLogicalName,
                        LinkFromAttributeName = "mzk_measurement",
                        LinkToAttributeName = "mzk_mmtcodeid",
                        JoinOperator = JoinOperator.Inner
                    };
                    //Commented to bring Objective and Subjetive both
                    //mmtCode.LinkCriteria.AddCondition("mzk_datatype", ConditionOperator.Equal, (int)mzk_mmtcodemzk_datatype.Objective);

                    cpsOutcome.LinkEntities.Add(mmtCode);
                    query.LinkEntities.Add(cpsOutcome);


                    entitycollection = repo.GetEntityCollection(query);

                    List<ClinicalTemplate> patientTemplates = new List<ClinicalTemplate>();
                    List<ClinicalTemplate> patientTemplatesEmpty = new List<ClinicalTemplate>();
                    ClinicalTemplate template = null;
                    Dictionary<string, string> templatesDictionary = new Dictionary<string, string>();
                    
                    foreach (Entity entity in entitycollection.Entities)
                    {
                        //mzk_uitemplatevital uitemplatevital = (mzk_uitemplatevital)entity;
                        PatientVitals vitals = new PatientVitals();
                        //if (uiTemplate.defaultVitals == mzk_uitemplatemzk_DefaultVitals.DefaultfromPatientDemographics && patientCaseDetail != null)
                        //{
                        //    MmtGroupCode mmtgroup = new MmtGroupCode();

                        //    if (!mmtgroup.isValidGroupById(uitemplatevital.mzk_Measurementgroup.Id.ToString(), patientCaseDetail.PatientId))
                        //    {
                        //        continue;
                        //    }
                        //}
                        if (((Microsoft.Xrm.Sdk.OptionSetValue)((entity.Attributes["MMTCode.mzk_datatype"] as AliasedValue).Value)).Value == (int)mzk_mmtcodemzk_datatype.Objective)
                        {
                            await vitals.addDefaultVitals(EncounterNo.ToString(), (entity.Attributes["MMTCode.mzk_mmtcodeid"] as AliasedValue).Value.ToString(), false, (entity.Attributes["CPSO.mzk_casepathwaystateoutcomeid"] as AliasedValue).Value.ToString());
                        }
                        else if (((Microsoft.Xrm.Sdk.OptionSetValue)((entity.Attributes["MMTCode.mzk_datatype"] as AliasedValue).Value)).Value == (int)mzk_mmtcodemzk_datatype.Subjective)
                        {
                            string mzk_sonotesclinicaltemplate = "";
                            mzk_sonotesclinicaltemplate = entity.Attributes.ContainsKey("CPSO.mzk_sonotesclinicaltemplate") ? ((entity.Attributes["CPSO.mzk_sonotesclinicaltemplate"] as AliasedValue).Value as EntityReference).Id.ToString() : "";//(entity.Attributes["CPSO.mzk_sonotesclinicaltemplate"] as AliasedValue).Value.ToString();
                            template = new ClinicalTemplate();
                            template.Id = mzk_sonotesclinicaltemplate;// uitemplateclinicaltemplate.mzk_ClinicalTemplate.Id.ToString();
                            template.CPSOid = (entity.Attributes["CPSO.mzk_casepathwaystateoutcomeid"] as AliasedValue).Value.ToString();

                            if (patientCaseDetail != null)
                            {
                                ClinicalTemplate clinicalTemplateValidate = new ClinicalTemplate();
                                if (mzk_sonotesclinicaltemplate != "")
                                {
                                    if (!clinicalTemplateValidate.isValidCriteriaById(template.Id, patientCaseDetail.PatientId))
                                    {
                                        continue;
                                    }
                                }
                            }

                            if (mzk_sonotesclinicaltemplate != "")
                            {
                                //string uniqueTemplateID = null;
                                //templatesDictionary.TryGetValue(mzk_sonotesclinicaltemplate, out uniqueTemplateID);
                                //if (uniqueTemplateID == null)
                                //{
                                //    templatesDictionary.Add(mzk_sonotesclinicaltemplate, mzk_sonotesclinicaltemplate);
                                //    patientTemplates.Add(template);
                                //}
                                patientTemplates.Add(template);
                            }
                            else
                            {
                                patientTemplatesEmpty.Add(template);
                            }
                        }
                    }

                    if (patientTemplates != null && patientTemplates.Count > 0 && patientCaseDetail != null)
                    {
                        template = new ClinicalTemplate();
                        await template.addClinicalTemplatesAndInsertFindings(patientCaseDetail.PatientId, EncounterNo.ToString(), patientTemplates);
                    }

                    if (patientTemplatesEmpty != null && patientTemplatesEmpty.Count > 0 && patientCaseDetail != null)
                    {
                        template = new ClinicalTemplate();
                        await template.addAnEmptyClinicalTemplateAndMultipleFindings(patientCaseDetail.PatientId, EncounterNo.ToString(), patientTemplatesEmpty);
                    }

                    SoapEntityRepository repoLabOrder = SoapEntityRepository.GetService();
                    query = new QueryExpression(mzk_casepathwaystateactivityworkflow.EntityLogicalName);
                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(false);
                    query.Criteria.AddCondition("mzk_casepathwaystateactivityworkflowid", ConditionOperator.Equal, cpsaWorkflowId);

                    LinkEntity casePathwayState = new LinkEntity()
                    {
                        Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(false),
                        EntityAlias = "CPS",
                        LinkFromEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                        LinkToEntityName = mzk_casepathwaystate.EntityLogicalName,
                        LinkFromAttributeName = "mzk_casepathwaystate",
                        LinkToAttributeName = "mzk_casepathwaystateid",
                        JoinOperator = JoinOperator.Inner
                    };

                    LinkEntity activityMaster = new LinkEntity()
                    {
                        Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(false),
                        EntityAlias = "AM",
                        LinkFromEntityName = mzk_casepathwaystate.EntityLogicalName,
                        LinkToEntityName = mzk_activitymaster.EntityLogicalName,
                        LinkFromAttributeName = "mzk_activitymaster",
                        LinkToAttributeName = "mzk_activitymasterid",
                        JoinOperator = JoinOperator.Inner
                    };

                    LinkEntity cpsaData = new LinkEntity()
                    {
                        Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_product", "mzk_frequency", "mzk_route", "mzk_urgency", "mzk_specimensource", "mzk_unit", "mzk_medicationordertype", "mzk_referralcategory", "mzk_speciality", "mzk_category", "mzk_educationresource", "mzk_internalreferringphysician", "mzk_externalreferringphysician", "mzk_dose"),
                        EntityAlias = "CPSAD",
                        LinkFromEntityName = mzk_casepathwaystate.EntityLogicalName,
                        LinkToEntityName = mzk_casepathwayactivitydata.EntityLogicalName,
                        LinkFromAttributeName = "mzk_casepathwaystateid",
                        LinkToAttributeName = "mzk_casepathwaystate",
                        JoinOperator = JoinOperator.Inner
                    };


                    LinkEntity activityType = new LinkEntity()
                    {
                        Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_category"),
                        EntityAlias = "AT",
                        LinkFromEntityName = mzk_activitymaster.EntityLogicalName,
                        LinkToEntityName = mzk_activitytype.EntityLogicalName,
                        LinkFromAttributeName = "mzk_activitytype",
                        LinkToAttributeName = "mzk_activitytypeid",
                        JoinOperator = JoinOperator.Inner
                    };

                    FilterExpression filter = cpsaData.LinkCriteria.AddFilter(LogicalOperator.Or);
                    filter.AddCondition("mzk_category", ConditionOperator.Equal, (int)mzk_activitycategory.RxOrder);
                    filter.AddCondition("mzk_category", ConditionOperator.Equal, (int)mzk_activitycategory.RxAdmin);
                    filter.AddCondition("mzk_category", ConditionOperator.Equal, (int)mzk_activitycategory.TxOrder);
                    filter.AddCondition("mzk_category", ConditionOperator.Equal, (int)mzk_activitycategory.TxAdmin);
                    filter.AddCondition("mzk_category", ConditionOperator.Equal, (int)mzk_activitycategory.LabOrder);
                    filter.AddCondition("mzk_category", ConditionOperator.Equal, (int)mzk_activitycategory.RadOrder);
                    filter.AddCondition("mzk_category", ConditionOperator.Equal, (int)mzk_activitycategory.ReferralOrder);
                    filter.AddCondition("mzk_category", ConditionOperator.Equal, (int)mzk_activitycategory.Educate);


                    activityMaster.LinkEntities.Add(activityType);
                    casePathwayState.LinkEntities.Add(activityMaster);
                    casePathwayState.LinkEntities.Add(cpsaData);
                    query.LinkEntities.Add(casePathwayState);

                    entitycollection = repoLabOrder.GetEntityCollection(query);

                    //fetch default clinic
                    QueryExpression queryUser = new QueryExpression(xrm.Incident.EntityLogicalName);

                    queryUser.Criteria.AddCondition("incidentid", ConditionOperator.Equal, new Guid(CaseId));

                    LinkEntity careTeamMember = new LinkEntity(xrm.Incident.EntityLogicalName, mzk_casecareteammember.EntityLogicalName, "mzk_casecareteam", "mzk_casecareteam", JoinOperator.Inner);
                    queryUser.LinkEntities.Add(careTeamMember);

                    LinkEntity resource = new LinkEntity(mzk_casecareteammember.EntityLogicalName, BookableResource.EntityLogicalName, "mzk_user", "userid", JoinOperator.Inner);
                    careTeamMember.LinkEntities.Add(resource);

                    LinkEntity resourceClinic = new LinkEntity(BookableResource.EntityLogicalName, mzk_resourceclinic.EntityLogicalName, "bookableresourceid", "mzk__resource", JoinOperator.Inner);
                    resource.LinkEntities.Add(resourceClinic);

                    LinkEntity organizationalUnit = new LinkEntity(mzk_resourceclinic.EntityLogicalName, msdyn_organizationalunit.EntityLogicalName, "mzk_organizationalunit", "msdyn_organizationalunitid", JoinOperator.Inner);
                    organizationalUnit.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                    organizationalUnit.LinkCriteria.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_orgunittype.NursingUnit);
                    organizationalUnit.EntityAlias = "clinic";

                    resourceClinic.LinkEntities.Add(organizationalUnit);

                    string treatmentLocation = "";

                     EntityCollection clinicCollection = repoLabOrder.GetEntityCollection(queryUser);

                     if(clinicCollection != null && clinicCollection.Entities != null && clinicCollection.Entities.Count > 0)
                     {
                         if (clinicCollection.Entities[0].Attributes.Contains("clinic.msdyn_organizationalunitid"))
                         {
                             treatmentLocation = (clinicCollection.Entities[0].Attributes["clinic.msdyn_organizationalunitid"] as AliasedValue).Value.ToString();
                         }
                     }                     

                    foreach (Entity entity in entitycollection.Entities)
                    {
                        mzk_activitycategory category = (mzk_activitycategory)((entity.Attributes["CPSAD.mzk_category"] as AliasedValue).Value as OptionSetValue).Value;
                        string productId = string.Empty;
                        string frequency = string.Empty;
                        string route = string.Empty;
                        string urgency = string.Empty;
                        string specimenSource = string.Empty;
                        string unit = string.Empty;
                        string medicationOrderType = string.Empty;
                        string returnId = string.Empty;
                        int referralCategory = -1;
                        string speciality = string.Empty;
                        string educationResourceId = string.Empty;
                        string internalDoctorId = string.Empty;
                        string externalDoctorId = string.Empty;
                        string dose = string.Empty;

                        if (entity.Attributes.Contains("CPSAD.mzk_product"))
                            productId = ((entity.Attributes["CPSAD.mzk_product"] as AliasedValue).Value as EntityReference).Id.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_frequency"))
                            frequency = ((entity.Attributes["CPSAD.mzk_frequency"] as AliasedValue).Value as EntityReference).Id.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_route"))
                            route = ((entity.Attributes["CPSAD.mzk_route"] as AliasedValue).Value as EntityReference).Id.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_urgency"))
                            urgency = ((entity.Attributes["CPSAD.mzk_urgency"] as AliasedValue).Value as OptionSetValue).Value.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_specimensource"))
                            specimenSource = ((entity.Attributes["CPSAD.mzk_specimensource"] as AliasedValue).Value as EntityReference).Id.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_unit"))
                            unit = ((entity.Attributes["CPSAD.mzk_unit"] as AliasedValue).Value as EntityReference).Id.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_medicationordertype"))
                            medicationOrderType = ((entity.Attributes["CPSAD.mzk_medicationordertype"] as AliasedValue).Value as OptionSetValue).Value.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_referralcategory"))
                            referralCategory = ((OptionSetValue)(entity.Attributes["CPSAD.mzk_referralcategory"] as AliasedValue).Value).Value;
                        if (entity.Attributes.Contains("CPSAD.mzk_speciality"))
                            speciality = ((entity.Attributes["CPSAD.mzk_speciality"] as AliasedValue).Value as EntityReference).Id.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_educationresource"))
                            educationResourceId = ((entity.Attributes["CPSAD.mzk_educationresource"] as AliasedValue).Value as EntityReference).Id.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_internalreferringphysician"))
                            internalDoctorId = ((entity.Attributes["CPSAD.mzk_internalreferringphysician"] as AliasedValue).Value as EntityReference).Id.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_externalreferringphysician"))
                            externalDoctorId = ((entity.Attributes["CPSAD.mzk_externalreferringphysician"] as AliasedValue).Value as EntityReference).Id.ToString();
                        if (entity.Attributes.Contains("CPSAD.mzk_dose"))
                            dose = ((entity.Attributes["CPSAD.mzk_dose"] as AliasedValue).Value).ToString();


                        if (category == mzk_activitycategory.RxOrder || category == mzk_activitycategory.RxAdmin)
                        {
                            PatientMedication patientMedication = new PatientMedication();
                            patientMedication.EncounterId = EncounterNo;
                            patientMedication.CaseId = CaseId;
                            //patientMedication.clinicRecId = axAppoitnmentRefId;
                            patientMedication.StartDate = DateTime.Now.Date;
                            patientMedication.EndDate = DateTime.Now.Date;
                            patientMedication.MedicationName = productId;
                            patientMedication.Frequency = frequency;
                            patientMedication.Route = route;
                            patientMedication.Urgency = urgency;
                            patientMedication.Unit = unit;
                            patientMedication.Type = string.IsNullOrEmpty(medicationOrderType) ? "3" : medicationOrderType;
                            patientMedication.treatmentLocationId = treatmentLocation;
                            patientMedication.Duration = "1";
                            patientMedication.Dosage = dose;

                            returnId = await patientMedication.addPatientOrder(patientMedication, category == mzk_activitycategory.RxOrder);

                            if (string.IsNullOrEmpty(returnId))
                            {
                                throw new ValidationException("Unable to create patient order");
                            }
                        }
                        else if (category == mzk_activitycategory.TxOrder || category == mzk_activitycategory.TxAdmin)
                        {
                            PatientProcedure patientProcedure = new PatientProcedure();
                            patientProcedure.EncounterId = EncounterNo;
                            patientProcedure.CaseId = CaseId;
                            //  patientProcedure.clinicRecId = axAppoitnmentRefId;
                            patientProcedure.OrderDate = DateTime.Now.Date;
                            patientProcedure.ProcedureId = productId;
                            patientProcedure.treatmentLocationId = treatmentLocation;

                            returnId = await patientProcedure.addPatientOrder(patientProcedure, category == mzk_activitycategory.TxOrder);

                            if (string.IsNullOrEmpty(returnId))
                            {
                                throw new ValidationException("Unable to create patient order");
                            }
                        }
                        else if (category == mzk_activitycategory.LabOrder)
                        {
                            PatientLabOrder patientLabOrder = new PatientLabOrder();
                            patientLabOrder.EncounterId = EncounterNo;
                            patientLabOrder.CaseId = CaseId;
                            // patientLabOrder.clinicRecId = axAppoitnmentRefId;
                            patientLabOrder.OrderDate = DateTime.Now.Date;
                            patientLabOrder.TestName = productId;
                            patientLabOrder.Frequency = frequency;

                            if (!string.IsNullOrEmpty(specimenSource))
                            {
                                patientLabOrder.specimensourcelist = new List<string>();
                                patientLabOrder.specimensourcelist.Add(specimenSource);
                            }

                            returnId = await patientLabOrder.addPatientOrder(patientLabOrder, true);

                            if (string.IsNullOrEmpty(returnId))
                            {
                                throw new ValidationException("Unable to create patient order");
                            }
                        }
                        else if (category == mzk_activitycategory.RadOrder)
                        {
                            PatientRadiologyOrder patientRadOrder = new PatientRadiologyOrder();
                            patientRadOrder.EncounterId = EncounterNo;
                            patientRadOrder.CaseId = CaseId;
                            // patientRadOrder.clinicRecId = axAppoitnmentRefId;
                            patientRadOrder.OrderDate = DateTime.Now.Date;
                            patientRadOrder.TestName = productId;
                            patientRadOrder.Frequency = frequency;

                            returnId = await patientRadOrder.addPatientOrder(patientRadOrder, true);

                            if (string.IsNullOrEmpty(returnId))
                            {
                                throw new ValidationException("Unable to create patient order");
                            }
                        }

                        else if (category == mzk_activitycategory.ReferralOrder)
                        {
                            PatientReferralOrder patientReferral = new PatientReferralOrder();

                            patientReferral.SpecialtyId = speciality;
                            patientReferral.CaseId = CaseId;
                            patientReferral.OrderDate = DateTime.Now.Date;
                            patientReferral.EncounterId = EncounterNo;

                            if (referralCategory == (int)mzk_referralcategory.Internal)
                            {
                                patientReferral.Category = (int)mzk_referralcategory.Internal;
                                patientReferral.ReferralId = internalDoctorId;
                            }
                            else if (referralCategory == (int)mzk_referralcategory.External)
                            {
                                patientReferral.Category = (int)mzk_referralcategory.External;
                                patientReferral.ReferralId = externalDoctorId;
                            }

                            returnId = await patientReferral.addPatientOrder(patientReferral);

                            if (string.IsNullOrEmpty(returnId))
                            {
                                throw new ValidationException("Unable to create patient order");
                            }

                        }

                        else if (category == mzk_activitycategory.Educate)
                        {
                            EducationalResource educationalResource = new EducationalResource();
                            List<EducationalResource> listEducationalResource = new List<EducationalResource>();

                            educationalResource.educationalResourceId = educationResourceId;

                            listEducationalResource.Add(educationalResource);

                            bool check = await educationalResource.addPatientEncounterEducationalResource(listEducationalResource);
                        }
                    }


                    //List<ClinicalTemplate> patientTemplates = new List<ClinicalTemplate>();

                    //ClinicalTemplate template = null;

                    //query = new QueryExpression(mzk_uitemplateclinicaltemplate.EntityLogicalName);

                    //query.Criteria.AddCondition("mzk_uitemplate", ConditionOperator.Equal, new Guid(encounterTemplateId));
                    //query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                    //query.AddOrder("mzk_priority", OrderType.Ascending);

                    //entitycollection = repo.GetEntityCollection(query);

                    //foreach (Entity entity in entitycollection.Entities)
                    //{
                    //    mzk_uitemplateclinicaltemplate uitemplateclinicaltemplate = (mzk_uitemplateclinicaltemplate)entity;

                    //    template = new ClinicalTemplate();

                    //    template.Id = uitemplateclinicaltemplate.mzk_ClinicalTemplate.Id.ToString();

                    //    if (uiTemplate.defaultClinicalTemplate == mzk_uitemplatemzk_DefaultClinicalTemplates.DefaultfromPatientDemographics && patientCaseDetail != null)
                    //    {
                    //        ClinicalTemplate clinicalTemplateValidate = new ClinicalTemplate();

                    //        if (!clinicalTemplateValidate.isValidCriteriaById(template.Id, patientCaseDetail.PatientId))
                    //        {
                    //            continue;
                    //        }
                    //    }

                    //    patientTemplates.Add(template);
                    //}


                    //if (patientTemplates != null && patientTemplates.Count > 0 && patientCaseDetail != null)
                    //{
                    //    template = new ClinicalTemplate();

                    //    await template.addClinicalTemplates(patientCaseDetail.PatientId, EncounterNo.ToString(), patientTemplates);

                    //}
                }

                if (!string.IsNullOrEmpty(encounterTemplateId))
                {
                    Boolean addDefaults = true;

                    if(!string.IsNullOrEmpty(cpsaWorkflowId))
                    {
                        SoapEntityRepository soapRepo = SoapEntityRepository.GetService();
                        QueryExpression query = new QueryExpression(mzk_casepathwaystateactivityworkflow.EntityLogicalName);
                        query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(false);
                        query.Criteria.AddCondition("mzk_casepathwaystateactivityworkflowid", ConditionOperator.Equal, cpsaWorkflowId);

                        LinkEntity casePathwayState = new LinkEntity()
                        {
                            Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true),
                            EntityAlias = "AW",
                            LinkFromEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                            LinkToEntityName = mzk_activityworkflow.EntityLogicalName,
                            LinkFromAttributeName = "mzk_activityworkflow",
                            LinkToAttributeName = "mzk_activityworkflowid",
                            JoinOperator = JoinOperator.Inner
                        };

                        query.LinkEntities.Add(casePathwayState);

                        EntityCollection  entitycollection = soapRepo.GetEntityCollection(query);

                        if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                        {
                            if(entitycollection.Entities[0].Attributes.Contains("AW.mzk_uitemplateselection"))
                            {
                                if (((entitycollection.Entities[0].Attributes["AW.mzk_uitemplateselection"] as AliasedValue).Value as OptionSetValue).Value == (int)mzk_uitemplateselection.Dynamic)
                                {
                                    addDefaults = false;
                                }
                            }
                        }                        
                    }

                    if (addDefaults)
                    {
                        UITemplate uiTemplate = new UITemplate().getUITemplateDetails(encounterTemplateId).Result;

                        if (uiTemplate != null)
                        {
                            SoapEntityRepository repo = SoapEntityRepository.GetService();

                            QueryExpression query = null;

                            EntityCollection entitycollection = null;

                            PatientCase patientCase = new PatientCase();
                            PatientCase patientCaseDetail = await patientCase.getCaseDetails(CaseId);

                            query = new QueryExpression(mzk_uitemplatevital.EntityLogicalName);

                            query.Criteria.AddCondition("mzk_uitemplate", ConditionOperator.Equal, new Guid(encounterTemplateId));
                            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                            query.AddOrder("mzk_priority", OrderType.Ascending);

                            entitycollection = repo.GetEntityCollection(query);

                            foreach (Entity entity in entitycollection.Entities)
                            {
                                mzk_uitemplatevital uitemplatevital = (mzk_uitemplatevital)entity;
                                PatientVitals vitals = new PatientVitals();

                                if (uiTemplate.defaultVitals == mzk_uitemplatemzk_DefaultVitals.DefaultfromPatientDemographics && patientCaseDetail != null)
                                {
                                    MmtGroupCode mmtgroup = new MmtGroupCode();

                                    if (!mmtgroup.isValidGroupById(uitemplatevital.mzk_Measurementgroup.Id.ToString(), patientCaseDetail.PatientId))
                                    {
                                        continue;
                                    }
                                }
                                await vitals.addDefaultVitals(EncounterNo.ToString(), uitemplatevital.mzk_Measurementgroup.Id.ToString(), true, "");
                            }

                            List<ClinicalTemplate> patientTemplates = new List<ClinicalTemplate>();

                            ClinicalTemplate template = null;

                            query = new QueryExpression(mzk_uitemplateclinicaltemplate.EntityLogicalName);

                            query.Criteria.AddCondition("mzk_uitemplate", ConditionOperator.Equal, new Guid(encounterTemplateId));
                            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                            query.AddOrder("mzk_priority", OrderType.Ascending);

                            entitycollection = repo.GetEntityCollection(query);

                            foreach (Entity entity in entitycollection.Entities)
                            {
                                mzk_uitemplateclinicaltemplate uitemplateclinicaltemplate = (mzk_uitemplateclinicaltemplate)entity;

                                template = new ClinicalTemplate();

                                template.Id = uitemplateclinicaltemplate.mzk_ClinicalTemplate.Id.ToString();

                                if (uiTemplate.defaultClinicalTemplate == mzk_uitemplatemzk_DefaultClinicalTemplates.DefaultfromPatientDemographics && patientCaseDetail != null)
                                {
                                    ClinicalTemplate clinicalTemplateValidate = new ClinicalTemplate();

                                    if (!clinicalTemplateValidate.isValidCriteriaById(template.Id, patientCaseDetail.PatientId))
                                    {
                                        continue;
                                    }
                                }
                                patientTemplates.Add(template);
                            }


                            if (patientTemplates != null && patientTemplates.Count > 0 && patientCaseDetail != null)
                            {
                                template = new ClinicalTemplate();
                                await template.addClinicalTemplates(patientCaseDetail.PatientId, EncounterNo.ToString(), patientTemplates);
                            }
                        }
                    }
                }
            }

            return EncounterNo.ToString();
            //if (EncounterNo.ToString() != null)
            //    return true;
            //else
            //    return false;

        }
        public async Task<List<UITemplate>> getEncounterTemplate(string encounterTemplateId, string cpsaWorkflowId, string patientId)
        {
            List<UITemplate> listModel = new List<UITemplate>();
            UITemplate model;

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_uitemplatesmenulist.EntityLogicalName);

                query.Criteria.AddCondition("mzk_uitemplateid", ConditionOperator.Equal, new Guid(encounterTemplateId));
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_ordering");
                query.AddOrder("mzk_ordering", OrderType.Ascending);

                LinkEntity EntityList = new LinkEntity(mzk_uitemplatesmenulist.EntityLogicalName, mzk_menulist.EntityLogicalName, "mzk_menulist", "mzk_menulistid", JoinOperator.Inner);

                EntityList.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityList.EntityAlias = mzk_menulist.EntityLogicalName;

                if (!string.IsNullOrEmpty(cpsaWorkflowId))
                {
                    if (IsDynamic(cpsaWorkflowId))
                    {
                        FilterExpression childFilter = EntityList.LinkCriteria.AddFilter(LogicalOperator.Or);
                        childFilter.AddCondition("mzk_includeindynamicencountertemplates", ConditionOperator.Equal, (int)mzk_noyes.Yes);
                        IncludeActivityType(childFilter, cpsaWorkflowId);
                        IncludeMeasurementType(childFilter, cpsaWorkflowId);
                        IncludePendingOrders(childFilter, patientId);
                    }
                }

                query.LinkEntities.Add(EntityList);

                EntityCollection entitycol = repo.GetEntityCollection(query);

                foreach (Entity entity in entitycol.Entities)
                {
                    model = new UITemplate();

                    if (entity.Attributes.Contains("mzk_menulist.mzk_encountermenulist"))
                    {
                        model.controlName = entity.FormattedValues["mzk_menulist.mzk_encountermenulist"].ToString();
                    }

                    if (entity.Attributes.Contains("mzk_ordering"))
                    {
                        model.order = Convert.ToInt32(entity.Attributes["mzk_ordering"].ToString());
                    }
                    else
                    {
                        model.order = 0;
                    }

                    listModel.Add(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listModel;
        }

        public bool IsDynamic(string cpsaWorkflowId)
        {
            bool isDynamic = false;
            try
            {
                SoapEntityRepository repoCPSAW = SoapEntityRepository.GetService();
                EntityCollection entityCollection = null;
                QueryExpression query = new QueryExpression(mzk_activityworkflow.EntityLogicalName);
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_uitemplateselection");

                LinkEntity activityWorkflow = new LinkEntity()
                {
                    Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(false),
                    EntityAlias = "CPSAW",
                    LinkFromEntityName = mzk_activityworkflow.EntityLogicalName,
                    LinkToEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                    LinkFromAttributeName = "mzk_activityworkflowid",
                    LinkToAttributeName = "mzk_activityworkflow",
                    JoinOperator = JoinOperator.Inner
                };

                activityWorkflow.LinkCriteria.AddCondition("mzk_casepathwaystateactivityworkflowid", ConditionOperator.Equal, cpsaWorkflowId);

                query.LinkEntities.Add(activityWorkflow);

                entityCollection = repoCPSAW.GetEntityCollection(query);

                if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                {
                    mzk_activityworkflow aw = (mzk_activityworkflow)entityCollection.Entities[0];
                    mzk_uitemplateselection templateType = (mzk_uitemplateselection)aw.mzk_UItemplateselection.Value;
                    if (templateType == mzk_uitemplateselection.Dynamic)
                    {
                        isDynamic = true;
                    }
                }

                return isDynamic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void IncludeActivityType(FilterExpression filter, string cpsaWorkflowId)
        {
            SoapEntityRepository repoCPSAW = SoapEntityRepository.GetService();
            EntityCollection entityCollection = null;

            QueryExpression query = new QueryExpression(mzk_activitytype.EntityLogicalName);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_category");

            LinkEntity activityMaster = new LinkEntity()
            {
                Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(false),
                EntityAlias = "AM",
                LinkFromEntityName = mzk_activitytype.EntityLogicalName,
                LinkToEntityName = mzk_activitymaster.EntityLogicalName,
                LinkFromAttributeName = "mzk_activitytypeid",
                LinkToAttributeName = "mzk_activitytype",
                JoinOperator = JoinOperator.Inner
            };

            LinkEntity cpState = new LinkEntity()
            {
                Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(false),
                EntityAlias = "CPS",
                LinkFromEntityName = mzk_activitymaster.EntityLogicalName,
                LinkToEntityName = mzk_casepathwaystate.EntityLogicalName,
                LinkFromAttributeName = "mzk_activitymasterid",
                LinkToAttributeName = "mzk_activitymaster",
                JoinOperator = JoinOperator.Inner
            };

            LinkEntity cpsaWorkflow = new LinkEntity()
            {
                Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(false),
                EntityAlias = "CPSAW",
                LinkFromEntityName = mzk_casepathwaystate.EntityLogicalName,
                LinkToEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                LinkFromAttributeName = "mzk_casepathwaystateid",
                LinkToAttributeName = "mzk_casepathwaystate",
                JoinOperator = JoinOperator.Inner
            };

            cpsaWorkflow.LinkCriteria.AddCondition("mzk_casepathwaystateactivityworkflowid", ConditionOperator.Equal, cpsaWorkflowId);

            cpState.LinkEntities.Add(cpsaWorkflow);
            activityMaster.LinkEntities.Add(cpState);
            query.LinkEntities.Add(activityMaster);

            entityCollection = repoCPSAW.GetEntityCollection(query);

            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
            {
                mzk_activitytype activityType = (mzk_activitytype)entityCollection.Entities[0];
                mzk_activitycategory category = (mzk_activitycategory)activityType.mzk_Category.Value;
                if (category == mzk_activitycategory.RxOrder || category == mzk_activitycategory.RxAdmin)
                {
                    filter.AddCondition("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asMedication);
                }
                else if (category == mzk_activitycategory.TxOrder || category == mzk_activitycategory.TxAdmin)
                {
                    filter.AddCondition("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asOrderProcedure);
                }
                else if (category == mzk_activitycategory.LabOrder)
                {
                    filter.AddCondition("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asOrderLab);
                }
                else if (category == mzk_activitycategory.RadOrder)
                {
                    filter.AddCondition("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asRadiology);
                }
                else if (category == mzk_activitycategory.ReferralOrder)
                {
                    filter.AddCondition("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asReferral);
                }
                else if (category == mzk_activitycategory.Educate)
                {
                    filter.AddCondition("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asResources);
                }

            }
        }
        public void IncludeMeasurementType(FilterExpression filter, string cpsaWorkflowId)
        {
            SoapEntityRepository repoCPSAW = SoapEntityRepository.GetService();
            EntityCollection entityCollection = null;
            bool isSubjective = false;
            bool isObjective = false;

            QueryExpression query = new QueryExpression(mzk_casepathwaystateactivityworkflow.EntityLogicalName);
            query.Criteria.AddCondition("mzk_casepathwaystateactivityworkflowid", ConditionOperator.Equal, cpsaWorkflowId);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(false);

            LinkEntity cpState = new LinkEntity()
            {
                Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(false),
                EntityAlias = "CPS",
                LinkFromEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                LinkToEntityName = mzk_casepathwaystate.EntityLogicalName,
                LinkFromAttributeName = "mzk_casepathwaystate",
                LinkToAttributeName = "mzk_casepathwaystateid",
                JoinOperator = JoinOperator.Inner
            };

            LinkEntity cpsOutcome = new LinkEntity()
            {
                Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(false),
                EntityAlias = "CPSO",
                LinkFromEntityName = mzk_casepathwaystate.EntityLogicalName,
                LinkToEntityName = mzk_casepathwaystateoutcome.EntityLogicalName,
                LinkFromAttributeName = "mzk_casepathwaystateid",
                LinkToAttributeName = "mzk_casepathwaystate",
                JoinOperator = JoinOperator.Inner
            };

            LinkEntity mmtCode = new LinkEntity()
            {
                Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_datatype"),
                EntityAlias = "MMTCode",
                LinkFromEntityName = mzk_casepathwaystateoutcome.EntityLogicalName,
                LinkToEntityName = mzk_mmtcode.EntityLogicalName,
                LinkFromAttributeName = "mzk_measurement",
                LinkToAttributeName = "mzk_mmtcodeid",
                JoinOperator = JoinOperator.Inner
            };

            cpsOutcome.LinkEntities.Add(mmtCode);
            cpState.LinkEntities.Add(cpsOutcome);
            query.LinkEntities.Add(cpState);

            entityCollection = repoCPSAW.GetEntityCollection(query);

            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
            {
                mzk_mmtcodemzk_datatype dataType;
                foreach (Entity entity in entityCollection.Entities)
                {
                    if (entity.Attributes.Contains("MMTCode.mzk_datatype"))
                    {
                        dataType = (mzk_mmtcodemzk_datatype)((entity.Attributes["MMTCode.mzk_datatype"] as AliasedValue).Value as OptionSetValue).Value;
                        if (dataType == mzk_mmtcodemzk_datatype.Objective)
                            isObjective = true;
                        if (dataType == mzk_mmtcodemzk_datatype.Subjective)
                            isSubjective = true;

                    }
                }

                if (isSubjective)
                    filter.AddCondition("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asClinicalDocumentation);
                if (isObjective)
                    filter.AddCondition("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asVitalSigns);
            }
        }

        public void IncludePendingOrders(FilterExpression filter, string patientId) {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_patientorder.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                FilterExpression mainFilter = new FilterExpression(LogicalOperator.And);
                ConditionExpression condition = new ConditionExpression("mzk_customer", ConditionOperator.Equal, patientId);
                mainFilter.Conditions.Add(condition);

                FilterExpression childFilterOr = new FilterExpression(LogicalOperator.Or);
                
                childFilterOr.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_patientordermzk_Type.SpecialTest);
                childFilterOr.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_patientordermzk_Type.Procedure);
                FilterExpression childFilterAnd = new FilterExpression(LogicalOperator.And);
                childFilterAnd.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_urgency.Stat);
                childFilterAnd.AddCondition("mzk_urgency", ConditionOperator.Equal, (int)mzk_patientordermzk_Type.Medication);
                childFilterOr.AddFilter(childFilterAnd);

                mainFilter.AddFilter(childFilterOr);
                query.Criteria.AddFilter(mainFilter);


                LinkEntity entityTypeProduct = new LinkEntity(mzk_patientorder.EntityLogicalName, xrm.Product.EntityLogicalName, "mzk_productid", "productid", JoinOperator.Inner);

                entityTypeProduct.LinkCriteria.AddCondition("productstructure", ConditionOperator.Equal, (int)ProductProductStructure.Product);

                query.LinkEntities.Add(entityTypeProduct);


                LinkEntity EntityEncounter = new LinkEntity(mzk_patientorder.EntityLogicalName, mzk_patientencounter.EntityLogicalName, "mzk_patientencounterid", "mzk_patientencounterid", JoinOperator.Inner);

                EntityEncounter.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_appointment");
                EntityEncounter.EntityAlias = "mzk_patientencounter";

                LinkEntity EntityCase = new LinkEntity(mzk_patientencounter.EntityLogicalName, xrm.Incident.EntityLogicalName, "mzk_caseid", "incidentid", JoinOperator.Inner);
                EntityCase.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("customerid", "incidentid");
                EntityCase.EntityAlias = "mzk_case";
                EntityCase.LinkCriteria.AddCondition("mzk_casetype", ConditionOperator.Equal, (int)mzk_casetype.OutPatient);

                query.LinkEntities.Add(EntityEncounter);
                EntityEncounter.LinkEntities.Add(EntityCase);

                EntityCollection entitycol = repo.GetEntityCollection(query);

                foreach (Entity entity in entitycol.Entities)
                {
                    mzk_patientorder order = (mzk_patientorder)entity;

                    StatusManager statusManagerObj = new StatusManager(order.mzk_StatusManagerDetail.Id.ToString());
                    mzk_orderstatus status = (mzk_orderstatus)order.mzk_OrderStatus.Value;

                    if (statusManagerObj.getHierarchy())
                    {
                        List<ActionManager> actionList = statusManagerObj.getNextActionList();

                        if (!statusManagerObj.isFulfilmentAction())
                        {
                            if (actionList == null || actionList.Count == 0)
                            {
                                continue;
                            }
                            else
                            {
                                ConditionExpression innerCondition = null;

                                if (order.mzk_Type.Value == (int)mzk_patientordermzk_Type.Medication)
                                {
                                    innerCondition = new ConditionExpression("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asMedication);
                                }
                                else if (order.mzk_Type.Value == (int)mzk_patientordermzk_Type.Procedure)
                                {
                                    innerCondition = new ConditionExpression("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asOrderProcedure);
                                }
                                else if (order.mzk_Type.Value == (int)mzk_patientordermzk_Type.SpecialTest)
                                {
                                    innerCondition = new ConditionExpression("mzk_encountermenulist", ConditionOperator.Equal, (int)mzk_menulisttype.asSpecialTest);
                                }

                                if (innerCondition != null && !filter.Conditions.Contains(innerCondition))
                                {
                                    filter.AddCondition(innerCondition);
                                }
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string getEncounterTemplateId(string worklistTypeID, string cpsaWorkflowId, string resourceId = null, string patientId = null, string appointmentId = null)
        {
            string encounterTempalteId = string.Empty;

            if (string.IsNullOrEmpty(cpsaWorkflowId))
            {
                Appointment apptModel = new Appointment();
                User userModel = new User();

                List<Speciality> specialityList = userModel.getSpecialities(resourceId);
                string appointmentTypeId = string.Empty;
                bool isProgressive = false;
                
                if(!string.IsNullOrEmpty(patientId) && !string.IsNullOrEmpty(resourceId))
                {
                    isProgressive = Appointment.IsAppointmentProgressive(patientId, resourceId);
                }
                if (!string.IsNullOrEmpty(appointmentId))
                {
                    appointmentTypeId = apptModel.getScheduleType(appointmentId);
                }
                try
                {
                    SoapEntityRepository repo = SoapEntityRepository.GetService();
                    EntityCollection entitycollection = repo.GetEntityCollection(this.getEncounterTemplateQuery(worklistTypeID, specialityList, appointmentTypeId, isProgressive));

                    if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                    {
                        mzk_uitemplate uiTempalte = (mzk_uitemplate)entitycollection.Entities[0];

                        encounterTempalteId = uiTempalte.mzk_uitemplateId.Value.ToString();
                    }
                }
                catch (CustomException ex)
                {
                    throw ex;
                }
            }
            else
            {
                try
                {
                    SoapEntityRepository repoCPSAW = SoapEntityRepository.GetService();
                    QueryExpression query = new QueryExpression("mzk_casepathwaystateactivityworkflow");
                    query.Criteria.AddCondition("mzk_casepathwaystateactivityworkflowid", ConditionOperator.Equal, cpsaWorkflowId);
                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(false);

                    LinkEntity activityWorkflow = new LinkEntity()
                    {
                        Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_uitemplate"),
                        EntityAlias = "AW",
                        LinkFromEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                        LinkToEntityName = mzk_activityworkflow.EntityLogicalName,
                        LinkFromAttributeName = "mzk_activityworkflow",
                        LinkToAttributeName = "mzk_activityworkflowid",
                        JoinOperator = JoinOperator.Inner
                    };

                    query.LinkEntities.Add(activityWorkflow);

                    EntityCollection entityCollection = repoCPSAW.GetEntityCollection(query);
                    if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                    {
                        if (entityCollection[0].Attributes.Contains("AW.mzk_uitemplate"))
                            encounterTempalteId = ((entityCollection[0].Attributes["AW.mzk_uitemplate"] as AliasedValue).Value as EntityReference).Id.ToString();
                    }
                }
                catch (CustomException ex)
                {
                    throw ex;
                }
            }

            return encounterTempalteId;
        }
        private Microsoft.Xrm.Sdk.Query.QueryExpression getEncounterTemplateQuery(string worklistTypeID, List<Speciality> specialityList, string appointmentType, bool isProgressive)
        {
            QueryExpression query = new QueryExpression(mzk_uitemplate.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_uitemplateid");

            query.Criteria.AddCondition("mzk_type", ConditionOperator.Equal, 1);

            if (!string.IsNullOrEmpty(appointmentType))
            {
                FilterExpression childFilterSpecility = query.Criteria.AddFilter(LogicalOperator.Or);

                childFilterSpecility.AddCondition("mzk_appointmenttype", ConditionOperator.Equal, new Guid(appointmentType));

                childFilterSpecility.AddCondition("mzk_appointmenttype", ConditionOperator.Null);

            }
            if (!string.IsNullOrEmpty(worklistTypeID))
            {
                query.Criteria.AddCondition("mzk_worklisttypeid", ConditionOperator.Equal, new Guid(worklistTypeID));

                if (mzk_worklisttypemzk_Type.todaysConsultation == new Worklist().getWorklistType(worklistTypeID))
                {
                    query.Criteria.AddCondition("mzk_progressive", ConditionOperator.Equal, isProgressive);
                }
            }

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);

            foreach (Speciality entity in specialityList)
            {
                if (entity.SpecialityId != null && entity.SpecialityId != string.Empty)
                {
                    childFilter.AddCondition("mzk_specialityid", ConditionOperator.Equal, new Guid(entity.SpecialityId));
                }

                childFilter.AddCondition("mzk_specialityid", ConditionOperator.Null);
            }

            return query;
        }
        public async Task<bool> updateEncounter(string EncounterId, string value, string userId, bool isAutoSignoff = false)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Entity encounterEntity = new Entity("mzk_patientencounter");

                if (!string.IsNullOrEmpty(EncounterId))
                {
                    encounterEntity = entityRepository.GetEntity("mzk_patientencounter", new Guid(EncounterId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_encounterstatus", "mzk_encountertype", "mzk_caseid", "mzk_casepathwaystateactivityworkflow", "mzk_appointment"));
                }
                mzk_casetype caseType = PatientCase.getCaseType(EncounterId);
                mzk_encountertype enctype = (mzk_encountertype)((OptionSetValue)encounterEntity.Attributes["mzk_encountertype"]).Value;

                if (!string.IsNullOrEmpty(value))
                {
                    string caseId = ((EntityReference)encounterEntity.Attributes["mzk_caseid"]).Id.ToString();

                    if (caseType == mzk_casetype.Emergency)
                    {
                        PatientCase patientCase = new PatientCase();
                        if (enctype == mzk_encountertype.PrimaryAssessment)
                        {
                            PatientDiagnosis pp = new PatientDiagnosis();
                            PatientDisposition pd = new PatientDisposition();

                            if (pd.getPatientDisposition(EncounterId, "", "", DateTime.MinValue, DateTime.MinValue).Result.Count > 0)
                            {
                                int outComeVale = pd.getPatientDisposition(EncounterId, "", "", DateTime.MinValue, DateTime.MinValue).Result[0].OutComeValue;
                                if (outComeVale == (int)mzk_dispositionmzk_Outcome.Admitted || outComeVale == (int)mzk_dispositionmzk_Outcome.DischargeAgainstMedicalAdvice_DAMA
                                 || outComeVale == (int)mzk_dispositionmzk_Outcome.Transferred || outComeVale == (int)mzk_dispositionmzk_Outcome.TreatedReleased_Home)
                                {
                                    if (pp.getEncounterDiagnosis(EncounterId).Count < 1)
                                    {
                                        throw new ValidationException("Diagnosis is mandatory before sign off");
                                    }
                                    else
                                    {
                                        await patientCase.markClinicalDischarge(caseId);

                                        encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                                        entityRepository.UpdateEntity(encounterEntity);
                                    }
                                }
                                else
                                {
                                    if (outComeVale == 0)
                                    {
                                        throw new ValidationException("Outcome is  mandatory in Disposition Form");
                                    }
                                    await patientCase.markClinicalDischarge(caseId);

                                    encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                                    entityRepository.UpdateEntity(encounterEntity);
                                }
                            }
                            else
                            {
                                throw new ValidationException("Dispostion is mandatory before sign off");
                            }
                        }
                        else if (enctype == mzk_encountertype.Treatment)
                        {
                            await patientCase.markPhysicalDischarge(caseId);

                            encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                            entityRepository.UpdateEntity(encounterEntity);
                        }
                        else
                        {
                            encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                            entityRepository.UpdateEntity(encounterEntity);
                        }
                    }
                    else if (caseType == mzk_casetype.OutPatient)
                    {                      
                            List<PatientVitals> vitalsList = new List<PatientVitals>();
                            string mzk_casepathwaystateactivityworkflowID = "";

                        //Activity engine code start.
                        if (encounterEntity.Attributes.Contains("mzk_casepathwaystateactivityworkflow"))
                        {
                            mzk_casepathwaystateactivityworkflowID = ((EntityReference)encounterEntity.Attributes["mzk_casepathwaystateactivityworkflow"]).Id.ToString();

                            Entity mzk_casepathwaystateactivityworkflowIDEntity = this.GetCasePathwayStateActivityWorkflowByID(mzk_casepathwaystateactivityworkflowID);
                            string currentState = "";
                            if (mzk_casepathwaystateactivityworkflowIDEntity.Attributes.Contains("mzk_casepathwaystate"))
                            {
                                currentState = ((EntityReference)mzk_casepathwaystateactivityworkflowIDEntity.Attributes["mzk_casepathwaystate"]).Id.ToString();
                            }

                            if (!string.IsNullOrEmpty(EncounterId))
                            {
                                vitalsList = new PatientVitals().getPatientEncounterVitals(null, EncounterId, false, true).Result.ToList();

                                //ClinicalTemplate clinicTemplate = new ClinicalTemplate();
                                //List<ClinicalTemplateNarration> patientNarrationList = clinicTemplate.getPatientsClinicalTempalteNarration("", EncounterId, "", false, 0, true,"", currentState);

                                if (vitalsList.Count != 0)
                                {
                                    if (vitalsList.Any(item => item.CasepathwayStateOutcomeId != string.Empty && item.CasepathwayStateOutcomeId != null && item.valueUpdated == false))
                                    {
                                        throw new ValidationException("Outcome is  mandatory before sign off");
                                    }
                                }

                                ClinicalTemplate clinicTemplate = new ClinicalTemplate();
                                List<ClinicalTemplateNarration> listModel = new List<ClinicalTemplateNarration>();
                                listModel = clinicTemplate.getPatientsClinicalTempalteNarration("", EncounterId, "", false, 0, false, "", currentState);
                                if (listModel.Count < 1)
                                {
                                    if (enctype == mzk_encountertype.Consultation)
                                    {
                                        throw new ValidationException("Clinical documentation is mandatory before sign off");
                                    }
                                }
                                else
                                {
                                    //check model.ansIn is not empty in any
                                    bool throwValidationError = false;
                                    foreach (ClinicalTemplateNarration model in listModel)
                                    {
                                        if (model.ansIn == null || model.ansIn == string.Empty)
                                        {
                                            throwValidationError = true;
                                        }
                                    }
                                    if (throwValidationError)
                                    {
                                        throw new ValidationException("Clinical documentation is mandatory before sign off");
                                    }
                                }
                                this.updateEncounterCasePathwayOutcomes(mzk_casepathwaystateactivityworkflowID, vitalsList, listModel);
                            }

                        }
                        //Activity engine code end.

                        if (enctype == mzk_encountertype.Consultation)
                        {
                            if (isAutoSignoff)
                            {
                                encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                                entityRepository.UpdateEntity(encounterEntity);
                            }
                            else
                            {
                                ClinicalTemplate clinicTemplate = new ClinicalTemplate();
                                List<ClinicalTemplateNarration> listModel = new List<ClinicalTemplateNarration>();
                                listModel = clinicTemplate.getPatientsClinicalTempalteNarration("", EncounterId, "", false, 0, true);
                                
                                if (listModel.Count < 1)
                                {
                                    throw new ValidationException("Clinical documentation is mandatory before sign off");
                                }

                                encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                                entityRepository.UpdateEntity(encounterEntity);
                            }
                            //else
                            //{
                            //    PatientDiagnosis pp = new PatientDiagnosis();
                            //    PatientDisposition pd = new PatientDisposition();

                            //    if (pd.getPatientDisposition(EncounterId, "", "", DateTime.MinValue, DateTime.MinValue).Result.Count > 0)
                            //    {
                            //        int outComeVale = pd.getPatientDisposition(EncounterId, "", "", DateTime.MinValue, DateTime.MinValue).Result[0].OutComeValue;
                            //        if (outComeVale == (int)mzk_dispositionmzk_Outcome.Admitted || outComeVale == (int)mzk_dispositionmzk_Outcome.DischargeAgainstMedicalAdvice_DAMA
                            //         || outComeVale == (int)mzk_dispositionmzk_Outcome.Transferred || outComeVale == (int)mzk_dispositionmzk_Outcome.TreatedReleased_Home)
                            //        {
                            //            if (pp.getEncounterDiagnosis(EncounterId).Count < 1)
                            //            {
                            //                throw new ValidationException("Diagnosis is mandatory before sign off");
                            //            }
                            //            else
                            //            {
                            //                encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                            //                entityRepository.UpdateEntity(encounterEntity);
                            //            }
                            //        }
                            //        else
                            //        {
                            //            encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                            //            entityRepository.UpdateEntity(encounterEntity);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        throw new ValidationException("Dispostion is mandatory before sign off");
                            //    }
                            //}
                        }
                        else
                        {
                            encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                            entityRepository.UpdateEntity(encounterEntity);
                        }
                    }
                    else
                    {
                        encounterEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                        entityRepository.UpdateEntity(encounterEntity);
                    }

                    new ConsultationSummary().getConsultationSummary(EncounterId, true);

                    Entity encounterLogEntity = new Entity("mzk_patientencounterlog");
                    //encounterLogEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                    encounterLogEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(EncounterId));
                    if (isAutoSignoff == true)
                    {
                        encounterLogEntity.Attributes["mzk_message"] = "Done";
                    }
                    encounterLogEntity.Attributes["mzk_autosignoff"] = isAutoSignoff;
                    entityRepository.CreateEntity(encounterLogEntity);

                    if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                    {
                        if (Convert.ToInt32(mzk_encounterstatus.Signed) == Convert.ToInt32(value))
                        {
                            if (enctype == mzk_encountertype.Consultation && (encounterEntity.Attributes.Contains("mzk_appointment") || !string.IsNullOrEmpty(caseId)))
                            {
                                string appoitnmentId = string.Empty;

                                if (encounterEntity.Attributes.Contains("mzk_appointment"))
                                {
                                    appoitnmentId = (encounterEntity.Attributes["mzk_appointment"] as EntityReference).Id.ToString();
                                }
                                
                                CaseRepository caseRepo = new CaseRepository(); //Need to work
                                //caseRepo.updateCaseTransStatus(0, HMServiceStatus.Complete, true, axAppoitnmentRefId, "", caseId);                                
                            }
                            else if (mzk_encountertype.PrimaryAssessment == enctype && !string.IsNullOrEmpty(caseId))
                            {
                                CaseRepository caseRepo = new CaseRepository();
                                caseRepo.updateCaseTransStatus(0, HMServiceStatus.Complete, true, 0, "", caseId);
                            }
                            else if (mzk_encountertype.Assessment == enctype && !string.IsNullOrEmpty(userId))
                            {
                                PatientReferralOrder referral = new PatientReferralOrder().referralExist(caseId, userId);
                                if (referral != null && !string.IsNullOrEmpty(referral.CaseTransRecId))
                                {
                                    CaseRepository caseRepo = new CaseRepository();
                                    long caseTransRecId = (long)Convert.ToDecimal(referral.CaseTransRecId);

                                    caseRepo.updateCaseTransStatus(caseTransRecId, HMServiceStatus.Complete, true, 0, "", "");
                                }
                            }
                        }
                    }

                    PatientDiagnosis patientDiagnosis = new PatientDiagnosis();
                    Task<List<PatientDiagnosis>> pd1 = patientDiagnosis.getPatientDiagnosis(EncounterId, "", "", DateTime.MinValue, DateTime.MinValue, 0, "");

                    for (int diagnosisCount = 0; diagnosisCount < pd1.Result.Count; diagnosisCount++)
                    {
                        if (pd1.Result[diagnosisCount].IsProblem == "1")
                        {
                            PatientProblem patientProblem = new PatientProblem();

                            patientProblem.Chronicity = pd1.Result[diagnosisCount].Chronicity;
                            patientProblem.ProblemType = pd1.Result[diagnosisCount].ProblemType;
                            patientProblem.onSetNotes = pd1.Result[diagnosisCount].OnsetNotes;
                            patientProblem.problemId = pd1.Result[diagnosisCount].DiagnosisId;
                            patientProblem.onSetDate = pd1.Result[diagnosisCount].OnsetDate;
                            patientProblem.PatientId = pd1.Result[diagnosisCount].PatientId;

                            Task<string> id = patientProblem.addPatientProblem(patientProblem);
                        }
                    }                    
                }
                else
                {
                    throw new Exception("null value");
                }
                return true;
            }
            catch (Exception ex)
            {
                Entity encounterLogEntity = new Entity("mzk_patientencounterlog");
                // encounterLogEntity.Attributes["mzk_encounterstatus"] = new OptionSetValue(Convert.ToInt16(value));
                encounterLogEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(EncounterId));
                encounterLogEntity.Attributes["mzk_autosignoff"] = isAutoSignoff;
                encounterLogEntity.Attributes["mzk_message"] = ex.Message;
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                entityRepository.CreateEntity(encounterLogEntity);
                throw ex;
            }
        }

        public async Task<bool> updateSummaryJson(string EncounterId, string json)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            Entity encounterEntity = new Entity("mzk_patientencounter");

            if (!string.IsNullOrEmpty(EncounterId))
                encounterEntity = entityRepository.GetEntity("mzk_patientencounter", new Guid(EncounterId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_jsonresponce", "mzk_summaryupdatedtime"));

            if (!string.IsNullOrEmpty(json))
            {
                encounterEntity.Attributes["mzk_jsonresponce"] = json;
                encounterEntity.Attributes["mzk_summaryupdatedtime"] = DateTime.Now;
            }

            entityRepository.UpdateEntity(encounterEntity);

            return true;
        }

        public async Task<PatientEncounter> encounterDetails(int encounterType, string userId, string activityWorkflow = "", string appointmentId = "")
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            PatientEncounter patientEncounter = new PatientEncounter();
            mzk_patientencounter patientencounterEntity;

            QueryExpression query = new QueryExpression(mzk_patientencounter.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            if (!string.IsNullOrEmpty(appointmentId))
            {
                query.Criteria.AddCondition("mzk_appointment", ConditionOperator.Equal, new Guid(appointmentId));
            }
            if (encounterType > 0)
            {
                query.Criteria.AddCondition("mzk_encountertype", ConditionOperator.Equal, encounterType);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query.Criteria.AddCondition("createdby", ConditionOperator.Equal, new Guid(userId));
            }

            if (!string.IsNullOrEmpty(activityWorkflow))
            {
                query.Criteria.AddCondition("mzk_casepathwaystateactivityworkflow", ConditionOperator.Equal, new Guid(activityWorkflow));
            }

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {
                patientencounterEntity = (mzk_patientencounter)entitycollection.Entities[0];

                patientEncounter.EncounterId = patientencounterEntity.mzk_patientencounterId.Value.ToString();
                patientEncounter.EncounterStatusValue = patientencounterEntity.mzk_encounterstatus != null ? patientencounterEntity.mzk_encounterstatus.Value : 0;
            }

            return patientEncounter;
        }

        public async Task<List<PatientEncounter>> encounterDetails(PatientEncounter encounter)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            List<PatientEncounter> patientEncounter = new List<PatientEncounter>();
            QueryExpression query = new QueryExpression(mzk_patientencounter.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            //Search Order
            if (!string.IsNullOrEmpty(encounter.EncounterType))
            {
                if (encounter.EncounterType == "1")
                    query.Criteria.AddCondition("mzk_encountertype", ConditionOperator.Equal, (int)mzk_encountertype.Triage);
                else
                    query.Criteria.AddCondition("mzk_encountertype", ConditionOperator.Equal, Convert.ToInt32(encounter.EncounterType));
            }

            if (!string.IsNullOrEmpty(encounter.SearchFilters))
            {
                if (encounter.SearchFilters == Convert.ToString(mzk_encounterfilter.Open))
                    query.Criteria.AddCondition("mzk_encounterstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_encounterstatus.Open));
                if (encounter.SearchFilters == Convert.ToString(mzk_encounterfilter.Signed))
                    query.Criteria.AddCondition("mzk_encounterstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_encounterstatus.Signed));
            }
            //Search Order
            if (!string.IsNullOrEmpty(encounter.searchEncounter))
                query.Criteria.AddCondition("mzk_encounterno", ConditionOperator.Like, ("%" + encounter.searchEncounter + "%"));


            if (!string.IsNullOrEmpty(encounter.CaseId) && (!string.IsNullOrEmpty(encounter.EncounterId)))
            {
                query.Criteria.AddCondition("mzk_caseid", ConditionOperator.Equal, encounter.CaseId);
                query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.NotEqual, encounter.EncounterId);
            }

            if (!string.IsNullOrEmpty(encounter.EncounterId) && string.IsNullOrEmpty(encounter.CaseId))
                query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, encounter.EncounterId);

            if (!string.IsNullOrEmpty(encounter.CaseId))
                query.Criteria.AddCondition("mzk_caseid", ConditionOperator.Equal, encounter.CaseId);


            if (!string.IsNullOrEmpty(encounter.AppointmentId))
            {
                query.Criteria.AddCondition("mzk_appointment", ConditionOperator.Equal, new Guid(encounter.AppointmentId));
            }  
                
            OrderExpression orderby = new OrderExpression();
            orderby.AttributeName = "createdon";
            orderby.OrderType = OrderType.Descending;

            query.Orders.Add(orderby);

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {
                foreach (mzk_patientencounter patientencounterEntity in entitycollection.Entities)
                {
                    PatientEncounter model = new PatientEncounter();
                    if (patientencounterEntity.mzk_allergyReviewStatus == true)
                        model.AllergyReviewStatus = "1";
                    else
                        model.AllergyReviewStatus = "0";

                    //EcnounterNumber
                    if (patientencounterEntity.Attributes.Contains("mzk_encounterno"))
                        model.EncounterNo = patientencounterEntity.mzk_EncounterNo.ToString();
                    //Encounter Id
                    model.EncounterId = patientencounterEntity.Id.ToString();
                    //Encounter Status
                    if (patientencounterEntity.Attributes.Contains("mzk_encounterstatus"))
                    {
                        model.EncounterStatusValue = ((OptionSetValue)patientencounterEntity.mzk_encounterstatus).Value;
                        model.EncounterStatus = patientencounterEntity.FormattedValues["mzk_encounterstatus"].ToString();
                    }

                    if (patientencounterEntity.Attributes.Contains("mzk_encountertype"))
                    {
                        model.EncounterType = ((OptionSetValue)patientencounterEntity.mzk_EncounterType).Value.ToString();
                        model.EncounterTypeName = patientencounterEntity.FormattedValues["mzk_encountertype"].ToString();

                        mzk_encountertype encType = (mzk_encountertype)Convert.ToInt32(model.EncounterType);
                        model.isConsultation = encType == mzk_encountertype.Consultation;
                    }

                    if (patientencounterEntity.Attributes.Contains("mzk_jsonresponce"))
                    {
                        model.SummaryJson = patientencounterEntity.Attributes["mzk_jsonresponce"].ToString();
                    }
                    //CreateOn
                    if (patientencounterEntity.Attributes.Contains("createdon"))
                        model.CreatedOn = Convert.ToDateTime(patientencounterEntity.CreatedOn);
                    //CreatedBy
                    if (patientencounterEntity.Attributes.Contains("createdby"))
                    {
                        model.CreatedBy = patientencounterEntity.CreatedBy.Name;
                        model.CreatedById = patientencounterEntity.CreatedBy.Id.ToString();
                    }

                    if (patientencounterEntity.Attributes.Contains("mzk_appointment"))
                        model.AppointmentId = patientencounterEntity.mzk_appointment.Id.ToString();

                    if (patientencounterEntity.Attributes.Contains("mzk_caseid"))
                        model.CaseId = patientencounterEntity.mzk_caseid.Id.ToString();

                    Entity logEntity = this.GetEncounterLog(encounter.EncounterId);

                    if (logEntity != null)
                        model.SignedOn = Convert.ToDateTime(logEntity.Attributes["createdon"]);
                    //CreatedBy
                    if (logEntity != null)
                        model.SignedBy = ((EntityReference)logEntity.Attributes["createdby"]).Name;

                    //AllergyReviewedOn
                    if (patientencounterEntity.Attributes.Contains("mzk_allergyreviewedon"))
                        model.AllergyReviewedOn = Convert.ToDateTime(patientencounterEntity.Attributes["mzk_allergyreviewedon"]);
                    //AllergyReviewedByName
                    if (patientencounterEntity.Attributes.Contains("mzk_allergyreviewedbyid"))
                        model.AllergyReviewedByName = ((EntityReference)patientencounterEntity.Attributes["mzk_allergyreviewedbyid"]).Name;

                    if (patientencounterEntity.Attributes.Contains("mzk_encountertemplate"))
                        model.EncounterTemplateId = ((EntityReference)patientencounterEntity.mzk_EncounterTemplate).Id.ToString();

                    if (patientencounterEntity.Attributes.Contains("mzk_rxnumber"))
                        model.RxNumber = patientencounterEntity.Attributes["mzk_rxnumber"].ToString();

                    if (patientencounterEntity.Attributes.Contains("mzk_instructiontonurse"))
                        model.Instruction = patientencounterEntity.Attributes["mzk_instructiontonurse"].ToString();

                    if (patientencounterEntity.Attributes.Contains("mzk_customer"))
                        model.PatientId = ((EntityReference)patientencounterEntity.Attributes["mzk_customer"]).Id.ToString();

                    mzk_casetype caseType = PatientCase.getCaseType("", model.CaseId);

                    model.caseTypeValue = caseType;
                    model.diagnosisRequired = CaseParameter.getDiagnosisRequired(caseType);

                    patientEncounter.Add(model);
                }
            }

            return patientEncounter;
        }

        public bool DuplicateDetection(string EncounterId, string ProductId, string specimenSourceId = null)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(mzk_patientorder.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, EncounterId);
            query.Criteria.AddCondition("mzk_productid", ConditionOperator.Equal, ProductId);
            query.Criteria.AddCondition("mzk_orderstatus", ConditionOperator.Equal, (int)mzk_orderstatus.Ordered);
            if (!string.IsNullOrEmpty(specimenSourceId))
                query.Criteria.AddCondition("mzk_specimensource", ConditionOperator.Equal, specimenSourceId);

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                return false;
            else
                return true;
        }
        public async Task<bool> UpdateEncounterAllergies(PatientEncounter patientEncounter)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            Entity encounterEntity = new Entity("mzk_patientencounter");

            if (!string.IsNullOrEmpty(patientEncounter.EncounterId))
                encounterEntity = entityRepository.GetEntity("mzk_patientencounter", new Guid(patientEncounter.EncounterId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_encounterstatus", "mzk_allergyreviewstatus", "mzk_allergyreviewedon"));

            if (!string.IsNullOrEmpty(patientEncounter.AllergyReviewStatus) && patientEncounter.AllergyReviewStatus == "1")
                encounterEntity.Attributes["mzk_allergyreviewstatus"] = true;
            else
                encounterEntity.Attributes["mzk_allergyreviewstatus"] = false;

            if (!string.IsNullOrEmpty(patientEncounter.AllergyReviewedById))
                encounterEntity.Attributes["mzk_allergyreviewedbyid"] = new EntityReference("systemuser", new Guid(patientEncounter.AllergyReviewedById));

            encounterEntity.Attributes["mzk_allergyreviewedon"] = DateTime.Now;

            entityRepository.UpdateEntity(encounterEntity);

            return true;
        }
        public Entity GetEncounterLog(string encounterId)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(mzk_patientencounterlog.EntityLogicalName);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            if (!string.IsNullOrEmpty(encounterId))
            {
                query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, encounterId);
                query.Criteria.AddCondition("mzk_encounterstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_encounterstatus.Signed));
            }

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            if (entitycollection.Entities.Count > 0)
            {
                return entitycollection.Entities[0];
            }
            else
            {
                return null;
            }
        }
        //public EntityCollection GetEncounterSONotesFinding(string encounterId)
        //{
        //    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
        //    QueryExpression query = new QueryExpression(mzk_patientsonotesfinding.EntityLogicalName);
        //    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

        //    if (!string.IsNullOrEmpty(encounterId))
        //    {
        //        LinkEntity entityTypePatientSONotesTemplate = new LinkEntity(mzk_patientsonotesfinding.EntityLogicalName, mzk_patientsonotestemplate.EntityLogicalName, "mzk_patientsonotestemplate", "mzk_patientsonotestemplateid", JoinOperator.Inner);
        //        entityTypePatientSONotesTemplate.LinkCriteria.AddCondition("mzk_patientencounter", ConditionOperator.Equal, new Guid(encounterId));
        //        query.LinkEntities.Add(entityTypePatientSONotesTemplate);
        //    }

        //    EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
        //    return entitycollection;

        //}

        public Entity GetCasePathwayStateActivityWorkflowByID(string mzk_casepathwaystateactivityworkflowID)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(mzk_casepathwaystateactivityworkflow.EntityLogicalName);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            query.Criteria.AddCondition("mzk_casepathwaystateactivityworkflowid", ConditionOperator.Equal, new Guid(mzk_casepathwaystateactivityworkflowID));

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            if (entitycollection.Entities.Count > 0)
            {
                return entitycollection.Entities[0];
            }
            else
            {
                return null;
            }

        }

        public bool updateEncounterCasePathwayOutcomes(string mzk_casepathwaystateactivityworkflowStr, List<PatientVitals> vitalsList, List<ClinicalTemplateNarration> patientNarrationList)
        {
            try
            {
                if (vitalsList != null && vitalsList.Count() > 0)
                {
                    foreach (PatientVitals PatientVitalsClass in vitalsList)
                    {
                        this.updateCasePathwayStateOutcomeWithValues(PatientVitalsClass.CasepathwayStateOutcomeId, PatientVitalsClass.MeasurementValue, PatientVitalsClass.MeasurementValue2);
                    }
                }

                if (patientNarrationList != null && patientNarrationList.Count() > 0)
                {
                    foreach (ClinicalTemplateNarration PatientVitalsClass in patientNarrationList)
                    {
                        int isPresentResult = 0;

                        if (PatientVitalsClass.narrationType == (int)mzk_narrationtype.NoSelection ||
                            PatientVitalsClass.narrationType == (int)mzk_narrationtype.Positive)
                        {
                            isPresentResult = (int)mzk_ispresent.Present;
                        }
                        else if (PatientVitalsClass.narrationType == (int)mzk_narrationtype.Negative)
                        {
                            isPresentResult = (int)mzk_ispresent.Absent;
                        }


                        this.updateCasePathwayStateOutcomeWithValues(PatientVitalsClass.mzk_casepathwaystateoutcomeStr, 0, 0, "IsPresent", isPresentResult);
                    }

                }

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Entity mzk_casepathwaystateactivityworkflowEntity = entityRepository.GetEntity("mzk_casepathwaystateactivityworkflow", new Guid(mzk_casepathwaystateactivityworkflowStr), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                mzk_casepathwaystateactivityworkflowEntity.Attributes["mzk_status"] = new OptionSetValue((int)mzk_casepathwaystatus.Completed);
                entityRepository.UpdateEntity(mzk_casepathwaystateactivityworkflowEntity);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return false;
        }

        public bool updateCasePathwayStateOutcomeWithValues(string CasePathwayStateOutcomeID, double val1, double val2, string isPresent = "", int mzk_ispresentVal = 0)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            mzk_casepathwaystateoutcome mzk_casepathwaystateoutcomeEntity = new mzk_casepathwaystateoutcome();

            if (!string.IsNullOrEmpty(CasePathwayStateOutcomeID))
            {
                mzk_casepathwaystateoutcomeEntity = (mzk_casepathwaystateoutcome)entityRepository.GetEntity("mzk_casepathwaystateoutcome", new Guid(CasePathwayStateOutcomeID), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));


                mzk_casepathwaystateoutcomeEntity.Attributes["mzk_recordedlowerrange"] = Convert.ToDouble(val1);
                mzk_casepathwaystateoutcomeEntity.Attributes["mzk_recordedupperrange"] = Convert.ToDouble(val2);

                if (!string.IsNullOrEmpty(isPresent) && mzk_ispresentVal > 0)
                {
                    mzk_casepathwaystateoutcomeEntity.mzk_RecordedIsPresent = new OptionSetValue(mzk_ispresentVal);
                }
                entityRepository.UpdateEntity(mzk_casepathwaystateoutcomeEntity);
            }
            return true;
        }
        public async Task<bool> updateNurseInstruction(string encounterId, string Instruction)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Entity encounterEntity = entityRepository.GetEntity("mzk_patientencounter", new Guid(encounterId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_instructiontonurse"));
                encounterEntity.Attributes["mzk_instructiontonurse"] = Instruction;
                entityRepository.UpdateEntity(encounterEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<bool> ConsultationComplete(string EncounterId)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            Entity encounterEntity = new Entity("mzk_patientencounter");

            try
            {
                if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                {
                    encounterEntity = entityRepository.GetEntity("mzk_patientencounter", new Guid(EncounterId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_encounterstatus", "mzk_appointment", "mzk_caseid"));
                    
                    string caseId = string.Empty;

                    if (encounterEntity.Attributes.Contains("mzk_caseid"))
                    {
                        caseId = (encounterEntity.Attributes["mzk_caseid"] as EntityReference).Id.ToString();

                        CaseRepository caseRepo = new CaseRepository();

                        caseRepo.updateCaseTransStatus(0, HMServiceStatus.Complete, true, 0, "", caseId);
                    }
                }               

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool checkRXNumber(string EncounterId)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            List<PatientEncounter> patientEncounter = new List<PatientEncounter>();
            QueryExpression query = new QueryExpression(mzk_patientencounter.EntityLogicalName);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_rxnumber");

            if (!string.IsNullOrEmpty(EncounterId))
                query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(EncounterId));

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            if (entitycollection.Entities.Count > 0)
            {
                if (entitycollection.Entities[0].Attributes.Contains("mzk_rxnumber"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public string getRXNuber(string EncounterId)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            List<PatientEncounter> patientEncounter = new List<PatientEncounter>();
            QueryExpression query = new QueryExpression(mzk_patientencounter.EntityLogicalName);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_rxnumber");

            if (!string.IsNullOrEmpty(EncounterId))
            {
                query.Criteria.AddCondition("mzk_rxnumber", ConditionOperator.NotNull);
            }

            OrderExpression orderby = new OrderExpression();
            orderby.AttributeName = "createdon";
            orderby.OrderType = OrderType.Descending;
            query.Orders.Add(orderby);

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            if (entitycollection.Entities.Count > 0)
            {
                if (entitycollection.Entities[0].Attributes.Contains("mzk_rxnumber"))
                    return entitycollection.Entities[0].Attributes["mzk_rxnumber"].ToString();
                else
                    return "0";
            }
            else
            {
                return "0";
            }
        }
        public bool updateRxNumber(string EncounterId, string rxumber)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            Entity encounterEntity = new Entity("mzk_patientencounter");

            if (!string.IsNullOrEmpty(EncounterId))
                encounterEntity = entityRepository.GetEntity("mzk_patientencounter", new Guid(EncounterId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_rxnumber"));

            if (!string.IsNullOrEmpty(rxumber))
            {
                encounterEntity.Attributes["mzk_rxnumber"] = rxumber;
            }
            entityRepository.UpdateEntity(encounterEntity);

            return true;
        }
        public async Task<List<PatientEncounter>> GetEncounterList(string patientguid, string SpecialityId, int caseType, int encounterType)
        {
            List<PatientEncounter> PatientEncounter = new List<PatientEncounter>();
            #region Patient Patient Case Query
            QueryExpression query = new QueryExpression("incident");

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            if (!string.IsNullOrEmpty(patientguid))
            {
                childFilter.AddCondition("customerid", ConditionOperator.Equal, new Guid(patientguid));
            }
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("ticketnumber");
            if (caseType != 0)
                childFilter.AddCondition("mzk_casetype", ConditionOperator.Equal, caseType);

            
            if (!string.IsNullOrEmpty(SpecialityId))
            {
                LinkEntity clinic = new LinkEntity("incident", "mzk_organizationalunit", "mzk_organizationalunit", "mzk_organizationalunitid", JoinOperator.Inner);
                clinic.LinkCriteria.AddCondition("mzk_speciality", ConditionOperator.Equal, new Guid(SpecialityId));
                clinic.EntityAlias = "clinic";
                query.LinkEntities.Add(clinic);
            }            

            LinkEntity EntityEncounter = new LinkEntity("incident", "mzk_patientencounter", "incidentid", "mzk_caseid", JoinOperator.Inner);
            EntityEncounter.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_encounterno", "mzk_patientencounterid", "createdon", "createdby");
            EntityEncounter.EntityAlias = "Encounter";
            if (encounterType != 0 && caseType == (int)mzk_casetype.OutPatient)
            {
                EntityEncounter.LinkCriteria.AddCondition("mzk_encountertype", ConditionOperator.Equal, encounterType);
            }
            else
            {
                if (caseType == (int)mzk_casetype.OutPatient)
                    EntityEncounter.LinkCriteria.AddCondition("mzk_encountertype", ConditionOperator.NotEqual, (int)mzk_encountertype.Consultation);
            }

            LinkEntity EntityUser = new LinkEntity("mzk_patientencounter", "systemuser", "createdby", "systemuserid", JoinOperator.Inner);
            EntityUser.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_axresourcerefrecid");
            if (encounterType != 0 && caseType == (int)mzk_casetype.OutPatient)
            {
                EntityUser.LinkCriteria.AddCondition("mzk_resourcecategory", ConditionOperator.Equal, (int)SystemUsermzk_ResourceCategory.Medical);
            }
            else
            {
                if (caseType == (int)mzk_casetype.OutPatient)
                    EntityUser.LinkCriteria.AddCondition("mzk_resourcecategory", ConditionOperator.Equal, (int)SystemUsermzk_ResourceCategory.Clinical);
            }
            EntityUser.EntityAlias = "User";

            OrderExpression orderby = new OrderExpression();
            orderby.AttributeName = "createdon";
            orderby.OrderType = OrderType.Descending;

            query.Orders.Add(orderby);
            query.LinkEntities.Add(EntityEncounter);
            EntityEncounter.LinkEntities.Add(EntityUser);
            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                PatientEncounter model = new PatientEncounter();

                if (entity.Attributes.Contains("ticketnumber"))
                    model.CaseNumber = entity["ticketnumber"].ToString();
                if (entity.Attributes.Contains("Encounter.mzk_encounterno"))
                    model.EncounterNo = (entity.Attributes["Encounter.mzk_encounterno"] as AliasedValue).Value.ToString();
                if (entity.Attributes.Contains("Encounter.mzk_patientencounterid"))
                    model.EncounterId = (entity.Attributes["Encounter.mzk_patientencounterid"] as AliasedValue).Value.ToString();
                if (entity.Attributes.Contains("Encounter.createdon"))
                    model.CreatedOn = ((DateTime)(entity.Attributes["Encounter.createdon"] as AliasedValue).Value);

                if (entity.Attributes.Contains("Encounter.createdby"))
                    model.CreatedBy = ((EntityReference)(entity.Attributes["Encounter.createdby"] as AliasedValue).Value).Name;

                if (entity.Attributes.Contains("User.mzk_axresourcerefrecid"))
                    model.AXRefrecid = (decimal)(entity.Attributes["User.mzk_axresourcerefrecid"] as AliasedValue).Value;

                if (entity.Attributes.Contains("incidentid"))
                    model.CaseId = entity.Id.ToString();

                if (!string.IsNullOrEmpty(SpecialityId))
                {                                            
                        PatientEncounter.Add(model);   
                }
                else
                {
                    PatientEncounter.Add(model);
                }
            }
            return PatientEncounter;
        }

        public List<PatientEncounter> encounterDetailsFromList(List<string> encounterIdList)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            List<PatientEncounter> patientEncList = new List<PatientEncounter>();

            mzk_patientencounter patientencounterEntity;

            QueryExpression query = new QueryExpression(mzk_patientencounter.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);
            
            if (encounterIdList != null)
            {
                foreach (string item in encounterIdList)
                {
                    childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(item));
                }
            }

            EntityCollection entitycol = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycol.Entities)
            {
                PatientEncounter patientEncounter = new PatientEncounter();

                patientencounterEntity = (mzk_patientencounter)entity;

                patientEncounter.EncounterId = patientencounterEntity.mzk_patientencounterId.Value.ToString();
                patientEncounter.EncounterStatusValue = patientencounterEntity.mzk_encounterstatus != null ? patientencounterEntity.mzk_encounterstatus.Value : 0;

                patientEncounter.EncounterType = patientencounterEntity.mzk_EncounterType.Value.ToString();                

                if (patientencounterEntity.Attributes.Contains("mzk_appointment"))
                    patientEncounter.AppointmentId = (patientencounterEntity.mzk_appointment).Id.ToString();

                patientEncList.Add(patientEncounter);
            }

            return patientEncList;
        }

        public List<PatientEncounter> encounterDetailsFromList(List<string> appointmentIdList, List<string> encounterIdList)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            List<PatientEncounter> patientEncList = new List<PatientEncounter>();

            mzk_patientencounter patientencounterEntity;

            QueryExpression query = new QueryExpression(mzk_patientencounter.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);

            if (appointmentIdList != null)
            {
                foreach (string appointmentId in appointmentIdList)
                {
                    childFilter.AddCondition("mzk_appointment", ConditionOperator.Equal, new Guid(appointmentId));
                }
            }

            if (encounterIdList != null)
            {
                foreach (string encounterId in encounterIdList)
                {
                    childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(encounterId));
                }
            }

            EntityCollection entitycol = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycol.Entities)
            {
                PatientEncounter patientEncounter = new PatientEncounter();

                patientencounterEntity = (mzk_patientencounter)entity;

                patientEncounter.EncounterId = patientencounterEntity.mzk_patientencounterId.Value.ToString();
                patientEncounter.EncounterStatusValue = patientencounterEntity.mzk_encounterstatus != null ? patientencounterEntity.mzk_encounterstatus.Value : 0;

                patientEncounter.EncounterType = patientencounterEntity.mzk_EncounterType.Value.ToString();

                patientEncounter.AppointmentId = patientencounterEntity.mzk_appointment.Id.ToString();

                patientEncList.Add(patientEncounter);
            }

            return patientEncList;
        }

        public string getEncounterIdByWorkflowId(string cpsaWorkflowId)
        {
            string encounterId = string.Empty;
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            Microsoft.Xrm.Sdk.Query.ColumnSet columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_casepathwaystate");

            mzk_casepathwaystateactivityworkflow CPSAW = (mzk_casepathwaystateactivityworkflow)entityRepository.GetEntity(mzk_casepathwaystateactivityworkflow.EntityLogicalName, new Guid(cpsaWorkflowId), columns);

            QueryExpression query = new QueryExpression(mzk_patientencounter.EntityLogicalName);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientencounterid");
            query.Criteria.AddCondition("mzk_encountertype", ConditionOperator.Equal, (int)mzk_encountertype.Triage);

            LinkEntity linkCPSAW = new LinkEntity()
            {
                LinkFromEntityName = mzk_patientencounter.EntityLogicalName,
                LinkToEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                LinkFromAttributeName = "mzk_casepathwaystateactivityworkflow",
                LinkToAttributeName = "mzk_casepathwaystateactivityworkflowid",
                EntityAlias = "CPSAW",
                JoinOperator =  JoinOperator.Inner
            };

            linkCPSAW.LinkCriteria.AddCondition("mzk_casepathwaystate", ConditionOperator.Equal, new Guid(CPSAW.mzk_CasePathwayState.Id.ToString()));

            query.LinkEntities.Add(linkCPSAW);

            EntityCollection entitycol = entityRepository.GetEntityCollection(query);
            if (entitycol.Entities.Count > 0) {
                if (entitycol.Entities[0].Attributes.Contains("mzk_patientencounterid")) {
                    encounterId = entitycol.Entities[0].Attributes["mzk_patientencounterid"].ToString(); 
                }
            }
            
            return encounterId;
        }

        public async Task<List<PatientEncounter>> getEncounterDetails(PatientEncounter encounter)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            List<PatientEncounter> patientEncounter = new List<PatientEncounter>();
            QueryExpression query = new QueryExpression(mzk_patientencounter.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            //Search Order
            if (!string.IsNullOrEmpty(encounter.EncounterType))
            {
                if (encounter.EncounterType == "1")
                    query.Criteria.AddCondition("mzk_encountertype", ConditionOperator.Equal, (int)mzk_encountertype.Triage);
                else
                    query.Criteria.AddCondition("mzk_encountertype", ConditionOperator.Equal, Convert.ToInt32(encounter.EncounterType));
            }


            if (!string.IsNullOrEmpty(encounter.CaseId) && (!string.IsNullOrEmpty(encounter.EncounterId)))
            {
                query.Criteria.AddCondition("mzk_caseid", ConditionOperator.Equal, encounter.CaseId);
                query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.NotEqual, encounter.EncounterId);
            }

            if (!string.IsNullOrEmpty(encounter.EncounterId) && string.IsNullOrEmpty(encounter.CaseId))
                query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, encounter.EncounterId);

            if (!string.IsNullOrEmpty(encounter.CaseId))
                query.Criteria.AddCondition("mzk_caseid", ConditionOperator.Equal, encounter.CaseId);

            if (!string.IsNullOrEmpty(encounter.AppointmentId))
                query.Criteria.AddCondition("mzk_appointment", ConditionOperator.Equal, new Guid(encounter.AppointmentId));

            OrderExpression orderby = new OrderExpression();
            orderby.AttributeName = "createdon";
            orderby.OrderType = OrderType.Descending;

            query.Orders.Add(orderby);

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {
                foreach (mzk_patientencounter patientencounterEntity in entitycollection.Entities)
                {

                    PatientEncounter model = new PatientEncounter();

                    //Encounter Id
                    model.EncounterId = patientencounterEntity.Id.ToString();
                    //Encounter Status
                    if (patientencounterEntity.Attributes.Contains("mzk_encounterstatus"))
                    {
                        model.EncounterStatusValue = ((OptionSetValue)patientencounterEntity.mzk_encounterstatus).Value;
                        model.EncounterStatus = patientencounterEntity.FormattedValues["mzk_encounterstatus"].ToString();
                    }

                    if (patientencounterEntity.Attributes.Contains("createdon"))
                        model.CreatedOn = Convert.ToDateTime(patientencounterEntity.CreatedOn);

                    if (patientencounterEntity.Attributes.Contains("mzk_encountertype"))
                    {
                        model.EncounterType = ((OptionSetValue)patientencounterEntity.mzk_EncounterType).Value.ToString();
                        model.EncounterTypeName = patientencounterEntity.FormattedValues["mzk_encountertype"].ToString();
                    }


                    //CreatedBy
                    if (patientencounterEntity.Attributes.Contains("createdby"))
                    {
                        model.CreatedBy = patientencounterEntity.CreatedBy.Name;
                        model.CreatedById = patientencounterEntity.CreatedBy.Id.ToString();
                    }

                    if (patientencounterEntity.Attributes.Contains("mzk_appointment"))
                        model.AppointmentId = patientencounterEntity.mzk_appointment.Id.ToString();

                    if (patientencounterEntity.Attributes.Contains("mzk_customer"))
                        model.PatientId = patientencounterEntity.mzk_customer.Id.ToString();

                    if (patientencounterEntity.Attributes.Contains("mzk_caseid"))
                        model.CaseId = ((EntityReference)patientencounterEntity.mzk_caseid).Id.ToString();

                    if (patientencounterEntity.Attributes.Contains("mzk_encountertemplate"))
                        model.EncounterTemplateId = ((EntityReference)patientencounterEntity.mzk_EncounterTemplate).Id.ToString();

                    if (patientencounterEntity.Attributes.Contains("mzk_jsonresponce"))
                    {
                        model.SummaryJson = patientencounterEntity.Attributes["mzk_jsonresponce"].ToString();
                    }

                    patientEncounter.Add(model);

                }
            }

            return patientEncounter;
        }



        public async Task<Guid> CreatePatientEncounterCRM(PatientEncounter condition)
        {
            try
            {

                Entity contact = new Entity("msemr_encounter");

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
                    contact["msemr_encounterstartdate"] = Convert.ToDateTime(condition.RecordedDate);
                }

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();


                QueryExpression queryExpression = new QueryExpression("msemr_encounter");
                queryExpression.Criteria.AddCondition("mzk_externalemrid", ConditionOperator.Equal, condition.Externalemrid);

                queryExpression.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryExpression);

                if (entitycollection != null)
                {
                    if (entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities[0].Attributes.Contains("msemr_encounterid"))
                        {
                            contact["msemr_encounterid"] = new Guid(entitycollection.Entities[0].Attributes["msemr_encounterid"].ToString());
                        }
                        entityRepository.UpdateEntity(contact);
                        contact.Id = new Guid(contact["msemr_encounterid"].ToString());
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

        public async Task<List<PatientEncounter>> getPatientEncounterCRM(string patientGuid, DateTime startdate, DateTime enddate)
        {
            try
            {
                List<PatientEncounter> list = new List<PatientEncounter>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression("msemr_encounter");

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
                        PatientEncounter obj = new PatientEncounter();
                        obj = getPatientEncounterModelFilled(entitycollection[i], obj, "");
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

        public static PatientEncounter getPatientEncounterModelFilled(Entity entity, PatientEncounter obs, string accountAlias)
        {
            if (entity.Attributes.Contains("msemr_name"))
            {
                obs.Title = (entity.Attributes["msemr_name"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_encounterid"))
            {
                obs.EncounterId = (entity.Attributes["msemr_encounterid"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_subjectpatient"))
            {
                obs.PatientID = new Guid((entity.Attributes["msemr_subjectpatient"] as EntityReference).Id.ToString());
            }

            if (entity.Attributes.Contains("msemr_encounterstartdate"))
            {
                obs.RecordedDate = Convert.ToDateTime(entity.Attributes["msemr_encounterstartdate"]);
            }

            return obs;
        }

    }
}
