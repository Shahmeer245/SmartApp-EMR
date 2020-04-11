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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using xrm;

namespace MazikCareService.Core.Models
{
    public class PatientLabOrder : PatientOrder, IPatientLabOrder
    {
        public string TestName { get; set; }
        public string AssociatedDiagnosis { get; set; }
        public string AssociatedDiagnosisId { get; set; }
        public string Frequency { get; set; }
        public string FrequencyId { get; set; }
        public string ClinicalNotes { get; set; }
        public string Antibiotics { get; set; }
        public string AntibioticsComments { get; set; }
        public bool antiBioticRequired { get; set; }
        public bool commentsRequired { get; set; }
        public string ResultStatus { get; set; }
        public string ResultStatusText { get; set; }
        public string UrgencyId { get; set; }
        public string ReportPath { get; set; }
        public string UrgencyName { get; set; }
        public string LISLink { get; set; }
        public string Instructionstopatients { get; set; }
        public List<string> specimensourcelist { get; set; }

        public string SpecimenSourceId { get; set; }
        public string SpecimenSourceName { get; set; }
        public string SampleLocation { get; set; }
        public string LocationCode { get; set; }

        public async Task<List<PatientLabOrder>> getPatientOrder(string patientguid, string patientEncounter, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId, string caseId = null, int pageNumber = 0)
        {
            List<PatientLabOrder> PatientLabOrder = new List<PatientLabOrder>();
            #region Patient Lab Order Query
            QueryExpression query = new QueryExpression("mzk_patientorder");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);

