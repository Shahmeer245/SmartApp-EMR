using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class TokenClass
    {
        public string token { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string domain { get; set; }

        public bool isWorker { get; set; }

        public string customerContactId { get; set; }

        public List<string> Previliges { get; set; }
        public string customerAccount { get; set; }

        public string proxyAccountUserWorkerID { get; set; }
    }
}
