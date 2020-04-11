using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Models.HL7;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class Address
    {
        public string addressType
        {
            get; set;
        }

        public string city
        {
            get; set;
        }

        public string country
        {
            get; set;
        }

        public string zipCode
        {
            get; set;
        }

        public string street
        {
            get; set;
        }

        public string state
        {
            get; set;
        }

        public string address
        {
            get; set;
        }

        public string name
        {
            get; set;
        }
        public int addressTypeValue { get; set; }
        public string contactId { get; set; }
        public string addressId { get; set; }



        public async Task<bool> addAddress(Address address)
        {
            try
            {
                QueryExpression query = new QueryExpression(xrm.Contact.EntityLogicalName);
                query.Criteria.AddCondition("contactid", ConditionOperator.Equal, new Guid(address.contactId));
                query.ColumnSet = new ColumnSet(true);
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entityCollection = entityRepository.GetEntityCollection(query);
                foreach (Entity entity in entityCollection.Entities)
                {
                    if (!entity.Attributes.Contains("address1_composite"))
                    {
                        if (!string.IsNullOrEmpty(address.addressTypeValue.ToString()))
                            entity["address1_addresstypecode"] = new OptionSetValue(address.addressTypeValue);
                        if (!string.IsNullOrEmpty(address.city))
                            entity["address1_city"] = address.city;
                        if (!string.IsNullOrEmpty(address.country))
                            entity["address1_country"] = address.country;
                        if (!string.IsNullOrEmpty(address.zipCode))
                            entity["address1_postalcode"] = address.zipCode;
                        if (!string.IsNullOrEmpty(address.street))
                            entity["address1_line1"] = address.street;
                        if (!string.IsNullOrEmpty(address.state))
                            entity["address1_stateorprovince"] = address.state;
                        if (!string.IsNullOrEmpty(address.name))
                            entity["address1_name"] = address.name;

                        entityRepository.UpdateEntity(entity);
                        //return true;


                    }
                    else if (!entity.Attributes.Contains("address2_composite"))
                    {
                        if (!string.IsNullOrEmpty(address.addressTypeValue.ToString()))
                            entity["address2_addresstypecode"] = new OptionSetValue(address.addressTypeValue);
                        if (!string.IsNullOrEmpty(address.city))
                            entity["address2_city"] = address.city;
                        if (!string.IsNullOrEmpty(address.country))
                            entity["address2_country"] = address.country;
                        if (!string.IsNullOrEmpty(address.zipCode))
                            entity["address2_postalcode"] = address.zipCode;
                        if (!string.IsNullOrEmpty(address.street))
                            entity["address2_line1"] = address.street;
                        if (!string.IsNullOrEmpty(address.state))
                            entity["address2_stateorprovince"] = address.state;
                        if (!string.IsNullOrEmpty(address.name))
                            entity["address2_name"] = address.name;

                        entityRepository.UpdateEntity(entity);
                        //return true;
                    }
                    else if (!entity.Attributes.Contains("address3_composite"))
                    {
                        if (!string.IsNullOrEmpty(address.addressTypeValue.ToString()))
                            entity["address3_addresstypecode"] = new OptionSetValue(address.addressTypeValue);
                        if (!string.IsNullOrEmpty(address.city))
                            entity["address3_city"] = address.city;
                        if (!string.IsNullOrEmpty(address.country))
                            entity["address3_country"] = address.country;
                        if (!string.IsNullOrEmpty(address.zipCode))
                            entity["address3_postalcode"] = address.zipCode;
                        if (!string.IsNullOrEmpty(address.street))
                            entity["address3_line1"] = address.street;
                        if (!string.IsNullOrEmpty(address.state))
                            entity["address3_stateorprovince"] = address.state;
                        if (!string.IsNullOrEmpty(address.name))
                            entity["address3_name"] = address.name;

                        entityRepository.UpdateEntity(entity);
                       // return true;

                    }
                    else
                    {
                        Entity customerAddress = new Entity(xrm.CustomerAddress.EntityLogicalName);
                        customerAddress["parentid"] = new EntityReference(xrm.Contact.EntityLogicalName, new Guid(address.contactId));
                        if (!string.IsNullOrEmpty(address.addressTypeValue.ToString()))
                            customerAddress["addresstypecode"] = new OptionSetValue(address.addressTypeValue);
                        if (!string.IsNullOrEmpty(address.city))
                            customerAddress["city"] = address.city;
                        if (!string.IsNullOrEmpty(address.country))
                            customerAddress["country"] = address.country;
                        if (!string.IsNullOrEmpty(address.zipCode))
                            customerAddress["postalcode"] = address.zipCode;
                        if (!string.IsNullOrEmpty(address.street))
                            customerAddress["line1"] = address.street;
                        if (!string.IsNullOrEmpty(address.state))
                            customerAddress["stateorprovince"] = address.state;
                        if (!string.IsNullOrEmpty(address.name))
                            customerAddress["name"] = address.name;

                        entityRepository.CreateEntity(customerAddress);
                        //return true;
                    }

                }

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
                //return false;

            }
        }

        public async Task<bool> updateAddress(Address address)
        {
            try
            {
                if (!string.IsNullOrEmpty(address.addressId))
                {
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    Entity customerAddress = new Entity(xrm.CustomerAddress.EntityLogicalName);

                    customerAddress.Id = new Guid(address.addressId);
                    if (!string.IsNullOrEmpty(address.addressTypeValue.ToString()))
                        customerAddress["addresstypecode"] = new OptionSetValue(address.addressTypeValue);
                    if (!string.IsNullOrEmpty(address.city))
                        customerAddress["city"] = address.city;
                    if (!string.IsNullOrEmpty(address.country))
                        customerAddress["country"] = address.country;
                    if (!string.IsNullOrEmpty(address.zipCode))
                        customerAddress["postalcode"] = address.zipCode;
                    if (!string.IsNullOrEmpty(address.street))
                        customerAddress["line1"] = address.street;
                    if (!string.IsNullOrEmpty(address.state))
                        customerAddress["stateorprovince"] = address.state;
                    if (!string.IsNullOrEmpty(address.name))
                        customerAddress["name"] = address.name;

                    entityRepository.UpdateEntity(customerAddress);

                    return true;
                }
                else
                {
                    throw new ValidationException("Address Id not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
