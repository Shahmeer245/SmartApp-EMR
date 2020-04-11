using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "ICheifComplaint")]
    public interface ICheifComplaint
    {
        [OperationContract]
        Task<List<CheifComplaint>> getCheifComplaint(string patientguid, string encounterId, long appointmentRecId, string caseId);

        [OperationContract]
        Task<bool> updateCheifComplaint(string encounterId, string CheifComplaint);
    }
}
