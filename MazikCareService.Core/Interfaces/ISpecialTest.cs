using MazikCareService.Core.Models;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    interface ISpecialTest
    {
    }

    [ServiceContract(Name = "IPatientSpecialTest")]
    public interface IPatientSpecialTest
    {
        [OperationContract]
        Task<List<PatientSpecialTest>> getPatientOrder(string patientguid, string patientEncounter,string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId,string caseId=null, int pageNumber = 0);

        [OperationContract]
        Task<string> addPatientOrder(PatientSpecialTest patientSpecialTest, bool isActivityOrder = false);

        QueryExpression getSpecialTestQuery(string patientguid, string patientEncounter, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId,string caseId=null, int pageNumber = 0);

        [OperationContract]
        Task<bool> updatePatientOrder(PatientSpecialTest patientSpecialTest);

    }
}
