using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class HL7_PatientVIsit
    {
        public string SetID { set; get; }
        public string Pvq { set; get; }
        public string PatientClass  { set; get; }
        public HL7_AssignedPatientLocation AssignedPatientLocation  { set; get; }
        public HL7_AttendingDoctor  AttendingDoctor  { set; get; }
        public string AdmitSource { set; get; }
        public string AmbulatoryStatus  { set; get; }
        public string VisitNumber { set; get; }



    }
    public enum PatientClass
    {
        O,
        I,
        E,
    }
}
