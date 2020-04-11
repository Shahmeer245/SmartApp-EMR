using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IClinic")]
    public interface IClinic
    {
        [OperationContract]
        Task<List<Clinic>> getUserClinicsTree(string resourceId);
    }
}
