using Helper;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.BiztalkRepository;
using MazikLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class RIS
    {
        public string ADT_A04(string aptRecId, string caseId, string patientId)
        {
            RISRepository risObj = new RISRepository();
            return risObj.ADT_A04(aptRecId, caseId, patientId);

        }
        public string ADT_A08(string aptRecId, string caseId, string patientId)
        {
            RISRepository risObj = new RISRepository();
            return risObj.ADT_A08(aptRecId, caseId, patientId);

        }
        public string ADT_A40(string patientId, string mergepatientId)
        {
            RISRepository risObj = new RISRepository();
            return risObj.ADT_A40(patientId, mergepatientId);

        }
        public string ORM_O01(string patientId, string OrderId, string apptRecId, string caseId, string OrderStatus)
        {
            RISRepository risObj = new RISRepository();
            return risObj.ORM_O01(patientId, OrderId, apptRecId, caseId, OrderStatus);

        }

        public void ReceiveADTA04Acknowledgment(Acknowledgment pAck)
        {
            Log.updateLog(pAck.MessageControlId, pAck.acknowledgmentCode, pAck.TextMessage);

        }

        public void ReceiveADTA40Acknowledgment(Acknowledgment pAck)
        {
            Log.updateLog(pAck.MessageControlId, pAck.acknowledgmentCode, pAck.TextMessage);

        }

        public void ReceiveADTA08Acknowledgment(Acknowledgment pAck)
        {
            Log.updateLog(pAck.MessageControlId, pAck.acknowledgmentCode, pAck.TextMessage);

        }

        public void ReceiveORMO01Acknowledgment(Acknowledgment pAck)
        {
            Log.updateLog(pAck.MessageControlId, pAck.acknowledgmentCode, pAck.TextMessage);

        }

        public void ORUResult(RIS_OrderResult pAck)
        {
            if (pAck != null)
            {
                bool ret = false;
                PatientOrder order = null;
                StatusManagerParams parms = null;
                PatientRadiologyOrder radiologyOrder = null;

                try
                {
                    if (pAck.ResultStatus == "F" && pAck.ReportStatus == "GDT")
                    {
                        order = new PatientOrder();

                        ret = order.verifiedReportedOrder(pAck.OrderId, "", parms);
                    }

                    radiologyOrder = new PatientRadiologyOrder();

                    radiologyOrder.Id = pAck.OrderId;
                    radiologyOrder.ReportPath = pAck.ReportPath;
                    radiologyOrder.ResultStatus = pAck.ResultStatus;
                    radiologyOrder.RISLink = pAck.ImageUrl;

                    if (!radiologyOrder.updateResult(radiologyOrder).Result)
                    {
                        throw new ValidationException("Unable to update patient result");
                    }

                    ret = true;
                }
                catch (Exception ex)
                {
                    ret = false;

                    Log.createLog(pAck.MessageControlId, mzk_messagetype.ORU01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AE, ex.Message);

                    throw ex;
                }

                if (ret)
                {

                    if (!Log.createLog(pAck.MessageControlId, mzk_messagetype.ORU01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AA))
                    {
                        throw new ValidationException("Unable to create ORU message log");
                    }

                }
            }
        }

        public void ORMOutBound(ORMOutbound pAck)
        {
            if (pAck != null)
            {
                bool ret = false;
                PatientOrder order = null;
                StatusManagerParams parms = null;

                try
                {
                    switch (pAck.OrderStatus)
                    {
                        case "IP":
                        case "SC":
                            order = new PatientOrder();

                            ret = order.startOrder(pAck.OrderId, "", parms);
                            break;

                        case "CM":
                            order = new PatientOrder();

                            ret = order.completeOrder(pAck.OrderId, "", parms);
                            break;
                        case "DC":
                            order = new PatientOrder();

                            ret = order.discontinueOrder(pAck.OrderId, "", parms);
                            break;
                        case "CA":
                            order = new PatientOrder();
                            parms = new StatusManagerParams();
                            parms.comments = pAck.Comment;

                            ret = order.cancelOrder(pAck.OrderId, "", parms);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ret = false;

                    Log.createLog(pAck.MessageControlId, mzk_messagetype.ORM01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AE, ex.Message);


                    throw ex;
                }

                if (ret)
                {

                    if (!Log.createLog(pAck.MessageControlId, mzk_messagetype.ORM01, pAck.OrderId, mzk_messagedirection.Inbound, mzk_acknowledgecode.AA, ""))
                    {
                        throw new ValidationException("Unable to create ADT08 message log");
                    }

                }
            }
        }


        private static void DecodeAckCode(string ackCode, out string description, out bool isSuccess)
        {
            var codeMap = new Dictionary<string, string>
                {
                    {"AA","Application Accept"},
                    {"AE","Application Error"},
                    {"AR","Application Reject"}
                };
            isSuccess = ackCode == "AA";
            codeMap.TryGetValue(ackCode, out description);
        }
    }
}
