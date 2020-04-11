using MazikCareService.Core.Enums;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class PatientVisitAppointment
    {
        public string VisitNumber { get; set; }
        public string VisitStatus { get; set; }
        public string VisitType { get; set; }
        public DateTime VisitStartDate { get; set; }
        public DateTime VisitEndDate { get; set; }

        public string WorkOrderId { get; set; }

        public string CaseId { get; set; }

        public string ResourceId { get; set; }
        public string UserId { get; set; }
        public string ResourceName { get; set; }

        public string ResourceImage { get; set; }

        public async Task<List<PatientVisitAppointment>> getVisitAppointments(string patientId, VisitAppointmentFilterBy filterBy)
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            Configuration configuration = new Configuration();
            configuration = configuration.getConfiguration();
            List<PatientVisitAppointment> listObj = new List<PatientVisitAppointment>();
            QueryExpression query = new QueryExpression("msdyn_workorder");

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_visitnumber", "mzk_visitstatus",
                "mzk_visittype", "mzk_proposedvisitdatetime", "mzk_scheduledenddatetime", "mzk_scheduledstartdatetime",
                "msdyn_servicerequest", "mzk_actualvisitstartdatetime", "mzk_actualvisitenddatetime", "mzk_schedulestatus");

            query.Criteria.AddCondition("mzk_visittype", ConditionOperator.NotNull);
            query.Criteria.AddCondition("mzk_visitstatus", ConditionOperator.NotEqual, 275380003);//Cancelled



            if (filterBy == VisitAppointmentFilterBy.Today)
            {
                FilterExpression filterExpMain = query.Criteria.AddFilter(LogicalOperator.Or);

                FilterExpression filterExp1 = filterExpMain.AddFilter(LogicalOperator.And);
                FilterExpression subFilterExp1 = filterExp1.AddFilter(LogicalOperator.Or);
                subFilterExp1.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380000);//Proposed
                subFilterExp1.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
                filterExp1.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 00, 00, 00));
                filterExp1.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59));

                FilterExpression filterExp2 = filterExpMain.AddFilter(LogicalOperator.And);
                filterExp2.AddCondition("mzk_schedulestatus", ConditionOperator.Equal, 275380001);//Scheduled
                FilterExpression typeFilter = filterExp2.AddFilter(LogicalOperator.Or);
                FilterExpression nurseFilter = typeFilter.AddFilter(LogicalOperator.And);
                nurseFilter.AddCondition("mzk_visittype", ConditionOperator.Equal, 275380000);//Nurse Visit
                nurseFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 00, 00, 00));
                nurseFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59));
                FilterExpression deliveryFilter = typeFilter.AddFilter(LogicalOperator.And);
                deliveryFilter.AddCondition("mzk_visittype", ConditionOperator.Equal, 275380001);//Delivery Visit
                deliveryFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 00, 00, 00));
                deliveryFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59));

                FilterExpression filterExp3 = filterExpMain.AddFilter(LogicalOperator.And);
                FilterExpression subFilterExp3 = filterExp3.AddFilter(LogicalOperator.Or);
                subFilterExp3.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380002);//Completed
                subFilterExp3.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380016);//Delivered
                subFilterExp3.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380015);//Visit Started
                filterExp3.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 00, 00, 00));
                filterExp3.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59));

                //filterExp = filterExpMain.AddFilter(LogicalOperator.And);
                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
                //filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00));
                //filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));

                //filterExp = filterExpMain.AddFilter(LogicalOperator.And);
                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380002);//Completed
                //filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00));
                //filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));
            }
            else if (filterBy == VisitAppointmentFilterBy.Past)
            {
                FilterExpression filterExpMain = query.Criteria.AddFilter(LogicalOperator.Or);

                FilterExpression filterExp1 = filterExpMain.AddFilter(LogicalOperator.And);
                FilterExpression subFilterExp1 = filterExp1.AddFilter(LogicalOperator.Or);
                subFilterExp1.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380000);//Proposed
                subFilterExp1.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
                if (configuration.pastVisitMonths != 0)
                    filterExp1.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.GreaterThan, DateTime.UtcNow.Date.AddMonths(-configuration.pastVisitMonths));
                filterExp1.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.LessThan, DateTime.UtcNow.Date);

                FilterExpression filterExp2 = filterExpMain.AddFilter(LogicalOperator.And);
                filterExp2.AddCondition("mzk_schedulestatus", ConditionOperator.Equal, 275380001);//Scheduled
                FilterExpression typeFilter = filterExp2.AddFilter(LogicalOperator.Or);
                FilterExpression nurseFilter = typeFilter.AddFilter(LogicalOperator.And);
                nurseFilter.AddCondition("mzk_visittype", ConditionOperator.Equal, 275380000);//Nurse Visit
                if (configuration.pastVisitMonths != 0)
                    nurseFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterThan, DateTime.UtcNow.Date.AddMonths(-configuration.pastVisitMonths));
                nurseFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessThan, DateTime.UtcNow.Date);
                FilterExpression deliveryFilter = typeFilter.AddFilter(LogicalOperator.And);
                deliveryFilter.AddCondition("mzk_visittype", ConditionOperator.Equal, 275380001);//Delivery Visit
                if (configuration.pastVisitMonths != 0)
                    deliveryFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterThan, DateTime.UtcNow.Date.AddMonths(-configuration.pastVisitMonths));
                deliveryFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessThan, DateTime.UtcNow.Date);
                //nurseFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 00, 00, 00));
                //nurseFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59));

                FilterExpression filterExp3 = filterExpMain.AddFilter(LogicalOperator.And);
                FilterExpression subFilterExp3 = filterExp3.AddFilter(LogicalOperator.Or);
                subFilterExp3.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380002);//Completed
                subFilterExp3.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380016);//Delivered
                subFilterExp3.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380015);//Visit Started
                if (configuration.pastVisitMonths != 0)
                    filterExp3.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.GreaterThan, DateTime.UtcNow.Date.AddMonths(-configuration.pastVisitMonths));
                filterExp3.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.LessThan, DateTime.UtcNow.Date);
                #region Old Filter
                //FilterExpression filterExpMain = query.Criteria.AddFilter(LogicalOperator.Or);

                //FilterExpression filterExp = filterExpMain.AddFilter(LogicalOperator.And);
                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380000);//Proposed
                //if (configuration.pastVisitMonths != 0)
                //    filterExp.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.GreaterThan, DateTime.Now.AddMonths(-configuration.pastVisitMonths));
                //filterExp.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.LessThan, DateTime.Now.Date);

                //filterExp = filterExpMain.AddFilter(LogicalOperator.And);
                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
                //if (configuration.pastVisitMonths != 0)
                //    filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterThan, DateTime.Now.AddMonths(-configuration.pastVisitMonths));
                //filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessThan, DateTime.Now.Date);

                //filterExp = filterExpMain.AddFilter(LogicalOperator.And);
                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380002);//Completed
                //if (configuration.pastVisitMonths != 0)
                //    filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.GreaterThan, DateTime.Now.AddMonths(-configuration.pastVisitMonths));
                //filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.LessThan, DateTime.Now.Date);
                #endregion
            }
            else if (filterBy == VisitAppointmentFilterBy.Future)
            {
                FilterExpression filterExpMain = query.Criteria.AddFilter(LogicalOperator.Or);

                FilterExpression filterExp1 = filterExpMain.AddFilter(LogicalOperator.And);
                FilterExpression subFilterExp1 = filterExp1.AddFilter(LogicalOperator.Or);
                subFilterExp1.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380000);//Proposed
                subFilterExp1.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
                //if (configuration.pastVisitMonths != 0)
                //    filterExp1.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.LessThan, DateTime.UtcNow.Date.AddMonths(-configuration.futureVisitMonths));
                filterExp1.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.GreaterThan, DateTime.UtcNow.Date);

                FilterExpression filterExp2 = filterExpMain.AddFilter(LogicalOperator.And);
                filterExp2.AddCondition("mzk_schedulestatus", ConditionOperator.Equal, 275380001);//Scheduled
                FilterExpression typeFilter = filterExp2.AddFilter(LogicalOperator.Or);
                FilterExpression nurseFilter = typeFilter.AddFilter(LogicalOperator.And);
                nurseFilter.AddCondition("mzk_visittype", ConditionOperator.Equal, 275380000);//Nurse Visit
                if (configuration.futureVisitMonths != 0)
                    nurseFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessThan, DateTime.UtcNow.Date.AddMonths(configuration.futureVisitMonths));
                nurseFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterThan, DateTime.UtcNow.Date);
                FilterExpression deliveryFilter = typeFilter.AddFilter(LogicalOperator.And);
                deliveryFilter.AddCondition("mzk_visittype", ConditionOperator.Equal, 275380001);//Delivery Visit
                if (configuration.futureVisitMonths != 0)
                    deliveryFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessThan, DateTime.UtcNow.Date.AddMonths(configuration.futureVisitMonths));
                deliveryFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterThan, DateTime.UtcNow.Date);
                //nurseFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 00, 00, 00));
                //nurseFilter.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59));

                FilterExpression filterExp3 = filterExpMain.AddFilter(LogicalOperator.And);
                FilterExpression subFilterExp3 = filterExp3.AddFilter(LogicalOperator.Or);
                subFilterExp3.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380002);//Completed
                subFilterExp3.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380016);//Delivered
                subFilterExp3.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380015);//Visit Started
                if (configuration.futureVisitMonths != 0)
                    filterExp3.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.LessThan, DateTime.UtcNow.Date.AddMonths(configuration.futureVisitMonths));
                filterExp3.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.GreaterThan, DateTime.UtcNow.Date);
                #region Old Filter
                //FilterExpression filterExpMain = query.Criteria.AddFilter(LogicalOperator.Or);

                //FilterExpression filterExp = filterExpMain.AddFilter(LogicalOperator.And);
                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380000);//Proposed
                //filterExp.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.GreaterThan, DateTime.Now.Date);
                //if (configuration.futureVisitMonths != 0)
                //    filterExp.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.LessThan, DateTime.Now.AddMonths(configuration.futureVisitMonths));

                //filterExp = filterExpMain.AddFilter(LogicalOperator.And);
                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
                //filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterThan, DateTime.Now.Date);
                //if (configuration.futureVisitMonths != 0)
                //    filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessThan, DateTime.Now.AddMonths(configuration.futureVisitMonths));

                //filterExp = filterExpMain.AddFilter(LogicalOperator.And);
                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380002);//Completed
                //filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.GreaterThan, DateTime.Now.Date);
                //if (configuration.futureVisitMonths != 0)
                //    filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.LessThan, DateTime.Now.AddMonths(configuration.futureVisitMonths));
                #endregion

                //FilterExpression filterExp = query.Criteria.AddFilter(LogicalOperator.Or);

                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380004);//Ready for Dispense

                //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380000);//Proposed
            }




            //if (filterBy == VisitAppointmentFilterBy.Today)
            //{
            //    FilterExpression filterExpMain = query.Criteria.AddFilter(LogicalOperator.Or);

            //    FilterExpression filterExp = filterExpMain.AddFilter(LogicalOperator.And);
            //    filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380000);//Proposed
            //    filterExp.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00));
            //    filterExp.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));

            //    filterExp = filterExpMain.AddFilter(LogicalOperator.And);
            //    filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
            //    filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00));
            //    filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));

            //    filterExp = filterExpMain.AddFilter(LogicalOperator.And);
            //    filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380002);//Completed
            //    filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.GreaterEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00));
            //    filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.LessEqual, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));
            //}
            //else if (filterBy == VisitAppointmentFilterBy.Past)
            //{
            //    FilterExpression filterExpMain = query.Criteria.AddFilter(LogicalOperator.Or);

            //    FilterExpression filterExp = filterExpMain.AddFilter(LogicalOperator.And);
            //    filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380000);//Proposed
            //    if (configuration.pastVisitMonths != 0)
            //        filterExp.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.GreaterThan, DateTime.Now.AddMonths(-configuration.pastVisitMonths));
            //    filterExp.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.LessThan, DateTime.Now.Date);

            //    filterExp = filterExpMain.AddFilter(LogicalOperator.And);
            //    filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
            //    if (configuration.pastVisitMonths != 0)
            //        filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterThan, DateTime.Now.AddMonths(-configuration.pastVisitMonths));
            //    filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessThan, DateTime.Now.Date);

            //    filterExp = filterExpMain.AddFilter(LogicalOperator.And);
            //    filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380002);//Completed
            //    if (configuration.pastVisitMonths != 0)
            //        filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.GreaterThan, DateTime.Now.AddMonths(-configuration.pastVisitMonths));
            //    filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.LessThan, DateTime.Now.Date);
            //}
            //else if (filterBy == VisitAppointmentFilterBy.Future)
            //{
            //    FilterExpression filterExpMain = query.Criteria.AddFilter(LogicalOperator.Or);

            //    FilterExpression filterExp = filterExpMain.AddFilter(LogicalOperator.And);
            //    filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380000);//Proposed
            //    filterExp.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.GreaterThan, DateTime.Now.AddHours(5));
            //    if (configuration.futureVisitMonths != 0)
            //        filterExp.AddCondition("mzk_proposedvisitdatetime", ConditionOperator.LessThan, DateTime.Now.AddMonths(configuration.futureVisitMonths));

            //    filterExp = filterExpMain.AddFilter(LogicalOperator.And);
            //    filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
            //    filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.GreaterThan, DateTime.Now.AddHours(5));
            //    if (configuration.futureVisitMonths != 0)
            //        filterExp.AddCondition("mzk_scheduledstartdatetime", ConditionOperator.LessThan, DateTime.Now.AddMonths(configuration.futureVisitMonths));

            //    filterExp = filterExpMain.AddFilter(LogicalOperator.And);
            //    filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380002);//Completed
            //    filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.GreaterThan, DateTime.Now.Date.AddHours(5));
            //    if (configuration.futureVisitMonths != 0)
            //        filterExp.AddCondition("mzk_actualvisitstartdatetime", ConditionOperator.LessThan, DateTime.Now.AddMonths(configuration.futureVisitMonths));


            //    //FilterExpression filterExp = query.Criteria.AddFilter(LogicalOperator.Or);

            //    //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380001);//Confirmed
            //    //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380004);//Ready for Dispense

            //    //filterExp.AddCondition("mzk_visitstatus", ConditionOperator.Equal, 275380000);//Proposed
            //}


            query.AddOrder("mzk_visitnumber", OrderType.Ascending);
            //query.AddOrder("mzk_scheduledstartdatetime", OrderType.Descending);
            LinkEntity workOrderType = new LinkEntity("msdyn_workorder", "msdyn_workordertype", "msdyn_workordertype", "msdyn_workordertypeid", JoinOperator.Inner);
            workOrderType.Columns = new ColumnSet("mzk_duration", "mzk_durationunit");
            workOrderType.EntityAlias = "WorkOrderType";
            query.LinkEntities.Add(workOrderType);

            LinkEntity entityTypePatient = new LinkEntity("msdyn_workorder", "account", "msdyn_serviceaccount", "accountid", JoinOperator.Inner);

            entityTypePatient.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(false);
            entityTypePatient.EntityAlias = "Patient";
            entityTypePatient.LinkCriteria.AddCondition("primarycontactid", ConditionOperator.Equal, new Guid(patientId));

            query.LinkEntities.Add(entityTypePatient);

            LinkEntity entityTypeResource = new LinkEntity("msdyn_workorder", "bookableresource", "mzk_resource", "bookableresourceid", JoinOperator.LeftOuter);

            entityTypeResource.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("name", "userid", "bookableresourceid");
            entityTypeResource.EntityAlias = "BookingResource";

            query.LinkEntities.Add(entityTypeResource);

            LinkEntity entityTypeUser = new LinkEntity("bookableresource", "systemuser", "userid", "systemuserid", JoinOperator.LeftOuter);

            entityTypeUser.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("entityimage");
            entityTypeUser.EntityAlias = "ResourceUser";

            entityTypeResource.LinkEntities.Add(entityTypeUser);

            EntityCollection entitycol = repo.GetEntityCollection(query);

            if (entitycol != null && entitycol.Entities != null)
            {
                foreach (Entity entity in entitycol.Entities)
                {
                    PatientVisitAppointment modelObj = new PatientVisitAppointment();

                    modelObj.WorkOrderId = entity.Id.ToString();

                    if (entity.Attributes.Contains("mzk_visitnumber"))
                    {
                        modelObj.VisitNumber = entity["mzk_visitnumber"].ToString();
                    }
                    if (entity.Attributes.Contains("mzk_visitstatus"))
                    {
                        modelObj.VisitStatus = entity.FormattedValues["mzk_visitstatus"].ToString();
                    }

                    if (entity.Attributes.Contains("mzk_visittype"))
                    {
                        modelObj.VisitType = entity.FormattedValues["mzk_visittype"].ToString();
                    }

                    
                    if (entity.Attributes.Contains("mzk_proposedvisitdatetime"))
                    {
                        modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_proposedvisitdatetime"]);
                    }

                    if (modelObj.VisitStatus.Equals("Proposed"))
                    {
                        if (entity.Attributes.Contains("mzk_proposedvisitdatetime"))
                        {
                            modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_proposedvisitdatetime"]);
                            if (entity.Attributes.Contains("WorkOrderType.mzk_duration"))
                            {
                                if (entity.Attributes.Contains("WorkOrderType.mzk_durationunit"))
                                {
                                    if (entity.FormattedValues["WorkOrderType.mzk_durationunit"].Equals("Minutes"))
                                    {
                                        modelObj.VisitEndDate = modelObj.VisitStartDate.AddMinutes(Convert.ToDouble(entity.GetAttributeValue<AliasedValue>("WorkOrderType.mzk_duration").Value));
                                    }
                                    else if (entity.FormattedValues["WorkOrderType.mzk_durationunit"].Equals("Hours"))
                                    {
                                        modelObj.VisitEndDate = modelObj.VisitStartDate.AddHours(Convert.ToDouble(entity.GetAttributeValue<AliasedValue>("WorkOrderType.mzk_duration").Value));
                                    }
                                }
                            }
                                
                        }
                    }
                    //if (modelObj.VisitStatus.Equals("Scheduled"))
                    //{
                    //    if (entity.Attributes.Contains("mzk_scheduledenddatetime"))
                    //    {
                    //        modelObj.VisitEndDate = Convert.ToDateTime(entity["mzk_scheduledenddatetime"]);
                    //    }

                    //    if (entity.Attributes.Contains("mzk_scheduledstartdatetime"))
                    //    {
                    //        modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_scheduledstartdatetime"]);
                    //    }
                    //}
                    if (modelObj.VisitStatus.Equals("Completed"))
                    {
                        if (entity.Attributes.Contains("mzk_actualvisitenddatetime"))
                        {
                            modelObj.VisitEndDate = Convert.ToDateTime(entity["mzk_actualvisitenddatetime"]);
                        }

                        if (entity.Attributes.Contains("mzk_actualvisitstartdatetime"))
                        {
                            modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_actualvisitstartdatetime"]);
                        }
                    }

                    //if (modelObj.VisitType != null && modelObj.VisitType.Equals("Nursing Visit"))
                    //{
                    //    if (entity.Attributes.Contains("mzk_expectedarrivalstarttimewindow"))
                    //        modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_expectedarrivalstarttimewindow"]);
                    //    if (entity.Attributes.Contains("mzk_expectedarrivalendtimewindow"))
                    //        modelObj.VisitEndDate = Convert.ToDateTime(entity["mzk_expectedarrivalendtimewindow"]);
                    //}

                    if (entity.Attributes.Contains("msdyn_servicerequest"))
                    {
                        modelObj.CaseId = (entity["msdyn_servicerequest"] as EntityReference).Id.ToString();
                    }

                    if (entity.Attributes.Contains("BookingResource.bookableresourceid"))
                    {
                        modelObj.ResourceId = (entity["BookingResource.bookableresourceid"] as AliasedValue).Value.ToString();
                    }

                    if (entity.Attributes.Contains("ResourceUser.entityimage") && entity["ResourceUser.entityimage"] != null)
                    {
                        modelObj.ResourceImage = Convert.ToBase64String((entity["ResourceUser.entityimage"] as AliasedValue).Value as Byte[]);
                    }

                    if (entity.Attributes.Contains("BookingResource.name"))
                    {
                        modelObj.ResourceName = (entity["BookingResource.name"] as AliasedValue).Value.ToString();
                    }

                    if (entity.Attributes.Contains("BookingResource.userid"))
                    {
                        modelObj.UserId = ((entity["BookingResource.userid"] as AliasedValue).Value as EntityReference).Id.ToString();
                    }
                    if (entity.Attributes.Contains("mzk_schedulestatus"))
                    {
                        if ((entity["mzk_schedulestatus"] as OptionSetValue).Value.Equals(275380001))
                        {
                            if (entity.Attributes.Contains("mzk_scheduledenddatetime"))
                            {
                                modelObj.VisitEndDate = Convert.ToDateTime(entity["mzk_scheduledenddatetime"]);
                            }

                            if (entity.Attributes.Contains("mzk_scheduledstartdatetime"))
                            {
                                modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_scheduledstartdatetime"]);
                            }
                        }
                    }

                    listObj.Add(modelObj);
                }
            }

            return listObj;
        }

        public async Task<bool> createRescheduleRequest(string workOrderId, DateTime startDate, DateTime endDate)
        {
            bool status = true;
            int count = 0;
            SoapEntityRepository repo = SoapEntityRepository.GetService();

            Entity entity = repo.GetEntity(xrm.msdyn_workorder.EntityLogicalName, new Guid(workOrderId), new ColumnSet("mzk_numberoftimerescheduled"));
            if (entity.Attributes.Contains("mzk_numberoftimerescheduled"))
            {
                count = Int32.Parse(entity["mzk_numberoftimerescheduled"].ToString());
            }


            Entity workOrder = repo.GetEntity("msdyn_workorder",new Guid(workOrderId),new ColumnSet("mzk_scheduledstartdatetime"));//new Entity("msdyn_workorder", new Guid(workOrderId));
            workOrder["mzk_lastscheduledstartdatetime"] = workOrder["mzk_scheduledstartdatetime"];
            workOrder["mzk_scheduledstartdatetime"] = startDate;
            workOrder["mzk_scheduledenddatetime"] = endDate;
            workOrder["mzk_numberoftimerescheduled"] = count + 1;

            repo.UpdateEntity(workOrder);

            QueryExpression query = new QueryExpression("bookableresourcebooking");
            query.Criteria.AddCondition("msdyn_workorder", ConditionOperator.Equal, workOrder.Id);

            EntityCollection collection = repo.GetEntityCollection(query);

            if (collection != null && collection.Entities != null && collection.Entities.Count > 0)
            {
                Entity booking = collection.Entities[0];
                booking["starttime"] = startDate;
                booking["endtime"] = endDate;
                booking["msdyn_actualarrivaltime"] = null;
                repo.UpdateEntity(booking);
            }

            return status;
        }


        public async Task<List<PatientVisitProducts>> getActiveVisits(string patientId)
        {
            if (!string.IsNullOrEmpty(patientId))
            {
                List<PatientVisitProducts> workOrders = new List<PatientVisitProducts>();
                QueryExpression query = new QueryExpression(xrm.Incident.EntityLogicalName);
                query.Criteria.AddCondition("customerid", ConditionOperator.Equal, new Guid(patientId));
                query.ColumnSet = new ColumnSet("mzk_contract");

                LinkEntity caseWorkOrder = new LinkEntity(xrm.Incident.EntityLogicalName, xrm.msdyn_workorder.EntityLogicalName, "incidentid", "msdyn_servicerequest", JoinOperator.Inner)
                {
                    Columns = new ColumnSet("mzk_scheduledstartdatetime", "mzk_visitnumber", "msdyn_workorderid"),
                    LinkCriteria =
                    {
                    Conditions = 
                    {
                        new ConditionExpression("mzk_visitstatus",ConditionOperator.Equal, 275380001),//Confirmed (Previously it was Scheduled 13)
                        new ConditionExpression("mzk_visittype",ConditionOperator.Equal, 275380001)//Delivery Visit
                    },
                        FilterOperator = LogicalOperator.And
                    },
                    EntityAlias = "workOrder"
                };
                query.LinkEntities.Add(caseWorkOrder);

                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entityCollection = repo.GetEntityCollection(query);
                var groupedWorkOrders = entityCollection.Entities.GroupBy(item => (item.GetAttributeValue<Guid>("incidentid")));
                foreach (var groupedVisits in groupedWorkOrders)
                {

                    foreach (Entity entity in groupedVisits)
                    {
                        PatientVisitProducts workOrder = new PatientVisitProducts();
                        if (entity.Attributes.Contains("workOrder.msdyn_workorderid"))
                        {
                            workOrder.workOrderId = entity.GetAttributeValue<AliasedValue>("workOrder.msdyn_workorderid").Value.ToString();
                        }
                        if (entity.Attributes.Contains("workOrder.mzk_scheduledstartdatetime"))
                        {
                            workOrder.visitDate = (DateTime)(entity.GetAttributeValue<AliasedValue>("workOrder.mzk_scheduledstartdatetime").Value);
                        }
                        if (entity.Attributes.Contains("workOrder.mzk_visitnumber"))
                        {
                            workOrder.visitNumber = Int32.Parse(entity.GetAttributeValue<AliasedValue>("workOrder.mzk_visitnumber").Value.ToString());
                        }
                        if (entity.Attributes.Contains("mzk_contract"))
                        {
                            workOrder.contract = (entity["mzk_contract"] as EntityReference).Name;
                        }

                        workOrders.Add(workOrder);
                      
                    }
                   
                }
                return workOrders;
            }
            else
            {
                throw new ValidationException("Patient Id missing");
            }
        }

        public async Task<bool> cancelVisit(string workOrderId, string cancellationReason, string userId)
        {
            try
            {
                if (!string.IsNullOrEmpty(workOrderId))
                {
                    SoapEntityRepository repo = SoapEntityRepository.GetService();
                    Entity workOrder = new Entity(xrm.msdyn_workorder.EntityLogicalName);
                    workOrder.Id = new Guid(workOrderId);
                    workOrder["mzk_visitstatus"] = new OptionSetValue(275380003);//Cancelled
                    workOrder["mzk_cancelreason"] = new EntityReference("mzk_reasoncode", new Guid(cancellationReason));
                    workOrder["mzk_cancelledby"] = new EntityReference("contact", new Guid(userId));
                    workOrder["mzk_cancellationdatetime"] = DateTime.Now;
                    repo.UpdateEntity(workOrder);
                    return true;
                }
                else
                {
                    throw new ValidationException("WorkOrder Id missing");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<PatientVisitAppointment> getVisitDetails(string workOrderId)
        {
            if (!string.IsNullOrEmpty(workOrderId))
            {
                try
                {
                    PatientVisitAppointment modelObj = new PatientVisitAppointment();
                    SoapEntityRepository repo = SoapEntityRepository.GetService();
                    Entity entity = repo.GetEntity("msdyn_workorder", new Guid(workOrderId), new ColumnSet("mzk_visitnumber", "mzk_visitstatus", "mzk_visittype", "mzk_proposedvisitdatetime", "mzk_scheduledenddatetime", "mzk_scheduledstartdatetime", "msdyn_servicerequest", "mzk_actualvisitstartdatetime", "mzk_actualvisitenddatetime"));
                    modelObj.WorkOrderId = entity.Id.ToString();

                    if (entity.Attributes.Contains("mzk_visitnumber"))
                    {
                        modelObj.VisitNumber = entity["mzk_visitnumber"].ToString();
                    }
                    if (entity.Attributes.Contains("mzk_visitstatus"))
                    {
                        modelObj.VisitStatus = entity.FormattedValues["mzk_visitstatus"].ToString();
                    }

                    if (entity.Attributes.Contains("mzk_visittype"))
                    {
                        modelObj.VisitType = entity.FormattedValues["mzk_visittype"].ToString();
                    }

                    if (entity.Attributes.Contains("mzk_scheduledenddatetime"))
                    {
                        modelObj.VisitEndDate = Convert.ToDateTime(entity["mzk_scheduledenddatetime"]);
                    }

                    if (entity.Attributes.Contains("mzk_scheduledstartdatetime"))
                    {
                        modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_scheduledstartdatetime"]);
                    }
                    else if (entity.Attributes.Contains("mzk_proposedvisitdatetime"))
                    {
                        modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_proposedvisitdatetime"]);
                    }

                    if (modelObj.VisitStatus.Equals("Proposed"))
                    {
                        if (entity.Attributes.Contains("mzk_proposedvisitdatetime"))
                            modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_proposedvisitdatetime"]);
                    }
                    if (modelObj.VisitStatus.Equals("Scheduled"))
                    {
                        if (entity.Attributes.Contains("mzk_scheduledenddatetime"))
                        {
                            modelObj.VisitEndDate = Convert.ToDateTime(entity["mzk_scheduledenddatetime"]);
                        }

                        if (entity.Attributes.Contains("mzk_scheduledstartdatetime"))
                        {
                            modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_scheduledstartdatetime"]);
                        }
                    }
                    if (modelObj.VisitStatus.Equals("Completed"))
                    {
                        if (entity.Attributes.Contains("mzk_actualvisitenddatetime"))
                        {
                            modelObj.VisitEndDate = Convert.ToDateTime(entity["mzk_actualvisitenddatetime"]);
                        }

                        if (entity.Attributes.Contains("mzk_actualvisitstartdatetime"))
                        {
                            modelObj.VisitStartDate = Convert.ToDateTime(entity["mzk_actualvisitstartdatetime"]);
                        }
                    }
                    if (entity.Attributes.Contains("msdyn_servicerequest"))
                    {
                        modelObj.CaseId = (entity["msdyn_servicerequest"] as EntityReference).Id.ToString();
                    }
                    return modelObj;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new ValidationException("WorkOrder Id missing");
            }
        }
    }
}
