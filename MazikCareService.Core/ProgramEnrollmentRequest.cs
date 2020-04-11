using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core
{
    public class ProgramEnrollmentRequest
    {
        public string patientName { get; set; }

        public string id { get; set; }
        public string patientId { get; set; }
        public string patientEncounterId { get; set; }
        public string programId { get; set; }
        public string status { get; set; }
        public string principalDiagnosisId { get; set; }
        public DateTime createdOn { get; set; }
        public string programName { get; set; }
        public string principalDiagnosisName { get; set; }
        public string statusValue { get; internal set; }
    }
}
