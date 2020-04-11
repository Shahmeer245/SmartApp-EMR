using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
   public class HL7_EventType
    {

        public string EventTypecode
        {
            get;
            set;
        }
        public string RecordedDateTime
        {
            get;
            set;
        }
        public string DateTimeOccurred
        {
            get;
            set;
        }
        public string DateTimePlanneEvent
        {
            get;
            set;
        }
       
    }
}
