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
   public class ReferringPhysician:IReferringPhysician
    {
        public string id{
            get;
            set;
        }
        public string name {
            get;
            set;
        }
        public string address
        {
            get;
            set;
        }
        public string Category
        {
            get;
            set;
        }
        public List<ReferringPhysician> getReferringPhysician(string SpecialityId = null)
        {
            List<ReferringPhysician> ReferringPhysician = new List<ReferringPhysician>();
            #region Referring Physician Query
            QueryExpression query = new QueryExpression("mzk_referringphysician");
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_referringphysicianid", "mzk_name");         
           
            if (!string.IsNullOrEmpty(SpecialityId))
            {
                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);                
                childFilter.AddCondition("mzk_specialityid", ConditionOperator.Equal, new Guid(SpecialityId));
            }

            LinkEntity ReferralAddress = new LinkEntity("mzk_referringphysician", "contact", "mzk_contactid", "contactid", JoinOperator.LeftOuter);
            ReferralAddress.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("address1_composite");
            query.LinkEntities.Add(ReferralAddress);

            OrderExpression order = new OrderExpression();
            order.AttributeName = "mzk_name";
            order.OrderType = OrderType.Ascending;

            query.Orders.Add(order);


            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                ReferringPhysician model = new ReferringPhysician();
                model.id = entity.Id.ToString();
                if(entity.Attributes.Contains("mzk_name"))
                model.name = entity.Attributes["mzk_name"].ToString();
                if(entity.Attributes.Contains("contact1.address1_composite"))
                model.address = (entity.Attributes["contact1.address1_composite"] as AliasedValue).Value.ToString().Replace("\n"," ").Replace("\r", " ");
                model.Category = "2";
                ReferringPhysician.Add(model);
            }
            return ReferringPhysician;
        }
    }
}
