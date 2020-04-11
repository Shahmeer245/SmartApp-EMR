using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    [ServiceContract(Name = "IPatientDisposition")]
    public interface IPatientDisposition
    {
        [OperationContract]
        Task<List<PatientDisposition>> getPatientDisposition(string patientEncounter, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate);

        [OperationContract]
        Task<string> AddPatientDisposition(PatientDisposition patientDisposition);

        [OperationContract]
        Task<bool> updatePatientDisposition(PatientDisposition patientDisposition);
    }
}
