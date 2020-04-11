using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class StatusManagerParams
    {
        public string comments
        {
            get; set;
        }
        public string location
        {
            get; set;
        }

        public string batchNumber
        {
            get; set;
        }

        public DateTime expiryDate
        {
            get; set;
        }
    }
}
