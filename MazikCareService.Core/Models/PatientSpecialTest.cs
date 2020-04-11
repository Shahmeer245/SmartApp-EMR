using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using MazikLogger;
using xrm;
using Helper;
using MazikCareService.AXRepository.AXServices;

namespace MazikCareService.Core.Models
{
    public class PatientSpecialTest : PatientOrder, IPatientSpecialTest
    {
        public string TestName { get; set; }
        public string Diagnosis { get; set; }
        public string DiagnosisId { get; set; }
        public string Frequency { get; set; }
        public string FrequencyId { get; set; }
        public string AssociatedDiagnosis { get; set; }
        public string AssociatedDiagnosisId { get; set; }
        //public string Importance { get; set; }
        //public string ImportanceId { get; set; }
        public string UrgencyId { get; set; }
        public string UrgencyName { get; set; }
        public string OrderingPhysician { get; set; }
        public string OrderingPhysicianId { get; set; }
        public string ClinicalNotes { get; set; }
     
        public QueryExpression getSpecialTestQuery(string patientguid, string patientEncounter,string SearchFilters,string searchOrder,DateTime startDate,DateTime endDate, bool forFulfillment, string orderId,string caseId=null, int pageNumber = 0)
        {
            QueryExpression query = new QueryExpression("mzk_patientorder");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            if (!string.IsNullOrEmpty(orderId))
            {
                childFilter.AddCondition("mzk_patientorderid", ConditionOperator.Equal, new Guid(orderId));
            }
            if (SearchFilters != mzk_orderstatus.Cancelled.ToString())
            {
                childFilter.AddCondition("mzk_orderstatus", ConditionOperator.NotEqual, (int)mzk_orderstatus.Cancelled);
            }
            if (!string.IsNullOrEmpty(caseId))
            {
                childFilter.AddCondition("mzk_caseid", ConditionOperator.Equal, new Guid(caseId));
            }

            if (!string.IsNullOrEmpty(patientguid))
            {
                childFilter.AddCondition("mzk_customer", ConditionOperator.Equal, new Guid(patientguid));
            }
            else if (!string.IsNullOrEmpty(patientEncounter))
            {
                childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(patientEncounter));
            }
            //Patient Order Type :: Special test
            childFilter.AddCondition("mzk_type", ConditionOperator.Equal, "6");
            
