using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class HL7_ORM_O01
    {
        public HL7_MSH MSH
        {
            get;
            set;
        }
        public HL7_EventType EventType
        {
            get;
            set;
        }
        public HL7_Patient PID { set; get; }
        public HL7_PatientVIsit PV1 { set; get; }
        public HL7_ORC ORC { set; get; }
        public HL7_OBR OBR { set; get; }
        public HL7_NTE NTE { set; get; }
        public string MsgControlId { set; get; }
        public async Task<HL7_ORM_O01> GetRIS_ORM_O01_Message(string patientId, string orderId, string appointmentRecId, string caseId, string OrderStatus)
        {

            PatientRadiologyOrder pOrd = new PatientRadiologyOrder();
            HL7_ORM_O01 ORM_O01_Msg = new HL7_ORM_O01();
            var resultorder = await pOrd.getPatientOrder(string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MinValue, false, orderId, null,true);
            foreach (PatientRadiologyOrder pOrder in resultorder)
            {

                ORM_O01_Msg.MSH = new HL7_MSH();

                ORM_O01_Msg.EventType = new HL7_EventType();
                ORM_O01_Msg.EventType.DateTimeOccurred= DateTime.Now.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.EventType.RecordedDateTime = DateTime.Now.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.EventType.DateTimePlanneEvent = DateTime.Now.ToString("yyyyMMddHHmm");


                ORM_O01_Msg.ORC = new HL7_ORC();
                switch (pOrder.OrderStatus)
                {
                    case "1":
                        ORM_O01_Msg.ORC.OrderStatus = "SC";
                        ORM_O01_Msg.ORC.OrderControlCode = "NW";
                        break;
                    case "2":
                        ORM_O01_Msg.ORC.OrderStatus = "CA";
                        ORM_O01_Msg.ORC.OrderControlCode = "CA";
                        break;
                    case "3":
                        ORM_O01_Msg.ORC.OrderStatus = "SC";
                        ORM_O01_Msg.ORC.OrderControlCode = "NW";
                        break;
                    case "11":
                        ORM_O01_Msg.ORC.OrderStatus = "SC";
                        ORM_O01_Msg.ORC.OrderControlCode = "NW";
                        break;
                    case "12":
                        ORM_O01_Msg.ORC.OrderStatus = "CM";
                        ORM_O01_Msg.ORC.OrderControlCode = "NW";
                        break;
                    default:
                        break;
                }
                ORM_O01_Msg.ORC.StartDateTime = pOrder.ScheduleStartDateTime.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.ORC.EndDateTime = pOrder.ScheduleEndDateTime.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.ORC.PlacerOrderNumber = new HL7_OrderNumber();
                ORM_O01_Msg.ORC.PlacerOrderNumber.ID = pOrder.OrderNumber;
                ORM_O01_Msg.ORC.PlacerOrderNumber.NamespaceID = "NA";
                ORM_O01_Msg.ORC.FillerOrderNumber = new HL7_OrderNumber();
                ORM_O01_Msg.ORC.FillerOrderNumber.ID = pOrder.OrderNumber;
                ORM_O01_Msg.ORC.FillerOrderNumber.NamespaceID = "NA";
                ORM_O01_Msg.ORC.OrderingProvider = new HL7_Provider();

                if (pOrder.orderingProvider != null)
                {
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.lastName))
                        pOrder.orderingProvider.lastName = "NA";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.firstName))
                        pOrder.orderingProvider.firstName = "NA";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.middleName))
                        pOrder.orderingProvider.middleName = "NA";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.code))
                        pOrder.orderingProvider.code = "NA";

                    ORM_O01_Msg.ORC.OrderingProvider.FamilyName = pOrder.orderingProvider.lastName;
                    ORM_O01_Msg.ORC.OrderingProvider.GivenName = pOrder.orderingProvider.firstName;
                    ORM_O01_Msg.ORC.OrderingProvider.Initial = pOrder.orderingProvider.middleName;
                    ORM_O01_Msg.ORC.OrderingProvider.ID = pOrder.orderingProvider.code;
                }
                else
                {
                    ORM_O01_Msg.ORC.OrderingProvider.FamilyName = "NA";
                    ORM_O01_Msg.ORC.OrderingProvider.GivenName = "NA";
                    ORM_O01_Msg.ORC.OrderingProvider.Initial = "NA";
                    ORM_O01_Msg.ORC.OrderingProvider.ID = "NA";
                }


                ORM_O01_Msg.ORC.EnteredBy = new HL7_Provider();
                ORM_O01_Msg.ORC.EnteredBy.FamilyName = "NA";
                ORM_O01_Msg.ORC.EnteredBy.GivenName = "NA";
                ORM_O01_Msg.ORC.EnteredBy.ID = "NA";
                ORM_O01_Msg.ORC.EnteredBy.Initial = "NA";

                
                ORM_O01_Msg.ORC.OrderingFacility = new HL7_OrderingFacility();
                ORM_O01_Msg.ORC.OrderingFacility.AssigningAuthority = "NA";
                ORM_O01_Msg.ORC.OrderingFacility.ID = "NA";

                ORM_O01_Msg.OBR = new HL7_OBR();
                ORM_O01_Msg.OBR.FillerOrderNumber = new HL7_OrderNumber();
                ORM_O01_Msg.OBR.PlacerOrderNumber = new HL7_OrderNumber();
                ORM_O01_Msg.OBR.UniversalServiceIdentifier = new HL7_UniversalServiceIdentifier();

                if (string.IsNullOrEmpty(pOrder.ProductFamilyCode))
                {
                    pOrder.ProductFamilyCode = "NA";
                }

                ORM_O01_Msg.OBR.DiagnosticServSectId = pOrder.ProductFamilyCode;

                ORM_O01_Msg.OBR.ID = "NA";
                if (string.IsNullOrEmpty(pOrder.ClinicalNotes))
                {
                    pOrder.ClinicalNotes = "NA";
                }
                if (string.IsNullOrEmpty(pOrder.OrderNumber))
                {
                    pOrder.OrderNumber = "NA";
                }
                ORM_O01_Msg.OBR.Accessionnumber = pOrder.Id; 
                ORM_O01_Msg.OBR.PlacerField1 = pOrder.Id;
              //  ORM_O01_Msg.OBR.PlacerField1 =  pOrder.OrderNumber;
                ORM_O01_Msg.OBR.PlacerField2 = pOrder.Id;
                ORM_O01_Msg.OBR.FillderField1 = pOrder.OrderNumber;
                ORM_O01_Msg.OBR.FillerOrderNumber.ID = pOrder.OrderNumber;
                ORM_O01_Msg.OBR.FillerOrderNumber.NamespaceID = "NA";
                ORM_O01_Msg.OBR.ResultStatusCode = "NA";
                ORM_O01_Msg.OBR.StartDateTime = pOrder.ScheduleStartDateTime.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.OBR.EndDateTime = pOrder.ScheduleEndDateTime.ToString("yyyyMMddHHmm");
                if (string.IsNullOrEmpty(pOrder.TestId))
                {
                    pOrder.TestId = "NA";
                }
                if (string.IsNullOrEmpty(pOrder.TestName))
                {
                    pOrder.TestName = "NA";
                }


                ORM_O01_Msg.OBR.UniversalServiceIdentifier.ID = pOrder.TestId;
                ORM_O01_Msg.OBR.UniversalServiceIdentifier.Text = pOrder.TestName;
                ORM_O01_Msg.OBR.UniversalServiceIdentifier.Nameofcodingsystem = "NA";

                ORM_O01_Msg.OBR.ReasonforStudy = "NA";
                if (string.IsNullOrEmpty(pOrder.ClinicalNotes))
                    pOrder.ClinicalNotes = "NA";
                ORM_O01_Msg.OBR.RelevantClinicalInf = Helper.encodeEscapeChar(pOrder.ClinicalNotes);

                ORM_O01_Msg.OBR.ScheduledDateTime = pOrder.ScheduleStartDateTime.ToString("yyyyMMddHHmm");//we update start date
                ORM_O01_Msg.OBR.UniversalServiceIdentifier.AlternateIdentifier = "NA";
                ORM_O01_Msg.OBR.UniversalServiceIdentifier.AlternateText = "NA";
                if (string.IsNullOrEmpty(pOrder.OrderNumber))
                    ORM_O01_Msg.OBR.PlacerOrderNumber.ID = "NA";
                else
                    ORM_O01_Msg.OBR.PlacerOrderNumber.ID = pOrder.OrderNumber;
                
                ORM_O01_Msg.OBR.FillerOrderNumber.NamespaceID = "NA";
                ORM_O01_Msg.OBR.PlacerOrderNumber.NamespaceID = "NA";

                if (string.IsNullOrEmpty(pOrder.OrderNumber))
                    ORM_O01_Msg.OBR.FillerOrderNumber.ID = "NA";
                else
                    ORM_O01_Msg.OBR.FillerOrderNumber.ID = pOrder.OrderNumber;

                ORM_O01_Msg.ORC.PlacerOrderNumber.NamespaceID = "NA";
                ORM_O01_Msg.ORC.PlacerGroupNumber = "NA";
                ORM_O01_Msg.ORC.FillerOrderNumber.NamespaceID = "NA";

                ORM_O01_Msg.ORC.EnteringOrganization =new HL7_EnteringOrganization();
                ORM_O01_Msg.ORC.EnteringOrganization.ID = "NazirBupa";
                ORM_O01_Msg.ORC.EnteringOrganization.Nameofcodingsystem = "";
                ORM_O01_Msg.ORC.EnteringOrganization.Text = "NazirBupa";

                ORM_O01_Msg.OBR.OrderingProvider = new HL7_Provider();

                if (pOrder.orderingProvider != null)
                {
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.lastName))
                        pOrder.orderingProvider.lastName = "NA";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.firstName))
                        pOrder.orderingProvider.firstName = "NA";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.middleName))
                        pOrder.orderingProvider.middleName = "NA";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.code))
                        pOrder.orderingProvider.code = "NA";

                    ORM_O01_Msg.OBR.OrderingProvider.FamilyName = pOrder.orderingProvider.lastName;
                    ORM_O01_Msg.OBR.OrderingProvider.GivenName = pOrder.orderingProvider.firstName;
                    ORM_O01_Msg.OBR.OrderingProvider.Initial = pOrder.orderingProvider.middleName;
                    ORM_O01_Msg.OBR.OrderingProvider.ID = pOrder.orderingProvider.code;

                }
                else
                {
                    ORM_O01_Msg.OBR.OrderingProvider.FamilyName ="NA";
                    ORM_O01_Msg.OBR.OrderingProvider.GivenName = "NA";
                    ORM_O01_Msg.OBR.OrderingProvider.Initial = "NA";
                    ORM_O01_Msg.OBR.OrderingProvider.ID = "NA";
                }
                ORM_O01_Msg.ORC.TransactionDateTime = DateTime.Now.ToString("yyyyMMddHHmm");
                if (pOrder.UrgencyId == "1")
                    ORM_O01_Msg.OBR.Priority = "R";
                else if (pOrder.UrgencyId == "2")
                    ORM_O01_Msg.OBR.Priority = "A";
                else
                    ORM_O01_Msg.OBR.Priority = "";
                if (string.IsNullOrEmpty(pOrder.LocationCode))
                    ORM_O01_Msg.OBR.OrderLocation = "";
                else
                ORM_O01_Msg.OBR.OrderLocation = pOrder.LocationCode;

                break;
            }

            ADT_A04_Message _obj_A04_Message = new ADT_A04_Message();
            var adt = await _obj_A04_Message.GetADT_A04_Message(appointmentRecId, "", patientId);
            ORM_O01_Msg.PID = adt.PID;
            ORM_O01_Msg.PV1 = adt.PV1;
            if (ORM_O01_Msg.OBR.OrderingProvider != null)
            {
                adt.PV1.AttendingDoctor.IDNumber = ORM_O01_Msg.OBR.OrderingProvider.ID;
                if (string.IsNullOrEmpty(adt.PV1.AttendingDoctor.GivenName))
                    adt.PV1.AttendingDoctor.IDNumber = "NA";
                adt.PV1.AttendingDoctor.FamilyName = ORM_O01_Msg.OBR.OrderingProvider.FamilyName;
                if (string.IsNullOrEmpty(adt.PV1.AttendingDoctor.GivenName))
                    adt.PV1.AttendingDoctor.FamilyName = "NA";
                adt.PV1.AttendingDoctor.GivenName = ORM_O01_Msg.OBR.OrderingProvider.GivenName;
                if (string.IsNullOrEmpty(adt.PV1.AttendingDoctor.GivenName))
                    adt.PV1.AttendingDoctor.GivenName = "NA";
            }
            else
            {
                adt.PV1.AttendingDoctor.GivenName = "NA";
                adt.PV1.AttendingDoctor.FamilyName = "NA";
                adt.PV1.AttendingDoctor.IDNumber = "NA";
            }

            ORM_O01_Msg.NTE = new HL7_NTE();
            ORM_O01_Msg.NTE.SetID = "NA";
            ORM_O01_Msg.NTE.Comment = "NA";
            ORM_O01_Msg.NTE.CommentType = "PC"; //CI,PC
            ORM_O01_Msg.NTE.Identifier = "NA";

           
            return ORM_O01_Msg;

            #region defualt values
            //HL7_ORM_O01 ORM_O01_Msg = new HL7_ORM_O01();
            //ORM_O01_Msg.MSH = new HL7_MSH();
            //ORM_O01_Msg.EventType = new HL7_EventType();
            //ORM_O01_Msg.ORC = new HL7_ORC();
            //ORM_O01_Msg.ORC.OrderControlCode = "NW";
            //ORM_O01_Msg.ORC.OrderStatus = "SC";
            //ORM_O01_Msg.ORC.StartDateTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            //ORM_O01_Msg.ORC.EndDateTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            //ORM_O01_Msg.ORC.TransactionDateTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            //ORM_O01_Msg.ORC.PlacerOrderNumber = new HL7_OrderNumber();
            //ORM_O01_Msg.ORC.PlacerOrderNumber.NamespaceID = "EPIC";
            //ORM_O01_Msg.ORC.PlacerOrderNumber.ID = "987654";
            //ORM_O01_Msg.ORC.FillerOrderNumber = new HL7_OrderNumber();
            //ORM_O01_Msg.ORC.FillerOrderNumber.NamespaceID = "EPC";
            //ORM_O01_Msg.ORC.FillerOrderNumber.ID = "76543";
            //ORM_O01_Msg.ORC.OrderingProvider = new HL7_Provider();
            //ORM_O01_Msg.ORC.OrderingProvider.ID = "1173";
            //ORM_O01_Msg.ORC.OrderingProvider.FamilyName = "MATTHEWS";
            //ORM_O01_Msg.ORC.OrderingProvider.GivenName = "JAMES";
            //ORM_O01_Msg.ORC.OrderingProvider.Initial = "A";

            //ORM_O01_Msg.ORC.EnteredBy = new HL7_Provider();
            //ORM_O01_Msg.ORC.EnteredBy.FamilyName = "PATTERSON";
            //ORM_O01_Msg.ORC.EnteredBy.GivenName = "JAMES";
            //ORM_O01_Msg.ORC.OrderingProvider = new HL7_Provider();
            //ORM_O01_Msg.ORC.OrderingFacility = new HL7_OrderingFacility();

            //ORM_O01_Msg.OBR = new HL7_OBR();
            //ORM_O01_Msg.OBR.FillerOrderNumber = new HL7_OrderNumber();
            //ORM_O01_Msg.OBR.PlacerOrderNumber = new HL7_OrderNumber();
            //ORM_O01_Msg.OBR.UniversalServiceIdentifier = new HL7_UniversalServiceIdentifier();
            //ORM_O01_Msg.OBR.OrderingProvider = new HL7_Provider();
            //ORM_O01_Msg.OBR.ID = "1";
            //ORM_O01_Msg.OBR.PlacerField2 = "Placer";
            //ORM_O01_Msg.OBR.FillderField1 = "Placer+";
            //ORM_O01_Msg.OBR.ResultStatusCode = "R";
            //ORM_O01_Msg.OBR.StartDateTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            //ORM_O01_Msg.OBR.EndDateTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            //ORM_O01_Msg.OBR.UniversalServiceIdentifier.Text = "MRI Abdomen with Contrast";
            //ORM_O01_Msg.OBR.UniversalServiceIdentifier.AlternateIdentifier = "MI-MR-0002";
            //ORM_O01_Msg.OBR.PlacerOrderNumber.ID = "363463";
            //ORM_O01_Msg.OBR.PlacerOrderNumber.NamespaceID = "EPC";
            //ORM_O01_Msg.OBR.FillerOrderNumber.NamespaceID = "MI-MR-0001";
            //ORM_O01_Msg.OBR.FillerOrderNumber.ID = "1858";
            //ORM_O01_Msg.OBR.OrderingProvider.FamilyName = "MATTHEWS";
            //ORM_O01_Msg.OBR.OrderingProvider.GivenName = "JAMES";
            //ORM_O01_Msg.OBR.OrderingProvider.Initial = "A";
            //ORM_O01_Msg.OBR.OrderingProvider.ID = "1173";
            //ORM_O01_Msg.PID.Address = new Address();
            //ORM_O01_Msg.PID.PhoneNumberHome = new HL7_Contact();
            //ORM_O01_Msg.PID.PhoneNumberBusiness = new HL7_Contact();

            #endregion
        }

        public async Task<HL7_ORM_O01> GetLIS_ORM_O01_Message(string patientId, string orderId, string appointmentRecId, string caseId, string OrderStatus)
        {

            PatientLabOrder pOrd = new PatientLabOrder();
            HL7_ORM_O01 ORM_O01_Msg = new HL7_ORM_O01();
            var resultorder = await pOrd.getPatientOrder(string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MinValue, false, orderId, null);
            foreach (PatientLabOrder pOrder in resultorder)
            {

                ORM_O01_Msg.MSH = new HL7_MSH();

                ORM_O01_Msg.EventType = new HL7_EventType();
                ORM_O01_Msg.EventType.DateTimeOccurred = DateTime.Now.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.EventType.RecordedDateTime = DateTime.Now.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.EventType.DateTimePlanneEvent = DateTime.Now.ToString("yyyyMMddHHmm");


                ORM_O01_Msg.ORC = new HL7_ORC();
                switch (pOrder.OrderStatus)
                {
                    case "1":
                        ORM_O01_Msg.ORC.OrderStatus = "NW";
                        ORM_O01_Msg.ORC.OrderControlCode = "NW";
                        break;
                    case "2":
                        ORM_O01_Msg.ORC.OrderStatus = "CA";
                        ORM_O01_Msg.ORC.OrderControlCode = "CA";
                        break;
                    case "3":
                        ORM_O01_Msg.ORC.OrderStatus = "NW";
                        ORM_O01_Msg.ORC.OrderControlCode = "NW";
                        break;
                    case "11":
                        ORM_O01_Msg.ORC.OrderStatus = "NW";
                        ORM_O01_Msg.ORC.OrderControlCode = "NW";
                        break;
                    case "12":
                        ORM_O01_Msg.ORC.OrderStatus = "CM";
                        ORM_O01_Msg.ORC.OrderControlCode = "NW";
                        break;
                    default:
                        break;
                }
                ORM_O01_Msg.ORC.StartDateTime = pOrder.OrderDate.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.ORC.EndDateTime = pOrder.OrderDate.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.ORC.PlacerOrderNumber = new HL7_OrderNumber();
                ORM_O01_Msg.ORC.PlacerOrderNumber.ID = pOrder.OrderNumber;
                ORM_O01_Msg.ORC.PlacerOrderNumber.NamespaceID = " ";
                ORM_O01_Msg.ORC.FillerOrderNumber = new HL7_OrderNumber();
                ORM_O01_Msg.ORC.FillerOrderNumber.ID = pOrder.OrderNumber;
                ORM_O01_Msg.ORC.FillerOrderNumber.NamespaceID = " ";
                ORM_O01_Msg.ORC.OrderingProvider = new HL7_Provider();

                if (pOrder.orderingProvider != null)
                {
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.lastName))
                        pOrder.orderingProvider.lastName = " ";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.firstName))
                        pOrder.orderingProvider.firstName = " ";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.middleName))
                        pOrder.orderingProvider.middleName = " ";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.code))
                        pOrder.orderingProvider.code = " ";

                    ORM_O01_Msg.ORC.OrderingProvider.FamilyName = pOrder.orderingProvider.lastName;
                    ORM_O01_Msg.ORC.OrderingProvider.GivenName = pOrder.orderingProvider.firstName;
                    ORM_O01_Msg.ORC.OrderingProvider.Initial = pOrder.orderingProvider.middleName;
                    ORM_O01_Msg.ORC.OrderingProvider.ID = pOrder.orderingProvider.code;
                }
                else
                {
                    ORM_O01_Msg.ORC.OrderingProvider.FamilyName = " ";
                    ORM_O01_Msg.ORC.OrderingProvider.GivenName = " ";
                    ORM_O01_Msg.ORC.OrderingProvider.Initial = " ";
                    ORM_O01_Msg.ORC.OrderingProvider.ID = " ";
                }


                ORM_O01_Msg.ORC.EnteredBy = new HL7_Provider();
                ORM_O01_Msg.ORC.EnteredBy.FamilyName = " ";
                ORM_O01_Msg.ORC.EnteredBy.GivenName = " ";
                ORM_O01_Msg.ORC.EnteredBy.ID = " ";
                ORM_O01_Msg.ORC.EnteredBy.Initial = " ";


                ORM_O01_Msg.ORC.OrderingFacility = new HL7_OrderingFacility();
                ORM_O01_Msg.ORC.OrderingFacility.AssigningAuthority = " ";
                ORM_O01_Msg.ORC.OrderingFacility.ID = " ";

                ORM_O01_Msg.OBR = new HL7_OBR();
                ORM_O01_Msg.OBR.FillerOrderNumber = new HL7_OrderNumber();
                ORM_O01_Msg.OBR.PlacerOrderNumber = new HL7_OrderNumber();
                ORM_O01_Msg.OBR.UniversalServiceIdentifier = new HL7_UniversalServiceIdentifier();


                ORM_O01_Msg.OBR.ID = " ";
                if (string.IsNullOrEmpty(pOrder.ClinicalNotes))
                {
                    pOrder.ClinicalNotes = " ";
                }
                if (string.IsNullOrEmpty(pOrder.OrderNumber))
                {
                    pOrder.OrderNumber = " ";
                }
                ORM_O01_Msg.OBR.Accessionnumber = " ";
                ORM_O01_Msg.OBR.PlacerField1 = Helper.encodeEscapeChar(pOrder.ClinicalNotes);
                //  ORM_O01_Msg.OBR.PlacerField1 =  pOrder.OrderNumber;
                ORM_O01_Msg.OBR.PlacerField2 = pOrder.Id;
                ORM_O01_Msg.OBR.FillderField1 = pOrder.OrderNumber;
                ORM_O01_Msg.OBR.FillerOrderNumber.ID = pOrder.OrderNumber;
                ORM_O01_Msg.OBR.FillerOrderNumber.NamespaceID = " ";
                ORM_O01_Msg.OBR.ResultStatusCode = " ";
                ORM_O01_Msg.OBR.StartDateTime = pOrder.OrderDate.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.OBR.EndDateTime = pOrder.OrderDate.ToString("yyyyMMddHHmm");
                if (string.IsNullOrEmpty(pOrder.TestId))
                {
                    pOrder.TestId = " ";
                }
                if (string.IsNullOrEmpty(pOrder.TestName))
                {
                    pOrder.TestName = " ";
                }

                ORM_O01_Msg.OBR.UniversalServiceIdentifier.ID = pOrder.TestId;
               
                ORM_O01_Msg.OBR.UniversalServiceIdentifier.Text = pOrder.TestName;
                ORM_O01_Msg.OBR.UniversalServiceIdentifier.Nameofcodingsystem = " ";

                ORM_O01_Msg.OBR.ReasonforStudy = " ";
                ORM_O01_Msg.OBR.RelevantClinicalInf = " ";
                ORM_O01_Msg.OBR.ScheduledDateTime = DateTime.Now.ToString("yyyyMMddHHmm");
                ORM_O01_Msg.OBR.UniversalServiceIdentifier.AlternateIdentifier = " ";
                ORM_O01_Msg.OBR.UniversalServiceIdentifier.AlternateText = " ";

                if (string.IsNullOrEmpty(pOrder.OrderNumber))
                    ORM_O01_Msg.OBR.PlacerOrderNumber.ID = " ";
                else
                    ORM_O01_Msg.OBR.PlacerOrderNumber.ID = pOrder.OrderNumber;

                ORM_O01_Msg.OBR.FillerOrderNumber.NamespaceID = " ";
                ORM_O01_Msg.OBR.PlacerOrderNumber.NamespaceID = " ";

                if (string.IsNullOrEmpty(pOrder.OrderNumber))
                    ORM_O01_Msg.OBR.FillerOrderNumber.ID = " ";
                else
                    ORM_O01_Msg.OBR.FillerOrderNumber.ID = pOrder.OrderNumber;
                ORM_O01_Msg.ORC.PlacerOrderNumber.NamespaceID = " ";
                ORM_O01_Msg.ORC.PlacerGroupNumber = " ";
                ORM_O01_Msg.ORC.FillerOrderNumber.NamespaceID = " ";

                ORM_O01_Msg.ORC.EnteringOrganization = new HL7_EnteringOrganization();
                ORM_O01_Msg.ORC.EnteringOrganization.ID = "NazirBupa";
                ORM_O01_Msg.ORC.EnteringOrganization.Nameofcodingsystem = "";
                ORM_O01_Msg.ORC.EnteringOrganization.Text = "NazirBupa";

                ORM_O01_Msg.OBR.OrderingProvider = new HL7_Provider();

                if (pOrder.orderingProvider != null)
                {
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.lastName))
                        pOrder.orderingProvider.lastName = " ";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.firstName))
                        pOrder.orderingProvider.firstName = " ";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.middleName))
                        pOrder.orderingProvider.middleName = " ";
                    if (string.IsNullOrEmpty(pOrder.orderingProvider.code))
                        pOrder.orderingProvider.code = " ";

                    ORM_O01_Msg.OBR.OrderingProvider.FamilyName = pOrder.orderingProvider.lastName;
                    ORM_O01_Msg.OBR.OrderingProvider.GivenName = pOrder.orderingProvider.firstName;
                    ORM_O01_Msg.OBR.OrderingProvider.Initial = pOrder.orderingProvider.middleName;
                    ORM_O01_Msg.OBR.OrderingProvider.ID = pOrder.orderingProvider.code;

                }
                else
                {
                    ORM_O01_Msg.OBR.OrderingProvider.FamilyName = " ";
                    ORM_O01_Msg.OBR.OrderingProvider.GivenName = " ";
                    ORM_O01_Msg.OBR.OrderingProvider.Initial = " ";
                    ORM_O01_Msg.OBR.OrderingProvider.ID = " ";
                }
                ORM_O01_Msg.ORC.TransactionDateTime = pOrder.OrderDate.ToString("yyyyMMddHHmm");
                if (pOrder.UrgencyId == "1")
                    ORM_O01_Msg.OBR.Priority = "R";
                else if (pOrder.UrgencyId == "2")
                ORM_O01_Msg.OBR.Priority = "S";
                else
                    ORM_O01_Msg.OBR.Priority = "";
                if (string.IsNullOrEmpty(pOrder.LocationCode))
                    ORM_O01_Msg.OBR.OrderLocation = "";
                else
                    ORM_O01_Msg.OBR.OrderLocation = pOrder.LocationCode;
                break;

            }

            ADT_A04_Message _obj_A04_Message = new ADT_A04_Message();
            var result = await _obj_A04_Message.GetADT_A04_Message(appointmentRecId, "", patientId);
            ORM_O01_Msg.PID = result.PID;
            ORM_O01_Msg.PV1 = result.PV1;
            ORM_O01_Msg.NTE = new HL7_NTE();
            ORM_O01_Msg.NTE.SetID = " ";
            ORM_O01_Msg.NTE.Comment = " ";
            ORM_O01_Msg.NTE.CommentType = "PC"; //CI,PC
            ORM_O01_Msg.NTE.Identifier = " ";
            return ORM_O01_Msg;

            #region defualt values
            //HL7_ORM_O01 ORM_O01_Msg = new HL7_ORM_O01();
            //ORM_O01_Msg.MSH = new HL7_MSH();
            //ORM_O01_Msg.EventType = new HL7_EventType();
            //ORM_O01_Msg.ORC = new HL7_ORC();
            //ORM_O01_Msg.ORC.OrderControlCode = "NW";
            //ORM_O01_Msg.ORC.OrderStatus = "SC";
            //ORM_O01_Msg.ORC.StartDateTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            //ORM_O01_Msg.ORC.EndDateTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            //ORM_O01_Msg.ORC.TransactionDateTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            //ORM_O01_Msg.ORC.PlacerOrderNumber = new HL7_OrderNumber();
            //ORM_O01_Msg.ORC.PlacerOrderNumber.NamespaceID = "EPIC";
            //ORM_O01_Msg.ORC.PlacerOrderNumber.ID = "987654";
            //ORM_O01_Msg.ORC.FillerOrderNumber = new HL7_OrderNumber();
            //ORM_O01_Msg.ORC.FillerOrderNumber.NamespaceID = "EPC";
            //ORM_O01_Msg.ORC.FillerOrderNumber.ID = "76543";
            //ORM_O01_Msg.ORC.OrderingProvider = new HL7_Provider();
            //ORM_O01_Msg.ORC.OrderingProvider.ID = "1173";
            //ORM_O01_Msg.ORC.OrderingProvider.FamilyName = "MATTHEWS";
            //ORM_O01_Msg.ORC.OrderingProvider.GivenName = "JAMES";
            //ORM_O01_Msg.ORC.OrderingProvider.Initial = "A";

            //ORM_O01_Msg.ORC.EnteredBy = new HL7_Provider();
            //ORM_O01_Msg.ORC.EnteredBy.FamilyName = "PATTERSON";
            //ORM_O01_Msg.ORC.EnteredBy.GivenName = "JAMES";
            //ORM_O01_Msg.ORC.OrderingProvider = new HL7_Provider();
            //ORM_O01_Msg.ORC.OrderingFacility = new HL7_OrderingFacility();

            //ORM_O01_Msg.OBR = new HL7_OBR();
            //ORM_O01_Msg.OBR.FillerOrderNumber = new HL7_OrderNumber();
            //ORM_O01_Msg.OBR.PlacerOrderNumber = new HL7_OrderNumber();
            //ORM_O01_Msg.OBR.UniversalServiceIdentifier = new HL7_UniversalServiceIdentifier();
            //ORM_O01_Msg.OBR.OrderingProvider = new HL7_Provider();
            //ORM_O01_Msg.OBR.ID = "1";
            //ORM_O01_Msg.OBR.PlacerField2 = "Placer";
            //ORM_O01_Msg.OBR.FillderField1 = "Placer+";
            //ORM_O01_Msg.OBR.ResultStatusCode = "R";
            //ORM_O01_Msg.OBR.StartDateTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            //ORM_O01_Msg.OBR.EndDateTime = System.DateTime.Now.ToString("yyyyMMddHHmm");
            //ORM_O01_Msg.OBR.UniversalServiceIdentifier.Text = "MRI Abdomen with Contrast";
            //ORM_O01_Msg.OBR.UniversalServiceIdentifier.AlternateIdentifier = "MI-MR-0002";
            //ORM_O01_Msg.OBR.PlacerOrderNumber.ID = "363463";
            //ORM_O01_Msg.OBR.PlacerOrderNumber.NamespaceID = "EPC";
            //ORM_O01_Msg.OBR.FillerOrderNumber.NamespaceID = "MI-MR-0001";
            //ORM_O01_Msg.OBR.FillerOrderNumber.ID = "1858";
            //ORM_O01_Msg.OBR.OrderingProvider.FamilyName = "MATTHEWS";
            //ORM_O01_Msg.OBR.OrderingProvider.GivenName = "JAMES";
            //ORM_O01_Msg.OBR.OrderingProvider.Initial = "A";
            //ORM_O01_Msg.OBR.OrderingProvider.ID = "1173";
            //ORM_O01_Msg.PID.Address = new Address();
            //ORM_O01_Msg.PID.PhoneNumberHome = new HL7_Contact();
            //ORM_O01_Msg.PID.PhoneNumberBusiness = new HL7_Contact();

            #endregion
        }

    }
}
