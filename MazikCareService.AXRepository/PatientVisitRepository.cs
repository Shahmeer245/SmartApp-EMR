
using MazikCareService.AXRepository.AXServices;
using MazikLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.AXRepository
{
    public class PatientVisitRepository
    {
        public bool updateVisitReason(Guid caseId, long appointmentRecId, string visitReason)
        {
            //HMPatientVisitServiceClient client = null;
            bool ret = false;

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMPatientServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();
                    updateVisitReasonResponse response = ((HMPatientVisitService)channel).updateVisitReason(new updateVisitReason() { _apptRecId = appointmentRecId, _caseGuid = caseId, _visitReason = visitReason });
                    if (response.result)
                    {
                        ret = response.result;
                    }
                    else
                    {
                        new ValidationException(CommonRepository.getErrorMessage(response.Infolog));
                    }
                    //var result = ((HMPatientService)channel).getPatientBasicDetails(new getPatientBasicDetails() { _patientRecId = "0" }).result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //try
            //{
            //    client = new HMPatientVisitServiceClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    ret = client.updateVisitReason(callContext, caseId, appointmentRecId, visitReason);
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

        public HMPatientVisitContract[] getPatientVisit(Guid caseId, long appointmentRecId, long patientRecId)
        {
            //HMPatientVisitServiceClient client = new HMPatientVisitServiceClient();

            HMPatientVisitContract[] contract = null;

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //HMPatientVisitContract[] contract = client.getPatientVisit(callContext, caseId, appointmentRecId, patientRecId);
            //client.Close();

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMPatientServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();
                    getPatientVisitResponse response = ((HMPatientVisitService)channel).getPatientVisit(new getPatientVisit() { _apptRecId = appointmentRecId, _caseGuid = caseId, _patientRecId = patientRecId  });
                    if (response.result != null)
                    {
                        contract = response.result;
                    }
                    else
                    {
                        new ValidationException(CommonRepository.getErrorMessage(response.Infolog));
                    }
                    //var result = ((HMPatientService)channel).getPatientBasicDetails(new getPatientBasicDetails() { _patientRecId = "0" }).result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return contract;
        }
    }
}
