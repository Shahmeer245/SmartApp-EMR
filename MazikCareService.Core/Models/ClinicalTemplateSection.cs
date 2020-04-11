using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class ClinicalTemplateSection
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }

        public int historyType { get; set; }

        public bool history { get; set; }
        public bool showHeading
        {
            get; set;
        }

        public List<ClinicalTemplateQuestion> questions
        {
            get; set;
        }
    }
}
