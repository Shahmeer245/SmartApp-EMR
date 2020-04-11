using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
 public   class LIS_OrderResult
    {
        public string PatientId { set; get; }
        public string OrderId { set; get; }
        public string ReportPath { set; get; }
        public string OrderStatus { set; get; }
        public string ResultStatus { set; get; }
        public string ObservationResultStatus { set; get; }
        public string Priority { set; get; }
        public string MessageControlId { get; set; }
        public string ImageUrl { set; get; }
        public string AbnormalResult { set; get; }
    }
}
