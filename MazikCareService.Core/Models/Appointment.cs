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
    public class Appointment
    {
        public Patient patient { get; set; }
        public string appointmentNumber { get; set; }
        public string status { get; set; }
        public string caseId { get; set; }
        public List<User> careTeam { get; set; }
        public int statusValue { get; set; }
        public string startDateTime { get; set; }
        public string endDateTime { get; set; }
        public string appointmentId { get; set; }

        public Appointment getAppointmentDetails(string appointmentGuid = null)
        {
            Appointment model = new Appointment();
            
            model = getUserAppointments(null,null,DateTime.MinValue,DateTime.MaxValue,false,null,appointmentGuid).FirstOrDefault<Appointment>();
               
            return model;  
        }

        public static Appointment getAppointmentModelFilledByAppointment(Entity entity, Appointment model)
        {
            if (entity.Attributes.Contains("mzk_starttime"))
            {
                model.startDateTime = Convert.ToDateTime(entity.Attributes["mzk_starttime"]).ToString();
            }
            if (entity.Attributes.Contains("mzk_endtime"))
            {
                model.endDateTime = Convert.ToDateTime(entity.Attributes["mzk_endtime"]).ToString();
            }

            User careTeamMember = new User();
            List<User> listCareTeam = new List<User>();

            if (entity.Attributes.Contains("mzk_resource"))
            {
                careTeamMember.Name = ((EntityReference)(entity.Attributes["mzk_resource"])).Name;
            }
            listCareTeam.Add(careTeamMember);

            model.careTeam = listCareTeam;

            if (entity.Attributes.Contains("mzk_appointmentnumber"))
            {
                model.appointmentNumber = entity.Attributes["mzk_appointmentnumber"].ToString();
            }

            if (entity.Attributes.Contains("mzk_patientappointmentid"))
            {
                model.appointmentId = entity.Attributes["mzk_patientappointmentid"].ToString();
            }

            return model;
        }

        public static Appointment getAppointmentModelFilledByAppoinment(Entity entity, Appointment model, string aliasPatientAppointment)
        {
            if (!string.IsNullOrEmpty(aliasPatientAppointment))
            {
                aliasPatientAppointment += ".";
            }

            if (entity.Attributes.Contains(aliasPatientAppointment + "mzk_starttime"))
            {
                model.startDateTime = Convert.ToDateTime((entity.Attributes[aliasPatientAppointment + "mzk_starttime"] as AliasedValue).Value).ToString();
            }

            if (entity.Attributes.Contains(aliasPatientAppointment + "mzk_endtime"))
            {
                model.endDateTime = Convert.ToDateTime((entity.Attributes[aliasPatientAppointment + "mzk_endtime"] as AliasedValue).Value).ToString();
            }

            User careTeamMember = new User();
            List<User> listCareTeam = new List<User>();

            if (entity.Attributes.Contains(aliasPatientAppointment + "mzk_resource"))
            {
                careTeamMember.Name = ((EntityReference)((entity.Attributes[aliasPatientAppointment + "mzk_resource"] as AliasedValue).Value)).Name;
            }

            listCareTeam.Add(careTeamMember);

            model.careTeam = listCareTeam;

            if (entity.Attributes.Contains(aliasPatientAppointment + "mzk_appointmentnumber"))
            {
                model.appointmentNumber = ((EntityReference)((entity.Attributes[aliasPatientAppointment + "mzk_appointmentnumber"] as AliasedValue).Value)).ToString();
            }

            if (entity.Attributes.Contains(aliasPatientAppointment + "mzk_patientappointmentid"))
            {
                model.appointmentId = ((AliasedValue)(entity.Attributes[aliasPatientAppointment + "mzk_patientappointmentid"])).Value.ToString();
            }

            return model;
        }

        public static List<Appointment> getUserAppointments(string resourceId, string clinicId, DateTime startDate, DateTime endDate, bool onlyTriaged, string searchOrder, string appointmentId, string patientId = null, bool activitiesIncluded = true)
        {
            List<Appointment> appointments = new List<Appointment>();

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            QueryExpression query = new QueryExpression(mzk_patientappointment.EntityLogicalName);
            
            query.ColumnSet = new ColumnSet(true);

            if (!string.IsNullOrEmpty(appointmentId))
            {
                query.Criteria.AddCondition("mzk_patientappointmentid", ConditionOperator.Equal, new Guid(appointmentId));
            }

            LinkEntity bookingstatus = new LinkEntity(mzk_patientappointment.EntityLogicalName, BookingStatus.EntityLogicalName, "mzk_appointmentstatus", "bookingstatusid", JoinOperator.Inner);
            bookingstatus.Columns = new ColumnSet(true);
            bookingstatus.EntityAlias = "bookingstatus";
            
            LinkEntity bookableresource = new LinkEntity(mzk_patientappointment.EntityLogicalName, BookableResource.EntityLogicalName, "mzk_resource", "bookableresourceid", JoinOperator.Inner);
            bookableresource.Columns = new ColumnSet(true);
            bookableresource.EntityAlias = "bookableresource";
            if (!string.IsNullOrEmpty(resourceId))
            {
                bookableresource.LinkCriteria.AddCondition("bookableresourceid", ConditionOperator.Equal, new Guid(resourceId));
            }
            
            LinkEntity user = new LinkEntity(BookableResource.EntityLogicalName, SystemUser.EntityLogicalName, "userid", "systemuserid", JoinOperator.Inner);
            user.Columns = new ColumnSet(true);
            user.EntityAlias = "user";
            bookableresource.LinkEntities.Add(user);

            LinkEntity contact = new LinkEntity(mzk_patientappointment.EntityLogicalName, Contact.EntityLogicalName, "mzk_customerid", "contactid", JoinOperator.Inner);

            if (!string.IsNullOrEmpty(patientId))
            {
                contact.LinkCriteria.AddCondition("contactid", ConditionOperator.Equal, new Guid(patientId));
            }

            contact.Columns = new ColumnSet("contactid","entityimage", "gendercode","birthdate",
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
                                                "mzk_salutation",
                                                "mzk_nationality",
                                                "mzk_nationalidexpirydate",
                                                "mzk_nationalidtype",
                                                "preferredcontactmethodcode",
                                                "mzk_vippatient",
                                                "firstname",
                                                "lastname",
                                                "mzk_age",
                                                "telephone2"); 
            contact.EntityAlias = "contact";
            query.LinkEntities.Add(contact);

            LinkEntity account = new LinkEntity(Contact.EntityLogicalName, Account.EntityLogicalName, "contactid", "primarycontactid", JoinOperator.LeftOuter);
            account.Columns = new ColumnSet(true);
            account.EntityAlias = "account";
            contact.LinkEntities.Add(account);

            LinkEntity patientCase = new LinkEntity(mzk_patientappointment.EntityLogicalName, Incident.EntityLogicalName, "mzk_caseid", "incidentid", JoinOperator.Inner);
            patientCase.Columns = new ColumnSet(true);
            patientCase.EntityAlias = "case";

            if (!string.IsNullOrEmpty(clinicId))
            {
                LinkEntity clinic = new LinkEntity(mzk_patientappointment.EntityLogicalName, msdyn_organizationalunit.EntityLogicalName, "mzk_organizationunit", "msdyn_organizationalunitid", JoinOperator.Inner);
                clinic.Columns = new ColumnSet(true);
                clinic.EntityAlias = "clinic";
                clinic.LinkCriteria.AddCondition("msdyn_organizationalunitid", ConditionOperator.Equal, new Guid(clinicId));

                query.LinkEntities.Add(clinic);
            }

            if (startDate != null && startDate != DateTime.MinValue)
            {
                query.Criteria.AddCondition("mzk_starttime", ConditionOperator.GreaterEqual, startDate);
            }

            if (endDate != null && endDate != DateTime.MaxValue)
            {          
                query.Criteria.AddCondition("mzk_endtime", ConditionOperator.LessEqual, endDate);
            }
                        
            query.LinkEntities.Add(bookableresource);
            query.LinkEntities.Add(bookingstatus);
            query.LinkEntities.Add(patientCase);

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {
                List<mzk_patientappointment> bookableResourceBookings = new List<mzk_patientappointment>(); 

                foreach (Entity entity in entitycollection.Entities)
                {
                    mzk_patientappointment patientAppointment = (mzk_patientappointment)entity;
                    Appointment model = new Appointment();

                    model = getAppointmentModelFilledByAppointment(patientAppointment, model);
                    
                    if (patientAppointment.Attributes.Contains("contact.contactid"))
                    {
                        model.patient = Patient.getPatientModelFilled(patientAppointment, new Patient(), contact.EntityAlias, account.EntityAlias);
                    }

                    if (patientAppointment.Attributes.Contains("bookingstatus.msdyn_fieldservicestatus"))
                    {
                        model.statusValue = ((OptionSetValue)(patientAppointment.Attributes["bookingstatus.msdyn_fieldservicestatus"] as AliasedValue).Value).Value;
                        model.status = (patientAppointment.FormattedValues["bookingstatus.msdyn_fieldservicestatus"]).ToString();
                    }

                    if (patientAppointment.Attributes.Contains("case.incidentid"))
                    {
                        model.caseId = ((patientAppointment.Attributes["case.incidentid"] as AliasedValue).Value).ToString();                        
                    }

                    appointments.Add(model);
                }
            }
            
            return appointments;
        }

        public string getScheduleType(string appointmentGuid = null)
        {
            string appointmentTypeId = "";

            SoapEntityRepository repo = SoapEntityRepository.GetService();    
            
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            QueryExpression query = new QueryExpression(mzk_patientappointment.EntityLogicalName);

            query.Criteria.AddCondition("mzk_patientappointmentid", ConditionOperator.Equal, new Guid(appointmentGuid));

            query.ColumnSet = new ColumnSet("mzk_appointmenttype");
                
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {
                mzk_patientappointment appointment = (mzk_patientappointment)entitycollection.Entities[0];

                if (appointment.Attributes.Contains("mzk_appointmenttype"))
                {
                    appointmentTypeId = ((EntityReference)(appointment.Attributes["mzk_appointmenttype"])).Id.ToString();
                }
            }
            
            return appointmentTypeId;            
        }

        public static bool IsAppointmentProgressive(string resourceId, string patientId)
        {
            List<Appointment> appointments = getUserAppointments(resourceId, null, DateTime.MinValue, DateTime.MaxValue, false, null, null, patientId);

            if (appointments != null && appointments.Count() > 0)
            {
                List<Appointment> appointments2 = appointments.Where(item => (msdyn_bookingsystemstatus)item.statusValue == msdyn_bookingsystemstatus.Completed || (msdyn_bookingsystemstatus)item.statusValue == msdyn_bookingsystemstatus.InProgress).ToList<Appointment>();

                if(appointments2.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

             return false;
        }
    }
}
