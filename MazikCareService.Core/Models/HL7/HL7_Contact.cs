using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
   public  class HL7_Contact
    {
       public string TelephoneNumber { set; get; }
       public string TelecommunicatorUsecode { set; get; }
       public string TelecommunicatorEquipmentType  { set; get; }

    }
}
