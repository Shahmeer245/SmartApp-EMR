using Helper;
using MazikCareService.AXRepository;
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
using xrm;

namespace MazikCareService.Core.Models
{
   public class PatientProcedure : PatientOrder, IPateintProcedure
    {
               
        public string   Title           { get; set; }
        public string   ProcedureId     { get; set; }
        public string   CareProvider    { get; set; }
        public string   CareProviderId  { get; set; }
        public string   Designation     { get; set; }
        public string   Notes           { get; set; }
        public string   CPTCode         { get; set; }

        public Guid PatientID { get; set; }
        public string Externalemrid { get; set; }
        public DateTime RecordedDate { get; set; }

        public async Task<List<PatientProcedure>> getPatientOrder(string patientguid, string patientEncounter, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment,string orderId,string caseId=null,bool isCancel=true, int pageNumber = 0)
        {
            if (string.IsNullOrEmpty(patientguid) && string.IsNullOrEmpty(caseId) && string.IsNullOrEmpty(patientEncounter) && string.IsNullOrEmpty(orderId))
                throw new ValidationException("Parameter missing");

            List<PatientProcedure> PatientProcedure = new List<PatientProcedure>();
            mzk_patientorder patientOrderEntity = new mzk_patientorder();
            #region Patient Procedure Query
            QueryExpression query = new QueryExpression(mzk_patientorder.EntityLogicalName);
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

            if (isCancel == false)
            {
                childFilter.AddCondition("mzk_orderstatus", ConditionOperator.NotEqual, Convert.ToInt32(mzk_orderstatus.Cancelled));
            }

            //Search Filter
            if (!string.IsNullOrEmpty(SearchFilters))
            {
                if (SearchFilters == Convert.ToString(mzk_procedurefilter.Ordered))
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Ordered));
                if (SearchFilters == Convert.ToString(mzk_procedurefilter.Started))
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Started));
                if (SearchFilters == Convert.ToString(mzk_procedurefilter.Canceled))
                    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Cancelled));
            }

            //Search Order
            if (!string.IsNullOrEmpty(searchOrder))
                childFilter.AddCondition("mzk_productidname", ConditionOperator.Like, ("%" + searchOrder + "%"));

            //Search Date
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                childFilter.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });

            //Patient Order Type :: Procedure
            childFilter.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_patientordermzk_Type.Procedure);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientorderid",
                                                                    "mzk_productid",
                                                                    "mzk_patientencounterid",
                                                                    "mzk_patientordernumber",
                                                                    "mzk_orderdate",
                                                                    "mzk_orderstatus",
                                                                    "mzk_comments",
                                                                    "mzk_userid","createdon",
                                                                    "mzk_statusmanagerdetail",
                                                                    "mzk_treatmentlocation", "mzk_orderinglocation");

            LinkEntity EntityProduct = new LinkEntity("mzk_patientorder", "product", "mzk_productid", "productid", JoinOperator.LeftOuter);
            EntityProduct.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_cptcodeid");

            LinkEntity EntityUser = new LinkEntity("mzk_patientorderid", "systemuser", "mzk_userid", "systemuserid", JoinOperator.LeftOuter);
            EntityUser.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("positionid");
            
            query.LinkEntities.Add(EntityProduct);
            query.LinkEntities.Add(EntityUser);

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
                PatientProcedure model = new PatientProcedure();

                if (!this.getPatientOrder(model, entity, forFulfillment, orderId, mzk_entitytype.ProcedureOrder))
                {
                    continue;
                }

                if (entity.Attributes.Contains("mzk_productid"))
                    model.Title             = ((EntityReference)entity.Attributes["mzk_productid"]).Name;

                if (entity.Attributes.Contains("product3.mzk_cptcodeid"))
                    model.CPTCode = ((EntityReference)(entity.Attributes["product3.mzk_cptcodeid"] as AliasedValue).Value).Name;
                else
                if (entity.Attributes.Contains("product1.mzk_cptcodeid"))
                    model.CPTCode = ((EntityReference)(entity.Attributes["product1.mzk_cptcodeid"] as AliasedValue).Value).Name;

                if (entity.Attributes.Contains("mzk_userid"))
                {
                    model.CareProvider = ((EntityReference)entity.Attributes["mzk_userid"]).Name;
                    model.CareProviderId = ((EntityReference)entity.Attributes["mzk_userid"]).Id.ToString();
                }
                if (entity.Attributes.Contains("systemuser4.positionid"))
                    model.Designation = ((EntityReference)(entity.Attributes["systemuser4.positionid"] as AliasedValue).Value).Name;
                else
                    if (entity.Attributes.Contains("systemuser2.positionid"))
                    model.Designation = ((EntityReference)(entity.Attributes["systemuser2.positionid"] as AliasedValue).Value).Name;
                if (entity.Attributes.Contains("mzk_comments"))
                    model.Notes = entity.Attributes["mzk_comments"].ToString();
                                
                if (entity.Attributes.Contains("mzk_treatmentlocation"))
                {
                    model.treatmentLocationId = entity.GetAttributeValue<EntityReference>("mzk_treatmentlocation").Id.ToString();
                    model.treatmentLocation = entity.GetAttributeValue<EntityReference>("mzk_treatmentlocation").Name;
                }

                PatientOrderLog log = this.getOrderStatusLogDetails(Convert.ToInt32(model.OrderStatus), model.Id);

                if (log != null)
                {
                    model.StatusNotes = log.Comments;
                }

                PatientProcedure.Add(model);
            }

            if (pageNumber > 0 && entitycollection != null)
            {
                Pagination.totalCount = entitycollection.TotalRecordCount;
            }

            return PatientProcedure;
        }

        public async Task<List<PatientProcedure>> getPatientOrder(List<Entity> patientOrders)
        {
            List<PatientProcedure> PatientProcedure = new List<PatientProcedure>();

            foreach (Entity entity in patientOrders)
            {
                PatientProcedure model = new PatientProcedure();

                if (entity.Attributes.Contains("mzk_productid"))
                    model.Title = ((EntityReference)entity.Attributes["mzk_productid"]).Name;

                if (entity.Attributes.Contains("mzk_comments"))
                    model.Notes = entity.Attributes["mzk_comments"].ToString();

                if (entity.Id != null)
                    model.Id = entity.Id.ToString();

                PatientProcedure.Add(model);
            }

            return PatientProcedure;
        }

        public async Task<string> addPatientOrder(PatientProcedure patientProcedure, bool isActivityOrder= false)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            mzk_patientorder patientOrderEntity = new mzk_patientorder();

            try
            {                
                if (!string.IsNullOrEmpty(patientProcedure.Notes))
                    patientOrderEntity.mzk_Comments = patientProcedure.Notes;
                if (!string.IsNullOrEmpty(patientProcedure.CareProvider))
                    patientOrderEntity.mzk_UserId = new EntityReference(xrm.SystemUser.EntityLogicalName, new Guid(patientProcedure.CareProvider));

                mzk_casetype caseType = mzk_casetype.OutPatient;

                patientOrderEntity.mzk_appointable = true;

                if (!string.IsNullOrEmpty(patientProcedure.appointmentId))
                {
                    patientOrderEntity.mzk_orderingappointment = new EntityReference("mzk_patientorder", new Guid(patientProcedure.appointmentId));
                    patientOrderEntity.mzk_fulfillmentappointment = new EntityReference("mzk_patientorder", new Guid(patientProcedure.appointmentId));

                    if (!string.IsNullOrEmpty(patientProcedure.PatientId))
                    {
                        patientOrderEntity.mzk_customer = new EntityReference("contact", new Guid(patientProcedure.PatientId));
                    }
                    if (!string.IsNullOrEmpty(patientProcedure.CaseTransRecId))
                    {
                        patientOrderEntity.mzk_AXCaseTransRefRecId = Convert.ToDecimal(patientProcedure.CaseTransRecId);
                    }

                    if (!string.IsNullOrEmpty(patientProcedure.ProcedureId))
                    {
                        patientOrderEntity.mzk_ProductId = new EntityReference(xrm.Product.EntityLogicalName,  Products.getProductId(patientProcedure.ProcedureId));

                        if(patientOrderEntity.mzk_ProductId == null && patientOrderEntity.mzk_ProductId.Id == Guid.Empty)
                        {
                            throw new ValidationException("Product not found for the corresponding item. Please contact system administrator");
                        }
                    }                        
                                        
                    patientOrderEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;
                    patientOrderEntity.mzk_FulfillmentDate = patientProcedure.FulfillmentDate;
                }
                else if(patientProcedure.EncounterId != null && patientProcedure.EncounterId != string.Empty)
                {
                    if (!string.IsNullOrEmpty(patientProcedure.ProcedureId))
                    {
                        patientOrderEntity.mzk_ProductId = new EntityReference(xrm.Product.EntityLogicalName, new Guid(patientProcedure.ProcedureId));
                    }

                    patientOrderEntity.mzk_PatientEncounterId = new EntityReference(xrm.mzk_patientencounter.EntityLogicalName, new Guid(patientProcedure.EncounterId));
                    PatientEncounter encounter = new PatientEncounter();
                    encounter.EncounterId = patientProcedure.EncounterId;
                    encounter = encounter.encounterDetails(encounter).Result.ToList().First<PatientEncounter>();
                    PatientId = encounter.PatientId;                    
                    caseType = encounter.caseTypeValue;
                    patientOrderEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(PatientId));

                    if (patientProcedure.OrderDate != DateTime.MinValue)
                    {
                        patientOrderEntity.mzk_OrderDate = patientProcedure.OrderDate;
                    }
                    else
                    {
                        patientOrderEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;                        
                    }

                    patientOrderEntity.Attributes["mzk_fulfillmentdate"] = patientOrderEntity.Attributes["mzk_orderdate"];
                }

                if (!string.IsNullOrEmpty(patientProcedure.treatmentLocationId))
                {
                    patientOrderEntity.Attributes["mzk_treatmentlocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientProcedure.treatmentLocationId));
                }
                patientOrderEntity.mzk_Type = new OptionSetValue((int)mzk_patientordermzk_Type.Procedure);

                StatusManager statusManager = StatusManager.getRootStatus(mzk_entitytype.ProcedureOrder, caseType, isActivityOrder);

                if (statusManager != null)
                {
                    patientOrderEntity.mzk_OrderStatus = new OptionSetValue(statusManager.status);
                    patientOrderEntity.mzk_StatusManagerDetail = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, new Guid(statusManager.StatusId));
                }

                if (patientProcedure.clinicRecId > 0)
                {
                    patientOrderEntity.Attributes["mzk_axclinicrefrecid"] = Convert.ToDecimal(patientProcedure.clinicRecId);
                }

                if (!string.IsNullOrEmpty(patientProcedure.orderingLocationId))
                {
                    patientOrderEntity.Attributes["mzk_orderinglocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientProcedure.orderingLocationId));
                }

                bool isDuplicateAllowed = false;

                if (!string.IsNullOrEmpty(patientProcedure.EncounterId) && !string.IsNullOrEmpty(patientProcedure.ProcedureId))
                {
                    isDuplicateAllowed = new PatientEncounter().DuplicateDetection(patientProcedure.EncounterId, patientProcedure.ProcedureId);
                }
                else
                {
                    isDuplicateAllowed = true;
                }           

                if (isDuplicateAllowed == true)
                {
                    Id = Convert.ToString(entityRepository.CreateEntity(patientOrderEntity));

                    if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                    {
                        if (!string.IsNullOrEmpty(patientProcedure.EncounterId))
                        {
                            if (patientOrderEntity.Attributes.Contains("mzk_treatmentlocation"))
                            {
                                Clinic clinic = new Clinic().getClinicDetails(patientOrderEntity.GetAttributeValue<EntityReference>("mzk_treatmentlocation").Id.ToString());
                                await this.createCaseTrans(patientProcedure.EncounterId, Id, patientProcedure.ProcedureId, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value, 1, "", AXRepository.AXServices.HMUrgency.None, "", "", "", 0, clinic.mzk_axclinicrefrecid);
                            }
                            else
                            {
                                await this.createCaseTrans(patientProcedure.EncounterId, Id, patientProcedure.ProcedureId, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value, 1, "", AXRepository.AXServices.HMUrgency.None, "", "", "", 0,0);
                            }
                        }
                    }
                }
                else
                {
                    throw new ValidationException("Same procedure cannot be added multiple times.");
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

            return Id.ToString();
        }
        public async Task<bool> updatePatientOrder(PatientProcedure patientProcedure)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Entity encounterEntity = entityRepository.GetEntity("mzk_patientorder", new Guid(patientProcedure.Id) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_comments", "mzk_userid", "mzk_orderdate"));
                //mzk_patientorder patientOrderEntity =  new mzk_patientorder();

                if (!string.IsNullOrEmpty(patientProcedure.Notes))
                    encounterEntity.Attributes["mzk_comments"] = patientProcedure.Notes;
                if (!string.IsNullOrEmpty(patientProcedure.CareProvider))
                    encounterEntity.Attributes["mzk_userid"] = new EntityReference(xrm.SystemUser.EntityLogicalName, new Guid(patientProcedure.CareProvider));
                if (patientProcedure.OrderDate!= DateTime.MinValue)
                    encounterEntity.Attributes["mzk_orderdate"] =patientProcedure.OrderDate;
                if (!string.IsNullOrEmpty(patientProcedure.treatmentLocationId))
                    encounterEntity.Attributes["mzk_treatmentlocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientProcedure.treatmentLocationId));

                entityRepository.UpdateEntity(encounterEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<Guid> CreatePatientProcedureCRM(PatientProcedure condition)
        {
            try
            {

                Entity contact = new Entity("msemr_procedure");

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
                    contact["msemr_description"] = condition.Title;
                }
                if (condition.RecordedDate != null)
                {
                    contact["msemr_datetime"] = Convert.ToDateTime(condition.RecordedDate);
                }

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();


                QueryExpression queryExpression = new QueryExpression("msemr_procedure");
                queryExpression.Criteria.AddCondition("mzk_externalemrid", ConditionOperator.Equal, condition.Externalemrid);

                queryExpression.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryExpression);

                if (entitycollection != null)
                {
                    if (entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities[0].Attributes.Contains("msemr_procedureid"))
                        {
                            contact["msemr_procedureid"] = new Guid(entitycollection.Entities[0].Attributes["msemr_procedureid"].ToString());
                        }
                        entityRepository.UpdateEntity(contact);
                        contact.Id = new Guid(contact["msemr_procedureid"].ToString());
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

        public async Task<List<PatientProcedure>> getPatientProcedureCRM(string patientGuid, DateTime startdate, DateTime enddate)
        {
            try
            {
                List<PatientProcedure> list = new List<PatientProcedure>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression("msemr_procedure");

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
                        PatientProcedure obj = new PatientProcedure();
                        obj = getPatientProcedureModelFilled(entitycollection[i], obj, "");
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

        public static PatientProcedure getPatientProcedureModelFilled(Entity entity, PatientProcedure obs, string accountAlias)
        {
            if (entity.Attributes.Contains("msemr_description"))
            {
                obs.Title = (entity.Attributes["msemr_description"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_procedureid"))
            {
                obs.ProcedureId = (entity.Attributes["msemr_procedureid"]).ToString();
            }

            if (entity.Attributes.Contains("msemr_patient"))
            {
                obs.PatientID = new Guid((entity.Attributes["msemr_patient"] as EntityReference).Id.ToString());
            }

            if (entity.Attributes.Contains("msemr_datetime"))
            {
                obs.RecordedDate = Convert.ToDateTime(entity.Attributes["msemr_datetime"]);
            }

            return obs;
        }

    }
}