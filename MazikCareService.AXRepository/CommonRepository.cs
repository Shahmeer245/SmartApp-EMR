
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
    public class CommonRepository
    {
        public string getDocumentsBase64(long docuRefRecId)
        {
            string file = string.Empty;

            //HMCommonClient client = new HMCommonClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };
            
            //string file = client.getDocumentsBase64(callContext, docuRefRecId);

            //client.Close();

            //return file;

            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMCommonClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    getDocumentsBase64Response obj = ((HMCommon)channel).getDocumentsBase64(new getDocumentsBase64() { docuRefRecId = docuRefRecId });

                    if (!string.IsNullOrEmpty(obj.result))
                    {
                        file = obj.result;
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

            return file;
        }

        public Dictionary<int, int> checkItemInStock(string itemId, long clinicRecId)
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();

            //HMCommonClient client = new HMCommonClient();

            //CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //Dictionary<int, int> ret = client.checkItemInStock(callContext, itemId, clinicRecId);

            //client.Close();

            //return ret;



            try
            {
                var endpointAddress = SoapHelper.GetEndPointAddress();
                var binding = SoapHelper.GetBinding();

                var client = new HMCommonClient(binding, endpointAddress);
                var channel = client.InnerChannel;
                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    checkItemInStockResponse obj = ((HMCommon)channel).checkItemInStock(new checkItemInStock() { _clinicRecId = clinicRecId, _itemId = itemId });

                    if (obj.result.Length > 0)
                    {
                        string[] splitString = obj.result[0].Split(',');
                        int i = 0;
                        foreach(string s in splitString)
                        {
                            int j = 0;
                            if (int.TryParse(s, out j))
                            {
                                ret.Add(i, j);
                                i++;
                            }
                        }
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

        public bool sendPatientSms(long patientRecId, string smsMessage)
        {
            bool ret = false;

            //HMCommonClient client = new HMCommonClient();            

            //try
            //{
            //    client = new HMCommonClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    ret = client.sendPatientSms(callContext, patientRecId, smsMessage);
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

                var client = new HMCommonClient(binding, endpointAddress);
                var channel = client.InnerChannel;

                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    sendPatientSmsResponse obj = ((HMCommon)channel).sendPatientSms(new sendPatientSms() { _patientRecId = patientRecId, _sms = smsMessage });

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

        public bool sendResourceSms(long resoureRecId, string smsMessage)
        {
            bool ret = false;

            //HMCommonClient client = new HMCommonClient();            

            //try
            //{
            //    client = new HMCommonClient();

            //    CallContext callContext = new CallContext { Company = AppSettings.GetByKey("axCompany") };
            //    client.ClientCredentials.Windows.ClientCredential = new NetworkCredential { Domain = AppSettings.GetByKey("axDomain"), UserName = AppSettings.GetByKey("axUserName"), Password = AppSettings.GetByKey("axPassword") };

            //    ret = client.sendResourceSms(callContext, resoureRecId, smsMessage);
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

                var client = new HMCommonClient(binding, endpointAddress);
                var channel = client.InnerChannel;

                using (OperationContextScope operationContextScope = new OperationContextScope(channel))
                {
                    SoapHelper.channelHelper();

                    sendResourceSmsResponse obj = ((HMCommon)channel).sendResourceSms(new sendResourceSms() { _resourceRecId = resoureRecId, _sms = smsMessage });

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

        public static string getErrorMessage(Infolog result)
        {
            string errorMessage = "";

            foreach (InfologEntry infoLogEntry in result.Entries)
            {
                if (errorMessage != "")
                {
                    errorMessage += "\n" + infoLogEntry.Message;
                }
                else
                {
                    errorMessage = infoLogEntry.Message;
                }
            }

            return errorMessage;
        }
    }


}
