using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class Activity
    {
        public Patient patient { get; set; }

        public Appointment appointment { get; set; }

        public string activityDateTime { get; set; }

        public string caseId { get; set; }

        public string clinicName { get; set; }
        public string clinicCode { get; set; }

        public string roomName { get; set; }
        public string AgeGroup { get; set; }

        public string encounterId { get; set; }

        public int type { get; set; }
        public string colorCode { get; set; }

        public List<User> careTeam { get; set; }

        public string ActiveProblems { get; set; }

        public string ActiveAllergies { get; set; }

        public int Status { get; set; }
        public string StatusText { get; set; }

        public EncounterCreation EncounterCreationStatus { get; set; }

        public string EncounterSigoffLabel { get; set; }

        public string casePathwayActivityWorkflowId { get; set; }
        /* public enum ActivityStatus {None, Open,
             [Description("Pending Consultation")]PendingConsultation,
             [Description("Pending Triage")]PendingTriage,
             [Description("In Triage")]InTriage,
             [Description("In Consultation")]InConsultation,
             [Description("Consultation Complete")]ConsultationComplete,
             [Description("Triage Complete")]TriageComplete,
             [Description("In Treatment")]InTreatment,
             [Description("Treatment Complete")]TreatmentComplete,
             [Description("Pending Treatment")]PendingTreatment,
             [Description("In Assessment")]InAssessment,
             [Description("Assessment Complete")]AssessmentComplete,
             [Description("Pending Assessment")]PendingAssessment
         }*/

        public enum EncounterCreation { None = 0, CreateEncounter = 1, ResumeEncounter = 2, NoAction = 3 }

        public enum EncounterSigoff
        {
            [Description("Sign Off")]
            SignOff = 0,
            [Description("Clinically Discharge")]
            ClinicallyDischarge = 1,
            [Description("Physically Discharge")]
            PhysicallyDischarge = 2
        }
    }
    
}
