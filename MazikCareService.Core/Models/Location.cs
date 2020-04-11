using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;
using MazikLogger;
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
    public class Location
    {
        public string locationId { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        
        //public List<Physician> physicians { get; set; }

        public async Task<List<Location>> GetLocations(string practiceId)
        {

            List<Location> Locations = new List<Location>();
            QueryExpression query = new QueryExpression(xrm.Account.EntityLogicalName);
            query.Criteria.AddCondition("accountid", ConditionOperator.Equal, new Guid(practiceId));
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("accountid","name","address1_composite","address2_composite","address1_city", "address1_stateorprovince", "address1_postalcode");
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            foreach (Entity entity in entitycollection.Entities)
            {
                Location model = new Location();

                if (entity.Attributes.Contains("accountid"))
                    model.locationId = entity["accountid"].ToString();
                if (entity.Attributes.Contains("name"))
                    model.name = entity["name"].ToString();
                if (entity.Attributes.Contains("address1_composite"))
                    model.address1 = entity["address1_composite"].ToString();
                if (entity.Attributes.Contains("address2_composite"))
                    model.address2 = entity["address2_composite"].ToString();
                if (entity.Attributes.Contains("address1_city"))
                    model.city = entity["address1_city"].ToString();
                if (entity.Attributes.Contains("address1_stateorprovince"))
                    model.state = entity["address1_stateorprovince"].ToString();
                if (entity.Attributes.Contains("address1_postalcode"))
                    model.zip = entity["address1_postalcode"].ToString();

                Locations.Add(model);
            }
               
           
            return Locations;
        }
    }
}
