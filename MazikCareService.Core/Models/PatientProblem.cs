using Helper;
using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
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
    public class PatientProblem : IPatientProblem
    {
        public string PatientId { get; set; }
        //public string Id { get; set; }
        
        public string problemName{ get; set; }
        public string problemId { get; set; }
        public int status { get; set; }
        public string StatusText { get; set; }
        public string onSetNotes { get; set; }
        public DateTime onSetDate { get; set; }
        public Guid Id { get; set; }
        public int ProblemType { get; set; }
        public string ProblemTypeText { get; set; }
        public int Chronicity { get; set; }
        public string ChronicityText { get; set; }
        public DateTime CreatedOn { get; set; }
        public string IcdCode { get; set; }
                
        public async Task<List<PatientProblem>> getPatientProblems(string patientguid, bool OnlyActive, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate,bool isChronic =false, int pageNumber = 0)
        {
            List<PatientProblem> PatientProblem = new List<PatientProblem>();
            #region Patient Problem Query
            QueryExpression query = new QueryExpression("mzk_patientproblem");
            
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            if (!string.IsNullOrEmpty(patientguid))
            {
                childFilter.AddCondition("mzk_customerid", ConditionOperator.Equal, new Guid(patientguid));
            }

            if (isChronic)
            {
                query.Criteria.AddCondition("mzk_chronicity", ConditionOperator.Equal, (int)mzk_patientproblemmzk_Chronicity.Chronic);
            }

            if (!string.IsNullOrEmpty(SearchFilters))
            {
                if (SearchFilters == Convert.ToString(mzk_diagnosisfilter.Active))
                    childFilter.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientproblemmzk_status.Active));
                if (SearchFilters == Convert.ToString(mzk_diagnosisfilter.Resolved))
                    childFilter.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientproblemmzk_status.Resolved));   
            }

            if (OnlyActive)
            {
                childFilter.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientproblemmzk_status.Active));
            }

            //Search Date
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                childFilter.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });            

            LinkEntity EntityDiagnosis = new LinkEntity("mzk_patientproblem", "mzk_concept", "mzk_diagnosisconceptid", "mzk_conceptid", JoinOperator.Inner);
            EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_icdcodeid");
            //Search Order
            if (!string.IsNullOrEmpty(searchOrder))
                EntityDiagnosis.LinkCriteria.AddCondition("mzk_conceptname", ConditionOperator.Like, ("%" + searchOrder + "%"));
            //Patient Order Type :: Problem
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_diagnosisconceptid",
                                                                    "mzk_comments", "mzk_status",
                                                                    "mzk_chronicity",
                                                                    "mzk_problemtype", "mzk_onsetdate", "mzk_customerid", "createdon", "mzk_onsetnotes");
            OrderExpression order = new OrderExpression();
            order.AttributeName = "createdon";
            order.OrderType = OrderType.Descending;

            query.LinkEntities.Add(EntityDiagnosis);
            query.Orders.Add(order);

            if (pageNumber > 0)
            {
                query.PageInfo = new Microsoft.Xrm.Sdk.Query.PagingInfo();
                query.PageInfo.Count = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                query.PageInfo.PageNumber = pageNumber;
                query.PageInfo.PagingCookie = null;
                query.PageInfo.ReturnTotalRecordCount = true;
            }

            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection =  entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                PatientProblem model = new PatientProblem();

                if (entity.Attributes.Contains("mzk_patientproblemid"))
                    model.Id = entity.Id;

                if (entity.Attributes.Contains("mzk_concept1.mzk_conceptname"))
                    model.problemName = (entity.Attributes["mzk_concept1.mzk_conceptname"] as AliasedValue).Value.ToString();

                if (entity.Attributes.Contains("mzk_concept1.mzk_icdcodeid"))
                    model.IcdCode = ((EntityReference)((AliasedValue)entity.Attributes["mzk_concept1.mzk_icdcodeid"]).Value).Name;

                if (entity.Attributes.Contains("mzk_comments"))
                    model.onSetNotes = entity["mzk_comments"].ToString();
                if(entity.Attributes.Contains("mzk_onsetnotes"))
                    model.onSetNotes = entity["mzk_onsetnotes"].ToString();
                if (entity.Attributes.Contains("mzk_onsetdate"))
                    model.onSetDate = Convert.ToDateTime(entity["mzk_onsetdate"]);

                if (entity.Attributes.Contains("mzk_problemtype"))
                {
                    model.ProblemType = ((OptionSetValue)entity.Attributes["mzk_problemtype"]).Value;
                    model.ProblemTypeText = entity.FormattedValues["mzk_problemtype"].ToString();
                }
                if (entity.Attributes.Contains("mzk_chronicity"))
                {
                    model.Chronicity = ((OptionSetValue)entity.Attributes["mzk_chronicity"]).Value;
                    model.ChronicityText = entity.FormattedValues["mzk_chronicity"].ToString();
                }

                if (entity.Attributes.Contains("mzk_status"))
                {
                    model.status = (entity["mzk_status"] as OptionSetValue).Value;
                    model.StatusText = entity.FormattedValues["mzk_status"].ToString();
                }

                if (entity.Attributes.Contains("createdon"))
                    model.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);


                PatientProblem.Add(model);
            }

            if (pageNumber > 0 && entitycollection != null)
            {
                Pagination.totalCount = entitycollection.TotalRecordCount;
            }

            return PatientProblem;
        }

        public List<PatientProblem> getPatientProblemsFromList(List<string> patientguidList, string SearchFilters, DateTime startDate, DateTime endDate, bool isChronic)
        {
            List<PatientProblem> PatientProblem = new List<PatientProblem>();

            #region Patient Problem Query
            QueryExpression query = new QueryExpression("mzk_patientproblem");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);

            foreach (string patientId in patientguidList)
            {
                childFilter.AddCondition("mzk_customerid", ConditionOperator.Equal, new Guid(patientId));
            }

            if (isChronic)
            {
                query.Criteria.AddCondition("mzk_chronicity", ConditionOperator.Equal, (int)mzk_patientproblemmzk_Chronicity.Chronic);
            }

            if (!string.IsNullOrEmpty(SearchFilters))
            {
                if (SearchFilters == Convert.ToString(mzk_diagnosisfilter.Active))
                    childFilter.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientproblemmzk_status.Active));
                if (SearchFilters == Convert.ToString(mzk_diagnosisfilter.Resolved))
                    childFilter.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientproblemmzk_status.Resolved));
            }

            //Search Date
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                childFilter.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });

            LinkEntity EntityDiagnosis = new LinkEntity("mzk_patientproblem", "mzk_concept", "mzk_diagnosisconceptid", "mzk_conceptid", JoinOperator.Inner);
            EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_icdcodeid");

            //Patient Order Type :: Problem
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_diagnosisconceptid",
                                                                    "mzk_comments", "mzk_status",
                                                                    "mzk_chronicity",
                                                                    "mzk_problemtype", "mzk_onsetdate", "mzk_customerid", "createdon");
                        
            OrderExpression order = new OrderExpression();
            order.AttributeName = "createdon";
            order.OrderType = OrderType.Descending;

            query.LinkEntities.Add(EntityDiagnosis);
            query.Orders.Add(order);            

            #endregion

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            mzk_patientproblem problems;

            foreach (Entity entity in entitycollection.Entities)
            {
                PatientProblem modelProblem = new PatientProblem();

                problems = (mzk_patientproblem)entity;

                if (entity.Attributes.Contains("mzk_concept1.mzk_conceptname"))
                    modelProblem.problemName = (entity.Attributes["mzk_concept1.mzk_conceptname"] as AliasedValue).Value.ToString();
                if (problems.Attributes.Contains("mzk_customerid"))
                    modelProblem.PatientId = problems.mzk_customerid.Id.ToString();

                PatientProblem.Add(modelProblem);
            }           

            return PatientProblem;
        }

        public async Task<string> addPatientProblem(PatientProblem patientProblem)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            Entity patientProblemEntity = new Entity("mzk_patientproblem");

            if (patientProblem.problemId != null && patientProblem.problemId != string.Empty)
                patientProblemEntity.Attributes["mzk_diagnosisconceptid"] = new EntityReference("mzk_concept", new Guid(patientProblem.problemId));
            else
                throw new ValidationException("Problem must be selected");

            if (patientProblem.PatientId != null && patientProblem.PatientId != string.Empty)
                patientProblemEntity.Attributes["mzk_customerid"] = new EntityReference("contact", new Guid(patientProblem.PatientId));
            if (patientProblem.onSetNotes != null && patientProblem.onSetNotes != string.Empty)
                patientProblemEntity.Attributes["mzk_onsetnotes"] = patientProblem.onSetNotes;
            if (patientProblem.onSetDate != null && patientProblem.onSetDate != DateTime.MinValue)
                patientProblemEntity.Attributes["mzk_onsetdate"] = Convert.ToDateTime(patientProblem.onSetDate);
            if (patientProblem.ProblemType != 0 && patientProblem.ProblemType.ToString() != string.Empty)
                patientProblemEntity.Attributes["mzk_problemtype"] = new OptionSetValue(patientProblem.ProblemType);
            //if (patientProblem.Chronicity != 0 && patientProblem.Chronicity.ToString() != string.Empty)
                //patientProblemEntity.Attributes["mzk_chronicity"] = new OptionSetValue(patientProblem.Chronicity);

            patientProblemEntity.Attributes["mzk_status"] = new OptionSetValue((int)mzk_patientproblemmzk_status.Active);
            Id = entityRepository.CreateEntity(patientProblemEntity);

            return Id.ToString();
             
       }
        public async Task<bool> updatePatientProblem(PatientProblem patientProblem)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientProblem.Id.ToString()))
                {
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    Entity entity = new Entity("mzk_patientproblem");
                    entity.Id = new Guid(patientProblem.Id.ToString());
                    if (!string.IsNullOrEmpty(patientProblem.onSetNotes))
                    {
                        entity["mzk_onsetnotes"] = patientProblem.onSetNotes;
                    }
                    if (patientProblem.status != 0)
                    {
                        entity["mzk_status"] = new OptionSetValue(Convert.ToInt32(patientProblem.status));
                    }
                    if (patientProblem.onSetDate != DateTime.MinValue && patientProblem.onSetDate != null)
                    {
                        entity["mzk_onsetdate"] = patientProblem.onSetDate;
                    }
                    if (patientProblem.ProblemType != 0)
                    {
                        entity["mzk_problemtype"] = new OptionSetValue(Convert.ToInt32(patientProblem.ProblemType));
                    }

                    entityRepository.UpdateEntity(entity);
                    return true;
                }
                else
                {
                    throw new ValidationException("Id not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #region Old Code
            //try
            //{
            //    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            //    Entity encounterEntity = entityRepository.GetEntity("mzk_patientproblem", patientProblem.Id,
            //        new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_diagnosisconceptid", "mzk_status", "mzk_onsetdate", "mzk_comments", "mzk_customerid"));

            //    if (patientProblem.problemId != null && patientProblem.problemId != string.Empty)
            //        encounterEntity.Attributes["mzk_diagnosisconceptid"] = new EntityReference("mzk_concept", new Guid(patientProblem.problemId));
            //    if (patientProblem.PatientId != null && patientProblem.PatientId != string.Empty)
            //        encounterEntity.Attributes["mzk_customerid"] = new EntityReference("contact", new Guid(patientProblem.PatientId));
            //    if (patientProblem.onSetNotes != null && patientProblem.onSetNotes != string.Empty)
            //        encounterEntity.Attributes["mzk_comments"] = patientProblem.onSetNotes;
            //    if (patientProblem.onSetDate != null && patientProblem.onSetDate != DateTime.MinValue)
            //        encounterEntity.Attributes["mzk_onsetdate"] = Convert.ToDateTime(patientProblem.onSetDate);
            //    //if (patientProblem.ProblemType != 0 && patientProblem.ProblemType.ToString() != string.Empty)
            //    //    encounterEntity.Attributes["mzk_problemtype"] = new OptionSetValue(patientProblem.ProblemType);
            //    if (patientProblem.Chronicity != 0 && patientProblem.Chronicity.ToString() != string.Empty)
            //        encounterEntity.Attributes["mzk_chronicity"] = new OptionSetValue(patientProblem.Chronicity);

            //    entityRepository.UpdateEntity(encounterEntity);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            #endregion

        }

        public async Task<bool> updatePatientProblems( List<PatientProblem> problemList)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                foreach (PatientProblem model in problemList)
                {
                    Entity entity = null;
                    if (!string.IsNullOrEmpty(model.Id.ToString()))
                        entity = entityRepository.GetEntity("mzk_patientproblem", new Guid(model.Id.ToString()), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                    entity.Attributes["mzk_status"] = new OptionSetValue(Convert.ToInt32(mzk_patientproblemmzk_status.Resolved));
                    entityRepository.UpdateEntity(entity);
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
