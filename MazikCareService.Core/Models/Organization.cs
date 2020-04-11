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
   

    public class  Organization
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Logo { get; set; }
        public string address { get; set;}
        public string phone { get; set; }
        public string email { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }

        public async Task<string> getCompanyLogo()
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            string logo = string.Empty;
            QueryExpression query = new QueryExpression(WebResource.EntityLogicalName);            

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("content");

            LinkEntity entityTypeTheme = new LinkEntity(WebResource.EntityLogicalName, Theme.EntityLogicalName, "webresourceid", "logoid", JoinOperator.Inner);

            entityTypeTheme.LinkCriteria.AddCondition("isdefaulttheme", ConditionOperator.Equal, true);

            query.LinkEntities.Add(entityTypeTheme);

            EntityCollection entitycol = repo.GetEntityCollection(query);

            if (entitycol != null && entitycol.Entities != null && entitycol.Entities.Count > 0)
            {
                WebResource logoResource = (WebResource) entitycol.Entities[0];
                //logoResource.WebResourceId
                logo = logoResource.Content;
            }

            return logo;
        }
        public async Task<Organization> getOrganizationDetail()
        {
            try
            {
                Organization organization = new Organization();
                QueryExpression query = new QueryExpression(BusinessUnit.EntityLogicalName);
                query.Criteria.AddCondition("parentbusinessunitid",ConditionOperator.Null);
                query.ColumnSet = new ColumnSet("name", "address1_telephone1", "emailaddress", "address1_line1", "address1_line2", "address1_longitude", "address1_latitude", "address1_line3", "address1_city", "address1_stateorprovince", "address1_postalcode", "address1_country");
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entityCollection = entityRepository.GetEntityCollection(query);
                foreach (Entity entity in entityCollection.Entities)
                {
                    if (entity.Attributes.Contains("name"))
                        organization.Name = entity["name"].ToString();
                    if (entity.Attributes.Contains("address1_telephone1"))
                        organization.phone = entity["address1_telephone1"].ToString();
                    if (entity.Attributes.Contains("emailaddress"))
                        organization.email = entity["emailaddress"].ToString();
                    if (entity.Attributes.Contains("address1_line1"))
                        organization.address = entity["address1_line1"].ToString();
                    if (entity.Attributes.Contains("address1_line2"))
                        organization.address +=" "+entity["address1_line2"].ToString();
                    if (entity.Attributes.Contains("address1_line3"))
                        organization.address += " " + entity["address1_line3"].ToString();
                    if (entity.Attributes.Contains("address1_city"))
                        organization.address += " " + entity["address1_city"].ToString();
                    if (entity.Attributes.Contains("address1_stateorprovince"))
                        organization.address += " " + entity["address1_stateorprovince"].ToString();
                    if (entity.Attributes.Contains("address1_postalcode"))
                        organization.address += " " + entity["address1_postalcode"].ToString();
                    if (entity.Attributes.Contains("address1_country"))
                        organization.address += " " + entity["address1_country"].ToString();
                    if (entity.Attributes.Contains("address1_latitude"))
                        organization.latitude = Convert.ToDecimal(entity["address1_latitude"]);
                    if (entity.Attributes.Contains("address1_longitude"))
                        organization.longitude = Convert.ToDecimal(entity["address1_longitude"]);

                }
                return organization;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
