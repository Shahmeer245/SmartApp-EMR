using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IPatientDiagnosis")]
    public interface IPatientDiagnosis
    {
        [OperationContract]
        Task<string> AddPatientDiagnosis(PatientDiagnosis patientDiagnosis);

        Task<List<PatientDiagnosis>> getPatientDiagnosis(string encountid, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, long appointmentRecId, string caseId);

        [OperationContract]
        Task<bool> updatePatientDiagnosis(PatientDiagnosis patientDiagnosis);
    }
}
