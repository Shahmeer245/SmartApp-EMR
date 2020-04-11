using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.CRMRepository.Entities
{
   public class AccountBase:EntityBase
    {
        public string name { get; set; }
        public string description { get; set; }
        public bool creditonhold { get; set; }
        public float address1_latitude { get; set; }
        public int revenue { get; set; }
        public int accountcategorycode { get; set; }
    }
}
