using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class HL7_AssignedPatientLocation
    {
        public string PointOfCare { set; get; }
        public string Room { set; get; }
        public string Bed { set; get; }
        public string AdmissionType  { set; get; }

    }
}
