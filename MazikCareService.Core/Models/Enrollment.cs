using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
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
    public class Enrollment
    {

        public PhysicianSection physicianSection { get; set; }
        public PatientSection patientSection { get; set; }
        public PatientContactSection patientContactSection { get; set; }
        public BillingSection billingSection { get; set; }
        public MedicalSection medicalSection { get; set; }
        public InsuranceSection insuranceSection { get; set; }
        public DeviceSection deviceSection { get; set; }
        public AdditionalServiceSection additionalServiceSection { get; set; }
        public NotesSection notesSection { get; set; }

        public string enrollmentId { get; set; }

        public string baselineDate { get; set; }

        public List<Insurance> insurance { get; set; }
        public Patient patient { get; set; }




        public async Task<List<EnrollmentSettings>> GetEnrollmentSettings(string practiceId)
        {
            List<EnrollmentSettings> Enrollments = new List<EnrollmentSettings>();
            EnrollmentSettings Enrollment = new EnrollmentSettings();
            Enrollments.Add(Enrollment);
            return Enrollments;
        }

        public async Task<bool> PostAddPhysicianToClinic(string practiceId, Physician physician)
        {
            return true;
        }

        public async Task<bool> PostRequestNewInsurance(string practiceId, Insurance insurance, string userCreatedBy)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            int value = 275380002;
            Entity entity = new Entity(Account.EntityLogicalName);
            entity.Attributes["mzk_accounttype"] = new OptionSetValue(value);
            entity.Attributes["mzk_usercreatedby"] = userCreatedBy; //Addition
            entity.Attributes["mzk_usermodifiedby"] = userCreatedBy; //Addition
            entity.Attributes["name"] = insurance.insuranceName;
            entity.Attributes["telephone2"] = insurance.phone;
            entity.Attributes["fax"] = insurance.fax;
            entity.Attributes["address1_composite"] = insurance.address1;
            entity.Attributes["address2_composite"] = insurance.address2;
            entity.Attributes["address1_city"] = insurance.city;
            entity.Attributes["address1_stateorprovince"] = insurance.state;
            entity.Attributes["address1_postalcode"] = insurance.zip;
            entityRepository.CreateEntity(entity);
            return true;
        }

        public async Task<bool> PostSaveEnrollment(string practiceId, Enrollment enrollment, string userCreatedBy)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            /*Entity contact = new Entity("contact");
            int value = 275380001;
            contact["mzk_contacttype"] = new OptionSetValue(value);
            contact["firstname"] = enrollment.patient.firstName;
            contact["lastname"] = enrollment.patient.lastName;
            contact["fullname"] = enrollment.patient.name;
            Guid contactguid = new Guid();
            contactguid = entityRepository.CreateEntity(contact);*/

            //QueryExpression query = new QueryExpression(Contact.EntityLogicalName);
            //query.Criteria.AddCondition;


            Entity enroll = new Entity(Incident.EntityLogicalName);
            enroll["mzk_casetype"] = new OptionSetValue(1);
            enroll["mzk_usercreatedby"] = userCreatedBy; //Addition
            enroll["mzk_usermodifiedby"] = userCreatedBy; // Addition
            if (!string.IsNullOrEmpty(practiceId))
            {
                EntityReference clinic_lookup = new EntityReference(Account.EntityLogicalName, new Guid(practiceId));
                //clinic_lookup.Id = new Guid(practiceId);
                enroll["mzk_practice"] = clinic_lookup;
            }
            //if (contactguid!=null)
            if (!string.IsNullOrEmpty(enrollment.patient.patientId))
            {
                EntityReference patient_lookup = new EntityReference(Contact.EntityLogicalName, new Guid(enrollment.patient.patientId));
                //patient_lookup.Id = new Guid(enrollment.patient.patientId);//contactguid;
                enroll["customerid"] = patient_lookup;
                //enroll["customerid"] = patient_lookup.Id;
            }

            // DateTime dt = new DateTime();
            //  dt=
            //  enroll["mzk_baseline"] = 
            entityRepository.CreateEntity(enroll);


            return true;
        }


        public async Task<List<Enrollment>> GetEnrollments(string practiceId)
        {
            List<Enrollment> Enrollments = new List<Enrollment>();

            QueryExpression query = new QueryExpression(Incident.EntityLogicalName);
            // query.PageInfo = new PagingInfo();
            //  query.PageInfo.Count = 200; // or 50, or whatever      // This is to limit the number of records to be fetched //
            //  query.PageInfo.PageNumber = 1;
            //query.ColumnSet = new ColumnSet("ticketnumber", "mzk_baseline");
            //query.Criteria.AddCondition("mzk_practice", ConditionOperator.Equal, new Guid(practiceId));
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            if (!string.IsNullOrEmpty(practiceId))
            {
                childFilter.AddCondition("mzk_practice", ConditionOperator.Equal, new Guid(practiceId));
            }
            childFilter.AddCondition("mzk_baseline", ConditionOperator.NotNull);
            childFilter.AddCondition("mzk_insuranceverified", ConditionOperator.Equal, true);
            childFilter.AddCondition("mzk_senttobilling", ConditionOperator.Equal, false);


            query.ColumnSet = new ColumnSet("ticketnumber", "mzk_baseline");

            LinkEntity Link1 = new LinkEntity(Incident.EntityLogicalName, Contact.EntityLogicalName, "customerid", "contactid", JoinOperator.Inner)
            {
                Columns = new ColumnSet("contactid", "mzk_patientname", "firstname", "middlename", "lastname", "address1_composite", "address2_composite", "address1_city", "address1_stateorprovince", "address1_postalcode", "telephone2", "emailaddress1", "birthdate", "gendercode"),
                EntityAlias = "EnrollPatients",
            };

            LinkEntity Link2 = new LinkEntity("incident", "mzk_caseinsurancecarrier", "incidentid", "mzk_caseid", JoinOperator.LeftOuter)
            {
                Columns = new ColumnSet("mzk_insurancetype"),
                EntityAlias = "EnrollInsurance",

            };
            LinkEntity Link3 = new LinkEntity("mzk_caseinsurancecarrier", "mzk_patientinsurancecarrier", "mzk_patientinsurancecarrier", "mzk_patientinsurancecarrierid", JoinOperator.LeftOuter)
            {
                Columns = new ColumnSet("mzk_account", "mzk_patientinsurancepolicynumber"),
                EntityAlias = "CasePatient",
            };
            LinkEntity Link4 = new LinkEntity(Contact.EntityLogicalName, Account.EntityLogicalName, "contactid", "primarycontactid", JoinOperator.Inner)
            {
                Columns = new ColumnSet("accountnumber"),
                EntityAlias = "ContactAccount",
            };
            Link1.LinkEntities.Add(Link4);
            Link2.LinkEntities.Add(Link3);
            query.LinkEntities.Add(Link1);
            query.LinkEntities.Add(Link2);

            OrderExpression orderby = new OrderExpression();
            orderby.AttributeName = "incidentid";
            orderby.OrderType = OrderType.Ascending;

            query.Orders.Add(orderby);

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            var c = entitycollection;


            var groupedEnrollments = entitycollection.Entities.GroupBy(item => (item.GetAttributeValue<Guid>("incidentid")));

            foreach (var groupedEnrollmentsCurrent in groupedEnrollments)
            { // For each grouped enrollment

                Enrollment model = new Enrollment();
                Patient patient = new Patient();
                Insurance insurance;
                List<Insurance> InsuranceList = new List<Insurance>();
                foreach (Entity entity in groupedEnrollmentsCurrent)
                {

                    string accTypeText = "";
                    if (entity.Attributes.Contains("EnrollInsurance.mzk_insurancetype"))
                    {
                        accTypeText = entity.FormattedValues["EnrollInsurance.mzk_insurancetype"].ToString();
                    }

                    if (entity.Attributes.Contains("mzk_baseline"))
                    {
                        if (entity.Attributes.Contains("ticketnumber"))
                            model.enrollmentId = entity["ticketnumber"].ToString();
                        if (entity.Attributes.Contains("mzk_baseline"))
                            model.baselineDate = Convert.ToDateTime(entity["mzk_baseline"]).ToString();
                        if (entity.Attributes.Contains("EnrollPatients.contactid"))
                            patient.patientId = ((AliasedValue)entity["EnrollPatients.contactid"]).Value.ToString();
                        if (entity.Attributes.Contains("EnrollPatients.firstname"))
                            patient.firstName = ((AliasedValue)entity["EnrollPatients.firstname"]).Value.ToString();
                        if (entity.Attributes.Contains("EnrollPatients.middlename"))
                            patient.middleName = ((AliasedValue)entity["EnrollPatients.middlename"]).Value.ToString();
                        if (entity.Attributes.Contains("EnrollPatients.lastname"))
                            patient.lastName = ((AliasedValue)entity["EnrollPatients.lastname"]).Value.ToString();
                        
                        if (entity.Attributes.Contains("EnrollPatients.address1_city"))
                            patient.name = ((AliasedValue)entity["EnrollPatients.address1_city"]).Value.ToString();
                       
                        if (entity.Attributes.Contains("EnrollPatients.telephone2"))
                            patient.phone = ((AliasedValue)entity["EnrollPatients.telephone2"]).Value.ToString();
                        if (entity.Attributes.Contains("EnrollPatients.emailaddress1"))
                            patient.emailAddress = ((AliasedValue)entity["EnrollPatients.emailaddress1"]).Value.ToString();
                        
                        if (entity.Attributes.Contains("EnrollPatients.gendercode"))
                            patient.gender = entity.FormattedValues["EnrollPatients.gendercode"].ToString();
                        if (entity.Attributes.Contains("ContactAccount.accountnumber"))
                            patient.mrn = ((AliasedValue)entity["ContactAccount.accountnumber"]).Value.ToString();
                        //((AliasedValue)entity["EnrollPatients.gendercode"]).Value.ToString();

                        insurance = new Insurance();
                        if (entity.Attributes.Contains("CasePatient.mzk_account"))
                            insurance.insuranceName = entity.FormattedValues["CasePatient.mzk_account"].ToString();//lookupname;//((AliasedValue)entity["CasePatient.mzk_account"]).Value.ToString();
                        if (entity.Attributes.Contains("CasePatient.mzk_patientinsurancepolicynumber"))
                            insurance.insurancePolicyNumber = ((AliasedValue)entity["CasePatient.mzk_patientinsurancepolicynumber"]).Value.ToString();
                        if (entity.Attributes.Contains("EnrollInsurance.mzk_insurancetype"))
                            insurance.insuranceType = accTypeText;//((OptionSetValue)((AliasedValue)entity["EnrollInsurance.mzk_insurancetype"]).Value).Value.ToString();

                        model.patient = patient;
                        if (!string.IsNullOrEmpty(insurance.insuranceName) || !string.IsNullOrEmpty(insurance.insurancePolicyNumber))
                        {
                            InsuranceList.Add(insurance);
                        }


                    }

                }
                if (InsuranceList.Count > 0)
                {
                    model.insurance = InsuranceList;
                }

                Enrollments.Add(model);
            }

            return Enrollments;
        }

        public async Task<bool> UpdateEnrollmentByCentricityId(string enrollmentId, string centricityId, string userModifiedBy)
        {
            /*QueryExpression query = new QueryExpression();
            query.EntityName = "incident";
            query.ColumnSet = new ColumnSet(true);
            if (!string.IsNullOrEmpty(enrollmentId))
            {
                query.Criteria.AddCondition("ticketnumber", ConditionOperator.Equal, new Guid(enrollmentId));
            }*/
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            // EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            /*if (entitycollection.Entities.Count == 1)
            {
                Entity currentEntity = new Entity("incident");
                //currentid.Id = entitycollection.Entities["ticketnumber"].centricityId;
            }*/

            Entity incident = new Entity("incident");
            incident.Id = new Guid(enrollmentId);
            incident.Attributes["mzk_usermodifiedby"] = userModifiedBy;
            incident.Attributes["mzk_centricityid"] = centricityId;
            try
            {
                entityRepository.UpdateEntity(incident);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return true;
        }

        public async Task<bool> CreateEnrollmentOrder(string enrollmentId)//externalOrderId, string externalPatientId, string providerCode, string facilityCode, string serviceType, string message, string messageType)
        {
            if (!string.IsNullOrEmpty(enrollmentId))
            {
                Guid accountId = new Guid();
                Guid practiceId = new Guid();
                Guid priceListId = new Guid();
                Guid workOrderId = new Guid();
                //**Fetching Record from Enrollment Id for PatientId and PracticeId**//
                QueryExpression query = new QueryExpression(Incident.EntityLogicalName);
                query.Criteria.AddCondition("incidentid", ConditionOperator.Equal, enrollmentId);
                query.ColumnSet = new ColumnSet("mzk_practice");
                LinkEntity Link1 = new LinkEntity(Incident.EntityLogicalName, Account.EntityLogicalName, "customerid", "primarycontactid", JoinOperator.Inner)
                {
                    Columns = new ColumnSet("accountid"),
                    EntityAlias="EnrollAccount",
                };
                query.LinkEntities.Add(Link1);
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                foreach (Entity entity in entitycollection.Entities)
                {
                    if (entity.Attributes.Contains("mzk_practice"))
                    {
                        EntityReference practice_lookup = new EntityReference();
                        practice_lookup = (EntityReference)entity["mzk_practice"];
                        practiceId = practice_lookup.Id;
                    }
                    if (entity.Attributes.Contains("EnrollAccount.accountid"))
                    {
                        accountId = new Guid(((AliasedValue)entity["EnrollAccount.accountid"]).Value.ToString());
                    }
                }


                /* Gettign priceListId*/
                try
                {

                    QueryExpression pl_query = new QueryExpression(PriceLevel.EntityLogicalName);
                    pl_query.ColumnSet = new ColumnSet("pricelevelid");
                    EntityCollection pl_entityCollection = entityRepository.GetEntityCollection(pl_query);

                    if (pl_entityCollection != null && pl_entityCollection.Entities != null && pl_entityCollection.Entities.Count > 0)
                    {
                        priceListId = new Guid(pl_entityCollection.Entities[0].Attributes["pricelevelid"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                /*EnrollmentOrderCreation*/
                Entity workOrderEntity = new Entity(msdyn_workorder.EntityLogicalName);
                if (!string.IsNullOrEmpty(enrollmentId))
                {
                    workOrderEntity["mzk_enrollment"] = new EntityReference("mzk_enrollment", new Guid(enrollmentId));
                }
                if (accountId != Guid.Empty)
                {
                    workOrderEntity["msdyn_serviceaccount"] = new EntityReference("account", accountId);
                }
                if (priceListId != Guid.Empty)
                {
                    workOrderEntity["msdyn_pricelist"] = new EntityReference("pricelevel", priceListId);
                }
                if (practiceId != Guid.Empty)
                {
                    workOrderEntity["mzk_practice"] = new EntityReference("account", practiceId);
                }
                try
                {
                    workOrderId = entityRepository.CreateEntity(workOrderEntity);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                /*WorkOrderProductCreation*/
                QueryExpression el_query = new QueryExpression(mzk_patientorder.EntityLogicalName);
                query.Criteria.AddCondition("mzk_caseid", ConditionOperator.Equal,new Guid(enrollmentId));
                query.ColumnSet = new ColumnSet(true);

                EntityCollection entityCollection = entityRepository.GetEntityCollection(el_query);

                foreach (Entity entity in entityCollection.Entities)
                {
                    Entity workorderproductEntity = new Entity("msdyn_workorderproduct");

                    workorderproductEntity["msdyn_workorder"] = new EntityReference("msdyn_workorder", workOrderId);

                    if (entity.Attributes.Contains("mzk_product"))
                    {
                        workorderproductEntity["msdyn_product"] = new EntityReference("product", entity.GetAttributeValue<EntityReference>("mzk_product").Id);
                    }

                    if (entity.Attributes.Contains("mzk_quantity"))
                    {
                        workorderproductEntity["msdyn_quantity"] = (double)entity.GetAttributeValue<int>("mzk_quantity");
                    }

                    workorderproductEntity["msdyn_linestatus"] = new OptionSetValue(275380000);

                    if (priceListId != Guid.Empty)
                    {
                        workOrderEntity["msdyn_pricelist"] = new EntityReference("pricelevel", priceListId);
                    }

                    entityRepository.CreateEntity(workorderproductEntity);
                }

                return true;
                

            }
            return false;
        }

        public async Task<bool> UpdateEnrollmentDeviceDate(string enrollmentId, DateTime dateOfService, string userModifiedBy)
        {

            if (!string.IsNullOrEmpty(enrollmentId))
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(Incident.EntityLogicalName);
                query.Criteria.AddCondition("incidentid", ConditionOperator.Equal, new Guid(enrollmentId));
                query.ColumnSet = new ColumnSet("mzk_baseline", "mzk_baselinereceived");


                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                if (entitycollection.Entities.Count > 0)
                {
                    foreach (Entity entity in entitycollection.Entities)
                    {
                        try
                        {

                            entity["mzk_baseline"] = dateOfService;
                            entity["mzk_baselinereceived"] = true;
                            entity["mzk_usermodifiedby"] = userModifiedBy;

                      
                            entityRepository.UpdateEntity(entity);
                            return true;
                        }
                        catch
                        {
                            throw new customException(string.Format("Update not Successful"));
                        }


                    }
                }
                else
                {
                    throw new customException(string.Format("Enrollment Id not Available"));
                    //Exception e = new Exception();
                    //e.InnerException.Data.Add("Message", "Enrollment Id not Available");
                    //throw e;
                    //return false;
                }
            }
            else
            {
                throw new customException(string.Format("Enrollment Id not Available"));
            }
            return false;

                    //Entity incident = new Entity(Incident.EntityLogicalName);
                    // incident.Id = new Guid(enrollmentId);
                    //incident.Attributes["mzk_usermodifiedby"] = userModifiedBy;
                    //incident.Attributes["mzk_baseline"] = dateOfService;
                    //incident.Attributes["mzk_baselinereceived"] = true;




        }
    }
    public class customException : Exception
    {
        public customException(string message) 
            : base(message)
        { }
    }

        public class PhysicianSection
        {
            public string locationId { get; set; }
            public List<Physician> physicians { get; set; }
        }

        public class PatientSection
        {
            public string primaryPatientID { get; set; }
            public string firstName { get; set; }
            public string middleInitial { get; set; }
            public string lastName { get; set; }
            public string dob { get; set; }
            public string secondaryPatientID { get; set; }
            public string tertiaryPatientID { get; set; }
            public string language { get; set; }
        }

        public class PhoneNumber
        {
            public string phoneNumber { get; set; }
            public string phoneType { get; set; }
            public bool sms { get; set; }
        }

        public class EmailAddress
        {
            public string emailAddress { get; set; }
            public string emailType { get; set; }
            public bool sms { get; set; }
        }

        public class EmergencyContact
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string phone { get; set; }
            public string relationship { get; set; }
        }

        public class ShippingAddress
        {
            public string address1 { get; set; }
            public string address2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }
        }

        public class PatientContactSection
        {
            public List<PhoneNumber> phoneNumbers { get; set; }
            public List<EmailAddress> emailAddresses { get; set; }
            public List<EmergencyContact> emergencyContacts { get; set; }
            public ShippingAddress shippingAddress { get; set; }
        }

        public class GuarantorContact
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string phone { get; set; }
            public string relationship { get; set; }
        }

        public class BillingSection
        {
            public bool billingAddressSameAsShipping { get; set; }
            public GuarantorContact guarantorContact { get; set; }
            public string address1 { get; set; }
            public string address2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }

            public List<Section> sections { get; set; }
        }

        public class DiagnosisCode
        {
            public string code { get; set; }
        }

        public class OtherCondition
        {
            public bool sensitiveSkin { get; set; }
            public bool? pediatricElectrodes { get; set; }
        }

        public class MedicalSection
        {
            public List<DiagnosisCode> diagnosisCodes { get; set; }
            public string additionalClinicalNotes { get; set; }
            public List<OtherCondition> otherConditions { get; set; }
            public string pacemaker { get; set; }
            public string sex { get; set; }
        }

        public class PrimaryInsurance
        {
            public string insurance { get; set; }
            public string relationship { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string dob { get; set; }
            public string memberID { get; set; }
            public string groupID { get; set; }
            public string authorizationNumber { get; set; }
            public string ipaName { get; set; }
            public string ipaAddress { get; set; }
            public string ipaPhone { get; set; }
        }

        public class SecondaryInsurance
        {
            public string insurance { get; set; }
            public string relationship { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string dob { get; set; }
            public string memberID { get; set; }
            public string groupID { get; set; }
            public string authorizationNumber { get; set; }
            public string ipaName { get; set; }
            public string ipaAddress { get; set; }
            public string ipaPhone { get; set; }
        }

        public class InsuranceSection
        {
            public bool noInsurance { get; set; }
            public bool insuranceToFollow { get; set; }
            public PrimaryInsurance primaryInsurance { get; set; }
            public SecondaryInsurance secondaryInsurance { get; set; }
        }

        public class DeviceSection
        {
            public string device { get; set; }
            public string hookUp { get; set; }
            public string deliveryType { get; set; }
            public string serialNumber { get; set; }
            public string deployDuration { get; set; }
            public int numberOfDays { get; set; }
            public string expectedServiceDate { get; set; }
        }

        public class ClinicalJustification
        {
        }

        public class AlternateServicePreference
        {
        }


        public class AdditionalServiceSection
        {
            public ClinicalJustification clinicalJustification { get; set; }
            public AlternateServicePreference alternateServicePreference { get; set; }
            public Notes notes { get; set; }
        }

        public class NotesSection
        {
            public string specialInstructions { get; set; }
        }

        public class Settings
        {
            public string specialInstructions { get; set; }
        }


        // Enrollment class and objects end here
        public class CreateNewModal
        {
            public bool switchFirstLast { get; set; }
        }

        public class Label
        {
            public string primaryLabel { get; set; }
            public string secondaryLabel { get; set; }
            public string tertiaryLabel { get; set; }
        }

        public class Validation
        {
            public string primaryID { get; set; }
            public string secondaryID { get; set; }
            public string tertiaryID { get; set; }
        }

        public class FieldPosition
        {
            public int serviceType { get; set; }
            public int? device { get; set; }
            public int? hookup { get; set; }
            public int? deliveryType { get; set; }
            public int? serialNumber { get; set; }
            public int? deployDuration { get; set; }
        }

        public class Section
        {
            public string section { get; set; }
            public CreateNewModal createNewModal { get; set; }
            public string defaultLanguage { get; set; }
            public bool? switchFirstLast { get; set; }
            public List<string> hiddenFields { get; set; }
            public List<Label> labels { get; set; }
            public List<string> requiredFields { get; set; }
            public List<Validation> validations { get; set; }
            public string defaultSex { get; set; }
            public string defaultDeliveryType { get; set; }
            public string __invalid_name__defaultDevice { get; set; }
            public int? defaultDuration { get; set; }
            public string defaultHookup { get; set; }
            public string __invalid_name__serviceType { get; set; }
            public List<FieldPosition> fieldPositions { get; set; }
        }

        public class EnrollmentSettings
        {
            public List<Section> sections { get; set; }
        }
    }


