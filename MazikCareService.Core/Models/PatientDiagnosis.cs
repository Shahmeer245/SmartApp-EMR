using MazikCareService.Core.Interfaces;
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
   public class PatientDiagnosis : IPatientDiagnosis
    {       
        public string EncounterId { get; set; }
        public string PatientId { get; set; }
        public string   DiagnosedById       { get; set; }
        public string DiagnosisId { get; set; }
        public string   DiagnosedByName     { get; set; }
        public DateTime DiagnosedOn         { get; set; }
        public string   Id         { get; set; }
        public string   DiagnosisName       { get; set; }
        public string   DiagnosisNumber     { get; set; }
        public int      Status              { get; set; }
        public string StatusText { get; set; }
        public string   OnsetNotes          { get; set; }
        public DateTime   OnsetDate { get; set; }
        public int      Risk                { get; set; }
        public int      ProblemType { get; set; }
        public string      ProblemTypeText { get; set; }
        public int Chronicity { get; set; }
        public string ChronicityText { get; set; }
        public DateTime CreatedOn { get; set; }
        public string IsProblem { get; set; }
        public string ICDCode { get; set; }
        public bool isOtherDiagnosis { get; set; }
        public string OtherDiagnosis { get; set; }
        public string CancelNotes { get; set; }
        public string PartnerHospitalId { get; set; }
        public string PartnerHospitalName { get; set; }
        public async Task<string> AddPatientDiagnosis(PatientDiagnosis patientDiagnosis)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            Entity patientDiagnosisEntity = new Entity("mzk_patientencounterdiagnosis");

            if (patientDiagnosis.isOtherDiagnosis == true)
            {
                Concept concept = new Concept();
                concept.ConceptType = ((int)mzk_conceptmzk_Category.Diagnosis).ToString();
                string OtherDiagnosisid = string.Empty;
                if (!string.IsNullOrEmpty(patientDiagnosis.OtherDiagnosis))
                {
                    concept.name = patientDiagnosis.OtherDiagnosis;
                    concept.ConceptNumber = patientDiagnosis.OtherDiagnosis;
                }
                else
                {
                    throw new ValidationException("Other Diagnosis  must be filled");
                }
                concept.addConcept(concept);
                patientDiagnosisEntity.Attributes["mzk_isother"] = patientDiagnosis.isOtherDiagnosis;
                if (!string.IsNullOrEmpty(patientDiagnosis.OtherDiagnosis))
                    patientDiagnosisEntity.Attributes["mzk_Other"]= patientDiagnosis.OtherDiagnosis;
            }

            if (patientDiagnosis.DiagnosisId != null && patientDiagnosis.DiagnosisId != string.Empty)
                patientDiagnosisEntity.Attributes["mzk_diagnosisconceptid"] = new EntityReference("mzk_concept", new Guid(patientDiagnosis.DiagnosisId));
            if (patientDiagnosis.EncounterId != null && patientDiagnosis.EncounterId != string.Empty)
            {
                patientDiagnosisEntity.Attributes["mzk_patientencounterid"] = new EntityReference("mzk_patientencounter", new Guid(patientDiagnosis.EncounterId));
                PatientEncounter encounter = new PatientEncounter();
                encounter.EncounterId = patientDiagnosis.EncounterId;
                PatientId = encounter.getEncounterDetails(encounter).Result.ToList().First<PatientEncounter>().PatientId;
                if (!string.IsNullOrEmpty(PatientId))
                    patientDiagnosisEntity.Attributes["mzk_customerid"] = new EntityReference("contact", new Guid(PatientId));
            }
            if (patientDiagnosis.OnsetNotes != null && patientDiagnosis.OnsetNotes != string.Empty)
                patientDiagnosisEntity.Attributes["mzk_comments"]           = patientDiagnosis.OnsetNotes;
            if (patientDiagnosis.OnsetDate != null && patientDiagnosis.OnsetDate != DateTime.MinValue)
                patientDiagnosisEntity.Attributes["mzk_onsetdate"]          =Convert.ToDateTime( patientDiagnosis.OnsetDate);
            if (patientDiagnosis.ProblemType != 0 && patientDiagnosis.ProblemType.ToString() != string.Empty)
                patientDiagnosisEntity.Attributes["mzk_problemtype"]        = new OptionSetValue(patientDiagnosis.ProblemType);
            if (patientDiagnosis.Chronicity != 0 && patientDiagnosis.Chronicity.ToString() != string.Empty)
                patientDiagnosisEntity.Attributes["mzk_chronicity"] = new OptionSetValue(patientDiagnosis.Chronicity);

            if (!string.IsNullOrEmpty(patientDiagnosis.IsProblem) && patientDiagnosis.IsProblem == "1")
                patientDiagnosisEntity.Attributes["mzk_isproblem"] = true;
            else
                patientDiagnosisEntity.Attributes["mzk_isproblem"] = false;

            patientDiagnosisEntity.Attributes["mzk_status"]             = new OptionSetValue(1);

          

            Id = Convert.ToString(entityRepository.CreateEntity(patientDiagnosisEntity));
            
            return Id;
        }

        public async Task<List<PatientDiagnosis>> getPatientDiagnosis(string encounterid, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, long appointmentRecId, string caseId)
        {
            List<PatientDiagnosis> PatientDiagnosis = new List<PatientDiagnosis>();
            #region Patient Diadnosis Query
            QueryExpression query = new QueryExpression("mzk_patientencounterdiagnosis");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);

            childFilter.AddCondition("mzk_status", ConditionOperator.NotEqual,(int)mzk_patientencounterdiagnosismzk_Status.Cancel);
            if (!string.IsNullOrEmpty(SearchFilters))
            {
                if (SearchFilters == Convert.ToString(mzk_diagnosisfilter.Active))
                    childFilter.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientencounterdiagnosismzk_Status.Active));
                if (SearchFilters == Convert.ToString(mzk_diagnosisfilter.Resolved))
                    childFilter.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientencounterdiagnosismzk_Status.Resolved));
            }

            //Search Date
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                childFilter.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });

            // Diagnosis
            if (!string.IsNullOrEmpty(encounterid))
            {
                childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, encounterid);
            }

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_diagnosisconceptid",
                                                                    "mzk_patientencounterid",
                                                                    "mzk_comments",
                                                                    "createdon",
                                                                    "createdby",
                                                                    "mzk_patientencounterdiagnosisnum",
                                                                    "mzk_status",
                                                                    "mzk_patientencounterdiagnosisid",
                                                                    "mzk_chronicity",
                                                                    "mzk_problemtype", "mzk_onsetdate", "mzk_isproblem", "mzk_customerid", "mzk_isother","mzk_other");

            LinkEntity EntityDiagnosis = new LinkEntity("mzk_patientencounterdiagnosis", "mzk_concept", "mzk_diagnosisconceptid", "mzk_conceptid", JoinOperator.Inner);
            EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_icdcodeid");
            EntityDiagnosis.EntityAlias = "Diagnosis";

            //Search Order
            if (!string.IsNullOrEmpty(searchOrder))
            {
                EntityDiagnosis.LinkCriteria.FilterOperator = LogicalOperator.Or;
                EntityDiagnosis.LinkCriteria.AddCondition("mzk_conceptname", ConditionOperator.Like, ("%" + searchOrder + "%"));
                EntityDiagnosis.LinkCriteria.AddCondition("mzk_icdcodeidname", ConditionOperator.Like, ("%" + searchOrder + "%"));
            }

            LinkEntity EntityEncounter = new LinkEntity("mzk_patientencounterdiagnosis", "mzk_patientencounter", "mzk_patientencounterid", "mzk_patientencounterid", JoinOperator.Inner);
            EntityEncounter.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_caseid", "mzk_patientencounterid");

            if (!string.IsNullOrEmpty(caseId))
            {
                EntityEncounter.LinkCriteria.AddCondition("mzk_caseid", ConditionOperator.Equal, new Guid(caseId));             
            }

            if (appointmentRecId > 0)
            {
                EntityEncounter.LinkCriteria.AddCondition("mzk_axrefappointmentrecid", ConditionOperator.Equal, Convert.ToDecimal(appointmentRecId));
            }

            LinkEntity EntityCase = new LinkEntity("mzk_patientencounter", "incident", "mzk_caseid", "incidentid", JoinOperator.Inner);
            EntityCase.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("customerid");
            

            OrderExpression order = new OrderExpression();
            order.AttributeName = "createdon";
            order.OrderType = OrderType.Descending;


            query.LinkEntities.Add(EntityDiagnosis);
            query.LinkEntities.Add(EntityEncounter);
            EntityEncounter.LinkEntities.Add(EntityCase);

            query.Orders.Add(order);
            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                PatientDiagnosis model = new PatientDiagnosis();

                if (entity.Attributes.Contains("Diagnosis.mzk_conceptname"))
                {
                    model.DiagnosisName = (entity.Attributes["Diagnosis.mzk_conceptname"] as AliasedValue).Value.ToString();
                }

                if (entity.Attributes.Contains("mzk_diagnosisconceptid"))
                {
                    model.DiagnosisId = ((EntityReference)entity["mzk_diagnosisconceptid"]).Id.ToString();
                }

                if (entity.Attributes.Contains("Diagnosis.mzk_icdcodeid"))
                {
                    model.ICDCode = ((EntityReference)((AliasedValue)entity.Attributes["Diagnosis.mzk_icdcodeid"]).Value).Name;
                }

                if (entity.Attributes.Contains("mzk_patientencounterid"))
                    model.EncounterId = ((EntityReference)entity["mzk_patientencounterid"]).Id.ToString();

                

                if (entity.Attributes.Contains("mzk_comments"))
                    model.OnsetNotes                = entity.Attributes["mzk_comments"].ToString();
                
                if (entity.Attributes.Contains("mzk_patientencounterdiagnosisnum"))
                    model.DiagnosisNumber = entity["mzk_patientencounterdiagnosisnum"].ToString();

                if (entity.Attributes.Contains("mzk_status"))
                {
                    model.Status = (entity["mzk_status"] as OptionSetValue).Value;
                    model.StatusText = entity.FormattedValues["mzk_status"].ToString();
                }
                
                if (entity.Attributes.Contains("mzk_onsetdate"))
                    model.OnsetDate = Convert.ToDateTime( entity.Attributes["mzk_onsetdate"]);

                if (entity.Attributes.Contains("mzk_isproblem") && entity.Attributes["mzk_isproblem"].ToString() == "True")
                    model.IsProblem = "1";
                else
                    model.IsProblem = "0";

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

                if (entity.Attributes.Contains("mzk_customerid"))
                {
                    model.PatientId = ((EntityReference)entity.Attributes["mzk_customerid"]).Id.ToString();
                }

                if (entity.Attributes.Contains("mzk_patientencounterdiagnosisid"))
                    model.Id = entity.Id.ToString();

                if (entity.Attributes.Contains("createdon"))
                    model.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);

                if (entity.Attributes.Contains("isother"))
                    model.isOtherDiagnosis = (bool)(entity.Attributes["isother"]);

                if (entity.Attributes.Contains("other"))
                    model.OtherDiagnosis = entity.Attributes["other"].ToString();


                PatientDiagnosis.Add(model);
            }
            return PatientDiagnosis;
        }

        public async Task<bool> updatePatientDiagnosis(PatientDiagnosis patientDiagnosis)
        {
            try
            {
                SoapEntityRepository enityRepository = SoapEntityRepository.GetService();
                Entity encounterEntity = enityRepository.GetEntity("mzk_patientencounterdiagnosis", new Guid(patientDiagnosis.Id) { }
                            , new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_diagnosisconceptid",
                                                                    "mzk_patientencounterid",
                                                                    "mzk_comments",
                                                                    "createdon",
                                                                    "createdby",
                                                                    "mzk_patientencounterdiagnosisnum",
                                                                    "mzk_status",
                                                                    "mzk_patientencounterdiagnosisid",
                                                                    "mzk_chronicity",
                                                                    "mzk_problemtype", "mzk_onsetdate", "mzk_isproblem"));

                if (patientDiagnosis.Status == (int)mzk_patientencounterdiagnosismzk_Status.Cancel)
                {
                    PatientDiagnosis diagnosis = new PatientDiagnosis();
                    if (diagnosis.getEncounterDiagnosis(((EntityReference)encounterEntity.Attributes["mzk_patientencounterid"]).Id.ToString()).Count > 1)
                    {
                        if (patientDiagnosis.Status != 0)
                            encounterEntity.Attributes["mzk_status"] = new OptionSetValue(patientDiagnosis.Status);

                        if (patientDiagnosis.CancelNotes != null && patientDiagnosis.CancelNotes != string.Empty)
                            encounterEntity.Attributes["mzk_cancelcomments"] = patientDiagnosis.CancelNotes;
                        enityRepository.UpdateEntity(encounterEntity);
                        return true;
                    }
                    else {
                        throw new ValidationException("At least one diagnosis must be active in encounter");
                    }
                }
                else
                {
                    if (patientDiagnosis.OnsetNotes != null && patientDiagnosis.OnsetNotes != string.Empty)
                        encounterEntity.Attributes["mzk_comments"] = patientDiagnosis.OnsetNotes;
                    if (patientDiagnosis.OnsetDate != null && patientDiagnosis.OnsetDate != DateTime.MinValue)
                        encounterEntity.Attributes["mzk_onsetdate"] = Convert.ToDateTime(patientDiagnosis.OnsetDate);
                    if (patientDiagnosis.ProblemType != 0 && patientDiagnosis.ProblemType.ToString() != string.Empty)
                        encounterEntity.Attributes["mzk_problemtype"] = new OptionSetValue(patientDiagnosis.ProblemType);
                    if (patientDiagnosis.Chronicity != 0 && patientDiagnosis.Chronicity.ToString() != string.Empty)
                        encounterEntity.Attributes["mzk_chronicity"] = new OptionSetValue(patientDiagnosis.Chronicity);
                    if (!string.IsNullOrEmpty(patientDiagnosis.IsProblem) && patientDiagnosis.IsProblem == "1")
                        encounterEntity.Attributes["mzk_isproblem"] = true;
                    else
                        encounterEntity.Attributes["mzk_isproblem"] = false;

                   
                    enityRepository.UpdateEntity(encounterEntity);
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool hasDiagnosis(string encounterid)
        {
            bool diagnosis = false;

            if (!CaseParameter.getDiagnosisRequired(PatientCase.getCaseType(encounterid)))
            {
                return true;
            }

            #region Patient Diadnosis Query
            QueryExpression query = new QueryExpression("mzk_patientencounterdiagnosis");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            // Diagnosis
            childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, encounterid);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientencounterdiagnosisid");

            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            if(entitycollection.Entities.Count>0)
               diagnosis = true;

            return diagnosis;
        }

        public List<PatientDiagnosis> getEncounterDiagnosis(string encounterid)
        {
            List<PatientDiagnosis> PatientDiagnosis = new List<PatientDiagnosis>();
            #region Patient Diadnosis Query
            QueryExpression query = new QueryExpression("mzk_patientencounterdiagnosis");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            // Diagnosis
            childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, encounterid);
            childFilter.AddCondition("mzk_status", ConditionOperator.NotEqual, 2);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_diagnosisconceptid",
                                                                    "mzk_patientencounterid",
                                                                    "mzk_patientencounterdiagnosisnum");

            LinkEntity EntityDiagnosis = new LinkEntity("mzk_patientencounterdiagnosis", "mzk_concept", "mzk_diagnosisconceptid", "mzk_conceptid", JoinOperator.LeftOuter);
            EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname");

            query.LinkEntities.Add(EntityDiagnosis);

            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                PatientDiagnosis model = new PatientDiagnosis();
                if (entity.Attributes.Contains("mzk_concept1.mzk_conceptname"))
                    model.DiagnosisName = (entity.Attributes["mzk_concept1.mzk_conceptname"] as AliasedValue).Value.ToString();



                if (entity.Attributes.Contains("mzk_patientencounterdiagnosisnum"))
                    model.DiagnosisNumber = entity.Attributes["mzk_patientencounterdiagnosisnum"].ToString();

                if (entity.Attributes.Contains("mzk_diagnosisconceptid"))
                    model.DiagnosisId = ((EntityReference)entity.Attributes["mzk_diagnosisconceptid"]).Id.ToString();
                PatientDiagnosis.Add(model);
            }
            return PatientDiagnosis;
        }
    }
}


