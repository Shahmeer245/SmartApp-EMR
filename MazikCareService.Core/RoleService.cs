using MazikCareService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazikCareService.Core.Models;
using System.ServiceModel;

namespace MazikCareService.Core
{
    public class RoleService : IRoleService
    {
      
        public  Task<List<Role>> GetRoles()
        {
            RoleRepository _rep = new RoleRepository();
            return   _rep.GetRoles();
        }
    }
}
