using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
   public class HL7_EnteringOrganization
    {
        public string ID
        {
            get;
            set;
        }
        public string Nameofcodingsystem
        {
            get;set;
        }
        public string Text
        {
            get; set;
        }
    }
}
