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
   public class Insurance
    {
        public string insuranceId { get; set; }
        public string insuranceName { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string insuranceType { get; set; }
        public string insurancePolicyNumber { get; set; }



     /*  public async Task<List<Insurance>> getInsurance(string InsuranceName, string InsuranceGroupId)
        {
            
            List<Insurance> Insurance = new List<Insurance>();
            if (string.IsNullOrEmpty(InsuranceGroupId))
            {
                int value = 275380002;
                QueryExpression q = new QueryExpression("account");
                q.Criteria.AddCondition("mzk_accounttype", ConditionOperator.Equal, value);
                q.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("accountid","name","telephone1","fax","address1_composite","address2_composite","address1_city","address1_stateorprovince","address1_postalcode");
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(q);
                
                foreach (Entity entity in entitycollection.Entities)
                {
                    Insurance insurance = new Insurance();
                    if (entity.Attributes.Contains("accountid"))
                    {
                        
                        insurance.insuranceId = entity["accountid"].ToString();
                    }
                    if (entity.Attributes.Contains("name"))
                    {

                        insurance.insuranceName = entity["name"].ToString();
                    }
                    if (entity.Attributes.Contains("telephone1"))
                    {

                        insurance.phone = entity["telephone1"].ToString();
                    }
                    if (entity.Attributes.Contains("fax"))
                    {

                        insurance.fax = entity["fax"].ToString();
                    }
                    if (entity.Attributes.Contains("address1_composite"))
                    {

                        insurance.address1 = entity["address1_composite"].ToString();
                    }
                    if (entity.Attributes.Contains("address2_composite"))
                    {
   
                        insurance.address2 = entity["address2_composite"].ToString();
                    }
                    if (entity.Attributes.Contains("address1_city"))
                    {

                        insurance.city = entity["address1_city"].ToString();
                    }
                    if (entity.Attributes.Contains("address1_stateorprovince"))
                    {

                        insurance.state = entity["address1_stateorprovince"].ToString();
                    }
                    if (entity.Attributes.Contains("address1_postalcode"))
                    {

                        insurance.zip = entity["address1_postalcode"].ToString();
                    }
                    Insurance.Add(insurance);
                }
                return Insurance;

            }
            else
            {
                
                QueryExpression query = new QueryExpression("mzk_payerpolicy");
                FilterExpression childfilter = query.Criteria.AddFilter(LogicalOperator.And);
                childfilter.AddCondition("mzk_payerpolicyid", ConditionOperator.Equal, new Guid(InsuranceGroupId));
                childfilter.AddCondition("mzk_name", ConditionOperator.Equal, InsuranceName);
                LinkEntity link = new LinkEntity("mzk_payerpolicy", "mzk_payerpolicygroup", "mzk_payerpolicyid", "mzk_payerpolicygroupid", JoinOperator.Inner)
                {
                    Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("accountid", "name", "telephone1", "fax", "address1_composite", "address2_composite", "address1_city", "address1_stateorprovince", "address1_postalcode"),
                    EntityAlias = "payergroup",
                };
                query.LinkEntities.Add(link);
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                
                foreach (Entity entity in entitycollection.Entities)
                {
                    Insurance insurance = new Insurance();
                    if (entity.Attributes.Contains("payergroup.accountid"))
                    {
                        insurance.insuranceId = ((AliasedValue)(entity["payergroup.accountid"])).Value.ToString();
                    }
                    if (entity.Attributes.Contains("payergroup.name"))
                    {
                        insurance.insuranceName = ((AliasedValue)(entity["payergroup.name"])).Value.ToString();
                    }
                    if (entity.Attributes.Contains("payergroup.telephone1"))
                    {
                        insurance.phone = ((AliasedValue)(entity["payergroup.telephone1"])).Value.ToString();
                    }
                    if (entity.Attributes.Contains("payergroup.fax"))
                    {
                        insurance.fax = ((AliasedValue)(entity["payergroup.fax"])).Value.ToString();
                    }
                    if (entity.Attributes.Contains("payergroup.address1_composite"))
                    {
                        insurance.address1 = ((AliasedValue)(entity["payergroup.address1_composite"])).Value.ToString();
                    }
                    if (entity.Attributes.Contains("payergroup.address2_composite"))
                    {
                        insurance.address2 = ((AliasedValue)(entity["payergroup.address2_composite"])).Value.ToString();
                    }
                    if (entity.Attributes.Contains("payergroup.address1_city"))
                    {
                        insurance.city = ((AliasedValue)(entity["payergroup.address1_city"])).Value.ToString();
                    }
                    if (entity.Attributes.Contains("payergroup.address1_stateorprovince"))
                    {
                        insurance.state = ((AliasedValue)(entity["payergroup.address1_stateorprovince"])).Value.ToString();
                    }
                    if (entity.Attributes.Contains("payergroup.address1_postalcode"))
                    {
                        insurance.zip = ((AliasedValue)(entity["payergroup.address1_postalcode"])).Value.ToString();
                    }

                    Insurance.Add(insurance);
                }
                return Insurance;
            }









            //p = await getInsurance(Phy_uid);
            //return Insurance;
        }*/

    }
}
