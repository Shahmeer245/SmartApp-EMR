using MazikCareService.AXRepository.AXServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MazikLogger;
using System.ServiceModel;

namespace MazikCareService.AXRepository
{
    public class AppointmentRepository
    {
        public HMAppointmentContract[] GetUserAppointments(long resourceRecId, long clinicRecId, DateTime startDate, DateTime endDate, bool onlyTriaged, string searchOrder)
        {
            //HMAppointmentSchedulingServiceClient client = new HMAppointmentSchedulingServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain") , UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //HMAppointmentContract[] contract = client.getUserAppointments(callContext, resourceRecId, clinicRecId, startDate, endDate, onlyTriaged, searchOrder);
            //client.Close();

            //return contract;

            //HMAppointmentContract[] contract = null;

            //try
            //{
            //    var endpointAddress = SoapHelper.GetEndPointAddress();
            //    var binding = SoapHelper.GetBinding();

            //    var client = new HMAppointmentSchedulingServiceClient(binding, endpointAddress);
            //    var channel = client.InnerChannel;

            //    using (OperationContextScope operationContextScope = new OperationContextScope(channel))
            //    {
            //        SoapHelper.channelHelper();

            //        sendPatientSmsResponse obj = ((HMAppointmentSchedulingService)channel).getUserAppointments(new getUserAppointments() { _patientRecId = patientRecId, _sms = smsMessage });

            //        if (obj.result != null)
            //        {
            //            contract = obj.result;
            //        }
            //        else
            //        {
            //            throw new ValidationException(CommonRepository.getErrorMessage(obj.Infolog));
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            return null;
        }

        public HMAppointmentContract GetAppointmentDetails(long appointmentRefRecId)
        {
            //HMAppointmentSchedulingServiceClient client = new HMAppointmentSchedulingServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //HMAppointmentContract contract = client.getAppointmentDetails(callContext, appointmentRefRecId);
            //client.Close();

            //return contract;

            HMAppointmentContract contract = new HMAppointmentContract();

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMAppointmentSchedulingServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;

                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    getAppointmentDetailsResponse obj = ((HMAppointmentSchedulingService)channel).getAppointmentDetails(new getAppointmentDetails() { _appointmentRecId = appointmentRefRecId });

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

        public bool IsAppointmentProgressive(long appointmentRefRecId)
        {
            //HMAppointmentSchedulingServiceClient client = new HMAppointmentSchedulingServiceClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //bool contract = client.isAppointmentProgressive(callContext, appointmentRefRecId);
            //client.Close();

            //return contract;

            bool ret = false;

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMAppointmentSchedulingServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;

                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    isAppointmentProgressiveResponse obj = ((HMAppointmentSchedulingService)channel).isAppointmentProgressive(new isAppointmentProgressive() { _appointmentRecId = appointmentRefRecId });

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

        public bool updateDispositionDetails(long appointmentRecId, string comments, string followUp)
        {
            bool ret = false;

            //HMAppointmentSchedulingServiceClient client = null;            

            //try
            //{
            //    client = new HMAppointmentSchedulingServiceClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    ret = client.updateDispositionDetails(callContext, appointmentRecId, comments, followUp);
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

                var client = new HMAppointmentSchedulingServiceClient(binding, endpointAddress);
                var channel = client.InnerChannel;

                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    updateDispositionDetailsResponse obj = ((HMAppointmentSchedulingService)channel).updateDispositionDetails(new updateDispositionDetails() { _appointmentRecId = appointmentRecId, _dispositionComments = comments, _dispositionFollowUp = followUp });

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
    }
}
