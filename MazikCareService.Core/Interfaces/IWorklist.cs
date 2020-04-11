using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IWorklist")]
    public interface IWorklist
    {
        [OperationContract]
        Task<List<Worklist>> getWorklistTypes(string userId, bool fetchByRole = false);

        [OperationContract]
        Task<List<Activity>> getUserWorkListData(string worklistTypeID, string userId, string clinicId, DateTime date, string SearchFilters, string searchOrder, string timezone, string resourceId);

    }
}
