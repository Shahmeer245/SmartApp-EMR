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
    public class PatientRepository
    {
        public HMPatientInfoContract GetPatientBasicDetails(string patientRecId)
        {
            HMPatientInfoContract contract = new HMPatientInfoContract();

            //HMPatientServiceClient client = new HMPatientServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //HMPatientInfoContract contract = new HMPatientInfoContract();

            //client.getPatientBasicDetails(callContext, patientRecId, contract);
            //client.Close();

            //return contract;

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMPatientServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();
                    getPatientBasicDetailsResponse response = ((HMPatientService)channel).getPatientBasicDetails(new getPatientBasicDetails() { _patientRecId = patientRecId });
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

        //public HMPatientInfoContract[] SearchPatientDetails(string searchValue, HMSearchPatientBy searchBy)
        //{
        //    HMPatientServiceClient client = new HMPatientServiceClient();

        //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
        //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

        //    HMPatientInfoContract[] contract = client.searchPatientDetails(callContext, searchValue, searchBy);
        //    client.Close();

        //    return contract;
        //}

        public HMPatientDataContract GetPatientDetails(string patientRecId)
        {
            HMPatientDataContract contract = null;

            //HMPatientServiceClient client = new HMPatientServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //contract = client.getPatientDetails(callContext, patientRecId);
            //client.Close();

            return contract;
        }

        public long createPatient(HMPatientDataContract contract)
        {
            long ret = 0;
            //HMPatientServiceClient client = null;

            //try
            //{
            //    client = new HMPatientServiceClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    ret = client.createPatient(callContext, contract);
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

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMPatientServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();
                    createPatientResponse response = ((HMPatientService)channel).createPatient(new createPatient() { _contract = contract });
                    if (response.result > 0)
                    {
                       // Helper.Files.SaveToCSV(response.result.ToString(), "ex", DateTime.Now, DateTime.Now);
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
                //Helper.Files.SaveToCSV(ex.Message, "ex", DateTime.Now, DateTime.Now);
                throw ex;
            }

            return ret;
        }

        public long createPatientInsurance(long insuranceRecId, long patientRecId)
        {
            long patientInsuranceRecId = 0;     

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMCaseServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();
                    createPatientInsuranceResponse response = ((HMCaseService)channel).createPatientInsurance(new createPatientInsurance() { _insuranceRecId = insuranceRecId, _patientREcId = patientRecId });

                    if (response.result > 0)
                    {
                        patientInsuranceRecId = response.result;
                    }
                    else
                    {
                        new ValidationException(CommonRepository.getErrorMessage(response.Infolog));
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return patientInsuranceRecId;
        }

    }
}
