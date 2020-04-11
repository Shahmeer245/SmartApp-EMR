using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models.HL7
{
    public class HL7_Patient
    {

        public string PatientID
        {
            get;
            set;
        }
        public string MRNNumber
        {
            get;
            set;
        }
        public string AssingingAuthority
        {
            get;
            set;
        }
        public string AssigningAuthorityIdentifierCode { get; set; }
        public string FamilyName
        {
            get;
            set;
        }
        public string GivenName
        {
            get;
            set;
        }
        public string Initials
        {
            get;
            set;
        }
        public string Suffix
        {
            get;
            set;
        }
        public string Dateofbirth
        {
            get;
            set;
        }
        public string AdministrativeSex
        {
            get;
            set;
        }
        public string Race
        {
            get;
            set;
        }
        public HL7_Address Address
        {
            get;
            set;
        }
        public string MaritalStatus
        {
            get;
            set;
        }
        public string AccountNumber
        {
            get;
            set;
        }
        public string SSNNumber
        {
            get;
            set;
        }
        public HL7_Contact PhoneNumberHome { set; get; }
        public HL7_Contact PhoneNumberBusiness { set; get; }
    }
}
