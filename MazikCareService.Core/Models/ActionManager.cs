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
    public class ActionManager
    {
        public string name
        {
            get; set;
        }

        public string Id
        {
            get; set;
        }

        public string StatusId
        {
            get; set;
        }
      
        public  bool isFlip
        {
            get; set;
        }

        public int FlipType
        {
            get; set;
        }

        public List<ActionManager> getList()
        {
            List<ActionManager> list = new List<ActionManager>();

            QueryExpression query = new QueryExpression(xrm.mzk_actionmanager.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (xrm.mzk_actionmanager entity in entitycollection.Entities)
            {
                ActionManager model = new ActionManager();
                model.Id = entity.Id.ToString();
                model.name = entity.mzk_actionname;
                list.Add(model);
            }
            return list;
        }
    }
}
