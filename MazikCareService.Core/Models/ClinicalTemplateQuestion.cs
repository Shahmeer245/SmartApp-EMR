using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class ClinicalTemplateQuestion
    {
        public string SecId { get; set; }
        public string question { get; set; }

        public string measurementId { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string SubSecId { get; set; }
        public string templateId { get; set; }
        public string ansIn { get; set; }
        public bool isParagraph
        {
            get; set;
        }
        public int soapType
        {
            get; set;
        }
        public int responseType
        {
            get; set;
        }

        public ClinicalTemplateNarration multiSelect { get; set; }

        public ClinicalTemplateNarration yes { get; set; }

        public ClinicalTemplateNarration no { get; set; }

        public ClinicalTemplateNarration noSelection { get; set; }
    }
}
