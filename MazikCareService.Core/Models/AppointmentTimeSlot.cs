using MazikCareService.CRMRepository;
using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using MazikLogger;

namespace MazikCareService.Core.Models
{
    public class AppointmentTimeSlot
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ResourceId { get; set; }
        public string SlotId { get; set; }

        public async Task<List<AppointmentTimeSlot>> getRescheduleVisitDates(string userId, DateTime currentBookDate,string workOrderId , DateTime startDate , DateTime endDate)
        {
            List<AppointmentTimeSlot> listObj = new List<AppointmentTimeSlot>();
            Configuration configuration = new Configuration();
            SoapEntityRepository repo = SoapEntityRepository.GetService();

            if (string.IsNullOrEmpty(userId))
            {
                throw new ValidationException("Resource not booked for this visit.");
            }

            if (!string.IsNullOrEmpty(workOrderId))
            {
                QueryExpression query = new QueryExpression(xrm.msdyn_workorder.EntityLogicalName);
                query.Criteria.AddCondition("msdyn_workorderid", ConditionOperator.Equal, new Guid(workOrderId));
                query.ColumnSet = new ColumnSet("mzk_visitstatus", "mzk_actualvisitstartdatetime", "mzk_numberoftimerescheduled");
                LinkEntity link1 = new LinkEntity(xrm.msdyn_workorder.EntityLogicalName,xrm.mzk_prescription.EntityLogicalName, "mzk_prescription", "mzk_prescriptionid",JoinOperator.LeftOuter)
                {
                    Columns = new ColumnSet("mzk_prescriptionstatus"),
                    EntityAlias="Prescription"
                };
                LinkEntity link2 = new LinkEntity(xrm.mzk_prescription.EntityLogicalName, xrm.Opportunity.EntityLogicalName, "mzk_referral", "opportunityid", JoinOperator.LeftOuter)
                {
                    Columns = new ColumnSet("mzk_status"),
                    EntityAlias="Referral"
                };
                LinkEntity link3 = new LinkEntity(xrm.msdyn_workorder.EntityLogicalName, xrm.msdyn_workordertype.EntityLogicalName, "msdyn_workordertype", "msdyn_workordertypeid", JoinOperator.LeftOuter)
                {
                    Columns = new ColumnSet("mzk_canberescheduledbythepatient"),
                    EntityAlias = "WorkOrderType"
                };
                link1.LinkEntities.Add(link2);
                query.LinkEntities.Add(link1);
                query.LinkEntities.Add(link3);
                EntityCollection entitycollection = repo.GetEntityCollection(query);

                configuration = configuration.getConfiguration();

                foreach (Entity entity in  entitycollection.Entities)
                {
                    if (entity.Attributes.Contains("WorkOrderType.mzk_canberescheduledbythepatient"))
                    {

                        if (entity.GetAttributeValue<AliasedValue>("WorkOrderType.mzk_canberescheduledbythepatient").Value.Equals(false))
                        {
                            QueryExpression bu_query = new QueryExpression(xrm.BusinessUnit.EntityLogicalName);
                            bu_query.Criteria.AddCondition("parentbusinessunitid", ConditionOperator.Null);
                            bu_query.ColumnSet = new ColumnSet("mzk_contactcentrenumber", "mzk_facility","name");
                            bu_query.TopCount = 1;
                            EntityCollection entityCollection = repo.GetEntityCollection(bu_query);
                            if (entityCollection.Entities.Count > 0)
                            {
                                if (entityCollection.Entities[0].Attributes.Contains("mzk_contactcentrenumber") && entityCollection.Entities[0].Attributes.Contains("name"))
                                {
                                    throw new ValidationException("This type of treatment/appointment cannot be re-scheduled using the app. Please call " +entityCollection.Entities[0]["name"]+" on " + entityCollection.Entities[0]["mzk_contactcentrenumber"].ToString() + " to discuss your treatment options.");
                                }
                                else
                                {
                                    throw new ValidationException("This type of treatment/appointment cannot be re-scheduled using the app.");
                                }
                            }
                            else
                            {
                                throw new ValidationException("This type of treatment/appointment cannot be re-scheduled using the app.");
                            }
                        }
                    }

                    if (entity.Attributes.Contains("mzk_visitstatus"))
                    {
                        if (entity["mzk_visitstatus"].Equals(new OptionSetValue(275380002)))
                        {
                            throw new ValidationException("Visit cannot be rescheduled as it is already delivered");
                        }
                        if (entity["mzk_visitstatus"].Equals(new OptionSetValue(275380000)))
                        {
                            if (entity.Attributes.Contains("mzk_proposedvisitdatetime"))
                            {
                                DateTime visitDate = (DateTime)entity["mzk_proposedvisitdatetime"];
                                int days = (int)(visitDate - DateTime.Now).TotalDays;
                                if (configuration.numberOfDays > 0)
                                {
                                    if (days < configuration.numberOfDays)
                                    {
                                        throw new ValidationException("Visit can only be rescheduled " + configuration.numberOfDays.ToString() + " days prior to your delivery date.");
                                    }
                                }
                            }
                        }
                        if (entity["mzk_visitstatus"].Equals(new OptionSetValue(275380001)))
                        {
                            if (entity.Attributes.Contains("mzk_scheduledstartdatetime"))
                            {
                                DateTime visitDate = (DateTime)entity["mzk_scheduledstartdatetime"];
                                int days = (int)(visitDate - DateTime.Now).TotalDays;
                                if (configuration.numberOfDays > 0)
                                {
                                    if (days < configuration.numberOfDays)
                                    {
                                        throw new ValidationException("Visit can only be rescheduled " + configuration.numberOfDays.ToString() + " days prior to your delivery date.");
                                    }
                                }
                            }
                        }
                    }

                    if (entity.Attributes.Contains("mzk_numberoftimerescheduled"))
                    {
                        int timesRescheduled = entity.GetAttributeValue<int>("mzk_numberoftimerescheduled");
                        if (timesRescheduled >= configuration.rescheduleDaysAllowed)
                        {
                            throw new ValidationException("You cannot reschedule more than "+configuration.rescheduleDaysAllowed.ToString()+" times");
                        }
                    }
                    
                    if (entity.Attributes.Contains("Referral.mzk_status"))
                    {
                        if (entity.GetAttributeValue<AliasedValue>("Referral.mzk_status").Value.Equals(new OptionSetValue(275380008)))
                        {
                            throw new ValidationException("Visit cannot be rescheduled as referral is marked finished");
                        }
                    }

                    if (entity.Attributes.Contains("Prescription.mzk_prescriptionstatus"))
                    {
                        if (entity.GetAttributeValue<AliasedValue>("Prescription.mzk_prescriptionstatus").Value.Equals(new OptionSetValue(275380002)))
                        {
                            throw new ValidationException("Visit cannot be rescheduled as prescription is on hold");
                        }
                    }
                    if (entity.Attributes.Contains("Prescription.mzk_prescriptionstatus"))
                    {
                        if (entity.GetAttributeValue<AliasedValue>("Prescription.mzk_prescriptionstatus").Value.Equals(new OptionSetValue(100000000)))
                        {
                            throw new ValidationException("Visit cannot be rescheduled as prescription is expired");
                        }
                    }
                }
            }

            QueryScheduleRequest scheduleRequest = new QueryScheduleRequest
            {
                ResourceId = new Guid(userId),
                Start = startDate,
                End = endDate,
                TimeCodes = new TimeCode[] { TimeCode.Available }
            };
            QueryScheduleResponse scheduleResponse = (QueryScheduleResponse)repo.Execute(scheduleRequest);

            if (scheduleResponse != null)
            {
                foreach (var item in scheduleResponse.TimeInfos)
                {
                    AppointmentTimeSlot modelObj = new AppointmentTimeSlot();

                    modelObj.StartDate = item.Start.Value;
                    modelObj.EndDate = item.End.Value;

                    listObj.Add(modelObj);
                }
            }

            return listObj;
        }
    }
}

