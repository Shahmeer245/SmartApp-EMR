using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;
using System.Reflection;
using MazikLogger;

namespace MazikCareService.Core.Models
{
    public class StatusManager
    {
        List<StatusManager> listModel = new List<StatusManager>();

        string statusManagerDetailsIdMain;


        public string name
        {
            get; set;
        }
        public string StatusId
        {
            get; set;
        }

        public string statusText
        {
            get; set;
        }
        public string flipTypeText
        {
            get; set;
        }
        public string ParentStatusId
        {
            get; set;
        }

        public string RevertStatusId
        {
            get; set;
        }

        public string ActionManagerId
        {
            get; set;
        }

        public bool isFlip
        {
            get; set;
        }

        public bool Undo
        {
            get; set;
        }

        public int FlipType
        {
            get; set;
        }

        public int status
        {
            get; set;
        }

        public int revertStatus
        {
            get; set;
        }
        public bool allowEdit
        {
            get; set;
        }
        public bool fulfillmentAction
        {
            get; set;
        }
        public bool showEndTime
        {
            get; set;
        }
        public bool showStartTime
        {
            get; set;
        }
        public bool showTimer
        {
            get; set;
        }

        public bool createCharge
        {
            get; set;
        }
        public bool sendOrm
        {
            get; set;
        }

        public StatusManager()
        {
           
        }

        public StatusManager(string _statusManagerDetailsId)
        {
            statusManagerDetailsIdMain = _statusManagerDetailsId;
        }

