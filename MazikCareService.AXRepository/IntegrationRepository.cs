
using MazikCareService.AXRepository.AXServices;
using MazikLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.AXRepository
{
    public class IntegrationRepository
    {
        public bool createLog(string messageNumber, HMHL7MessageType messageType, string refId, HMHL7MessageDirection direction, string ackCode, string _textMessage)
        {
            HMIntegrationServiceClient client = null;
            bool ret = false;

            //try
            //{
            //    client = new HMIntegrationServiceClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    ret = client.createLog(callContext, messageNumber, messageType, refId, direction, ackCode, _textMessage);
            //}
            //catch (System.ServiceModel.FaultException<AifFault> aiffaultException)
            //{
            //    throw ValidationException.create(aiffaultException.Message, aiffaultException.HelpLink, aiffaultException.Source);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    client.Close();
            //}

            return ret;
        }

        public bool updateLog(string messageNumber, string ackCode, string _textMessage)
        {
            HMIntegrationServiceClient client = null;
            bool ret = false;

            //try
            //{
            //    client = new HMIntegrationServiceClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    ret = client.updateLog(callContext, messageNumber, ackCode, _textMessage);
            //}
            //catch (System.ServiceModel.FaultException<AifFault> aiffaultException)
            //{
            //    throw ValidationException.create(aiffaultException.Message, aiffaultException.HelpLink, aiffaultException.Source);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    client.Close();
            //}

            return ret;
        }
    }
}
