using MazikCareService.Core.Enums;
using MazikCareService.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;




namespace MazikCareWebApi.Models
{
    public class ApiModel
    {
        public string practiceId { get; set; }

        public string resourceId { get; set; }
        public string notesAnnotationId { get; set; }
        public string PostId { get; set; }
        public string patientEncounterId { get; set; }
        public string statusManagerId { get; set; }
        public List<string> dropDownName { get; set; }
        public string documentRecId { get; set; }
        public string patientId { get; set; }
        public string message { get; set; }
        public long patientRecId { get; set; }
        public bool forFulfillment { get; set; }
        public bool getDocuments { get; set; }
        public bool getAddresses { get; set; }
        public bool getRelationship { get; set; }
        public string narrationGuid { get; set; }
        public string EncounterTemplateId { get; set; }
        public string currentpage { get; set; }
        public long AxAppoitnmentRefRecId { get; set; }
        public string appointmentId { get; set; }
        public string CaseId { get; set; }
        public string DosageId { get; set; }
        public string OptionSetName { get; set; }
        public string TestName { get; set; }
        public string MRN { get; set; }
        public string AssociatedDiagnosis { get; set; }
        public string Frequency { get; set; }
        public DateTime StudyDate { get; set; }
        public DateTime date { get; set; }
        public string Location { get; set; }
        public string ClinicalNotes { get; set; }
        public bool registered { get; set; }
        public string CheifComplaint { get; set; }
        public string VisitReason { get; set; }
        public string NurseInstruction { get; set; }
        public string Antibiotics { get; set; }
        public string AntibioticsComments { get; set; }
        public string entityName { get; set; }
        public string OptionSetAttributeValue { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string worklistTypeID { get; set; }
        public string userId { get; set; }
        public string clinicId { get; set; }
        public long resourceRecId { get; set; }
        public string ConceptId { get; set; }
        public string templateGuid { get; set; }
        public string SearchFilters { get; set; }
        public string searchOrder { get; set; }
        public string favourite { get; set; }
        public string statusManagerDetailsId { get; set; }
        public string orderId { get; set; }
        public string workOrderId { get; set; }
        public string timezone { get; set; }
        public StatusManagerParams ParamsValues { get; set; }

        public VisitAppointmentFilterBy visitAppointmentFilterBy { get; set; }

        public string attributename { get; set; }
        public string defaultVal { get; set; }
        public string typeValue { get; set; }


        public int encounterType { get; set; }

        public int caseType { get; set; }

        public bool IsActive { get; set; }
        public bool summaryUpdated { get; set; }

        public string AllergyReviewedBy { get; set; }
        public string SpecialityId { get; set; }
        public string cpsaWorkflowId { get; set; }

        public string contactId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public List<ClinicalTemplateNarration> narrationList { get; set; }

        public List<ClinicalTemplate> templateList { get; set; }

        public List<PatientAllergy> allergyList { get; set; }
        public List<PatientProblem> problemList { get; set; }
        public string caseNumber { get; set; }
        public bool isNew { get; set; }
        public string deviceId { get; set; }
        public string enrollmentId { get; set; }
        public string centricityId { get; set; }
        public string physicianId { get; set; }

        public string insuranceId { get; set; }

        public string insuranceName { get; set; }

        public Physician physician { get; set; }

        public Insurance insurance { get; set; }

        public Enrollment enrollment { get; set; }

        public DateTime dateOfService { get; set; }

        public string externalOrderId { get; set; }
        public string externalPatientId { get; set; }
        public string providerCode { get; set; }
        public string facilityCode { get; set; }
        public string serviceType { get; set; }
        public string messageType { get; set; }
        public string diagnosticCodeId { get; set; }
        public string diagnosticCodeName { get; set; }
        public string userCreatedBy { get; set; }
        public string userModifiedBy { get; set; }
        public string npi { get; set; }
        public string choice { get; set; }
        public Patient patient { get; set; }
        public PatientCondition patientCondition { get; set; }
        public PatientCarePlan patientCarePlan { get; set; }
        public PatientAllergy patientAllergy { get; set; }
        public PatientObservation patientObservation { get; set; }
        public PatientProcedure patientProcedure { get; set; }
        public PatientEncounter patientEncounter { get; set; }
        public PatientDevice patientDevice { get; set; }
        public PatientProcedureRequest patientProcedureRequest { get; set; }
        public PatientAppointment patientAppointment { get; set; }
        public string mmtCodeId { get; set; }
        public string referralId { get; set; }
        public Alert alert { get; set; }
        public List<PatientVisitProducts> productsList { get; set; }
        public string FAQId { get; set; }
        public Relationship relation { get; set; }
        public string carerId { get; set; }
        public string patientOrderId { get; set; }
        public string cancellationReason { get; set; }
        public string doseId { get; set; }
        public string dateTime { get; set; }
        public string skipReasonId { get; set; }
    
    }
    public enum Choice
    {
        Add,
        Modify,
        Delete
    }


}
