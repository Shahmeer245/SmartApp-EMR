using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "INotes")]
    interface INotes
    {
        [OperationContract]
        Task<List<Notes>> getNotes(Notes notes);

        [OperationContract]
        Task<string> AddNotes(Notes  notes);

        [OperationContract]
        Task<bool> updateNotes(Notes notes);
    }
}
