using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class PatientVisitTracking
    {
        public string TimeToArrive { get; set; }

        public string ExpectedArrivalTime { get; set; }
        public string WorkOrderId { get; set; }
        public string Status { get; set; }

        public async Task<PatientVisitTracking> getVisitTrackingDetails(string workOrderId)
        {
            PatientVisitTracking ret = new PatientVisitTracking();

            ret.TimeToArrive = "40 minutes";
            ret.ExpectedArrivalTime = "12:30 pm";
            ret.Status = "On the Way";

            return ret;
        }
    }
}
