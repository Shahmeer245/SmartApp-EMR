using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Models.HL7;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class Patient
    {
        #region Properties
        public string name
        {
            get; set;
        }

        public string firstName
        {
            get; set;
        }

        public string lastName
        {
            get; set;
        }

        public string middleName
        {
            get; set;
        }

        public string salutation
        {
            get; set;
        }

        public string phone
        {
            get; set;
        }

        public DateTime dateOfBirth
        {
            get; set;
        }

        public string language
        {
            get; set;
        }

        public string age
        {
            get; set;
        }

        public string patientId
        {
            get; set;
        }

        public string mrn
        {
            get; set;
        }

        public string gender
        {
            get; set;
        }

        public int genderValue
        {
            get; set;
        }

        public string nationality
        {
            get; set;
        }

        public string vip
        {
            get; set;
        }

        public string arabicFirstName
        {
            get; set;
        }

        public string arabicLastName
        {
            get; set;
        }

        public string arabicMiddleName
        {
            get; set;
        }

        public string hijriDateOfBirth
        {
            get; set;
        }

        public List<Document> patientDocuments
        {
            get; set;
        }

        public List<Address> addresses
        {
            get; set;
        }

        public List<Relationship> patientRelationShips
        {
            get; set;
        }


        public DateTime registrationDate
        {
            get; set;
        }

        public string maritalStatus
        {
            get; set;
        }

        public string nationalIdType
        {
            get; set;
        }

        public int nationalIdTypeValue
        {
            get; set;
        }
        
        public string nationalIdValue
        {
            get; set;
        }

        public string nationalIdExpirationDate
        {
            get; set;
        }

        public string nationalIdExpirationDateHijri
        {
            get; set;
        }

        public string preferredModeOfCommunication
        {
            get; set;
        }

        public string fax
        {
            get; set;
        }

        public string email
        {
            get; set;
        }

        public string mobile
        {
            get; set;
        }

        public string workPhone
        {
            get; set;
        }

        public string homePhone
        {
            get; set;
        }

        public string arabicName
        {
            get; set;
        }

        public Address primaryAddress
        {
            get; set;
        }
        public string image
        {
            get; set;
        }

        public string emailAddress
        {
            get; set;
        }

        public DateTime ActivityStartDate
        {
            get; set;
        }

        public string Externalemrid { get; set; }

        public string patientDeviceId { get; set; }

        public bool faceRecognitionEnabled { get; set; }
        public bool fingerprintEnabled { get; set; }
        public bool pinCodeEnabled { get; set; }
        public bool agreeToTermsAndCondition { get; set; }
        public bool patientDetailsVerified { get; set; }
        #endregion

        #region Methods
        //TODO createAccount not used
        public async Task<bool> createAccount(Patient patObject)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                Entity account = new Entity(Account.EntityLogicalName);

                account["mzk_axrefrecid"] = Convert.ToDecimal(patObject.patientId);
                account["name"] = patObject.name;
                account["accountnumber"] = patObject.mrn;
                if (patObject.dateOfBirth != DateTime.MinValue)
                    account["mzk_patientdateofbirth"] = patObject.dateOfBirth;

                if (!string.IsNullOrEmpty(patObject.gender))
                    account["mzk_gender"] = new OptionSetValue(Convert.ToInt32(patObject.gender));


                if (!string.IsNullOrEmpty(patObject.arabicName))
                    account["mzk_arabicname"] = patObject.arabicName;



                repo.CreateEntity(account);

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //TODO updateAccount not used
        public async Task<bool> updateAccount(Patient patObject)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(Account.EntityLogicalName);

                query.Criteria.AddCondition("mzk_axrefrecid", ConditionOperator.Equal, Convert.ToDecimal(patObject.patientId));

                EntityCollection entitycollection = repo.GetEntityCollection(query);

                if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                {
                    Entity account = entitycollection.Entities[0];

                    if (!string.IsNullOrEmpty(patObject.name))
                    {
                        account["name"] = patObject.name;
                    }

                    if (patObject.dateOfBirth != DateTime.MinValue)
                        account["mzk_patientdateofbirth"] = patObject.dateOfBirth;

                    if (!string.IsNullOrEmpty(patObject.gender))
                        account["mzk_gender"] = new OptionSetValue(Convert.ToInt32(patObject.gender));

                    if (!string.IsNullOrEmpty(patObject.arabicName))
                        account["mzk_arabicname"] = patObject.arabicName;

                    repo.UpdateEntity(account);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> updatePatientRecId(Patient patObject, long recId)
        {
            try
            {
                //throw new ValidationException(patObject.patientId + " " + recId.ToString());

                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(Account.EntityLogicalName);

                query.Criteria.AddCondition("accountid", ConditionOperator.Equal, patObject.patientId);

                //throw new ValidationException(patObject.patientId.ToString() + " " + recId.ToString());

                EntityCollection entitycollection = repo.GetEntityCollection(query);

                //throw new ValidationException(patObject.patientId.ToString() + " " + recId.ToString() + " - " + entitycollection.Entities.Count.ToString());
                //if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                //{
                //    Entity account = entitycollection.Entities[0];

                //    account["mzk_axrefrecid"] = Convert.ToDecimal(369369369);

                //    repo.UpdateEntity(account);

                //    return true;
                //}

                if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                {
                    xrm.Account account = (xrm.Account)entitycollection.Entities[0];

                    account.mzk_AXRefRecId = Convert.ToDecimal(recId);

                    repo.UpdateEntity(account);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<long> createPatient(Patient patObject)
        {
            try
            {
                PatientRepository repo = new PatientRepository();
                HMPatientDataContract contract = new HMPatientDataContract();
                //HMPatientDocumentContract docContract = null;
                //List<HMPatientDocumentContract> listDocContract = new List<HMPatientDocumentContract>();

                // Patient patientDetails = new Patient().getPatientDetails(patObject.patientId).Result;

                //if (patientDetails != null)
                // {
                //contract.parmFullName = patientDetails.name;
                //contract.parmDateOfBirth = patientDetails.dateOfBirth;
                //contract.parmGenderValue = patientDetails.genderValue;
                //contract.parmNationalIdTypeValue = patientDetails.nationalIdTypeValue;
                //contract.parmNationalIdValue = patientDetails.nationalIdValue;
                //contract.parmMRN = patientDetails.mrn;
                //contract.parmEmail = patientDetails.email;
                //contract.parmHomePhone = patientDetails.phone;

                contract.parmFullName = patObject.name;
                contract.parmDateOfBirth = patObject.dateOfBirth;
                contract.parmGenderValue = patObject.genderValue;
                contract.parmNationalIdTypeValue = patObject.nationalIdTypeValue;
                contract.parmNationalIdValue = patObject.nationalIdValue;
                contract.parmMRN = patObject.mrn;
                contract.parmEmail = patObject.email;
                contract.parmHomePhone = patObject.phone;


                //if(patObject.patientDocuments != null)
                //{
                //    foreach (Document doc in patObject.patientDocuments)
                //    {
                //        docContract = new HMPatientDocumentContract();

                //        docContract.parmName = doc.name;
                //        docContract.parmType = doc.type;
                //        docContract.parmDocument = doc.document;

                //        listDocContract.Add(docContract);
                //    }
                //}

                //contract.parmDocumentList = listDocContract.ToArray();

                return repo.createPatient(contract);
                // }
                // else
                //  return 0;
            }
            catch (Exception ex)
            {
                // Helper.Files.SaveToCSV(ex.Message, "tt", DateTime.Now, DateTime.Now);
                throw ex;
            }
        }

        public static Patient getPatientModelFilled(Contact contact, Patient patient, string accountAlias)
        {
            if (contact.Attributes.Contains(accountAlias + ".mzk_arabicname"))
            {
                patient.arabicName = ((AliasedValue)(contact.Attributes[accountAlias + ".mzk_arabicname"])).Value.ToString();
            }

            if (contact.Attributes.Contains("msemr_ActivityStartDate"))
            {
                patient.ActivityStartDate = (DateTime)(contact.Attributes["msemr_ActivityStartDate"]);
            }

            if (contact.Attributes.Contains("mzk_patientmrn"))
            {
                patient.mrn = (contact.Attributes["mzk_patientmrn"]).ToString();
            }            

            if (contact.Attributes.Contains("createdon"))
            {
                patient.registrationDate = (DateTime)(contact.Attributes["createdon"]);
            }

            if (contact.Attributes.Contains("contactid"))
            {
                patient.patientId = contact.ContactId.ToString();
            }

            if (contact.Attributes.Contains("fullname"))
            {
                patient.name = contact.FullName;
            }

            if (contact.Attributes.Contains("mzk_vippatient"))
            {
                patient.vip = contact.mzk_vippatient.Value.ToString();
            }

            if (contact.Attributes.Contains("entityimage"))
            {                
                patient.image = Convert.ToBase64String(contact.EntityImage);
            }

            if (contact.Attributes.Contains("gendercode"))
            {
                patient.genderValue = contact.GenderCode.Value;
                patient.gender = (contact.FormattedValues["gendercode"]).ToString();
            }
            else
            {
                patient.gender = "";
            }

            if (contact.Attributes.Contains("birthdate"))
            {
                patient.dateOfBirth = Convert.ToDateTime(contact.BirthDate);
                //AgeHelper ageHelper = new AgeHelper(DateTime.Now);
                //patient.age = ageHelper.getAgeInYears(patient.dateOfBirth).ToString() + " years";
            }
            if (contact.Attributes.Contains("mzk_agecalculated"))
            {
                patient.age = contact["mzk_agecalculated"].ToString();
            }
            else
            {
                patient.age = "";
            }

            //if (contact.Attributes.Contains("telephone1"))
            //{
            //    patient.phone = contact.Telephone1;
            //}

            if (contact.Attributes.Contains("fax"))
            {
                patient.fax = contact.Fax;
            }

            if (contact.Attributes.Contains("emailaddress1"))
            {
                patient.email = contact.EMailAddress1;
            }            

            if (contact.Attributes.Contains("mzk_patientlanguage"))
            {
                patient.language = (contact.Attributes["mzk_patientlanguage"] as EntityReference).Name;
            }

            if (contact.Attributes.Contains("familystatuscode"))
            {
                patient.maritalStatus = (contact.FormattedValues["familystatuscode"]).ToString();
            }

            if (contact.Attributes.Contains("mzk_nationality"))
            {
                patient.nationality = contact.mzk_nationality.Name;
            }

            if (contact.Attributes.Contains("mzk_salutation"))
            {
                patient.salutation = contact.Salutation;
            }

            if (contact.Attributes.Contains("mzk_nationalidexpirydate"))
            {
                patient.nationalIdExpirationDate = contact.mzk_nationalidexpirydate.ToString();
            }

            if (contact.Attributes.Contains("mzk_nationalidtype"))
            {
                patient.nationalIdTypeValue = contact.mzk_nationalidtype.Value;
                patient.nationalIdType = (contact.FormattedValues["mzk_nationalidtype"]).ToString();
            }
            
            if (contact.Attributes.Contains("preferredcontactmethodcode"))
            {
                patient.preferredModeOfCommunication = (contact.FormattedValues["preferredcontactmethodcode"]).ToString();
            }

            if (contact.Attributes.Contains("telephone2"))
            {
                patient.homePhone = contact.Telephone2;
            }

            if (contact.Attributes.Contains("telephone1"))
            {
                patient.workPhone = contact.Telephone1;
            }

            if (contact.Attributes.Contains("mobilephone"))
            {
                patient.mobile = contact.MobilePhone;
            }

            if (contact.Attributes.Contains("firstname"))
            {
                patient.firstName = contact.FirstName;
            }
            if (contact.Attributes.Contains("lastname"))
            {
                patient.lastName = contact.LastName;
            }
            patient.primaryAddress = new Address();

            if (contact.Attributes.Contains("address1_name"))
            {
                patient.primaryAddress.addressType = contact.Address1_Name;
            }

            if (contact.Attributes.Contains("address1_composite"))
            {
                patient.primaryAddress.address = contact.Address1_Composite;
            }

            if (contact.Attributes.Contains("address1_city"))
            {
                patient.primaryAddress.city = contact.Address1_City;
            }

            if (contact.Attributes.Contains("address1_stateorprovince"))
            {
                patient.primaryAddress.state = contact.Address1_StateOrProvince;
            }

            if (contact.Attributes.Contains("address1_line1"))
            {
                patient.primaryAddress.street = contact.Address1_Line1;
            }

            if (contact.Attributes.Contains("address1_line2"))
            {
                patient.primaryAddress.street += " " + contact.Address1_Line2;
            }

            if (contact.Attributes.Contains("address1_line3"))
            {
                patient.primaryAddress.street += " " + contact.Address1_Line3;
            }

            if (contact.Attributes.Contains("address1_country"))
            {
                patient.primaryAddress.country = contact.Address1_Country;
            }

            if (contact.Attributes.Contains("address1_postalcode"))
            {
                patient.primaryAddress.zipCode = contact.Address1_PostalCode;
            }

            return patient;
        }

        public static Patient getPatientModelFilled(Entity entity, Patient patient, string contactAlias, string accountAlias)
        { 
            if (entity.Attributes.Contains(accountAlias + ".mzk_arabicname"))
            {
                patient.arabicName = ((AliasedValue)(entity.Attributes[accountAlias + ".mzk_arabicname"])).Value.ToString();
            }

            if (entity.Attributes.Contains(accountAlias + ".accountnumber"))
            {
                patient.mrn = ((AliasedValue)(entity.Attributes[accountAlias + ".accountnumber"])).Value.ToString();
            }            

            if (entity.Attributes.Contains(accountAlias + ".createdon"))
            {
                patient.registrationDate = Convert.ToDateTime(((AliasedValue)(entity.Attributes[accountAlias + ".createdon"])).Value.ToString());
            }

            if (entity.Attributes.Contains(contactAlias + ".contactid"))
            {
                patient.patientId = ((AliasedValue)(entity.Attributes[contactAlias + ".contactid"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".mzk_vippatient"))
            {
                patient.vip = ((AliasedValue)(entity.Attributes[contactAlias + ".mzk_vippatient"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".fullname"))
            {
                patient.name = ((AliasedValue)(entity.Attributes[contactAlias + ".fullname"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".entityimage"))
            {
                byte[] imageInBytes = (entity.Attributes[contactAlias + ".entityimage"] as AliasedValue).Value as byte[];
                patient.image = Convert.ToBase64String(imageInBytes);
            }

            if (entity.Attributes.Contains(contactAlias + ".gendercode"))
            {
                patient.genderValue = ((OptionSetValue)((AliasedValue)(entity.Attributes[contactAlias + ".gendercode"])).Value).Value;
                patient.gender = (entity.FormattedValues[contactAlias + ".gendercode"]).ToString();
            }
            else
            {
                patient.gender = "";
            }

            if (entity.Attributes.Contains(contactAlias + ".birthdate"))
            {
                patient.dateOfBirth = Convert.ToDateTime(((AliasedValue)(entity.Attributes[contactAlias + ".birthdate"])).Value.ToString());
                //AgeHelper ageHelper = new AgeHelper(DateTime.Now);
                //patient.age = ageHelper.getAgeInYears(patient.dateOfBirth).ToString() + " years";
            }
            if (entity.Attributes.Contains(contactAlias + ".mzk_agecalculated"))
            {
                patient.age = ((AliasedValue)(entity.Attributes[contactAlias + ".mzk_agecalculated"])).Value.ToString();
            }
            else
            {
                patient.age = "";
            }

            if (entity.Attributes.Contains(contactAlias + ".fax"))
            {
                patient.fax = ((AliasedValue)(entity.Attributes[contactAlias + ".fax"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".emailaddress1"))
            {
                patient.email = ((AliasedValue)(entity.Attributes[contactAlias + ".emailaddress1"])).Value.ToString();
            }            

            if (entity.Attributes.Contains(contactAlias + ".mzk_patientlanguage"))
            {
                patient.language = ((EntityReference)((AliasedValue)(entity.Attributes[contactAlias + ".mzk_patientlanguage"])).Value).Name.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".familystatuscode"))
            {
                patient.maritalStatus = (entity.FormattedValues[contactAlias + ".familystatuscode"]).ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".mzk_nationality"))
            {
                patient.nationality = ((EntityReference)(((AliasedValue)(entity.Attributes[contactAlias + ".mzk_nationality"])).Value)).Name.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".mzk_salutation"))
            {
                patient.salutation = (entity.FormattedValues[contactAlias + ".mzk_salutation"]).ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".mzk_nationalidexpirydate"))
            {
                patient.nationalIdExpirationDate = Convert.ToDateTime(((AliasedValue)(entity.Attributes[contactAlias + ".mzk_nationalidexpirydate"])).Value.ToString()).ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".mzk_nationalidtype"))
            {
                patient.nationalIdTypeValue = ((OptionSetValue)((AliasedValue)(entity.Attributes[contactAlias + ".mzk_nationalidtype"])).Value).Value;
            }
            
            if (entity.Attributes.Contains(contactAlias + ".preferredcontactmethodcode"))
            {
                patient.preferredModeOfCommunication = (entity.FormattedValues[contactAlias + ".preferredcontactmethodcode"]).ToString();
            }

            //if (entity.Attributes.Contains(contactAlias + ".telephone1"))
            //{
            //    patient.phone = ((AliasedValue)(entity.Attributes[contactAlias + ".telephone1"])).Value.ToString();
            //}

            if (entity.Attributes.Contains(contactAlias + ".telephone2"))
            {
                patient.phone = ((AliasedValue)(entity.Attributes[contactAlias + ".telephone2"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".firstname"))
            {
                patient.firstName = ((AliasedValue)(entity.Attributes[contactAlias + ".firstname"])).Value.ToString();
            }
            if (entity.Attributes.Contains(contactAlias + ".lastname"))
            {
                patient.lastName = ((AliasedValue)(entity.Attributes[contactAlias + ".lastname"])).Value.ToString();
            }

            patient.primaryAddress = new Address();

            if (entity.Attributes.Contains(contactAlias + ".address1_name"))
            {
                patient.primaryAddress.addressType = ((AliasedValue)(entity.Attributes[contactAlias + ".address1_name"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".address1_city"))
            {
                patient.primaryAddress.city = ((AliasedValue)(entity.Attributes[contactAlias + ".address1_city"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".address1_stateorprovince"))
            {
                patient.primaryAddress.state = ((AliasedValue)(entity.Attributes[contactAlias + ".address1_stateorprovince"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".address1_line1"))
            {
                patient.primaryAddress.street = ((AliasedValue)(entity.Attributes[contactAlias + ".address1_line1"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".address1_line2"))
            {
                patient.primaryAddress.street += " " + ((AliasedValue)(entity.Attributes[contactAlias + ".address1_line2"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".address1_line3"))
            {
                patient.primaryAddress.street += " " + ((AliasedValue)(entity.Attributes[contactAlias + ".address1_line3"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".address1_country"))
            {
                patient.primaryAddress.country = ((AliasedValue)(entity.Attributes[contactAlias + ".address1_country"])).Value.ToString();
            }

            if (entity.Attributes.Contains(contactAlias + ".address1_postalcode"))
            {
                patient.primaryAddress.zipCode = ((AliasedValue)(entity.Attributes[contactAlias + ".address1_postalcode"])).Value.ToString();
            }

            return patient;
        }
        public async Task<Patient> getPatientDetails(string patientGuid, bool getDocuments = false, bool getAddresses = false, bool getRelationship = false)
        {
            try
            {
                Patient patient = new Patient();
                List<Document> patientDocsList = new List<Document>();
                List<Address> patientAddressList = new List<Address>();
                List<Relationship> patientRelationshipList = new List<Relationship>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(Contact.EntityLogicalName);

                if (string.IsNullOrEmpty(patientGuid))
                {
                    throw new ValidationException("Patient Id must be specified");
                }

                query.Criteria.AddCondition("contactid", ConditionOperator.Equal, new Guid(patientGuid));

                query.ColumnSet = new ColumnSet("contactid"
                    , "fullname"
                    , "telephone1"
                    , "emailaddress1"
                    , "msemr_activitystartdate"
                    , "birthdate");


                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                Contact contact = new Contact();


                if (entitycollection != null)
                {
                    for (int i = 0; i < entitycollection.Entities.Count; i++)
                    {
                        contact = (Contact)entitycollection.Entities[i];                  
                    }

                }
                
                patient = getPatientModelFilled(contact, patient, "");

                return patient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<Guid> CreatePatientCRM(Patient patient)
        {
            try
            {

                Entity contact = new Entity("contact");

                if (patient.Externalemrid != "")
                {
                    contact["mzk_externalemrid"] = patient.Externalemrid;
                }
                if (patient.firstName != "")
                {
                    contact["firstname"] = patient.firstName;
                }
                if (patient.lastName != "")
                {
                    contact["lastname"] = patient.lastName;
                }
                if (patient.phone != "")
                {
                    contact["telephone1"] = patient.phone;
                }
                if (patient.email != "")
                {
                    contact["emailaddress1"] = patient.email;
                }
                if (patient.dateOfBirth != null)
                {
                    contact["birthdate"] = Convert.ToDateTime(patient.dateOfBirth);
                }

                contact["msemr_contacttype"] = new OptionSetValue(935000000); // Contact Type: Employee

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(Contact.EntityLogicalName);

                QueryExpression queryExpression = new QueryExpression(Contact.EntityLogicalName);
                queryExpression.Criteria.AddCondition("mzk_externalemrid", ConditionOperator.Equal, patient.Externalemrid);
                queryExpression.ColumnSet = new ColumnSet(true);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryExpression);

                if (entitycollection != null)
                {
                    if (entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities[0].Attributes.Contains("contactid"))
                        {
                            contact["contactid"] = new Guid(entitycollection.Entities[0].Attributes["contactid"].ToString());
                        }
                        entityRepository.UpdateEntity(contact);
                        contact.Id = new Guid(contact["contactid"].ToString());
                    }
                    else
                    {
                        contact.Id = entityRepository.CreateEntity(contact);
                    }
                }
                else
                {
                    contact.Id = entityRepository.CreateEntity(contact);
                }

                return contact.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        

        //public async Task<Patient> getPatientDetails(string patientGuid, bool getDocuments = false, bool getAddresses = false, bool getRelationship = false)
        //{
        //    try
        //    {
        //        Patient patient = new Patient();
        //        List<Document> patientDocsList = new List<Document>();
        //        List<Address> patientAddressList = new List<Address>();
        //        List<Relationship> patientRelationshipList = new List<Relationship>();

        //        // Get Patient from CRM
        //        SoapEntityRepository entityRepository = SoapEntityRepository.GetService();                    

        //        QueryExpression query = new QueryExpression(Contact.EntityLogicalName);

        //        if (string.IsNullOrEmpty(patientGuid))
        //        {
        //            throw new ValidationException("Patient Id must be specified");
        //        }

        //        query.Criteria.AddCondition("contactid", ConditionOperator.Equal, new Guid(patientGuid));

        //        query.ColumnSet = new ColumnSet("contactid", "entityimage", "gendercode", "birthdate",
        //                                            "fullname",
        //                                            "telephone1",
        //                                            "mobilephone",
        //                                            "fax",
        //                                            "emailaddress1",
        //                                            "address1_name",
        //                                            "address1_city",
        //                                            "address1_stateorprovince",
        //                                            "address1_line1",
        //                                            "address1_line2",
        //                                            "address1_line3",
        //                                            "address1_country",
        //                                            "address1_postalcode",
        //                                            "mzk_patientlanguage",
        //                                            "familystatuscode",
        //                                            "mzk_nationality",
        //                                            "mzk_salutation",
        //                                            "mzk_nationalidexpirydate",
        //                                            "mzk_nationalidtype",
        //                                            "preferredcontactmethodcode",
        //                                            "mzk_vippatient",
        //                                            "firstname",
        //                                            "lastname",
        //                                            "mzk_agecalculated",
        //                                            "telephone2"); 

        //        LinkEntity account = new LinkEntity(Contact.EntityLogicalName, Account.EntityLogicalName, "contactid", "primarycontactid", JoinOperator.LeftOuter);
        //        account.Columns = new ColumnSet(true);
        //        query.LinkEntities.Add(account);

        //        EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

        //        if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
        //        {
        //            Contact contact = (Contact)entitycollection.Entities[0];

        //            patient = getPatientModelFilled(contact, patient, account.EntityAlias);

        //            List<QueryExpression> queryExpressionList = new List<QueryExpression>();

        //            if(getDocuments)
        //            {
        //                QueryExpression queryChild = new QueryExpression(Annotation.EntityLogicalName);

        //                queryChild.Criteria.AddCondition("objectid", ConditionOperator.Equal, contact.ContactId);
        //                queryChild.Criteria.AddCondition("isdocument", ConditionOperator.Equal, true);
        //                queryChild.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, Contact.EntityLogicalName);

        //                queryChild.ColumnSet = new ColumnSet("annotationid", "mimetype", "filename");

        //                queryExpressionList.Add(queryChild);
        //            }

        //            if (getAddresses)
        //            {
        //                QueryExpression queryChild = new QueryExpression(xrm.CustomerAddress.EntityLogicalName);

        //                queryChild.Criteria.AddCondition("parentid", ConditionOperator.Equal, contact.ContactId);

        //                queryChild.ColumnSet = new ColumnSet("addresstypecode", "composite", "name");

        //                queryChild.Criteria.AddCondition("composite", ConditionOperator.NotNull);

        //                queryExpressionList.Add(queryChild);
        //            }

        //            if (getRelationship)
        //            {
        //                QueryExpression queryChild = new QueryExpression(xrm.mzk_relationship.EntityLogicalName);

        //                queryChild.Criteria.AddCondition("mzk_customer", ConditionOperator.Equal, contact.ContactId);

        //                queryChild.ColumnSet = new ColumnSet("mzk_relationshipid", "mzk_carer", "mzk_nextofkin");

        //                LinkEntity linkEntityMasterData = new LinkEntity(xrm.mzk_relationship.EntityLogicalName, xrm.mzk_masterdata.EntityLogicalName, "mzk_role", "mzk_masterdataid", JoinOperator.Inner);
        //                linkEntityMasterData.EntityAlias = "role";
        //                linkEntityMasterData.Columns = new ColumnSet("mzk_description","mzk_code");

        //                queryChild.LinkEntities.Add(linkEntityMasterData);

        //                LinkEntity linkEntityConnectedTo = new LinkEntity(xrm.mzk_relationship.EntityLogicalName, xrm.Contact.EntityLogicalName, "mzk_connectedto", "contactid", JoinOperator.Inner);
        //                linkEntityConnectedTo.EntityAlias = "ConnectedTo";
        //                linkEntityConnectedTo.Columns = new ColumnSet("fullname", "telephone1","mobilephone");

        //                queryChild.LinkEntities.Add(linkEntityConnectedTo);

        //                queryExpressionList.Add(queryChild);
        //            }

        //            ExecuteMultipleResponse reponseMultiple = entityRepository.ExecuteRetrieveMultiple(queryExpressionList);

        //            if (reponseMultiple != null && reponseMultiple.Responses != null)
        //            {
        //                foreach (ExecuteMultipleResponseItem item in reponseMultiple.Responses)
        //                {
        //                    RetrieveMultipleResponse retrieveResponse = (RetrieveMultipleResponse)item.Response;

        //                    if (retrieveResponse != null && retrieveResponse.EntityCollection != null)
        //                    {            
        //                        switch(retrieveResponse.EntityCollection.EntityName)
        //                        {
        //                            case Annotation.EntityLogicalName:
        //                                Document doc;

        //                                foreach (Entity entity in retrieveResponse.EntityCollection.Entities)
        //                                {
        //                                    doc = new Document();
        //                                    doc.recId = entity.Attributes.Contains("annotationid") ? (entity.Attributes["annotationid"]).ToString() : string.Empty;
        //                                    doc.name = entity.Attributes.Contains("filename") ? (entity.Attributes["filename"]).ToString() : string.Empty;
        //                                    doc.type = entity.Attributes.Contains("mimetype") ? (entity.Attributes["mimetype"]).ToString() : string.Empty;
        //                                    patientDocsList.Add(doc);
        //                                }

        //                                patient.patientDocuments = patientDocsList;
        //                                break;
        //                            case xrm.CustomerAddress.EntityLogicalName:
        //                                Address address;

        //                                foreach (Entity entity in retrieveResponse.EntityCollection.Entities)
        //                                {
        //                                    address = new Address();
        //                                    address.addressType = entity.Attributes.Contains("addresstypecode") ? entity.FormattedValues["addresstypecode"].ToString() : string.Empty;
        //                                    address.name = entity.Attributes.Contains("name") ? (entity.Attributes["name"]).ToString() : string.Empty;
        //                                    address.address = entity.Attributes.Contains("composite") ? (entity.Attributes["composite"]).ToString() : string.Empty;
        //                                    patientAddressList.Add(address);
        //                                }

        //                                patient.addresses = patientAddressList;
        //                                break;
        //                            case mzk_relationship.EntityLogicalName:
        //                                Relationship relationship;

        //                                foreach (Entity entity in retrieveResponse.EntityCollection.Entities)
        //                                {
        //                                    relationship = new Relationship();
        //                                    relationship.relationshipId = entity.Attributes.Contains("mzk_relationshipid") ? entity.Attributes["mzk_relationshipid"].ToString() : string.Empty;
        //                                    relationship.mobile = entity.Attributes.Contains("ConnectedTo.mobilephone") ? (entity.Attributes["ConnectedTo.mobilephone"] as AliasedValue).Value.ToString() : string.Empty;
        //                                    //relationship.mobile = entity.Attributes.Contains("ConnectedTo.telephone1") ? (entity.Attributes["ConnectedTo.telephone1"] as AliasedValue).Value.ToString() : string.Empty;
        //                                    relationship.name = entity.Attributes.Contains("ConnectedTo.fullname") ? (entity.Attributes["ConnectedTo.fullname"] as AliasedValue).Value.ToString() : string.Empty;
        //                                    //relationship.type = entity.Attributes.Contains("role.mzk_description") ? (entity.Attributes["role.mzk_description"] as AliasedValue).Value.ToString() : string.Empty;
        //                                    relationship.type = entity.Attributes.Contains("role.mzk_code") ? (entity.Attributes["role.mzk_code"] as AliasedValue).Value.ToString() : string.Empty;
        //                                    if (entity.Attributes.Contains("mzk_carer"))
        //                                    {
        //                                        relationship.carer = (bool)entity["mzk_carer"];
        //                                    }
        //                                    if (entity.Attributes.Contains("mzk_nextofkin"))
        //                                    {
        //                                        relationship.nextOfKin = (bool)entity["mzk_nextofkin"];
        //                                    }
        //                                    patientRelationshipList.Add(relationship);
        //                                }

        //                                patient.patientRelationShips = patientRelationshipList;
        //                                break;
        //                        }
        //                    }
        //                }
        //            }   
        //        }

        //        return patient;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<List<Patient>> getPatients(string patientGuId)
        {
            try
            {
                List<Patient> patient = new List<Patient>();
                List<Document> patientDocsList = new List<Document>();

                // Get Patient from CRM
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(Contact.EntityLogicalName);

                if (!string.IsNullOrEmpty(patientGuId))
                {
                    query.Criteria.AddCondition("contactid", ConditionOperator.Equal, new Guid(patientGuId));
                }

                query.ColumnSet = new ColumnSet("contactid", "entityimage", "gendercode", "birthdate",
                                                    "fullname",
                                                    "telephone1",
                                                    "fax",
                                                    "emailaddress1",
                                                    "address1_name",
                                                    "address1_city",
                                                    "address1_stateorprovince",
                                                    "address1_line1",
                                                    "address1_line2",
                                                    "address1_line3",
                                                    "address1_country",
                                                    "address1_postalcode",
                                                    "mzk_patientlanguage",
                                                    "familystatuscode",
                                                    "mzk_nationality",
                                                    "mzk_salutation",
                                                    "mzk_nationalidexpirydate",
                                                    "mzk_nationalidtype",
                                                    "preferredcontactmethodcode",
                                                    "mzk_vippatient",
                                                    "firstname",
                                                    "lastname",
                                                    "mzk_agecalculated",
                                                    "telephone2"); 

                LinkEntity account = new LinkEntity(Contact.EntityLogicalName, Account.EntityLogicalName, "contactid", "primarycontactid", JoinOperator.LeftOuter);
                account.Columns = new ColumnSet(true);
                account.EntityAlias = "account";
                query.LinkEntities.Add(account);

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                {
                    foreach(Entity entity in entitycollection.Entities)
                    {
                        Contact contact = (Contact)entity;

                        Patient patientToAdd = new Patient();

                        patientToAdd = getPatientModelFilled(contact, patientToAdd, account.EntityAlias);

                        QueryExpression queryNotes = new QueryExpression(Annotation.EntityLogicalName);

                        queryNotes.Criteria.AddCondition("objectid", ConditionOperator.Equal, contact.ContactId);
                        queryNotes.Criteria.AddCondition("isdocument", ConditionOperator.Equal, true);
                        queryNotes.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, Contact.EntityLogicalName);

                        queryNotes.ColumnSet = new ColumnSet("annotationid", "mimetype", "filename");

                        EntityCollection entitycollectionNotes = entityRepository.GetEntityCollection(queryNotes);

                        Document doc;

                        foreach (Entity entity1 in entitycollectionNotes.Entities)
                        {
                            if (entity1 != null)
                            {
                                doc = new Document();
                                doc.recId = entity1.Attributes.Contains("annotationid") ? (entity1.Attributes["annotationid"]).ToString() : string.Empty;
                                doc.name = entity1.Attributes.Contains("filename") ? (entity1.Attributes["filename"]).ToString() : string.Empty;
                                doc.type = entity1.Attributes.Contains("mimetype") ? (entity1.Attributes["mimetype"]).ToString() : string.Empty;
                                patientDocsList.Add(doc);
                            }
                        }
                        patientToAdd.patientDocuments = patientDocsList;
                        patient.Add(patientToAdd);
                    }                    
                }
                return patient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public enum HMSearchPatientBy
        {
            MRN = 3,
            Name = 1
        }

        public async Task<List<Patient>> searchPatientDetails(string searchValue, string searchBy)
        {
            try
            {
                List<Patient> listPatient = new List<Patient>();

                Patient patient = new Patient();

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(Contact.EntityLogicalName);
                query.ColumnSet = new ColumnSet(true);

                query.Criteria.AddCondition("mzk_contacttype", ConditionOperator.Equal, (int) mzk_contacttype.Patient);

                if ((HMSearchPatientBy)Convert.ToInt32(searchBy) == HMSearchPatientBy.Name)
                {
                    query.Criteria.AddCondition("fullname", ConditionOperator.Like, ("%" + searchValue + "%"));
                }

                LinkEntity account;
                if ((HMSearchPatientBy)Convert.ToInt32(searchBy) == HMSearchPatientBy.MRN)
                {
                    account = new LinkEntity(Contact.EntityLogicalName, Account.EntityLogicalName, "contactid", "primarycontactid", JoinOperator.Inner);
                    account.LinkCriteria.AddCondition("accountnumber", ConditionOperator.Like, ("%" + searchValue + "%"));
                }
                else
                {
                    account = new LinkEntity(Contact.EntityLogicalName, Account.EntityLogicalName, "contactid", "primarycontactid", JoinOperator.LeftOuter);
                }
                account.Columns = new ColumnSet(true);
                account.EntityAlias = "account";
                query.LinkEntities.Add(account);

                LinkEntity TS = new LinkEntity(Contact.EntityLogicalName, "mzk_hl7ts", "mzk_datetimeofbirth", "mzk_hl7tsid", JoinOperator.LeftOuter);
                TS.Columns = new ColumnSet(true);
                TS.EntityAlias = "TS";
                query.LinkEntities.Add(TS);

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                {
                    foreach (Entity entity in entitycollection.Entities)
                    {
                        Contact contact = (Contact)entity;

                        patient = new Patient();

                        if (contact.Attributes.Contains("account.createdon"))
                        {
                            patient.registrationDate = Convert.ToDateTime((contact.Attributes["account.createdon"] as AliasedValue).Value);
                        }
                        if (contact.Attributes.Contains("account.accountnumber"))
                        {
                            patient.mrn = (contact.Attributes["account.accountnumber"] as AliasedValue).Value.ToString();
                        }

                        if (contact.Attributes.Contains("contactid"))
                        {
                            patient.patientId = contact.ContactId.ToString();
                        }

                        if (contact.Attributes.Contains("fullname"))
                        {
                            patient.name = contact.FullName;
                        }

                        if (contact.Attributes.Contains("birthdate"))
                        {
                            patient.dateOfBirth = Convert.ToDateTime(contact.BirthDate);
                            //AgeHelper ageHelper = new AgeHelper(DateTime.Now);
                            //patient.age = ageHelper.getAgeInYears(patient.dateOfBirth).ToString() + " years";
                        }
                        //if (contact.Attributes.Contains("mzk_datetimeofbirth"))
                        //{
                        //    patient.dateOfBirth = (DateTime)(contact.Attributes["TS.mzk_datetime"] as AliasedValue).Value;
                        //    //AgeHelper ageHelper = new AgeHelper(DateTime.Now);
                        //    //patient.age = ageHelper.getAgeInYears(patient.dateOfBirth).ToString();
                        //    patient.dob = (DateTime)(contact.Attributes["TS.mzk_datetime"] as AliasedValue).Value;
                        //}
                        if (contact.Attributes.Contains("gendercode"))
                        {
                            patient.genderValue = contact.GenderCode.Value;
                            patient.gender = contact.FormattedValues["gendercode"].ToString();
                        }
                        if (contact.Attributes.Contains("telephone1"))
                        {
                            patient.phone = contact.Telephone1;
                        }
                        if (contact.Attributes.Contains("emailaddress1"))
                        {
                            patient.email = contact.EMailAddress1;
                        }
                        if (contact.Attributes.Contains("familystatuscode"))
                        {
                            patient.maritalStatus = contact.FormattedValues["familystatuscode"].ToString();
                        }
                       
                        if (contact.Attributes.Contains("mzk_agecalculated"))
                        {
                            patient.age = contact["mzk_agecalculated"].ToString();
                        }
                        if (contact.Attributes.Contains("firstname"))
                        {
                            patient.firstName = contact.FirstName.ToString();
                        }
                        if (contact.Attributes.Contains("lastname"))
                        {
                            patient.lastName = contact.LastName.ToString();
                        }

                        listPatient.Add(patient);
                    }
                }
                return listPatient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }          
        
        public async Task<Activity> getPatientBasicInfo(string patientGuId, string appGuId = null, string caseId = null)
        {
            try
            {
                Activity model = new Activity();
                AppointmentRepository ApptRepo = new AppointmentRepository();
                PatientRepository patientRepo = new PatientRepository();

                model.patient = new Patient();
                
                List<Appointment> appointments = Appointment.getUserAppointments(null, null, DateTime.MinValue, DateTime.MaxValue, false, null, appGuId, patientGuId);

                Appointment appointment = appointments.FirstOrDefault();

                model.patient = appointment.patient;

                model.appointment = appointment;

                model.caseId = model.appointment.caseId;

                if (!string.IsNullOrEmpty(caseId) && string.IsNullOrEmpty(appGuId))
                {
                    List<string> caseIdList = new List<string>();
                    caseIdList.Add(caseId);
                    List<User> careTeamMembersList = new List<User>();
                    careTeamMembersList = new User().getCareTeamUsersFromList(caseIdList);
                    model.appointment.careTeam = careTeamMembersList;
                }
                
                PatientAllergy patientAllergy = new PatientAllergy();
                List<PatientAllergy> allergiesList = await patientAllergy.getPatientAllergies(patientGuId, "Active", "", DateTime.MinValue, DateTime.MinValue, true);

                foreach (PatientAllergy allergy in allergiesList)
                {
                    if (allergy.allergen != null)
                    {
                        if (model.ActiveAllergies == string.Empty || model.ActiveAllergies == null)
                        {
                            model.ActiveAllergies = allergy.allergen.name;
                        }
                        else
                        {
                            model.ActiveAllergies += ", " + allergy.allergen.name;
                        }
                    }
                }

                PatientProblem patientProblem = new PatientProblem();
                List<PatientProblem> problemsList = await patientProblem.getPatientProblems(patientGuId, true, "Active", "", DateTime.MinValue, DateTime.MinValue, true);

                foreach (PatientProblem problem in problemsList)
                {
                    if (model.ActiveProblems == string.Empty || model.ActiveProblems == null)
                    {
                        model.ActiveProblems = problem.problemName;
                    }
                    else
                    {
                        model.ActiveProblems += ", " + problem.problemName;
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
                
        public string getPatientIdFromRefRecId(decimal patientRecId)
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(Account.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("accountid");

            query.Criteria.AddCondition("mzk_axrefrecid", ConditionOperator.Equal, patientRecId);

            EntityCollection entitycol = repo.GetEntityCollection(query);

            if (entitycol != null && entitycol.Entities.Count > 0)
            {
                patientId = entitycol.Entities[0]["accountid"].ToString();
            }

            return patientId;
        }

        public Dictionary<decimal, string> getPatientIdListFromRefRecIdList(List<decimal> patientRecIdList)
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(Account.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("accountid", "mzk_axrefrecid");

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);

            foreach (decimal item in patientRecIdList)
            {
                childFilter.AddCondition("mzk_axrefrecid", ConditionOperator.Equal, item);
            }

            EntityCollection entitycol = repo.GetEntityCollection(query);

            Dictionary<decimal, string> patientList = new Dictionary<decimal, string>();

            foreach (Entity entity in entitycol.Entities)
            {
                Account acct = (Account)entity;
                patientList.Add(acct.mzk_AXRefRecId.Value, acct.AccountId.Value.ToString());
            }

            return patientList;
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
                patientId = entitycol.Entities[0]["mzk_axrefrecid"].ToString();
            }

            return patientId;
        }

        public Dictionary<string, long> getPatientRecIdListFromGuidList(List<string> patientGuIdList)
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(Account.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_axrefrecid", "accountid");

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);

            foreach (string item in patientGuIdList)
            {
                childFilter.AddCondition("accountid", ConditionOperator.Equal, new Guid(item));
            }

            EntityCollection entitycol = repo.GetEntityCollection(query);

            Dictionary<string, long> patientList = new Dictionary<string, long>();

            foreach (Entity entity in entitycol.Entities)
            {
                Account acct = (Account)entity;
                if (acct.mzk_AXRefRecId != null && acct.mzk_AXRefRecId.Value > 0)
                {
                    patientList.Add(acct.AccountId.Value.ToString(), Convert.ToInt64(acct.mzk_AXRefRecId.Value));
                }
            }

            return patientList;
        }
               
        public async Task<Activity> getPatientVisitDetails(long appointmentRecId, string caseId, long patientRecId)
        {
            try
            {
                Patient patient = new Patient();
                List<Relationship> patientRelationList = new List<Relationship>();

                Activity model = new Activity();

                if (!string.IsNullOrEmpty(caseId))
                {
                    SoapEntityRepository entityRepo = SoapEntityRepository.GetService();

                    Incident entity = (Incident)entityRepo.GetEntity(Incident.EntityLogicalName, new Guid(caseId), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                    if (entity != null)
                    {
                        if (entity.Attributes.Contains("mzk_organizationalunit"))
                            model.clinicName = entity.GetAttributeValue<EntityReference>("mzk_organizationalunit").Name;

                        model.type = (int)HMCaseType.Emergency;
                    }
                }

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(Contact.EntityLogicalName);

                query.ColumnSet = new ColumnSet(true);

                LinkEntity account = new LinkEntity(Contact.EntityLogicalName, Account.EntityLogicalName, "contactid", "primarycontactid", JoinOperator.LeftOuter);
                account.Columns = new ColumnSet(true);
                account.EntityAlias = "account";
                query.LinkEntities.Add(account);

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                {
                    Contact contact = (Contact)entitycollection.Entities[0];

                    if (contact.Attributes.Contains("account.createdon"))
                    {
                        patient.registrationDate = Convert.ToDateTime(((AliasedValue)(contact.Attributes["account.createdon"])).Value);
                    }
                    if (contact.Attributes.Contains("account.accountnumber"))
                    {
                        patient.mrn = ((AliasedValue)(contact.Attributes["account.accountnumber"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("account.primarycontactid"))
                    {
                        patient.patientId = ((AliasedValue)(contact.Attributes["account.primarycontactid"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("fullname"))
                    {
                        patient.name = ((AliasedValue)(contact.Attributes["fullname"])).Value.ToString(); ;
                    }

                    if (contact.Attributes.Contains("birthdate"))
                    {
                        patient.dateOfBirth = Convert.ToDateTime(contact["birthdate"] as AliasedValue);
                        AgeHelper ageHelper = new AgeHelper(DateTime.Now);
                        patient.age = ageHelper.getAgeInYears(model.patient.dateOfBirth).ToString() + " years";
                    }
                    if (contact.Attributes.Contains("gendercode"))
                    {
                        patient.genderValue = ((OptionSetValue)((AliasedValue)(contact.Attributes["gendercode"])).Value).Value;
                    }

                    if (contact.Attributes.Contains("telephone1"))
                    {
                        patient.homePhone = ((AliasedValue)(contact.Attributes["telephone1"])).Value.ToString();
                        patient.workPhone = ((AliasedValue)(contact.Attributes["telephone1"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("emailaddress1"))
                    {
                        patient.email = ((AliasedValue)(contact.Attributes["emailaddress1"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("address1_name"))
                    {
                        patient.primaryAddress.addressType = ((AliasedValue)(contact.Attributes["address1_name"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("address1_city"))
                    {
                        patient.primaryAddress.city = ((AliasedValue)(contact.Attributes["address1_city"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("address1_stateorprovince"))
                    {
                        patient.primaryAddress.state = ((AliasedValue)(contact.Attributes["address1_stateorprovince"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("address1_line1"))
                    {
                        patient.primaryAddress.street = ((AliasedValue)(contact.Attributes[".address1_line1"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains(".address1_line2"))
                    {
                        patient.primaryAddress.street += " " + ((AliasedValue)(contact.Attributes[".address1_line2"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("address1_line3"))
                    {
                        patient.primaryAddress.street += " " + ((AliasedValue)(contact.Attributes["address1_line3"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("address1_country"))
                    {
                        patient.primaryAddress.country = ((AliasedValue)(contact.Attributes["address1_country"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("address1_postalcode"))
                    {
                        patient.primaryAddress.zipCode = ((AliasedValue)(contact.Attributes["address1_postalcode"])).Value.ToString();
                    }

                    if (contact.Attributes.Contains("familystatuscode"))
                    {
                        patient.maritalStatus = contact.FormattedValues["familystatuscode"].ToString();
                    }
                }
                model.patient = patient;

                return model;
            }            
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string getAgeGroup(DateTime patientBirthDate)
        {
            string ret = string.Empty;
            AgeHelper ageHelper = new AgeHelper(DateTime.Now);
            Patient patient = new Patient();
            PatientRepository patientRepo = new PatientRepository();

            // if(patientGender)
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(mzk_agevalidation.EntityLogicalName);

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);
            childFilter.AddCondition("mzk_agegroup", ConditionOperator.Equal, (int)mzk_agevalidationmzk_AgeGroup.Adult);
            childFilter.AddCondition("mzk_agegroup", ConditionOperator.Equal, (int)mzk_agevalidationmzk_AgeGroup.Paediatric);

            //query.Criteria.AddFilter(LogicalOperator.Or);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_agefromunit", "mzk_agefromvalue", "mzk_agetounit", "mzk_agetovalue", "mzk_agegroup");
            //query.Criteria.AddCondition("mzk_agegroup", ConditionOperator.Equal, (int) mzk_agevalidationmzk_AgeGroup.Adult);
            //query.Criteria.AddCondition("mzk_agegroup", ConditionOperator.Equal,(int) mzk_agevalidationmzk_AgeGroup.Paediatric);
            query.Criteria.Filters.Add(childFilter);
            EntityCollection entitycollection = repo.GetEntityCollection(query);
            foreach (Entity entity in entitycollection.Entities)
            {
                mzk_agevalidation agevalidation = (mzk_agevalidation)entity;

                if (ageHelper.isAgeMatched(patientBirthDate, (Helper.Enum.DayWeekMthYr)agevalidation.mzk_Agefromunit.Value, agevalidation.mzk_Agefromvalue.Value, (Helper.Enum.DayWeekMthYr)agevalidation.mzk_Agetounit.Value, agevalidation.mzk_Agetovalue.Value))
                {
                    ret = agevalidation.FormattedValues["mzk_agegroup"].ToString();
                }
            }

            return ret;
        }

        public async Task<bool> updatePatientRIS(string patientId)
        {
            RIS ris = new RIS();

            try
            {
                string messageId = ris.ADT_A08("", "", patientId);

                if (string.IsNullOrEmpty(messageId))
                {
                    throw new ValidationException("Error sending ADT08 message. Contact your system administrator");
                }
                
                if (!Log.createLog(messageId, mzk_messagetype.ADT08, patientId, mzk_messagedirection.Outbound, mzk_acknowledgecode.Empty, ""))
                {
                    throw new ValidationException("Unable to create ADT08 message log");
                }               

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> createPatientRIS(string apptId, string patientId)
        {
            RIS ris = new RIS();

            try
            {
                string messageId = ris.ADT_A04(apptId, "", patientId);

                if (string.IsNullOrEmpty(messageId))
                {
                    throw new ValidationException("Error sending ADT04 message. Contact your system administrator");
                }
                
                if (!Log.createLog(messageId, mzk_messagetype.ADT04, patientId, mzk_messagedirection.Outbound, mzk_acknowledgecode.Empty, ""))
                {
                    throw new ValidationException("Unable to create ADT04 message log");
                }               

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> mergePatient(string sourcePatientId, string targetPatientId)
        {
            RIS ris = new RIS();

            try
            {
                string messageId = ris.ADT_A40(sourcePatientId, targetPatientId);

                if (string.IsNullOrEmpty(messageId))
                {
                    throw new ValidationException("Error sending ADT40 message. Contact your system administrator");
                }

                
                    if (!Log.createLog(messageId, mzk_messagetype.ADT40, targetPatientId, mzk_messagedirection.Outbound, mzk_acknowledgecode.Empty, ""))
                    {
                        throw new ValidationException("Unable to create ADT40 message log");
                    }
                

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> updatePatientDetails(Patient model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.patientId))
                {
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    Entity patient = new Entity(xrm.Contact.EntityLogicalName);

                    patient.Id = new Guid(model.patientId);
                    if (!string.IsNullOrEmpty(model.firstName))
                        patient["firstname"] = model.firstName;
                    if (!string.IsNullOrEmpty(model.lastName))
                        patient["lastname"] = model.lastName;
                    if (model.dateOfBirth != DateTime.MinValue)
                        patient["birthdate"] = model.dateOfBirth;
                    if (!model.genderValue.Equals(0))
                        patient["gendercode"] = new OptionSetValue(model.genderValue);

                    if (!string.IsNullOrEmpty(model.homePhone))
                        patient["telephone2"] = model.homePhone;
                    if (!string.IsNullOrEmpty(model.mobile))
                        patient["mobilephone"] = model.mobile;
                    if (!string.IsNullOrEmpty(model.workPhone))
                        patient["telephone1"] = model.workPhone;
                    if (!string.IsNullOrEmpty(model.fax))
                        patient["fax"] = model.fax;
                    if (!string.IsNullOrEmpty(model.email))
                        patient["emailaddress1"] = model.email;

                    if (!string.IsNullOrEmpty(model.patientDeviceId))
                        patient["mzk_patientdeviceid"] = model.patientDeviceId;

                    patient["mzk_facerecognitionenabled"] = model.faceRecognitionEnabled;
                    patient["mzk_fingerprintenabled"] = model.fingerprintEnabled;
                    patient["mzk_pincodeenabled"] = model.pinCodeEnabled;
                    patient["mzk_agreetotermsandconditions"] = model.agreeToTermsAndCondition;
                    patient["mzk_patientdetailsverified"] = model.patientDetailsVerified;
                    entityRepository.UpdateEntity(patient);

                    return true;
                }
                else
                {
                    throw new ValidationException("Patient Id not found");
                }
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<ReturnStatus> checkPatientAppAccess(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                ReturnStatus returnStatus = new ReturnStatus();

                var handler = new JwtSecurityTokenHandler();
                dynamic jsonToken = handler.ReadToken(token);

                List<System.Security.Claims.Claim> claimList = jsonToken.Claims;

                System.Security.Claims.Claim oid = claimList.Where(item => item.Type == "oid").FirstOrDefault();

                if (oid != null)
                { 
                    SoapEntityRepository repo = SoapEntityRepository.GetService();

                    QueryExpression query = new QueryExpression("contact");
                    query.Criteria.AddCondition("mzk_appobjectid", ConditionOperator.Equal, oid.Value);
                    query.ColumnSet = new ColumnSet("mzk_contacttype", "mzk_facerecognitionenabled", "mzk_fingerprintenabled", "mzk_pincodeenabled","firstname", "mzk_agreetotermsandconditions", "mzk_patientdetailsverified");
                    EntityCollection collection = repo.GetEntityCollection(query);

                    if (collection != null && collection.Entities != null && collection.Entities.Count > 0)
                    {
                        if (collection.Entities[0].Attributes.Contains("mzk_contacttype"))
                        {
                            string contactType = collection.Entities[0].FormattedValues["mzk_contacttype"];
                            if (contactType.Equals("Patient"))
                            {
                                query = new QueryExpression(xrm.Opportunity.EntityLogicalName);
                                query.Criteria.AddCondition("parentcontactid", ConditionOperator.Equal, new Guid(collection.Entities[0].Id.ToString()));

                                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);
                                childFilter.AddCondition("mzk_status", ConditionOperator.Equal, 275380001);//Active
                                childFilter.AddCondition("mzk_status", ConditionOperator.Equal, 275380004);//Reviewed

                                EntityCollection entitycollection = repo.GetEntityCollection(query);

                                if (entitycollection.Entities.Count.Equals(0))
                                {
                                    returnStatus.status = false;
                                    returnStatus.message = "Cannot login since the patient does not have any active referrals";
                                }
                                else
                                {
                                    if (collection.Entities[0].Attributes.Contains("mzk_facerecognitionenabled"))
                                        returnStatus.faceRecognitionEnabled = (bool)collection.Entities[0]["mzk_facerecognitionenabled"];
                                    if (collection.Entities[0].Attributes.Contains("mzk_fingerprintenabled"))
                                        returnStatus.fingerprintEnabled = (bool)collection.Entities[0]["mzk_fingerprintenabled"];
                                    if (collection.Entities[0].Attributes.Contains("mzk_pincodeenabled"))
                                        returnStatus.pinCodeEnabled = (bool)collection.Entities[0]["mzk_pincodeenabled"];
                                    if (collection.Entities[0].Attributes.Contains("mzk_agreetotermsandconditions"))
                                        returnStatus.agreeTotermsAndCondition = (bool)collection.Entities[0]["mzk_agreetotermsandconditions"];
                                    if (collection.Entities[0].Attributes.Contains("mzk_patientdetailsverified"))
                                        returnStatus.patientDetailsVerified = (bool)collection.Entities[0]["mzk_patientdetailsverified"];
                                    if (collection.Entities[0].Attributes.Contains("firstname"))
                                        returnStatus.name = collection.Entities[0]["firstname"].ToString();
                                    returnStatus.message = collection.Entities[0].Id.ToString();
                                    returnStatus.status = true;
                                }
                            }
                            else
                            {
                                if (collection.Entities[0].Attributes.Contains("mzk_facerecognitionenabled"))
                                    returnStatus.faceRecognitionEnabled = (bool)collection.Entities[0]["mzk_facerecognitionenabled"];
                                if (collection.Entities[0].Attributes.Contains("mzk_fingerprintenabled"))
                                    returnStatus.fingerprintEnabled = (bool)collection.Entities[0]["mzk_fingerprintenabled"];
                                if (collection.Entities[0].Attributes.Contains("mzk_pincodeenabled"))
                                    returnStatus.pinCodeEnabled = (bool)collection.Entities[0]["mzk_pincodeenabled"];
                                if (collection.Entities[0].Attributes.Contains("firstname"))
                                    returnStatus.name = collection.Entities[0]["firstname"].ToString();
                                returnStatus.message = collection.Entities[0].Id.ToString();
                                returnStatus.status = true;
                            }
                        }
                        
                    }
                    else
                    {
                        returnStatus.status = false;
                        returnStatus.message = "User is not a registered user";
                    }
                }
                else
                {
                    returnStatus.status = false;
                    returnStatus.message = "User is not a registered user";
                }                

                return returnStatus;
            }
            else
            {
                throw new AuthenticationException("Token not found");
            }
        }
        #endregion
    }
    
}
