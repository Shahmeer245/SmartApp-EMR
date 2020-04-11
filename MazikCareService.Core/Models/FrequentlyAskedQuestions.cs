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
    public class FrequentlyAskedQuestions
    {
        public string FAQId { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public List<QuestionAndAnswer> qnA { get; set; }

        public async Task<List<FrequentlyAskedQuestions>> getFAQS(string patientId)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientId))
                {
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    List<FrequentlyAskedQuestions> FAQS = new List<FrequentlyAskedQuestions>();
                    FAQS = getGeneralFAQS(entityRepository).Result;
                    FAQS.AddRange(getContractSpecificFAQS(patientId, entityRepository).Result);
                    return FAQS;
                }
                else
                {
                    throw new ValidationException("Patient Id missing");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<QuestionAndAnswer>> getFAQDetails(string FAQId)
        {
            try
            {
                if (!string.IsNullOrEmpty(FAQId))
                {
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    List<QuestionAndAnswer> QnAS = new List<QuestionAndAnswer>();
                    QueryExpression query = new QueryExpression("mzk_faqconfigurationqna");
                    query.ColumnSet = new ColumnSet("mzk_question", "mzk_answer");
                    query.Criteria.AddCondition("mzk_faqconfiguration", ConditionOperator.Equal, FAQId);
                    EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                    foreach (Entity entity in entitycollection.Entities)
                    {
                        QuestionAndAnswer QnA = new QuestionAndAnswer();
                        if (entity.Attributes.Contains("mzk_question"))
                        {
                            QnA.Question = entity["mzk_question"].ToString();
                        }
                        if (entity.Attributes.Contains("mzk_answer"))
                        {
                            QnA.Answer = entity["mzk_answer"].ToString();
                        }
                        QnAS.Add(QnA);
                    }
                    return QnAS;
                }
                else
                {
                    throw new ValidationException("Id is missing");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<FrequentlyAskedQuestions>> getGeneralFAQS(SoapEntityRepository entityRepository)
        {
            try
            {
                List<FrequentlyAskedQuestions> FAQS = new List<FrequentlyAskedQuestions>();
                QueryExpression query = new QueryExpression("mzk_faqconfiguration");
                query.Criteria.AddCondition("mzk_servicespecific", ConditionOperator.Equal, false);
                query.ColumnSet = new ColumnSet("mzk_faqconfigurationid", "mzk_name", "entityimage");
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                foreach (Entity entity in entitycollection.Entities)
                {
                    FrequentlyAskedQuestions FAQ = new FrequentlyAskedQuestions();
                    if (entity.Attributes.Contains("mzk_faqconfigurationid"))
                    {
                        FAQ.FAQId = entity["mzk_faqconfigurationid"].ToString();
                    }
                    if (entity.Attributes.Contains("mzk_name"))
                    {
                        FAQ.name = entity["mzk_name"].ToString();
                    }
                    if (entity.Attributes.Contains("entityimage"))
                    {
                        byte[] imageInBytes = (entity.Attributes["entityimage"]) as byte[];
                        FAQ.image = Convert.ToBase64String(imageInBytes);
                    }
                    FAQS.Add(FAQ);
                }
                return FAQS;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<FrequentlyAskedQuestions>> getContractSpecificFAQS(string patientId, SoapEntityRepository entityRepository)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientId))
                {
                    List<FrequentlyAskedQuestions> FAQS = new List<FrequentlyAskedQuestions>();
                    QueryExpression query = new QueryExpression("mzk_faqconfiguration");
                    query.Criteria.AddCondition("mzk_servicespecific", ConditionOperator.Equal, true);
                    query.ColumnSet = new ColumnSet("mzk_faqconfigurationid", "mzk_name", "entityimage");
                    LinkEntity FAQContract = new LinkEntity("mzk_faqconfiguration", "mzk_faqconfigurationcontract", "mzk_faqconfigurationid", "mzk_faqconfiguration", JoinOperator.LeftOuter) { };
                    LinkEntity FAQContractManagement = new LinkEntity("mzk_faqconfigurationcontract", "mzk_contractmanagement", "mzk_contract", "mzk_contractmanagementid", JoinOperator.LeftOuter) { };
                    LinkEntity ContractReferral = new LinkEntity("mzk_contractmanagement", "opportunity", "mzk_contractmanagementid", "mzk_contract", JoinOperator.Inner)
                    {
                        LinkCriteria = {

                            Conditions =
                            {
                                new ConditionExpression("mzk_status",ConditionOperator.Equal,275380001),//Active
                                new ConditionExpression("mzk_status", ConditionOperator.Equal, 275380004)//Reviewed
                            },
                            FilterOperator = LogicalOperator.Or
                        },
                    };
                    LinkEntity ReferralPatient = new LinkEntity("opportunity", "contact", "parentcontactid", "contactid", JoinOperator.Inner)
                    {
                        LinkCriteria = {

                            Conditions =
                            {
                                new ConditionExpression("contactid",ConditionOperator.Equal,new Guid(patientId)),
                            }
                        },
                    };

                    FAQContract.LinkEntities.Add(FAQContractManagement);
                    FAQContractManagement.LinkEntities.Add(ContractReferral);
                    ContractReferral.LinkEntities.Add(ReferralPatient);
                    query.LinkEntities.Add(FAQContract);

                    EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                    var groupedFAQS = entitycollection.Entities.GroupBy(item => (item.GetAttributeValue<Guid>("mzk_faqconfigurationid")));
                    foreach (var faqs in groupedFAQS)
                    {
                        foreach (Entity entity in faqs)
                        {
                            FrequentlyAskedQuestions FAQ = new FrequentlyAskedQuestions();
                            if (entity.Attributes.Contains("mzk_faqconfigurationid"))
                            {
                                FAQ.FAQId = entity["mzk_faqconfigurationid"].ToString();
                            }
                            if (entity.Attributes.Contains("mzk_name"))
                            {
                                FAQ.name = entity["mzk_name"].ToString();
                            }
                            if (entity.Attributes.Contains("entityimage"))
                            {
                                byte[] imageInBytes = (entity.Attributes["entityimage"]) as byte[];
                                FAQ.image = Convert.ToBase64String(imageInBytes);
                            }
                            FAQS.Add(FAQ);
                            break;
                        }
                    }
                    return FAQS;

                }
                else
                {
                    throw new ValidationException("Patient Id missing");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
    public class QuestionAndAnswer
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
