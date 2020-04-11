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
   public class EncounterSetup: IEncounterSetup
    {

        public string Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public List<EncounterSetup> getEncounterSetup(string type)
        {
            List<EncounterSetup> EncounterSetup = new List<EncounterSetup>();
            #region Patient Encounter Setup
            QueryExpression query = new QueryExpression("mzk_encountersetup");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            childFilter.AddCondition("mzk_type", ConditionOperator.Equal, type);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet( "mzk_code", "mzk_description");
            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                EncounterSetup model = new EncounterSetup();
                model.Code = entity.Id.ToString();//.Attributes["mzk_ordersetupid"].ToString();
                model.Description = entity.Attributes["mzk_description"].ToString();
                EncounterSetup.Add(model);
            }
            return EncounterSetup;
        }
    }
}
