
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
    public class ResourceRepository
    {
        public HMSpecialityDataContract[] getResourceSpecialities(long resourceRecId)
        {
            HMSpecialityDataContract[] contract = null;

            //HMResourceServiceClient client = new HMResourceServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //contract = client.getResourceSpecialitiesList(callContext, resourceRecId);
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
                    getResourceSpecialitiesListResponse response = ((HMResourceService)channel).getResourceSpecialitiesList(new getResourceSpecialitiesList() { _hmResourceMasterRecId = resourceRecId });
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

        public HMResourceDataContract[] getResources(long specialityRecId)
        {
            HMResourceDataContract[] contract = null;

            //HMResourceServiceClient client = new HMResourceServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //contract = client.getResourceList(callContext, specialityRecId);
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
                    getResourceListResponse response = ((HMResourceService)channel).getResourceList(new getResourceList() { _specialityRecId = specialityRecId });
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

        public HMResourceDataContract getResourceDetails(long resourceRecId)
        {
            HMResourceDataContract contract = new HMResourceDataContract();

            //HMResourceServiceClient client = new HMResourceServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //HMResourceDataContract contract = client.getResourceDetails(callContext, resourceRecId);
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
                    getResourceDetailsResponse response = ((HMResourceService)channel).getResourceDetails(new getResourceDetails() { _recId = resourceRecId });
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

        //public HMResourceLoginSMSCodeContract generateResourceSMSCode(long resourcRecId, long smsRecId)
        //{
        //    HMResourceServiceClient client = null; 
        //    HMResourceLoginSMSCodeContract contract = null;

        //    try
        //    {
        //        client = new HMResourceServiceClient();

        //        CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
        //        client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

        //        contract = client.generateResourceSMSCode(callContext, resourcRecId, smsRecId);
        //    }
        //    catch (System.ServiceModel.FaultException<AifFault> aiffaultException)
        //    {
        //        throw ValidationException.create(aiffaultException.Message, aiffaultException.HelpLink, aiffaultException.Source);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        client.Close();
        //    }

        //    return contract;
        //}

        //public HMResourceLoginSMSCodeContract verifyResourceSMSCode(string smsCode, long smsRecId)
        //{
        //    HMResourceServiceClient client = null;
        //    HMResourceLoginSMSCodeContract contract = null;

        //    try
        //    {
        //        client = new HMResourceServiceClient();

        //        CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
        //        client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

        //        contract = client.verifyResourceSMSCode(callContext, smsCode, smsRecId);
        //    }
        //    catch (System.ServiceModel.FaultException<AifFault> aiffaultException)
        //    {
        //        throw ValidationException.create(aiffaultException.Message, aiffaultException.HelpLink, aiffaultException.Source);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        client.Close();
        //    }

        //    return contract;
        //}
    }
}
