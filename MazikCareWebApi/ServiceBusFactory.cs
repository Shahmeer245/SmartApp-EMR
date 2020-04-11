using MazikCareService.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareWebApi
{
    public class ServiceBusFactory
    {
        static ChannelFactory<IMazikCareServiceChannel> channelFactory;

        static ServiceBusFactory()
        {
            // Create shared secret token credentials for authentication
            /*channelFactory = new ChannelFactory<IMazikCareServiceChannel>(new NetTcpRelayBinding(), ConfigurationManager.AppSettings["relayName"]);
            channelFactory.Endpoint.Behaviors.Add(new TransportClientEndpointBehavior
            {
                TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                    "RootManageSharedAccessKey", ConfigurationManager.AppSettings["accessKey"])
            });*/
        }

        public static IMazikCareServiceChannel GetService()
        {
            return channelFactory.CreateChannel();
        }
    }
}
