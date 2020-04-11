using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class HL7_OBR
    {
        public string ID
        {
            get;
            set;
        }
        public HL7_OrderNumber PlacerOrderNumber
        {
            get;
            set;
        }
        public HL7_OrderNumber FillerOrderNumber
        {
            get;
            set;
        }
        public HL7_UniversalServiceIdentifier UniversalServiceIdentifier
        {
            get;
            set;
        }
        public HL7_Provider OrderingProvider
        {
            get;
            set;
        }
        public string PlacerField2
        {
            get;
            set;
        }
        public string PlacerField1
        {
            get;
            set;
        }
        public string FillderField1
        {
            get;
            set;
        }
        public string ResultStatusCode
        {
            get;
            set;
        }
        public string StartDateTime
        {
            get;
            set;
        }
        public string EndDateTime
        {
            get;
            set;
        }
        public string ReasonforStudy
        {
            get;
            set;
        }
        public string RelevantClinicalInf
        {
            get;
            set;
        }
        public string ScheduledDateTime
        {
            get;
            set;
        }
        public string Accessionnumber
        {
            set; get;

        }
        public string Priority { set; get; }
        public string OrderLocation { set; get; }
        public string DiagnosticServSectId { set; get; }



    }
}
