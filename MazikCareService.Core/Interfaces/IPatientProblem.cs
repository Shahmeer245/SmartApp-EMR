using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IPatientProblem")]
    public interface IPatientProblem
    {
        [OperationContract]
        Task<List<PatientProblem>> getPatientProblems(string patientguid, bool OnlyActive, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate,bool isChronic, int pageNumber = 0);

        [OperationContract]
        Task<string> addPatientProblem(PatientProblem patientProblem);
        [OperationContract]
        Task<bool> updatePatientProblem(PatientProblem patientproblem);
    }
}
