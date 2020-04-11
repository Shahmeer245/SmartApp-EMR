using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
   
    [ServiceContract(Name = "IPateintProcedure")]
    public interface IPateintProcedure
    {
        [OperationContract]
        Task<List<PatientProcedure>> getPatientOrder(string patientguid, string patientEncounter ,string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId,string caseId, bool isCancel = true, int pageNumber = 0);

        [OperationContract]
        Task<string> addPatientOrder(PatientProcedure patientProcedure, bool isActivityOrder = false);

        [OperationContract]
        Task<bool> updatePatientOrder(PatientProcedure patientProcedure);
    }
}
