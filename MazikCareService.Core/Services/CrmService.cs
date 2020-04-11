using MazikCareService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;
using MazikCareService.CRMRepository;
using System.ServiceModel;

namespace MazikCareService.Core.Services
{
  
    public class CrmService : ICrmService
    {

        private EntityRepository<Systemuser> _crmrep;
        private EntityRepository<Organization> _orgrep;
        public CrmService()
        {
            _crmrep = new EntityRepository<Systemuser>();
            _orgrep = new EntityRepository<Organization>();
        }


        public async Task<Models.Organization> GetOrganization(string id)
        {
            Organization org = await _orgrep.Get(id, "organizations");

            Models.Organization orgn = new Models.Organization();
            orgn.Name = org.Name;

            return orgn;
        }

        public async Task<List<Models.User>> GetUsers()
        {
            List<Models.User> users = new List<Models.User>();

            try
            {
                List<Systemuser> list = await _crmrep.GetAll("systemusers");

                foreach (Systemuser item in list)
                {
                    Models.User user = new Models.User();
                    user.Name = item.Fullname;
                    user.Title = item.Title;
                    var org = await this.GetOrganization(item.Organizationid.ToString());
                    user.Company = org.Name;
                    users.Add(user);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return users;
        }

        public async  Task<Models.User> GetUser(string id)
        {
            Systemuser user = await _crmrep.Get(id, "systemusers");
            Models.User usr = new Models.User();
            usr.Name = user.Fullname;
            usr.Title = user.Title;
            var org =  await this.GetOrganization(user.Organizationid.ToString());
            usr.Company = org.Name;

            return usr;
        }

        public async Task<Models.User> GetUserByDomain(string property, string value)
        {
            List<Models.User> users = new List<Models.User>();
            Models.User usr = new Models.User();
            List<Systemuser> list = await _crmrep.FilterbyContains("systemusers", property, value);
            Systemuser user = null;
            if (list.Count() > 0)
            {
                user = list.FirstOrDefault();
               
                usr.Name = user.Fullname;
                usr.Title = user.Title;
                var org = await this.GetOrganization(user.Organizationid.ToString());
                usr.Company = org.Name;
            }
            return usr;
        }

       
    }
}
