using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class NTE
    {
        public string Code { get; set; }
        public string CommentText { get; set; }
        public string CommentDescription { get; set; }
    }
}
