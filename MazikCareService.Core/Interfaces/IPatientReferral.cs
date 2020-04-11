using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
   
    [ServiceContract(Name = "IPatientReferral")]
    public interface IPatientReferralOrder
    {
        [OperationContract]
        Task<List<PatientReferralOrder>> getPatientOrder(string patientguid, string patientEncounter, string SearchFilters, string searchOrder, DateTime startDate, DateTime endDate, bool forFulfillment, string orderId,string caseId= null, int pageNumber = 0, string fulfillmentAppointmentId = null, string orderingAppointmentId = null);

        [OperationContract]
        Task<string> addPatientOrder(PatientReferralOrder _patientReferral);

        [OperationBehavior]
        Task<bool> updatePatientOrder(PatientReferralOrder patientReferral); 
    }
}
