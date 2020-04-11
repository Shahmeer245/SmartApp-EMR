using Helper;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MazikCareService.Core.Models
{
    public class Mentions
    {
        public string id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string type { get; set; }

        public List<Mentions> getMentions(string name)
        {
            List<Mentions> mentionsList = new List<Mentions>();

            QueryExpression query = new QueryExpression("systemuser");
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname", "systemuserid");
            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");
            if (!string.IsNullOrEmpty(name.ToLower()))
            {
                ConditionExpression conditionLikeFullName = new ConditionExpression();
                conditionLikeFullName.AttributeName = "fullname";
                conditionLikeFullName.Operator = ConditionOperator.Like;
                conditionLikeFullName.Values.Add("%" + name + "%");
                query.Criteria.AddCondition(conditionLikeFullName);

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                foreach (Entity entity in entitycollection.Entities)
                {
                    Mentions model = new Mentions();
                    //model.userId = entity.Id.ToString();
                    if (entity.Attributes.Contains("fullname"))
                        model.name = entity.Attributes["fullname"].ToString();

                    if (entity.Attributes.Contains("systemuserid"))
                        model.id = entity.Attributes["systemuserid"].ToString();
                    
                    model.type = xrm.SystemUser.EntityTypeCode.ToString();
                    mentionsList.Add(model);
                }

            }

            return mentionsList;
        }
    }

    
}
