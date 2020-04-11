using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Interfaces
{
    [ServiceContract(Name = "IPatientEncounter")]
    public interface IPatientEncounter
    {
        [OperationContract]
        Task<string> addPatientEncounter(string CaseId, int encounterType, string worklistTypeID, long resourceRecId, string cpsaWorkflowId, string resourceId, string patientId, string appoitmentId);

        [OperationContract]
        Task<List<UITemplate>> getEncounterTemplate(string encounterTemplateId, string cpsaWorkflowId, string patientId);

        [OperationContract]
        Task<bool> updateEncounter(string encounterId , string key, string userId, bool isAutoSignoff = false);
    }
}
