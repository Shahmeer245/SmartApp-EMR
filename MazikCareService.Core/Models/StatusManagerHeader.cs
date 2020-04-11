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
    public class StatusManagerHeader
    {
        public string name
        {
            get; set;
        }

        public string Id
        {
            get; set;
        }

        public int caseType
        {
            get; set;
        }

        public string caseTypeText
        {
            get; set;
        }

        public string entityTypeText
        {
            get; set;
        }

        public int entityType
        {
            get; set;
        }

        public DateTime effectiveFrom
        {
            get; set;
        }

        public DateTime effectiveTo
        {
            get; set;
        }       

        public List<StatusManager> details
        {
            get; set;
        }

     /*   public async Task<bool> saveStatusManager(StatusManagerHeader statusManagerHeader)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                mzk_statusmanager statusManager = new mzk_statusmanager();

                statusManager.mzk_CaseType = new OptionSetValue(statusManagerHeader.caseType);
                statusManager.mzk_EntityType = new OptionSetValue(statusManagerHeader.entityType);
                statusManager.mzk_EffectiveFrom = statusManagerHeader.effectiveFrom;
                statusManager.mzk_EffectiveTo = statusManagerHeader.effectiveTo;
                statusManager.mzk_name = statusManagerHeader.name;

                //Guid statusManagerGUID = repo.CreateEntity(statusManager);


                //if (statusManagerGUID != null && statusManagerGUID != Guid.Empty)
                {
                    List<StatusManager> details = statusManagerHeader.details;
                    Dictionary<string, EntityCollection> detailsDict = new Dictionary<string, EntityCollection>();
                    Dictionary<string, mzk_statusmanagerdetails> detailsObjDict = new Dictionary<string, mzk_statusmanagerdetails>();
                    EntityCollection entityCollection = new EntityCollection();
                    Microsoft.Xrm.Sdk.Relationship relationship = null;

                    foreach (StatusManager modelDetails in details)
                    {
                        mzk_statusmanagerdetails statusmanagerdetails = new mzk_statusmanagerdetails();

                        //statusmanagerdetails.mzk_name = modelSection.Name;

                        //statusmanagerdetails.mzk_StatusManagerId = new EntityReference(mzk_statusmanager.EntityLogicalName, statusManagerGUID);

                        statusmanagerdetails.mzk_SendORM = modelDetails.sendOrm;
                        statusmanagerdetails.mzk_ShowEndTime = modelDetails.showEndTime;
                        statusmanagerdetails.mzk_showflip = modelDetails.isFlip;
                        statusmanagerdetails.mzk_ShowStartTime = modelDetails.showStartTime;
                        statusmanagerdetails.mzk_ShowTimer = modelDetails.showTimer;
                        statusmanagerdetails.mzk_AllowEdit = modelDetails.allowEdit;
                        statusmanagerdetails.mzk_ActionManagerId = new EntityReference(mzk_actionmanager.EntityLogicalName, new Guid(modelDetails.ActionManagerId));
                        statusmanagerdetails.mzk_CreateCharge = modelDetails.createCharge;
                        statusmanagerdetails.mzk_FlipType = new OptionSetValue(modelDetails.FlipType);
                        statusmanagerdetails.mzk_FulfillmentAction = modelDetails.fulfillmentAction;
                        statusmanagerdetails.mzk_Undo = modelDetails.Undo;
                        statusmanagerdetails.mzk_OrderStatus = new OptionSetValue(modelDetails.status);

                        EntityCollection parentDetail = null;

                        if (!string.IsNullOrEmpty(modelDetails.ParentStatusId) && detailsDict.TryGetValue(modelDetails.ParentStatusId, out parentDetail))
                        {
                            //statusmanagerdetails.mzk_ParentStatusId = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, parentDetailGUID);

                            if (parentDetail == null)
                            {
                                parentDetail = new EntityCollection();
                            }

                            parentDetail.Entities.Add(statusmanagerdetails);

                            detailsDict[modelDetails.ParentStatusId] = parentDetail;
                        }

                        //Guid detailGUID = repo.CreateEntity(statusmanagerdetails);

                        detailsDict.Add(modelDetails.StatusId, null);
                        detailsObjDict.Add(modelDetails.StatusId, statusmanagerdetails);

                        entityCollection.Entities.Add(statusmanagerdetails);
                    }

                    foreach (KeyValuePair<string, EntityCollection> dict in detailsDict)
                    {
                        if (dict.Value != null && dict.Value.Entities.Count > 0)
                        {
                            mzk_statusmanagerdetails statusmanagerdetailsObj = null;

                            if (detailsObjDict.TryGetValue(dict.Key, out statusmanagerdetailsObj))
                            {
                                relationship = new Microsoft.Xrm.Sdk.Relationship("mzk_mzk_statusmanagerdetails_mzk_statusmanage");

                                relationship.PrimaryEntityRole = EntityRole.Referenced;

                                statusmanagerdetailsObj.RelatedEntities.Add(relationship, dict.Value);
                            }
                        }
                    }

                    relationship = new Microsoft.Xrm.Sdk.Relationship("mzk_mzk_statusmanager_mzk_statusmanagerdetail");

                    statusManager.RelatedEntities.Add(relationship, entityCollection);

                    repo.CreateEntity(statusManager);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }*/

        public async Task<bool> saveStatusManager(StatusManagerHeader statusManagerHeader)
        {
            Guid statusManagerGUID = Guid.Empty;
            SoapEntityRepository repo = SoapEntityRepository.GetService();

            try
            { 
                mzk_statusmanager statusManager = new mzk_statusmanager();

                if (string.IsNullOrEmpty(statusManagerHeader.name))
                {
                    throw new ValidationException("Name must be filled in");
                }

                if (statusManagerHeader.caseType == 0)
                {
                    throw new ValidationException("Case type must be filled in");
                }                

                if (statusManagerHeader.entityType == 0)
                {
                    throw new ValidationException("Entity type must be filled in");
                }

                if (statusManagerHeader.effectiveFrom.Date < DateTime.Now.Date)
                {
                    throw new ValidationException("Effective from must be greater or equal than today's date");
                }

                if (statusManagerHeader.effectiveTo.Date < DateTime.Now.Date)
                {
                    throw new ValidationException("Effective to must be greater or equal than today's date");
                }

                if (statusManagerHeader.effectiveTo.Date < statusManagerHeader.effectiveFrom.Date)
                {
                    throw new ValidationException("Effective to must be greater or equal than Effective from");
                }

                if (this.checkExists(statusManagerHeader))
                {
                    throw new ValidationException("Status manager for the selected entity and case type already exist for the selected date range");
                }

                statusManager.mzk_CaseType = new OptionSetValue(statusManagerHeader.caseType);
                statusManager.mzk_EntityType = new OptionSetValue(statusManagerHeader.entityType);
                statusManager.mzk_EffectiveFrom = statusManagerHeader.effectiveFrom;
                statusManager.mzk_EffectiveTo = statusManagerHeader.effectiveTo;
                statusManager.mzk_name = statusManagerHeader.name;

                statusManagerGUID = repo.CreateEntity(statusManager);

                if (statusManagerGUID != null && statusManagerGUID != Guid.Empty)
                {
                    List<StatusManager> details = statusManagerHeader.details;
                    Dictionary<string, Guid> detailsDict = new Dictionary<string, Guid>();

                    foreach (StatusManager modelDetails in details)
                    {
                        mzk_statusmanagerdetails statusmanagerdetails = new mzk_statusmanagerdetails();

                        if (string.IsNullOrEmpty(modelDetails.ActionManagerId))
                        {
                            throw new ValidationException("Action manager must be filled in");
                        }

                        //statusmanagerdetails.mzk_name = modelSection.Name;

                        statusmanagerdetails.mzk_StatusManagerId = new EntityReference(mzk_statusmanager.EntityLogicalName, statusManagerGUID);

                        Guid parentDetailGUID = Guid.Empty;

                        if (!string.IsNullOrEmpty(modelDetails.ParentStatusId) && detailsDict.TryGetValue(modelDetails.ParentStatusId, out parentDetailGUID))
                        {
                            statusmanagerdetails.mzk_ParentStatusId = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, parentDetailGUID);
                        }

                        statusmanagerdetails.mzk_SendORM = modelDetails.sendOrm;
                        statusmanagerdetails.mzk_ShowEndTime = modelDetails.showEndTime;
                        statusmanagerdetails.mzk_showflip = modelDetails.isFlip;
                        statusmanagerdetails.mzk_ShowStartTime = modelDetails.showStartTime;
                        statusmanagerdetails.mzk_ShowTimer = modelDetails.showTimer;
                        statusmanagerdetails.mzk_AllowEdit = modelDetails.allowEdit;
                        statusmanagerdetails.mzk_ActionManagerId = new EntityReference(mzk_actionmanager.EntityLogicalName, new Guid(modelDetails.ActionManagerId));
                        statusmanagerdetails.mzk_CreateCharge = modelDetails.createCharge;

                        if (modelDetails.isFlip)
                        {
                            statusmanagerdetails.mzk_FlipType = new OptionSetValue(modelDetails.FlipType);
                        }

                        statusmanagerdetails.mzk_FulfillmentAction = modelDetails.fulfillmentAction;
                        statusmanagerdetails.mzk_Undo = modelDetails.Undo;
                        statusmanagerdetails.mzk_OrderStatus = new OptionSetValue(modelDetails.status);

                        Guid detailGUID = repo.CreateEntity(statusmanagerdetails);

                        detailsDict.Add(modelDetails.StatusId, detailGUID);                        
                    }
                }
            }
            catch (Exception ex)
            {
                if (statusManagerGUID != null && statusManagerGUID != Guid.Empty)
                {
                    repo.DeleteEntity(mzk_statusmanager.EntityLogicalName, statusManagerGUID);
                }

                throw ex;
            }

            return true;
        }

        public bool checkExists(StatusManagerHeader statusManagerHeader)
        {
            try
            {
                QueryExpression query = new QueryExpression(mzk_statusmanager.EntityLogicalName);

                query.Criteria.AddCondition("mzk_entitytype", ConditionOperator.Equal, statusManagerHeader.entityType);
                query.Criteria.AddCondition("mzk_casetype", ConditionOperator.Equal, statusManagerHeader.caseType);
                query.Criteria.AddCondition("mzk_effectivefrom", ConditionOperator.LessEqual, statusManagerHeader.effectiveFrom.ToShortDateString());
                query.Criteria.AddCondition("mzk_effectiveto", ConditionOperator.GreaterEqual, statusManagerHeader.effectiveTo.ToShortDateString());
                
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(query);                               

                if (entitycol != null && entitycol.Entities != null && entitycol.Entities.Count > 0)
                {
                    return true;
                }                

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<StatusManagerHeader>> getStatusManagerList()
        {
            try
            {                
                QueryExpression query = new QueryExpression(mzk_statusmanager.EntityLogicalName);
                query.ColumnSet = new ColumnSet(true);

                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(query);

                List<StatusManagerHeader> list = new List<StatusManagerHeader>();

                foreach (mzk_statusmanager statusManager in entitycol.Entities)
                {
                    list.Add(this.fillStatusManagerHeader(statusManager));
                }                

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<StatusManagerHeader> getStatusManagerDetails(string statusManagerId)
        {
            try
            {
                QueryExpression query = new QueryExpression(mzk_statusmanager.EntityLogicalName);
                query.ColumnSet = new ColumnSet(true);

                query.Criteria.AddCondition("mzk_statusmanagerid", ConditionOperator.Equal, new Guid(statusManagerId));

                LinkEntity entityTypeDetails = new LinkEntity(mzk_statusmanager.EntityLogicalName, mzk_statusmanagerdetails.EntityLogicalName, "mzk_statusmanagerid", "mzk_statusmanagerid", JoinOperator.Inner);
                entityTypeDetails.EntityAlias = "mzk_statusmanagerdetails";
                entityTypeDetails.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                LinkEntity entityTypeAction = new LinkEntity(mzk_statusmanagerdetails.EntityLogicalName, mzk_actionmanager.EntityLogicalName, "mzk_actionmanagerid", "mzk_actionmanagerid", JoinOperator.Inner);
                entityTypeAction.EntityAlias = "mzk_actionmanager";
                entityTypeAction.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                entityTypeDetails.LinkEntities.Add(entityTypeAction);

                query.LinkEntities.Add(entityTypeDetails);

                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(query);

                StatusManagerHeader model = new StatusManagerHeader();
                StatusManager modelDetails = null;
                List<StatusManager> list = new List<StatusManager>();

                bool isFirst = true;

                foreach (Entity entity in entitycol.Entities)
                {
                    if (isFirst)
                    {
                        isFirst = false;

                        mzk_statusmanager statusManager = (mzk_statusmanager)entity;

                        model =  this.fillStatusManagerHeader(statusManager);
                    }

                    modelDetails = new StatusManager();

                    if (entity.Attributes.Contains("mzk_actionmanager.mzk_actionname"))
                    {
                        modelDetails.name = ((AliasedValue)entity["mzk_actionmanager.mzk_actionname"]).Value.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_actionmanagerid"))
                    {
                        modelDetails.ActionManagerId = ((EntityReference)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_actionmanagerid"]).Value).Id.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_allowedit"))
                    {
                        modelDetails.allowEdit = (bool)(entity["mzk_statusmanagerdetails.mzk_allowedit"] as AliasedValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_fulfillmentaction"))
                    {
                        modelDetails.fulfillmentAction = (bool)(entity["mzk_statusmanagerdetails.mzk_fulfillmentaction"] as AliasedValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_showendtime"))
                    {
                        modelDetails.showEndTime = (bool)(entity["mzk_statusmanagerdetails.mzk_showendtime"] as AliasedValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_showstarttime"))
                    {
                        modelDetails.showStartTime = (bool)(entity["mzk_statusmanagerdetails.mzk_showstarttime"] as AliasedValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_undo"))
                    {
                        modelDetails.Undo = (bool)(entity["mzk_statusmanagerdetails.mzk_undo"] as AliasedValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_showtimer"))
                    {
                        modelDetails.showTimer = (bool)(entity["mzk_statusmanagerdetails.mzk_showtimer"] as AliasedValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_orderstatus"))
                    {
                        modelDetails.status = (((entity["mzk_statusmanagerdetails.mzk_orderstatus"] as AliasedValue).Value) as OptionSetValue).Value;
                        modelDetails.statusText = entity.FormattedValues["mzk_statusmanagerdetails.mzk_orderstatus"].ToString();
                    }

                    //  if (entity.Attributes.Contains("mzk_statusmanagerdetailsParentStatus.mzk_orderstatus"))
                    // {
                    //model.parentStatus = (((AliasedValue)entity["mzk_statusmanagerdetailsParentStatus.mzk_orderstatus"]).Value as OptionSetValue).Value;
                    //}

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_statusmanagerdetailsid"))
                    {
                        modelDetails.StatusId = (entity["mzk_statusmanagerdetails.mzk_statusmanagerdetailsid"] as AliasedValue).Value.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_parentstatusid"))
                    {
                       modelDetails.ParentStatusId = ((EntityReference)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_parentstatusid"]).Value).Id.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_showflip"))
                    {
                        modelDetails.isFlip = (bool)(entity["mzk_statusmanagerdetails.mzk_showflip"] as AliasedValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_sendorm"))
                    {
                        modelDetails.sendOrm = (bool)(entity["mzk_statusmanagerdetails.mzk_sendorm"] as AliasedValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_createcharge"))
                    {
                        modelDetails.createCharge = (bool)(entity["mzk_statusmanagerdetails.mzk_createcharge"] as AliasedValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_fliptype"))
                    {
                        modelDetails.FlipType = (((entity["mzk_statusmanagerdetails.mzk_fliptype"] as AliasedValue).Value) as OptionSetValue).Value;
                        modelDetails.flipTypeText = entity.FormattedValues["mzk_statusmanagerdetails.mzk_fliptype"].ToString();
                    }

                    list.Add(modelDetails);
                }

                model.details = list;

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public StatusManagerHeader fillStatusManagerHeader(mzk_statusmanager statusManager)
        {
            try
            {
                StatusManagerHeader model = new StatusManagerHeader();

                model.caseType = statusManager.mzk_CaseType.Value;
                model.caseTypeText = statusManager.FormattedValues["mzk_casetype"].ToString();
                model.entityType = statusManager.mzk_EntityType.Value;
                model.entityTypeText = statusManager.FormattedValues["mzk_entitytype"].ToString();

                if (statusManager.mzk_EffectiveFrom.HasValue)
                {
                    model.effectiveFrom = statusManager.mzk_EffectiveFrom.Value;
                }

                if (statusManager.mzk_EffectiveTo.HasValue)
                {
                    model.effectiveTo = statusManager.mzk_EffectiveTo.Value;
                }

                model.name = statusManager.mzk_name;
                model.Id = statusManager.mzk_statusmanagerId.Value.ToString();
                
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
