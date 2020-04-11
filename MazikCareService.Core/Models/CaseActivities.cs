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
using System.Linq;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class CaseActivities
    {
        public string activityMaster { get; set; }
        public int activityNumber { get; set; }
        public string casePathway { get; set; }
        public string casePathwayName { get; set; }
        public string status { get; set; }
        
        public async Task<List<CaseActivities>> getCaseActivities(string referralId)
        {
            List<CaseActivities> caseActivities = new List<CaseActivities>();
            QueryExpression query = new QueryExpression(Opportunity.EntityLogicalName);
            if (!string.IsNullOrEmpty(referralId))
            {
                query.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, new Guid(referralId));
                LinkEntity ReferralPrescription = new LinkEntity(Opportunity.EntityLogicalName, xrm.mzk_prescription.EntityLogicalName, "opportunityid", "mzk_referral", JoinOperator.Inner) { };
                LinkEntity PrescriptionCase = new LinkEntity(xrm.mzk_prescription.EntityLogicalName, Incident.EntityLogicalName, "mzk_case", "incidentid", JoinOperator.Inner) { };
                LinkEntity CasePathway = new LinkEntity(Incident.EntityLogicalName, xrm.mzk_casepathway.EntityLogicalName, "incidentid", "mzk_caseid", JoinOperator.Inner)
                {
                    Columns = new ColumnSet("mzk_code"),
                    EntityAlias="CasePathway"
                };
                LinkEntity CasePathwayState = new LinkEntity(xrm.mzk_casepathway.EntityLogicalName, xrm.mzk_casepathwaystate.EntityLogicalName, "mzk_casepathwayid", "mzk_casepathway", JoinOperator.Inner)
                {
                    Columns = new ColumnSet("mzk_activitymaster", "mzk_activitynumber", "mzk_casepathway", "mzk_status"),
                    LinkCriteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression("mzk_statetype",ConditionOperator.Equal,1) // StateType = Activity
                        }
                    },
                    EntityAlias = "CasePathwayState"
                };
                CasePathway.LinkEntities.Add(CasePathwayState);
                PrescriptionCase.LinkEntities.Add(CasePathway);
                ReferralPrescription.LinkEntities.Add(PrescriptionCase);
                query.LinkEntities.Add(ReferralPrescription);

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                foreach (Entity entity in entitycollection.Entities)
                {
                    CaseActivities caseActivity = new CaseActivities();
                    if (entity.Attributes.Contains("CasePathwayState.mzk_activitymaster"))
                    {
                        caseActivity.activityMaster = ((EntityReference)(entity.GetAttributeValue<AliasedValue>("CasePathwayState.mzk_activitymaster")).Value).Name.ToString();
                    }
                    if (entity.Attributes.Contains("CasePathwayState.mzk_activitynumber"))
                    {
                        caseActivity.activityNumber = (int)((AliasedValue)entity["CasePathwayState.mzk_activitynumber"]).Value;
                    }
                    if (entity.Attributes.Contains("CasePathwayState.mzk_casepathway"))
                    {
                       caseActivity.casePathway= ((EntityReference)(entity.GetAttributeValue<AliasedValue>("CasePathwayState.mzk_casepathway")).Value).Id.ToString();
                    }
                    if (entity.Attributes.Contains("CasePathway.mzk_code"))
                    {
                        caseActivity.casePathwayName = ((AliasedValue)entity["CasePathway.mzk_code"]).Value.ToString();
                    }
                    if (entity.Attributes.Contains("CasePathwayState.mzk_status"))
                    {
                        caseActivity.status = entity.FormattedValues["CasePathwayState.mzk_status"].ToString();
                    }
                    caseActivities.Add(caseActivity);
                }


                return caseActivities;
            }
            else {
                throw new ValidationException("Referral Id not Found");
            }
        }
    }
}
