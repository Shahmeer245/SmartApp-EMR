using Helper;
using MazikCareService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareWebApi
{
    public class ServiceFactory
    {
        public static dynamic GetService(Type serviceType)
        {
            if (AppSettings.GetByKey("AzureBus") == "true")
            {
                return ServiceBusFactory.GetService();
            }
            else
            {
                var instance = Activator.CreateInstance(serviceType);
                return instance;
            }
        }
    }
}
