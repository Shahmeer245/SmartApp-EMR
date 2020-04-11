using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class OrderSetup : IOrderSetup
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

       public List<OrderSetup> getOrderSetup(string type,string Dosage=null)
        {
            List<OrderSetup> OrderSetup = new List<OrderSetup>();
            if (!string.IsNullOrEmpty(Dosage) && (type =="1"|| type =="2"))
            {
                //string entity1 = "mzk_dosageform";
                //string entity2 = "mzk_ordersetup";
                //string relationshipEntityName = "mzk_mzk_dosageform_mzk_frequency";
                //QueryExpression query = new QueryExpression(entity1);
                //LinkEntity linkEntity1 = new LinkEntity(entity1, relationshipEntityName, "mzk_dosageformid", "mzk_dosageformid", JoinOperator.Inner);
                //LinkEntity linkEntity2 = new LinkEntity(relationshipEntityName, entity2, "mzk_ordersetupid", "mzk_ordersetupid", JoinOperator.Inner);
                //linkEntity2.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_ordersetupid", "mzk_code", "mzk_description");
                //linkEntity2.EntityAlias = "OrderSetup";
                //linkEntity1.LinkEntities.Add(linkEntity2);
                //query.LinkEntities.Add(linkEntity1);              
                //linkEntity2.LinkCriteria = new FilterExpression();
                //linkEntity1.LinkCriteria.AddCondition(new ConditionExpression("mzk_dosageformid", ConditionOperator.Equal, Dosage));
                //#region Patient Order Setup
                QueryExpression query = new QueryExpression("mzk_dosageform");
                //FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                //childFilter.AddCondition("mzk_dosageformid", ConditionOperator.Equal, Dosage);
                //LinkEntity OrderSetupEntity = null;

                if (type == "1")
                {
                    string entity1 = "mzk_dosageform";
                    string entity2 = "mzk_ordersetup";
                    string relationshipEntityName = "mzk_mzk_dosageform_mzk_frequency";
                   // QueryExpression query = new QueryExpression(entity1);
                    LinkEntity linkEntity1 = new LinkEntity(entity1, relationshipEntityName, "mzk_dosageformid", "mzk_dosageformid", JoinOperator.Inner);
                    LinkEntity linkEntity2 = new LinkEntity(relationshipEntityName, entity2, "mzk_ordersetupid", "mzk_ordersetupid", JoinOperator.Inner);
                    linkEntity2.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_ordersetupid", "mzk_code", "mzk_description");
                    linkEntity2.EntityAlias = "OrderSetup";
                    linkEntity1.LinkEntities.Add(linkEntity2);
                    query.LinkEntities.Add(linkEntity1);
                    linkEntity2.LinkCriteria = new FilterExpression();
                    linkEntity1.LinkCriteria.AddCondition(new ConditionExpression("mzk_dosageformid", ConditionOperator.Equal, Dosage));
                }
                if (type == "2")
                {
                    string entity1 = "mzk_dosageform";
                    string entity2 = "mzk_ordersetup";
                    string relationshipEntityName = "mzk_mzk_dosageform_mzk_route";
                   // QueryExpression query = new QueryExpression(entity1);
                    LinkEntity linkEntity1 = new LinkEntity(entity1, relationshipEntityName, "mzk_dosageformid", "mzk_dosageformid", JoinOperator.Inner);
                    LinkEntity linkEntity2 = new LinkEntity(relationshipEntityName, entity2, "mzk_ordersetupid", "mzk_ordersetupid", JoinOperator.Inner);
                    linkEntity2.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_ordersetupid", "mzk_code", "mzk_description");
                    linkEntity2.EntityAlias = "OrderSetup";
                    linkEntity1.LinkEntities.Add(linkEntity2);
                    query.LinkEntities.Add(linkEntity1);
                    linkEntity2.LinkCriteria = new FilterExpression();
                    linkEntity1.LinkCriteria.AddCondition(new ConditionExpression("mzk_dosageformid", ConditionOperator.Equal, Dosage));
                }

                //#endregion
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                foreach (Entity entity in entitycollection.Entities)
                {
                    OrderSetup model = new OrderSetup();
                    model.Code = ((AliasedValue)entity.Attributes["OrderSetup.mzk_ordersetupid"]).Value.ToString();// entity.Id.ToString();
                    if (entity.Attributes.Contains("OrderSetup.mzk_description"))
                        model.Description = ((AliasedValue)entity.Attributes["OrderSetup.mzk_description"]).Value.ToString();
                    OrderSetup.Add(model);
                }
            }
            else
            {

                #region Patient Order Setup
                QueryExpression query = new QueryExpression("mzk_ordersetup");
                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                childFilter.AddCondition("mzk_type", ConditionOperator.Equal, type);
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_ordersetupid", "mzk_code", "mzk_description");
                #endregion
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                foreach (Entity entity in entitycollection.Entities)
                {
                    OrderSetup model = new OrderSetup();
                    model.Code = entity.Id.ToString();
                    if (entity.Attributes.Contains("mzk_description"))
                        model.Description = entity.Attributes["mzk_description"].ToString();
                    OrderSetup.Add(model);
                }
            }
            return OrderSetup;
        }

       public string getDescription(Guid Id, string column)
       {
            Entity entity = SoapEntityRepository.GetService().GetEntity("mzk_ordersetup", Id, new ColumnSet(column));

            if(entity != null && entity.Attributes.Contains(column))
            {
                return entity.Attributes[column].ToString();
            }

            return "";
       }
    }
}
