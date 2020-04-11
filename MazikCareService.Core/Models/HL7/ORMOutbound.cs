using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
  public  class ORMOutbound
    {
        public string PatientId { set; get; }
        public string OrderId { set; get; }
        public string OrderStatus { set; get; }
        public string Comment { set; get; }
        public NTE NTE { set; get; }
        public string MessageControlId { get; set; }
        public string Location { set; get; }

    }
}
