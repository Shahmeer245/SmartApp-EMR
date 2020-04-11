using MazikCareService.AXRepository;
using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class PatientCaseInsurance
    {
        public async Task<string> createCaseInsurance(string caseId)
        {
            try
            {
                CaseRepository caseRepo = new CaseRepository();                

                string caseInsuranceId = string.Empty;

                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(Incident.EntityLogicalName);

                query.Criteria.AddCondition("incidentid", ConditionOperator.Equal, new Guid(caseId));

                query.ColumnSet = new ColumnSet("incidentid");

                LinkEntity patient = new LinkEntity("incident", "contact", "customerid", "contactid", JoinOperator.Inner);
                patient.Columns = new ColumnSet(false);
                patient.EntityAlias = "patient";

                LinkEntity patientInsurance = new LinkEntity("contact", "mzk_patientinsurancecarrier", "contactid", "mzk_customerid", JoinOperator.Inner);
                patientInsurance.Columns = new ColumnSet("mzk_patientinsurancecarrierid");
                patientInsurance.EntityAlias = "patientInsurance";

                patient.LinkEntities.Add(patientInsurance);
                query.LinkEntities.Add(patient);                

                EntityCollection entityCollection = repo.GetEntityCollection(query);

                if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                {
                    SoapEntityRepository repository = SoapEntityRepository.GetService();

                    Entity caseInsurance = new Entity("mzk_caseinsurancecarrier");

                    if (entityCollection.Entities[0].Attributes.Contains("incidentid"))
                    {
                        caseInsurance["mzk_case"] = new EntityReference("incidentid", new Guid(entityCollection.Entities[0].Attributes["incidentid"].ToString()));
                    }
                    if (entityCollection.Entities[0].Attributes.Contains("patientInsurance.mzk_patientinsurancecarrierid"))
                    {
                        caseInsurance["mzk_patientinsurancecarrier"] = new EntityReference("mzk_patientinsurancecarrier", new Guid((entityCollection.Entities[0].Attributes["patientInsurance.mzk_patientinsurancecarrierid"] as AliasedValue).Value.ToString()));
                    }

                    caseInsuranceId = Convert.ToString(repository.CreateEntity(caseInsurance));
                }

                return caseInsuranceId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
