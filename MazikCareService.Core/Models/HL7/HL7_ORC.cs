using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class HL7_ORC
    {
        public string OrderControlCode
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
        public string PlacerGroupNumber
        {
            get;
            set;
        }
        public string OrderStatus
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
        public string TransactionDateTime
        {
            get;
            set;
        }
        public HL7_Provider EnteredBy
        {
            get;
            set;
        }
        public HL7_Provider OrderingProvider
        {
            get;
            set;
        }
        public HL7_EnteringOrganization EnteringOrganization
        {
            set; get;
        }
        public HL7_OrderingFacility OrderingFacility
        {
            get;
            set;
        }


    }
}
