using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.Integration
{
    public class MessageQuery
    {
        public System.Guid Id { get; set; }
        public System.Guid QueryId { get; set; }
        public System.Guid MessageHeaderId { get; set; }

        public virtual Query Query { get; set; }
        public virtual MessageHeader MessageHeader { get; set; }
    }
}
