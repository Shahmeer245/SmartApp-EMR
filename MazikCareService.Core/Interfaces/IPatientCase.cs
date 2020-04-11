using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IPatientCase")]
    public interface IPatientCase
    {
        [OperationContract]
        Task<PatientCase> addPatientCase(string patientid, long patientRecId, int caseType, string clinicRecId);

        [OperationContract]
        Task<List<PatientCase>> getPatientCase(string patientid);
    }
}
