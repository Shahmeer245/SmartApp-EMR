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
    [ServiceContract(Name = "IPatientAllergy")]
    public  interface IPatientAllergy
    {

        [OperationContract]
        Task<List<PatientAllergy>> getPatientAllergies(string patientguid, string SearchFilters, string searchAllergy, DateTime startDate, DateTime endDate, bool OnlyActive = false);

        [OperationContract]
        Task<string> addPatientAllergy(PatientAllergy patientAllergy);

        [OperationContract]
        Task<bool> updatePatientAllergy(PatientAllergy patientAllergy);
                
    }
}
