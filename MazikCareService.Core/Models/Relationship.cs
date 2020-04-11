using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class Relationship
    {
        public string relationshipId
        {
            get; set;
        }
        public string name
        {
            get; set;
        }

        public string type
        {
            get; set;
        }

        public string fax
        {
            get; set;
        }

        public string email
        {
            get; set;
        }

        public string mobile
        {
            get; set;
        }

        public string phone
        {
            get; set;
        }
        public string firstname
        {
            get; set;
        }
        public string lastname
        {
            get; set;
        }
        public bool carer
        {
            get; set;
        }
        public string patientId
        {
            get; set;
        }
        public string image
        {
            get; set;
        }
        public bool nextOfKin
        {
            get; set;
        }
        public async Task<bool> addRelationship(Relationship relationship)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                if (relationship.patientId != null)
                {
                    QueryExpression query = new QueryExpression(xrm.Contact.EntityLogicalName);
                    FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                    if (!string.IsNullOrEmpty(relationship.firstname))
                    {
                        childFilter.AddCondition("firstname", ConditionOperator.Equal, relationship.firstname);
                    }
                    if (!string.IsNullOrEmpty(relationship.lastname))
                    {
                        childFilter.AddCondition("lastname", ConditionOperator.Equal, relationship.lastname);
                    }
                    if (!string.IsNullOrEmpty(relationship.mobile))
                    {
                        childFilter.AddCondition("mobilephone", ConditionOperator.Equal, relationship.mobile);
                    }
                    if (!string.IsNullOrEmpty(relationship.email))
                    {
                        childFilter.AddCondition("emailaddress1", ConditionOperator.Equal, relationship.email);
                    }
                    //childFilter.AddCondition("mzk_carer", ConditionOperator.Equal, relationship.carer);
                    //childFilter.AddCondition("mzk_nextofkin", ConditionOperator.Equal, relationship.nextOfKin);

                    EntityCollection entityCollection = repo.GetEntityCollection(query);
                    if (entityCollection.Entities.Count > 0)
                    {
                        Entity relation = new Entity(xrm.mzk_relationship.EntityLogicalName);

                        relation["mzk_customer"] = new EntityReference(xrm.Contact.EntityLogicalName, new Guid(relationship.patientId));
                        if (!string.IsNullOrEmpty(relationship.type))
                            relation["mzk_role"] = new EntityReference(xrm.mzk_masterdata.EntityLogicalName, new Guid(relationship.type));

                        if (entityCollection.Entities[0].Attributes.Contains("contactid"))
                            relation["mzk_connectedto"] = new EntityReference(xrm.Contact.EntityLogicalName, new Guid(entityCollection.Entities[0]["contactid"].ToString()));

                        relation["mzk_carer"] = relationship.carer;
                        relation["mzk_nextofkin"] = relationship.nextOfKin;
                        repo.CreateEntity(relation);
                        return true;
                    }
                    else
                    {
                        Entity contact = new Entity(xrm.Contact.EntityLogicalName);
                        if (!string.IsNullOrEmpty(relationship.firstname))
                            contact["firstname"] = relationship.firstname;
                        if (!string.IsNullOrEmpty(relationship.lastname))
                            contact["lastname"] = relationship.lastname;
                        if (!string.IsNullOrEmpty(relationship.mobile))
                            contact["mobilephone"] = relationship.mobile;
                        if (!string.IsNullOrEmpty(relationship.email))
                            contact["emailaddress1"] = relationship.email;
                        contact["mzk_contacttype"] = new OptionSetValue(275380000);

                        Guid contactid = new Guid();
                        contactid = repo.CreateEntity(contact);

                        if (contactid != null)
                        {
                            Entity relation = new Entity(xrm.mzk_relationship.EntityLogicalName);
                            relation["mzk_customer"] = new EntityReference(xrm.Contact.EntityLogicalName, new Guid(relationship.patientId));
                            if (!string.IsNullOrEmpty(relationship.type))
                                relation["mzk_role"] = new EntityReference(xrm.mzk_masterdata.EntityLogicalName, new Guid(relationship.type));
                            relation["mzk_connectedto"] = new EntityReference(xrm.Contact.EntityLogicalName, contactid);
                            relation["mzk_carer"] = relationship.carer;
                            relation["mzk_nextofkin"] = relationship.nextOfKin;
                            repo.CreateEntity(relation);
                            return true;
                        }
                        return false;
                    }
                }
                else
                {
                    throw new ValidationException("Patient Id missing");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> updateRelationship(string relationshipId, bool nextofKin , bool carer)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                if (!string.IsNullOrEmpty(relationshipId))
                {
                    Entity relationship = new Entity("mzk_relationship");
                    relationship.Id = new Guid(relationshipId);

                    relationship["mzk_carer"] = carer;
                    relationship["mzk_nextofkin"] = nextofKin;
                    repo.UpdateEntity(relationship);
                    return true;

                }
                else
                {
                    throw new ValidationException("Relationship Id not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Relationship>> getCarerRelations(string carerId)
        {
            try
            {
                if (!string.IsNullOrEmpty(carerId))
                {
                    List<Relationship> relations = new List<Relationship>();
                    QueryExpression query = new QueryExpression(xrm.mzk_relationship.EntityLogicalName);
                    query.Criteria.AddCondition("mzk_connectedto", ConditionOperator.Equal, new Guid(carerId));
                    LinkEntity RelationshipContact = new LinkEntity(xrm.mzk_relationship.EntityLogicalName, xrm.Contact.EntityLogicalName, "mzk_customer", "contactid", JoinOperator.LeftOuter)
                    {
                        Columns = new ColumnSet("contactid", "fullname", "entityimage"),
                        EntityAlias = "Contact"
                    };
                    query.LinkEntities.Add(RelationshipContact);
                    SoapEntityRepository repo = SoapEntityRepository.GetService();
                    EntityCollection entityCollection = repo.GetEntityCollection(query);
                    foreach(Entity entity in entityCollection.Entities)
                    {
                        Relationship relation = new Relationship();
                        if (entity.Attributes.Contains("Contact.contactid"))
                        {
                            relation.patientId = entity.GetAttributeValue<AliasedValue>("Contact.contactid").Value.ToString();
                        }
                        if (entity.Attributes.Contains("Contact.fullname"))
                        {
                            relation.name = entity.GetAttributeValue<AliasedValue>("Contact.fullname").Value.ToString();
                        }
                        if (entity.Attributes.Contains("Contact.entityimage"))
                        {
                            byte[] imageInBytes = (entity.Attributes["Contact.entityimage"] as AliasedValue).Value as byte[];
                            relation.image = Convert.ToBase64String(imageInBytes);
                        }
                        relations.Add(relation);
                    }
                    return relations;
                }
                else
                {
                    throw new ValidationException("Carer Id missing");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AppHeader> useHeader(string patientId)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientId))
                {
                    SoapEntityRepository repo = SoapEntityRepository.GetService();
                    AppHeader Header = new AppHeader();
                    List<int> headers = new List<int>(); 
                    Configuration configuration = new Configuration();
                    configuration = configuration.getConfiguration();
                    if (configuration.patientAppHeader1Value != 0)
                    {
                        headers.Add(configuration.patientAppHeader1Value);
                    }
                    if (configuration.patientAppHeader2Value != 0)
                    {
                        headers.Add(configuration.patientAppHeader2Value);
                    }
                    if (configuration.patientAppHeader3Value != 0)
                    {
                        headers.Add(configuration.patientAppHeader3Value);
                    }
                    List<string> parameters = new List<string>();
                    int i = 0;
                    foreach (int header in headers)
                    {
                        parameters.Add(mapFields(header));
                        
                    }
                    Entity contact = repo.GetEntity(xrm.Contact.EntityLogicalName, new Guid(patientId), new ColumnSet(parameters.ToArray()));
                    if (contact != null)
                    {
                        Header.field1Label = configuration.patientAppHeader1Label;
                        Header.field2Label = configuration.patientAppHeader2Label;
                        Header.field3Label = configuration.patientAppHeader3Label;
                        string Value = null;
                        if (parameters.Count > 0)
                        {
                            if (contact.Attributes.Contains(parameters[0]))
                            {
                                Value = getValue(contact,parameters[0]);
                                if (configuration.patientAppHeader1Label != null)
                                {
                                    Header.field1Value = Value;
                                }
                                else if (configuration.patientAppHeader2Label != null)
                                {
                                    Header.field2Value = Value;
                                }
                                else if (configuration.patientAppHeader3Label != null)
                                {
                                    Header.field3Value = Value;
                                }
                            }
                        }
                        if (parameters.Count > 1)
                        {
                            if (contact.Attributes.Contains(parameters[1]))
                            {
                                Value = getValue(contact, parameters[1]);
                                if (configuration.patientAppHeader2Label != null)
                                {
                                    if (Header.field2Value == null)
                                    {
                                        Header.field2Value = Value;
                                    }
                                    else
                                    {
                                        Header.field3Value = Value;
                                    }
                                }
                                else if (configuration.patientAppHeader3Label != null)
                                {
                                    Header.field3Value = Value;
                                }
                            }
                        }
                        if (parameters.Count > 2)
                        {
                            if (contact.Attributes.Contains(parameters[2]))
                            {
                                Value = getValue(contact, parameters[2]);
                                if (Header.field3Value == null)
                                {
                                    Header.field3Value = Value;
                                }
                            }
                        }

                    }
                    return Header;
                    
                }
                else
                {
                    throw new ValidationException("Patient Id is missing");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string mapFields(int fieldValue)
        {
            try
            {
                return Enum.GetName(typeof(HeaderMapping), fieldValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string getValue(Entity obj , string attr)
        {
            string Value = null;
            var type = obj[attr].GetType().Name;
            if (type != null)
            {
                if (type.Equals("EntityReference"))
                {
                    Value = (obj[attr] as EntityReference).Name.ToString();
                }
                else if (type.Equals("OptionSetValue"))
                {
                    Value = obj.FormattedValues[attr];
                }
                else if (type.Equals("DateTime"))
                {
                    DateTime date = (DateTime)obj[attr];
                    Value = date.ToShortDateString();
                }
                else
                {
                    Value = obj[attr].ToString();
                }
            }
            return Value;
        }


    }
    public class AppHeader
    {
        public string field1Label { get; set; }
        public string field1Value { get; set; }
        public string field2Label { get; set; }
        public string field2Value { get; set; }
        public string field3Label { get; set; }
        public string field3Value { get; set; }

        
    }

    public enum HeaderMapping
    {
        mzk_title =  275380000,
        firstname =  275380001,
        lastname =   275380002,
        gendercode = 275380003,
        birthdate =  275380004,
        mzk_agecalculated = 275380005,
        familystatuscode = 275380006,
        mzk_nationality = 275380007,
        mzk_ethnicity = 275380008,
        mzk_religion = 275380009,
        mzk_primaryidentificationnumber = 275380010

    }
}
