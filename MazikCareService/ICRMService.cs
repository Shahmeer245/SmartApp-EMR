using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICRMService" in both code and config file together.
    [ServiceContract]
    public interface ICRMService
    {
        [OperationContract]
        Task<List<Role>> GetRoles();

        string GetData();
    }
}
