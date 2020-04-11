using System.Collections.Generic;
using System.Linq;

namespace MazikCareService.Core.Models
{
    public class LoginModelOutput
    {
        public string token { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public string resourceRecId { get; set; }
        public string resourceId { get; set; }
        public string designation { get; set; }

        public string organization { get; set; }

        public long smsCodeId { get; set; }
        public bool doLogout { get; set; }
        public string fullName { get; set; }
        public string image { get; set; }

        public string errorMessage { get; set; }
        public bool success { get; set; }
        public List<string> privileges { get; set; }
    }
}