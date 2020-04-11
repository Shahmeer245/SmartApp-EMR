using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class PowerBIReport
    {
        public string webUrl
        {
            get; set;
        }

        public string embeddedUrl
        {
            get; set;
        }

        public string reportId
        {
            get; set;
        }
        public string reportName
        {
            get; set;
        }

        public string isDefault
        {
            get; set;
        }
    }
}
