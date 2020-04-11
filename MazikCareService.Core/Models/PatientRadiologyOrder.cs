using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Interfaces;
using MazikCareService.Core.Models.HL7;
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
    public class PatientRadiologyOrder : PatientOrder, IPatientRadiologyOrder
    {

        public string TestName { get; set; }
        public string AssociatedDiagnosis { get; set; }
        public string AssociatedDiagnosisId { get; set; }
        public string Frequency { get; set; }
        public string FrequencyId { get; set; }
        public string StudyDate { get; set; }
        public string StudyDateText { get; set; }
        public string ClinicalNotes { get; set; }
        public string UrgencyId { get; set; }
        public string LocationCode { get; set; }
        public string ResultStatus { get; set; }

        public string ProductFamilyCode { get; set; }
        public string ResultStatusText { get; set; }
        public string RISLink { get; set; }
        public string ReportPath { get; set; }
        public string Specialty { get; set; }
        public bool registered { get; set; }
        public DateTime ScheduleStartDateTime { get; set; }
        public DateTime ScheduleEndDateTime { get; set; }


        public async Task<List<PatientRadiologyOrder>> getPatientOrder(string patientguid, string patientEncounter, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId, string caseId = null, bool fromRIs = false, int pageNumber = 0)
        {
            List<PatientRadiologyOrder> PatientRadiologyOrder = new List<PatientRadiologyOrder>();
            try
            {
                if (string.IsNullOrEmpty(patientguid) && string.IsNullOrEmpty(caseId) && string.IsNullOrEmpty(patientEncounter) && string.IsNullOrEmpty(orderId))
                    throw new ValidationException("Parameter missing");

                #region Patient Radiology Order Query
                QueryExpression query = new QueryExpression("mzk_patientorder");
                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);

                if (!string.IsNullOrEmpty(orderId))
                {
                    childFilter.AddCondition("mzk_patientorderid", ConditionOperator.Equal, new Guid(orderId));
                }

                if (!fromRIs && SearchFilters != mzk_orderstatus.Cancelled.ToString())
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.NotEqual, (int)mzk_orderstatus.Cancelled);

                if (!string.IsNullOrEmpty(caseId))
                {
                    childFilter.AddCondition("mzk_caseid", ConditionOperator.Equal, new Guid(caseId));
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

                childFilter.AddCondition("mzk_type", ConditionOperator.Equal, "3");
                //Patient Order Type :: Radiology
                if (!string.IsNullOrEmpty(SearchFilters))
                {

                    if (SearchFilters == Convert.ToString(mzk_radiologyfilter.Ordered))
                        childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Ordered));
                    if (SearchFilters == Convert.ToString(mzk_radiologyfilter.Paid))
                        childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Paid));
                    if (SearchFilters == Convert.ToString(mzk_radiologyfilter.Cancelled))
                        childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Cancelled));
                }
                //Search Order
                if (!string.IsNullOrEmpty(searchOrder))
                    childFilter.AddCondition("mzk_productidname", ConditionOperator.Like, ("%" + searchOrder + "%"));

                //Search Date
                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                    childFilter.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_productid",
                                                                        "mzk_patientordernumber",
                                                                        "mzk_associateddiagnosisid",
                                                                        "mzk_frequencyid",
                                                                        "mzk_patientencounterid",
                                                                        "mzk_orderdate",
                                                                        "mzk_clinicalnotes",
                                                                        "mzk_studydate",
                                                                        "mzk_orderstatus",
                                                                        "createdby",
                                                                        "mzk_resultstatus",
                                                                        "mzk_reportpath",
                                                                        "mzk_reporturl",
                                                                        "mzk_axclinicrefrecid",
                                                                        "mzk_statusmanagerdetail",
                                                                        "createdon",
                                                                        "mzk_rislink",
                                                                        "mzk_fulfillmentdate",
                                                                        "mzk_fulfillmentappointment",
                                                                        "mzk_orderingappointment",
                                                                        "mzk_treatmentlocation", "mzk_orderinglocation");

                if (!string.IsNullOrEmpty(orderId))
                {
                    LinkEntity Resource = new LinkEntity("mzk_patientorder", "systemuser", "createdby", "systemuserid", JoinOperator.Inner);
                    Resource.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname", "systemuserid");
                    Resource.EntityAlias = "Resource";
                    query.LinkEntities.Add(Resource);
                }

                LinkEntity EntityDiagnosis = new LinkEntity("mzk_patientorder", "mzk_concept", "mzk_associateddiagnosisid", "mzk_conceptid", JoinOperator.LeftOuter);
                EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_icdcodeid");

                LinkEntity EntityFrequecy = new LinkEntity("mzk_patientorder", "mzk_ordersetup", "mzk_frequencyid", "mzk_ordersetupid", JoinOperator.LeftOuter);
                EntityFrequecy.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description");
                
                OrderExpression orderby = new OrderExpression();
                orderby.AttributeName = "createdon";
                orderby.OrderType = OrderType.Descending;

                LinkEntity ProductRecord = new LinkEntity("mzk_patientorder", "product", "mzk_productid", "productid", JoinOperator.LeftOuter);
                ProductRecord.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("productnumber");
                ProductRecord.EntityAlias = "ProductRecord";

                LinkEntity EntityFamily = new LinkEntity("product", "product", "parentproductid", "productid", JoinOperator.LeftOuter);
                EntityFamily.EntityAlias = "ProductFamily";
                EntityFamily.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("productnumber");
                ProductRecord.LinkEntities.Add(EntityFamily);

                query.LinkEntities.Add(EntityFrequecy);
                query.LinkEntities.Add(EntityDiagnosis);
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

                foreach (Entity entity in entitycollection.Entities)
                {
                    PatientRadiologyOrder model = new PatientRadiologyOrder();

                    if (!this.getPatientOrder(model, entity, forFulfillment, orderId, mzk_entitytype.RadiologyOrder))
                    {
                        continue;
                    }

                    if (entity.Attributes.Contains("mzk_productid"))
                        model.TestName = ((EntityReference)entity["mzk_productid"]).Name;

                    if (entity.Attributes.Contains("ProductRecord.productnumber"))
                        model.TestId = (entity.Attributes["ProductRecord.productnumber"] as AliasedValue).Value.ToString();

                    if (entity.Attributes.Contains("ProductFamily.productnumber"))
                        model.ProductFamilyCode = (entity.Attributes["ProductFamily.productnumber"] as AliasedValue).Value.ToString();

                    if (entity.Attributes.Contains("mzk_frequencyid"))
                        model.FrequencyId = ((EntityReference)entity["mzk_frequencyid"]).Id.ToString();

                    if (entity.Attributes.Contains("createdon"))
                        model.CreatedOn = (DateTime)entity["createdon"];

                    if (entity.Attributes.Contains("mzk_fulfillmentdate"))
                        model.FulfillmentDate = (DateTime)entity["mzk_fulfillmentdate"];

                    if (entity.Attributes.Contains("mzk_ordersetup3.mzk_description"))
                        model.Frequency = (entity.Attributes["mzk_ordersetup3.mzk_description"] as AliasedValue).Value.ToString();
                    else
                        if (entity.Attributes.Contains("mzk_ordersetup1.mzk_description"))
                        model.Frequency = (entity.Attributes["mzk_ordersetup1.mzk_description"] as AliasedValue).Value.ToString();
                    
                    if (entity.Attributes.Contains("mzk_associateddiagnosisid"))
                        model.AssociatedDiagnosisId = ((EntityReference)entity["mzk_associateddiagnosisid"]).Id.ToString();

                    if (entity.Attributes.Contains("mzk_concept4.mzk_conceptname"))
                        model.AssociatedDiagnosis = (entity.Attributes["mzk_concept4.mzk_conceptname"] as AliasedValue).Value.ToString();
                    else
                        if (entity.Attributes.Contains("mzk_concept2.mzk_conceptname"))
                        model.AssociatedDiagnosis = (entity.Attributes["mzk_concept2.mzk_conceptname"] as AliasedValue).Value.ToString();

                    if (entity.Attributes.Contains("mzk_concept4.mzk_icdcodeid"))
                        model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_concept4.mzk_icdcodeid"]).Value).Name;
                    else
                       if (entity.Attributes.Contains("mzk_concept2.mzk_icdcodeid"))
                        model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_concept2.mzk_icdcodeid"]).Value).Name;


                    if (entity.Attributes.Contains("mzk_clinicalnotes"))
                        model.ClinicalNotes = entity["mzk_clinicalnotes"].ToString();

                    if (entity.Attributes.Contains("mzk_studydate"))
                    {
                        model.StudyDate = (entity["mzk_studydate"] as OptionSetValue).Value.ToString();
                        model.StudyDateText = entity.FormattedValues["mzk_studydate"].ToString();
                    }

                    if (entity.Attributes.Contains("mzk_orderinglocation"))
                    {
                        model.orderingLocationId = entity.GetAttributeValue<EntityReference>("mzk_orderinglocation").Id.ToString();
                        model.orderingLocation = entity.GetAttributeValue<EntityReference>("mzk_orderinglocation").Name;
                    }

                    if (entity.Attributes.Contains("mzk_reporturl"))
                        model.RISLink = entity["mzk_reporturl"].ToString();

                    if (entity.Attributes.Contains("mzk_resultstatus"))
                    {
                        model.ResultStatus = (entity["mzk_resultstatus"] as OptionSetValue).Value.ToString();
                        model.ResultStatusText = entity.FormattedValues["mzk_resultstatus"].ToString();
                    }

                    if (entity.Attributes.Contains("mzk_reportpath"))
                        model.ReportPath = entity["mzk_reportpath"].ToString();
                    

                    if (!string.IsNullOrEmpty(orderId))
                    {
                        if (entity.Attributes.Contains("createdby"))
                        {
                            orderingProvider = new User();

                            if (entity.Attributes.Contains("Resource.fullname"))
                                orderingProvider.Name = (entity.Attributes["Resource.fullname"] as AliasedValue).Value.ToString();

                            if (entity.Attributes.Contains("Resource.systemuserid"))
                                orderingProvider.userId = (entity.Attributes["Resource.systemuserid"] as AliasedValue).Value.ToString();

                            model.orderingProvider = orderingProvider;
                        }      

                        if (string.IsNullOrEmpty(model.UrgencyId))
                        {
                            if (string.IsNullOrEmpty(model.StudyDate))
                            {
                                if (!string.IsNullOrEmpty(model.EncounterId))
                                {
                                    mzk_casetype caseType = PatientCase.getCaseType(model.EncounterId);

                                    model.UrgencyId = CaseParameter.getDefaultUrgency(caseType).urgencyId;
                                }
                                else if (entity.Attributes.Contains("mzk_fulfillmentappointment"))
                                {
                                    mzk_casetype caseType = mzk_casetype.OutPatient;

                                    model.UrgencyId = CaseParameter.getDefaultUrgency(caseType).urgencyId;
                                }
                            }
                            else
                            {
                                mzk_patientordermzk_StudyDate studyDate = (mzk_patientordermzk_StudyDate)Convert.ToInt32(model.StudyDate);

                                switch (studyDate)
                                {
                                    case mzk_patientordermzk_StudyDate.Routine:
                                    case mzk_patientordermzk_StudyDate.Thismonth:
                                    case mzk_patientordermzk_StudyDate._2weeks:
                                        model.UrgencyId = ((int)mzk_patientordermzk_Urgency.Routine).ToString();
                                        break;
                                    case mzk_patientordermzk_StudyDate.Urgent:
                                        model.UrgencyId = ((int)mzk_patientordermzk_Urgency.Stat).ToString();
                                        break;
                                }
                            }
                        }

                        if (entity.Attributes.Contains("mzk_fulfillmentappointment"))
                        {
                            Appointment appt = new Appointment().getAppointmentDetails((entity["mzk_fulfillmentappointment"] as EntityReference).Id.ToString());

                            if (appt != null)
                            {
                                model.ScheduleEndDateTime = Convert.ToDateTime(appt.endDateTime);
                                model.ScheduleStartDateTime = Convert.ToDateTime(appt.startDateTime);
                            }
                        }
                    }
                    PatientRadiologyOrder.Add(model);
                }

                if (pageNumber > 0 && entitycollection != null)
                {
                    Pagination.totalCount = entitycollection.TotalRecordCount;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return PatientRadiologyOrder;
        }

        public async Task<List<PatientRadiologyOrder>> getPatientOrder(List<Entity> patientOrders)
        {
            List<PatientRadiologyOrder> PatientRadiologyOrder = new List<PatientRadiologyOrder>();
            try
            {
                foreach (Entity entity in patientOrders)
                {
                    PatientRadiologyOrder model = new PatientRadiologyOrder();

                    if (entity.Attributes.Contains("mzk_productid"))
                        model.TestName = ((EntityReference)entity["mzk_productid"]).Name;

                    if (entity.Attributes.Contains("mzk_clinicalnotes"))
                        model.ClinicalNotes = entity["mzk_clinicalnotes"].ToString();

                    if (entity.Id != null)
                        model.Id = entity.Id.ToString();

                    PatientRadiologyOrder.Add(model);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return PatientRadiologyOrder;
        }

        public async Task<string> addPatientOrder(PatientRadiologyOrder patientRadiologyOrder, bool isActivityOrder = false)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            mzk_patientorder patientOrderEntity = new mzk_patientorder();

            try
            {
                mzk_casetype caseType = mzk_casetype.OutPatient;

                patientOrderEntity.mzk_appointable = true;

                if (!string.IsNullOrEmpty(patientRadiologyOrder.appointmentId))
                {
                    patientOrderEntity.mzk_orderingappointment = new EntityReference("mzk_patientorder", new Guid(patientRadiologyOrder.appointmentId));
                    patientOrderEntity.mzk_fulfillmentappointment = new EntityReference("mzk_patientorder", new Guid(patientRadiologyOrder.appointmentId));

                    if (!string.IsNullOrEmpty(patientRadiologyOrder.PatientId))
                        patientOrderEntity.mzk_customer = new EntityReference("contact", new Guid(patientRadiologyOrder.PatientId));
                    if (!string.IsNullOrEmpty(patientRadiologyOrder.CaseTransRecId))
                        patientOrderEntity.mzk_AXCaseTransRefRecId = Convert.ToDecimal(patientRadiologyOrder.CaseTransRecId);
                    if (!string.IsNullOrEmpty(patientRadiologyOrder.CaseId))
                        patientOrderEntity.mzk_caseid = new EntityReference("incident", new Guid(patientRadiologyOrder.CaseId));

                    if (!string.IsNullOrEmpty(patientRadiologyOrder.TestName))
                    {
                        patientOrderEntity.mzk_ProductId = new EntityReference(xrm.Product.EntityLogicalName, Products.getProductId(patientRadiologyOrder.TestName));

                        if (patientOrderEntity.mzk_ProductId == null && patientOrderEntity.mzk_ProductId.Id == Guid.Empty)
                        {
                            throw new ValidationException("Product not found for the corresponding item. Please contact system administrator");
                        }
                    }

                    patientOrderEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;
                    patientOrderEntity.mzk_FulfillmentDate = patientRadiologyOrder.FulfillmentDate;

                    if (patientRadiologyOrder.clinicRecId > 0)
                        patientOrderEntity.Attributes["mzk_axclinicrefrecid"] = Convert.ToDecimal(patientRadiologyOrder.clinicRecId);                    

                    if (!string.IsNullOrEmpty(patientRadiologyOrder.orderingLocationId))
                        patientOrderEntity.Attributes["mzk_orderinglocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientRadiologyOrder.orderingLocationId));


                    if (!string.IsNullOrEmpty(patientRadiologyOrder.treatmentLocationId))
                        patientOrderEntity.Attributes["mzk_treatmentlocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientRadiologyOrder.treatmentLocationId));
                    
                }
                else if (patientRadiologyOrder.EncounterId != null && patientRadiologyOrder.EncounterId != string.Empty)
                {
                    if (patientRadiologyOrder.TestName != null && patientRadiologyOrder.TestName != string.Empty)
                        patientOrderEntity.Attributes["mzk_productid"] = new EntityReference("product", new Guid(patientRadiologyOrder.TestName));
                    if (patientRadiologyOrder.Frequency != null && patientRadiologyOrder.Frequency != string.Empty)
                        patientOrderEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientRadiologyOrder.Frequency));
                    if (patientRadiologyOrder.AssociatedDiagnosis != null && patientRadiologyOrder.AssociatedDiagnosis != string.Empty)
                        patientOrderEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_concept", new Guid(patientRadiologyOrder.AssociatedDiagnosis));
                    if (patientRadiologyOrder.ClinicalNotes != null && patientRadiologyOrder.ClinicalNotes != string.Empty)
                        patientOrderEntity.Attributes["mzk_clinicalnotes"] = patientRadiologyOrder.ClinicalNotes;
                    if (patientRadiologyOrder.StudyDate != null && patientRadiologyOrder.StudyDate != string.Empty)
                        patientOrderEntity.Attributes["mzk_studydate"] = new OptionSetValue(Convert.ToInt32(patientRadiologyOrder.StudyDate));

                    if (patientRadiologyOrder.clinicRecId > 0)
                        patientOrderEntity.Attributes["mzk_axclinicrefrecid"] = Convert.ToDecimal(patientRadiologyOrder.clinicRecId);

                    if (!string.IsNullOrEmpty(patientRadiologyOrder.orderingLocationId))
                        patientOrderEntity.Attributes["mzk_orderinglocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientRadiologyOrder.orderingLocationId));
                    
                    if (!string.IsNullOrEmpty(patientRadiologyOrder.treatmentLocationId))
                        patientOrderEntity.Attributes["mzk_treatmentlocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientRadiologyOrder.treatmentLocationId));

                    if (patientRadiologyOrder.OrderDate != DateTime.MinValue)
                    {
                        patientOrderEntity.Attributes["mzk_orderdate"] = patientRadiologyOrder.OrderDate;
                    }
                    else
                    {
                        patientOrderEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;
                    }

                    patientOrderEntity.Attributes["mzk_fulfillmentdate"] = patientOrderEntity.Attributes["mzk_orderdate"];
                }

                //for Radiology
                patientOrderEntity.Attributes["mzk_type"] = new OptionSetValue(3);

                if (patientRadiologyOrder.EncounterId != null && patientRadiologyOrder.EncounterId != string.Empty)
                {
                    patientOrderEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(patientRadiologyOrder.EncounterId));
                    PatientEncounter encounter = new PatientEncounter();
                    encounter.EncounterId = patientRadiologyOrder.EncounterId;

                    List<PatientEncounter> listEncounter = encounter.encounterDetails(encounter).Result;
                    
                    if (!string.IsNullOrEmpty(CaseId))
                    {
                        Speciality specialty = new Speciality();
                        patientOrderEntity.Attributes["mzk_specialtyname"] = specialty.getSpeciality(CaseId);
                    }

                    encounter = encounter.encounterDetails(encounter).Result.ToList().First<PatientEncounter>();
                    PatientId = encounter.PatientId;                    
                    caseType = encounter.caseTypeValue;
                    patientOrderEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(PatientId));
                }

                StatusManager statusManager = StatusManager.getRootStatus(mzk_entitytype.RadiologyOrder, caseType, isActivityOrder);

                if (statusManager != null)
                {
                    patientOrderEntity.mzk_OrderStatus = new OptionSetValue(statusManager.status);
                    patientOrderEntity.mzk_StatusManagerDetail = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, new Guid(statusManager.StatusId));
                }        

                bool isDuplicateAllowed = false;
                if (!string.IsNullOrEmpty(patientRadiologyOrder.EncounterId) && !string.IsNullOrEmpty(patientRadiologyOrder.TestName))
                    isDuplicateAllowed = new PatientEncounter().DuplicateDetection(patientRadiologyOrder.EncounterId, patientRadiologyOrder.TestName);
                else
                    isDuplicateAllowed = true;

                if (isDuplicateAllowed == true)
                {
                    Id = Convert.ToString(entityRepository.CreateEntity(patientOrderEntity));

                    if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                    {
                        if (!string.IsNullOrEmpty(patientRadiologyOrder.EncounterId))
                        {
                            if (patientOrderEntity.Attributes.Contains("mzk_treatmentlocation"))
                            {
                                Clinic clinic = new Clinic().getClinicDetails(patientOrderEntity.GetAttributeValue<EntityReference>("mzk_treatmentlocation").Id.ToString());
                                await this.createCaseTrans(patientRadiologyOrder.EncounterId, Id, patientRadiologyOrder.TestName, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value, 1, "", AXRepository.AXServices.HMUrgency.None, "", "", "", 0, clinic.mzk_axclinicrefrecid);
                            }
                            else
                            {
                                await this.createCaseTrans(patientRadiologyOrder.EncounterId, Id, patientRadiologyOrder.TestName, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value, 1, "", AXRepository.AXServices.HMUrgency.None, "", "", "", 0, 0);
                            }
                        }
                    }
                }
                else
                {
                    throw new ValidationException("Same Radiology Test cannot be added multiple times.");
                }

                if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(patientRadiologyOrder.appointmentId) && AppSettings.GetByKey("RISIntegration").ToLower() == true.ToString().ToLower())
                {
                    RIS ris = new RIS();
                    Patient patient = new Patient();

                    if (patientRadiologyOrder.registered)
                    {
                        await patient.updatePatientRIS(patientRadiologyOrder.PatientId);
                    }
                    else
                    {
                        await patient.createPatientRIS(patientRadiologyOrder.appointmentId, patientRadiologyOrder.PatientId);
                    }
                }

                //await this.addContrastOrder(patientRadiologyOrder, Id);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    entityRepository.DeleteEntity(mzk_patientorder.EntityLogicalName, new Guid(Id));
                }

                throw ex;
            }
            return Id.ToString();
        }
        public async Task<bool> updatePatientOrder(PatientRadiologyOrder patientRadiologyOrder)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Entity encounterEntity = entityRepository.GetEntity("mzk_patientorder", new Guid(patientRadiologyOrder.Id) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_frequencyid", "mzk_associateddiagnosisid", "mzk_clinicalnotes", "mzk_studydate", "mzk_orderstatus"));
                
                if (!string.IsNullOrEmpty(patientRadiologyOrder.treatmentLocationId))
                    encounterEntity.Attributes["mzk_treatmentlocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientRadiologyOrder.treatmentLocationId));
                if (patientRadiologyOrder.Frequency != string.Empty && patientRadiologyOrder.Frequency != null)
                    encounterEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientRadiologyOrder.Frequency));
                if (patientRadiologyOrder.AssociatedDiagnosis != string.Empty && patientRadiologyOrder.AssociatedDiagnosis != null)
                    encounterEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_concept", new Guid(patientRadiologyOrder.AssociatedDiagnosis));
                if (patientRadiologyOrder.ClinicalNotes != string.Empty && patientRadiologyOrder.ClinicalNotes != null)
                    encounterEntity.Attributes["mzk_clinicalnotes"] = patientRadiologyOrder.ClinicalNotes;
                if (patientRadiologyOrder.StudyDate != string.Empty && patientRadiologyOrder.StudyDate != null)
                    encounterEntity.Attributes["mzk_studydate"] = new OptionSetValue(Convert.ToInt32(patientRadiologyOrder.StudyDate));
                if (patientRadiologyOrder.OrderStatus != string.Empty && patientRadiologyOrder.OrderStatus != null)
                    encounterEntity.Attributes["mzk_orderstatus"] = new OptionSetValue(Convert.ToInt32(patientRadiologyOrder.OrderStatus));
                if (patientRadiologyOrder.OrderDate != DateTime.MinValue)
                    encounterEntity.Attributes["mzk_orderdate"] = patientRadiologyOrder.OrderDate;

                entityRepository.UpdateEntity(encounterEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> updateResult(PatientRadiologyOrder patientRadiologyOrder)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                mzk_patientorder patientOrderEntity = (mzk_patientorder)entityRepository.GetEntity("mzk_patientorder", new Guid(patientRadiologyOrder.Id) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_orderstatus", "mzk_axcasetransrefrecid"));

                if (patientRadiologyOrder.ReportPath != string.Empty && patientRadiologyOrder.ReportPath != null)
                    patientOrderEntity.mzk_ReportPath = patientRadiologyOrder.ReportPath;

                if (patientRadiologyOrder.ResultStatus != string.Empty && patientRadiologyOrder.ResultStatus != null)
                {
                    mzk_resultstatus resultStatus = mzk_resultstatus.Final;

                    switch (patientRadiologyOrder.ResultStatus)
                    {
                        case "P":
                            resultStatus = mzk_resultstatus.Preliminary;
                            break;
                        case "C":
                            resultStatus = mzk_resultstatus.Corrected;
                            break;
                        case "F":
                            resultStatus = mzk_resultstatus.Final;
                            break;
                    }

                    patientOrderEntity.mzk_ResultStatus = new OptionSetValue((int)resultStatus);
                }

                if (patientRadiologyOrder.RISLink != string.Empty && patientRadiologyOrder.RISLink != null)
                    patientOrderEntity.mzk_ReportURL = patientRadiologyOrder.RISLink;

                if (patientOrderEntity.mzk_AXCaseTransRefRecId.HasValue)
                {
                    CaseRepository caseRepo = new CaseRepository();

                    caseRepo.updateReportUrl((long)Convert.ToDecimal(patientOrderEntity.mzk_AXCaseTransRefRecId.Value), patientRadiologyOrder.RISLink, patientRadiologyOrder.ReportPath);
                }

                entityRepository.UpdateEntity(patientOrderEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> addContrastOrder(PatientRadiologyOrder patientRadiologyOrder, string Id)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            Products prod = new Products();
            try
            {
                prod = prod.getProductDetails(patientRadiologyOrder.TestName);

                if (prod.contrastOrder)
                {
                    QueryExpression query = new QueryExpression(ProductSubstitute.EntityLogicalName);

                    query.Criteria.AddCondition("productid", ConditionOperator.Equal, new Guid(patientRadiologyOrder.TestName));
                    query.Criteria.AddCondition("salesrelationshiptype", ConditionOperator.Equal, (int)ProductSubstituteSalesRelationshipType.Contrast);
                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("substitutedproductid");
                    LinkEntity ProductRecord = new LinkEntity(ProductSubstitute.EntityLogicalName, xrm.Product.EntityLogicalName, "substitutedproductid", "productid", JoinOperator.Inner);
                    ProductRecord.EntityAlias = "ProductRecord";
                    ProductRecord.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_frequencyid", "mzk_urgencyid");
                    ProductRecord.LinkCriteria.AddCondition("mzk_producttype", ConditionOperator.Equal, (int)mzk_producttype.Lab);

                    query.LinkEntities.Add(ProductRecord);

                    EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                    foreach (ProductSubstitute entity in entitycollection.Entities)
                    {
                        PatientLabOrder patientLabOrder = new PatientLabOrder();

                        patientLabOrder.TestName = entity.SubstitutedProductId.Id.ToString();

                        if (entity.Attributes.Contains("ProductRecord.mzk_frequencyid"))
                            patientLabOrder.Frequency = ((EntityReference)(entity.Attributes["ProductRecord.mzk_frequencyid"] as AliasedValue).Value).Id.ToString();

                        patientLabOrder.AssociatedDiagnosis = patientRadiologyOrder.AssociatedDiagnosis;
                        patientLabOrder.ClinicalNotes = patientRadiologyOrder.ClinicalNotes;
                        patientLabOrder.EncounterId = patientRadiologyOrder.EncounterId;
                        patientLabOrder.PatientId = patientRadiologyOrder.PatientId;
                        patientLabOrder.OrderDate = patientRadiologyOrder.OrderDate;
                        patientLabOrder.clinicRecId = patientRadiologyOrder.clinicRecId;
                        patientLabOrder.orderingLocationId = patientRadiologyOrder.orderingLocationId;
                        patientLabOrder.ParentOrderId = Id;

                        if (entity.Attributes.Contains("ProductRecord.mzk_urgencyid"))
                            patientLabOrder.UrgencyId = ((EntityReference)(entity.Attributes["ProductRecord.mzk_urgencyid"] as AliasedValue).Value).Id.ToString();

                        //patientLabOrder.AntibioticsComments;
                        //patientLabOrder.Antibiotics

                        if (string.IsNullOrEmpty(await patientLabOrder.addPatientOrder(patientLabOrder)))
                        {
                            throw new ValidationException("Unable to create contrast order");
                        }
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
