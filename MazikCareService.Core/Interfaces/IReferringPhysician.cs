using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IReferringPhysician")]
    public interface IReferringPhysician
    {
        [OperationContract]
        List<ReferringPhysician> getReferringPhysician(string SpecialityId = null);
    }
}