            if (SearchFilters != mzk_orderstatus.Cancelled.ToString())
            {
                childFilter.AddCondition("mzk_orderstatus", ConditionOperator.NotEqual, (int)mzk_orderstatus.Cancelled);
            }
            if (!string.IsNullOrEmpty(caseId))
            {
                childFilter.AddCondition("mzk_caseid", ConditionOperator.Equal, new Guid(caseId));
            }
            if (!string.IsNullOrEmpty(orderId))
            {
                childFilter.AddCondition("mzk_patientorderid", ConditionOperator.Equal, new Guid(orderId));
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
            //Patient Order Type :: Laboratory
            childFilter.AddCondition("mzk_type", ConditionOperator.Equal, "2");
            //Search Filter
            if (!string.IsNullOrEmpty(SearchFilters))
            {
                if (SearchFilters == Convert.ToString(mzk_labfilter.Ordered))
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Ordered));
                if (SearchFilters == Convert.ToString(mzk_labfilter.Paid))
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Paid));
                if (SearchFilters == Convert.ToString(mzk_labfilter.Cancelled))
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
                                                                    "mzk_patientencounterid",
                                                                    "mzk_frequencyid",
                                                                    "mzk_orderdate",
                                                                    "mzk_customer",
                                                                    "createdby",
                                                                    "mzk_clinicalnotes",
                                                                    "mzk_resultstatus",
                                                                    "mzk_reportpath",
                                                                    "mzk_reporturl",
                                                                    "mzk_antibiotics",
                                                                    "mzk_orderstatus",
                                                                    "mzk_axclinicrefrecid",
                                                                    "mzk_statusmanagerdetail",
                                                                    "mzk_antibioticscomments",
                                                                    "createdon",
                                                                    "mzk_fulfillmentdate",
                                                                    "mzk_urgency",
                                                                    "mzk_lislink",
                                                                    "mzk_instructionstopatients",
                                                                    "mzk_specimensource",
                                                                    "mzk_treatmentlocation", "mzk_orderinglocation");

            if (!string.IsNullOrEmpty(orderId))
            {
                LinkEntity Resource = new LinkEntity("mzk_patientorder", "systemuser", "createdby", "systemuserid", JoinOperator.Inner);
                Resource.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname", "systemuserid");

                query.LinkEntities.Add(Resource);
            }

            LinkEntity EntityDiagnosis = new LinkEntity("mzk_patientorder", "mzk_concept", "mzk_associateddiagnosisid", "mzk_conceptid", JoinOperator.LeftOuter);
            EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_icdcodeid");

            LinkEntity EntityFrequecy = new LinkEntity("mzk_patientorder", "mzk_ordersetup", "mzk_frequencyid", "mzk_ordersetupid", JoinOperator.LeftOuter);
            EntityFrequecy.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description");

            LinkEntity EntitySpecimenSource = new LinkEntity("mzk_patientorder", "mzk_ordersetup", "mzk_specimensource", "mzk_ordersetupid", JoinOperator.LeftOuter);
            EntitySpecimenSource.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description");
            EntitySpecimenSource.EntityAlias = "SpecimenSource";

            OrderExpression orderby = new OrderExpression();
            orderby.AttributeName = "createdon";
            orderby.OrderType = OrderType.Descending;

            LinkEntity ProductRecord = new LinkEntity("mzk_patientorder", "product", "mzk_productid", "productid", JoinOperator.LeftOuter);
            ProductRecord.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("productnumber");
            ProductRecord.EntityAlias = "ProductRecord";

            LinkEntity EntityFamily = new LinkEntity("product", "product", "parentproductid", "productid", JoinOperator.LeftOuter);
            EntityFamily.EntityAlias = "ProductFamily";
            EntityFamily.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_antibioticmandatory", "mzk_commentsmandatory", "mzk_controlleddrug");
            ProductRecord.LinkEntities.Add(EntityFamily);

            query.LinkEntities.Add(EntityFrequecy);
            query.LinkEntities.Add(EntityDiagnosis);
            query.LinkEntities.Add(ProductRecord);
            query.LinkEntities.Add(EntitySpecimenSource);

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
            if (string.IsNullOrEmpty(patientguid) && string.IsNullOrEmpty(caseId) && string.IsNullOrEmpty(patientEncounter) && string.IsNullOrEmpty(orderId))
                throw new ValidationException("Parameter missing");

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            User orderingProvider;

            foreach (Entity entity in entitycollection.Entities)
            {
                PatientLabOrder model = new PatientLabOrder();

                if (!this.getPatientOrder(model, entity, forFulfillment, orderId, mzk_entitytype.LabOrder))
                {
                    continue;
                }                

                if (entity.Attributes.Contains("mzk_productid"))
                    model.TestName = ((EntityReference)entity["mzk_productid"]).Name;
                if (entity.Attributes.Contains("ProductRecord.productnumber"))
                    model.TestId = (entity.Attributes["ProductRecord.productnumber"] as AliasedValue).Value.ToString();
                if (entity.Attributes.Contains("mzk_frequencyid"))
                    model.FrequencyId = ((EntityReference)entity["mzk_frequencyid"]).Id.ToString();

                if (entity.Attributes.Contains("mzk_customer"))
                    model.PatientId = ((EntityReference)entity["mzk_customer"]).Id.ToString();

                if (entity.Attributes.Contains("mzk_ordersetup3.mzk_description"))
                    model.Frequency = (entity.Attributes["mzk_ordersetup3.mzk_description"] as AliasedValue).Value.ToString();
                else
                    if (entity.Attributes.Contains("mzk_ordersetup1.mzk_description"))
                    model.Frequency = (entity.Attributes["mzk_ordersetup1.mzk_description"] as AliasedValue).Value.ToString();

                if (entity.Attributes.Contains("createdon"))
                    model.CreatedOn = (DateTime)entity["createdon"];

                if (entity.Attributes.Contains("mzk_fulfillmentdate"))
                    model.FulfillmentDate = (DateTime)entity["mzk_fulfillmentdate"];

                if (entity.Attributes.Contains("mzk_associateddiagnosisid"))
                    model.AssociatedDiagnosisId = ((EntityReference)entity["mzk_associateddiagnosisid"]).Id.ToString();

                if (entity.Attributes.Contains("mzk_concept5.mzk_conceptname"))
                    model.AssociatedDiagnosis = (entity.Attributes["mzk_concept5.mzk_conceptname"] as AliasedValue).Value.ToString();
                else
                    if (entity.Attributes.Contains("mzk_concept3.mzk_conceptname"))
                    model.AssociatedDiagnosis = (entity.Attributes["mzk_concept3.mzk_conceptname"] as AliasedValue).Value.ToString();

                if (entity.Attributes.Contains("mzk_concept5.mzk_icdcodeid"))
                    model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_concept5.mzk_icdcodeid"]).Value).Name;
                else
                    if (entity.Attributes.Contains("mzk_concept3.mzk_icdcodeid"))
                    model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_concept3.mzk_icdcodeid"]).Value).Name;

                if (entity.Attributes.Contains("mzk_clinicalnotes"))
                    model.ClinicalNotes = entity["mzk_clinicalnotes"].ToString();

                if (entity.Attributes.Contains("mzk_antibiotics") && entity.Attributes["mzk_antibiotics"].ToString() == "True")
                    model.Antibiotics = "1";
                else
                    model.Antibiotics = "0";

                if (entity.Attributes.Contains("mzk_antibioticscomments"))
                    model.AntibioticsComments = entity["mzk_antibioticscomments"].ToString();

                if (entity.Attributes.Contains("mzk_lislink"))
                    model.LISLink = entity["mzk_lislink"].ToString();

                if (entity.Attributes.Contains("mzk_urgency"))
                    model.UrgencyName = entity.FormattedValues["mzk_urgency"].ToString();
                //else
                //    if (entity.Attributes.Contains("mzk_ordersetup2.mzk_description"))
                //    model.UrgencyName = (entity.Attributes["mzk_ordersetup2.mzk_description"] as AliasedValue).Value.ToString();

                if (entity.Attributes.Contains("mzk_urgency"))
                    model.UrgencyId = ((OptionSetValue)entity.Attributes["mzk_urgency"]).Value.ToString();

                if (entity.Attributes.Contains("ProductFamily.mzk_antibioticmandatory"))
                    model.antiBioticRequired = (bool)((AliasedValue)entity.Attributes["ProductFamily.mzk_antibioticmandatory"]).Value;

                if (entity.Attributes.Contains("ProductFamily.mzk_commentsmandatory"))
                    model.commentsRequired = (bool)((AliasedValue)entity.Attributes["ProductFamily.mzk_commentsmandatory"]).Value;


                if (entity.Attributes.Contains("mzk_specimensource"))
                    model.SpecimenSourceId = ((EntityReference)entity["mzk_specimensource"]).Id.ToString();

                if (entity.Attributes.Contains("SpecimenSource.mzk_description"))
                    model.SpecimenSourceName = (entity.Attributes["SpecimenSource.mzk_description"] as AliasedValue).Value.ToString();


                if (entity.Attributes.Contains("mzk_instructionstopatients"))
                    model.Instructionstopatients = entity.Attributes["mzk_instructionstopatients"].ToString();

                if (entity.Attributes.Contains("mzk_reporturl"))
                    model.LISLink = entity["mzk_reporturl"].ToString();

                if (entity.Attributes.Contains("mzk_resultstatus"))
                {
                    model.ResultStatus = (entity["mzk_resultstatus"] as OptionSetValue).Value.ToString();
                    model.ResultStatusText = entity.FormattedValues["mzk_resultstatus"].ToString();
                }

                PatientOrderLog log = this.getOrderStatusLogDetails(Convert.ToInt32(model.OrderStatus), model.Id);

                if (log != null)
                {
                    model.SampleLocation = log.Location;
                }

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
                        mzk_casetype caseType = PatientCase.getCaseType(model.EncounterId);

                        model.UrgencyId = CaseParameter.getDefaultUrgency(caseType).urgencyId;
                    }

                    if (entity.Attributes.Contains("mzk_orderinglocation"))
                    {
                        model.orderingLocationId = entity.GetAttributeValue<EntityReference>("mzk_orderinglocation").Id.ToString();
                        model.orderingLocation = entity.GetAttributeValue<EntityReference>("mzk_orderinglocation").Name.ToString();
                    }
                }

                PatientLabOrder.Add(model);
            }

            if (pageNumber > 0 && entitycollection != null)
            {
                Pagination.totalCount = entitycollection.TotalRecordCount;
            }

            return PatientLabOrder;
        }

        public async Task<List<PatientLabOrder>> getPatientOrder(List<Entity> patientOrders)
        {
            List<PatientLabOrder> PatientLabOrder = new List<PatientLabOrder>();

            foreach (Entity entity in patientOrders)
            {
                PatientLabOrder model = new PatientLabOrder();

                if (entity.Attributes.Contains("mzk_productid"))
                    model.TestName = ((EntityReference)entity["mzk_productid"]).Name;

                if (entity.Id != null)
                    model.Id = entity.Id.ToString();

                if (entity.Attributes.Contains("mzk_clinicalnotes"))
                    model.ClinicalNotes = entity["mzk_clinicalnotes"].ToString();

                PatientLabOrder.Add(model);
            }

            return PatientLabOrder;
        }

        public async Task<string> addPatientOrder(PatientLabOrder patientLabOrder, bool isActivityOrder = false)
        {
            if (patientLabOrder.specimensourcelist != null && patientLabOrder.specimensourcelist.Count > 0)
            {
                for (int specimentCount = 0; specimentCount < patientLabOrder.specimensourcelist.Count; specimentCount++)
                {
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    xrm.mzk_patientorder patientOrderEntity = new xrm.mzk_patientorder();
                    try
                    {
                        mzk_casetype caseType = mzk_casetype.OutPatient;

                        patientOrderEntity.mzk_appointable = true;

                        if (!string.IsNullOrEmpty(patientLabOrder.appointmentId))
                        {
                            patientOrderEntity.mzk_orderingappointment = new EntityReference("mzk_patientorder", new Guid(patientLabOrder.appointmentId));
                            patientOrderEntity.mzk_fulfillmentappointment = new EntityReference("mzk_patientorder", new Guid(patientLabOrder.appointmentId));
                        }

                        if (!string.IsNullOrEmpty(patientLabOrder.TestName))
                            patientOrderEntity.Attributes["mzk_productid"] = new EntityReference("product", new Guid(patientLabOrder.TestName));
                        else
                            throw new ValidationException("Test Name must be selected");

                        if (!string.IsNullOrEmpty(patientLabOrder.Frequency))
                            patientOrderEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientLabOrder.Frequency));
                        if (!string.IsNullOrEmpty(patientLabOrder.AssociatedDiagnosis))
                            patientOrderEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_concept", new Guid(patientLabOrder.AssociatedDiagnosis));
                        if (!string.IsNullOrEmpty(patientLabOrder.ClinicalNotes))
                            patientOrderEntity.Attributes["mzk_clinicalnotes"] = patientLabOrder.ClinicalNotes;
                        if (!string.IsNullOrEmpty(patientLabOrder.AntibioticsComments))
                            patientOrderEntity.Attributes["mzk_antibioticscomments"] = patientLabOrder.AntibioticsComments;
                        if (!string.IsNullOrEmpty(patientLabOrder.EncounterId))
                        {
                            patientOrderEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(patientLabOrder.EncounterId));
                            PatientEncounter encounter = new PatientEncounter();
                            encounter.EncounterId = patientLabOrder.EncounterId;
                            encounter = encounter.encounterDetails(encounter).Result.ToList().First<PatientEncounter>();
                            PatientId = encounter.PatientId;
                            CaseId = encounter.CaseId.ToString();
                            caseType = encounter.caseTypeValue;
                            patientOrderEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(PatientId));
                        }


                        if (patientLabOrder.OrderDate != DateTime.MinValue)
                            patientOrderEntity.Attributes["mzk_orderdate"] = patientLabOrder.OrderDate;
                        else
                            patientOrderEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;

                        patientOrderEntity.Attributes["mzk_fulfillmentdate"] = patientOrderEntity.Attributes["mzk_orderdate"];

                        if (patientLabOrder.UrgencyId != string.Empty && patientLabOrder.UrgencyId != null)
                            patientOrderEntity.Attributes["mzk_urgency"] = new OptionSetValue(Convert.ToInt32(patientLabOrder.UrgencyId));

                        if (!string.IsNullOrEmpty(patientLabOrder.Antibiotics) && patientLabOrder.Antibiotics == "1")
                            patientOrderEntity.Attributes["mzk_antibiotics"] = true;
                        else
                            patientOrderEntity.Attributes["mzk_antibiotics"] = false;
                        
                        if (!string.IsNullOrEmpty(patientLabOrder.orderingLocationId))
                        {
                            patientOrderEntity.Attributes["mzk_orderinglocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientLabOrder.orderingLocationId));                            
                            patientOrderEntity.Attributes["mzk_specialityname"] = new Clinic().getClinicDetails(patientLabOrder.orderingLocationId).speciality;
                        }

                        if (!string.IsNullOrEmpty(patientLabOrder.ParentOrderId))
                            patientOrderEntity.Attributes["mzk_parentorder"] = new EntityReference("mzk_patientorder", new Guid(patientLabOrder.ParentOrderId));

                        if (!string.IsNullOrEmpty(patientLabOrder.Instructionstopatients))
                            patientOrderEntity.Attributes["mzk_instructionstopatients"] = patientLabOrder.Instructionstopatients;

                        patientOrderEntity.Attributes["mzk_type"] = new OptionSetValue(2);
                        StatusManager statusManager = StatusManager.getRootStatus(mzk_entitytype.LabOrder, caseType, isActivityOrder);

                        if (statusManager != null)
                        {
                            patientOrderEntity.mzk_OrderStatus = new OptionSetValue(statusManager.status);
                            patientOrderEntity.mzk_StatusManagerDetail = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, new Guid(statusManager.StatusId));
                        }

                        if (patientLabOrder.specimensourcelist[specimentCount] != null)
                            patientOrderEntity.Attributes["mzk_specimensource"] = new EntityReference("mzk_ordersetup", new Guid(patientLabOrder.specimensourcelist[specimentCount]));

                        bool isDuplicateAllowed = false;

                        Products listProduct = new Products().getProductDetails(patientLabOrder.TestName);
                        if (listProduct != null)
                        {
                            if (listProduct.IsSpecimenSource == false)
                            {
                                if (!string.IsNullOrEmpty(patientLabOrder.EncounterId) && !string.IsNullOrEmpty(patientLabOrder.TestName))
                                    isDuplicateAllowed = new PatientEncounter().DuplicateDetection(patientLabOrder.EncounterId, patientLabOrder.TestName);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(patientLabOrder.EncounterId) && !string.IsNullOrEmpty(patientLabOrder.TestName) && !string.IsNullOrEmpty(patientLabOrder.specimensourcelist[specimentCount]))
                                    isDuplicateAllowed = new PatientEncounter().DuplicateDetection(patientLabOrder.EncounterId, patientLabOrder.TestName, patientLabOrder.specimensourcelist[specimentCount]);
                            }

                            if (isDuplicateAllowed == true)
                            {
                                Id = Convert.ToString(entityRepository.CreateEntity(patientOrderEntity));
                            }
                            else
                            {
                                throw new ValidationException("Same Lab Test cannot be added multiple times.");
                            }
                        }

                        if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                        {
                            if (!string.IsNullOrEmpty(patientLabOrder.EncounterId))
                            {
                                await this.createCaseTrans(patientLabOrder.EncounterId, Id, patientLabOrder.TestName, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value);
                            }
                        }

                        if (AppSettings.GetByKey("LISIntegration").ToLower() == true.ToString().ToLower() && statusManager.sendOrm)
                        {
                            this.sendORMLISMessage(PatientId, 0, patientLabOrder.EncounterId, Id, CaseId, statusManager.status.ToString());
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
                }
            }
            else
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                xrm.mzk_patientorder patientOrderEntity = new xrm.mzk_patientorder();
                try
                {
                    mzk_casetype caseType = mzk_casetype.OutPatient;

                    patientOrderEntity.mzk_appointable = true;

                    if (!string.IsNullOrEmpty(patientLabOrder.appointmentId))
                    {
                        patientOrderEntity.mzk_orderingappointment = new EntityReference("mzk_patientorder", new Guid(patientLabOrder.appointmentId));
                        patientOrderEntity.mzk_fulfillmentappointment = new EntityReference("mzk_patientorder", new Guid(patientLabOrder.appointmentId));
                    }

                    if (!string.IsNullOrEmpty(patientLabOrder.TestName))
                        patientOrderEntity.Attributes["mzk_productid"] = new EntityReference("product", new Guid(patientLabOrder.TestName));
                    else
                        throw new ValidationException("Test Name must be selected");

                    if (!string.IsNullOrEmpty(patientLabOrder.Frequency))
                        patientOrderEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientLabOrder.Frequency));
                    if (!string.IsNullOrEmpty(patientLabOrder.AssociatedDiagnosis))
                        patientOrderEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_concept", new Guid(patientLabOrder.AssociatedDiagnosis));
                    if (!string.IsNullOrEmpty(patientLabOrder.ClinicalNotes))
                        patientOrderEntity.Attributes["mzk_clinicalnotes"] = patientLabOrder.ClinicalNotes;
                    if (!string.IsNullOrEmpty(patientLabOrder.AntibioticsComments))
                        patientOrderEntity.Attributes["mzk_antibioticscomments"] = patientLabOrder.AntibioticsComments;
                    if (!string.IsNullOrEmpty(patientLabOrder.EncounterId))
                    {
                        patientOrderEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(patientLabOrder.EncounterId));
                        PatientEncounter encounter = new PatientEncounter();
                        encounter.EncounterId = patientLabOrder.EncounterId;
                        encounter = encounter.encounterDetails(encounter).Result.ToList().First<PatientEncounter>();
                        PatientId = encounter.PatientId;
                        CaseId = encounter.CaseId.ToString();
                        caseType = encounter.caseTypeValue;
                        patientOrderEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(PatientId));
                    }

                    if (patientLabOrder.OrderDate != DateTime.MinValue)
                        patientOrderEntity.Attributes["mzk_orderdate"] = patientLabOrder.OrderDate;
                    else
                        patientOrderEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;

                    patientOrderEntity.Attributes["mzk_fulfillmentdate"] = patientOrderEntity.Attributes["mzk_orderdate"];

                    if (patientLabOrder.UrgencyId != string.Empty && patientLabOrder.UrgencyId != null)
                        patientOrderEntity.Attributes["mzk_urgency"] = new OptionSetValue(Convert.ToInt32(patientLabOrder.UrgencyId));

                    if (!string.IsNullOrEmpty(patientLabOrder.Antibiotics) && patientLabOrder.Antibiotics == "1")
                        patientOrderEntity.Attributes["mzk_antibiotics"] = true;
                    else
                        patientOrderEntity.Attributes["mzk_antibiotics"] = false;                    

                    if (!string.IsNullOrEmpty(patientLabOrder.orderingLocationId))
                    {
                        patientOrderEntity.Attributes["mzk_orderinglocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientLabOrder.orderingLocationId));

                        patientOrderEntity.Attributes["mzk_specialityname"] = new Clinic().getClinicDetails(patientLabOrder.orderingLocationId).speciality;
                    }

                    if (!string.IsNullOrEmpty(patientLabOrder.ParentOrderId))
                        patientOrderEntity.Attributes["mzk_parentorder"] = new EntityReference("mzk_patientorder", new Guid(patientLabOrder.ParentOrderId));

                    if (!string.IsNullOrEmpty(patientLabOrder.Instructionstopatients))
                        patientOrderEntity.Attributes["mzk_instructionstopatients"] = patientLabOrder.Instructionstopatients;

                    patientOrderEntity.Attributes["mzk_type"] = new OptionSetValue(2);
                    StatusManager statusManager = StatusManager.getRootStatus(mzk_entitytype.LabOrder, caseType, isActivityOrder);

                    if (statusManager != null)
                    {
                        patientOrderEntity.mzk_OrderStatus = new OptionSetValue(statusManager.status);
                        patientOrderEntity.mzk_StatusManagerDetail = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, new Guid(statusManager.StatusId));
                    }

                    bool isDuplicateAllowed = false;
                    if (!string.IsNullOrEmpty(patientLabOrder.EncounterId) && !string.IsNullOrEmpty(patientLabOrder.TestName))
                        isDuplicateAllowed = new PatientEncounter().DuplicateDetection(patientLabOrder.EncounterId, patientLabOrder.TestName);

                    if (isDuplicateAllowed == true)
                    {
                        Id = Convert.ToString(entityRepository.CreateEntity(patientOrderEntity));
                    }
                    else
                    {
                        throw new ValidationException("Same Lab Test cannot be added multiple times.");
                    }

                    if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                    {
                        if (!string.IsNullOrEmpty(patientLabOrder.EncounterId))
                        {
                            await this.createCaseTrans(patientLabOrder.EncounterId, Id, patientLabOrder.TestName, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value);
                        }
                    }

                    if (AppSettings.GetByKey("LISIntegration").ToLower() == true.ToString().ToLower() && statusManager.sendOrm)
                    {
                        this.sendORMLISMessage(PatientId, 0, patientLabOrder.EncounterId, Id, CaseId, statusManager.status.ToString());
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
            }
            return Id.ToString();
        }

        public async Task<bool> updatePatientOrder(PatientLabOrder patientLabOrder)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Entity encounterEntity = entityRepository.GetEntity("mzk_patientorder", new Guid(patientLabOrder.Id) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_antibiotics", "mzk_clinicalnotes", "mzk_antibioticscomments", "mzk_frequencyid", "mzk_orderstatus", "mzk_instructionstopatients"));

                if (patientLabOrder.Antibiotics != string.Empty && patientLabOrder.Antibiotics == "1")
                    encounterEntity.Attributes["mzk_antibiotics"] = true;
                else
                    encounterEntity.Attributes["mzk_antibiotics"] = false;


                if (patientLabOrder.Instructionstopatients != string.Empty && patientLabOrder.Instructionstopatients != null)
                    encounterEntity.Attributes["mzk_instructionstopatients"] = patientLabOrder.Instructionstopatients;

                if (patientLabOrder.ClinicalNotes != string.Empty && patientLabOrder.ClinicalNotes != null)
                    encounterEntity.Attributes["mzk_clinicalnotes"] = patientLabOrder.ClinicalNotes;
                if (patientLabOrder.AntibioticsComments != null)
                    encounterEntity.Attributes["mzk_antibioticscomments"] = patientLabOrder.AntibioticsComments;
                if (patientLabOrder.OrderStatus != string.Empty && patientLabOrder.OrderStatus != null)
                    encounterEntity.Attributes["mzk_orderstatus"] = new OptionSetValue(Convert.ToInt32(patientLabOrder.OrderStatus));
                if (patientLabOrder.Frequency != string.Empty && patientLabOrder.Frequency != null)
                    encounterEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientLabOrder.Frequency));
                if (patientLabOrder.AssociatedDiagnosis != string.Empty && patientLabOrder.AssociatedDiagnosis != null)
                    encounterEntity.Attributes["mzk_associateddiagnosisid"] = new EntityReference("mzk_concept", new Guid(patientLabOrder.AssociatedDiagnosis));
                if (patientLabOrder.OrderDate != DateTime.MinValue)
                    encounterEntity.Attributes["mzk_orderdate"] = patientLabOrder.OrderDate;

                if (patientLabOrder.UrgencyId != string.Empty && patientLabOrder.UrgencyId != null)
                    encounterEntity.Attributes["mzk_urgency"] = new OptionSetValue(Convert.ToInt32(patientLabOrder.UrgencyId));

                entityRepository.UpdateEntity(encounterEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> updateResult(PatientLabOrder patientLabOrder)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                mzk_patientorder patientOrderEntity = (mzk_patientorder)entityRepository.GetEntity("mzk_patientorder", new Guid(patientLabOrder.Id) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_orderstatus", "mzk_axcasetransrefrecid"));


                if (patientLabOrder.ResultStatus != string.Empty && patientLabOrder.ResultStatus != null)
                {
                    mzk_resultstatus resultStatus = mzk_resultstatus.Final;

                    switch (patientLabOrder.ResultStatus)
                    {
                        case "P":
                            resultStatus = mzk_resultstatus.Preliminary;
                            break;
                        case "C":
                            resultStatus = mzk_resultstatus.Corrected;
                            break;
                        case "f":
                            resultStatus = mzk_resultstatus.Final;
                            break;
                    }

                    patientOrderEntity.mzk_ResultStatus = new OptionSetValue((int)resultStatus);
                }

                if (patientLabOrder.LISLink != string.Empty && patientLabOrder.LISLink != null)
                    patientOrderEntity.mzk_ReportURL = HttpUtility.UrlEncode(patientLabOrder.LISLink);

                if (patientLabOrder.ReportPath != string.Empty && patientLabOrder.ReportPath != null)
                    patientOrderEntity.mzk_ReportPath = patientLabOrder.ReportPath;

                if (patientOrderEntity.mzk_AXCaseTransRefRecId.HasValue)
                {
                    CaseRepository caseRepo = new CaseRepository();

                    caseRepo.updateReportUrl((long)Convert.ToDecimal(patientOrderEntity.mzk_AXCaseTransRefRecId.Value), patientLabOrder.LISLink, patientLabOrder.ReportPath);
                }

                entityRepository.UpdateEntity(patientOrderEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
