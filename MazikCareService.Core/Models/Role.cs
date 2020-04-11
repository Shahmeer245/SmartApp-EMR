using MazikCareService.CRMRepository;
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
   public class Role
    {
        public Guid? roleid { get; set; }
        public string name { get; set; }
        public Guid? organizationid { get; set; }
        public Guid _businessunitid_value { get; set; }

        public List<Role> getRoles(string userId)
        {
            List<Role> role = new List<Role>();
            #region Role
            QueryExpression query = new QueryExpression(SystemUserRoles.EntityLogicalName);
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, new Guid(userId));
            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            foreach (SystemUserRoles entity in entitycollection.Entities)
            {
                Role model = new Role();

                model.roleid = entity.RoleId;

                role.Add(model);
            }
            return role;
        }
    }
}
