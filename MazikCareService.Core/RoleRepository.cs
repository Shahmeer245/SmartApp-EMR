using MazikCareService.CRMRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;

namespace MazikCareService.Core
{
    public class RoleRepository
    {
        public async Task<List<Models.Role>> GetRoles()
        {
            EntityRepository<Role> _rep = new EntityRepository<Role>();
            List<Role> roles = await _rep.GetAll("roles");
            List<Models.Role> mrole = new List<Models.Role>();
            foreach (Role role in roles)
            {
                Models.Role rolee = new Models.Role();
                rolee.roleid = role.Roleid;
                rolee.name = role.Name;
                rolee.organizationid = role.Organizationid;
                mrole.Add(rolee);
            }

            return mrole;
           
        }

    }
}
