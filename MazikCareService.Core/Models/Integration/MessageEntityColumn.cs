using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.Integration
{
    public class MessageEntityColumn
    {
        public System.Guid Id { get; set; }
        public System.Guid MessageEntityId { get; set; }
        public string ColumnName { get; set; }
        public string FieldType { get; set; }
        public Nullable<bool> IsMandatory { get; set; }
        public string Value { get; set; }

        public virtual MessageEntity MessageEntity { get; set; }
    }
}
