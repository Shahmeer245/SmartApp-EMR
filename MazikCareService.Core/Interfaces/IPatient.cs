using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IPatient")]
    interface IPatient
    {
        [OperationContract]
        Task<bool> createAccount(Patient patObject);
    }
}
