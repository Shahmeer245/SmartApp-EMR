using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class PatientOrderAdministration
    {
        public string administrationStatus { get; set; }
        public string quantityAdministered { get; set; }
        public bool selfAdministered { get; set; }
        public string skipReason { get; set; }
        public DateTime startDateTime { get; set; }
        public string unit { get; set; }

        public async  Task<List<PatientOrderAdministration>> getPatientOrderLog(string patientOrderId)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientOrderId))
                {
                    SoapEntityRepository repo = SoapEntityRepository.GetService();
                    List<PatientOrderAdministration> patientOrderAdministrations = new List<PatientOrderAdministration>();
                    QueryExpression query = new QueryExpression("mzk_patientorderadministration");
                    query.Criteria.AddCondition("mzk_patientorder", ConditionOperator.Equal, new Guid(patientOrderId));
                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_administrationstatus", "mzk_qtyadministered", "mzk_selfadministered", "mzk_skipreason", "mzk_starttime", "mzk_unit");
                    EntityCollection entityCollection = repo.GetEntityCollection(query);
                    foreach (Entity entity in entityCollection.Entities)
                    {
                        PatientOrderAdministration patientOrderAdministration = new PatientOrderAdministration();
                        if (entity.Attributes.Contains("mzk_administrationstatus"))
                            patientOrderAdministration.administrationStatus = entity.FormattedValues["mzk_administrationstatus"].ToString();
                        if (entity.Attributes.Contains("mzk_qtyadministered"))
                            patientOrderAdministration.quantityAdministered = entity["mzk_qtyadministered"].ToString();
                        if (entity.Attributes.Contains("mzk_selfadministered"))
                            patientOrderAdministration.selfAdministered = (bool)entity["mzk_selfadministered"];
                        if (entity.Attributes.Contains("mzk_skipreason"))
                            patientOrderAdministration.skipReason = (entity["mzk_skipreason"] as EntityReference).Name;
                        if (entity.Attributes.Contains("mzk_starttime"))
                            patientOrderAdministration.startDateTime = Convert.ToDateTime(entity["mzk_starttime"]);
                        if (entity.Attributes.Contains("mzk_unit"))
                            patientOrderAdministration.unit = (entity["mzk_unit"] as EntityReference).Name;
                        patientOrderAdministrations.Add(patientOrderAdministration);
                    }
                    return patientOrderAdministrations;
                }
                else
                {
                    throw new ValidationException("PatientOrder Id not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
