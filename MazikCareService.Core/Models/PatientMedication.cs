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
    public class PatientMedication : PatientOrder, IPatientMedication 
    {
        public string MedicationName { get; set; }
        public string ProductId { get; set; }
        public string MedicationOrderId { get; set; }
        public string Frequency { get; set; }
        public string FrequencyArabic { get; set; }
        public string FrequencyId { get; set; }
        public int    Refill { get; set; }
        public string Unit { get; set; }
        public string UnitArabic { get; set; }
        public string UnitId { get; set; }
        public string Dosage { get; set; }
        public string Route { get; set; }
        public string RouteArabic { get; set; }
        public string RouteId { get; set; }
        public string Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public string TypeText { get; set; }
        public string AssociatedDiagnosis { get; set; }
        public string AssociatedDiagnosisId { get; set; }
        public string NotesforPharmacist { get; set; }
        public string Instructionstopatients { get; set; }
        public string Discontinue { get; set; }
        public string PRNIndication { get; set; }
        public string PRNIndicationText { get; set; }
        public string Cause { get; set; }
        public string Urgency { get; set; }
        public string UrgencyText { get; set; }

        public DateTime ExpiryDate { get; set; }
        public string BatchNum { get; set; }        
        public string MedicationOrderType { get; set; }
        public bool controlledDrugs { get; set; }
        public string Other { get; set; }
        public string isActive { get; set; }
        public string SIG { get; set; }
        public int deliveredQuantity { get; set; }
        public int remainingQuantity { get; set; }
        public int totalQuantity { get; set; }
        public string MedicationType { get; set; }
        public List<Reminder> Reminders { get; set; }
        public bool isReminderSet { get; set; }
        public List<string> MedicationInstructions { get; set; }

        //For Dose Entity
        /*  public string[] Dose { get; set; }
          public string[] DoseId { get; set; }
          public string DoseType { get; set; }
          public string[] DoseDuration { get; set; }
          public string[] DoseDurationId { get; set; }
          public DateTime[] DoseStartDate { get; set; }
          public DateTime[] DoseEndDate { get; set; }
          public string[] DoseFrequency { get; set; }
          public string[] DoseFrequencyArabic { get; set; }
          public string[] DoseFrequencyId { get; set; }
          public string[] DoseInstructions { get; set; }
          public string[] DoseSpecialdosing { get; set; }
          public string[] DoseUnit { get; set; }
          public string[] DoseUnitArabic { get; set; }
          public string[] DoseUnitId { get; set; }
          public string DoseNumber { get; set; }
          public string[] DoseDeleteId { get; set; }*/

        public async Task<List<PatientMedication>> getPatientOrder(string patientguid, string patientEncounter,string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId,string caseId = null, int pageNumber = 0, bool forHistory = false, bool checkLogs = true)
        {
            if (string.IsNullOrEmpty(patientguid) && string.IsNullOrEmpty(caseId) && string.IsNullOrEmpty(patientEncounter) && string.IsNullOrEmpty(orderId))
                throw new ValidationException("Parameter missing");

            List<PatientMedication> PatientMedication = new List<PatientMedication>();
            #region Patient Medication Query
            QueryExpression query = new QueryExpression("mzk_patientorder");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            if (!string.IsNullOrEmpty(orderId))
            {
                childFilter.AddCondition("mzk_patientorderid", ConditionOperator.Equal, new Guid(orderId));
            }
            if (!string.IsNullOrEmpty(caseId))
            {
                childFilter.AddCondition("mzk_caseid", ConditionOperator.Equal, new Guid(caseId));
            }

            if (SearchFilters != mzk_orderstatus.Cancelled.ToString())
            {
                childFilter.AddCondition("mzk_orderstatus", ConditionOperator.NotEqual, (int)mzk_orderstatus.Cancelled);
            }
            if (!string.IsNullOrEmpty(patientguid))
            {
                childFilter.AddCondition("mzk_customer", ConditionOperator.Equal, new Guid(patientguid));
            }
            else
            {
                if (!string.IsNullOrEmpty(patientEncounter))
                {
                    childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(patientEncounter));
                }
            }

            if(forFulfillment)
            {
                query.Criteria.AddCondition("mzk_urgency", ConditionOperator.Equal, (int)mzk_patientordermzk_Urgency.Stat);
            }

            childFilter.AddCondition("mzk_type", ConditionOperator.Equal, "1");

            //Patient Order Type :: Medication
            if (!string.IsNullOrEmpty(SearchFilters))
            {
                if (SearchFilters == Convert.ToString(mzk_medicationfilter.Active))
                {
                    childFilter.AddCondition("mzk_startdate", ConditionOperator.LessEqual, DateTime.UtcNow);
                    childFilter.AddCondition("mzk_enddate", ConditionOperator.GreaterEqual, DateTime.UtcNow);
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.NotEqual, 275380000);//Expired
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.NotEqual, Convert.ToInt32(mzk_orderstatus.Cancelled));
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.NotEqual, Convert.ToInt32(mzk_orderstatus.Discontinued));
                }
                if (SearchFilters == Convert.ToString(mzk_medicationfilter.Cancelled))
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal,Convert.ToInt32( mzk_orderstatus.Cancelled));
                if (SearchFilters == Convert.ToString(mzk_medicationfilter.Discontinued))
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Discontinued));               
            }
            //Search Order
            if (!string.IsNullOrEmpty(searchOrder))
                childFilter.AddCondition("mzk_productidname", ConditionOperator.Like, ("%" + searchOrder + "%"));

            //Search Date
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                childFilter.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientorderid",
                                                                    "mzk_productid",
                                                                    "mzk_patientordernumber",
                                                                    "mzk_dose",
                                                                    "mzk_unitid",
                                                                    "mzk_patientencounterid",
                                                                    "mzk_notesforpharmacist",
                                                                    "mzk_startdate",
                                                                    "mzk_routeid",
                                                                    "mzk_associateddiagnosisid",
                                                                    "mzk_other",
                                                                    "mzk_medicationordertype",
                                                                    "mzk_medicationtype",
                                                                    "mzk_prnindication",
                                                                    "mzk_instructionstopatients",
                                                                    "mzk_enddate",
                                                                    "mzk_duration",
                                                                    "mzk_frequencyid",
                                                                    "mzk_axcasetransrefrecid",
                                                                    "mzk_cause",
                                                                    "mzk_discontinue",
                                                                    "mzk_orderstatus",
                                                                    "mzk_patientordernumber",
                                                                    "mzk_orderdate",
                                                                    "createdon",
                                                                    "mzk_urgency",
                                                                    "mzk_refill",
                                                                    "mzk_statusmanagerdetail",
                                                                    "mzk_treatmentlocation",
                                                                    "mzk_orderinglocation",
                                                                    "mzk_caseid",
                                                                    "mzk_fulfillmentappointment",
                                                                    "mzk_orderingappointment", "mzk_isreminderset", "mzk_dosevalue");

            
            LinkEntity EntityDiagnosis = new LinkEntity("mzk_patientorder", "mzk_concept", "mzk_associateddiagnosisid", "mzk_conceptid", JoinOperator.LeftOuter);
            EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_icdcodeid");

            LinkEntity EntityFrequecy = new LinkEntity("mzk_patientorder", "mzk_ordersetup", "mzk_frequencyid", "mzk_ordersetupid", JoinOperator.LeftOuter);
            EntityFrequecy.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_arabicdescription", "mzk_frequencyday", "mzk_reminderafterdays");
            EntityFrequecy.EntityAlias = "Frequency";

            LinkEntity EntityRoute = new LinkEntity("mzk_patientorder", "mzk_ordersetup", "mzk_routeid", "mzk_ordersetupid", JoinOperator.LeftOuter);
            EntityRoute.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_arabicdescription");

            LinkEntity EntityUnit = new LinkEntity("mzk_patientorder", "mzk_unit", "mzk_unitid", "mzk_unitid", JoinOperator.LeftOuter);
            EntityUnit.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_arabicdescription");

            LinkEntity ProductRecord = new LinkEntity("mzk_patientorder", "product", "mzk_productid", "productid", JoinOperator.LeftOuter);
            ProductRecord.EntityAlias = "ProductRecord";

            LinkEntity EntityFamily = new LinkEntity("product", "product", "parentproductid", "productid", JoinOperator.LeftOuter);
            EntityFamily.EntityAlias = "ProductFamily";
            EntityFamily.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_antibioticmandatory", "mzk_commentsmandatory", "mzk_controlleddrug");
            ProductRecord.LinkEntities.Add(EntityFamily);

            OrderExpression orderby = new OrderExpression();
            orderby.AttributeName = "mzk_enddate";
            orderby.OrderType = OrderType.Ascending;

            //query.LinkEntities.Add(EntityDose);

            query.LinkEntities.Add(EntityFrequecy);
            query.LinkEntities.Add(EntityRoute);
            query.LinkEntities.Add(EntityDiagnosis);
            query.LinkEntities.Add(EntityUnit);
            query.LinkEntities.Add(ProductRecord);
            query.Orders.Add(orderby);

            if (!forFulfillment && pageNumber > 0)
            {
                query.PageInfo = new Microsoft.Xrm.Sdk.Query.PagingInfo();
                query.PageInfo.Count = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                query.PageInfo.PageNumber = pageNumber;
                query.PageInfo.PagingCookie = null;
                query.PageInfo.ReturnTotalRecordCount = true;
            }

            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                      
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            int value = 0;

            foreach (Entity entity in entitycollection.Entities)
            {
                PatientMedication model = new PatientMedication();
                model.Reminders = new List<Models.Reminder>();
                model.MedicationInstructions = new List<string>();
                if (!this.getPatientOrder(model, entity, forFulfillment, orderId, mzk_entitytype.MedicationOrder, forHistory))
                {
                    continue;
                }
                if (entity.Attributes.Contains("mzk_isreminderset"))
                    model.isReminderSet = (bool)entity["mzk_isreminderset"];
                if (entity.Attributes.Contains("mzk_medicationtype"))
                {
                    model.MedicationType = (entity["mzk_medicationtype"] as OptionSetValue).Value.ToString();
                    Reminder Reminder = new Reminder();
                    if (model.MedicationType.Equals("275380000")) // Regular
                    {
                        if (entity.Attributes.Contains("mzk_startdate") && entity.Attributes.Contains("mzk_enddate"))
                        {
                            Reminder.startDateTime = (DateTime)entity["mzk_startdate"];
                            Reminder.endDateTime = (DateTime)entity["mzk_enddate"];
                            if (entity.Attributes.Contains("mzk_dosevalue") &&
                                entity.Attributes.Contains("mzk_unitid") &&
                                entity.Attributes.Contains("mzk_routeid") &&
                                entity.Attributes.Contains("mzk_frequencyid")
                                )
                            {
                                Reminder.instruction = "Use " + entity["mzk_dosevalue"].ToString() + " " + (entity["mzk_unitid"] as EntityReference).Name + " " + (entity["mzk_routeid"] as EntityReference).Name;
                                model.MedicationInstructions.Add("Use " + entity["mzk_dosevalue"].ToString() + " " + (entity["mzk_unitid"] as EntityReference).Name + " " + (entity["mzk_routeid"] as EntityReference).Name +" "+ (entity["mzk_frequencyid"] as EntityReference).Name + " from " + Convert.ToDateTime(entity["mzk_startdate"]).ToShortDateString() + " to " + Convert.ToDateTime(entity["mzk_enddate"]).ToShortDateString());
                            }
                        }
                        if (entity.Attributes.Contains("Frequency.mzk_frequencyday"))
                            Reminder.frequencyPerDay = entity.GetAttributeValue<AliasedValue>("Frequency.mzk_frequencyday").Value.ToString();
                        if(entity.Attributes.Contains("Frequency.mzk_reminderafterdays"))
                            Reminder.reminderAfterDays = entity.GetAttributeValue<AliasedValue>("Frequency.mzk_reminderafterdays").Value.ToString();
                        model.Reminders.Add(Reminder);
                    }
                    else if (model.MedicationType.Equals("275380003")) // Loading Or Tapering
                    {
                        if (!string.IsNullOrEmpty(model.Id))
                        {
                            model.Reminders = getDose(model.Id);
                            model.MedicationInstructions = getMedicationInstructions(model.Id);
                        }
                    }
                   
                }

                if (entity.Attributes.Contains("mzk_productid"))
                {
                    model.MedicationName = ((EntityReference)entity["mzk_productid"]).Name;
                    model.ProductId = ((EntityReference)entity["mzk_productid"]).Id.ToString();
                }

                if (entity.Attributes.Contains("mzk_frequencyid"))
                    model.FrequencyId = ((EntityReference)entity["mzk_frequencyid"]).Id.ToString();

                if (entity.Attributes.Contains("mzk_ordersetup1.mzk_description"))
                {
                    model.Frequency = (entity.Attributes["mzk_ordersetup1.mzk_description"] as AliasedValue).Value.ToString();
                }
                else
                    if (entity.Attributes.Contains("mzk_ordersetup3.mzk_description"))
                {
                    model.Frequency = (entity.Attributes["mzk_ordersetup3.mzk_description"] as AliasedValue).Value.ToString();
                }

                if (entity.Attributes.Contains("mzk_ordersetup1.mzk_arabicdescription"))
                {
                    model.FrequencyArabic = (entity.Attributes["mzk_ordersetup1.mzk_arabicdescription"] as AliasedValue).Value.ToString();
                }
                else
                   if (entity.Attributes.Contains("mzk_ordersetup3.mzk_arabicdescription"))
                {
                    model.FrequencyArabic = (entity.Attributes["mzk_ordersetup3.mzk_arabicdescription"] as AliasedValue).Value.ToString();
                }

                if (entity.Attributes.Contains("mzk_dose"))
                    model.Dosage = entity.Attributes["mzk_dose"].ToString();
                
                if (entity.Attributes.Contains("mzk_unitid"))
                    model.UnitId = ((EntityReference)entity["mzk_unitid"]).Id.ToString();

                if (entity.Attributes.Contains("mzk_unit4.mzk_description"))
                {
                    model.Unit = (entity.Attributes["mzk_unit4.mzk_description"] as AliasedValue).Value.ToString();
                }
                else
                    if (entity.Attributes.Contains("mzk_unit6.mzk_description"))
                {
                    model.Unit = (entity.Attributes["mzk_unit6.mzk_description"] as AliasedValue).Value.ToString();
                }

                if (entity.Attributes.Contains("mzk_unit4.mzk_arabicdescription"))
                {
                   model.UnitArabic = (entity.Attributes["mzk_unit4.mzk_arabicdescription"] as AliasedValue).Value.ToString();
                }
                else
                    if (entity.Attributes.Contains("mzk_unit6.mzk_arabicdescription"))
                {
                    model.UnitArabic = (entity.Attributes["mzk_unit6.mzk_arabicdescription"] as AliasedValue).Value.ToString();
                }

                if (entity.Attributes.Contains("mzk_routeid"))
                    model.RouteId = ((EntityReference)entity["mzk_routeid"]).Id.ToString();

                if (entity.Attributes.Contains("mzk_ordersetup2.mzk_description"))
                {
                    model.Route = (entity.Attributes["mzk_ordersetup2.mzk_description"] as AliasedValue).Value.ToString();
                }
                else
                    if (entity.Attributes.Contains("mzk_ordersetup4.mzk_description"))
                {
                    model.Route = (entity.Attributes["mzk_ordersetup4.mzk_description"] as AliasedValue).Value.ToString();
                }

                if (entity.Attributes.Contains("mzk_ordersetup2.mzk_arabicdescription"))
                {
                    model.RouteArabic = (entity.Attributes["mzk_ordersetup2.mzk_arabicdescription"] as AliasedValue).Value.ToString();
                }
                else
                   if (entity.Attributes.Contains("mzk_ordersetup4.mzk_arabicdescription"))
                {
                    model.RouteArabic = (entity.Attributes["mzk_ordersetup4.mzk_arabicdescription"] as AliasedValue).Value.ToString();
                }

                if (entity.Attributes.Contains("mzk_duration"))
                    model.Duration = entity.Attributes["mzk_duration"].ToString();

                if (entity.Attributes.Contains("mzk_refill"))
                    model.Refill = Convert.ToInt32(entity.Attributes["mzk_refill"]);

                if (entity.Attributes.Contains("mzk_startdate"))
                    model.StartDate = Convert.ToDateTime(entity["mzk_startdate"]).Date ;// entity.Attributes["mzk_startdate"].ToString();

                if (entity.Attributes.Contains("mzk_enddate"))
                    model.EndDate = Convert.ToDateTime(entity["mzk_enddate"]).Date;// entity.Attributes["mzk_enddate"].ToString();

                if ((model.StartDate <= DateTime.Today) && (DateTime.Today <= model.EndDate))
                {
                    model.isActive = "true";
                }
                else
                {
                    model.isActive = "false";
                }

                if (entity.Attributes.Contains("ProductFamily.mzk_controlleddrug"))
                    model.controlledDrugs = (bool)((AliasedValue)entity.Attributes["ProductFamily.mzk_controlleddrug"]).Value;

                if (entity.Attributes.Contains("mzk_medicationordertype"))
                {
                    model.Type = (entity["mzk_medicationordertype"] as OptionSetValue).Value.ToString();
                    model.TypeText = entity.FormattedValues["mzk_medicationordertype"].ToString();
                }

                if (entity.Attributes.Contains("mzk_associateddiagnosisid"))
                    model.AssociatedDiagnosisId = ((EntityReference)entity["mzk_associateddiagnosisid"]).Id.ToString();

                if (entity.Attributes.Contains("mzk_concept3.mzk_conceptname"))
                    model.AssociatedDiagnosis = (entity.Attributes["mzk_concept3.mzk_conceptname"] as AliasedValue).Value.ToString();
                else
                    if (entity.Attributes.Contains("mzk_concept5.mzk_conceptname"))
                    model.AssociatedDiagnosis = (entity.Attributes["mzk_concept5.mzk_conceptname"] as AliasedValue).Value.ToString();

                if (entity.Attributes.Contains("mzk_concept3.mzk_icdcodeid"))
                    model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_concept3.mzk_icdcodeid"]).Value).Name;
                else
                    if (entity.Attributes.Contains("mzk_concept5.mzk_icdcodeid"))
                    model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_concept5.mzk_icdcodeid"]).Value).Name;


                if (entity.Attributes.Contains("mzk_notesforpharmacist"))
                    model.NotesforPharmacist = entity.Attributes["mzk_notesforpharmacist"].ToString();

                if (entity.Attributes.Contains("mzk_instructionstopatients"))
                    model.Instructionstopatients = entity.Attributes["mzk_instructionstopatients"].ToString();

                model.Instructionstopatients = string.Format("Use {0} {1} by {2} {3}", model.Dosage, model.Unit, model.Route, model.Frequency);

                if (entity.Attributes.Contains("mzk_discontinue") && entity.Attributes["mzk_discontinue"].ToString() == "True")
                    model.Discontinue = "1";
                else
                    model.Discontinue = "0";

                if (entity.Attributes.Contains("mzk_treatmentlocation"))
                {
                    model.treatmentLocationId = entity.GetAttributeValue<EntityReference>("mzk_treatmentlocation").Id.ToString();
                    model.treatmentLocation = entity.GetAttributeValue<EntityReference>("mzk_treatmentlocation").Name;
                }

                if (entity.Attributes.Contains("mzk_other"))
                {
                    model.Other = entity.Attributes["mzk_other"].ToString();
                }

                if (entity.Attributes.Contains("mzk_prnindication"))
                {
                    model.PRNIndication = (entity["mzk_prnindication"] as OptionSetValue).Value.ToString();
                    model.PRNIndicationText = entity.FormattedValues["mzk_prnindication"].ToString();
                }
                if (entity.Attributes.Contains("mzk_cause"))
                    model.Cause = entity.Attributes["mzk_cause"].ToString();

                if (entity.Attributes.Contains("mzk_urgency"))
                {
                    model.Urgency = ((OptionSetValue)entity["mzk_urgency"]).Value.ToString();
                    model.UrgencyText = entity.FormattedValues["mzk_urgency"].ToString();
                }

                if (entity.Attributes.Contains("mzk_axcasetransrefrecid"))
                {
                    CaseRepository caseRepo = new CaseRepository();

                    HMCaseTransContract contract = caseRepo.getCaseTransDetails((long)Convert.ToDecimal(entity.Attributes["mzk_axcasetransrefrecid"]));

                    if (contract != null)
                    {                        
                        if (!string.IsNullOrEmpty(contract.parmBatchNum))
                        {
                            model.BatchNum = contract.parmBatchNum;
                            model.ExpiryDate = contract.parmExpiryDate;
                        }
                        model.ManufacturerName = contract.parmManufacturerName;
                    }
                }

                if (checkLogs)
                {
                    PatientOrderLog log = this.getOrderStatusLogDetails(Convert.ToInt32(model.OrderStatus), model.Id);

                    if (log != null)
                    {
                        model.StatusNotes = log.Comments;
                    }
                }

                /*
                #region Dose Query
                value = 0;
                QueryExpression dosequery = new QueryExpression("mzk_dose");
                FilterExpression dosechildFilter = dosequery.Criteria.AddFilter(LogicalOperator.And);
                dosechildFilter.AddCondition("mzk_patientorderid", ConditionOperator.Equal, new Guid(entity.Id.ToString()));
                dosequery.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_dosetype", "mzk_dose", "mzk_startdate", "mzk_enddate", "mzk_frequencyid", "mzk_instructions", "mzk_specialdosing", "mzk_unitid", "mzk_duration");

                LinkEntity EntityFrequecyDose = new LinkEntity("mzk_dose", "mzk_ordersetup", "mzk_frequencyid", "mzk_ordersetupid", JoinOperator.LeftOuter);
                EntityFrequecyDose.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_arabicdescription");
                EntityFrequecyDose.EntityAlias = "EntityFrequecyDose";

                LinkEntity EntityUnitDose = new LinkEntity("mzk_dose", "mzk_unit", "mzk_unitid", "mzk_unitid", JoinOperator.LeftOuter);
                EntityUnitDose.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_arabicdescription");
                EntityUnitDose.EntityAlias = "EntityUnitDose";

                dosequery.LinkEntities.Add(EntityFrequecyDose);
                dosequery.LinkEntities.Add(EntityUnitDose);

                #endregion

                EntityCollection doseentitycollection = entityRepository.GetEntityCollection(dosequery);
                foreach (Entity doseentity in doseentitycollection.Entities)
                {
                    if (doseentity.Attributes.Contains("mzk_dosetype"))
                        model.DoseType = ((OptionSetValue)doseentity.Attributes["mzk_dosetype"]).Value.ToString();

                    if (doseentity.Attributes.Contains("mzk_dose"))
                    {
                        if (this.IsNullOrEmptyArray(model.Dose))
                            model.Dose = new string[doseentitycollection.Entities.Count];
                            model.Dose[value]   = doseentity.Attributes["mzk_dose"].ToString();
                    }

                    if (doseentity.Attributes.Contains("mzk_startdate"))
                    {
                        if (this.IsNullOrEmptyArrayDate(model.DoseStartDate))
                            model.DoseStartDate = new DateTime[doseentitycollection.Entities.Count];
                            model.DoseStartDate[value] = Convert.ToDateTime(doseentity.Attributes["mzk_startdate"]);
                    }

                    if (doseentity.Attributes.Contains("mzk_enddate"))
                    {
                        if (this.IsNullOrEmptyArrayDate(model.DoseEndDate))
                            model.DoseEndDate = new DateTime[doseentitycollection.Entities.Count];
                            model.DoseEndDate[value] = Convert.ToDateTime(doseentity.Attributes["mzk_enddate"]);
                    }

                    if (doseentity.Attributes.Contains("mzk_frequencyid"))
                    {
                        if (this.IsNullOrEmptyArray(model.DoseFrequency))
                        {
                            model.DoseFrequency = new string[doseentitycollection.Entities.Count];
                            model.DoseFrequencyId = new string[doseentitycollection.Entities.Count];
                            model.DoseFrequencyArabic = new string[doseentitycollection.Entities.Count];
                        }

                        model.DoseFrequencyId[value] = ((EntityReference)doseentity.Attributes["mzk_frequencyid"]).Id.ToString();

                        if (doseentity.Attributes.Contains("EntityFrequecyDose.mzk_description"))
                        {
                            model.DoseFrequency[value] = (doseentity.Attributes["EntityFrequecyDose.mzk_description"] as AliasedValue).Value.ToString();
                        }

                        if (doseentity.Attributes.Contains("EntityFrequecyDose.mzk_arabicdescription"))
                        {
                            model.DoseFrequencyArabic[value] = (doseentity.Attributes["EntityFrequecyDose.mzk_arabicdescription"] as AliasedValue).Value.ToString();
                        }
                    }

                    if (doseentity.Attributes.Contains("mzk_instructions"))
                    {
                        if (this.IsNullOrEmptyArray(model.DoseInstructions))
                            model.DoseInstructions = new string[doseentitycollection.Entities.Count];
                            model.DoseInstructions[value] = doseentity.Attributes["mzk_instructions"].ToString();
                    }

                    if (doseentity.Attributes.Contains("mzk_specialdosing") && doseentity.Attributes["mzk_specialdosing"].ToString() == "True")
                    {
                        if (this.IsNullOrEmptyArray(model.DoseSpecialdosing))
                            model.DoseSpecialdosing = new string[doseentitycollection.Entities.Count];
                            model.DoseSpecialdosing[value] = "1";
                    }
                    else
                    {
                        if (this.IsNullOrEmptyArray(model.DoseSpecialdosing))
                            model.DoseSpecialdosing = new string[doseentitycollection.Entities.Count];
                            model.DoseSpecialdosing[value] = "0";
                    }

                    if (doseentity.Attributes.Contains("mzk_unitid"))
                    {
                        if (this.IsNullOrEmptyArray(model.DoseUnit))
                        {
                            model.DoseUnit = new string[doseentitycollection.Entities.Count];
                            model.DoseUnitArabic = new string[doseentitycollection.Entities.Count];
                            model.DoseUnitId = new string[doseentitycollection.Entities.Count];
                        }

                        model.DoseUnitId[value] = ((EntityReference)doseentity.Attributes["mzk_unitid"]).Id.ToString();

                        if (doseentity.Attributes.Contains("EntityUnitDose.mzk_description"))
                        {
                            model.DoseUnit[value] = (doseentity.Attributes["EntityUnitDose.mzk_description"] as AliasedValue).Value.ToString();
                        }

                        if (doseentity.Attributes.Contains("EntityUnitDose.mzk_arabicdescription"))
                        {
                            model.DoseUnitArabic[value] = (doseentity.Attributes["EntityUnitDose.mzk_arabicdescription"] as AliasedValue).Value.ToString();
                        }
                    }

                    if (doseentity.Attributes.Contains("mzk_duration"))
                    {
                        if (this.IsNullOrEmptyArray(model.DoseDuration))
                        {
                            model.DoseDuration = new string[doseentitycollection.Entities.Count];
                            model.DoseDurationId = new string[doseentitycollection.Entities.Count];
                        }
                            model.DoseDurationId[value] = doseentity.Attributes["mzk_duration"].ToString();
                            model.DoseDuration[value] = doseentity.Attributes["mzk_duration"].ToString();
                    }

                    if (doseentity.Attributes.Contains("mzk_doseid"))
                    {
                        if (this.IsNullOrEmptyArray(model.DoseId))
                            model.DoseId = new string[doseentitycollection.Entities.Count];
                            model.DoseId[value] = doseentity.Id.ToString();
                    }
                    value++;
                }*/
                PatientMedication.Add(model);
            }

            if (pageNumber > 0 && entitycollection != null)
            {
                Pagination.totalCount = entitycollection.TotalRecordCount;
            }

            return PatientMedication;
        }

        public async Task<List<PatientMedication>> getPatientOrder(List<Entity> patientOrders)
        {
            List<PatientMedication> PatientMedication = new List<PatientMedication>();

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            int value = 0;

            foreach (Entity entity in patientOrders)
            {
                PatientMedication model = new PatientMedication();

                if (entity.Attributes.Contains("mzk_productid"))
                {
                    model.MedicationName = ((EntityReference)entity["mzk_productid"]).Name;
                    model.ProductId = ((EntityReference)entity["mzk_productid"]).Id.ToString();
                }

                if (entity.Attributes.Contains("mzk_ordersetupFrequency.mzk_description"))
                {
                    model.Frequency = (entity.Attributes["mzk_ordersetupFrequency.mzk_description"] as AliasedValue).Value.ToString();
                }

                if (entity.Attributes.Contains("mzk_conceptDiagnosis.mzk_conceptname"))
                    model.AssociatedDiagnosis = (entity.Attributes["mzk_conceptDiagnosis.mzk_conceptname"] as AliasedValue).Value.ToString();

                if (entity.Attributes.Contains("mzk_instructionstopatients"))
                    model.Instructionstopatients = entity.Attributes["mzk_instructionstopatients"].ToString();

                if (entity.Id != null)
                    model.Id = entity.Id.ToString();
                /*
                #region Dose Query
                value = 0;
                QueryExpression dosequery = new QueryExpression("mzk_dose");
                FilterExpression dosechildFilter = dosequery.Criteria.AddFilter(LogicalOperator.And);
                dosechildFilter.AddCondition("mzk_patientorderid", ConditionOperator.Equal, new Guid(entity.Id.ToString()));
                dosequery.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_dosetype", "mzk_dose", "mzk_startdate", "mzk_enddate", "mzk_frequencyid", "mzk_instructions", "mzk_specialdosing", "mzk_unitid", "mzk_duration");

                LinkEntity EntityFrequecyDose = new LinkEntity("mzk_dose", "mzk_ordersetup", "mzk_frequencyid", "mzk_ordersetupid", JoinOperator.LeftOuter);
                EntityFrequecyDose.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_arabicdescription");
                EntityFrequecyDose.EntityAlias = "EntityFrequecyDose";

                LinkEntity EntityUnitDose = new LinkEntity("mzk_dose", "mzk_unit", "mzk_unitid", "mzk_unitid", JoinOperator.LeftOuter);
                EntityUnitDose.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_arabicdescription");
                EntityUnitDose.EntityAlias = "EntityUnitDose";

                dosequery.LinkEntities.Add(EntityFrequecyDose);
                dosequery.LinkEntities.Add(EntityUnitDose);

                #endregion


                EntityCollection doseentitycollection = entityRepository.GetEntityCollection(dosequery);
                foreach (Entity doseentity in doseentitycollection.Entities)
                {
                    if (doseentity.Attributes.Contains("mzk_dosetype"))
                        model.DoseType = ((OptionSetValue)doseentity.Attributes["mzk_dosetype"]).Value.ToString();

                    if (doseentity.Attributes.Contains("mzk_dose"))
                    {
                        if (this.IsNullOrEmptyArray(model.Dose))
                            model.Dose = new string[doseentitycollection.Entities.Count];
                        model.Dose[value] = doseentity.Attributes["mzk_dose"].ToString();
                    }

                    if (doseentity.Attributes.Contains("mzk_startdate"))
                    {
                        if (this.IsNullOrEmptyArrayDate(model.DoseStartDate))
                            model.DoseStartDate = new DateTime[doseentitycollection.Entities.Count];
                        model.DoseStartDate[value] = Convert.ToDateTime(doseentity.Attributes["mzk_startdate"]);
                    }

                    if (doseentity.Attributes.Contains("mzk_enddate"))
                    {
                        if (this.IsNullOrEmptyArrayDate(model.DoseEndDate))
                            model.DoseEndDate = new DateTime[doseentitycollection.Entities.Count];
                        model.DoseEndDate[value] = Convert.ToDateTime(doseentity.Attributes["mzk_enddate"]);
                    }

                    if (doseentity.Attributes.Contains("mzk_frequencyid"))
                    {
                        if (this.IsNullOrEmptyArray(model.DoseFrequency))
                        {
                            model.DoseFrequency = new string[doseentitycollection.Entities.Count];
                            model.DoseFrequencyId = new string[doseentitycollection.Entities.Count];
                            model.DoseFrequencyArabic = new string[doseentitycollection.Entities.Count];
                        }

                        model.DoseFrequencyId[value] = ((EntityReference)doseentity.Attributes["mzk_frequencyid"]).Id.ToString();

                        if (doseentity.Attributes.Contains("EntityFrequecyDose.mzk_description"))
                        {
                            model.DoseFrequency[value] = (doseentity.Attributes["EntityFrequecyDose.mzk_description"] as AliasedValue).Value.ToString();
                        }

                        if (doseentity.Attributes.Contains("EntityFrequecyDose.mzk_arabicdescription"))
                        {
                            model.DoseFrequencyArabic[value] = (doseentity.Attributes["EntityFrequecyDose.mzk_arabicdescription"] as AliasedValue).Value.ToString();
                        }
                    }

                    if (doseentity.Attributes.Contains("mzk_instructions"))
                    {
                        if (this.IsNullOrEmptyArray(model.DoseInstructions))
                            model.DoseInstructions = new string[doseentitycollection.Entities.Count];
                        model.DoseInstructions[value] = doseentity.Attributes["mzk_instructions"].ToString();
                    }

                    if (doseentity.Attributes.Contains("mzk_specialdosing") && doseentity.Attributes["mzk_specialdosing"].ToString() == "True")
                    {
                        if (this.IsNullOrEmptyArray(model.DoseSpecialdosing))
                            model.DoseSpecialdosing = new string[doseentitycollection.Entities.Count];

                        model.DoseSpecialdosing[value] = "1";
                    }
                    else
                    {
                        if (this.IsNullOrEmptyArray(model.DoseSpecialdosing))
                            model.DoseSpecialdosing = new string[doseentitycollection.Entities.Count];

                        model.DoseSpecialdosing[value] = "0";
                    }

                    if (doseentity.Attributes.Contains("mzk_unitid"))
                    {
                        if (this.IsNullOrEmptyArray(model.DoseUnit))
                        {
                            model.DoseUnit = new string[doseentitycollection.Entities.Count];
                            model.DoseUnitArabic = new string[doseentitycollection.Entities.Count];
                            model.DoseUnitId = new string[doseentitycollection.Entities.Count];
                        }
                        model.DoseUnitId[value] = ((EntityReference)doseentity.Attributes["mzk_unitid"]).Id.ToString();

                        if (doseentity.Attributes.Contains("EntityUnitDose.mzk_description"))
                        {
                            model.DoseUnit[value] = (doseentity.Attributes["EntityUnitDose.mzk_description"] as AliasedValue).Value.ToString();
                        }

                        if (doseentity.Attributes.Contains("EntityUnitDose.mzk_arabicdescription"))
                        {
                            model.DoseUnitArabic[value] = (doseentity.Attributes["EntityUnitDose.mzk_arabicdescription"] as AliasedValue).Value.ToString();
                        }
                    }

                    if (doseentity.Attributes.Contains("mzk_duration"))
                    {
                        if (this.IsNullOrEmptyArray(model.DoseDuration))
                        {
                            model.DoseDuration = new string[doseentitycollection.Entities.Count];
                            model.DoseDurationId = new string[doseentitycollection.Entities.Count];
                        }
                        model.DoseDurationId[value] = doseentity.Attributes["mzk_duration"].ToString();
                        model.DoseDuration[value] = doseentity.Attributes["mzk_duration"].ToString();
                    }

                    if (doseentity.Attributes.Contains("mzk_doseid"))
                    {
                        if (this.IsNullOrEmptyArray(model.DoseId))
                            model.DoseId = new string[doseentitycollection.Entities.Count];
                        model.DoseId[value] = doseentity.Id.ToString();
                    }
                    value++;
                }*/
                PatientMedication.Add(model);
            }

            return PatientMedication;
        }

        public async Task<string> addPatientOrder(PatientMedication patientMedication, bool isActivityOrder = false)
        {

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            mzk_patientorder patientOrderEntity = new mzk_patientorder();

            try
            {
                decimal dose = 0, duration = 0, frequency = 0;
                int type = 4;
                string unit = string.Empty;
                string tempFreq = string.Empty;

                mzk_casetype caseType = mzk_casetype.OutPatient;

                patientOrderEntity.mzk_appointable = true;

                if (!string.IsNullOrEmpty(patientMedication.EncounterId))
                {
                    patientOrderEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(patientMedication.EncounterId));

                    PatientEncounter encounter = new PatientEncounter();
                    encounter.EncounterId = patientMedication.EncounterId;
                    encounter = encounter.encounterDetails(encounter).Result.ToList().First<PatientEncounter>();
                    PatientId = encounter.PatientId;
                    caseType = encounter.caseTypeValue;
                    patientOrderEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(PatientId));
                }

                if (!string.IsNullOrEmpty(patientMedication.appointmentId))
                {
                    patientOrderEntity.mzk_orderingappointment = new EntityReference("mzk_patientorder", new Guid(patientMedication.appointmentId));
                    patientOrderEntity.mzk_fulfillmentappointment = new EntityReference("mzk_patientorder", new Guid(patientMedication.appointmentId));
                }

                if (!string.IsNullOrEmpty(patientMedication.MedicationName))
                    patientOrderEntity.Attributes["mzk_productid"] = new EntityReference("product", new Guid(patientMedication.MedicationName));
                else
                    throw new ValidationException("Medication must be selected");

                if (!string.IsNullOrEmpty(patientMedication.Frequency))
                { 
                    patientOrderEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientMedication.Frequency));

                    tempFreq = new OrderSetup().getDescription(new Guid(patientMedication.Frequency), "mzk_frequencyperday");
                                       
                    if (!string.IsNullOrEmpty(tempFreq))
                    {
                        frequency = decimal.Parse(tempFreq);
                    }
                }

                if (!string.IsNullOrEmpty(patientMedication.Duration))
                {
                    patientOrderEntity.Attributes["mzk_duration"] = patientMedication.Duration;
                    duration = decimal.Parse(patientMedication.Duration);
                }

                if (!string.IsNullOrEmpty(patientMedication.Route))
                    patientOrderEntity.Attributes["mzk_routeid"] = new EntityReference("mzk_ordersetup", new Guid(patientMedication.Route));
                else
                    throw new ValidationException("Route must be selected");

                if (patientMedication.StartDate != DateTime.MinValue)
                    patientOrderEntity.Attributes["mzk_startdate"] = patientMedication.StartDate;
                else
                    throw new ValidationException("Start Date must be selected");

                if (patientMedication.EndDate != DateTime.MinValue)
                    patientOrderEntity.Attributes["mzk_enddate"] = patientMedication.EndDate;
                else
                    throw new ValidationException("End Date must be selected");

                if (!string.IsNullOrEmpty(patientMedication.Type))
                    patientOrderEntity.Attributes["mzk_medicationordertype"] = new OptionSetValue(Convert.ToInt32(patientMedication.Type));
                else
                    throw new ValidationException("Medication type must be selected");

                if (!string.IsNullOrEmpty(patientMedication.Dosage))
                {
                    patientOrderEntity.Attributes["mzk_dose"] = patientMedication.Dosage;
                    if(decimal.TryParse(patientMedication.Dosage,out dose))
                    dose = decimal.Parse(patientMedication.Dosage);
                }
                //else
                //    throw new ValidationException("Dosage field is required");

                if (!string.IsNullOrEmpty(patientMedication.Unit))
                {
                    patientOrderEntity.Attributes["mzk_unitid"] = new EntityReference("mzk_unit", new Guid(patientMedication.Unit));

                    unit = Models.Unit.getUnitFieldValue(new Guid(patientMedication.Unit), "mzk_code");
                }                

                if (!string.IsNullOrEmpty(patientMedication.AssociatedDiagnosis))
                    patientOrderEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_ordersetup", new Guid(patientMedication.AssociatedDiagnosis));
                else
                {
                    if (CaseParameter.getDiagnosisRequired(PatientCase.getCaseType(patientMedication.EncounterId)))
                    {
                        throw new ValidationException("Diagnosis must be selected");
                    }                    
                }

                if (!string.IsNullOrEmpty(patientMedication.NotesforPharmacist))
                    patientOrderEntity.Attributes["mzk_notesforpharmacist"] = patientMedication.NotesforPharmacist;
                //else
                  //  throw new ValidationException("Notes field is required");

                if (!string.IsNullOrEmpty(patientMedication.Instructionstopatients))
                    patientOrderEntity.Attributes["mzk_instructionstopatients"] = patientMedication.Instructionstopatients;
               // else
                  //  throw new ValidationException("Instrcution field is required");

                if (!string.IsNullOrEmpty(patientMedication.Discontinue) && patientMedication.Discontinue == "1")
                {
                    patientOrderEntity.Attributes["mzk_discontinue"] = true;
                    patientOrderEntity.Attributes["mzk_orderstatus"] = new OptionSetValue(Convert.ToInt32(mzk_orderstatus.Discontinued));
                }
                else
                {
                    patientOrderEntity.Attributes["mzk_discontinue"] = false;
                    StatusManager statusManager = StatusManager.getRootStatus(mzk_entitytype.MedicationOrder, caseType, isActivityOrder);

                    if (statusManager != null)
                    {
                        patientOrderEntity.mzk_OrderStatus = new OptionSetValue(statusManager.status);
                        patientOrderEntity.mzk_StatusManagerDetail = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, new Guid(statusManager.StatusId));
                    }
                }          

                if (patientMedication.clinicRecId > 0)
                    patientOrderEntity.Attributes["mzk_axclinicrefrecid"] = Convert.ToDecimal(patientMedication.clinicRecId);

                if (!string.IsNullOrEmpty(patientMedication.orderingLocationId))
                    patientOrderEntity.Attributes["mzk_orderinglocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientMedication.orderingLocationId));
                
                if (!string.IsNullOrEmpty(patientMedication.treatmentLocationId))
                    patientOrderEntity.Attributes["mzk_treatmentlocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientMedication.treatmentLocationId));

                if (!string.IsNullOrEmpty(patientMedication.PRNIndication))
                    patientOrderEntity.Attributes["mzk_prnindication"] = new OptionSetValue(Convert.ToInt32(patientMedication.PRNIndication));

                if (!string.IsNullOrEmpty(patientMedication.Cause))
                    patientOrderEntity.Attributes["mzk_cause"] = patientMedication.Cause;

                if (patientMedication.OrderDate != DateTime.MinValue)
                    patientOrderEntity.Attributes["mzk_orderdate"] = patientMedication.OrderDate;
                else
                    patientOrderEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;

                patientOrderEntity.Attributes["mzk_fulfillmentdate"] = patientOrderEntity.Attributes["mzk_orderdate"];

                if (!string.IsNullOrEmpty(patientMedication.Urgency))
                    patientOrderEntity.Attributes["mzk_urgency"] = new OptionSetValue(Convert.ToInt32(patientMedication.Urgency));

                if (patientMedication.Refill!=0)
                    patientOrderEntity.Attributes["mzk_refill"] = patientMedication.Refill;

                patientOrderEntity.Attributes["mzk_type"] = new OptionSetValue(1);

                if (patientMedication.Other != string.Empty && patientMedication.Other != null)
                    patientOrderEntity.Attributes["mzk_other"] = patientMedication.Other;
                bool isDuplicateAllowed = false;
                if (!string.IsNullOrEmpty(patientMedication.EncounterId) && !string.IsNullOrEmpty(patientMedication.MedicationName))
                    isDuplicateAllowed = new PatientEncounter().DuplicateDetection(patientMedication.EncounterId, patientMedication.MedicationName);

                if (isDuplicateAllowed == true)
                {
                    OrderNumber = Convert.ToString(entityRepository.CreateEntity(patientOrderEntity));
                }
                else
                {
                    throw new ValidationException("Same medication cannot be added multiple times.");
                }

                if (string.IsNullOrEmpty(patientMedication.Urgency))
                    patientMedication.Urgency = "1";
                mzk_patientordermzk_Urgency orderUrgency = (mzk_patientordermzk_Urgency)Convert.ToInt32(patientMedication.Urgency);

                if (OrderNumber.ToString() != null)
                {
                    PatientEncounter encounter = new PatientEncounter();
                    bool isRxNumber = encounter.checkRXNumber(patientMedication.EncounterId);
                    if (isRxNumber == false)
                    {
                      int rxNumber = Convert.ToInt32( encounter.getRXNuber(patientMedication.EncounterId))+1;
                      string formatedRXNumber = rxNumber.ToString().PadLeft(15, '0');
                      encounter.updateRxNumber(patientMedication.EncounterId, formatedRXNumber);
                    }

                    List<decimal> qtyList = new List<decimal>();
                    /*
                    if (!this.IsNullOrEmptyArray(patientMedication.Dose))
                    {
                        for (int count = 0; count < patientMedication.Dose.Length; count++)
                        {
                            Entity doseEntity = new Entity("mzk_dose");

                            if (!string.IsNullOrEmpty(patientMedication.DoseType))
                            {
                                doseEntity.Attributes["mzk_dosetype"] = new OptionSetValue(Convert.ToInt32(patientMedication.DoseType));
                                type = Convert.ToInt32(patientMedication.DoseType);
                                //mzk_dosemzk_DoseType
                                switch (type)
                                {                                   
                                    case 1:
                                        if (count == 0)
                                        {
                                            unit = Models.Unit.getUnitFieldValue(new Guid(patientMedication.DoseUnit[count]), "mzk_code");
                                        }

                                        dose = 0;

                                        if (orderUrgency == mzk_patientordermzk_Urgency.Stat)
                                        {
                                            frequency = 1;
                                        }
                                        else
                                        {
                                            frequency = 0;

                                            dose = decimal.Parse(patientMedication.Dose[count]);
                                            tempFreq = new OrderSetup().getDescription(new Guid(patientMedication.DoseFrequency[count]), "mzk_frequencyperday");

                                            if (!string.IsNullOrEmpty(tempFreq))
                                            {
                                                frequency = decimal.Parse(tempFreq);
                                            }
                                        }

                                        qtyList.Add(dose * frequency);

                                        break;
                                    case 2:
                                        if (count == 0)
                                        {
                                            unit = Models.Unit.getUnitFieldValue(new Guid(patientMedication.DoseUnit[count]), "mzk_code");
                                            dose = 0;                                            
                                        }

                                        if (decimal.Parse(patientMedication.Dose[count]) > dose)
                                        {
                                            dose = decimal.Parse(patientMedication.Dose[count]);
                                        }                                      
                                        break;
                                    case 3:
                                        if (count == 0)
                                        {
                                            unit = Models.Unit.getUnitFieldValue(new Guid(patientMedication.DoseUnit[count]), "mzk_code");                                            
                                        }

                                        dose = 0;
                                        frequency = 0;
                                        duration = 0;

                                        dose = decimal.Parse(patientMedication.Dose[count]);

                                        if (orderUrgency == mzk_patientordermzk_Urgency.Stat)
                                        {
                                            frequency = 1;
                                        }
                                        else
                                        {
                                            tempFreq = new OrderSetup().getDescription(new Guid(patientMedication.DoseFrequency[count]), "mzk_frequencyperday");

                                            if (!string.IsNullOrEmpty(tempFreq))
                                            {
                                                frequency = decimal.Parse(tempFreq);
                                            }
                                        }

                                        duration = decimal.Parse(patientMedication.DoseDuration[count]);

                                        qtyList.Add(dose * frequency * duration);

                                        break;
                                }
                            }                                

                            if (!string.IsNullOrEmpty(OrderNumber))
                                doseEntity.Attributes["mzk_patientorderid"] = new EntityReference("mzk_patientorder", new Guid(OrderNumber));

                            if (!string.IsNullOrEmpty(patientMedication.MedicationName))
                                doseEntity.Attributes["mzk_medicationnameid"] = new EntityReference("product", new Guid(patientMedication.MedicationName));

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseUnit))
                                doseEntity.Attributes["mzk_unitid"] = new EntityReference("mzk_unit", new Guid(patientMedication.DoseUnit[count]));

                            if (!this.IsNullOrEmptyArray(patientMedication.Dose))
                                doseEntity.Attributes["mzk_dose"] = patientMedication.Dose[count];

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseDuration))
                                doseEntity.Attributes["mzk_duration"] = patientMedication.DoseDuration[count];

                            if (!this.IsNullOrEmptyArrayDate(patientMedication.DoseEndDate))
                                doseEntity.Attributes["mzk_enddate"] = Convert.ToDateTime(patientMedication.DoseEndDate[count]);

                            if (!this.IsNullOrEmptyArrayDate(patientMedication.DoseStartDate))
                                doseEntity.Attributes["mzk_startdate"] = Convert.ToDateTime(patientMedication.DoseStartDate[count]);

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseFrequency))
                                doseEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientMedication.DoseFrequency[count]));

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseInstructions))
                                doseEntity.Attributes["mzk_instructions"] = patientMedication.DoseInstructions[count];

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseSpecialdosing) && patientMedication.DoseSpecialdosing[count] == "1")
                                doseEntity.Attributes["mzk_specialdosing"] = true;
                            else
                                doseEntity.Attributes["mzk_specialdosing"] = false;

                            DoseNumber = Convert.ToString(entityRepository.CreateEntity(doseEntity));
                        }
                    }*/

                    if (!string.IsNullOrEmpty(patientMedication.EncounterId))
                    {
                        if (orderUrgency == mzk_patientordermzk_Urgency.Stat)
                        {
                            frequency = 1;
                        }

                        decimal qty = this.getQuantity(type, dose, duration, frequency, qtyList);

                        HMUrgency urgency = HMUrgency.None;

                        if (orderUrgency == mzk_patientordermzk_Urgency.Routine)
                        {
                            urgency = HMUrgency.Routine;
                        }
                        else if (orderUrgency == mzk_patientordermzk_Urgency.Stat)
                        {
                            urgency = HMUrgency.Stat;
                        }
                        mzk_patientordermzk_PRNIndication prnIndication = 0;

                        if (patientMedication.PRNIndication != string.Empty && patientMedication.PRNIndication != null)
                            prnIndication = (mzk_patientordermzk_PRNIndication)Convert.ToInt32(patientMedication.PRNIndication);

                        if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                        {
                            if (!string.IsNullOrEmpty(patientMedication.EncounterId))
                            {
                                if (patientOrderEntity.Attributes.Contains("mzk_treatmentlocation"))
                                {
                                    Clinic clinic = new Clinic().getClinicDetails(patientOrderEntity.GetAttributeValue<EntityReference>("mzk_treatmentlocation").Id.ToString());
                                    await this.createCaseTrans(patientMedication.EncounterId, OrderNumber, patientMedication.MedicationName, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value, qty, unit, urgency, patientMedication.NotesforPharmacist, "", "", 0, clinic.mzk_axclinicrefrecid, prnIndication);
                                }
                                else
                                {
                                    await this.createCaseTrans(patientMedication.EncounterId, OrderNumber, patientMedication.MedicationName, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value, qty, unit, urgency, patientMedication.NotesforPharmacist, "", "", 0, 0, prnIndication);
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(OrderNumber))
                {
                    entityRepository.DeleteEntity(mzk_patientorder.EntityLogicalName, new Guid(OrderNumber));
                }

                throw ex;
            }
            return OrderNumber;
        }

        public decimal getQuantity(int type, decimal dose, decimal duration, decimal frequency, List<decimal> qtyList)
        {
            decimal qty = 0;

            switch (type)
            {
                case 4:
                    qty = dose * duration * frequency;
                    break;
                case 1:
                    if (qtyList != null && qtyList.Count() > 0)
                    {
                        qty = qtyList.Sum() * duration;
                    }                   
                   
                    break;
                case 2:
                    qty = dose * duration * frequency;
                    break;
                case 3:
                    if (qtyList != null && qtyList.Count() > 0)
                    {
                        qty = qtyList.Sum();
                    }
                    break;
            }

            return qty;
        }
        public async Task<bool> updatePatientOrder(PatientMedication patientMedication)
        {
            try
            {
                SoapEntityRepository enityRepository = SoapEntityRepository.GetService();
                Entity encounterEntity = enityRepository.GetEntity("mzk_patientorder", new Guid(patientMedication.Id) { }
                    , new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_unitid", "mzk_notesforpharmacist", "mzk_routeid", "mzk_associateddiagnosisid", "mzk_other", "mzk_medicationordertype", "mzk_instructionstopatients",
                    "mzk_dose", "mzk_prnindication", "mzk_duration", "mzk_enddate", "mzk_startdate", "mzk_cause", "mzk_frequencyid", "mzk_discontinue"));

                if (patientMedication.Unit != string.Empty && patientMedication.Unit != null)
                    encounterEntity.Attributes["mzk_unitid"] = new EntityReference("mzk_unit", new Guid(patientMedication.Unit));

                if (patientMedication.NotesforPharmacist != string.Empty && patientMedication.NotesforPharmacist != null)
                    encounterEntity.Attributes["mzk_notesforpharmacist"] = patientMedication.NotesforPharmacist;

                if (patientMedication.Route != string.Empty && patientMedication.Route != null)
                    encounterEntity.Attributes["mzk_routeid"] = new EntityReference("mzk_ordersetup", new Guid(patientMedication.Route));

                if (patientMedication.AssociatedDiagnosis != string.Empty && patientMedication.AssociatedDiagnosis != null)
                    encounterEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_ordersetup", new Guid(patientMedication.AssociatedDiagnosis));

                if (patientMedication.Other != string.Empty && patientMedication.Other != null)
                    encounterEntity.Attributes["mzk_other"] = patientMedication.Other;

                if (patientMedication.MedicationOrderType != string.Empty && patientMedication.MedicationOrderType != null)
                    encounterEntity.Attributes["mzk_medicationordertype"] = new OptionSetValue(Convert.ToInt32(patientMedication.MedicationOrderType));

                if (patientMedication.Dosage != string.Empty && patientMedication.Dosage != null)
                    encounterEntity.Attributes["mzk_dose"] = patientMedication.Dosage;

                if (patientMedication.Instructionstopatients != string.Empty && patientMedication.Instructionstopatients != null)
                    encounterEntity.Attributes["mzk_instructionstopatients"] = patientMedication.Instructionstopatients;

                if (patientMedication.PRNIndication != string.Empty && patientMedication.PRNIndication != null)
                    encounterEntity.Attributes["mzk_prnindication"] = new OptionSetValue(Convert.ToInt32(patientMedication.PRNIndication));
                else
                    encounterEntity.Attributes["mzk_prnindication"] = null;

                if (patientMedication.Duration != string.Empty && patientMedication.Duration != null)
                    encounterEntity.Attributes["mzk_duration"] = patientMedication.Duration;

                if (patientMedication.EndDate != DateTime.MinValue)
                    encounterEntity.Attributes["mzk_enddate"] = Convert.ToDateTime(patientMedication.EndDate);

                if (patientMedication.Cause != string.Empty && patientMedication.Cause != null)
                    encounterEntity.Attributes["mzk_cause"] = patientMedication.Cause;

                if (patientMedication.StartDate != DateTime.MinValue)
                    encounterEntity.Attributes["mzk_startdate"] = Convert.ToDateTime(patientMedication.StartDate);

                if (patientMedication.Frequency != string.Empty && patientMedication.Frequency != null)
                    encounterEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientMedication.Frequency));
                
                if (!string.IsNullOrEmpty(patientMedication.treatmentLocationId))
                    encounterEntity.Attributes["mzk_treatmentlocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientMedication.treatmentLocationId));

                if (!string.IsNullOrEmpty(patientMedication.Discontinue) && patientMedication.Discontinue == "1")
                {
                    encounterEntity.Attributes["mzk_discontinue"] = true;
                    encounterEntity.Attributes["mzk_orderstatus"] = new OptionSetValue(Convert.ToInt32(mzk_orderstatus.Discontinued));
                }
                else
                {
                    encounterEntity.Attributes["mzk_discontinue"] = false;
                    encounterEntity.Attributes["mzk_orderstatus"] = new OptionSetValue(Convert.ToInt32(mzk_orderstatus.Ordered));
                }

                if (!string.IsNullOrEmpty(patientMedication.Urgency))
                    encounterEntity.Attributes["mzk_urgency"] = new OptionSetValue(Convert.ToInt32(patientMedication.Urgency));

                if (patientMedication.Refill != 0)
                    encounterEntity.Attributes["mzk_refill"] = patientMedication.Refill;

                enityRepository.UpdateEntity(encounterEntity);
                /*if (!this.IsNullOrEmptyArray(patientMedication.DoseId))
                {
                    for (int count = 0; count < patientMedication.DoseId.Length; count++)
                    {
                        if (patientMedication.DoseId[count] != "0")
                        {
                            Entity doseEntity = enityRepository.GetEntity("mzk_dose", new Guid(patientMedication.DoseId[count]) { }
                           , new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_dosetype", "mzk_dose", "mzk_startdate", "mzk_enddate", "mzk_frequencyid", "mzk_instructions", "mzk_specialdosing", "mzk_unitid", "mzk_duration"));

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseUnit))
                                doseEntity.Attributes["mzk_unitid"] = new EntityReference("mzk_unit", new Guid(patientMedication.DoseUnit[count]));

                            if (!this.IsNullOrEmptyArray(patientMedication.Dose))
                                doseEntity.Attributes["mzk_dose"] = patientMedication.Dose[count];

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseDuration))
                                doseEntity.Attributes["mzk_duration"] = patientMedication.DoseDuration[count];
                           
                            if (!this.IsNullOrEmptyArrayDate(patientMedication.DoseEndDate))
                                doseEntity.Attributes["mzk_enddate"] = patientMedication.DoseEndDate[count];

                            if (!this.IsNullOrEmptyArrayDate(patientMedication.DoseStartDate))
                                doseEntity.Attributes["mzk_startdate"] = patientMedication.DoseStartDate[count];

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseFrequency))
                                doseEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientMedication.DoseFrequency[count]));

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseInstructions))
                                doseEntity.Attributes["mzk_instructions"] = patientMedication.DoseInstructions[count];

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseSpecialdosing) && patientMedication.DoseSpecialdosing[count] == "1")
                                doseEntity.Attributes["mzk_specialdosing"] = true;
                            else
                                doseEntity.Attributes["mzk_specialdosing"] = false;

                            enityRepository.UpdateEntity(doseEntity);
                        }
                        else
                        {
                            SoapEntityRepository enityRepositoryDose = SoapEntityRepository.GetService();
                            Entity doseEntity = new Entity("mzk_dose");
                            doseEntity.Attributes["mzk_patientorderid"] = new EntityReference("mzk_patientorder", new Guid(patientMedication.Id));
                            if (!string.IsNullOrEmpty(patientMedication.MedicationName))
                                doseEntity.Attributes["mzk_medicationnameid"] = new EntityReference("product", new Guid(patientMedication.MedicationName));

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseUnit))
                                doseEntity.Attributes["mzk_unitid"] = new EntityReference("mzk_unit", new Guid(patientMedication.DoseUnit[count]));

                            if (!this.IsNullOrEmptyArray(patientMedication.Dose))
                                doseEntity.Attributes["mzk_dose"] = patientMedication.Dose[count];

                            if (!string.IsNullOrEmpty(patientMedication.DoseType))
                                doseEntity.Attributes["mzk_dosetype"] = new OptionSetValue(Convert.ToInt32(patientMedication.DoseType));

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseDuration))
                                doseEntity.Attributes["mzk_duration"] = patientMedication.DoseDuration[count];

                            if (!this.IsNullOrEmptyArrayDate(patientMedication.DoseEndDate))
                                doseEntity.Attributes["mzk_enddate"] = patientMedication.DoseEndDate[count];

                            if (!this.IsNullOrEmptyArrayDate(patientMedication.DoseStartDate))
                                doseEntity.Attributes["mzk_startdate"] = patientMedication.DoseStartDate[count];

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseFrequency))
                                doseEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientMedication.DoseFrequency[count]));

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseInstructions))
                                doseEntity.Attributes["mzk_instructions"] = patientMedication.DoseInstructions[count];

                            if (!this.IsNullOrEmptyArray(patientMedication.DoseSpecialdosing) && patientMedication.DoseSpecialdosing[count] == "1")
                                doseEntity.Attributes["mzk_specialdosing"] = true;
                            else
                                doseEntity.Attributes["mzk_specialdosing"] = false;

                            DoseNumber = Convert.ToString(enityRepositoryDose.CreateEntity(doseEntity));
                        }

                    }
                }
                
                if (!this.IsNullOrEmptyArray(patientMedication.DoseDeleteId))
                {
                    for (int deldosecount = 0; deldosecount < patientMedication.DoseDeleteId.Length; deldosecount++)
                    {
                        SoapEntityRepository doseenityRepository = SoapEntityRepository.GetService();
                        doseenityRepository.DeleteEntity("mzk_dose", new Guid(patientMedication.DoseDeleteId[deldosecount]));
                    }
                }*/
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<bool> deleteMedicationDose(string Id)
        {
            try
            {
                SoapEntityRepository enityRepository = SoapEntityRepository.GetService();
                enityRepository.DeleteEntity("mzk_dose", new Guid(Id));   
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> createPatientOrderLog(string patientOrderId, string doseId , DateTime date , string skipReasonId)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                if (!string.IsNullOrEmpty(patientOrderId))
                {
                    Entity patientOrder = repo.GetEntity("mzk_patientorder", new Guid(patientOrderId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_prescribeddose", "mzk_dosevalue", "mzk_routeid", "mzk_unitid"));
                    Entity PatientOrderLog = new Entity("mzk_patientorderadministration");
                    if (!string.IsNullOrEmpty(skipReasonId))
                    {
                        PatientOrderLog["mzk_skipreason"] = new EntityReference("mzk_reasoncode",new Guid(skipReasonId));
                        PatientOrderLog["mzk_administrationstatus"] = new OptionSetValue(275380001); //Administration Status set to Skipped.
                    }
                    PatientOrderLog["mzk_patientorder"] = new EntityReference("mzk_patientorder", new Guid(patientOrderId));
                    
                    if (patientOrder.Attributes.Contains("mzk_routeid"))
                        PatientOrderLog["mzk_route"] = new EntityReference("mzk_ordersetup", new Guid((patientOrder["mzk_routeid"] as EntityReference).Id.ToString()));
                    PatientOrderLog["mzk_selfadministered"] = true;
                    if(date >  DateTime.MinValue)
                    PatientOrderLog["mzk_starttime"] = date;

                    if (!string.IsNullOrEmpty(doseId))
                    {
                        Entity medicationDose = repo.GetEntity("mzk_dose", new Guid(doseId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_medicationdose", "mzk_unitid"));
                        if (medicationDose.Attributes.Contains("mzk_medicationdose"))
                           PatientOrderLog["mzk_qtyadministered"] = Convert.ToDecimal(medicationDose["mzk_medicationdose"]);
                        if (medicationDose.Attributes.Contains("mzk_unitid"))
                            PatientOrderLog["mzk_unit"] = new EntityReference("mzk_unit", new Guid((medicationDose["mzk_unitid"] as EntityReference).Id.ToString()));
                    }
                    else
                    {
                        if (patientOrder.Attributes.Contains("mzk_dosevalue"))
                            PatientOrderLog["mzk_qtyadministered"] = Convert.ToDecimal(patientOrder["mzk_dosevalue"]);
                        if (patientOrder.Attributes.Contains("mzk_unitid"))
                            PatientOrderLog["mzk_unit"] = new EntityReference("mzk_unit", new Guid((patientOrder["mzk_unitid"] as EntityReference).Id.ToString()));
                    }
                    repo.CreateEntity(PatientOrderLog);
                    return true;
                }
                else
                {
                    throw new ValidationException("Patient Order Id not found");
                }
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        public  bool IsNullOrEmptyArray(string[] array)
        {
            return (array == null || array.Length == 0);
        }
        public bool IsNullOrEmptyArrayDate(DateTime[] array)
        {
            return (array == null || array.Length == 0);
        }

        public async Task<List<PatientMedication>> getPatientOrderHistory(string patientguid, bool isActive)
        {
            return this.getPatientOrder(patientguid, "", isActive ? "Active" : "", "", DateTime.MinValue, DateTime.MinValue, false, "", "", 0, true, false).Result;
        }
        public List<Reminder> getDose(string patientOrderId)
        {
            List<Reminder> Reminders = new List<Reminder>();
            QueryExpression query = new QueryExpression("mzk_patientorder");
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_routeid");
            query.Criteria.AddCondition("mzk_patientorderid",ConditionOperator.Equal,new Guid(patientOrderId));
            LinkEntity EntityDose = new LinkEntity("mzk_patientorder", "mzk_dose", "mzk_patientorderid", "mzk_medicationorder", JoinOperator.Inner);
            EntityDose.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_doseid","mzk_startdate", "mzk_enddate", "mzk_medicationdose", "mzk_unitid", "mzk_frequencyid");
            EntityDose.EntityAlias = "Dose";
            LinkEntity DoseOrder = new LinkEntity("mzk_dose", "mzk_ordersetup", "mzk_frequencyid", "mzk_ordersetupid", JoinOperator.Inner);
            DoseOrder.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_frequencyday", "mzk_reminderafterdays");
            DoseOrder.EntityAlias = "DoseOrder";
            EntityDose.LinkEntities.Add(DoseOrder);
            query.LinkEntities.Add(EntityDose);
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entityCollection = entityRepository.GetEntityCollection(query);
            foreach (Entity entity in entityCollection.Entities)
            {
                Reminder Reminder = new Reminder();
                if (entity.Attributes.Contains("Dose.mzk_doseid"))
                    Reminder.doseId = entity.GetAttributeValue<AliasedValue>("Dose.mzk_doseid").Value.ToString();
                if (entity.Attributes.Contains("Dose.mzk_startdate"))
                    Reminder.startDateTime = (DateTime)entity.GetAttributeValue<AliasedValue>("Dose.mzk_startdate").Value;
                if (entity.Attributes.Contains("Dose.mzk_enddate"))
                    Reminder.endDateTime = (DateTime)entity.GetAttributeValue<AliasedValue>("Dose.mzk_enddate").Value;
                if (entity.Attributes.Contains("DoseOrder.mzk_frequencyday"))
                    Reminder.frequencyPerDay = entity.GetAttributeValue<AliasedValue>("DoseOrder.mzk_frequencyday").Value.ToString();
                if (entity.Attributes.Contains("DoseOrder.mzk_reminderafterdays"))
                    Reminder.reminderAfterDays = entity.GetAttributeValue<AliasedValue>("DoseOrder.mzk_reminderafterdays").Value.ToString();
                if (entity.Attributes.Contains("Dose.mzk_medicationdose") && entity.Attributes.Contains("Dose.mzk_unitid") && entity.Attributes.Contains("Dose.mzk_frequencyid"))
                    Reminder.instruction = "Use " + entity.GetAttributeValue<AliasedValue>("Dose.mzk_medicationdose").Value.ToString() + " " + (entity.GetAttributeValue<AliasedValue>("Dose.mzk_unitid").Value as EntityReference).Name + " " + (entity["mzk_routeid"] as EntityReference).Name;
                Reminders.Add(Reminder);
            }


            return Reminders;
        }
        public List<string> getMedicationInstructions(string patientOrderId)
        {
            List<string> Instructions = new List<string>();
            QueryExpression query = new QueryExpression("mzk_patientorder");
            query.Criteria.AddCondition("mzk_patientorderid", ConditionOperator.Equal, new Guid(patientOrderId));
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_routeid");
            LinkEntity EntityDose = new LinkEntity("mzk_patientorder", "mzk_dose", "mzk_patientorderid", "mzk_medicationorder", JoinOperator.Inner);
            EntityDose.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_startdate", "mzk_enddate", "mzk_medicationdose", "mzk_unitid", "mzk_frequencyid");
            EntityDose.EntityAlias = "Dose";
            query.LinkEntities.Add(EntityDose);
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entityCollection = entityRepository.GetEntityCollection(query);
            foreach (Entity entity in entityCollection.Entities)
            {
                if (entity.Attributes.Contains("mzk_routeid") && entity.Attributes.Contains("Dose.mzk_startdate") && entity.Attributes.Contains("Dose.mzk_enddate") && entity.Attributes.Contains("Dose.mzk_medicationdose")
                    && entity.Attributes.Contains("Dose.mzk_unitid") && entity.Attributes.Contains("Dose.mzk_frequencyid"))
                {
                    Instructions.Add("Use " + entity.GetAttributeValue<AliasedValue>("Dose.mzk_medicationdose").Value.ToString() + 
                        " " + (entity.GetAttributeValue<AliasedValue>("Dose.mzk_unitid").Value as EntityReference).Name + 
                        " " + (entity["mzk_routeid"] as EntityReference).Name + 
                        " " + (entity.GetAttributeValue<AliasedValue>("Dose.mzk_frequencyid").Value as EntityReference).Name + 
                        " from " + Convert.ToDateTime(entity.GetAttributeValue<AliasedValue>("Dose.mzk_startdate").Value).ToShortDateString() + " to " + Convert.ToDateTime(entity.GetAttributeValue<AliasedValue>("Dose.mzk_enddate").Value).ToShortDateString());
                }
  
            }
            return Instructions;
        }
    }
}
