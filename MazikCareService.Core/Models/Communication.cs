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
    public class Communication
    {
        public string patientId { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public DateTime createdOn { get; set; }
        public string status { get; set; }

        public async Task<bool> saveCommunication(Communication post)
        {
            try
            {
                if (post != null)
                {
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    Entity entity = new Entity("mzk_communication");
                    if (!string.IsNullOrEmpty(post.patientId))
                    {
                        entity["regardingobjectid"] = new EntityReference(xrm.Contact.EntityLogicalName, new Guid(post.patientId));
                        if (!string.IsNullOrEmpty(post.subject))
                        {
                            entity["subject"] = post.subject;
                            if (!string.IsNullOrEmpty(post.description))
                                entity["description"] = post.description;

                                entityRepository.CreateEntity(entity);
                                return true;
                        }
                        else
                        {
                            throw new ValidationException("Subject field is mandatory");
                        }
                        
                    }
                    else
                    {
                        throw new ValidationException("Patient Id field is mandatory");
                    }
                }
                return false;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<List<Communication>> getCommunications(string patientId)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientId))
                {
                    List<Communication> posts = new List<Communication>();
                    QueryExpression query = new QueryExpression("mzk_communication");
                    query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, new Guid(patientId));
                    query.ColumnSet = new ColumnSet("regardingobjectid","subject", "description", "createdon", "statecode");
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                    foreach (Entity entity in entitycollection.Entities)
                    {
                        Communication post = new Communication();
                        if (entity.Attributes.Contains("regardingobjectid"))
                        {
                            post.patientId = ((EntityReference)entity["regardingobjectid"]).Id.ToString();
                        }
                        if (entity.Attributes.Contains("subject"))
                            post.subject = entity["subject"].ToString();
                        if (entity.Attributes.Contains("description"))
                            post.description = entity["description"].ToString();
                        if (entity.Attributes.Contains("createdon"))
                            post.createdOn = (DateTime)entity["createdon"];
                        if (entity.Attributes.Contains("statecode"))
                            post.status = entity.FormattedValues["statecode"].ToString();
                        posts.Add(post);
                    }

                    return posts;
                }
                else
                {
                    throw new ValidationException("Patient Id not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
