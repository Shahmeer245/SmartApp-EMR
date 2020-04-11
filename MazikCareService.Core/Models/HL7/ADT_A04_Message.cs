using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models.HL7
{
    public class ADT_A04_Message
    {
        public HL7_Patient PID { set; get; }
        public HL7_PatientVIsit PV1 { set; get; }
        public HL7_EventType EVN { set; get; }
        public string MsgControlId { set; get; }
        public async Task<ADT_A04_Message> GetADT_A04_Message(string appointmentRecId, string caseId,string patientId)
        {
            try
            {
                ADT_A04_Message Adt_A04_Msg = new ADT_A04_Message();
                HL7_Patient patient = new HL7_Patient();
                HL7_PatientVIsit PatientVisit = new HL7_PatientVIsit();
                Patient contract = new Patient();
                long patintId = 0, appointmentId=0;
                if (!string.IsNullOrEmpty(patientId))
                 patintId = Convert.ToInt64(patientId);
                if (!string.IsNullOrEmpty(appointmentRecId))
                    appointmentId  = Convert.ToInt64(appointmentRecId);
                if (string.IsNullOrEmpty(caseId))
                    caseId = string.Empty;
                  
               var result = contract.getPatientVisitDetails(appointmentId, caseId, patintId).Result as Activity;

               contract = result.patient;

               
                if (string.IsNullOrEmpty(contract.mrn))
                    patient.PatientID = " ";
                else
                    patient.PatientID = contract.mrn;


                if (string.IsNullOrEmpty(contract.mrn))
                    patient.MRNNumber = " ";
                else
                    patient.MRNNumber = contract.mrn;


                if (string.IsNullOrEmpty(contract.lastName))
                    patient.FamilyName = " ";
                else
                    patient.FamilyName = contract.lastName;

                if (string.IsNullOrEmpty(contract.firstName))
                    patient.GivenName = " ";
                else
                    patient.GivenName = contract.firstName;

                if (string.IsNullOrEmpty(contract.middleName))
                    patient.Initials = " ";
                else
                    patient.Initials = contract.middleName;

                if (string.IsNullOrEmpty(contract.dateOfBirth.ToString()))
                    patient.Dateofbirth = " ";
                else
                    patient.Dateofbirth = contract.dateOfBirth.ToString("yyyyMMddHHmm");
                //1 is male 
                //2 female
                if (contract.genderValue==1)
                    patient.AdministrativeSex = "M";
                if (contract.genderValue == 2)
                    patient.AdministrativeSex = "F";

                if (string.IsNullOrEmpty(contract.maritalStatus))
                    patient.MaritalStatus = " ";
                else
                    patient.MaritalStatus = contract.maritalStatus;

                if (string.IsNullOrEmpty(contract.mrn))
                    patient.AccountNumber = " ";
                else
                    patient.AccountNumber = contract.mrn;

                if (string.IsNullOrEmpty(contract.nationalIdValue))
                    patient.SSNNumber = " ";
                else
                    patient.SSNNumber = contract.nationalIdValue;

                patient.Address = new HL7_Address();
                if (contract.primaryAddress != null)
                {
                    
                    if (string.IsNullOrEmpty(contract.primaryAddress.addressType))
                        patient.Address.addressType = " ";
                    else
                        patient.Address.addressType = contract.primaryAddress.addressType;

                    if (string.IsNullOrEmpty(contract.primaryAddress.city))
                        patient.Address.city = " ";
                    else
                        patient.Address.city = contract.primaryAddress.city;

                    if (string.IsNullOrEmpty(contract.primaryAddress.state))
                        patient.Address.state = " ";
                    else
                        patient.Address.state = contract.primaryAddress.state;
                   
                    if (string.IsNullOrEmpty(contract.primaryAddress.street))
                        patient.Address.street = " ";
                    else
                        patient.Address.street = Helper.encodeEscapeChar(contract.primaryAddress.street);
                    
                    if (string.IsNullOrEmpty(contract.primaryAddress.country))
                        patient.Address.country = " ";
                    else
                        patient.Address.country = contract.primaryAddress.country;
                   
                    if (string.IsNullOrEmpty(contract.primaryAddress.zipCode))
                        patient.Address.zipCode = " ";
                    else
                        patient.Address.zipCode = contract.primaryAddress.zipCode;
                }

                patient.PhoneNumberHome = new HL7_Contact();
                
                if (string.IsNullOrEmpty(contract.homePhone))
                    patient.PhoneNumberHome.TelephoneNumber = " ";
                else
                    patient.PhoneNumberHome.TelephoneNumber = contract.homePhone;

                patient.PhoneNumberHome.TelecommunicatorUsecode = "PRN";
                patient.PhoneNumberHome.TelecommunicatorEquipmentType = "PH";

                patient.PhoneNumberBusiness = new HL7_Contact();
              
                if (string.IsNullOrEmpty(contract.workPhone))
                    patient.PhoneNumberBusiness.TelephoneNumber = " ";
                else
                    patient.PhoneNumberBusiness.TelephoneNumber = contract.workPhone;

                patient.PhoneNumberBusiness.TelecommunicatorUsecode = "WPN";
                patient.PhoneNumberBusiness.TelecommunicatorEquipmentType = "PH";

                patient.AssingingAuthority = "NazirBupa";
                patient.AssigningAuthorityIdentifierCode = "PI";
                if (string.IsNullOrEmpty(contract.mrn))
                    PatientVisit.SetID = contract.mrn = " ";
                else
                    PatientVisit.SetID = contract.mrn;
               
                PatientVisit.PatientClass = "O";
                if (result.type == 2)
                    PatientVisit.PatientClass ="O";
                if (result.type == 6)
                    PatientVisit.PatientClass = "E";

               
                if (result.appointment!=null)
                PatientVisit.VisitNumber = result.appointment.appointmentNumber;

                if (string.IsNullOrEmpty(PatientVisit.VisitNumber))
                    PatientVisit.VisitNumber = " ";

                PatientVisit.AssignedPatientLocation = new HL7_AssignedPatientLocation();
                
                PatientVisit.AssignedPatientLocation.Bed = " ";

                if (string.IsNullOrEmpty(result.roomName))
                    PatientVisit.AssignedPatientLocation.Room = " ";
                else
                    PatientVisit.AssignedPatientLocation.Room = result.roomName;


                if (string.IsNullOrEmpty(result.clinicName))
                    PatientVisit.AssignedPatientLocation.PointOfCare = " ";
                else
                    PatientVisit.AssignedPatientLocation.PointOfCare = result.clinicCode;
                //PatientVisit.AssignedPatientLocation.PointOfCare = "OP – Clinic"; //result.clinicName;

                
                PatientVisit.AttendingDoctor = new HL7_AttendingDoctor();
                if (result.careTeam != null && result.careTeam.Count > 0)
                {
                    PatientVisit.AttendingDoctor.Initials = " ";
                    PatientVisit.AttendingDoctor.IDNumber = result.careTeam.First().code;
                    if (string.IsNullOrEmpty(PatientVisit.AttendingDoctor.IDNumber))
                        PatientVisit.AttendingDoctor.IDNumber = " ";
                    PatientVisit.AttendingDoctor.FamilyName = result.careTeam.First().lastName;
                    if (string.IsNullOrEmpty(PatientVisit.AttendingDoctor.FamilyName))
                        PatientVisit.AttendingDoctor.FamilyName = " ";
                    PatientVisit.AttendingDoctor.GivenName = result.careTeam.First().firstName;
                    if (string.IsNullOrEmpty(PatientVisit.AttendingDoctor.GivenName))
                        PatientVisit.AttendingDoctor.GivenName = " ";
                    
                }
                else
                {
                    PatientVisit.AttendingDoctor.IDNumber = " ";
                    PatientVisit.AttendingDoctor.FamilyName = " ";
                    PatientVisit.AttendingDoctor.GivenName = " ";
                    PatientVisit.AttendingDoctor.Initials = " ";
                }
                PatientVisit.AttendingDoctor.IDNumber = "4782";
                Adt_A04_Msg.EVN = new HL7_EventType();
                Adt_A04_Msg.EVN.RecordedDateTime = DateTime.Now.ToString("yyyyMMddHHmm");
                Adt_A04_Msg.EVN.DateTimeOccurred = DateTime.Now.ToString("yyyyMMddHHmm");
                Adt_A04_Msg.PID = patient;
                Adt_A04_Msg.PV1 = PatientVisit;
                return Adt_A04_Msg;

                #region commented code
                //patient.PatientID = "CEU-000001";
                //patient.MRNNumber = "CEU-000001";
                //patient.FamilyName = "XYZ";
                //patient.GivenName = "YZX";
                //patient.Initials = "SYED";
                //patient.Dateofbirth = DateTime.Now;
                //patient.AdministrativeSex = "M";
                //patient.MaritalStatus = "Single";
                //patient.AccountNumber = "CEU-000001";
                //patient.SSNNumber = "12345678";
                //if (contract.primaryAddress != null)
                //{
                //patient.Address.addressType = contract.primaryAddress.parmAddressType;
                //patient.Address.city = "Riyad";
                //patient.Address.state = "SAU";
                //patient.Address.street = "ABD";
                //patient.Address.country = "SAU";
                //patient.Address.zipCode = "FDDG3434";
                //} 
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string getPatientRecIdFromGuid(string patientGuId)
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(Account.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_axrefrecid");

            query.Criteria.AddCondition("accountid", ConditionOperator.Equal, new Guid(patientGuId));

            EntityCollection entitycol = repo.GetEntityCollection(query);

            if (entitycol.Entities.Count > 0)
            {
                return entitycol.Entities[0]["mzk_axrefrecid"].ToString();
            }

            return string.Empty;
        }
    }

}
