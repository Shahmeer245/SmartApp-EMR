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
    public class PatientAllergy : IPatientAllergy
    {     
        public string Id
        {
            get; set;
        }
        
        public string name { get; set; }
        public Guid patientId { get; set; }
        public string patient { get; set; }
        public DateTime onsetDate{ get;set;}
        public Concept allergen{get;set;}
        public string status{get; set;}
        public string statusText{get; set;}
        public string onsetNotes{get; set;}
        public string reactionId{get; set;}
        public string reactionText{get; set;}
        public string deactivateReason {get; set;}
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string AllergyReviewedBy { get; set;}
        public bool isEdit { get; set; }
        public bool isOtherAllergy { get; set; }
        public string OtherAllergy { get; set; }
        public bool isOtherReaction { get; set; }
        public string OtherReaction { get; set; }
        public string reactionName { get; set; }
        public string reaction1 { get; set; }
        public string reaction2 { get; set; }
        public string reaction3 { get; set; }

        public string Externalemrid { get; set; }

        public DateTime RecordedDate { get; set; }

        public async Task<List<PatientAllergy>> getPatientAllergies(string patientguid,  string SearchFilters, string searchAllergy, DateTime startDate, DateTime endDate,bool OnlyActive = false)
        {
            List<PatientAllergy> patAllergy = new List<PatientAllergy>();

            try
            {
                #region Patient Allergy Query

                QueryExpression query = new QueryExpression();
                query.EntityName = "mzk_patientallergies";
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_status", "mzk_onsetdate", "mzk_comments", "mzk_patientallergiesid", "createdon", "createdby", "mzk_isother", "mzk_other", "mzk_isotherreaction", "mzk_otherreaction","mzk_reaction1","mzk_reaction2", "mzk_reaction3");
                if (!string.IsNullOrEmpty(patientguid))
                {
                    query.Criteria.AddCondition("mzk_customerid", ConditionOperator.Equal, new Guid(patientguid));
                }
                //Search Filter
                if (!string.IsNullOrEmpty(SearchFilters))
                {
                    if (SearchFilters == Convert.ToString(mzk_allergyfilter.Active))
                        query.Criteria.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientallergiesmzk_Status.Active));
                    if (SearchFilters == Convert.ToString(mzk_allergyfilter.Resolved))
                        query.Criteria.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientallergiesmzk_Status.Resolved));
                }
                
                //Search Date
                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                    query.Criteria.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });

                //only Active
                if (OnlyActive)
                {
                    query.Criteria.AddCondition("mzk_status", ConditionOperator.Equal, (int)mzk_patientallergiesmzk_Status.Active);
                }
                else
                {
                    query.Criteria.AddCondition("mzk_status", ConditionOperator.NotEqual, 3);
                }


                LinkEntity EntityConcept = new LinkEntity("mzk_patientallergies", "mzk_concept", "mzk_conceptid", "mzk_conceptid", JoinOperator.Inner);
                EntityConcept.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_conceptnumber", "mzk_allergentype");
                //Search Allergy
                if (!string.IsNullOrEmpty(searchAllergy))
                    EntityConcept.LinkCriteria.AddCondition("mzk_conceptname", ConditionOperator.Like, ("%" + searchAllergy + "%"));

                //LinkEntity EntityReactions = new LinkEntity("mzk_patientallergies", "mzk_patientallergyreaction", "mzk_patientallergiesid", "mzk_patientallergy", JoinOperator.LeftOuter);
                //EntityReactions.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_clinicalobservationconceptidid","mzk_name");

                //LinkEntity ReactionName = new LinkEntity("mzk_patientallergyreaction", "mzk_concept", "mzk_clinicalobservationconceptidid", "mzk_conceptid", JoinOperator.LeftOuter);
                //ReactionName.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname");
                query.LinkEntities.Add(EntityConcept);
                //query.LinkEntities.Add(EntityReactions);
                //EntityReactions.LinkEntities.Add(ReactionName);

                #endregion
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(query);

                mzk_patientallergies allergies;

                foreach (Entity entity in entitycol.Entities)
                {

                    PatientAllergy model = new PatientAllergy();

                    allergies = (mzk_patientallergies)entity;

                    model.Id = allergies.mzk_patientallergiesId.Value.ToString();
                    model.onsetNotes = allergies.mzk_comments;

                    if (allergies.mzk_OnsetDate.HasValue)
                        model.onsetDate = allergies.mzk_OnsetDate.Value;// String.Format("{0:" + CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + "}", allergies.mzk_OnsetDate.Value);
                    if (allergies.mzk_Status != null)
                    {
                        model.statusText = allergies.FormattedValues["mzk_status"].ToString();
                        model.status = (allergies.mzk_Status as OptionSetValue).Value.ToString();

                        if (model.status == Convert.ToInt32(mzk_patientallergiesmzk_Status.Resolved).ToString())
                        {
                            model.isEdit = false;
                        }
                        else
                        {
                            model.isEdit = true;
                        }
                    }
                    //if (entity.Attributes.Contains("mzk_patientallergyreaction2.mzk_clinicalobservationconceptidid"))
                    //{
                    //    model.reactionId = ((entity.Attributes["mzk_patientallergyreaction2.mzk_clinicalobservationconceptidid"] as AliasedValue).Value as EntityReference).Id.ToString();
                    //}
                    //if (entity.Attributes.Contains("mzk_patientallergyreaction2.mzk_name"))
                    //{
                    //    model.reactionName = ((entity.Attributes["mzk_patientallergyreaction2.mzk_name"] as AliasedValue).Value).ToString();
                    //}
                    if (entity.Attributes.Contains("mzk_reaction1"))
                    {
                        model.reactionName = (entity["mzk_reaction1"] as EntityReference).Name;
                        if (entity.Attributes.Contains("mzk_reaction2"))
                        {
                            model.reactionName += "," + (entity["mzk_reaction2"] as EntityReference).Name;
                        }
                        if (entity.Attributes.Contains("mzk_reaction3"))
                        {
                            model.reactionName += "," + (entity["mzk_reaction3"] as EntityReference).Name;
                        }

                    }
                    else if (entity.Attributes.Contains("mzk_reaction2"))
                    {
                        model.reactionName = (entity["reaction2"] as EntityReference).Name;
                        if (entity.Attributes.Contains("mzk_reaction3"))
                        {
                            model.reactionName += "," + (entity["mzk_reaction3"] as EntityReference).Name;
                        }
                    }
                    else if (entity.Attributes.Contains("mzk_reaction3"))
                    {
                        model.reactionName = (entity["mzk_reaction3"] as EntityReference).Name;
                    }

                    //if (entity.Attributes.Contains("mzk_concept3.mzk_conceptname"))
                    //{
                    //    model.reactionText = (entity.Attributes["mzk_concept3.mzk_conceptname"] as AliasedValue).Value.ToString();
                    //}
                    if (entity.Attributes.Contains("mzk_concept1.mzk_conceptnumber") && entity.Attributes.Contains("mzk_concept1.mzk_conceptname"))
                    {
                        model.allergen = new Concept()
                        {
                            conceptId = (entity.Attributes["mzk_concept1.mzk_conceptnumber"] as AliasedValue).Value.ToString(),
                            name = (entity.Attributes["mzk_concept1.mzk_conceptname"] as AliasedValue).Value.ToString()                            
                        };

                        if(entity.Contains("mzk_concept1.mzk_allergentype"))
                        {
                            model.allergen.ConceptType = entity.FormattedValues["mzk_concept1.mzk_allergentype"].ToString();
                        }
                    }

                    if (entity.Attributes.Contains("createdon"))
                        model.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);

                    if (entity.Attributes.Contains("createdby"))
                        model.CreatedBy = (entity.Attributes["createdby"] as EntityReference).Name;

                    if (entity.Attributes.Contains("isother"))
                        model.isOtherAllergy = (bool)(entity.Attributes["isother"]);

                    if (entity.Attributes.Contains("other"))
                        model.OtherAllergy = entity.Attributes["other"].ToString();

                    if (entity.Attributes.Contains("isotherreaction"))
                        model.isOtherReaction = (bool)(entity.Attributes["isotherreaction"]);

                    if (entity.Attributes.Contains("otherreaction"))
                        model.OtherReaction = entity.Attributes["otherreaction"].ToString();

                    patAllergy.Add(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return patAllergy;
        }

        public List<PatientAllergy> getPatientAllergiesFromList(List<string> patientguid, string SearchFilters, DateTime startDate, DateTime endDate)
        {
            List<PatientAllergy> patAllergy = new List<PatientAllergy>();

            try
            {

                #region Patient Allergy Query

                QueryExpression query = new QueryExpression();
                query.EntityName = "mzk_patientallergies";
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_customerid");

                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);

                foreach (string patientId in patientguid)
                {
                    childFilter.AddCondition("mzk_customerid", ConditionOperator.Equal, new Guid(patientId));
                }

                //Search Filter
                if (!string.IsNullOrEmpty(SearchFilters))
                {
                    if (SearchFilters == Convert.ToString(mzk_allergyfilter.Active))
                        query.Criteria.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientallergiesmzk_Status.Active));
                    if (SearchFilters == Convert.ToString(mzk_allergyfilter.Resolved))
                        query.Criteria.AddCondition("mzk_status", ConditionOperator.Equal, Convert.ToInt32(mzk_patientallergiesmzk_Status.Resolved));
                }
                query.Criteria.AddCondition("mzk_status", ConditionOperator.NotEqual, 3);
                //Search Date
                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                    query.Criteria.AddCondition("createdon", ConditionOperator.Between, new Object[] { startDate, endDate.AddHours(12) });                

                LinkEntity EntityConcept = new LinkEntity("mzk_patientallergies", "mzk_concept", "mzk_conceptid", "mzk_conceptid", JoinOperator.Inner);
                EntityConcept.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_conceptnumber");

                query.LinkEntities.Add(EntityConcept);

                #endregion
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(query);

                mzk_patientallergies allergies;
                
                foreach (Entity entity in entitycol.Entities)
                {
                    PatientAllergy modelAllergy = new PatientAllergy();

                    allergies = (mzk_patientallergies)entity;

                    if (entity.Attributes.Contains("mzk_concept1.mzk_conceptnumber") && entity.Attributes.Contains("mzk_concept1.mzk_conceptname"))
                    {
                        modelAllergy.allergen = new Concept()
                        {

                            conceptId = (entity.Attributes["mzk_concept1.mzk_conceptnumber"] as AliasedValue).Value.ToString(),
                            name = (entity.Attributes["mzk_concept1.mzk_conceptname"] as AliasedValue).Value.ToString()
                        };
                    }

                    if (allergies.Attributes.Contains("mzk_customerid"))
                        modelAllergy.patient = allergies.mzk_customerid.Id.ToString();

                    patAllergy.Add(modelAllergy);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return patAllergy;
        }

        public async Task<string> addPatientAllergy(PatientAllergy patientAllergy)
        {
            try
            {
                Guid PatientAllergyId;
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                mzk_patientallergies entity = new mzk_patientallergies();
                entity.mzk_Status = new OptionSetValue((int)mzk_patientallergiesmzk_Status.Active);

                //if (patientAllergy.onsetDate > DateTime.Now.Date)
                //    throw new ValidationException("OnSet Date must be less or equal to Today's Date");

                if (patientAllergy.onsetNotes != string.Empty)
                {
                    if (patientAllergy.onsetNotes.Length <= 1000)
                        entity.mzk_comments = patientAllergy.onsetNotes;
                    else
                        throw new ValidationException("The length of onset Notes exceeded the maximum allowed length of 1000");
                }

                if (patientAllergy.onsetDate != DateTime.MinValue)
                    entity.mzk_OnsetDate = patientAllergy.onsetDate;

                if (patientAllergy.name.ToString() != string.Empty)
                    entity.LogicalName = patientAllergy.name;

                if (patientAllergy.patient.ToString() != string.Empty)
                    entity.mzk_customerid = new EntityReference("contact", new Guid(patientAllergy.patient));

                if (patientAllergy.allergen != null && patientAllergy.allergen.ToString() != string.Empty)
                { 
                    if(!string.IsNullOrEmpty(patientAllergy.allergen.conceptId))
                    entity.mzk_ConceptId = new EntityReference("mzk_concept", new Guid(patientAllergy.allergen.conceptId));
                }
                if (!string.IsNullOrEmpty(patientAllergy.reaction1))
                {
                    entity["mzk_reaction1"] = new EntityReference("mzk_concept", new Guid(patientAllergy.reaction1));
                }
                if (!string.IsNullOrEmpty(patientAllergy.reaction2))
                {
                    entity["mzk_reaction2"] = new EntityReference("mzk_concept", new Guid(patientAllergy.reaction2));
                }
                if (!string.IsNullOrEmpty(patientAllergy.reaction3))
                {
                    entity["mzk_reaction3"] = new EntityReference("mzk_concept", new Guid(patientAllergy.reaction3));
                }
                PatientAllergyId = entityRepository.CreateEntity(entity);
                return PatientAllergyId.ToString();
                //else
                //{
                //    if (patientAllergy.isOtherAllergy == true)
                //    {
                //        Concept concept = new Concept();
                //        concept.ConceptType = ((int)mzk_conceptmzk_Category.Allergy).ToString();
                //        if (!string.IsNullOrEmpty(patientAllergy.OtherAllergy))
                //        {
                //            concept.name = patientAllergy.OtherAllergy;
                //            concept.ConceptNumber = patientAllergy.OtherAllergy;
                //        }
                //        else
                //        {
                //            throw new ValidationException("Other Allergy name must be filled");
                //        }
                //     string cId =   concept.addConcept(concept);
                //        entity.mzk_ConceptId = new EntityReference("mzk_concept", new Guid(cId));
                //        entity.mzk_isother = patientAllergy.isOtherAllergy;
                //        if (!string.IsNullOrEmpty(patientAllergy.OtherAllergy))
                //        entity.mzk_Other = patientAllergy.OtherAllergy;
                //    }
                //    else
                //    {
                //        if (entity.mzk_ConceptId == null)
                //        {
                //            throw new ValidationException("Allergy name must be selected");
                //        }
                //    }
                //}
                //PatientAllergyId = entityRepository.CreateEntity(entity);

                //string Otherreactionid = string.Empty;
                //if (patientAllergy.isOtherReaction == true)
                //{
                //    Concept concept = new Concept();
                //    concept.ConceptType =((int) mzk_conceptmzk_Category.Reaction).ToString();
                //    if (!string.IsNullOrEmpty(patientAllergy.OtherReaction))
                //    {
                //        concept.name = patientAllergy.OtherReaction;
                //        concept.ConceptNumber = patientAllergy.OtherReaction;
                //    }
                //    else
                //    {
                //        throw new ValidationException("Other Reaction  must be filled");
                //    }
                //    Otherreactionid= concept.addConcept(concept);
                //    entity.mzk_isOtherReaction = patientAllergy.isOtherReaction;
                //    if (!string.IsNullOrEmpty(patientAllergy.OtherReaction))
                //        entity.mzk_OtherReaction = patientAllergy.OtherReaction;
                //}

                //Entity reactionEntity = new Entity(mzk_patientallergyreaction.EntityLogicalName);

                //if (!string.IsNullOrEmpty(patientAllergy.reactionId))
                //    reactionEntity.Attributes["mzk_clinicalobservationconceptidid"] = new EntityReference("mzk_concept", new Guid(patientAllergy.reactionId));
                //else
                //    if (!string.IsNullOrEmpty(Otherreactionid))
                //    reactionEntity.Attributes["mzk_clinicalobservationconceptidid"] = new EntityReference("mzk_concept", new Guid(Otherreactionid));

                //if (!string.IsNullOrEmpty(PatientAllergyId.ToString()))
                //    reactionEntity.Attributes["mzk_patientallergy"] = new EntityReference("mzk_patientallergies", PatientAllergyId);

                //    entityRepository.CreateEntity(reactionEntity);
                //if (PatientAllergyId != null)
                //{
                //    Patient mzkpatient = new Patient();
                //    if (!string.IsNullOrEmpty(patientAllergy.patient))
                //        mzkpatient.patientId = patientAllergy.patient;

                //        mzkpatient.AllergyStatus = mzk_allergystatus.AllergyAdded;

                //    if (!string.IsNullOrEmpty(patientAllergy.AllergyReviewedBy))
                //        mzkpatient.AllergyReviewedby = patientAllergy.AllergyReviewedBy;

                //    await mzkpatient.UpdatePatientDetail(mzkpatient);
                //}
                //return PatientAllergyId.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> updatePatientAllergy(PatientAllergy patientAllergy)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientAllergy.Id))
                {
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    Entity entity = new Entity("mzk_patientallergies");
                    entity.Id = new Guid(patientAllergy.Id);
                    if (!string.IsNullOrEmpty(patientAllergy.onsetNotes))
                    {
                        entity["mzk_comments"] = patientAllergy.onsetNotes;
                    }
                    if (!string.IsNullOrEmpty(patientAllergy.status))
                    {
                        entity["mzk_status"] = new OptionSetValue(Convert.ToInt32(patientAllergy.status));
                    }
                    if (patientAllergy.onsetDate != DateTime.MinValue && patientAllergy.onsetDate != null)
                    {
                        entity["mzk_onsetdate"] = patientAllergy.onsetDate;
                    }
                    if (!string.IsNullOrEmpty(patientAllergy.deactivateReason))
                    {
                        entity["mzk_deactivatereason"] = patientAllergy.deactivateReason;
                    }
                    entity["mzk_isother"] = patientAllergy.isOtherAllergy;
                    if (!string.IsNullOrEmpty(patientAllergy.OtherAllergy))
                    {
                        entity["mzk_other"] = patientAllergy.OtherAllergy;
                    }
                    if (!string.IsNullOrEmpty(patientAllergy.reaction1))
                    {
                        entity["mzk_reaction1"] = new EntityReference("mzk_concept", new Guid(patientAllergy.reaction1));
                    }
                    if (!string.IsNullOrEmpty(patientAllergy.reaction2))
                    {
                        entity["mzk_reaction2"] = new EntityReference("mzk_concept", new Guid(patientAllergy.reaction2));
                    }
                    if (!string.IsNullOrEmpty(patientAllergy.reaction3))
                    {
                        entity["mzk_reaction3"] = new EntityReference("mzk_concept", new Guid(patientAllergy.reaction3));
                    }
                    entityRepository.UpdateEntity(entity);
                    return true;
                }
                else
                {
                    throw new ValidationException("Id not found");
                }
                //bool isNewReaction = true;
                //Entity entity = entityRepository.GetEntity("mzk_patientallergies", new Guid(patientAllergy.Id),
                //    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_comments", "mzk_status", "mzk_onsetdate"));

                //if (patientAllergy.onsetNotes != string.Empty)
                //    if (patientAllergy.onsetNotes.Length <= 1000)
                //        entity.Attributes["mzk_comments"] = patientAllergy.onsetNotes;
                //    else
                //        throw new ValidationException("The length of onset Notes exceeded the maximum allowed length of 1000");


                //if (patientAllergy.onsetDate != DateTime.MinValue)
                //    entity.Attributes["mzk_onsetdate"] = patientAllergy.onsetDate;

                //entityRepository.UpdateEntity(entity);

                //QueryExpression query = new QueryExpression();

                //query.EntityName = "mzk_patientallergyreaction";

                //query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientallergyreactionid");

                //query.Criteria.AddCondition("mzk_patientallergy", ConditionOperator.Equal, new Guid(patientAllergy.Id));

                //EntityCollection entityCollection = entityRepository.GetEntityCollection(query);

                //foreach (Entity entityChild in entityCollection.Entities)
                //{
                //    entityRepository.DeleteEntity("mzk_patientallergyreaction", new Guid(entityChild.Attributes["mzk_patientallergyreactionid"].ToString()));
                //}
                                
                //Entity reactionEntity = new Entity(mzk_patientallergyreaction.EntityLogicalName);

                //if (patientAllergy.reactionId != string.Empty)
                //    reactionEntity.Attributes["mzk_clinicalobservationconceptidid"] = new EntityReference("mzk_concept", new Guid(patientAllergy.reactionId));
                //if (patientAllergy.Id != string.Empty)
                //    reactionEntity.Attributes["mzk_patientallergy"] = new EntityReference("mzk_patientallergies", new Guid(patientAllergy.Id));

                //entityRepository.CreateEntity(reactionEntity);
                    
                //return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> updatePatientAllergies(string patientGuid,string AllergyReviewedBy, List<PatientAllergy> allergyList)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
              
                foreach (PatientAllergy model in allergyList)
                {
                    
                    Entity entity = null;
                    if (!string.IsNullOrEmpty(model.Id))
                        entity = entityRepository.GetEntity("mzk_patientallergies", new Guid(model.Id),new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_comments", "mzk_status", "mzk_onsetdate"));
                    
                    //if (!string.IsNullOrEmpty(model.status))
                    entity.Attributes["mzk_status"] =new OptionSetValue(Convert.ToInt32(model.status));
                    entity.Attributes["mzk_deactivatereason"] = model.deactivateReason;
                    entityRepository.UpdateEntity(entity);
                                        
                    xrm.mzk_patientallergylog patientAllergiesLogEntity = new xrm.mzk_patientallergylog();
                    if (!string.IsNullOrEmpty(model.Id))
                        patientAllergiesLogEntity.mzk_PatientAllergyId = new EntityReference("mzk_patientallergies",new Guid(model.Id));

                    if (!string.IsNullOrEmpty(model.status))
                        patientAllergiesLogEntity.mzk_Status = new OptionSetValue(Convert.ToInt32(model.status));

                        Id = Convert.ToString(entityRepository.CreateEntity(patientAllergiesLogEntity));
                        status = model.status;
                }

                //if (Id != null&& status==Convert.ToInt32(mzk_patientallergiesmzk_Status.Resolved).ToString())
                //{
                //    if (getActiveAllergyCount(patientGuid, mzk_patientallergiesmzk_Status.Active) == 0)
                //    {
                //        Patient mzkpatient = new Patient();
                //        mzkpatient.patientId = patientGuid;
                //        mzkpatient.AllergyStatus = mzk_allergystatus.NoKnownAllergy;
                //        mzkpatient.AllergyReviewedby = AllergyReviewedBy;
                //        mzkpatient.UpdatePatientDetail(mzkpatient);
                //    }
                //}

                        return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int getActiveAllergyCount(string patientGuid, mzk_patientallergiesmzk_Status allergyStatus)
        {
            QueryExpression query = new QueryExpression();
            query.EntityName = mzk_patientallergies.EntityLogicalName;
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("createdon");
            if (!string.IsNullOrEmpty(patientGuid))
            {
                query.Criteria.AddCondition("mzk_customerid", ConditionOperator.Equal, new Guid(patientGuid));
            }
            query.Criteria.AddCondition("mzk_status"    , ConditionOperator.Equal ,Convert.ToInt32( allergyStatus));

            SoapEntityRepository repo = SoapEntityRepository.GetService();
            EntityCollection entitycol = repo.GetEntityCollection(query);
            
            return entitycol.Entities.Count;
        }

        public async Task<Guid> CreatePatientAllergyCRM(PatientAllergy allergy)
        {
            try
            {

                Entity contact = new Entity("msemr_allergyintolerance");

                if (allergy.Externalemrid != "")
                {
                    contact["mzk_externalemrid"] = allergy.Externalemrid;
                }
                if (allergy.patientId != Guid.Empty)
                {
                    contact["msemr_patient"] = new EntityReference("contact", allergy.patientId);
                }
                if (allergy.name != "")
                {
                    contact["msemr_name"] = allergy.name;
                }
                if (allergy.RecordedDate != null)
                {
                    contact["mzk_authoredon"] = Convert.ToDateTime(allergy.RecordedDate);
                }
                

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();


                QueryExpression queryExpression = new QueryExpression("msemr_allergyintolerance");
                queryExpression.Criteria.AddCondition("mzk_externalemrid", ConditionOperator.Equal, allergy.Externalemrid);

                queryExpression.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryExpression);

                if (entitycollection != null)
                {
                    if (entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities[0].Attributes.Contains("msemr_allergyintoleranceid"))
                        {
                            contact["msemr_allergyintoleranceid"] = new Guid(entitycollection.Entities[0].Attributes["msemr_allergyintoleranceid"].ToString());
                        }
                        entityRepository.UpdateEntity(contact);
                        contact.Id = new Guid(contact["msemr_allergyintoleranceid"].ToString());
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

        public async Task<List<PatientAllergy>> getPatientAllergiesCRM(string patientGuid, DateTime startdate, DateTime enddate)
        {
            try
            {
                List<PatientAllergy> list = new List<PatientAllergy>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression("msemr_allergyintolerance");

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
                        PatientAllergy obj = new PatientAllergy();
                        obj = getPatientAllergyModelFilled(entitycollection[i], obj, "");
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


        public static PatientAllergy getPatientAllergyModelFilled(Entity entity, PatientAllergy patient, string accountAlias)
        {
            if (entity.Attributes.Contains("msemr_name"))
            {
                patient.name = (entity.Attributes["msemr_name"]).ToString();
            }
            if (entity.Attributes.Contains("msemr_allergyintoleranceid"))
            {
                patient.Id = (entity.Attributes["msemr_allergyintoleranceid"]).ToString();
            }
            if (entity.Attributes.Contains("msemr_patient"))
            {
                patient.patientId = new Guid((entity.Attributes["msemr_patient"] as EntityReference).Id.ToString());
            }
            if (entity.Attributes.Contains("createdon"))
            {
                patient.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);
            }
            if (entity.Attributes.Contains("mzk_authoredon"))
            {
                patient.RecordedDate = Convert.ToDateTime(entity.Attributes["mzk_authoredon"]);
            }
            

            return patient;
        }
    }
}
