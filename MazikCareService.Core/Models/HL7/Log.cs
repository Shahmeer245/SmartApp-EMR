using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class Log
    {
        public static bool createLog(string messageId, HMHL7MessageType messageType, string refId, HMHL7MessageDirection direction, string ackCode = "", string _textMessage = "")
        {
            try
            {
                IntegrationRepository repo = new IntegrationRepository();

                return repo.createLog(messageId, messageType, refId, direction, ackCode, _textMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool updateLog(string messageId, string ackCode, string _textMessage)
        {
            try
            {
                IntegrationRepository repo = new IntegrationRepository();

                return repo.updateLog(messageId, ackCode, _textMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool createLog(string messageId, mzk_messagetype messageType, string refId, mzk_messagedirection direction, mzk_acknowledgecode ackCode, string _textMessage = "")
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                
                Entity Hl7IntegrationLogs = new Entity("mzk_hl7integrationlogs");                

                if (!string.IsNullOrEmpty(messageId))
                    Hl7IntegrationLogs["mzk_messagecontrolid"] = messageId;

                Hl7IntegrationLogs["mzk_messagetype"] = new OptionSetValue((int)messageType);

                if (!string.IsNullOrEmpty(refId))
                    Hl7IntegrationLogs["mzk_refid"] = refId;

                Hl7IntegrationLogs["mzk_messagedirection"] = new OptionSetValue((int)direction);

                Hl7IntegrationLogs["mzk_acknowledgecode"] = new OptionSetValue((int)ackCode);

                if (!string.IsNullOrEmpty(_textMessage))
                    Hl7IntegrationLogs["mzk_textmessage"] = _textMessage;                

                repo.CreateEntity(Hl7IntegrationLogs);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool updateLog(string messageId, mzk_acknowledgecode ackCode, string _textMessage)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                Entity Hl7IntegrationLogs = new Entity("mzk_hl7integrationlogs");

                if (!string.IsNullOrEmpty(messageId))
                    Hl7IntegrationLogs["mzk_messagecontrolid"] = messageId;                

                Hl7IntegrationLogs["mzk_acknowledgecode"] = new OptionSetValue((int)ackCode);

                if (!string.IsNullOrEmpty(_textMessage))
                    Hl7IntegrationLogs["mzk_textmessage"] = _textMessage;

                repo.UpdateEntity(Hl7IntegrationLogs);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
