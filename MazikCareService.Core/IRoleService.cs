using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core
{
    [ServiceContract]
    public interface IRoleService
    {
       [OperationContract]
       Task<List<Role>> GetRoles(); 
    }
    //public interface IMazikCareServiceChannel : IClientChannel, IRoleService
    //{

    //}
}
