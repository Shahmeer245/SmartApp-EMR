using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.Integration
{
    public class MessageCondition
    {
        public System.Guid Id { get; set; }
        public string LogicalOperator { get; set; }
        public string ConditionalOperator { get; set; }
        public string Value { get; set; }
        public System.Guid MessageEntityId { get; set; }
        public string Field { get; set; }

        public virtual MessageEntity MessageEntity { get; set; }
    }
}
