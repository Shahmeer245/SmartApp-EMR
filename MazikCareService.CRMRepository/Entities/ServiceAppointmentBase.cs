using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.CRMRepository.Entities
{
    public class ServiceAppointmentBase : EntityBase
    {
        public string scheduledstart { get; set; }
        public string subject { get; set; }
        public int scheduleddurationminutes { get; set; }
        public string createdon { get; set; }
    }
}
