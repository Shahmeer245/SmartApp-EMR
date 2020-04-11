using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Models.HL7;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class Claim
    {
        public string mrn
        {
            get; set;
        }

        public string claimid
        {
            get; set;
        }

        public string validfrom
        {
            get; set;
        }

        public string validto
        {
            get; set;
        }

        public string total
        {
            get; set;
        }

        public async Task<bool> createClaim(Claim claimObject)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                Entity claim = new Entity("mzk_claim");

                //claim["mzk_axrefrecid"] = TODO for update case.
                claim["mzk_claimnumber"] = claimObject.claimid;
                claim["mzk_billableperiodstart"] = Convert.ToDateTime(claimObject.validfrom);
                claim["mzk_billableperiodend"] = Convert.ToDateTime(claimObject.validto);
                claim["mzk_total"] = Convert.ToInt32(claimObject.total);
                //claim["2"] = claimObject.mrn;
                string patientID = "cab1e473-82a7-e811-a83b-000d3a37c0db";//FHIR PLUS  "c97ec3eb-91a8-e811-a958-000d3a3ab029"; // James David
                EntityReference patientEntity = new EntityReference("contact", new Guid(patientID));
                claim["mzk_patient"] = patientEntity;
                claim["mzk_status"] = new OptionSetValue(935000000); //Active
                //string type = "codeable concept GUID";
                //EntityReference codeableconceptEntity = new EntityReference("msemr_codeableconcept", new Guid(type));
                //claim["mzk_type"] = codeableconceptEntity.Id;
                claim["mzk_use"] = new OptionSetValue(275380001); //Proposed
                string mzk_enterer = "235db0b8-99a7-e811-a83b-000d3a37c0db";//"18ae0176-97a8-e811-a958-000d3a3ab029"; // Usama Aziz
                EntityReference mzk_entererEntity = new EntityReference("contact", new Guid(mzk_enterer));
                claim["mzk_enterer"] = mzk_entererEntity;
                string mzk_provider = "34e6c1be-99a7-e811-a83b-000d3a37c0db";// "e7e95b25-9ba8-e811-a958-000d3a3ab029"; // Sara Green
                EntityReference mzk_providerEntity = new EntityReference("contact", new Guid(mzk_provider));
                claim["mzk_provider"] = mzk_providerEntity;
                string mzk_insurer = "d0b1e473-82a7-e811-a83b-000d3a37c0db";// "fe4904c0-9ba8-e811-a958-000d3a3ab029"; // AZ Insurance
                EntityReference mzk_insurerEntity = new EntityReference("account", new Guid(mzk_insurer));
                claim["mzk_insurer"] = mzk_insurerEntity;
                string mzk_organization = "2a5db0b8-99a7-e811-a83b-000d3a37c0db";// "57444ccd-9ba8-e811-a958-000d3a3ab029"; // AZ corp
                EntityReference mzk_organizationEntity = new EntityReference("account", new Guid(mzk_organization));
                claim["mzk_organization"] = mzk_organizationEntity;

                claim["mzk_priority"] = new OptionSetValue(275380000); //Immediate

                claim["mzk_payeeresourcetype"] = new OptionSetValue(275380001); //Patient
                claim["mzk_payeepartytype"] = new OptionSetValue(275380002); //Patient

                claim["mzk_payeepartytypepatient"] = patientEntity;

                //string mzk_payeetype = "codeable concept GUID";
                //EntityReference mzk_payeetypetEntity = new EntityReference("msemr_codeableconcept", new Guid(mzk_payeetype));
                //claim["mzk_payeetype"] = mzk_payeetypetEntity.Id;

                Guid claimGuid = repo.CreateEntity(claim);

                EntityReference claimEntityReference = new EntityReference("mzk_claim", claimGuid);

                Entity mzk_claimdiagnosis = new Entity("mzk_claimdiagnosis");
                mzk_claimdiagnosis["mzk_claim"] = claimEntityReference;

                //string mzk_diagnosistypecodeableconcept = "codeable concept GUID";
                //EntityReference mzk_diagnosistypecodeableconceptEntityRef = new EntityReference("msemr_codeableconcept", new Guid(mzk_diagnosistypecodeableconcept));
                //mzk_claimdiagnosis["mzk_diagnosistypecodeableconcept"] = mzk_diagnosistypecodeableconceptEntityRef.Id;

                mzk_claimdiagnosis["mzk_diagnosistype"] = new OptionSetValue(275380000); //Codeable concept

                mzk_claimdiagnosis["mzk_diagnosissequence"] = 1; 

                repo.CreateEntity(mzk_claimdiagnosis);

                Entity mzk_claimprocedure = new Entity("mzk_claimprocedure");
                mzk_claimprocedure["mzk_claim"] = claimEntityReference;

                //string mzk_proceduretypecodeableconcept = "codeable concept GUID";
                //EntityReference mzk_proceduretypecodeableconceptEntityRef = new EntityReference("msemr_codeableconcept", new Guid(mzk_proceduretypecodeableconcept));
                //mzk_claimprocedure["mzk_proceduretypecodeableconcept"] = mzk_proceduretypecodeableconceptEntityRef.Id;

                mzk_claimprocedure["mzk_proceduretype"] = new OptionSetValue(275380000); //Codeable concept

                mzk_claimprocedure["mzk_proceduresequence"] = 1;

                mzk_claimprocedure["mzk_proceduredate"] = DateTime.Now;

                repo.CreateEntity(mzk_claimprocedure);

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
