
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
    public class ClinicRepository
    {
        public HMClinicDataContract[] getClinicsTree(long resourceRecId)
        {
            HMClinicDataContract[] contract = null;

            //HMResourceServiceClient client = new HMResourceServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //contract = client.getResourceClinicsList(callContext, resourceRecId);
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
                    getResourceClinicsListResponse response = ((HMResourceService)channel).getResourceClinicsList(new getResourceClinicsList() { _hmResourceMasterRecId = resourceRecId });
                    if (response.result.Length > 0)
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

        public HMClinicDataContract[] getAllClinicsList(HMFileLocationType locationType)
        {

            HMClinicDataContract[] contract = null;

            //HMResourceServiceClient client = new HMResourceServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //contract = client.getClinicsList(callContext, locationType);
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
                    getClinicsListResponse response = ((HMResourceService)channel).getClinicsList(new getClinicsList() { _locationType = locationType });
                    if (response.result.Length > 0)
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

        public HMClinicDataContract getClinicDetails(long clinicRecId)
        {
            HMClinicDataContract contract = new HMClinicDataContract();

            //HMResourceServiceClient client = new HMResourceServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //contract = client.getClinicDetails(callContext, clinicRecId);
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
                    getClinicDetailsResponse response = ((HMResourceService)channel).getClinicDetails(new getClinicDetails() { _recId = clinicRecId });
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
