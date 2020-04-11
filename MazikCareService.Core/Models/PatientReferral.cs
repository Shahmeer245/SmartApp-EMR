using Helper;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;
using MazikLogger;
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
    public class PatientReferral
    {
        public string name { get; set; }
        public string contract { get; set; }
        public string diagnosis { get; set; }
        public DateTime requestedDateTime { get; set; }
        public string referringPrescriber { get; set; }
        public string referringHealthCareProvider { get; set; }
        public string referralId { get; set; }

        public async Task<List<PatientReferral>> viewReferrals(string patientId)
        {
            try
            {
                List<PatientReferral> patientReferrals = new List<PatientReferral>();
                if (!string.IsNullOrEmpty(patientId))
                {
                    
                    QueryExpression query = new QueryExpression(xrm.Opportunity.EntityLogicalName);
                    FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                    childFilter.AddCondition("parentcontactid", ConditionOperator.Equal, new Guid(patientId));
                    childFilter.AddCondition("mzk_opportunitytype", ConditionOperator.Equal,275380000);
                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("opportunityid", "name","mzk_contract", "createdon", "mzk_referringprescriber", "mzk_referringhealthcareprovider", "mzk_diagnosis");//"estimatedclosedate"
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                    foreach (Entity entity in entitycollection.Entities)
                    {
                        PatientReferral patientReferral = new PatientReferral();
                        if (entity.Attributes.Contains("opportunityid"))
                            patientReferral.referralId = entity["opportunityid"].ToString();
                        if (entity.Attributes.Contains("name"))
                            patientReferral.name = entity["name"].ToString();
                        if (entity.Attributes.Contains("mzk_contract"))
                        {
                            EntityReference contractLookUp = (EntityReference)entity.Attributes["mzk_contract"];
                            patientReferral.contract = contractLookUp.Name;
                        }
                        if (entity.Attributes.Contains("mzk_diagnosis"))
                        {
                            EntityReference diagnosisLookUp = (EntityReference)entity.Attributes["mzk_diagnosis"];
                            patientReferral.diagnosis = diagnosisLookUp.Name;
                        }

                        if (entity.Attributes.Contains("createdon"))
                            patientReferral.requestedDateTime = (DateTime)entity["createdon"];
                        if (entity.Attributes.Contains("mzk_referringprescriber"))
                        {
                            EntityReference referringPrescriberLookUp = (EntityReference)entity.Attributes["mzk_referringprescriber"];
                            patientReferral.referringPrescriber = referringPrescriberLookUp.Name;
                        }
                        if (entity.Attributes.Contains("mzk_referringhealthcareprovider"))
                        {
                            EntityReference healthCareProviderLookUp = (EntityReference)entity.Attributes["mzk_referringhealthcareprovider"];
                            patientReferral.referringHealthCareProvider = healthCareProviderLookUp.Name;
                        }

                        patientReferrals.Add(patientReferral);
                    }
                    return patientReferrals;
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
    }
}
