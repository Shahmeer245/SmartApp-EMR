using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class HL7_Address
    {
        public string addressType
        {
            get; set;
        }

        public string city
        {
            get; set;
        }

        public string country
        {
            get; set;
        }

        public string zipCode
        {
            get; set;
        }

        public string street
        {
            get; set;
        }

        public string state
        {
            get; set;
        }
    }
}
