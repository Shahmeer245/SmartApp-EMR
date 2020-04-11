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
    public class Unit
    {
        public string UnitId { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }

        public List<Unit> getUnit(string Type,string Dosage)
        {
            List<Unit> Unit = new List<Unit>();
            if (!string.IsNullOrEmpty(Dosage))
            {

                #region Dosage
                //QueryExpression query = new QueryExpression("mzk_dosageform");
                //FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                //childFilter.AddCondition("mzk_dosageformid", ConditionOperator.Equal, Dosage);
                //LinkEntity UnitEntity = new LinkEntity("mzk_dosageform", "mzk_unit", "mzk_dosageformid", "mzk_doseunit", JoinOperator.Inner);
                //UnitEntity.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_unitid");
                //UnitEntity.EntityAlias = "Unit";
                //query.LinkEntities.Add(UnitEntity);

                string entity1 = "mzk_dosageform";
                string entity2 = "mzk_unit";
                string relationshipEntityName = "mzk_mzk_dosageform_mzk_unit";
                QueryExpression query = new QueryExpression(entity1);
                LinkEntity linkEntity1 = new LinkEntity(entity1, relationshipEntityName, "mzk_dosageformid", "mzk_dosageformid", JoinOperator.Inner);
                LinkEntity linkEntity2 = new LinkEntity(relationshipEntityName, entity2, "mzk_unitid", "mzk_unitid", JoinOperator.Inner);
                linkEntity2.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_unitid");
                linkEntity2.EntityAlias = "Unit";
                linkEntity1.LinkEntities.Add(linkEntity2);
                query.LinkEntities.Add(linkEntity1);
                linkEntity1.LinkCriteria = new FilterExpression();
                linkEntity1.LinkCriteria.AddCondition(new ConditionExpression("mzk_dosageformid", ConditionOperator.Equal, Dosage));

                #endregion
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                foreach (Entity entity in entitycollection.Entities)
                {
                    Unit model = new Unit();
                    model.UnitId = ((AliasedValue)entity.Attributes["Unit.mzk_unitid"]).Value.ToString();
                    model.Description = ((AliasedValue)entity.Attributes["Unit.mzk_description"]).Value.ToString();

                    Unit.Add(model);
                }
            }
            else
            {
                #region Unit
                QueryExpression query = new QueryExpression("mzk_unit");
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_unitid", "mzk_code");
                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                childFilter.AddCondition("mzk_type", ConditionOperator.Equal, Type);                

                #endregion
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                foreach (Entity entity in entitycollection.Entities)
                {
                    Unit model = new Unit();
                    model.UnitId = entity.Id.ToString();//.Attributes["mzk_ordersetupid"].ToString();
                    model.Description = entity.Attributes["mzk_description"].ToString();
                    model.Code = entity.Attributes["mzk_code"].ToString();
                    //model.Address = entity.Attributes["address1_composite"].ToString();
                    Unit.Add(model);
                }
            }
            return Unit;
        }

        public static string getUnitFieldValue(Guid Id, string column)
        {
            return SoapEntityRepository.GetService().GetEntity("mzk_unit", Id, new ColumnSet(column)).Attributes[column].ToString();
        }
    }
}
