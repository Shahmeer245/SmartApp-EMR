using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
   public class HL7_AttendingDoctor
    {
       public string IDNumber { set; get; }
       public string FamilyName { set; get; }
       public string GivenName { set; get; }
       public string Initials { set; get; }
       public string HospitalService  { set; get; }


    }
}
