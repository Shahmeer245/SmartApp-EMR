using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
   public class HL7_NTE
    {
        public string SetID { set; get; }
        public string Comment { set; get; }
        public string CommentType { set; get; }
        public string Identifier { set; get; }
    }
}