        public bool executeAction(string statusManagerDetailsId, string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {  
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(mzk_statusmanagerdetails.EntityLogicalName);

                query.Criteria.AddCondition("mzk_statusmanagerdetailsid", ConditionOperator.Equal, new Guid(statusManagerDetailsId));
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                
                LinkEntity entityTypeAction = new LinkEntity(mzk_statusmanagerdetails.EntityLogicalName, mzk_actionmanager.EntityLogicalName, "mzk_actionmanagerid", "mzk_actionmanagerid", JoinOperator.Inner);
                entityTypeAction.EntityAlias = "mzk_actionmanager";
                entityTypeAction.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                query.LinkEntities.Add(entityTypeAction);

                EntityCollection entitycol = repo.GetEntityCollection(query);

                if (entitycol != null && entitycol.Entities != null && entitycol.Entities.Count > 0)
                {
                    string className = string.Empty;
                    string operationName = string.Empty;

                    Entity entity = entitycol.Entities[0];

                    if (entity.Attributes.Contains("mzk_actionmanager.mzk_actionclass"))
                    {
                        className = ((AliasedValue)entity["mzk_actionmanager.mzk_actionclass"]).Value.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_actionmanager.mzk_actionoperation"))
                    {
                        operationName = ((AliasedValue)entity["mzk_actionmanager.mzk_actionoperation"]).Value.ToString();
                    }

                    if (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(operationName))
                    { 
                        Type type = Type.GetType("MazikCareService.Core.Models." + className);

                        if (type != null)
                        {
                            MethodInfo methodInfo = type.GetMethod(operationName);

                            if (methodInfo != null)
                            {
                                dynamic result;
                                
                                object classInstance = Activator.CreateInstance(type, null);

                                object[] objParms = new object[3];
                                objParms[0] = orderId;
                                objParms[1] = encounterId;
                                objParms[2] = paramValues;
                                                                
                                try
                                {
                                    result = methodInfo.Invoke(classInstance, objParms);
                                }
                                catch (TargetInvocationException ex)
                                {
                                    throw ex.InnerException;
                                }

                                return result;
                            }
                        }                            
                    }
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        public List<StatusManager> getHierarchyByType(mzk_entitytype _entityType, mzk_casetype _caseType)
        {

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(mzk_statusmanager.EntityLogicalName);

                query.Criteria.AddCondition("mzk_entitytype", ConditionOperator.Equal, (int)_entityType);
                query.Criteria.AddCondition("mzk_casetype", ConditionOperator.Equal, (int)_caseType);
                query.Criteria.AddCondition("mzk_effectivefrom", ConditionOperator.LessEqual, DateTime.Now.ToShortDateString());
                query.Criteria.AddCondition("mzk_effectiveto", ConditionOperator.GreaterEqual, DateTime.Now.ToShortDateString());

                LinkEntity entityTypeDetails = new LinkEntity(mzk_statusmanager.EntityLogicalName, mzk_statusmanagerdetails.EntityLogicalName, "mzk_statusmanagerid", "mzk_statusmanagerid", JoinOperator.Inner);
                entityTypeDetails.EntityAlias = "mzk_statusmanagerdetails";
                entityTypeDetails.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                query.LinkEntities.Add(entityTypeDetails);

                LinkEntity entityTypeAction = new LinkEntity(mzk_statusmanagerdetails.EntityLogicalName, mzk_actionmanager.EntityLogicalName, "mzk_actionmanagerid", "mzk_actionmanagerid", JoinOperator.Inner);
                entityTypeAction.EntityAlias = "mzk_actionmanager";
                entityTypeAction.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                entityTypeDetails.LinkEntities.Add(entityTypeAction);

                LinkEntity entityTypeRevert = new LinkEntity(mzk_statusmanagerdetails.EntityLogicalName, mzk_statusmanagerdetails.EntityLogicalName, "mzk_revertstatus", "mzk_statusmanagerdetailsid", JoinOperator.LeftOuter);
                entityTypeRevert.EntityAlias = "mzk_statusmanagerdetailsRevertStatus";
                entityTypeRevert.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_orderstatus");

                entityTypeDetails.LinkEntities.Add(entityTypeRevert);

                EntityCollection entitycol = repo.GetEntityCollection(query);

                StatusManager model;
                List<StatusManager> listModelType = new List<StatusManager>();

                foreach (Entity entity in entitycol.Entities)
                {
                    model = new StatusManager();

                    if (entity.Attributes.Contains("mzk_actionmanager.mzk_actionname"))
                    {
                        model.name = ((AliasedValue)entity["mzk_actionmanager.mzk_actionname"]).Value.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_allowedit"))
                    {
                        model.allowEdit = (bool)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_allowedit"]).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_fulfillmentaction"))
                    {
                        model.fulfillmentAction = (bool)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_fulfillmentaction"]).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_showendtime"))
                    {
                        model.showEndTime = (bool)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_showendtime"]).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_showstarttime"))
                    {
                        model.showStartTime = (bool)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_showstarttime"]).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_undo"))
                    {
                        model.Undo = (bool)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_undo"]).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_showtimer"))
                    {
                        model.showTimer = (bool)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_showtimer"]).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_orderstatus"))
                    {
                        model.status = (((AliasedValue)(entity["mzk_statusmanagerdetails.mzk_orderstatus"])).Value as OptionSetValue).Value;
                    }

                    //  if (entity.Attributes.Contains("mzk_statusmanagerdetailsParentStatus.mzk_orderstatus"))
                    // {
                    //model.parentStatus = (((AliasedValue)entity["mzk_statusmanagerdetailsParentStatus.mzk_orderstatus"]).Value as OptionSetValue).Value;
                    //}

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_statusmanagerdetailsid"))
                    {
                        model.StatusId = ((AliasedValue)entity["mzk_statusmanagerdetails.mzk_statusmanagerdetailsid"]).Value.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_parentstatusid"))
                    {
                        model.ParentStatusId = ((EntityReference)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_parentstatusid"]).Value).Id.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_revertstatus"))
                    {
                        model.RevertStatusId = ((EntityReference)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_revertstatus"]).Value).Id.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetailsRevertStatus.mzk_orderstatus"))
                    {
                        model.revertStatus = (((AliasedValue)entity["mzk_statusmanagerdetailsRevertStatus.mzk_orderstatus"]).Value as OptionSetValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_showflip"))
                    {
                        model.isFlip = (bool)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_showflip"]).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_sendorm"))
                    {
                        model.sendOrm = (bool)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_sendorm"]).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_createcharge"))
                    {
                        model.createCharge = (bool)((AliasedValue)entity["mzk_statusmanagerdetails.mzk_createcharge"]).Value;
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetails.mzk_fliptype"))
                    {
                        model.FlipType = (((AliasedValue)(entity["mzk_statusmanagerdetails.mzk_fliptype"])).Value as OptionSetValue).Value;
                    }

                    listModelType.Add(model);
                }

                return listModelType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool filterHierarchyByType(List<StatusManager> listStatusManager)
        {

            try
            {
                listModel = listStatusManager.Where(item => item.ParentStatusId == statusManagerDetailsIdMain || item.StatusId == statusManagerDetailsIdMain).ToList();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool getHierarchy()
        {           

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();                

                EntityCollection entitycol = repo.GetEntityCollection(this.getActionsQuery());

                StatusManager model;

                foreach (Entity entity in entitycol.Entities)
                {
                    model = new StatusManager();                   

                    if (entity.Attributes.Contains("mzk_actionmanager.mzk_actionname"))
                    {
                        model.name = ((AliasedValue)entity["mzk_actionmanager.mzk_actionname"]).Value.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_allowedit"))
                    {
                        model.allowEdit = (bool)(entity["mzk_allowedit"]);
                    }

                    if (entity.Attributes.Contains("mzk_fulfillmentaction"))
                    {
                        model.fulfillmentAction = (bool)(entity["mzk_fulfillmentaction"]);
                    }

                    if (entity.Attributes.Contains("mzk_showendtime"))
                    {
                        model.showEndTime = (bool)(entity["mzk_showendtime"]);
                    }

                    if (entity.Attributes.Contains("mzk_showstarttime"))
                    {
                        model.showStartTime = (bool)(entity["mzk_showstarttime"]);
                    }

                    if (entity.Attributes.Contains("mzk_undo"))
                    {
                        model.Undo = (bool)(entity["mzk_undo"]);
                    }

                    if (entity.Attributes.Contains("mzk_showtimer"))
                    {
                        model.showTimer = (bool)(entity["mzk_showtimer"]);
                    }

                    if (entity.Attributes.Contains("mzk_orderstatus"))
                    {
                        model.status = ((entity["mzk_orderstatus"]) as OptionSetValue).Value;
                    }

                  //  if (entity.Attributes.Contains("mzk_statusmanagerdetailsParentStatus.mzk_orderstatus"))
                   // {
                        //model.parentStatus = (((AliasedValue)entity["mzk_statusmanagerdetailsParentStatus.mzk_orderstatus"]).Value as OptionSetValue).Value;
                    //}

                    if (entity.Attributes.Contains("mzk_statusmanagerdetailsid"))
                    {
                        model.StatusId = (entity["mzk_statusmanagerdetailsid"]).ToString();
                    }

                    if (entity.Attributes.Contains("mzk_parentstatusid"))
                    {
                        model.ParentStatusId = ((EntityReference)entity["mzk_parentstatusid"]).Id.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_revertstatus"))
                    {
                        model.RevertStatusId = ((EntityReference)entity["mzk_revertstatus"]).Id.ToString();
                    }

                    if (entity.Attributes.Contains("mzk_statusmanagerdetailsRevertStatus.mzk_orderstatus"))
                    {
                        model.revertStatus = (((AliasedValue)entity["mzk_statusmanagerdetailsRevertStatus.mzk_orderstatus"]).Value as OptionSetValue).Value;
                    }

                    if (entity.Attributes.Contains("mzk_showflip"))
                    {
                        model.isFlip = (bool)(entity["mzk_showflip"]);
                    }

                    if (entity.Attributes.Contains("mzk_sendorm"))
                    {
                        model.sendOrm = (bool)(entity["mzk_sendorm"]);
                    }

                    if (entity.Attributes.Contains("mzk_createcharge"))
                    {
                        model.createCharge = (bool)(entity["mzk_createcharge"]);
                    }

                    if (entity.Attributes.Contains("mzk_fliptype"))
                    {
                        model.FlipType = ((entity["mzk_fliptype"]) as OptionSetValue).Value;
                    }

                    listModel.Add(model);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public List<ActionManager> getNextActionList()
        {
            try
            {
                List<ActionManager> listModelAction = new List<ActionManager>();
                List<StatusManager> listModelNextActions = new List<StatusManager>();

                listModelNextActions = listModel.Where(status => status.StatusId != statusManagerDetailsIdMain && status.fulfillmentAction == true).ToList();
                ActionManager model;

                foreach (StatusManager statusManger in listModelNextActions)
                {
                    model = new ActionManager();

                    model.FlipType = statusManger.FlipType;
                    model.isFlip = statusManger.isFlip;
                    model.StatusId = statusManger.StatusId;
                    model.name = statusManger.name;

                    listModelAction.Add(model);
                }

                return listModelAction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool allowCancelled()
        {
            try
            {
                List<StatusManager> listModelStatus = listModel.Where(item => item.status == (int)mzk_orderstatus.Cancelled && item.StatusId != statusManagerDetailsIdMain).ToList();

                return (listModelStatus != null && listModelStatus.Count >= 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool allowOrdered()
        {
            try
            {
                List<StatusManager> listModelStatus = listModel.Where(item => item.status == (int)mzk_orderstatus.Ordered && item.StatusId != statusManagerDetailsIdMain).ToList();

                return (listModelStatus != null && listModelStatus.Count >= 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool showTimerCheck()
        {
            try
            {
                List<StatusManager> listModelStatus = listModel.Where(item => item.StatusId == statusManagerDetailsIdMain && item.showTimer == true).ToList();

                return (listModelStatus != null && listModelStatus.Count >= 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool isFulfilmentAction()
        {
            try
            {
                List<StatusManager> listModelStatus = listModel.Where(item => item.StatusId == statusManagerDetailsIdMain && item.fulfillmentAction == true).ToList();

                return (listModelStatus != null && listModelStatus.Count >= 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool showStartTimeDetails()
        {
            try
            {
                List<StatusManager> listModelStatus = listModel.Where(item => item.StatusId == statusManagerDetailsIdMain && item.showStartTime == true).ToList();

                return (listModelStatus != null && listModelStatus.Count >= 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int getStartTimerStatus()
        {
            try
            {  
                StatusManager modelStatus = listModel.Find(item => item.StatusId == statusManagerDetailsIdMain);
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                mzk_statusmanagerdetails statusmanagerdetails = null;

                statusmanagerdetails = (mzk_statusmanagerdetails)repo.GetEntity(mzk_statusmanagerdetails.EntityLogicalName, new Guid(modelStatus.ParentStatusId), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                if (statusmanagerdetails != null)
                {
                    while (statusmanagerdetails.mzk_statusmanagerdetailsId.HasValue)
                    {
                        if (statusmanagerdetails.mzk_ShowStartTime.HasValue && !statusmanagerdetails.mzk_ShowStartTime.Value)
                        {
                            statusmanagerdetails = (mzk_statusmanagerdetails)repo.GetEntity(mzk_statusmanagerdetails.EntityLogicalName, statusmanagerdetails.mzk_ParentStatusId.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                        }
                        else
                        {
                            break;
                        }
                    }

                    return statusmanagerdetails.mzk_OrderStatus.Value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool showEndTimeDetails()
        {
            try
            {
                List<StatusManager> listModelStatus = listModel.Where(item => item.StatusId == statusManagerDetailsIdMain && item.showEndTime == true).ToList();

                return (listModelStatus != null && listModelStatus.Count >= 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool allowEditing()
        {
            try
            {
                List<StatusManager> listModelStatus = listModel.Where(item => item.StatusId == statusManagerDetailsIdMain && item.allowEdit == true).ToList();

                return (listModelStatus != null && listModelStatus.Count == 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private QueryExpression getActionsQuery()
        {
            QueryExpression query = new QueryExpression(mzk_statusmanagerdetails.EntityLogicalName);

            FilterExpression filterExpression = query.Criteria.AddFilter(LogicalOperator.Or);
            filterExpression.AddCondition("mzk_parentstatusid", ConditionOperator.Equal, new Guid(statusManagerDetailsIdMain));
            filterExpression.AddCondition("mzk_statusmanagerdetailsid", ConditionOperator.Equal, new Guid(statusManagerDetailsIdMain));

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            
            LinkEntity entityTypeAction = new LinkEntity(mzk_statusmanagerdetails.EntityLogicalName, mzk_actionmanager.EntityLogicalName, "mzk_actionmanagerid", "mzk_actionmanagerid", JoinOperator.Inner);
            entityTypeAction.EntityAlias = "mzk_actionmanager";
            entityTypeAction.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            query.LinkEntities.Add(entityTypeAction);

            LinkEntity entityTypeRevert = new LinkEntity(mzk_statusmanagerdetails.EntityLogicalName, mzk_statusmanagerdetails.EntityLogicalName, "mzk_revertstatus", "mzk_statusmanagerdetailsid", JoinOperator.LeftOuter);
            entityTypeRevert.EntityAlias = "mzk_statusmanagerdetailsRevertStatus";
            entityTypeRevert.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_orderstatus");

            query.LinkEntities.Add(entityTypeRevert);

            return query;
        }

        public static StatusManager isValidAction(mzk_orderstatus nextStatus, string statusManagerDetailsId)
        {
            try
            {
                StatusManager statusManagerObj = new StatusManager(statusManagerDetailsId);

                if (statusManagerObj.getHierarchy())
                {
                    List<StatusManager> list = statusManagerObj.listModel.Where(item => item.status == (int)nextStatus && item.StatusId != statusManagerDetailsId).ToList();

                    if(list != null && list.Count > 0)
                    {
                        return list.First();
                    }
                    else
                    {
                        return null;  
                    }                                      
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StatusManager getRootStatus(mzk_entitytype entityType, mzk_casetype caseType, bool isActivityOrder = false)
        {
            try
            {
                StatusManager statusManagerObj = new StatusManager();

                QueryExpression query = new QueryExpression(mzk_statusmanager.EntityLogicalName);

                query.Criteria.AddCondition("mzk_entitytype", ConditionOperator.Equal, (int)entityType);
                query.Criteria.AddCondition("mzk_casetype", ConditionOperator.Equal, (int)caseType);
                query.Criteria.AddCondition("mzk_effectivefrom", ConditionOperator.LessEqual, DateTime.Now.ToShortDateString());
                query.Criteria.AddCondition("mzk_effectiveto", ConditionOperator.GreaterEqual, DateTime.Now.ToShortDateString());

                LinkEntity entityTypeDetails = new LinkEntity(mzk_statusmanager.EntityLogicalName, mzk_statusmanagerdetails.EntityLogicalName, "mzk_statusmanagerid", "mzk_statusmanagerid", JoinOperator.Inner);
                entityTypeDetails.EntityAlias = "mzk_statusmanagerdetailsNextStatus";

                if (isActivityOrder == true)
                {
                    entityTypeDetails.LinkCriteria.AddCondition("mzk_startingstatusforactivityorder", ConditionOperator.Equal, true);
                }
                else
                {
                    entityTypeDetails.LinkCriteria.AddCondition("mzk_startingstatusformanualorder", ConditionOperator.Equal, true);
                }
                entityTypeDetails.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                query.LinkEntities.Add(entityTypeDetails);

                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(query);

                StatusManager model = null;

                if(entitycol != null && entitycol.Entities != null && entitycol.Entities.Count > 0)
                {
                        model = new StatusManager();

                        bool activityOrder = (bool)(((AliasedValue)entitycol.Entities[0]["mzk_statusmanagerdetailsNextStatus.mzk_startingstatusforactivityorder"]).Value);

                        bool manualOrder = (bool)(((AliasedValue)entitycol.Entities[0]["mzk_statusmanagerdetailsNextStatus.mzk_startingstatusformanualorder"]).Value);
                    
                        if (entitycol.Entities[0].Attributes.Contains("mzk_statusmanagerdetailsNextStatus.mzk_orderstatus"))
                        {
                            model.status = (((AliasedValue)entitycol.Entities[0]["mzk_statusmanagerdetailsNextStatus.mzk_orderstatus"]).Value as OptionSetValue).Value;
                        }                                              

                        if (entitycol.Entities[0].Attributes.Contains("mzk_statusmanagerdetailsNextStatus.mzk_statusmanagerdetailsid"))
                        {
                            model.StatusId = ((AliasedValue)entitycol.Entities[0]["mzk_statusmanagerdetailsNextStatus.mzk_statusmanagerdetailsid"]).Value.ToString();
                        }

                        if (entitycol.Entities[0].Attributes.Contains("mzk_statusmanagerdetailsNextStatus.mzk_sendorm"))
                        {
                            model.sendOrm = (bool)(((AliasedValue)entitycol.Entities[0]["mzk_statusmanagerdetailsNextStatus.mzk_sendorm"]).Value);
                        }                    
                }
                else
                {
                    throw new ValidationException("Status manager not found. Please contact system administrator");
                }

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StatusManager isUndoValid(string statusManagerDetailsId)
        {
            try
            {
                StatusManager statusManagerObj = new StatusManager(statusManagerDetailsId);
                StatusManager model = null;

                if (statusManagerObj.getHierarchy())
                {
                    List<StatusManager> list = statusManagerObj.listModel.Where(item => item.Undo == true && item.StatusId != statusManagerDetailsId).ToList();

                    if (list != null && list.Count > 0)
                    {
                        model = list.First();    
                    }
                }
                
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static StatusManager getParentStatus(string statusManagerDetailsId)
        {
            try
            {
                StatusManager statusManagerObj = new StatusManager();

                QueryExpression query = new QueryExpression(mzk_statusmanagerdetails.EntityLogicalName);
                query.ColumnSet = new ColumnSet(true);

                query.Criteria.AddCondition("mzk_statusmanagerdetailsid", ConditionOperator.Equal, new Guid(statusManagerDetailsId));

                LinkEntity entityTypeDetails = new LinkEntity(mzk_statusmanagerdetails.EntityLogicalName, mzk_statusmanagerdetails.EntityLogicalName, "mzk_parentstatusid", "mzk_statusmanagerdetailsid", JoinOperator.Inner);
                entityTypeDetails.EntityAlias = "mzk_statusmanagerdetailsParentStatus";
                entityTypeDetails.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                query.LinkEntities.Add(entityTypeDetails);

                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(query);

                StatusManager model = null;

                if (entitycol != null && entitycol.Entities != null && entitycol.Entities.Count > 0)
                {
                    model = new StatusManager();                    

                    if (entitycol.Entities[0].Attributes.Contains("mzk_statusmanagerdetailsParentStatus.mzk_orderstatus"))
                    {
                        model.status = (((AliasedValue)entitycol.Entities[0]["mzk_statusmanagerdetailsParentStatus.mzk_orderstatus"]).Value as OptionSetValue).Value;
                    }

                    if (entitycol.Entities[0].Attributes.Contains("mzk_statusmanagerdetailsParentStatus.mzk_statusmanagerdetailsid"))
                    {
                        model.StatusId = ((AliasedValue)entitycol.Entities[0]["mzk_statusmanagerdetailsParentStatus.mzk_statusmanagerdetailsid"]).Value.ToString();
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*public static StatusManager getNextStatus(string statusManagerDetailsId)
        {
            try
            {
                StatusManager statusManagerObj = new StatusManager();

                QueryExpression query = new QueryExpression(mzk_statusmanagerdetails.EntityLogicalName);

                query.Criteria.AddCondition("mzk_parentstatusid", ConditionOperator.Equal, new Guid(statusManagerDetailsId));
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(query);

                StatusManager model = null;

                if (entitycol != null && entitycol.Entities != null && entitycol.Entities.Count > 0)
                {
                    model = new StatusManager();

                    if (entitycol.Entities[0].Attributes.Contains("mzk_statusmanagerdetailsNextStatus.mzk_orderstatus"))
                    {
                        model.status = (((AliasedValue)entitycol.Entities[0]["mzk_statusmanagerdetailsNextStatus.mzk_orderstatus"]).Value as OptionSetValue).Value;
                    }

                    if (entitycol.Entities[0].Attributes.Contains("mzk_statusmanagerdetailsNextStatus.mzk_statusmanagerdetailsid"))
                    {
                        model.StatusId = ((AliasedValue)entitycol.Entities[0]["mzk_statusmanagerdetailsNextStatus.mzk_statusmanagerdetailsid"]).Value.ToString();
                    }
                }
                else
                {
                    throw new ValidationException("Status manager not found. Please contact system administrator");
                }

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }*/
    }    
}
