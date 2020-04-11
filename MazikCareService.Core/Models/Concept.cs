using Helper;
using MazikCareService.CRMRepository;
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
   public class Concept
    {
        public string conceptId { get; set; }
        public string name { get; set; }
        public string ConceptNumber { get; set; }
        public string ConceptType { get; set; }
        public string ICDCode { get; set; }


        //mzk_icdcodeid
        public List<Concept> getConcept(string Type)
        {
            List<Concept> Concept = new List<Concept>();
            #region Patient Order Setup
            QueryExpression query = new QueryExpression("mzk_concept");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            childFilter.AddCondition("mzk_category", ConditionOperator.Equal, Type);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptid","mzk_conceptname", "mzk_conceptnumber");
            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                Concept model = new Concept();
                if(entity.Attributes.Contains("mzk_conceptname"))
                model.name = entity.Attributes["mzk_conceptname"].ToString();

                if (entity.Attributes.Contains("mzk_conceptnumber"))
                    model.ConceptNumber = entity.Attributes["mzk_conceptnumber"].ToString();

                if (entity.Attributes.Contains("mzk_conceptid"))
                    model.conceptId = entity.Id.ToString();
                Concept.Add(model);
            }
            return Concept;
        }

       /* public async Task<List<Concept>> getConceptData(Concept concept, int pageNumber = 0)
        {
            List<Concept> Concept = new List<Concept>();
            #region Concept
            QueryExpression query = new QueryExpression("mzk_concept");

            if (Convert.ToInt32(concept.ConceptType) == (int)mzk_conceptmzk_Category.Diagnosis)
            {
                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);

                childFilter.AddCondition("mzk_conceptname", ConditionOperator.Like, ("%" + concept.name.ToLower() + "%"));
                childFilter.AddCondition("mzk_conceptnumber", ConditionOperator.Like, ("%" + concept.name.ToLower() + "%"));
            }
            else
            {
                ConditionExpression condition1 = new ConditionExpression();
                condition1.AttributeName = "mzk_conceptname";
                condition1.Operator = ConditionOperator.Like;
                condition1.Values.Add("%" + concept.name.ToLower() + "%");

                FilterExpression filter1 = new FilterExpression();

                filter1.Conditions.Add(condition1);

                query.Criteria.AddFilter(filter1);
            }

            ConditionExpression condition2 = new ConditionExpression();
            condition2.AttributeName = "mzk_category";
            condition2.Operator = ConditionOperator.Equal;
            condition2.Values.Add(concept.ConceptType);

            
            FilterExpression filter2 = new FilterExpression();
            
            filter2.Conditions.Add(condition2);
                        
            query.Criteria.AddFilter(filter2);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_conceptid", "mzk_conceptnumber", "mzk_icdcodeid");
            LinkEntity EntityUserFavourite;
            if (!string.IsNullOrEmpty(concept.filter) && concept.filter == "favourite")
            {
                EntityUserFavourite = new LinkEntity("mzk_concept", "mzk_userfavourite", "mzk_conceptid", "mzk_conceptid", JoinOperator.Inner);

                EntityUserFavourite.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_userfavouriteid");
                EntityUserFavourite.LinkCriteria.AddCondition("mzk_userid", ConditionOperator.Equal, concept.UserId);
            }
            else
            {
                EntityUserFavourite = new LinkEntity("mzk_concept", "mzk_userfavourite", "mzk_conceptid", "mzk_conceptid", JoinOperator.LeftOuter);
                EntityUserFavourite.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            }
            EntityUserFavourite.EntityAlias = "ConceptFavourite";
            if (!string.IsNullOrEmpty(concept.UserId))
            {
                EntityUserFavourite.LinkCriteria.AddCondition("mzk_userid", ConditionOperator.Equal, concept.UserId);
                query.LinkEntities.Add(EntityUserFavourite);
            }

            if (pageNumber > 0)
            {
                query.PageInfo = new PagingInfo();
                query.PageInfo.Count = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                query.PageInfo.PageNumber = pageNumber;
                query.PageInfo.PagingCookie = null;
                query.PageInfo.ReturnTotalRecordCount = true;
            }

            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            List<string> conceptExistList = new List<string>();

            if (!string.IsNullOrEmpty(concept.EncounterId) && Convert.ToInt32(concept.ConceptType) == (int)mzk_conceptmzk_Category.Diagnosis)
            {
                query = new QueryExpression(mzk_patientencounterdiagnosis.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, concept.EncounterId);
                query.Criteria.AddCondition("mzk_status", ConditionOperator.Equal, (int)mzk_patientencounterdiagnosismzk_Status.Active);

                EntityCollection entitycollectionDiagnosis = entityRepository.GetEntityCollection(query);

                foreach (Entity entity in entitycollectionDiagnosis.Entities)
                {
                    mzk_patientencounterdiagnosis order = (mzk_patientencounterdiagnosis)entity;

                    conceptExistList.Add(order.mzk_DiagnosisConceptId.Id.ToString());
                }
            }

            foreach (Entity entity in entitycollection.Entities)
            {
                Concept model = new Concept();
                if (entity.Attributes.Contains("mzk_conceptname"))
                    model.name = entity.Attributes["mzk_conceptname"].ToString();

                if (entity.Attributes.Contains("mzk_conceptnumber"))
                    model.ConceptNumber = entity.Attributes["mzk_conceptnumber"].ToString();

                if (entity.Attributes.Contains("mzk_icdcodeid"))
                    model.ICDCode = ((EntityReference)entity.Attributes["mzk_icdcodeid"]).Name;

                if (entity.Attributes.Contains("mzk_conceptid"))
                    model.conceptId = entity.Id.ToString();

                if (!string.IsNullOrEmpty(concept.EncounterId))
                {
                    model.isAdded = !conceptExistList.Exists(item => item == entity.Id.ToString());
                    //model.isAdded = this.DuplicateDetection(concept.EncounterId, model.conceptId);
                }

                //Favourite  "mzk_userfavourite5.mzk_userfavouriteid"
                if (entity.Attributes.Contains("ConceptFavourite.mzk_userfavouriteid"))
                    model.FavouriteId = (entity.Attributes["ConceptFavourite.mzk_userfavouriteid"] as AliasedValue).Value.ToString();

                Concept.Add(model);
            }

            if (pageNumber > 0 && entitycollection != null)
            {
                Pagination.totalCount = entitycollection.TotalRecordCount;
            }

            return Concept;
        }
        */
        public bool DuplicateDetection(string EncounterId, string conceptId)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(mzk_patientencounterdiagnosis.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, EncounterId);
            query.Criteria.AddCondition("mzk_diagnosisconceptid", ConditionOperator.Equal, conceptId);
            query.Criteria.AddCondition("mzk_status", ConditionOperator.Equal, (int)mzk_patientencounterdiagnosismzk_Status.Active);

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                return false;
            else
                return true;
        }

        /*public async Task<string> AddUserFavourite(Concept concept)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                xrm.mzk_userfavourite userFavouriteEntity = new xrm.mzk_userfavourite();

                QueryExpression query = new QueryExpression(mzk_userfavourite.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);


                if (!string.IsNullOrEmpty(concept.UserId))
                {
                    query.Criteria.AddCondition("mzk_userid", ConditionOperator.Equal, new Guid(concept.UserId));
                    query.Criteria.AddCondition("mzk_conceptid", ConditionOperator.Equal, new Guid(concept.conceptId));
                }

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                if (entitycollection.Entities.Count > 1)
                {
                    throw new ValidationException("Favourite Already exist");
                }

                if (string.IsNullOrEmpty(FavouriteId))
                {

                    if (!string.IsNullOrEmpty(concept.conceptId))
                        userFavouriteEntity.Attributes["mzk_conceptid"] = new Microsoft.Xrm.Sdk.EntityReference("mzk_concept", new Guid(concept.conceptId));
                    if (!string.IsNullOrEmpty(concept.UserId))
                        userFavouriteEntity.mzk_UserId = new Microsoft.Xrm.Sdk.EntityReference("systemuser", new Guid(concept.UserId));
                    if (!string.IsNullOrEmpty(concept.ConceptType))
                        userFavouriteEntity.mzk_ProductType = new Microsoft.Xrm.Sdk.OptionSetValue(Convert.ToInt32(concept.ConceptType));

                    FavouriteId = Convert.ToString(entityRepository.CreateEntity(userFavouriteEntity));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return FavouriteId.ToString();
        }

        public async Task<bool> removeUserFavourite(Concept concept)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            mzk_userfavourite userFavouriteEntity = new mzk_userfavourite();

            try
            {
                QueryExpression query = new QueryExpression(mzk_userfavourite.EntityLogicalName);
                query.ColumnSet = new ColumnSet(true);

                if (!string.IsNullOrEmpty(concept.FavouriteId))
                {
                    query.Criteria.AddCondition("mzk_userfavouriteid", ConditionOperator.Equal, new Guid(concept.FavouriteId));
                }

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                if (entitycollection.Entities.Count < 1)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(concept.FavouriteId) && (!concept.FavouriteId.Equals("null")))
                    entityRepository.DeleteEntity(mzk_userfavourite.EntityLogicalName, new Guid(concept.FavouriteId));
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        */
        public string addConcept(Concept concept)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            mzk_concept entity = new mzk_concept();
            entity.mzk_Category = new OptionSetValue(Convert.ToInt32(concept.ConceptType));
            entity.mzk_ConceptName = concept.name;
            entity.mzk_ConceptNumber = concept.ConceptNumber;
            return entityRepository.CreateEntity(entity).ToString();
        }
    }
}
