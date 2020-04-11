using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MazikCareWebApi.Models
{
    public class WorkListApiModel : ApiModel
    {
        public string worklistTypeID { get; set; }

        public string userId { get; set; }

        public string clinicId { get; set; }

        public long resourceRecId { get; set; }
    }
}