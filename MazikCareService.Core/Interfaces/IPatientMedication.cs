using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IPatientMedication")]
    public interface IPatientMedication
    {
        [OperationContract]
        Task<List<PatientMedication>> getPatientOrder(string patientguid, string patientEncounter,string filter, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId, string caseId = null, int pageNumber = 0, bool forHistory = false, bool checkLogs = false);

        [OperationContract]
        Task<string> addPatientOrder(PatientMedication patientMedication, bool isActivityOrder = false);

        [OperationContract]
        Task<bool> updatePatientOrder(PatientMedication patientMedication);

        [OperationContract]
        Task<bool> deleteMedicationDose(string Id);
    }
}
