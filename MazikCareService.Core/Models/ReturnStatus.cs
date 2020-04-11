using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class ReturnStatus
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string name { get; set; }
        public bool fingerprintEnabled { get; set; }
        public bool pinCodeEnabled { get; set; }
        public bool faceRecognitionEnabled { get; set; }
        public bool patientDetailsVerified { get; set; }
        public bool agreeTotermsAndCondition { get; set; }
    }
}
