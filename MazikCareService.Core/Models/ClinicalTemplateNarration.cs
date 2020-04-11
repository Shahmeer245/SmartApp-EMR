using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class ClinicalTemplateNarration
    {
        public int type { get; set; }
        public string narrativeText { get; set; }
        public string Id { get; set; }
        public string ans { get; set; }
        public string ansIn { get; set; }
        public string patientFindingId { get; set; }
        public string templateId { get; set; }
        public string[] ansList { get; set; }
        public string[] choicesName { get; set; }
        public string comments { get; set; }
        public string file { get; set; }
        public string mimeType { get; set; }
        public int historyType { get; set; }
        public DateTime createdDateTime { get; set; }
        public string userName { get; set; }
        public List<NarrationChoices> choices
        {
            get; set;
        }
        public string mzk_casepathwaystateoutcomeStr { get; set; }

        public string measurementId { get; set; }
        public int narrationType { get; set; }

    }

    public class NarrationChoices
    {
        public string Id { get; set; }
        public string name { get; set; }

        public string Image { get; set; }
    }
}