            //Search Filter
            if (!string.IsNullOrEmpty(SearchFilters))
            {
                if (SearchFilters == Convert.ToString(mzk_specialtestfilter.Ordered))
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Ordered));
                if (SearchFilters == Convert.ToString(mzk_specialtestfilter.Paid))
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Paid));
                if (SearchFilters == Convert.ToString(mzk_specialtestfilter.Cancelled))
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Cancelled));
            }

            //Search Order
            if (!string.IsNullOrEmpty(searchOrder))
                childFilter.AddCondition("mzk_productidname", ConditionOperator.Like, ("%" + searchOrder + "%"));

            //Search Date
            if (startDate!= DateTime.MinValue && endDate !=DateTime.MinValue)
            childFilter.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate,endDate.AddHours(12)});

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_userid",
                                                                    "ownerid",
                                                                    "mzk_orderdate",
                                                                    "mzk_patientencounterid",
                                                                     "mzk_urgency",
                                                                    "mzk_frequencyid",
                                                                    "mzk_associateddiagnosisid",
                                                                    "mzk_productid",
                                                                    "mzk_patientordernumber",
                                                                    "mzk_statusmanagerdetail",
                                                                    "mzk_orderstatus","createdon",
                                                                    "mzk_clinicalnotes",
                                                                    "mzk_treatmentlocation", "mzk_orderinglocation");

            LinkEntity EntityDiagnosis = new LinkEntity("mzk_patientorder", "mzk_concept", "mzk_associateddiagnosisid", "mzk_conceptid", JoinOperator.LeftOuter);
            EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_icdcodeid");

            LinkEntity EntityFrequecy = new LinkEntity("mzk_patientorder", "mzk_ordersetup", "mzk_frequencyid", "mzk_ordersetupid", JoinOperator.LeftOuter);
            EntityFrequecy.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description");

            OrderExpression order = new OrderExpression();
            order.AttributeName = "createdon";
            order.OrderType = OrderType.Descending;

            query.LinkEntities.Add(EntityFrequecy);
            query.LinkEntities.Add(EntityDiagnosis);
            query.Orders.Add(order);

            if (!forFulfillment && pageNumber > 0)
            {
                query.PageInfo = new Microsoft.Xrm.Sdk.Query.PagingInfo();
                query.PageInfo.Count = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                query.PageInfo.PageNumber = pageNumber;
                query.PageInfo.PagingCookie = null;
                query.PageInfo.ReturnTotalRecordCount = true;
            }

            return query;
        }
        public async Task<List<PatientSpecialTest>> getPatientOrder(string patientguid, string patientEncounter, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId ,string caseId=null, int pageNumber = 0)
        {
            List<PatientSpecialTest> patientSpecialTestList = new List<PatientSpecialTest>();

            if (string.IsNullOrEmpty(patientguid) && string.IsNullOrEmpty(caseId) && string.IsNullOrEmpty(patientEncounter) && string.IsNullOrEmpty(orderId))
                throw new ValidationException("Parameter missing");
            
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
         
            EntityCollection entitycollection = entityRepository.GetEntityCollection(this.getSpecialTestQuery(patientguid, patientEncounter, SearchFilters,searchOrder, startDate,endDate,forFulfillment,orderId,caseId, pageNumber));

            foreach (Entity entity in entitycollection.Entities)
            {
                PatientSpecialTest model = new PatientSpecialTest();

                if (!this.getPatientOrder(model, entity, forFulfillment, orderId, mzk_entitytype.SpecialTestOrder))
                {
                    continue;
                }                

                if (entity.Attributes.Contains("mzk_productid"))
                    model.TestName = ((EntityReference)entity["mzk_productid"]).Name;

                if (entity.Attributes.Contains("mzk_associateddiagnosisid"))
                    model.DiagnosisId = ((EntityReference)entity["mzk_associateddiagnosisid"]).Id.ToString();

                if (entity.Attributes.Contains("mzk_concept5.mzk_conceptname"))
                    model.Diagnosis = (entity.Attributes["mzk_concept5.mzk_conceptname"] as AliasedValue).Value.ToString();
                else
                     if (entity.Attributes.Contains("mzk_concept3.mzk_conceptname"))
                    model.Diagnosis = (entity.Attributes["mzk_concept3.mzk_conceptname"] as AliasedValue).Value.ToString();


                if (entity.Attributes.Contains("mzk_concept5.mzk_icdcodeid"))
                    model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_concept5.mzk_icdcodeid"]).Value).Name;
                else
                    if (entity.Attributes.Contains("mzk_concept3.mzk_icdcodeid"))
                    model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_concept3.mzk_icdcodeid"]).Value).Name;

                if (entity.Attributes.Contains("mzk_frequencyid"))
                    model.FrequencyId = ((EntityReference)entity["mzk_frequencyid"]).Id.ToString();

                if (entity.Attributes.Contains("mzk_ordersetup3.mzk_description"))
                    model.Frequency = (entity.Attributes["mzk_ordersetup3.mzk_description"] as AliasedValue).Value.ToString();
                    else
                    if (entity.Attributes.Contains("mzk_ordersetup1.mzk_description"))
                    model.Frequency = (entity.Attributes["mzk_ordersetup1.mzk_description"] as AliasedValue).Value.ToString();

                if (entity.Attributes.Contains("mzk_urgency"))
                {
                    model.UrgencyName = entity.FormattedValues["mzk_urgency"].ToString();
                    model.UrgencyId = ((OptionSetValue)entity["mzk_urgency"]).Value.ToString();
                }

                if (entity.Attributes.Contains("mzk_treatmentlocation"))
                {
                    model.treatmentLocationId = entity.GetAttributeValue<EntityReference>("mzk_treatmentlocation").Id.ToString();
                    model.treatmentLocation = entity.GetAttributeValue<EntityReference>("mzk_treatmentlocation").Name;
                }

                if (entity.Attributes.Contains("ownerid"))
                {
                    model.OrderingPhysician = ((EntityReference)entity.Attributes["ownerid"]).Name;
                    model.OrderingPhysicianId = ((EntityReference)entity.Attributes["ownerid"]).Id.ToString();
                }               

                if (entity.Attributes.Contains("mzk_clinicalnotes"))
                    model.ClinicalNotes = entity.Attributes["mzk_clinicalnotes"].ToString();

                PatientOrderLog log = this.getOrderStatusLogDetails(Convert.ToInt32(model.OrderStatus), model.Id);

                if (log != null)
                {
                    model.StatusNotes = log.Comments;
                }

                patientSpecialTestList.Add(model);
            }

            if (pageNumber > 0 && entitycollection != null)
            {
                Pagination.totalCount = entitycollection.TotalRecordCount;
            }

            return patientSpecialTestList;
        }

        public async Task<List<PatientSpecialTest>> getPatientOrder(List<Entity> patientOrders)
        {
            List<PatientSpecialTest> patientSpecialTestList = new List<PatientSpecialTest>();

            foreach (Entity entity in patientOrders)
            {
                PatientSpecialTest model = new PatientSpecialTest();

                if (entity.Attributes.Contains("mzk_productid"))
                    model.TestName = ((EntityReference)entity["mzk_productid"]).Name;

                if (entity.Attributes.Contains("mzk_clinicalnotes"))
                    model.ClinicalNotes = entity.Attributes["mzk_clinicalnotes"].ToString();

                if (entity.Id != null)
                    model.Id = entity.Id.ToString();

                patientSpecialTestList.Add(model);
            }

            return patientSpecialTestList;
        }

        public async Task<string> addPatientOrder(PatientSpecialTest patientSpecialTest, bool isActivityOrder = false)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            xrm.mzk_patientorder patientOrderEntity = new xrm.mzk_patientorder();
            HMUrgency urgency = HMUrgency.None;
            try
            {
                mzk_casetype caseType = mzk_casetype.OutPatient;

                patientOrderEntity.mzk_appointable = true;

                if (!string.IsNullOrEmpty(patientSpecialTest.appointmentId))
                {
                    patientOrderEntity.mzk_orderingappointment = new EntityReference("mzk_patientorder", new Guid(patientSpecialTest.appointmentId));
                    patientOrderEntity.mzk_fulfillmentappointment = new EntityReference("mzk_patientorder", new Guid(patientSpecialTest.appointmentId));
                }

                if (patientSpecialTest.TestName != string.Empty && patientSpecialTest.TestName != null)
                    patientOrderEntity.Attributes["mzk_productid"] = new EntityReference("product", new Guid(patientSpecialTest.TestName));
                if (patientSpecialTest.Frequency != string.Empty && patientSpecialTest.Frequency != null)
                    patientOrderEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientSpecialTest.Frequency));
                if (patientSpecialTest.AssociatedDiagnosis != string.Empty && patientSpecialTest.AssociatedDiagnosis != null)
                    patientOrderEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_concept", new Guid(patientSpecialTest.AssociatedDiagnosis));
                if (patientSpecialTest.UrgencyId != string.Empty && patientSpecialTest.UrgencyId != null)
                {
                    patientOrderEntity.Attributes["mzk_urgency"] = new OptionSetValue(Convert.ToInt32(patientSpecialTest.UrgencyId));

                    mzk_patientordermzk_Urgency orderUrgency = (mzk_patientordermzk_Urgency)Convert.ToInt32(patientSpecialTest.UrgencyId);

                    if (orderUrgency == mzk_patientordermzk_Urgency.Routine)
                    {
                        urgency = HMUrgency.Routine;
                    }
                    else if (orderUrgency == mzk_patientordermzk_Urgency.Stat)
                    {
                        urgency = HMUrgency.Stat;
                    }
                }

                if (!string.IsNullOrEmpty(patientSpecialTest.treatmentLocationId))
                        patientOrderEntity.Attributes["mzk_treatmentlocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientSpecialTest.treatmentLocationId));

                if (patientSpecialTest.OrderingPhysician != string.Empty && patientSpecialTest.OrderingPhysician != null)
                    patientOrderEntity.Attributes["ownerid"] = new EntityReference("systemuser", new Guid(patientSpecialTest.OrderingPhysician));
                if (patientSpecialTest.EncounterId != string.Empty && patientSpecialTest.EncounterId != null)
                {
                    patientOrderEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(patientSpecialTest.EncounterId));
                    PatientEncounter encounter = new PatientEncounter();
                    encounter.EncounterId = patientSpecialTest.EncounterId;
                    encounter = encounter.encounterDetails(encounter).Result.ToList().First<PatientEncounter>();
                    PatientId = encounter.PatientId;                    
                    caseType = encounter.caseTypeValue;
                    patientOrderEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(PatientId));                   
                }
                //if (!string.IsNullOrEmpty(patientSpecialTest.OrderDate))
                //    patientOrderEntity.Attributes["mzk_orderdate"] =Convert.ToDateTime( patientSpecialTest.OrderDate);

                if (patientSpecialTest.OrderDate != DateTime.MinValue)
                    patientOrderEntity.Attributes["mzk_orderdate"] = patientSpecialTest.OrderDate;
                else
                    patientOrderEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;

                patientOrderEntity.Attributes["mzk_fulfillmentdate"] = patientOrderEntity.Attributes["mzk_orderdate"];

                if (patientSpecialTest.ClinicalNotes != string.Empty && patientSpecialTest.ClinicalNotes != null)
                    patientOrderEntity.Attributes["mzk_clinicalnotes"] = patientSpecialTest.ClinicalNotes;

                patientOrderEntity.Attributes["mzk_type"] = new OptionSetValue(6);
                StatusManager statusManager = StatusManager.getRootStatus(mzk_entitytype.SpecialTestOrder, caseType, isActivityOrder);

                if (statusManager != null)
                {
                    patientOrderEntity.mzk_OrderStatus = new OptionSetValue(statusManager.status);
                    patientOrderEntity.mzk_StatusManagerDetail = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, new Guid(statusManager.StatusId));
                }

                if (patientSpecialTest.clinicRecId > 0)
                    patientOrderEntity.Attributes["mzk_axclinicrefrecid"] = Convert.ToDecimal(patientSpecialTest.clinicRecId);

                if (!string.IsNullOrEmpty(patientSpecialTest.orderingLocationId))
                    patientOrderEntity.Attributes["mzk_orderinglocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientSpecialTest.orderingLocationId));

                bool isDuplicateAllowed = false;
                if (!string.IsNullOrEmpty(patientSpecialTest.EncounterId) && !string.IsNullOrEmpty(patientSpecialTest.TestName))
                    isDuplicateAllowed = new PatientEncounter().DuplicateDetection(patientSpecialTest.EncounterId, patientSpecialTest.TestName);

                if (isDuplicateAllowed == true)
                {
                    Id = Convert.ToString(entityRepository.CreateEntity(patientOrderEntity));
                }
                else
                {
                    throw new ValidationException("Same Special Test cannot be added multiple times.");
                }

                if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                {
                    if (!string.IsNullOrEmpty(patientSpecialTest.EncounterId))
                    {
                        if (patientOrderEntity.Attributes.Contains("mzk_treatmentlocation"))
                        {
                            Clinic clinic = new Clinic().getClinicDetails(patientOrderEntity.GetAttributeValue<EntityReference>("mzk_treatmentlocation").Id.ToString());
                            await this.createCaseTrans(patientSpecialTest.EncounterId, OrderNumber, patientSpecialTest.TestName, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value, 1, "", urgency, "", "", "", 0, clinic.mzk_axclinicrefrecid);
                        }
                        else
                        {
                            await this.createCaseTrans(patientSpecialTest.EncounterId, OrderNumber, patientSpecialTest.TestName, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value, 1, "", urgency, "", "", "", 0, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    entityRepository.DeleteEntity(mzk_patientorder.EntityLogicalName, new Guid(Id));
                }

                throw ex;
            }
            return Id;
        }
        public async Task<bool> updatePatientOrder(PatientSpecialTest patientSpecialTest)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Entity encounterEntity = entityRepository.GetEntity("mzk_patientorder", new Guid(patientSpecialTest.Id) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_frequencyid", "mzk_associateddiagnosisid", "mzk_orderstatus", "mzk_urgency", "mzk_orderdate"));

                if (patientSpecialTest.Frequency != string.Empty && patientSpecialTest.Frequency != null)
                    encounterEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientSpecialTest.Frequency));
                if (patientSpecialTest.AssociatedDiagnosis != string.Empty && patientSpecialTest.AssociatedDiagnosis != null)
                    encounterEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_concept", new Guid(patientSpecialTest.AssociatedDiagnosis));

                if (!string.IsNullOrEmpty(patientSpecialTest.treatmentLocationId))
                    encounterEntity.Attributes["mzk_treatmentlocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientSpecialTest.treatmentLocationId));

                if (patientSpecialTest.OrderStatus != string.Empty && patientSpecialTest.OrderStatus != null)
                    encounterEntity.Attributes["mzk_orderstatus"] = new OptionSetValue(Convert.ToInt32(patientSpecialTest.OrderStatus));
                if (patientSpecialTest.UrgencyId != string.Empty && patientSpecialTest.UrgencyId != null)
                    encounterEntity.Attributes["mzk_urgency"] = new OptionSetValue(Convert.ToInt32(patientSpecialTest.UrgencyId));
                //if (!string.IsNullOrEmpty( patientSpecialTest.OrderDate))
                //    encounterEntity.Attributes["mzk_orderdate"] =Convert.ToDateTime( patientSpecialTest.OrderDate);
                if (patientSpecialTest.OrderDate != DateTime.MinValue)
                    encounterEntity.Attributes["mzk_orderdate"] = patientSpecialTest.OrderDate;

                if (patientSpecialTest.ClinicalNotes != string.Empty && patientSpecialTest.ClinicalNotes != null)
                    encounterEntity.Attributes["mzk_clinicalnotes"] = patientSpecialTest.ClinicalNotes;

                entityRepository.UpdateEntity(encounterEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           

        }
    }
}
