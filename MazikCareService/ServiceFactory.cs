using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazikCareService.Core.Interfaces;
using Helper;

namespace MazikCareService
{
    ///  public class ServiceFactory<T> where T : IMazikCareServiceChannel

    public class ServiceFactory
    {
       // private static IMazikCareServiceChannel _service;
        //public ServiceFactory()
        //{
        //    _service = new IMazikCareServiceChannel();
        //}
        public static IMazikCareServiceChannel GetService(Type serviceType)
        {
            if (AppSettings.GetByKey("AzureBus") == "true")
            {
                return ServiceBusFactory.GetService();
            }
            else
            {
                IMazikCareServiceChannel instance = Activator.CreateInstance<IMazikCareServiceChannel>();
            
               // IMazikCareServiceChannel instance = (IMazikCareServiceChannel)Activator.CreateInstance(serviceType);
                return instance;
            }
        }

        //private  IMazikCareServiceChannel GetIISService(Type serviceType)
        //{
        //    return (IMazikCareServiceChannel)serviceType;
        //}
    }
}
