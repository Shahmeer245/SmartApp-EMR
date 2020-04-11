using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MazikCareService.Core.Interfaces
{
    
    [ServiceContract(Name = "IPatientLabOrder")]
    public interface IPatientLabOrder
    {
        [OperationContract]
        Task<List<PatientLabOrder>> getPatientOrder(string patientguid,string patientEcounter,string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId,string caseId=null, int pageNumber = 0);
        [OperationContract]
        Task<string> addPatientOrder(PatientLabOrder patientLabOrder, bool isActivityOrder = false);
        [OperationContract]
        Task<bool> updatePatientOrder(PatientLabOrder patientLabOrder);
    }
}
