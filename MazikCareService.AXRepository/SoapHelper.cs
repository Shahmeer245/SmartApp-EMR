using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Helper;

namespace MazikCareService.AXRepository
{
    
        public class SoapHelper
        {
            public static string GetSoapServiceUriString(string serviceName, string aosUriString)
            {
                var soapServiceUriStringTemplate = "{0}/soap/services/{1}";
                var soapServiceUriString = string.Format(soapServiceUriStringTemplate, aosUriString.TrimEnd('/'), serviceName);
                return soapServiceUriString;
            }

            public static EndpointAddress GetEndPointAddress()
            {
                var serviceUriString = SoapHelper.GetSoapServiceUriString(AppSettings.GetByKey("OperationsServiceGroupName"), AppSettings.GetByKey("OperationsUriString"));

                var endpointAddress = new System.ServiceModel.EndpointAddress(serviceUriString);

                return endpointAddress;
            }

            public static Binding GetBinding()
            {
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);

                // Set binding timeout and other configuration settings
                binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;

                binding.ReceiveTimeout = TimeSpan.MaxValue;
                binding.SendTimeout = TimeSpan.MaxValue;
                binding.MaxReceivedMessageSize = int.MaxValue;

                var httpsTransportBindingElement = binding.CreateBindingElements().OfType<HttpsTransportBindingElement>().FirstOrDefault();
                if (httpsTransportBindingElement != null)
                {
                    httpsTransportBindingElement.MaxPendingAccepts = 10000; // Largest posible is 100000, otherwise throws
                }

                var httpTransportBindingElement = binding.CreateBindingElements().OfType<HttpTransportBindingElement>().FirstOrDefault();
                if (httpTransportBindingElement != null)
                {
                    httpTransportBindingElement.MaxPendingAccepts = 10000; // Largest posible is 100000, otherwise throws
                }

                return binding;
            }

            //public static IClientChannel getBindingChannel()
            //{
            //    var authenticationHeader = OAuthHelper.GetAuthenticationHeader();
            //    var endpointAddress = SoapHelper.GetEndPointAddress();
            //    var binding = SoapHelper.GetBinding();

            //    var client = new HMPatientServiceClient(binding, endpointAddress);

            //    var channel = client.InnerChannel;
            //    //UserSessionInfo sessionInfo = null;
            //    using (OperationContextScope operationContextScope = new OperationContextScope(channel))
            //    {
            //        HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
            //        requestMessage.Headers[OAuthHelper.OAuthHeader] = authenticationHeader;
            //        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
            //        var result = ((HMPatientService)channel).getPatientBasicDetails(new getPatientBasicDetails() { _patientRecId = "0" }).result;
            //    }

            //    return channel;
            //}

            public static void channelHelper()
            {

                var authenticationHeader = OAuthHelper.GetAuthenticationHeader();
                HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers[OAuthHelper.OAuthHeader] = authenticationHeader;
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;

            }


        }
    }


