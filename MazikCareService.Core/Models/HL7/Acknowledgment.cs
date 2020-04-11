using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
   public class Acknowledgment
    {      
        public string Code { get; set; }
        public string MessageControlId { get; set; }
        public string TextMessage { get; set; }
        public string ApplicationErrorIdentifier { set; get; }
        public string ApplicationErrorMsg { set; get; }
        public mzk_acknowledgecode acknowledgmentCode { get; set; }
    }
}
