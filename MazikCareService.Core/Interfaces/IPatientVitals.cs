using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{

    [ServiceContract(Name = "IPatientVitals")]
    public  interface IPatientVitals
    {
        [OperationContract]
        Task<List<PatientVitals>> getPatientVitals(string patientguid,bool getGraphValues);

        [OperationContract]
        Task<List<PatientVitals>> getPatientEncounterVitals(string patientguid, string patientEncounter,bool graph,bool getDefault, string casePathwayStateOutcomeStateID = "", bool getUnitList = true);

        [OperationContract]
        Task<bool> AddVitals(List<PatientVitals> patientVitals, bool ignoreDuplicate, bool isGroup);

        [OperationContract]
        Task<bool> UpdateVitalValues(List<PatientVitals> patientVitals);
    }
}
