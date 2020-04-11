using Helper;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.BiztalkRepository;
using MazikLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MazikCareService.Core.Models.HL7
{
    public class LIS
    {
        public string ORM_O01(string patientId, string OrderId, string apptId, string caseId, string OrderStatus)
        {
            LISRepository risObj = new LISRepository();
            return risObj.ORM_O01(patientId, OrderId, apptId, caseId, OrderStatus);


        }
        public void ReceiveORMO01Acknowledgment(Acknowledgment pAck)
        {
            Log.updateLog(pAck.MessageControlId, pAck.acknowledgmentCode, pAck.TextMessage);
        }
        public string ORUResult(LIS_OrderResult pAck)
        {
            if (pAck != null)
            {
                bool ret = false;
                PatientOrder order = null;
                StatusManagerParams parms = null;
                PatientLabOrder labOrder = new PatientLabOrder();

                try
                {
                    PatientLabOrder labOrderDetails = labOrder.getPatientOrder(null, null, null, null, DateTime.MinValue, DateTime.MinValue, false, pAck.OrderId).Result.FirstOrDefault();

                    if (labOrderDetails != null)
                    {
                        if (pAck.ResultStatus == "F" && labOrderDetails.OrderStatus != ((int)mzk_orderstatus.VerifiedandReported).ToString())
                        {
                            order = new PatientOrder();

                            ret = order.verifiedReportedOrder(pAck.OrderId, "", parms);
                        }

                        labOrder = new PatientLabOrder();

                        labOrder.Id = pAck.OrderId;
                        labOrder.ResultStatus = pAck.ResultStatus;
                        labOrder.LISLink = Helper.decodeEscapeChar(pAck.ImageUrl);
                        labOrder.ReportPath = pAck.ReportPath;

                        if (!labOrder.updateResult(labOrder).Result)
                        {
                            throw new ValidationException("Unable to update patient result");
                        }

                        if (!string.IsNullOrEmpty(pAck.AbnormalResult) && pAck.AbnormalResult.ToLower() == "panic")
                        {
                            string testName = "";
                            string patientName = "";

                            testName = labOrderDetails.TestName;

                            Patient patient = new Patient().getPatientDetails(labOrderDetails.PatientId).Result;

                            if (patient != null)
                            {
                                patientName = patient.name;
                            }

                            if (!labOrder.sendPanicResult(labOrder.Id, string.Format("Panic result received for Patient {0} and Test {1} ", patientName, testName)))
                            {
                                throw new ValidationException("Unable to send panic result");
                            }
                        }

                        ret = true;
                    }
                    else
                    {
                        throw new ValidationException("Unable to find lab order");
                    }
                }
                catch (Exception ex)
                {
                    ret = false;

                    Log.createLog(pAck.MessageControlId, HMHL7MessageType.ORU01, pAck.OrderId, HMHL7MessageDirection.Inbound, "AE", ex.Message);

                    return Helper.generateACK("R01", pAck.MessageControlId, "AE", ex.Message);
                }

                if (ret)
                {
                    if (!Log.createLog(pAck.MessageControlId, mzk_messagetype.ORU01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AA))
                    {
                        throw new ValidationException("Unable to create ORU message log");
                    }

                    return Helper.generateACK("R01", pAck.MessageControlId, "AA", "");
                }
                else
                {
                    if (!Log.createLog(pAck.MessageControlId, mzk_messagetype.ORU01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AE, "Unknown error in updating result"))
                    {
                        throw new ValidationException("Unable to create ORU message log");
                    }

                    return Helper.generateACK("R01", pAck.MessageControlId, "AE", "Unknown error in updating result");
                }
            }
            else
            {
                if (!Log.createLog("", mzk_messagetype.ORU01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AE, "Invalid message format"))
                {
                    throw new ValidationException("Unable to create ORU message log");
                }

                return MazikCareService.Core.Models.HL7.Helper.generateACK("R01", "", "AE", "Invalid message format");
            }
        }


        public string ORMOutBound(ORMOutbound pAck)
        {
            if (pAck != null)
            {
                bool ret = false;
                PatientOrder order = null;
                StatusManagerParams parms = null;

                try
                {
                    switch (pAck.OrderStatus.ToLower().Trim())
                    {
                        case "sample collected":
                            order = new PatientOrder();
                            parms = new StatusManagerParams();

                            if (!string.IsNullOrEmpty(pAck.Location))
                            {
                                parms.location = pAck.Location;
                            }

                            ret = order.sampleCollected(pAck.OrderId, "", parms);
                            break;
                        case "sample received":
                            order = new PatientOrder();
                            parms = new StatusManagerParams();

                            if (!string.IsNullOrEmpty(pAck.Location))
                            {
                                parms.location = pAck.Location;
                            }

                            ret = order.sampleReceived(pAck.OrderId, "", parms);
                            break;
                        case "sample dispatched":
                            order = new PatientOrder();
                            parms = new StatusManagerParams();

                            if (!string.IsNullOrEmpty(pAck.Location))
                            {
                                parms.location = pAck.Location;
                            }

                            ret = order.sampleDispatched(pAck.OrderId, "", parms);
                            break;
                        case "sample in process":
                            order = new PatientOrder();
                            parms = new StatusManagerParams();

                            ret = order.sampleinProcess(pAck.OrderId, "", parms);
                            break;
                        case "quantity not sufficient":
                            order = new PatientOrder();
                            parms = new StatusManagerParams();

                            ret = order.quantityNotSufficient(pAck.OrderId, "", parms);
                            break;
                        case "sample sent out":
                            order = new PatientOrder();
                            parms = new StatusManagerParams();

                            if (!string.IsNullOrEmpty(pAck.Location))
                            {
                                parms.location = pAck.Location;
                            }

                            ret = order.sampleSentOut(pAck.OrderId, "", parms);
                            break;
                        case "need second sample":
                            order = new PatientOrder();
                            parms = new StatusManagerParams();

                            if (!string.IsNullOrEmpty(pAck.Location))
                            {
                                parms.location = pAck.Location;
                            }

                            ret = order.needSecondSample(pAck.OrderId, "", parms);
                            break;
                        case "ca":
                            order = new PatientOrder();
                            parms = new StatusManagerParams();

                            if (pAck.NTE != null)
                            {
                                parms.comments = pAck.NTE.CommentDescription;
                            }

                            ret = order.cancelOrder(pAck.OrderId, "", parms);
                            break;
                        case "dc":
                            order = new PatientOrder();
                            parms = new StatusManagerParams();

                            if (pAck.NTE != null)
                            {
                                parms.comments = pAck.NTE.CommentDescription;
                            }

                            ret = order.discontinueOrder(pAck.OrderId, "", parms);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ret = false;

                    Log.createLog(pAck.MessageControlId, mzk_messagetype.ORM01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AE, ex.Message);

                    return Helper.generateACK("O01", pAck.MessageControlId, "AE", ex.Message);
                }

                if (ret)
                {
                    if (!Log.createLog(pAck.MessageControlId, mzk_messagetype.ORM01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AA))
                    {
                        throw new ValidationException("Unable to create ORM message log");
                    }

                    return Helper.generateACK("O01", pAck.MessageControlId, "AA", "");
                }
                else
                {
                    if (parms != null)
                    {
                        if (!Log.createLog(pAck.MessageControlId, mzk_messagetype.ORM01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AE, "Unknown error in changing order status"))
                        {
                            throw new ValidationException("Unable to create ORM message log");
                        }

                        return Helper.generateACK("O01", pAck.MessageControlId, "AE", "Unknown error in changing order status");
                    }
                    else
                    {
                        if (!Log.createLog(pAck.MessageControlId, mzk_messagetype.ORM01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AE, "Unable to find any valid status for the order"))
                        {
                            throw new ValidationException("Unable to create ORM message log");
                        }

                        return Helper.generateACK("O01", pAck.MessageControlId, "AE", "Unable to find any valid status for the order");
                    }
                }

            }
            else
            {
                if (!Log.createLog("", mzk_messagetype.ORM01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AE, "Invalid message format"))
                {
                    throw new ValidationException("Unable to create ORM message log");
                }
                
                return MazikCareService.Core.Models.HL7.Helper.generateACK("O01", "", "AE", "Invalid message format");
            }
        }
    }
}
