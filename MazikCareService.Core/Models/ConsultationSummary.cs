using Helper;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class ConsultationSummary
    {
        //Patient
        public Patient                      patient                     { get; set; }
        public string                       patientEncounterId          { get; set; }
        public string                       patientCaseId               { get; set; }
        public string                       patientId                   { get; set; }
        public List<PatientEncounter>       listPatientEncounter        { get; set; }
        //SUBJECTIVE
        public List<CheifComplaint>         listCheifComplaint          { get; set; }
        public List<PatientVisit>           listVisitReason             { get; set; }
        //OBJECTIVE
        public List<PatientVitals>          listPatientVitals           { get; set; }
        public List<PatientAllergy>         listPatientAllergy          { get; set; }
        //ASSESSMENT
        public List<PatientDiagnosis>       listPatientDiagnosis        { get; set; }
        public List<PatientProblem>         listPatientProblem          { get; set; }
        //PLAN
        public List<PatientMedication>      listPatientMedication       { get; set; }
        public List<PatientLabOrder>        listPatientLabOrder         { get; set; }
        public List<PatientRadiologyOrder>  listPatientRadiologyOrder   { get; set; }
        public List<PatientSpecialTest>     listPatientSpecialTest      { get; set; }
        public List<PatientProcedure>       listPatientProcedure        { get; set; }
        public List<PatientReferralOrder>        listPatientReferral         { get; set; }
        public List<PatientDisposition>     listPatientDisposition      { get; set; }
        public List<PatientTherapy>         listPatientTherapy          { get; set; }
        public List<ClinicalTemplate>       listClinicalTemplate        { get; set; }
        public List<Notes>                  listProgressNotes           { get; set; }
        public DateTime                     SummaryUpdatedDate          { get; set; }
        public async Task<List<ConsultationSummary>> getConsultationSummary_OLD(string patientEncounterId,bool summaryUpdated=false)
        {
            try
            {

                var json=string.Empty;
                string triagePatientEncounterId = string.Empty;
                List<ConsultationSummary> ConsultationSummary = new List<ConsultationSummary>();
                ConsultationSummary model = new ConsultationSummary();
                PatientEncounter encounter = new PatientEncounter();
                encounter.EncounterId = patientEncounterId;
                if (summaryUpdated == true)
                {
                    model.listPatientEncounter = encounter.encounterDetails(encounter).Result.ToList();
                    patientCaseId = model.listPatientEncounter[0].CaseId;
                    patientId = model.listPatientEncounter[0].PatientId;
                    model.patientEncounterId = patientEncounterId;
                    model.patientId = patientId;
                    model.patientCaseId = patientCaseId;
                    model.patient = new Patient().getPatientBasicInfo(patientId).Result.patient;
                    model.listVisitReason = new PatientVisit().getVisitReason(null, patientEncounterId).Result.ToList();
                    model.listCheifComplaint = new CheifComplaint().getCheifComplaint(null, patientEncounterId, 0, "").Result.ToList();
                    model.listPatientAllergy = new PatientAllergy().getPatientAllergies(patientId, null, null, DateTime.MinValue, DateTime.MinValue, true).Result.ToList();
                    model.listPatientVitals = new PatientVitals().getPatientEncounterVitals(null, patientEncounterId, false, false).Result.ToList();
                    if (model.listPatientVitals.Count == 0)
                    {
                        PatientEncounter triageEncounter = new PatientEncounter();
                        triageEncounter.CaseId = patientCaseId;
                        triageEncounter.EncounterType = "1";
                        if (triageEncounter.encounterDetails(triageEncounter).Result.Count > 0)
                        {
                            triagePatientEncounterId = triageEncounter.encounterDetails(triageEncounter).Result.ToList().First<PatientEncounter>().EncounterId;
                            model.listPatientVitals = new PatientVitals().getPatientEncounterVitals(null, triagePatientEncounterId, false, false).Result.ToList();
                        }
                    }

                    model.listPatientMedication = new PatientMedication().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                    model.listPatientRadiologyOrder = new PatientRadiologyOrder().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                    model.listPatientLabOrder = new PatientLabOrder().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                    model.listPatientSpecialTest = new PatientSpecialTest().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, "").Result.ToList();
                    model.listPatientTherapy = new PatientTherapy().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                    model.listPatientReferral = new PatientReferralOrder().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                    model.listPatientProcedure = new PatientProcedure().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null,null,false).Result.ToList();
                    model.listPatientDiagnosis = new PatientDiagnosis().getPatientDiagnosis(patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, 0, "").Result.ToList();
                    model.listPatientProblem = new PatientProblem().getPatientProblems(patientId, true, null, null, DateTime.MinValue, DateTime.MinValue).Result.ToList();
                    model.listPatientDisposition = new PatientDisposition().getPatientDisposition(patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue).Result.ToList();
                    model.listClinicalTemplate = new ClinicalTemplate().getPatientClinicalTemplates(patientId, patientEncounterId).Result;
                    model.SummaryUpdatedDate = DateTime.Now;
                    Notes notes = new Notes();
                    notes.EntityType = "incident";
                    notes.ObjectId = patientCaseId;
                    model.listProgressNotes = new Notes().getNotes(notes).Result;
                    ConsultationSummary.Add(model);

                    json = JsonConvert.SerializeObject(model);
                    encounter.updateSummaryJson(patientEncounterId, json);
                }
                else
                {
                    PatientEncounter pe = new PatientEncounter();
                    if (pe.encounterDetails(encounter).Result.Count > 0)
                    {
                        json = pe.encounterDetails(encounter).Result[0].SummaryJson;
                        if (json != null)
                        {
                            model = JsonConvert.DeserializeObject<ConsultationSummary>(json);
                            ConsultationSummary.Add(model);
                        }
                        else
                        {
                            model.listPatientEncounter = pe.encounterDetails(encounter).Result.ToList();
                            patientCaseId = model.listPatientEncounter[0].CaseId;
                            patientId = model.listPatientEncounter[0].PatientId;
                            model.patientEncounterId = patientEncounterId;
                            model.patientId = patientId;
                            model.patientCaseId = patientCaseId;
                            
                            model.patient = new Patient().getPatientBasicInfo(patientId).Result.patient;
                            model.listVisitReason = new PatientVisit().getVisitReason(null, patientEncounterId).Result.ToList();
                            model.listCheifComplaint = new CheifComplaint().getCheifComplaint(null, patientEncounterId, 0, "").Result.ToList();
                            model.listPatientAllergy = new PatientAllergy().getPatientAllergies(patientId, null, null, DateTime.MinValue, DateTime.MinValue, true).Result.ToList();
                            model.listPatientVitals = new PatientVitals().getPatientEncounterVitals(null, patientEncounterId, false, false).Result.ToList();
                            if (model.listPatientVitals.Count == 0)
                            {
                                PatientEncounter triageEncounter = new PatientEncounter();
                                triageEncounter.CaseId = patientCaseId;
                                triageEncounter.EncounterType = "1";
                                if (triageEncounter.encounterDetails(triageEncounter).Result.Count > 0)
                                {
                                    triagePatientEncounterId = triageEncounter.encounterDetails(triageEncounter).Result.ToList().First<PatientEncounter>().EncounterId;
                                    model.listPatientVitals = new PatientVitals().getPatientEncounterVitals(null, triagePatientEncounterId, false, false).Result.ToList();
                                }
                            }
                            model.listPatientMedication = new PatientMedication().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                            model.listPatientRadiologyOrder = new PatientRadiologyOrder().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                            model.listPatientLabOrder = new PatientLabOrder().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                            model.listPatientSpecialTest = new PatientSpecialTest().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, "").Result.ToList();
                            model.listPatientTherapy = new PatientTherapy().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                            model.listPatientReferral = new PatientReferralOrder().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                            model.listPatientProcedure = new PatientProcedure().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                            model.listPatientDiagnosis = new PatientDiagnosis().getPatientDiagnosis(patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, 0, "").Result.ToList();
                            model.listPatientProblem = new PatientProblem().getPatientProblems(patientId, true, null, null, DateTime.MinValue, DateTime.MinValue).Result.ToList();
                            model.listPatientDisposition = new PatientDisposition().getPatientDisposition(patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue).Result.ToList();
                            model.listClinicalTemplate = new ClinicalTemplate().getPatientClinicalTemplates(patientId, patientEncounterId).Result;
                            model.SummaryUpdatedDate = DateTime.Now;
                            Notes notes = new Notes();
                            notes.EntityType = "incident";
                            notes.ObjectId = patientCaseId;
                            model.listProgressNotes = new Notes().getNotes(notes).Result;
                            ConsultationSummary.Add(model);

                            json = JsonConvert.SerializeObject(model);
                            encounter.updateSummaryJson(patientEncounterId, json);
                        }
                    }
                    else
                    {
                        model.listPatientEncounter = new PatientEncounter().encounterDetails(encounter).Result.ToList();
                        patientCaseId = model.listPatientEncounter[0].CaseId;
                        patientId = model.listPatientEncounter[0].PatientId;
                        model.patientEncounterId = patientEncounterId;
                        model.patientId = patientId;
                        model.patientCaseId = patientCaseId;
                        
                        model.patient = new Patient().getPatientBasicInfo(patientId).Result.patient;
                        model.listVisitReason = new PatientVisit().getVisitReason(null, patientEncounterId).Result.ToList();
                        model.listCheifComplaint = new CheifComplaint().getCheifComplaint(null, patientEncounterId, 0, "").Result.ToList();
                        model.listPatientAllergy = new PatientAllergy().getPatientAllergies(patientId, null, null, DateTime.MinValue, DateTime.MinValue, true).Result.ToList();
                        model.listPatientVitals = new PatientVitals().getPatientEncounterVitals(null, patientEncounterId, false, false).Result.ToList();
                        if (model.listPatientVitals.Count == 0)
                        {
                            PatientEncounter triageEncounter = new PatientEncounter();
                            triageEncounter.CaseId = patientCaseId;
                            triageEncounter.EncounterType = "1";
                            if (triageEncounter.encounterDetails(triageEncounter).Result.Count > 0)
                            {
                                triagePatientEncounterId = triageEncounter.encounterDetails(triageEncounter).Result.ToList().First<PatientEncounter>().EncounterId;
                                model.listPatientVitals = new PatientVitals().getPatientEncounterVitals(null, triagePatientEncounterId, false, false).Result.ToList();
                            }
                        }
                        model.listPatientMedication = new PatientMedication().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                        model.listPatientRadiologyOrder = new PatientRadiologyOrder().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                        model.listPatientLabOrder = new PatientLabOrder().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                        model.listPatientSpecialTest = new PatientSpecialTest().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, "").Result.ToList();
                        model.listPatientTherapy = new PatientTherapy().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                        model.listPatientReferral = new PatientReferralOrder().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                        model.listPatientProcedure = new PatientProcedure().getPatientOrder(null, patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, false, null).Result.ToList();
                        model.listPatientDiagnosis = new PatientDiagnosis().getPatientDiagnosis(patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, 0, "").Result.ToList();
                        model.listPatientProblem = new PatientProblem().getPatientProblems(patientId, true, null, null, DateTime.MinValue, DateTime.MinValue).Result.ToList();
                        model.listPatientDisposition = new PatientDisposition().getPatientDisposition(patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue).Result.ToList();
                        model.listClinicalTemplate = new ClinicalTemplate().getPatientClinicalTemplates(patientId, patientEncounterId).Result.ToList();
                        model.SummaryUpdatedDate = DateTime.Now;
                        Notes notes = new Notes();
                        notes.EntityType = "incident";
                        notes.ObjectId = patientCaseId;
                        model.listProgressNotes = new Notes().getNotes(notes).Result;
                        ConsultationSummary.Add(model);

                        json = JsonConvert.SerializeObject(model);
                        encounter.updateSummaryJson(patientEncounterId, json);
                    }
                }
                
                return ConsultationSummary;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ConsultationSummary>> getConsultationSummary(string patientEncounterId, bool summaryUpdated = false, bool isStartEncounter = false)
        {
            try
            {
                var json = string.Empty;
                
                List<ConsultationSummary> ConsultationSummary = new List<ConsultationSummary>();
                ConsultationSummary model = null;

                PatientEncounter encounter = new PatientEncounter();
                encounter.EncounterId = patientEncounterId;
                List<PatientEncounter> listPatientEncounter = encounter.getEncounterDetails(encounter).Result;

                if (listPatientEncounter != null)
                {
                    if (summaryUpdated == true)
                    {
                        model = await this.getConsultationSummaryModel(listPatientEncounter, isStartEncounter);

                        if (model != null)
                        {
                            ConsultationSummary.Add(model);
                        }
                    }
                    else
                    {
                        if (listPatientEncounter.Count > 0)
                        {
                            json = listPatientEncounter[0].SummaryJson;
                            if (json != null)
                            {
                                string decompressjson = string.Empty;
                                decompressjson = StringHelper.Decompress(json);
                                model = JsonConvert.DeserializeObject<ConsultationSummary>(decompressjson);
                                ConsultationSummary.Add(model);
                            }
                            else
                            {
                                model = await this.getConsultationSummaryModel(listPatientEncounter, isStartEncounter);

                                if (model != null)
                                {
                                    ConsultationSummary.Add(model);
                                }
                            }
                        }
                        else
                        {
                            model = await this.getConsultationSummaryModel(listPatientEncounter, isStartEncounter);

                            if (model != null)
                            {
                                ConsultationSummary.Add(model);
                            }
                        }
                    }
                }
                else
                {
                    throw new ValidationException("Unable to find Patient Encounter");
                }

                return ConsultationSummary;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ConsultationSummary>> triageConsultationSummary(string caseId, string cpsaWorkflowId)
        {
            try
            {
                List<ConsultationSummary> ConsultationSummary = new List<ConsultationSummary>();

                PatientEncounter enc = new PatientEncounter();
                if (!string.IsNullOrEmpty(cpsaWorkflowId)) {
                    patientEncounterId = enc.getEncounterIdByWorkflowId(cpsaWorkflowId);
                    if (string.IsNullOrEmpty(patientEncounterId)) {
                        throw new ValidationException("Nursing triage summary is not available");
                    }
                }
                else if (!string.IsNullOrEmpty(caseId))
                {
                    enc.CaseId = caseId;
                    enc.EncounterType = Convert.ToString((int)mzk_encountertype.Triage);

                    List<PatientEncounter> listPatEnc = enc.getEncounterDetails(enc).Result;

                    if (listPatEnc.Count > 0)
                    {
                        patientEncounterId = listPatEnc.First<PatientEncounter>().EncounterId;
                    }
                    else
                    {
                        throw new ValidationException("Nursing triage summary is not available");
                    }
                }

                if(!string.IsNullOrEmpty(patientEncounterId))
                {
                    return this.getConsultationSummary(patientEncounterId).Result;
                }

                return ConsultationSummary;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<ConsultationSummary>> assessmentConsultationSummary(string caseId)
        {
            try
            {
                List<ConsultationSummary> ConsultationSummary = new List<ConsultationSummary>();

                PatientEncounter enc = new PatientEncounter();
                if (!string.IsNullOrEmpty(caseId))
                {
                    enc.CaseId = caseId;
                    enc.EncounterType = Convert.ToString((int)mzk_encountertype.PrimaryAssessment);

                    List<PatientEncounter> listPatEnc = enc.getEncounterDetails(enc).Result;

                    if (listPatEnc.Count > 0)
                    {
                        patientEncounterId = listPatEnc.First<PatientEncounter>().EncounterId;
                    }
                    else
                    {
                        throw new ValidationException("Assessment summary is not available");
                    }
                }
                return this.getConsultationSummary(patientEncounterId).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<ConsultationSummary>> getCarePLan(string caseId)
        {
            try
            {
                List<ConsultationSummary> ConsultationSummary = new List<ConsultationSummary>();
                ConsultationSummary model = new ConsultationSummary();
                PatientMedication medication = new PatientMedication();
                PatientRadiologyOrder radiology = new PatientRadiologyOrder();
                PatientLabOrder lab = new PatientLabOrder();
                PatientSpecialTest st = new PatientSpecialTest();
                PatientTherapy pt = new PatientTherapy();
                PatientReferralOrder pr = new PatientReferralOrder();
                PatientProcedure pp = new PatientProcedure();

                PatientEncounter encounter = new PatientEncounter();
                encounter.CaseId = caseId;
                model.listPatientMedication = medication.getPatientOrder(null, null, null, null, DateTime.MinValue, DateTime.MinValue, false, null,caseId).Result.ToList();
                model.listPatientRadiologyOrder = radiology.getPatientOrder(null, null, null, null, DateTime.MinValue, DateTime.MinValue, false, null, caseId).Result.ToList();
                model.listPatientLabOrder = lab.getPatientOrder(null, null, null, null, DateTime.MinValue, DateTime.MinValue, false, null, caseId).Result.ToList();
                model.listPatientSpecialTest = st.getPatientOrder(null, null, null, null, DateTime.MinValue, DateTime.MinValue, false, "", caseId).Result.ToList();
                model.listPatientTherapy = pt.getPatientOrder(null, null, null, null, DateTime.MinValue, DateTime.MinValue, false, null, caseId).Result.ToList();
                model.listPatientReferral = pr.getPatientOrder(null, null, null, null, DateTime.MinValue, DateTime.MinValue, false, null, caseId).Result.ToList();
                model.listPatientProcedure = pp.getPatientOrder(null, null, null, null, DateTime.MinValue, DateTime.MinValue, false, null, caseId).Result.ToList();
                ConsultationSummary.Add(model);

                return ConsultationSummary;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<ConsultationSummary> getConsultationSummaryModel(List<PatientEncounter> listPatientEncounter, bool isStartEncounter = false)
        {
            var json = string.Empty;
            string triagePatientEncounterId = string.Empty;
            ConsultationSummary model = new ConsultationSummary();

            string patientEncounterId = listPatientEncounter[0].EncounterId;
            string EncounterTemplateId = listPatientEncounter[0].EncounterTemplateId;

            model.listPatientEncounter = listPatientEncounter;
            patientCaseId = model.listPatientEncounter[0].CaseId;
            patientId = new PatientCase().getCaseDetails(patientCaseId).Result.PatientId;
            model.patientEncounterId = patientEncounterId;
            model.patientId = patientId;
            model.patientCaseId = patientCaseId;

            model.patient = new Patient().getPatientDetails(patientId).Result;

            model.listVisitReason = new PatientVisit().getVisitReason("", "", patientCaseId, model.listPatientEncounter[0].AppointmentId).Result.ToList();

            model.listCheifComplaint = new List<CheifComplaint>();//new CheifComplaint().getCheifComplaint(null, patientEncounterId, 0, "").Result.ToList();

            model.listPatientAllergy = new PatientAllergy().getPatientAllergies(patientId, null, null, DateTime.MinValue, DateTime.MinValue, true).Result.ToList();

            model.listPatientVitals = new PatientVitals().getPatientEncounterVitals(null, patientEncounterId, false, false, "", false).Result.ToList();

            if (model.listPatientVitals.Count == 0 && model.listPatientEncounter[0].EncounterType != "1")
            {
                PatientEncounter triageEncounter = new PatientEncounter();
                triageEncounter.CaseId = patientCaseId;
                triageEncounter.EncounterType = "1";

                List<PatientEncounter> triageEncounterList = triageEncounter.getEncounterDetails(triageEncounter).Result.ToList();

                if (triageEncounterList.Count > 0)
                {
                    triagePatientEncounterId = triageEncounterList.First<PatientEncounter>().EncounterId;
                    model.listPatientVitals = new PatientVitals().getPatientEncounterVitals(null, triagePatientEncounterId, false, false, "", false).Result.ToList();
                }
            }

            if (!isStartEncounter && listPatientEncounter[0].EncounterType != ((int)mzk_encountertype.Triage).ToString())
            {
                EntityCollection patientOrderDetailsCollection = new PatientOrder().getPatientOrderDetails(patientEncounterId);

                if (patientOrderDetailsCollection != null)
                {
                    model.listPatientRadiologyOrder = new PatientRadiologyOrder().getPatientOrder(patientOrderDetailsCollection.Entities.Where(item => (item["mzk_type"] as OptionSetValue).Value.ToString() == ((int)mzk_patientordermzk_Type.Radiology).ToString()).ToList()).Result.ToList();
                    model.listPatientLabOrder = new PatientLabOrder().getPatientOrder(patientOrderDetailsCollection.Entities.Where(item => (item["mzk_type"] as OptionSetValue).Value.ToString() == ((int)mzk_patientordermzk_Type.Lab).ToString()).ToList()).Result.ToList();
                    model.listPatientSpecialTest = new PatientSpecialTest().getPatientOrder(patientOrderDetailsCollection.Entities.Where(item => (item["mzk_type"] as OptionSetValue).Value.ToString() == ((int)mzk_patientordermzk_Type.SpecialTest).ToString()).ToList()).Result.ToList();
                    model.listPatientTherapy = new List<PatientTherapy>();
                    model.listPatientReferral = new PatientReferralOrder().getPatientOrder(patientOrderDetailsCollection.Entities.Where(item => (item["mzk_type"] as OptionSetValue).Value.ToString() == ((int)mzk_patientordermzk_Type.Referral).ToString()).ToList()).Result.ToList();
                    model.listPatientProcedure = new PatientProcedure().getPatientOrder(patientOrderDetailsCollection.Entities.Where(item => (item["mzk_type"] as OptionSetValue).Value.ToString() == ((int)mzk_patientordermzk_Type.Procedure).ToString()).ToList()).Result.ToList();
                    model.listPatientMedication = new PatientMedication().getPatientOrder(patientOrderDetailsCollection.Entities.Where(item => (item["mzk_type"] as OptionSetValue).Value.ToString() == ((int)mzk_patientordermzk_Type.Medication).ToString()).ToList()).Result.ToList();
                }
            }
            else
            {
                model.listPatientRadiologyOrder = new List<PatientRadiologyOrder>();
                model.listPatientLabOrder = new List<PatientLabOrder>();
                model.listPatientSpecialTest = new List<PatientSpecialTest>();
                model.listPatientTherapy = new List<PatientTherapy>();
                model.listPatientReferral = new List<PatientReferralOrder>();
                model.listPatientProcedure = new List<PatientProcedure>();
                model.listPatientMedication = new List<PatientMedication>();
            }

            if (!isStartEncounter && listPatientEncounter[0].EncounterType != ((int)mzk_encountertype.Triage).ToString())
            {
                model.listPatientDiagnosis = new PatientDiagnosis().getPatientDiagnosis(patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue, 0, "").Result.ToList();
            }
            else
            {
                model.listPatientDiagnosis = new List<PatientDiagnosis>();
            }
            //   model.listPatientProblem = new PatientProblem().getPatientProblems(patientId, true, null, null, DateTime.MinValue, DateTime.MinValue).Result.ToList();
            model.listPatientProblem = new List<PatientProblem>();

            if (!isStartEncounter && listPatientEncounter[0].EncounterType != ((int)mzk_encountertype.Triage).ToString())
            {
                model.listPatientDisposition = new PatientDisposition().getPatientDisposition(patientEncounterId, null, null, DateTime.MinValue, DateTime.MinValue).Result.ToList();
            }
            else
            {
                model.listPatientDisposition = new List<PatientDisposition>();
            }

            if (!isStartEncounter)
            {
                model.listClinicalTemplate = new ClinicalTemplate().getPatientClinicalTemplates(patientId, patientEncounterId).Result;

            }
            else
            {
                model.listClinicalTemplate = new List<ClinicalTemplate>();
            }

            if (listPatientEncounter[0].EncounterType != ((int)mzk_encountertype.Triage).ToString())
            {
                model.listProgressNotes = new Notes().getCaseNotes(patientCaseId).Result;
            }
            else
            {
                model.listProgressNotes = new List<Notes>();
            }

            model.SummaryUpdatedDate = DateTime.Now;
            json = JsonConvert.SerializeObject(model);
            string compressJson = string.Empty;
            compressJson = StringHelper.Compress(json);

            new PatientEncounter().updateSummaryJson(patientEncounterId, compressJson);

            return model;
        }
    }
}
