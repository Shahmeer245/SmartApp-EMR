using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.Integration
{
    public class MessageEntity
    {
        public System.Guid Id { get; set; }
        public string EntityName { get; set; }
        public string LinkEntity { get; set; }
        public Nullable<int> JoinType { get; set; }
        public Nullable<int> Cardinality { get; set; }
        public System.Guid QueryId { get; set; }

        public virtual Query Query { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MessageEntityColumn> MessageEntityColumns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MessageCondition> MessageConditions { get; set; }
    }
}
