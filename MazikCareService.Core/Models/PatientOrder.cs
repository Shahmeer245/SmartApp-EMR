using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Models.HL7;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class PatientOrder
    {
        public string PatientId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime FulfillmentDate { get; set; }
        public string StatusNotes { get; set; }
        public string OrderStatus { get; set; }
        public string StatusManagerDetailsId { get; set; }
        public string CaseTransRecId { get; set; }
        public string OrderStatusText { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Id { get; set; }
        public string CaseId { get; set; }
        public string TestId { get; set; }
        public string ParentOrderId { get; set; }
        public string EncounterId { get; set; }
        public string ManufacturerName { get; set; }
        public string ICDCode { get; set; }

        public User orderingProvider { get; set; }
        public long clinicRecId { get; set; }
        public long productId { get; set; }

        public string product { get; set; }
        public string clinicid { get; set; }
        public List<ActionManager> nextActionList { get; set; }
        public bool allowEdit
        {
            get; set;
        }

        public bool showTimer
        {
            get; set;
        }

        public DateTime startTime
        {
            get; set;
        }

        public DateTime endTime
        {
            get; set;
        }

        public DateTime currentDateTime
        {
            get; set;
        }

        public bool allowCancel
        {
            get; set;
        }
        public bool allowOrder
        {
            get; set;
        }
        public string orderingLocationId
        {
            get; set;
        }
        public string treatmentLocationId
        {
            get; set;
        }
        public string orderingLocation
        {
            get; set;
        }
        public string treatmentLocation
        {
            get; set;
        }
        public string appointmentId
        {
            get; set;
        }
        public int snoozeAllowed
        {
            get;set;
        }
        public int snoozeTimeInMinutes
        {
            get; set;
        }


        public bool updateOrderStatus(PatientOrder patientOrder, string encounterId, StatusManagerParams paramValues, bool updateCaseTrans = false, string caseId = "")
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                mzk_patientorder entity = (mzk_patientorder)entityRepository.GetEntity("mzk_patientorder", new Guid(patientOrder.Id) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientorderid", "mzk_patientencounterid", "mzk_orderstatus", "mzk_type", "mzk_axcasetransrefrecid", "mzk_statusmanagerdetail", "mzk_customer", "mzk_orderingappointment","mzk_fulfillmentappointment"));

                if (entity != null && entity.mzk_patientorderId.HasValue)
                {
                    if (!string.IsNullOrEmpty(patientOrder.OrderStatus))
                    {
                        if (entity.mzk_StatusManagerDetail == null)
                        {
                            throw new ValidationException("Status Manager Id not set. Contact your system administrator");
                        }

                        StatusManager statusManager = StatusManager.isValidAction((mzk_orderstatus)Convert.ToInt32(patientOrder.OrderStatus), entity.mzk_StatusManagerDetail.Id.ToString());

                        if (statusManager != null)
                        {
                            if (AppSettings.GetByKey("RISIntegration").ToLower() == true.ToString().ToLower() && entity.mzk_Type.Value == (int)mzk_patientordermzk_Type.Radiology && statusManager.sendOrm)
                            {                                
                                string encounterIdOrder = "";
                                string appointmentId = string.Empty;

                                if(entity.mzk_fulfillmentappointment != null)
                                {
                                    appointmentId = ((EntityReference)entity.Attributes["mzk_fulfillmentappointment"]).Id.ToString();
                                }

                                if (entity.mzk_PatientEncounterId != null)
                                {
                                    encounterIdOrder = entity.mzk_PatientEncounterId.Id.ToString();
                                }

                                this.sendORMRISMessage(entity.mzk_customer.Id.ToString(), 0, encounterIdOrder, patientOrder.Id, caseId, patientOrder.OrderStatus, appointmentId);
                            }

                            if (AppSettings.GetByKey("LISIntegration").ToLower() == true.ToString().ToLower() && entity.mzk_Type.Value == (int)mzk_patientordermzk_Type.Lab && statusManager.sendOrm)
                            {
                                string appointmentId = string.Empty;

                                if (entity.mzk_fulfillmentappointment != null)
                                {
                                    appointmentId = ((EntityReference)entity.Attributes["mzk_fulfillmentappointment"]).Id.ToString();
                                }

                                string encounterIdOrder = "";

                                if (entity.mzk_PatientEncounterId != null)
                                {
                                    encounterIdOrder = entity.mzk_PatientEncounterId.Id.ToString();
                                }

                                this.sendORMLISMessage(entity.mzk_customer.Id.ToString(), 0, encounterIdOrder, patientOrder.Id, caseId, patientOrder.OrderStatus, appointmentId);
                            }

                            entity.mzk_OrderStatus = new OptionSetValue(Convert.ToInt32(patientOrder.OrderStatus));
                            entity.mzk_StatusManagerDetail = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, new Guid(statusManager.StatusId));

                            if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                            {
                                if (entity.mzk_AXCaseTransRefRecId.HasValue && !this.updateCaseTrans(Convert.ToInt64(entity.mzk_AXCaseTransRefRecId.Value), (mzk_orderstatus)Convert.ToInt32(patientOrder.OrderStatus), statusManager.createCharge, paramValues))
                                {
                                    throw new ValidationException("Error updating case trans. Contact your system administrator");
                                }
                            }                           

                            entityRepository.UpdateEntity(entity);

                            this.createOrderStatusLog(Convert.ToInt32(patientOrder.OrderStatus), patientOrder.Id, encounterId, paramValues);

                            return true;
                        }
                        else
                        {
                            throw new ValidationException("Cannot change order status. Invalid Action performed. System is not configured to change the current status to " + ((mzk_orderstatus)Convert.ToInt32(patientOrder.OrderStatus)).GetDescription());
                        }
                    }
                    else
                    {
                        throw new ValidationException("Order status cannot be empty");
                    }
                }
                else
                {
                    throw new ValidationException("Unable to find patient order");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static mzk_entitytype convertOrderTypeEntityType(mzk_patientordermzk_Type orderType)
        {
            switch (orderType)
            {
                case mzk_patientordermzk_Type.Procedure:
                    return mzk_entitytype.ProcedureOrder;
                case mzk_patientordermzk_Type.Lab:
                    return mzk_entitytype.LabOrder;
                case mzk_patientordermzk_Type.Medication:
                    return mzk_entitytype.MedicationOrder;
                case mzk_patientordermzk_Type.Radiology:
                    return mzk_entitytype.RadiologyOrder;
                case mzk_patientordermzk_Type.SpecialTest:
                    return mzk_entitytype.SpecialTestOrder;
                case mzk_patientordermzk_Type.Thrapy:
                    return mzk_entitytype.TherapyOrder;
                case mzk_patientordermzk_Type.Referral:
                    return mzk_entitytype.ReferralOrder;
            }

            return mzk_entitytype.LabOrder;
        }

        public bool revertOrderStatus(PatientOrder patientOrder, string encounterId, StatusManagerParams paramValues, bool updateCaseTrans = false, string caseId = "")
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                mzk_patientorder entity = (mzk_patientorder)entityRepository.GetEntity("mzk_patientorder", new Guid(patientOrder.Id) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientorderid", "mzk_patientencounterid", "mzk_orderstatus", "mzk_type", "mzk_axcasetransrefrecid", "mzk_statusmanagerdetail", "mzk_customer", "mzk_orderingappointment", "mzk_fulfillmentappointment"));

                if (entity != null && entity.mzk_patientorderId.HasValue)
                {
                    if (entity.mzk_StatusManagerDetail == null)
                    {
                        throw new ValidationException("Status Manager Id not set. Contact your system administrator");
                    }

                    StatusManager statusManagerUndo = StatusManager.isUndoValid(entity.mzk_StatusManagerDetail.Id.ToString());

                    if (statusManagerUndo != null && !string.IsNullOrEmpty(statusManagerUndo.StatusId))
                    {
                        if (!string.IsNullOrEmpty(statusManagerUndo.RevertStatusId) && statusManagerUndo.revertStatus > 0)
                        {
                            entity.mzk_OrderStatus = new OptionSetValue(statusManagerUndo.revertStatus);
                            entity.mzk_StatusManagerDetail = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, new Guid(statusManagerUndo.RevertStatusId));

                            if (AppSettings.GetByKey("RISIntegration").ToLower() == true.ToString().ToLower() && entity.mzk_Type.Value == (int)mzk_patientordermzk_Type.Radiology && statusManagerUndo.sendOrm)
                            {
                                string appointmentId = string.Empty;

                                if (entity.mzk_fulfillmentappointment != null)
                                {
                                    appointmentId = ((EntityReference)entity.Attributes["mzk_fulfillmentappointment"]).Id.ToString();
                                }

                                string encounterIdOrder = "";

                                if (entity.mzk_PatientEncounterId != null)
                                {
                                    encounterIdOrder = entity.mzk_PatientEncounterId.Id.ToString();
                                }

                                this.sendORMRISMessage(entity.mzk_customer.Id.ToString(), 0, encounterIdOrder, patientOrder.Id, caseId, ((int)mzk_orderstatus.Cancelled).ToString(), appointmentId);
                            }

                            if (AppSettings.GetByKey("LISIntegration").ToLower() == true.ToString().ToLower() && entity.mzk_Type.Value == (int)mzk_patientordermzk_Type.Lab && statusManagerUndo.sendOrm)
                            {
                                string appointmentId = string.Empty;

                                if (entity.mzk_fulfillmentappointment != null)
                                {
                                    appointmentId = ((EntityReference)entity.Attributes["mzk_fulfillmentappointment"]).Id.ToString();
                                }

                                this.sendORMLISMessage(entity.mzk_customer.Id.ToString(), 0, entity.mzk_PatientEncounterId.Id.ToString(), patientOrder.Id, caseId, ((int)mzk_orderstatus.Cancelled).ToString(), appointmentId);
                            }

                            if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                            {
                                if (updateCaseTrans && entity.mzk_AXCaseTransRefRecId.HasValue && entity.mzk_AXCaseTransRefRecId.Value > 0)
                                {
                                    if (!this.updateCaseTrans(Convert.ToInt64(entity.mzk_AXCaseTransRefRecId.Value), (mzk_orderstatus)statusManagerUndo.revertStatus, false, paramValues))
                                    {
                                        throw new ValidationException("Error updating case trans. Contact your system administrator");
                                    }
                                }
                            }

                            entityRepository.UpdateEntity(entity);

                            this.createOrderStatusLog(statusManagerUndo.revertStatus, patientOrder.Id, encounterId, paramValues);

                            return true;
                        }
                        else
                        {
                            throw new ValidationException("Revert Status not set in Status Manager. Please contact system administrator");
                        }
                    }
                    else
                    {
                        throw new ValidationException("Revert not allowed in Status Manager. Please contact system administrator");
                    }
                }
                else
                {
                    throw new ValidationException("Unable to find patient order");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Action Methods

        public bool refundOrder(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;

                return this.revertOrderStatus(patientOrder, "", paramValues, false, encounterId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool undoOrder(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;

                return this.revertOrderStatus(patientOrder, encounterId, paramValues, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool cancelOrder(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                if (paramValues == null || string.IsNullOrEmpty(paramValues.comments))
                {
                    throw new ValidationException("Comments must be filled in");
                }

                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.Cancelled).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool cancelAppointmentOrder(string orderId, string caseId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.Cancelled).ToString();

                return this.updateOrderStatus(patientOrder, "", paramValues, false, caseId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool declineOrder(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.Declined).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool verifiedReportedOrder(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.VerifiedandReported).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool sampleCollected(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.SampleCollected).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool sampleDispatched(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.SampleDispatched).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool sampleinProcess(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.SampleinProcess).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool sampleReceived(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.SampleReceived).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool needSecondSample(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.NeedSecondSample).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool quantityNotSufficient(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.Quantitynotsufficient).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool sampleSentOut(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.SampleSentOut).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool completeOrder(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.Completed).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool startOrder(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.Started).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool discontinueOrder(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.Discontinued).ToString();

                return this.updateOrderStatus(patientOrder, encounterId, paramValues, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool paidOrder(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.Paid).ToString();

                return this.updateOrderStatus(patientOrder, "", paramValues, false, encounterId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool pendingOrder(string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                PatientOrder patientOrder = new PatientOrder();

                patientOrder.Id = orderId;
                patientOrder.OrderStatus = ((int)mzk_orderstatus.Pending).ToString();

                return this.updateOrderStatus(patientOrder, "", paramValues, false, encounterId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public HMServiceStatus getServiceStatus(mzk_orderstatus orderStatus)
        {
            HMServiceStatus serviceStatus = HMServiceStatus.NotStarted;

            switch (orderStatus)
            {
                case mzk_orderstatus.Ordered:
                    serviceStatus = HMServiceStatus.Ordered;
                    break;
                case mzk_orderstatus.Paid:
                    serviceStatus = HMServiceStatus.Paid;
                    break;
                case mzk_orderstatus.Cancelled:
                    serviceStatus = HMServiceStatus.Cancelled;
                    break;
                case mzk_orderstatus.Declined:
                    serviceStatus = HMServiceStatus.Decline;
                    break;
                case mzk_orderstatus.Completed:
                    serviceStatus = HMServiceStatus.Complete;
                    break;
                case mzk_orderstatus.Started:
                    serviceStatus = HMServiceStatus.Started;
                    break;
                default:
                    serviceStatus = HMServiceStatus.NotStarted;
                    break;
            }

            return serviceStatus;
        }

        //public HMPRNIndication getPRNIndication(mzk_patientordermzk_PRNIndication prnIndication)
        //{
        //    HMPRNIndication indication = HMPRNIndication.None;

        //    switch (prnIndication)
        //    {
        //        case mzk_patientordermzk_PRNIndication.Fever:
        //            indication = HMPRNIndication.Fever;
        //            break;
        //        case mzk_patientordermzk_PRNIndication.Vomiting:
        //            indication = HMPRNIndication.Vomiting;
        //            break;
        //        case mzk_patientordermzk_PRNIndication.Pain:
        //            indication = HMPRNIndication.Pain;
        //            break;
        //        case mzk_patientordermzk_PRNIndication.Other:
        //            indication = HMPRNIndication.Other;
        //            break;
        //        default:
        //            indication = HMPRNIndication.None;
        //            break;
        //    }

        //    return indication;
        //}

        public bool createOrderStatusLog(int orderStatus, string orderId, string encounterId, StatusManagerParams paramValues)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                mzk_patientorderlog entity = new mzk_patientorderlog();

                entity.mzk_OrderStatus = new OptionSetValue(orderStatus);
                entity.mzk_PatientOrder = new EntityReference(mzk_patientorder.EntityLogicalName, new Guid(orderId));

                if (!string.IsNullOrEmpty(encounterId))
                {
                    entity.mzk_PatientEncounter = new EntityReference(mzk_patientencounter.EntityLogicalName, new Guid(encounterId));
                }

                if (paramValues != null)
                {
                    entity.mzk_Comments = paramValues.comments;
                    entity.mzk_BatchNumber = paramValues.batchNumber;

                    if (paramValues.expiryDate != DateTime.MinValue)
                        entity.mzk_ExpiryDate = paramValues.expiryDate;

                    if (!string.IsNullOrEmpty(paramValues.location))
                        entity.mzk_Location = paramValues.location;
                }

                entityRepository.CreateEntity(entity);

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool updateCaseTrans(long caseTransRecId, mzk_orderstatus orderStatus, bool createCharge, StatusManagerParams paramValues)
        {
            try
            {
                CaseRepository caseRepo = new CaseRepository();
                string batchId = string.Empty;

                if (paramValues != null)
                {
                    if (!string.IsNullOrEmpty(paramValues.batchNumber))
                    {
                        batchId = paramValues.batchNumber;
                    }
                }

                return caseRepo.updateCaseTransStatus(caseTransRecId, this.getServiceStatus(orderStatus), createCharge, 0, batchId, "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> createCaseTrans(string encounterId , string orderId, string productId, mzk_orderstatus orderStatus, decimal quantity = 1, string unit = "", HMUrgency urgency = HMUrgency.None, string notesToPharmacy = "", string patientInstructionsEng = "", string patientInstructionsArabic = "", long specialityRecId = 0, long treatmentLocationRecId = 0, mzk_patientordermzk_PRNIndication prnIndicationOrder = 0)
        {
            try
            {
                   CaseRepository caseRepo = new CaseRepository();
                   PatientEncounter patEnc = new PatientEncounter();
                   mzk_patientorder patientOrderEntity = null;
                   SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                   patEnc.EncounterId = encounterId;

                   List<PatientEncounter> list = await patEnc.getEncounterDetails(patEnc);

                   if (list != null && list.Count > 0)
                   {
                       patEnc = list.First();
                       string itemId = "";

                       if (!string.IsNullOrEmpty(productId))
                       {
                           itemId = Products.getItemId(productId);
                       }

                       if (specialityRecId == 0 && string.IsNullOrEmpty(itemId))
                       {
                           throw new ValidationException("Item Id not set for the product. Unable to create Order");
                       }

                       //HMPRNIndication prnIndication = HMPRNIndication.None;

                       //if(prnIndicationOrder > 0)
                       //{
                       //    prnIndication = this.getPRNIndication(prnIndicationOrder);
                       //}                 

                       long caseTransRecId = caseRepo.createCaseTrans(itemId, this.getServiceStatus(orderStatus), patEnc.CaseId, patEnc.AppointmentRefRecId, quantity, orderId, unit, urgency, notesToPharmacy , patientInstructionsEng , patientInstructionsArabic, specialityRecId, treatmentLocationRecId);

                       if (caseTransRecId > 0)
                       {
                           patientOrderEntity = (mzk_patientorder)entityRepository.GetEntity(mzk_patientorder.EntityLogicalName, new Guid(orderId), new Microsoft.Xrm.Sdk.Query.ColumnSet(false));

                           patientOrderEntity.mzk_AXCaseTransRefRecId = Convert.ToDecimal(caseTransRecId);

                           entityRepository.UpdateEntity(patientOrderEntity);
                       }
                       else
                       {
                           throw new ValidationException("Error creating case trans. Please contact system administrator");
                       }
                   }
                   else
                   {
                       throw new ValidationException("Unable to find encounter for the added order. Error creating case trans");
                   }

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Dictionary<string,string>> getWorkOrderProduct(string workorderproductId)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(msdyn_workorderproduct.EntityLogicalName);

            query.Criteria.AddCondition("msdyn_workorderproductid", ConditionOperator.Equal, new Guid(workorderproductId));
            query.ColumnSet = new ColumnSet(true);

            LinkEntity workorder = new LinkEntity(msdyn_workorderproduct.EntityLogicalName, msdyn_workorder.EntityLogicalName, "msdyn_workorder", "msdyn_workorderid", JoinOperator.Inner);
            workorder.Columns = new ColumnSet(true);
            workorder.EntityAlias = "workorder";

            LinkEntity incident = new LinkEntity(msdyn_workorder.EntityLogicalName, "incident", "msdyn_servicerequest", "incidentid", JoinOperator.Inner);
            incident.Columns = new ColumnSet(true);
            incident.EntityAlias = "incident";
            
            workorder.LinkEntities.Add(incident);
            query.LinkEntities.Add(workorder);    

            EntityCollection entityCollection = repo.GetEntityCollection(query);

            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
            {
               msdyn_workorderproduct product = (msdyn_workorderproduct)entityCollection.Entities[0];

                if (entityCollection.Entities[0].Attributes.Contains("incident.incidentid"))
                {
                    dictionary.Add("mzk_caseid", (entityCollection.Entities[0].Attributes["incident.incidentid"] as AliasedValue).Value.ToString());
                }
                if (entityCollection.Entities[0].Attributes.Contains("msdyn_product"))
                {
                    dictionary.Add("msdyn_product", (entityCollection.Entities[0].GetAttributeValue<EntityReference>("msdyn_product").Id.ToString()));
                }
            }

            return dictionary;
        }

        public async Task<bool> createCaseTransByCase(string caseId, string orderId, string productId, mzk_orderstatus orderStatus, string mzk_axclinicrefrecid = null)
        {
            try
            {
                CaseRepository caseRepo = new CaseRepository();
                mzk_patientorder patientOrderEntity = null;
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                string itemId = "";
                long caseTransRecId = 0;

                if (!string.IsNullOrEmpty(productId))
                {
                    itemId = Products.getItemId(productId);
                }

                if (string.IsNullOrEmpty(itemId))
                {
                    throw new ValidationException("Item Id not set for the product. Unable to create Order");
                }

                if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                {
                    caseTransRecId = caseRepo.createCaseTrans(itemId, this.getServiceStatus(orderStatus), caseId, 0, 1, orderId, "", HMUrgency.None, "", "", "", 0, Int64.Parse(mzk_axclinicrefrecid));
                }

                if (caseTransRecId > 0)
                {
                    if (!string.IsNullOrEmpty(orderId))
                    {
                        patientOrderEntity = (mzk_patientorder)entityRepository.GetEntity(mzk_patientorder.EntityLogicalName, new Guid(orderId), new Microsoft.Xrm.Sdk.Query.ColumnSet(false));

                        patientOrderEntity.mzk_AXCaseTransRefRecId = Convert.ToDecimal(caseTransRecId);

                        entityRepository.UpdateEntity(patientOrderEntity);
                    }
                }
                else
                {
                    throw new ValidationException("Error creating case trans. Please contact system administrator");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<bool> updateAppointmentDetails(string orderId, DateTime date, string appointmentId, string patientId, bool registered)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                mzk_patientorder entity = (mzk_patientorder)entityRepository.GetEntity("mzk_patientorder", new Guid(orderId) { },
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientorderid", "mzk_orderstatus", "mzk_type", "mzk_customer", "mzk_patientencounterid", "mzk_fulfillmentappointment"));

                if (entity != null && entity.mzk_patientorderId.HasValue)
                {
                    entity.mzk_FulfillmentDate = date;
                    entity.mzk_fulfillmentappointment = new EntityReference("mzk_patientorder", new Guid(appointmentId));

                    entityRepository.UpdateEntity(entity);

                    if ((!string.IsNullOrEmpty(appointmentId) && AppSettings.GetByKey("RISIntegration").ToLower() == true.ToString().ToLower() && entity.mzk_Type.Value == (int)mzk_patientordermzk_Type.Radiology))
                    {
                        RIS ris = new RIS();
                        Patient patient = new Patient();

                        bool adtRet = false;

                        if (registered)
                        {
                            adtRet = await patient.updatePatientRIS(appointmentId);
                        }
                        else
                        {
                            adtRet = await patient.createPatientRIS(appointmentId, patientId);
                        }

                        if (adtRet && PatientCase.getCaseType(entity.mzk_PatientEncounterId.Id.ToString()) == mzk_casetype.Emergency)
                        {
                            this.sendORMRISMessage(entity.mzk_customer.Id.ToString(), 0, entity.mzk_PatientEncounterId.Id.ToString(), entity.mzk_patientorderId.Value.ToString(), "", entity.mzk_OrderStatus.Value.ToString(), entity.mzk_fulfillmentappointment.Id.ToString());
                        }
                    }

                    return true;
                }
                else
                {
                    throw new ValidationException("Unable to find patient order");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PatientOrderLog getOrderStatusLogDetails(int orderStatus, string orderId)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_patientorderlog.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                query.Criteria.AddCondition("mzk_patientorder", ConditionOperator.Equal, new Guid(orderId));
                query.Criteria.AddCondition("mzk_orderstatus", ConditionOperator.Equal, orderStatus);

                query.AddOrder("createdon", OrderType.Descending);

                EntityCollection entitycol = entityRepository.GetEntityCollection(query);

                PatientOrderLog patientOrderLog = null;

                if (entitycol != null && entitycol.Entities != null && entitycol.Entities.Count > 0)
                {
                    mzk_patientorderlog entity = (mzk_patientorderlog)entitycol.Entities[0];

                    patientOrderLog = new PatientOrderLog();

                    patientOrderLog.logDateTime = entity.CreatedOn.Value;
                    patientOrderLog.Location = entity.mzk_Location;
                    patientOrderLog.Comments = entity.mzk_Comments;
                }

                return patientOrderLog;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool sendPanicResult(string orderId, string smsMessage)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_patientorder.EntityLogicalName);

                query.ColumnSet = new ColumnSet("createdby");
                query.Criteria.AddCondition("mzk_patientorderid", ConditionOperator.Equal, new Guid(orderId));

                LinkEntity UserEntity = new LinkEntity(mzk_patientorder.EntityLogicalName, SystemUser.EntityLogicalName, "createdby", "systemuserid", JoinOperator.Inner);
                UserEntity.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_axresourcerefrecid");
                UserEntity.EntityAlias = "UserEntity";

                query.LinkEntities.Add(UserEntity);

                EntityCollection entityCollection = entityRepository.GetEntityCollection(query);

                if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                {
                    mzk_patientorder entity = (mzk_patientorder)entityCollection.Entities[0];

                    entity.mzk_IsPanic = true;

                    entityRepository.UpdateEntity(entity);

                    if (entity.Attributes.Contains("UserEntity.mzk_axresourcerefrecid"))
                    {
                        long resourceRecId = Convert.ToInt64(((AliasedValue)entity.Attributes["UserEntity.mzk_axresourcerefrecid"]).Value);

                        CommonRepository repo = new CommonRepository();

                        return repo.sendResourceSms(resourceRecId, smsMessage);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new ValidationException("Unable to find patient order");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool sendORMLISMessage(string patientId, long axRefAppointmentRecId, string encounterId, string orderId, string caseId, string orderStatus, string appointmentId = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientId))
                {
                    string apptId = "";
                    
                    if (!string.IsNullOrEmpty(appointmentId))
                    {
                        apptId = appointmentId;
                    }
                    else
                    {
                        PatientEncounter enc = new PatientEncounter();

                        enc.EncounterId = encounterId;

                        List<PatientEncounter> listEnc = enc.encounterDetails(enc).Result;

                        if (listEnc != null && listEnc.Count > 0)
                        {
                            apptId = listEnc.FirstOrDefault().AppointmentId.ToString();
                        }
                    }
                    
                    LIS lis = new LIS();

                    string messageId = lis.ORM_O01(patientId, orderId, apptId, caseId, orderStatus);

                    if (string.IsNullOrEmpty(messageId))
                    {
                        throw new ValidationException("Error sending ORM message. Contact your system administrator");
                    }
                    
                    if (!Log.createLog(messageId, mzk_messagetype.ORM01, orderId, mzk_messagedirection.Outbound, mzk_acknowledgecode.Empty, ""))
                    {
                        throw new ValidationException("Unable to create ORM message log");
                    }                    

                    return true;
                }
                else
                {
                    throw new ValidationException("Unable to find Patient record");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool sendORMRISMessage(string patientId, long axRefAppointmentRecId, string encounterId, string orderId, string caseId, string orderStatus, string appointmentId = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientId))
                {
                    string apptId = "";
                    
                    if (!string.IsNullOrEmpty(appointmentId))
                    {
                        apptId = appointmentId;
                    }
                    else
                    {
                        PatientEncounter enc = new PatientEncounter();

                        enc.EncounterId = encounterId;

                        List<PatientEncounter> listEnc = enc.encounterDetails(enc).Result;

                        if (listEnc != null && listEnc.Count > 0)
                        {
                            apptId = listEnc.FirstOrDefault().AppointmentId.ToString();
                        }
                    }
                    
                    RIS ris = new RIS();

                    string messageId = ris.ORM_O01(patientId, orderId, apptId, caseId, orderStatus);

                    if (string.IsNullOrEmpty(messageId))
                    {
                        throw new ValidationException("Error sending ORM message. Contact your system administrator");
                    }
                                            
                    if (!Log.createLog(messageId, mzk_messagetype.ORM01, orderId, mzk_messagedirection.Outbound, mzk_acknowledgecode.Empty, ""))
                    {
                        throw new ValidationException("Unable to create ORM message log");
                    }                    

                    return true;
                }
                else
                {
                    throw new ValidationException("Unable to find Patient record");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EntityCollection getPatientOrderDetails(string patientEncounter)
        {
            #region Patient Order Query
            QueryExpression query = new QueryExpression(mzk_patientorder.EntityLogicalName);
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);

            childFilter.AddCondition("mzk_orderstatus", ConditionOperator.NotEqual, (int)mzk_orderstatus.Cancelled);

            if (!string.IsNullOrEmpty(patientEncounter))
                childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(patientEncounter));
            
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            LinkEntity EntityProduct = new LinkEntity("mzk_patientorder", "product", "mzk_productid", "productid", JoinOperator.LeftOuter);
            EntityProduct.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_cptcodeid");

            LinkEntity EntitySpecialty = new LinkEntity("mzk_patientorder", "characteristic", "mzk_specialityid", "characteristicid", JoinOperator.Inner);
            EntitySpecialty.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            EntitySpecialty.EntityAlias = "mzk_speciality";

            LinkEntity EntityReferringPhysician = new LinkEntity("mzk_patientorder", "mzk_referringphysician", "mzk_referraltoexternalid", "mzk_referringphysicianid", JoinOperator.LeftOuter);
            EntityReferringPhysician.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            EntityReferringPhysician.EntityAlias = "ReferringPhysician";

            LinkEntity EntityDiagnosis = new LinkEntity("mzk_patientorder", "mzk_concept", "mzk_associateddiagnosisid", "mzk_conceptid", JoinOperator.LeftOuter);
            EntityDiagnosis.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptname", "mzk_icdcodeid");
            EntityDiagnosis.EntityAlias = "mzk_conceptDiagnosis";

            LinkEntity EntityFrequecy = new LinkEntity("mzk_patientorder", "mzk_ordersetup", "mzk_frequencyid", "mzk_ordersetupid", JoinOperator.LeftOuter);
            EntityFrequecy.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_arabicdescription");
            EntityFrequecy.EntityAlias = "mzk_ordersetupFrequency";

            query.LinkEntities.Add(EntityProduct);
            query.LinkEntities.Add(EntitySpecialty);
            query.LinkEntities.Add(EntityReferringPhysician);
            query.LinkEntities.Add(EntityDiagnosis);
            query.LinkEntities.Add(EntityFrequecy);

            #endregion

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            return entitycollection;
        }

        public async Task<string> addPatientOrderByList(List<PatientOrder> patientOrderList)
        {

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            xrm.mzk_patientorder patientOrderEntity = new xrm.mzk_patientorder();
            try
            {
                PatientCase PatientCase = null;
                mzk_casetype caseType = mzk_casetype.OutPatient;
                StatusManager statusManager = null;
                Dictionary<string, Products> productList = null;

                if (patientOrderList != null)
                {
                    productList = Products.getProduct(patientOrderList.Select(item => item.TestId).ToList());
                }

                foreach (PatientOrder patientOrder in patientOrderList)
                {
                    Products productDetails = null;

                    patientOrderEntity.mzk_appointable = true;

                    if (!string.IsNullOrEmpty(patientOrder.appointmentId))
                    {
                        patientOrderEntity.mzk_orderingappointment = new EntityReference("mzk_patientorder", new Guid(patientOrder.appointmentId));
                        patientOrderEntity.mzk_fulfillmentappointment = new EntityReference("mzk_patientorder", new Guid(patientOrder.appointmentId));
                    }

                    if (!string.IsNullOrEmpty(patientOrder.TestId) && productList.TryGetValue(patientOrder.TestId, out productDetails))
                        patientOrderEntity.Attributes["mzk_productid"] = new EntityReference("product", new Guid(productDetails.ProductId));
                    else
                        throw new ValidationException("Test Name " + patientOrder.TestId + " not found in Setup");

                    if (!string.IsNullOrEmpty(patientOrder.CaseId))
                    {
                        patientOrderEntity.Attributes["mzk_caseid"] = new EntityReference(Incident.EntityLogicalName, new Guid(patientOrder.CaseId));

                        if (PatientCase == null)
                        {
                            PatientCase = new PatientCase().getCaseDetails(patientOrder.CaseId).Result;
                        }
                        caseType = (mzk_casetype)PatientCase.CaseTypeValue;
                        patientOrderEntity.Attributes["mzk_customer"] = new EntityReference("contact", new Guid(PatientCase.PatientId));
                    }

                    if (patientOrder.OrderDate != DateTime.MinValue)
                        patientOrderEntity.Attributes["mzk_orderdate"] = patientOrder.OrderDate;
                    else
                        patientOrderEntity.Attributes["mzk_orderdate"] = DateTime.Now.Date;

                    patientOrderEntity.Attributes["mzk_fulfillmentdate"] = patientOrderEntity.Attributes["mzk_orderdate"];                    

                    if (!string.IsNullOrEmpty(patientOrder.orderingLocationId))
                    {
                        patientOrderEntity.Attributes["mzk_orderinglocation"] = new EntityReference("mzk_organizationalunit", new Guid(patientOrder.orderingLocationId));
                                                
                        string specialityName = new Clinic().getClinicDetails(patientOrder.orderingLocationId).speciality;
                        
                            if (specialityName != null)
                            {
                                patientOrderEntity.Attributes["mzk_specialtyname"] = specialityName;
                            }                        
                    }

                    if (String.IsNullOrEmpty(productDetails.Type))
                    {
                        throw new ValidationException("Product type not set for item " + patientOrder.TestId);
                    }

                    mzk_producttype productType = (mzk_producttype)Convert.ToInt32(productDetails.Type);
                    mzk_patientordermzk_Type orderType = mzk_patientordermzk_Type.Lab;
                    mzk_entitytype entityType = mzk_entitytype.LabOrder;

                    switch (productType)
                    {
                        case mzk_producttype.Lab:
                            orderType = mzk_patientordermzk_Type.Lab;
                            entityType = mzk_entitytype.LabOrder;
                            break;
                        case mzk_producttype.Medication:
                            orderType = mzk_patientordermzk_Type.Medication;
                            entityType = mzk_entitytype.MedicationOrder;
                            break;
                        case mzk_producttype.Radiology:
                            orderType = mzk_patientordermzk_Type.Radiology;
                            entityType = mzk_entitytype.RadiologyOrder;
                            break;
                        case mzk_producttype.SpecialTest:
                            orderType = mzk_patientordermzk_Type.SpecialTest;
                            entityType = mzk_entitytype.SpecialTestOrder;
                            break;
                        case mzk_producttype.Procedure:
                            orderType = mzk_patientordermzk_Type.Procedure;
                            entityType = mzk_entitytype.ProcedureOrder;
                            break;

                    }

                    patientOrderEntity.Attributes["mzk_type"] = new OptionSetValue((int)orderType);

                    if (statusManager == null)
                    {
                        statusManager = StatusManager.getRootStatus(entityType, caseType);
                    }

                    if (statusManager != null)
                    {
                        patientOrderEntity.mzk_OrderStatus = new OptionSetValue(statusManager.status);
                        patientOrderEntity.mzk_StatusManagerDetail = new EntityReference(mzk_statusmanagerdetails.EntityLogicalName, new Guid(statusManager.StatusId));
                    }

                    /*    bool isDuplicateAllowed = false;
                        if (!string.IsNullOrEmpty(patientLabOrder.EncounterId) && !string.IsNullOrEmpty(patientLabOrder.TestName))
                            isDuplicateAllowed = new PatientEncounter().DuplicateDetection(patientLabOrder.EncounterId, patientLabOrder.TestName);

                        if (isDuplicateAllowed == true)
                        {*/
                    Id = Convert.ToString(entityRepository.CreateEntity(patientOrderEntity));
                    /*   }
                       else
                       {
                           throw new ValidationException("Same Lab Test cannot be added multiple times.");
                       }*/

                    if (AppSettings.GetByKey("OperationsIntegration").ToLower() == true.ToString().ToLower())
                    {
                        if (!string.IsNullOrEmpty(patientOrder.CaseId))
                        {
                            await this.createCaseTransByCase(patientOrder.CaseId, Id, productDetails.ProductId, (mzk_orderstatus)patientOrderEntity.mzk_OrderStatus.Value);
                        }
                    }

                    if (productType == mzk_producttype.Lab && AppSettings.GetByKey("LISIntegration").ToLower() == true.ToString().ToLower() && statusManager.sendOrm)
                    {
                        this.sendORMLISMessage(PatientCase.PatientId, 0, "", Id, patientOrder.CaseId, statusManager.status.ToString(), patientOrderEntity.mzk_orderingappointment.Id.ToString());
                    }
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

            return true.ToString();
        }

        #region Get PatientOrder
        public bool getPatientOrder(PatientOrder model, Entity entity, bool forFulfillment, string orderId, mzk_entitytype entityType, bool forHistory = false)
        {
            bool ret = true;
            Configuration configuration = new Configuration();
            configuration = configuration.getConfiguration();
            if (configuration.snoozeAllowedTimes != 0)
            {
                model.snoozeAllowed = configuration.snoozeAllowedTimes;
            }
            if (configuration.snoozeDefaultTime != 0)
            {
                model.snoozeTimeInMinutes = configuration.snoozeDefaultTime;
            }
            if (entity.Attributes.Contains("mzk_patientordernumber"))
                model.OrderNumber = entity.Attributes["mzk_patientordernumber"].ToString();

            if (entity.Attributes.Contains("mzk_patientorderid"))
                model.Id = entity.Id.ToString();

            if (entity.Attributes.Contains("mzk_orderdate"))
                model.OrderDate = Convert.ToDateTime(entity.Attributes["mzk_orderdate"]);

            if (entity.Attributes.Contains("mzk_orderstatus"))
            {
                model.OrderStatus = (entity["mzk_orderstatus"] as OptionSetValue).Value.ToString();
                model.OrderStatusText = entity.FormattedValues["mzk_orderstatus"].ToString();
            }

            if (entity.Attributes.Contains("mzk_patientencounterid"))
            {
                model.EncounterId = ((EntityReference)entity["mzk_patientencounterid"]).Id.ToString();
            }

            if (entity.Attributes.Contains("mzk_statusmanagerdetail"))
            {
                model.StatusManagerDetailsId = ((EntityReference)entity["mzk_statusmanagerdetail"]).Id.ToString();
            }           
            else if (!forHistory && entityType != mzk_entitytype.ReferralOrder)
            {
                ret = false;
            }
                 
            if (ret && !string.IsNullOrEmpty(model.StatusManagerDetailsId))
            {
                StatusManager statusManagerObj = new StatusManager(model.StatusManagerDetailsId);
                mzk_orderstatus status = (mzk_orderstatus)Convert.ToInt32(model.OrderStatus);

                if (forFulfillment)
                {
                    if (statusManagerObj.getHierarchy())
                    {
                        model.nextActionList = statusManagerObj.getNextActionList();

                        if (!statusManagerObj.isFulfilmentAction())
                        {
                            if (model.nextActionList == null || model.nextActionList.Count == 0)
                            {
                                ret = false;
                            }
                        }

                        if (ret)
                        {
                            model.showTimer = statusManagerObj.showTimerCheck();
                            model.currentDateTime = DateTime.UtcNow;

                            if (statusManagerObj.showStartTimeDetails())
                            {
                                PatientOrderLog log = this.getOrderStatusLogDetails((int)status, model.Id);

                                if (log != null)
                                {
                                    model.startTime = log.logDateTime;
                                }
                            }

                            if (statusManagerObj.showEndTimeDetails())
                            {
                                PatientOrderLog log = this.getOrderStatusLogDetails((int)status, model.Id);

                                if (log != null)
                                {
                                    model.endTime = log.logDateTime;
                                }

                                log = this.getOrderStatusLogDetails(statusManagerObj.getStartTimerStatus(), model.Id);

                                if (log != null)
                                {
                                    model.startTime = log.logDateTime;
                                }
                            }
                        }
                    }
                    else
                    {
                        ret = false;
                    }
                }
                else
                {
                    if (statusManagerObj.getHierarchy())
                    {
                        model.allowCancel = statusManagerObj.allowCancelled();
                        model.allowEdit = statusManagerObj.allowEditing();
                        model.allowOrder = statusManagerObj.allowOrdered();
                    }
                }
            }

            if (entity.Attributes.Contains("createdon"))
            {
                model.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);
            }

            return ret;
        }

        #endregion
    }

    public class PatientOrderLog
    {
        public DateTime logDateTime { get; set; }

        public string Location { get; set; }

        public string Comments { get; set; }
    }
}
