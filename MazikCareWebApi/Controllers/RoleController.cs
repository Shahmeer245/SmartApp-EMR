using MazikCareService.Core;
using MazikCareService.Core.Entities;
using MazikCareService.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace MazikCareService.Controllers
{
    public class RoleController : ApiController
    {
        
        private EntityRepository<Role> _rep;
        public RoleController()
        {
           _rep = new EntityRepository<Role>();
        }

        [HttpPost]
//        {"name":"RolefromAPI",
//"organizationid":"ee23dc43-0c4f-413f-af31-81e6b5c9c997",
//"_businessunitid_value":"5fbfc7e6-bafd-e511-93fc-d4bed99c484a"}
        public Task<bool> AddRole([FromBody]Role entity)
        {
            Task<bool> isadded = _rep.Add(entity, "roles");
            return isadded;
        }

        // GET: api/GetRole/39d1c7e6-bafd-e511-93fc-d4bed99c484a
        [HttpGet]
        public Task<Role> GetRole(string id)
        {

            Task<Role> role = _rep.Get(id, "roles");
            return role;
        }

        // GET: api/GetRoles
        [HttpGet]
        public Task<List<Role>> GetRoles()
        {
            Task<List<Role>> list = _rep.GetAll("roles");
            return list;
        }

        


    }
}
