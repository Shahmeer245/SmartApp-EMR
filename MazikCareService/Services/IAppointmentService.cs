using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MazikCareService.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAppointmentService" in both code and config file together.
    [ServiceContract]
    public interface IAppointmentService
    {
        [OperationContract]
        Appointment getAppointmentDetails(string appointmentId);
    }
}
