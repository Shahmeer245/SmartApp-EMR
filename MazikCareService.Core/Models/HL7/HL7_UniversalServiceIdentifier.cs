using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
   public class HL7_UniversalServiceIdentifier
    {
        public string ID
        {
            get;
            set;
        }
        public string Text
        {
            get;
            set;
        }
        public string AlternateIdentifier
        {
            get;
            set;
        }
        public string AlternateText
        {
            get;
            set;
        }
        public string Nameofcodingsystem
        {
            get;
            set;
        }
    }
}
