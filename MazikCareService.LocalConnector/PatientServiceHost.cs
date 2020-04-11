using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.LocalConnector
{
   
    public class PatientServiceHost : ServiceHost, IErrorHandler
    {
        #region CTOR
        public PatientServiceHost(Type serviceType) : base(serviceType)
        {

        }
        #endregion

        #region IErrorHandler Methods
        public bool HandleError(Exception error)
        {
            Console.WriteLine(error.ToString());

            if (!(error is FaultException))
                UnhandledExceptionHandler(error);
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {

        }
        #endregion

        #region Private Helper
        void UnhandledExceptionHandler(Exception ex)
        {
            Console.WriteLine(ex.ToString());

            Close();
            Environment.Exit(1);
        }
        #endregion
    }


}
