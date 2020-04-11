using Helper;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MazikCareService.Core.Models
{
    public class Configuration
    {
        public int numberOfDays { get; set; }
        public int rescheduleDaysAllowed { get; set; }
        public int ordersAllowedPerVisit { get; set; }
        public int orderingPriorHours { get; set; }
        public string patientAppHeader1Label { get; set; }
        public int patientAppHeader1Value { get; set; }
        public string patientAppHeader2Label { get; set; }
        public int patientAppHeader2Value { get; set; }
        public string patientAppHeader3Label { get; set; }
        public int patientAppHeader3Value { get; set; }
        public int cannotScheduleFailedDeliveriesAfterDays { get; set; }
        public string termsOfUse { get; set; }
        public string privacyPolicy { get; set; }
        public int pastVisitMonths { get; set; }
        public int futureVisitMonths { get; set; }
        public int snoozeAllowedTimes { get; set; }
        public int snoozeDefaultTime { get; set; }

        public Configuration getConfiguration()
        {
            Configuration configuration = new Configuration();
            QueryExpression query = new QueryExpression("mzk_configuration");
            query.ColumnSet = new ColumnSet("mzk_numberofdays", "mzk_timesallowedvisitreschedule", "mzk_numberofordersallowedpervisit",
                "mzk_snoozeallowedtimes", "mzk_snoozetimeinminutes", "mzk_ordernotallowedpriortothedeliveryhours", "mzk_patientheaderapp1", "mzk_patientheaderapp2", "mzk_patientheaderapp3", "mzk_cannotschedulefaileddeliveriesafterdays", "mzk_futurevisitsmonths", "mzk_pastvisitmonths");
            query.TopCount = 1;
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            foreach (Entity entity in entitycollection.Entities)
            {
                if (entity.Attributes.Contains("mzk_numberofdays"))
                {
                    configuration.numberOfDays = Int32.Parse(entity["mzk_numberofdays"].ToString());
                }
                if (entity.Attributes.Contains("mzk_timesallowedvisitreschedule"))
                {
                    configuration.rescheduleDaysAllowed = Int32.Parse(entity["mzk_timesallowedvisitreschedule"].ToString());
                }
                if (entity.Attributes.Contains("mzk_numberofordersallowedpervisit"))
                {
                    configuration.ordersAllowedPerVisit = Int32.Parse(entity["mzk_numberofordersallowedpervisit"].ToString());
                }
                if (entity.Attributes.Contains("mzk_ordernotallowedpriortothedeliveryhours"))
                {
                    configuration.orderingPriorHours = Int32.Parse(entity["mzk_ordernotallowedpriortothedeliveryhours"].ToString());
                }
                if (entity.Attributes.Contains("mzk_patientheaderapp1"))
                {
                    configuration.patientAppHeader1Label = entity.FormattedValues["mzk_patientheaderapp1"].ToString();
                    configuration.patientAppHeader1Value = (entity["mzk_patientheaderapp1"] as OptionSetValue).Value;
                }
                if (entity.Attributes.Contains("mzk_patientheaderapp2"))
                {
                    configuration.patientAppHeader2Label = entity.FormattedValues["mzk_patientheaderapp2"].ToString();
                    configuration.patientAppHeader2Value = (entity["mzk_patientheaderapp2"] as OptionSetValue).Value;
                }
                if (entity.Attributes.Contains("mzk_patientheaderapp3"))
                {
                    configuration.patientAppHeader3Label = entity.FormattedValues["mzk_patientheaderapp3"].ToString();
                    configuration.patientAppHeader3Value = (entity["mzk_patientheaderapp3"] as OptionSetValue).Value;
                }
                if (entity.Attributes.Contains("mzk_cannotschedulefaileddeliveriesafterdays"))
                {
                    configuration.cannotScheduleFailedDeliveriesAfterDays = Int32.Parse(entity["mzk_cannotschedulefaileddeliveriesafterdays"].ToString());
                }
                if (entity.Attributes.Contains("mzk_pastvisitmonths"))
                {
                    configuration.pastVisitMonths = Convert.ToInt32(entity["mzk_pastvisitmonths"]);
                }
                if (entity.Attributes.Contains("mzk_futurevisitsmonths"))
                {
                    configuration.futureVisitMonths = Convert.ToInt32(entity["mzk_futurevisitsmonths"]);
                }
                if (entity.Attributes.Contains("mzk_snoozeallowedtimes"))
                {
                    configuration.snoozeAllowedTimes = Convert.ToInt32(entity["mzk_snoozeallowedtimes"]);

                }
                if (entity.Attributes.Contains("mzk_snoozetimeinminutes"))
                {
                    configuration.snoozeDefaultTime = Convert.ToInt32(entity["mzk_snoozetimeinminutes"]);
                }
            }
            return configuration;
        }
        public async Task<Configuration> getTermsAndConditions()
        {
            try
            {
                Configuration configuration = new Configuration();
                QueryExpression query = new QueryExpression("mzk_configuration");
                query.ColumnSet = new ColumnSet("mzk_termsofuse", "mzk_privacypolicy");
                query.TopCount = 1;
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                foreach (Entity entity in entitycollection.Entities)
                {
                    if (entity.Attributes.Contains("mzk_termsofuse"))
                    {
                        configuration.termsOfUse = entity["mzk_termsofuse"].ToString();
                    }
                    if (entity.Attributes.Contains("mzk_privacypolicy"))
                    {
                        configuration.privacyPolicy = entity["mzk_privacypolicy"].ToString();
                    }
                }
                return configuration;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> getConfigurationTimeZone()
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression("mzk_configuration");
            LinkEntity ConfigurationTimeZone = new LinkEntity("mzk_configuration", "mzk_ordertimezone", "mzk_configurationid", "mzk_configuration", JoinOperator.Inner)
            {
                Columns  = new ColumnSet("mzk_timezone"),
                EntityAlias = "TimeZone"
            };
            query.LinkEntities.Add(ConfigurationTimeZone);
            EntityCollection entityCollection = repo.GetEntityCollection(query);
            foreach (Entity entity in entityCollection.Entities)
            {
                DateTime date = DateTime.UtcNow;
                date = TimeZoneInfo.ConvertTime(date, entity.GetAttributeValue<AliasedValue>("TimeZone.mzk_timezone").Value as TimeZoneInfo);
                //date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, entity.GetAttributeValue<AliasedValue>("TimeZone.mzk_timezone").Value.ToString());
            }
            return true;
        }
    }
}
