using Helper;
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
    public class CareProgram
    {
        public string id { get; set; }

        public int caseTypeId { get; set; }

        public string caseType { get; set; }

        public string code { get; set; }

        public string description { get; set; }

        public string masterPathwayId { get; set; }

        public string masterPathway { get; set; }

        public DateTime createdOn { get; set; }
        public string url { get; set; }

        public async Task<bool> addProgramEnrollmentRequest(string patientEncounterId, string careProgramId, string patientId)
        {
            bool programEnrollmentRequestCreated = false;

            string id = null;

            mzk_programenrollmentrequest programEnrollmentRequest = new mzk_programenrollmentrequest();
            
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            PatientDiagnosis patientDiagnosis = new PatientDiagnosis().getPatientDiagnosis(patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, 0, "").Result.ToList().FirstOrDefault();

            if (patientDiagnosis != null)
            {
                if (!string.IsNullOrEmpty(patientDiagnosis.DiagnosisId))
                    programEnrollmentRequest.mzk_PrincipalDiagnosis = new EntityReference(mzk_concept.EntityLogicalName, new Guid(patientDiagnosis.DiagnosisId));

            }

            if (!string.IsNullOrEmpty(patientId))
            programEnrollmentRequest.mzk_customerid = new EntityReference(Contact.EntityLogicalName, new Guid(patientId));

            if(!string.IsNullOrEmpty(careProgramId))
            programEnrollmentRequest.mzk_Program = new EntityReference(xrm.mzk_careprogram.EntityLogicalName, new Guid(careProgramId));

            if (!string.IsNullOrEmpty(patientEncounterId))
                programEnrollmentRequest.mzk_Encounter = new EntityReference(mzk_patientencounter.EntityLogicalName, new Guid(patientEncounterId));

            //programEnrollmentRequest.mzk_Status = mzk_programenrollmentrequeststatus.Requested;

            id = Convert.ToString(entityRepository.CreateEntity(programEnrollmentRequest));
            
            if(!string.IsNullOrEmpty(id))
            {
                programEnrollmentRequestCreated = true;
            }
            else
            {
                programEnrollmentRequestCreated = false;
            }
            
            return programEnrollmentRequestCreated;
        }

        public async Task<List<ProgramEnrollmentRequest>> getProgramEnrollmentRequest(string patientEncounterId)
        {
            List<ProgramEnrollmentRequest> programEnrollmentRequestList = new List<ProgramEnrollmentRequest>();

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            QueryExpression query = new QueryExpression(mzk_programenrollmentrequest.EntityLogicalName);

            query.ColumnSet = new ColumnSet(true);

            if (!string.IsNullOrEmpty(patientEncounterId))
            {
                query.Criteria.AddCondition("mzk_encounter", ConditionOperator.Equal, new Guid(patientEncounterId));
            }

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {
                foreach (Entity entity in entitycollection.Entities)
                {
                    ProgramEnrollmentRequest programEnrollmentRequestModel = new ProgramEnrollmentRequest();

                    mzk_programenrollmentrequest programEnrollmentRequestRecord = (mzk_programenrollmentrequest)entity;

                    programEnrollmentRequestModel.id = programEnrollmentRequestRecord.Id.ToString();

                    if (programEnrollmentRequestRecord.Attributes.Contains("createdon"))
                    {
                        programEnrollmentRequestModel.createdOn = Convert.ToDateTime(programEnrollmentRequestRecord.CreatedOn);
                    }

                    if (programEnrollmentRequestRecord.Attributes.Contains("mzk_customerid"))
                    {
                        programEnrollmentRequestModel.patientId = (programEnrollmentRequestRecord.mzk_customerid).Id.ToString();
                        programEnrollmentRequestModel.patientName = (programEnrollmentRequestRecord.FormattedValues["mzk_customerid"]).ToString();
                    }

                    if (programEnrollmentRequestRecord.Attributes.Contains("mzk_program"))
                    {
                        programEnrollmentRequestModel.programId = (programEnrollmentRequestRecord.mzk_Program).Id.ToString();
                        programEnrollmentRequestModel.programName = (programEnrollmentRequestRecord.FormattedValues["mzk_program"]).ToString();
                    }

                    if (programEnrollmentRequestRecord.Attributes.Contains("mzk_principaldiagnosis"))
                    {
                        programEnrollmentRequestModel.principalDiagnosisId = (programEnrollmentRequestRecord.mzk_PrincipalDiagnosis).Id.ToString();
                        programEnrollmentRequestModel.principalDiagnosisName = (programEnrollmentRequestRecord.FormattedValues["mzk_principaldiagnosis"]).ToString();
                    }

                    if (programEnrollmentRequestRecord.Attributes.Contains("mzk_encounter"))
                    {
                        programEnrollmentRequestModel.patientEncounterId = (programEnrollmentRequestRecord.mzk_Encounter).Id.ToString();
                    }

                    if (programEnrollmentRequestRecord.Attributes.Contains("mzk_description"))
                    {
                        programEnrollmentRequestModel.programId = (programEnrollmentRequestRecord.mzk_Program).Id.ToString();
                    }

                    if (programEnrollmentRequestRecord.Attributes.Contains("mzk_status"))
                    {
                        programEnrollmentRequestModel.statusValue = (programEnrollmentRequestRecord.mzk_Status).Value.ToString();
                        programEnrollmentRequestModel.status = (programEnrollmentRequestRecord.FormattedValues["mzk_status"]).ToString();
                    }

                    programEnrollmentRequestList.Add(programEnrollmentRequestModel);
                }
            }
            return programEnrollmentRequestList;
        }

        public async Task<List<CareProgram>> getCarePrograms(string Name)
        {
            List<CareProgram> carePrograms = new List<CareProgram>();

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            QueryExpression query = new QueryExpression(mzk_careprogram.EntityLogicalName);

            query.ColumnSet = new ColumnSet(true);

            query.Criteria.AddCondition("mzk_description", ConditionOperator.Like, ("%" + Name.ToLower() + "%"));

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {

                foreach (Entity entity in entitycollection.Entities)
                {
                    CareProgram model = new CareProgram();

                    mzk_careprogram careProgram = (mzk_careprogram)entity;

                    model.id = careProgram.Id.ToString();

                    if (careProgram.Attributes.Contains("createdon"))
                    {
                        model.createdOn = Convert.ToDateTime(careProgram.CreatedOn);
                    }
                    
                    if (careProgram.Attributes.Contains("mzk_casetype"))
                    {
                        model.caseTypeId = ((careProgram.mzk_CaseType)).Value;
                        model.caseType = (careProgram.FormattedValues["mzk_casetype"]).ToString();
                    }

                    if (careProgram.Attributes.Contains("mzk_code"))
                    {
                        model.code = careProgram.mzk_code.ToString();
                    }

                    if (careProgram.Attributes.Contains("mzk_description"))
                    {
                        model.description = careProgram.mzk_description.ToString();
                    }

                    if (careProgram.Attributes.Contains("mzk_masterpathway"))
                    {
                        model.masterPathwayId = (careProgram.mzk_masterpathway).Id.ToString();
                        model.masterPathway = (careProgram.mzk_masterpathway).Name.ToString();
                    }
                    
                    model.url = AppSettings.GetByKey("CRMURL") + "main.aspx?etn=mzk_careprogram&pagetype=entityrecord&id=%7B"   +  model.id  +  "%7D";

                    carePrograms.Add(model);
                }
            }
            return carePrograms;
        }

    }    
}
