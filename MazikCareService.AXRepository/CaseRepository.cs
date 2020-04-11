
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
    public class CaseRepository
    {
        public long createCaseTrans(string itemId, HMServiceStatus serviceStatus, string caseId, long appointmentRecId, decimal quantity, string orderId, string unit = "", HMUrgency urgency = HMUrgency.None, string notesToPharmacy = "", string patientInstructionsEng = "", string patientInstructionsArabic = "", long specialityRecId = 0, long treatmentLocationRecId = 0)
        {
            long caseTransRecId = 0;
            
            //HMCaseServiceClient client = null;            

            //try
            //{
            //    client = new HMCaseServiceClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    caseTransRecId = client.createCaseTrans(callContext, itemId, serviceStatus, new Guid(caseId), appointmentRecId, quantity, new Guid(orderId), unit, urgency, notesToPharmacy, patientInstructionsEng, patientInstructionsArabic, specialityRecId, prnIndication, treatmentLocationRecId);
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
            //return caseTransRecId;

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMCaseServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();
                    createCaseTransResponse obj = ((HMCaseService)channel).createCaseTrans(new createCaseTrans() { _itemid = itemId, _serviceStatus = serviceStatus, _caseGuid = new Guid(caseId), _apptRecId = appointmentRecId, _quantity = quantity, _orderId = string.IsNullOrEmpty(orderId) ? Guid.Empty : new Guid(orderId), _unit = unit, _urgency = urgency, notesToPharmacy = notesToPharmacy, patientInstructionsEng = patientInstructionsEng, patientInstructionsArabic = patientInstructionsArabic, _specialityRecId = specialityRecId , _treatmentLocation = treatmentLocationRecId});
                    if (obj.result > 0)
                    {
                        caseTransRecId = obj.result;
                    }
                    else
                    {
                        throw new ValidationException(CommonRepository.getErrorMessage(obj.Infolog));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseTransRecId;
        }

        public long createCase(long patientId, long clinicId, HMCaseType caseType, string caseId, string caseNumber)
        {
            long caseRecId = 0;

            //HMCaseServiceClient client = null;            

            //try
            //{
            //    client = new HMCaseServiceClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    caseRecId = client.createCase(callContext, patientId, clinicId, caseType);
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

            //return caseRecId;

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMCaseServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                   // Helper.Files.SaveToCSV(string.Format("{0}, {1}, {2},{3},{4}", patientId, clinicId, caseType, caseId, caseNumber), "rttrt", DateTime.Now, DateTime.Now);
                    SoapHelper.channelHelper();
                    createCaseResponse obj = ((HMCaseService)channel).createCase(new createCase() { _patientRecId = patientId, _organizationalUnitRecId = clinicId, _caseType = caseType, _crmRefRecId =new Guid(caseId), _caseNo = caseNumber});
                    if (obj.result > 0)
                    {
                       // Helper.Files.SaveToCSV(obj.result.ToString(), "rttrt", DateTime.Now, DateTime.Now);
                        caseRecId = obj.result;
                    }
                    else
                    {
                        throw new ValidationException(CommonRepository.getErrorMessage(obj.Infolog));
                    }
                }
            }
            catch (Exception ex)
            {
               // Helper.Files.SaveToCSV(ex.Message, "rttrt", DateTime.Now, DateTime.Now);
                throw ex;
            }
            return caseRecId;
        }

        public bool updateCaseTransStatus(long caseTransRecId, HMServiceStatus serviceStatus, bool createCharge, long appointmentRecId, string batchId, string caseId, long resourceRecId = 0)
        {
            bool ret = false;

            //HMCaseServiceClient client = null;            

            //try
            //{
            //    client = new HMCaseServiceClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };
            //    if (!string.IsNullOrEmpty(caseId))
            //    {
            //        ret = client.updateCaseTransStatus(callContext, caseTransRecId, serviceStatus, createCharge, appointmentRecId, batchId, new Guid(caseId), resourceRecId);
            //    }
            //    else
            //    {
            //        ret = client.updateCaseTransStatus(callContext, caseTransRecId, serviceStatus, createCharge, appointmentRecId, batchId, Guid.Empty, resourceRecId);
            //    }
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

            //return ret;

            try
            {
                updateCaseTransStatusResponse obj;

                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMCaseServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();
                    if (!string.IsNullOrEmpty(caseId))
                    {
                        obj  = ((HMCaseService)channel).updateCaseTransStatus(new updateCaseTransStatus() { _caseTransRecId = caseTransRecId, _serviceStatus = serviceStatus, createCharge = createCharge, _apptRecId = appointmentRecId, _batchId = batchId, _caseId = new Guid(caseId), _resourceRecId = resourceRecId });
                    }
                    else
                    {
                        obj = ((HMCaseService)channel).updateCaseTransStatus(new updateCaseTransStatus() { _caseTransRecId = caseTransRecId, _serviceStatus = serviceStatus, createCharge = createCharge, _apptRecId = appointmentRecId, _batchId = batchId, _caseId = Guid.Empty, _resourceRecId = resourceRecId });
                    }

                    if (obj.result)
                    {
                        ret = obj.result;                    
                    }
                    else
                    {
                        throw new ValidationException(CommonRepository.getErrorMessage(obj.Infolog));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }

        public bool updateReportUrl(long caseTransRecId, string reportUrl, string reportPath)
        {
            bool ret = false;

            //HMCaseServiceClient client = null;

            //try
            //{
            //    client = new HMCaseServiceClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    ret = client.updateReportUrl(callContext, caseTransRecId, reportPath, reportUrl);
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

            //return ret;
            try
            { 
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMCaseServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    updateReportUrlResponse obj = ((HMCaseService)channel).updateReportUrl(new updateReportUrl() { _hmcaseTransRecId = caseTransRecId, _reportPath = reportPath, _reportUrl = reportUrl });
                
                    if (obj.result)
                    {
                        ret = obj.result;
                    }
                    else
                    {
                        throw new ValidationException(CommonRepository.getErrorMessage(obj.Infolog));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }

        public bool updateCaseStatus(string caseId, HMCaseStatus caseStatus)
        {
            bool ret = false;

            //HMCaseServiceClient client = null;            

            //try
            //{
            //    client = new HMCaseServiceClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    ret = client.updateCaseStatus(callContext, caseId, caseStatus);
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

            //return ret;

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMCaseServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;

                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    updateCaseStatusResponse obj = ((HMCaseService)channel).updateCaseStatus(new updateCaseStatus() { _caseGUID = caseId, _caseStatus = caseStatus });

                    if (obj.result)
                    {
                        ret = obj.result;
                    }
                    else
                    {
                        throw new ValidationException(CommonRepository.getErrorMessage(obj.Infolog));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        public HMCaseTransContract getCaseTransDetails(long caseTransRecId)
        {
            HMCaseTransContract contract = new HMCaseTransContract();

            //HMCaseServiceClient client = new HMCaseServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //HMCaseTransContract contract = client.getCaseTransDetails(callContext, caseTransRecId);
            //client.Close();

            //return contract;

            try
            { 
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMCaseServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;

                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    getCaseTransDetailsResponse obj = ((HMCaseService)channel).getCaseTransDetails(new getCaseTransDetails() { _recId = caseTransRecId });

                    if (obj.result != null)
                    {
                        contract = obj.result;
                    }
                    else
                    {
                        throw new ValidationException(CommonRepository.getErrorMessage(obj.Infolog));
                    }
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return contract;
        }

        public long createCaseInsurance(long caseRecId, long patientInsuranceRecId)
        {
            long caseInsuranceRecId = 0;

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMCaseServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;

                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    createCaseInsuranceResponse obj = ((HMCaseService)channel).createCaseInsurance(new createCaseInsurance() { _caseRecId = caseRecId, _patientInsurance = patientInsuranceRecId });

                    if (obj.result > 0)
                    {
                        caseInsuranceRecId = obj.result;
                    }
                    else
                    {
                        throw new ValidationException(CommonRepository.getErrorMessage(obj.Infolog));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return caseInsuranceRecId;
        }        
        
    }
}
