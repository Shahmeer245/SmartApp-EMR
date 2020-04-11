using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MazikCareService.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AppointmentService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AppointmentService.svc or AppointmentService.svc.cs at the Solution Explorer and start debugging.
    public class AppointmentService : IAppointmentService
    {
        public Appointment getAppointmentDetails(string appointmentId)
        {
            Appointment model = new Appointment();
                        
            return model.getAppointmentDetails(appointmentId);
        }
    }
}
