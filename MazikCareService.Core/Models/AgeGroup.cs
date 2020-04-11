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
    public class AgeGroup
    {
        public string Description { get; set; }

        public string Id { get; set; }

        public List<AgeGroup> getList()
        {
            List<AgeGroup> list = new List<AgeGroup>();
            
            QueryExpression query = new QueryExpression(xrm.mzk_agevalidation.EntityLogicalName);
            
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
           
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                AgeGroup model = new AgeGroup();
                model.Id = entity.Id.ToString();
                model.Description = entity.Attributes["mzk_name"].ToString();
                list.Add(model);
            }
            return list;
        }
    }
}
