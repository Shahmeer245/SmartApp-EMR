using Helper;
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
    public class PatientTherapy : PatientOrder
    {   
        public string   Therapy     { get; set; }
        public string   Description { get; set; }
        public string   Frequency   { get; set; }
        public string   FrequencyId { get; set; }
        
        public async Task<List<PatientTherapy>> getPatientOrder(string patientguid, string patientEncounter, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId,string caseId=null)
        {
            if (string.IsNullOrEmpty(patientguid) && string.IsNullOrEmpty(caseId) && string.IsNullOrEmpty(patientEncounter) && string.IsNullOrEmpty(orderId))
                throw new ValidationException("Parameter missing");

            List<PatientTherapy> PatientTherapy = new List<PatientTherapy>();

            #region Patient Therapies Query
            QueryExpression query = new QueryExpression("mzk_patientorder");

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);

            childFilter.AddCondition("mzk_type", ConditionOperator.Equal, Convert.ToInt32(mzk_patientordermzk_Type.Thrapy));

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

            if (!string.IsNullOrEmpty(SearchFilters))
            {

                //if (SearchFilters == Convert.ToString(mzk_labfilter.Ordered))
                //    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Ordered));
                //if (SearchFilters == Convert.ToString(mzk_labfilter.Paid))
                //    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Paid));
                //if (SearchFilters == Convert.ToString(mzk_labfilter.Cancelled))
                //    childFilter.AddCondition("mzk_orderstatus", ConditionOperator.Equal, Convert.ToInt32(mzk_orderstatus.Cancelled));
            }
            //Search Order
            if (!string.IsNullOrEmpty(searchOrder))
                childFilter.AddCondition("mzk_productidname", ConditionOperator.Like, ("%" + searchOrder + "%"));

            //Search Date
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                childFilter.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });



            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientorderid",
                                                                    "mzk_productid",
                                                                    "mzk_patientencounterid",
                                                                    "mzk_patientordernumber",
                                                                    "mzk_orderdate",
                                                                    "mzk_orderstatus",
                                                                    "mzk_comments", "mzk_frequencyid", "createdon");
            
            LinkEntity EntityFrequecy = new LinkEntity("mzk_patientorder", "mzk_ordersetup", "mzk_frequencyid", "mzk_ordersetupid", JoinOperator.LeftOuter);
            EntityFrequecy.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description");
            query.LinkEntities.Add(EntityFrequecy);

            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();            

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                PatientTherapy model = new PatientTherapy();

                if (!this.getPatientOrder(model, entity, forFulfillment, orderId, mzk_entitytype.TherapyOrder))
                {
                    continue;
                }

                if (entity.Attributes.Contains("mzk_productid"))
                    model.Therapy           = ((EntityReference)entity["mzk_productid"]).Name;

                if (entity.Attributes.Contains("mzk_comments"))
                    model.Description       = entity.Attributes["mzk_comments"].ToString();
                
                if (entity.Attributes.Contains("mzk_frequencyid"))
                    model.FrequencyId = ((EntityReference)entity.Attributes["mzk_frequencyid"]).Id.ToString();

                if (entity.Attributes.Contains("mzk_ordersetup3.mzk_description"))
                    model.Frequency = ((AliasedValue)entity.Attributes["mzk_ordersetup3.mzk_description"]).Value.ToString();

                PatientTherapy.Add(model);
            }

            return PatientTherapy;
        }
        public async Task<string> addPatientOrder(PatientTherapy patientTherapy)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            mzk_patientorder patientOrderEntity = new mzk_patientorder();

            try
            {
                patientOrderEntity.mzk_appointable = true;

                if (!string.IsNullOrEmpty(patientTherapy.appointmentId))
                {
                    patientOrderEntity.mzk_orderingappointment = new EntityReference("mzk_patientorder", new Guid(patientTherapy.appointmentId));
                    patientOrderEntity.mzk_fulfillmentappointment = new EntityReference("mzk_patientorder", new Guid(patientTherapy.appointmentId));
                }

                if (!string.IsNullOrEmpty(patientTherapy.Therapy))
                    patientOrderEntity.Attributes["mzk_productid"] = new EntityReference("product", new Guid(patientTherapy.Therapy));
                if (!string.IsNullOrEmpty(patientTherapy.Frequency))
                    patientOrderEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientTherapy.Frequency));
                if (!string.IsNullOrEmpty(patientTherapy.Description))
                    patientOrderEntity.Attributes["mzk_comments"] = patientTherapy.Description;
                if (!string.IsNullOrEmpty(patientTherapy.EncounterId))
                {
                    patientOrderEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(patientTherapy.EncounterId));
                    PatientEncounter encounter = new PatientEncounter();
                    encounter.EncounterId = patientTherapy.EncounterId;
                    PatientId = encounter.getEncounterDetails(encounter).Result.ToList().First<PatientEncounter>().PatientId;
                    patientOrderEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(PatientId));
                }
                
                patientOrderEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;
                patientOrderEntity.Attributes["mzk_fulfillmentdate"] = patientOrderEntity.Attributes["mzk_orderdate"];

                if (patientTherapy.clinicRecId > 0)
                    patientOrderEntity.Attributes["mzk_axclinicrefrecid"] = Convert.ToDecimal(patientTherapy.clinicRecId);

                if (!string.IsNullOrEmpty(patientTherapy.orderingLocationId))
                    patientOrderEntity.Attributes["mzk_orderinglocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientTherapy.orderingLocationId));

                patientOrderEntity.Attributes["mzk_type"] = new OptionSetValue(Convert.ToInt32( mzk_patientordermzk_Type.Thrapy));
                patientOrderEntity.Attributes["mzk_orderstatus"] = new OptionSetValue((int)mzk_orderstatus.Ordered);
                Id = Convert.ToString(entityRepository.CreateEntity(patientOrderEntity));

                if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                {
                    if (!string.IsNullOrEmpty(patientTherapy.EncounterId))
                    {
                        await this.createCaseTrans(patientTherapy.EncounterId, Id, patientTherapy.Therapy, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value);
                    }
                }
                return Id.ToString();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(OrderNumber))
                {
                    entityRepository.DeleteEntity(mzk_patientorder.EntityLogicalName, new Guid(OrderNumber));
                }

                throw ex;
            }

        }
        public async Task<bool> updatePatientOrder(PatientTherapy patientTherapy)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Entity encounterEntity = entityRepository.GetEntity("mzk_patientorder", new Guid(patientTherapy.Id) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientorderid",
                                                            "mzk_productid",
                                                            "mzk_patientordernumber",
                                                            "mzk_category",
                                                            "mzk_orderdate",
                                                            "mzk_orderstatus",
                                                            "mzk_comments", "mzk_frequencyid"));

                if (!string.IsNullOrEmpty(patientTherapy.Therapy))
                    encounterEntity.Attributes["mzk_productid"] = new EntityReference("product", new Guid(patientTherapy.Therapy));
                if (!string.IsNullOrEmpty(patientTherapy.Frequency))
                    encounterEntity.Attributes["mzk_frequencyid"] = new EntityReference("mzk_ordersetup", new Guid(patientTherapy.Frequency));
                if (!string.IsNullOrEmpty(patientTherapy.Description))
                    encounterEntity.Attributes["mzk_comments"] = patientTherapy.Description;
                if (!string.IsNullOrEmpty(patientTherapy.EncounterId))
                    encounterEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(patientTherapy.EncounterId));
                if (patientTherapy.OrderDate != DateTime.MinValue)
                    encounterEntity.Attributes["mzk_orderdate"] = patientTherapy.OrderDate;

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
