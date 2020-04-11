using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IPatientRadiologyOrder")]
    public interface IPatientRadiologyOrder
    {
        [OperationContract]
        Task<List<PatientRadiologyOrder>> getPatientOrder(string patientguid,string patientEcounter,string SearchFilters,string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId,string caseId=null, bool fromRIs = false, int pageNumber = 0);
        [OperationContract]
        Task<string> addPatientOrder(PatientRadiologyOrder patientRadiologyOrder,bool isActivityOrder = false);
        [OperationContract]
        Task<bool> updatePatientOrder(PatientRadiologyOrder patientRadiologyOrder);
    }
}
