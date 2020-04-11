using MazikCareService.Core.Models;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
 
  
    [ServiceContract(Name = "ICrmService")]
    public interface ICrmService
    {
        [OperationContract]
        Task<List<User>> GetUsers();

        [OperationContract]
        Task<User> GetUser(string id);

        [OperationContract]
        Task<Models.User> GetUserByDomain(string DomainName,string value);

        [OperationContract]
        Task<Models.Organization> GetOrganization(string id);


    }
    public  interface IMazikCareServiceChannel : IClientChannel, ICrmService
    {

    }

}
