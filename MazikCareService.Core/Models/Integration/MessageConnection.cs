using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.Integration
{
    public class MessageConnection
    {
        public Guid Id { get; set; }
        public string ConnectionName { get; set; }
        public int ToolId { get; set; }
        public string ToolName { get; set; }
        public int ProtocolId { get; set; }
        public string ProtocolName { get; set; }

        public string ConnectionDetails { get; set; }
    }
}
