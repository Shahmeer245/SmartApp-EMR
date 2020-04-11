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
    public class PatientCase: IPatientCase
    {

        private EntityRepository<CRMRepository.Microsoft.Dynamics.CRM.Incident> _entityRep;

        //public PatientCase()
        //{
        //    _entityRep = new EntityRepository<Mzk_case>();
        //}

        public Guid     CaseId { get; set; }
        public string CaseNumber { get; set; }
        public string PatientId { get; set; }
        public string ContactId { get; set; }
        public int CaseTypeValue { get; set; }
        public long ClinicRecId { get; set; }
        public string CaseTypeName { get; set; }
        public DateTime CaseDate { get; set; }
        public int CaseStatusValue { get; set; }
        public string CaseStatusName { get; set; }
        public string ClinicId { get; set; }

        public async Task<PatientCase> addPatientCase(string patientid, long patientRecId, int caseType, string clinicRecId)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            PatientCase Case = null;

            if (patientid != null && patientid != string.Empty)
            {
                Entity CaseEntity = new Entity("incident");

                if (!string.IsNullOrEmpty(patientid))
                {
                    CaseEntity.Attributes["customerid"] = new EntityReference("contact", new Guid(patientid));
                }
                
                CaseEntity.Attributes["mzk_casetype"] = new OptionSetValue(caseType);
                //CaseEntity.Attributes["mzk_axclinicrefrecid"] = Convert.ToDecimal(clinicRecId);
                CaseEntity.Attributes["mzk_casedate"] = DateTime.Now.Date;
                CaseId = entityRepository.CreateEntity(CaseEntity);

                Entity entity = entityRepository.GetEntity(xrm.Incident.EntityLogicalName, CaseId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                xrm.Incident EntityCase = (xrm.Incident)entity;

                Case = new PatientCase();

                Case.CaseNumber = EntityCase.TicketNumber;
                Case.CaseId = CaseId;
            }

            return Case;
        }

        public async Task<long> createCase(string patientId, string clinicRecId, int caseType, string caseId, string caseNumber)
        {
            try
            {
                CaseRepository caseRepo = new CaseRepository();
                //string caseNumber = "";

                if (!string.IsNullOrEmpty(caseId))
                {
                    //QueryExpression query = new QueryExpression("incident");
                    //FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                    //childFilter.AddCondition("incidentid", ConditionOperator.Equal, new Guid(caseId));

                    //query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                    //LinkEntity entityTypeDetails = new LinkEntity(xrm.Incident.EntityLogicalName, xrm.Contact.EntityLogicalName, "customerid", "contactid", JoinOperator.Inner);
                    //entityTypeDetails.EntityAlias = "contact";
                    //entityTypeDetails.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_axrefrecid");

                    //query.LinkEntities.Add(entityTypeDetails);

                    //SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    //EntityCollection entitycollection = entityRepository.GetEntityCollection(query);                   

                    //if(entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                    //{
                    //    if (entitycollection.Entities[0].Attributes.Contains("contact.mzk_axrefrecid"))
                    //    {
                    //        patientRecId = Convert.ToInt64((entitycollection.Entities[0]["contact.mzk_axrefrecid"] as AliasedValue).Value);
                    //    }

                    //    if (entitycollection.Entities[0].Attributes.Contains("mzk_casetype"))
                    //    {
                    //        caseType = Convert.ToInt32((entitycollection.Entities[0]["mzk_casetype"] as OptionSetValue).Value);
                    //    }

                    //    if (entitycollection.Entities[0].Attributes.Contains("ticketnumber"))
                    //    {
                    //        caseNumber = (entitycollection.Entities[0]["ticketnumber"]).ToString();
                    //    }
                    //}
                }


                long patientRecId = 0;
                Decimal decID;
                if (!string.IsNullOrEmpty(patientId))
                {
                    decID = Convert.ToDecimal(patientId);
                    patientRecId = Convert.ToInt64(decID);
                }

                long caseRecId = caseRepo.createCase(patientRecId, Convert.ToInt64(clinicRecId), (HMCaseType)caseType, caseId, caseNumber);

                //Helper.Files.SaveToCSV(string.Format("{0}, {1}, {2}, {3}, {4}", caseRecId, patientRecId, Convert.ToInt64(clinicRecId), (HMCaseType)caseType, caseId), "test", DateTime.Now, DateTime.Now);

                return caseRecId;
            }
            catch (Exception ex)
            {
                // Helper.Files.SaveToCSV(ex.Message, "test", DateTime.Now, DateTime.Now);

                throw ex;
            }
        }

        public async Task<List<PatientCase>> getPatientCase(string patientguid)
        {
            List<PatientCase> PatientCase = new List<PatientCase>();
            #region Patient Patient Case Query
            QueryExpression query = new QueryExpression("incident");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            childFilter.AddCondition("customerid", ConditionOperator.Equal, new Guid(patientguid));
            //Patient Order Type :: Case
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            OrderExpression orderby = new OrderExpression();
            orderby.AttributeName = "createdon";
            orderby.OrderType = OrderType.Descending;

            query.Orders.Add(orderby);

            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                PatientCase model = new PatientCase();
                
                if (entity.Attributes.Contains("ticketnumber"))
                    model.CaseNumber = entity["ticketnumber"].ToString();
                if (entity.Attributes.Contains("createdon"))
                    model.CaseDate =Convert.ToDateTime(entity["createdon"]);
                if (entity.Attributes.Contains("mzk_casetype"))
                    model.CaseTypeValue = ((OptionSetValue)entity.Attributes["mzk_casetype"]).Value;
                if (entity.Attributes.Contains("mzk_casetype"))
                    model.CaseTypeName = entity.FormattedValues["mzk_casetype"].ToString();
                if (entity.Attributes.Contains("mzk_casestatus"))
                    model.CaseStatusValue = ((OptionSetValue)entity.Attributes["mzk_casestatus"]).Value;
                if (entity.Attributes.Contains("mzk_casestatus"))
                    model.CaseStatusName = entity.FormattedValues["mzk_casestatus"].ToString();

                model.CaseId = entity.Id;
                PatientCase.Add(model);
            }

            return PatientCase;
        }

        public async Task<PatientCase> getCaseDetails(string caseGuid)
        {
            PatientCase model = new PatientCase();
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                xrm.Incident entity = (xrm.Incident)entityRepository.GetEntity(xrm.Incident.EntityLogicalName, new Guid(caseGuid), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                if (entity != null)
                {
                    if (entity.Attributes.Contains("ticketnumber"))
                        model.CaseNumber = entity["ticketnumber"].ToString();
                    if (entity.Attributes.Contains("createdon"))
                        model.CaseDate = Convert.ToDateTime(entity["createdon"]);
                    if (entity.Attributes.Contains("mzk_casetype"))
                        model.CaseTypeValue = ((OptionSetValue)entity.Attributes["mzk_casetype"]).Value;
                    if (entity.Attributes.Contains("mzk_casetype"))
                        model.CaseTypeName = entity.FormattedValues["mzk_casetype"].ToString();
                    if (entity.Attributes.Contains("mzk_casestatus"))
                        model.CaseStatusValue = ((OptionSetValue)entity.Attributes["mzk_casestatus"]).Value;
                    if (entity.Attributes.Contains("mzk_casestatus"))
                        model.CaseStatusName = entity.FormattedValues["mzk_casestatus"].ToString();
                    if (entity.CustomerId != null)
                    { 
                        model.PatientId = entity.CustomerId.Id.ToString();
                    }
                    if (entity.Attributes.Contains("mzk_organizationalunit"))
                    {
                        model.ClinicId = entity.mzk_OrganizationalUnit.Id.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }

        public async Task<bool> markClinicalDischarge(string caseGuid)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                xrm.Incident entity = (xrm.Incident)entityRepository.GetEntity(xrm.Incident.EntityLogicalName, new Guid(caseGuid), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                if (entity != null)
                {
                    if (entity.Attributes.Contains("mzk_casestatus"))
                    {
                        mzk_casestatus caseStatus  = (mzk_casestatus)((OptionSetValue)entity.Attributes["mzk_casestatus"]).Value;

                        if(caseStatus != mzk_casestatus.Open)
                        {
                            throw new ValidationException("Only Open cases can be Clinically discharged");
                        }

                        entity.mzk_casestatus = new OptionSetValue((int)mzk_casestatus.ClinicallyDischarged);

                        entityRepository.UpdateEntity(entity);

                        this.createStatusLog((int)mzk_casestatus.ClinicallyDischarged, caseGuid);

                        if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                        {
                            CaseRepository caseRepo = new CaseRepository();

                            caseRepo.updateCaseStatus(caseGuid, HMCaseStatus.ClinicalDischarge);
                        }
                    }

                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public async Task<bool> markClosed(string caseGuid)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                xrm.Incident entity = (xrm.Incident)entityRepository.GetEntity(xrm.Incident.EntityLogicalName, new Guid(caseGuid), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                if (entity != null)
                {
                    if (entity.Attributes.Contains("mzk_casestatus"))
                    {
                        mzk_casestatus caseStatus = (mzk_casestatus)((OptionSetValue)entity.Attributes["mzk_casestatus"]).Value;

                        if (caseStatus != mzk_casestatus.Open)
                        {
                            throw new ValidationException("Only Open cases can be Closed");
                        }

                        entity.mzk_casestatus = new OptionSetValue((int)mzk_casestatus.Closed);

                        entityRepository.UpdateEntity(entity);

                        this.createStatusLog((int)mzk_casestatus.Closed, caseGuid);                        
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public async Task<bool> markPhysicalDischarge(string caseGuid)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                xrm.Incident entity = (xrm.Incident)entityRepository.GetEntity(xrm.Incident.EntityLogicalName, new Guid(caseGuid), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                if (entity != null)
                {
                    if (entity.Attributes.Contains("mzk_casestatus"))
                    {
                        mzk_casestatus caseStatus = (mzk_casestatus)((OptionSetValue)entity.Attributes["mzk_casestatus"]).Value;

                        if (caseStatus != mzk_casestatus.FinanciallyDischarged)
                        {
                            throw new ValidationException("Only Financially discharged cases can be Physically discharged");
                        }

                        entity.mzk_casestatus = new OptionSetValue((int)mzk_casestatus.PhysicallyDischarged);

                        entityRepository.UpdateEntity(entity);

                        this.createStatusLog((int)mzk_casestatus.PhysicallyDischarged, caseGuid);

                        if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                        {
                            CaseRepository caseRepo = new CaseRepository();

                            caseRepo.updateCaseStatus(caseGuid, HMCaseStatus.PhysicalDischarge);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }


        public async Task<bool> markFinancialDischarge(string caseGuid)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                xrm.Incident entity = (xrm.Incident)entityRepository.GetEntity(xrm.Incident.EntityLogicalName, new Guid(caseGuid), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                if (entity != null)
                {
                    if (entity.Attributes.Contains("mzk_casestatus"))
                    {
                        mzk_casestatus caseStatus = (mzk_casestatus)((OptionSetValue)entity.Attributes["mzk_casestatus"]).Value;

                        if (caseStatus != mzk_casestatus.ClinicallyDischarged)
                        {
                            throw new ValidationException("Only Clinically discharged cases can be Financially discharged");
                        }

                        entity.mzk_casestatus = new OptionSetValue((int)mzk_casestatus.FinanciallyDischarged);

                        entityRepository.UpdateEntity(entity);

                        this.createStatusLog((int)mzk_casestatus.FinanciallyDischarged, caseGuid);                        
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }


        public bool createStatusLog(int caseStatus, string caseId)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                mzk_caselog entity = new mzk_caselog();

                entity.mzk_CaseStatus = new OptionSetValue(caseStatus);
                entity.mzk_caseid = new EntityReference(xrm.Incident.EntityLogicalName, new Guid(caseId));
                
                entityRepository.CreateEntity(entity);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static mzk_casetype getCaseType(string encounterId, string caseId = "")
        {
            try
            {
                mzk_casetype caseType = mzk_casetype.OutPatient;

                PatientCase model = new PatientCase();
                                
                QueryExpression query = new QueryExpression(xrm.Incident.EntityLogicalName);
                
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_casetype");

                if (string.IsNullOrEmpty(caseId))
                {
                    LinkEntity enc = new LinkEntity(xrm.Incident.EntityLogicalName, mzk_patientencounter.EntityLogicalName, "incidentid", "mzk_caseid", JoinOperator.Inner);
                    enc.LinkCriteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(encounterId));
                    query.LinkEntities.Add(enc);
                }
                else
                {
                    query.Criteria.AddCondition("incidentid", ConditionOperator.Equal, new Guid(caseId));
                }

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                {
                    xrm.Incident entity = (xrm.Incident)entitycollection.Entities[0];
                    caseType = entity.mzk_CaseType != null ? (mzk_casetype)entity.mzk_CaseType.Value : caseType;
                }

                return caseType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
