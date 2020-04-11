using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
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
    public class User
    {
        public string Name { get; set; }

        public string code { get; set; }
        public string firstName
        {
            get; set;
        }

        public string lastName
        {
            get; set;
        }

        public string middleName
        {
            get; set;
        }

        public string userId { get; set; }
        public string Address { get; set; }
        public long resourceRecId { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string PrimaryContact { get; set; }

        public string image { get; set; }

        public string caseId { get; set; }

        public List<Speciality> getSpecialities(string resourceId = null)
        {
            List<Speciality> listModel = new List<Speciality>();

            Speciality model;
            
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(BookableResourceCharacteristic.EntityLogicalName);

                query.Criteria.AddCondition("resource", ConditionOperator.Equal, new Guid(resourceId));

                query.ColumnSet = new ColumnSet("characteristic");

                EntityCollection entityCollection = repo.GetEntityCollection(query);

                foreach (Entity entity in entityCollection.Entities)
                {
                    BookableResourceCharacteristic bookableResource = (BookableResourceCharacteristic)entity;

                    model = new Speciality();

                    if (entity.Attributes.Contains("characteristic"))
                    {
                        model.Description = bookableResource.Characteristic.Name;
                        model.SpecialityId = bookableResource.Characteristic.Id.ToString();
                    }

                    listModel.Add(model);
                }

            return listModel;
        }

        public List<User> getUser(long resourceRecId = 0)
        {
            List<User> User = new List<User>();
            #region Users
            QueryExpression query = new QueryExpression("systemuser");
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname", "address1_composite");

            if (resourceRecId > 0)
            {
                query.Criteria.AddCondition("mzk_axresourcerefrecid", ConditionOperator.Equal, Convert.ToDecimal(resourceRecId));
            }
            #endregion
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            foreach (Entity entity in entitycollection.Entities)
            {
                User model = new User();
                model.userId = entity.Id.ToString();
                if (entity.Attributes.Contains("fullname"))
                    model.Name = entity.Attributes["fullname"].ToString();

                if (entity.Attributes.Contains("address1_composite"))
                    model.Address = entity.Attributes["address1_composite"].ToString();

                User.Add(model);
            }
            return User;
        }
        public static List<string> getPrivileges(string userId)
        {
            List<string> privilegeList = new List<string>();

            QueryExpression query = new QueryExpression(SystemUserRoles.EntityLogicalName);
            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, new Guid(userId));

            LinkEntity linkEntity = new LinkEntity(SystemUserRoles.EntityLogicalName, mzk_securityroleappprivilege.EntityLogicalName, "roleid", "mzk_securityrole", JoinOperator.LeftOuter);
            linkEntity.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_appprivilege");
            linkEntity.EntityAlias = mzk_securityroleappprivilege.EntityLogicalName;
            query.LinkEntities.Add(linkEntity);

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                if (entity.Attributes.Contains("mzk_securityroleappprivilege.mzk_appprivilege") && (entity.Attributes["mzk_securityroleappprivilege.mzk_appprivilege"] as AliasedValue) != null)
                {
                    privilegeList.Add(((EntityReference)(entity.Attributes["mzk_securityroleappprivilege.mzk_appprivilege"] as AliasedValue).Value).Name.ToString());
                }
            }
            return privilegeList;
        }

        public List<PowerBIReport> getPowerBIReports(string userId)
        {
            List<PowerBIReport> powerBIReportList = new List<PowerBIReport>();

            QueryExpression query = new QueryExpression(SystemUserRoles.EntityLogicalName);
            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, new Guid(userId));

            LinkEntity linkEntity = new LinkEntity(SystemUserRoles.EntityLogicalName, "mzk_securityrolepowerbi", "roleid", "mzk_securityrole", JoinOperator.LeftOuter);
            //linkEntity.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_appprivilege");
            linkEntity.EntityAlias = "mzk_securityrolepowerbi";

            LinkEntity linkEntityReport = new LinkEntity("mzk_securityrolepowerbi", "mzk_powerbireport", "mzk_powerbireport", "mzk_powerbireportid", JoinOperator.LeftOuter);
            linkEntityReport.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_weburl", "mzk_embedurl", "mzk_id", "mzk_name");
            linkEntityReport.EntityAlias = "mzk_powerbireport";
            linkEntity.LinkEntities.Add(linkEntityReport);
            query.LinkEntities.Add(linkEntity);

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            PowerBIReport model;
            bool firstTime = true;

            foreach (Entity entity in entitycollection.Entities)
            {
                model = new PowerBIReport();

                if (entity.Attributes.Contains("mzk_powerbireport.mzk_id") && (entity.Attributes["mzk_powerbireport.mzk_id"] as AliasedValue) != null)
                {
                    model.reportId = (((entity.Attributes["mzk_powerbireport.mzk_id"] as AliasedValue).Value).ToString());
                }

                if (entity.Attributes.Contains("mzk_powerbireport.mzk_name") && (entity.Attributes["mzk_powerbireport.mzk_name"] as AliasedValue) != null)
                {
                    model.reportName = (((entity.Attributes["mzk_powerbireport.mzk_name"] as AliasedValue).Value).ToString());
                }

                if (entity.Attributes.Contains("mzk_powerbireport.mzk_embedurl") && (entity.Attributes["mzk_powerbireport.mzk_embedurl"] as AliasedValue) != null)
                {
                    model.embeddedUrl = (((entity.Attributes["mzk_powerbireport.mzk_embedurl"] as AliasedValue).Value).ToString());
                }

                if (entity.Attributes.Contains("mzk_powerbireport.mzk_weburl") && (entity.Attributes["mzk_powerbireport.mzk_weburl"] as AliasedValue) != null)
                {
                    model.webUrl = (((entity.Attributes["mzk_powerbireport.mzk_weburl"] as AliasedValue).Value).ToString());
                }

                if (firstTime)
                {
                    model.isDefault = "True";
                    firstTime = false;
                }
                else
                {
                    model.isDefault = "";
                }
                if (model.reportId != null && powerBIReportList.FindIndex(x => x.reportId == model.reportId) < 0)
                {
                    powerBIReportList.Add(model);
                }
            }

            return powerBIReportList;
        }
        public List<User> getUsers(long specialtyRecId = 0, bool isReferral = false, string loginUserId = null, string specialtyId = null)
        {
            List<User> listModel = new List<User>();
            User model;
            
                QueryExpression query = new QueryExpression(SystemUser.EntityLogicalName);
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("systemuserid", "fullname", "address1_composite");
                query.Criteria.AddCondition("mzk_isreferral", ConditionOperator.Equal, isReferral);
                if (!string.IsNullOrEmpty(loginUserId))
                    query.Criteria.AddCondition("systemuserid", ConditionOperator.NotEqual, new Guid(loginUserId));

                LinkEntity resource = new LinkEntity("systemuser", "bookableresource", "systemuserid", "userid", JoinOperator.Inner);
                resource.Columns = new ColumnSet(true);
                resource.EntityAlias = "resource";

                LinkEntity resourceCharacterstic = new LinkEntity("bookableresource", "bookableresourcecharacteristic", "bookableresourceid", "resource", JoinOperator.Inner);
                resourceCharacterstic.Columns = new ColumnSet(true);
                resourceCharacterstic.LinkCriteria.AddCondition("characteristic", ConditionOperator.Equal, new Guid(specialtyId));
                resourceCharacterstic.EntityAlias = "resourceCharacterstic";

                query.LinkEntities.Add(resource);
                resource.LinkEntities.Add(resourceCharacterstic);

                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(query);

                foreach (Entity entity in entitycol.Entities)
                {
                    SystemUser user = (SystemUser)entity;

                    model = new User();

                    if (entitycol.Entities[0].Attributes.Contains("systemuserid"))
                        model.userId = user.SystemUserId.ToString();

                    if (entitycol.Entities[0].Attributes.Contains("fullname"))
                        model.Name = user.FullName;

                    if (entitycol.Entities[0].Attributes.Contains("address1_composite"))
                        model.Address = user.Address1_Composite;

                    listModel.Add(model);
                }
            
            return listModel.OrderBy(item => item.Name).ToList();
        }

        public static string getDescription(Guid Id, string column)
        {
            Entity entity = SoapEntityRepository.GetService().GetEntity(xrm.SystemUser.EntityLogicalName, Id, new ColumnSet(column));

            if (entity != null && entity.Attributes.Contains(column))
            {
                return entity.Attributes[column].ToString();
            }

            return "";
        }



        public async Task<User> getUserEntity(string id)
        {
            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            Entity entity = entityRepository.GetEntity("systemuser", Guid.Parse(id), new ColumnSet("systemuserid", "fullname", "entityimage"));

            User filledUser = entityToUser(entity);

            return filledUser;

        }

        public User entityToUser(Entity entity)
        {
            User user = new User();
            user.userId = entity.Id.ToString();
            user.Name = entity.Attributes["fullname"].ToString();
            user.image = Convert.ToBase64String(entity.Attributes["entityimage"] as byte[]);
            /*user.type = (entity.Attributes["type"] as OptionSetValue).Value.ToString();
            user.createdon = entity.Attributes["createdon"].ToString();
            user.modifiedon = entity.Attributes["modifiedon"].ToString();
            user.source = (entity.Attributes["source"] as OptionSetValue).Value.ToString();
            user.text = entity.Attributes["text"].ToString();
            user.subject = ((EntityReference)entity.Attributes["regardingobjectid"]).Name;
            */
            return user;
        }

        public List<User> getCareTeamUsersFromList(List<string> caseguid)
        {
            List<User> retList = new List<User>();
            try
            {
                #region Query
                QueryExpression queryUser = new QueryExpression(Incident.EntityLogicalName);
                FilterExpression filter = queryUser.Criteria.AddFilter(LogicalOperator.Or);
                foreach (string item in caseguid)
                {
                    filter.AddCondition("incidentid", ConditionOperator.Equal, new Guid(item));
                }

                LinkEntity careTeamMember = new LinkEntity(Incident.EntityLogicalName, mzk_casecareteammember.EntityLogicalName, "mzk_casecareteam", "mzk_casecareteam", JoinOperator.Inner);
                queryUser.LinkEntities.Add(careTeamMember);

                LinkEntity user = new LinkEntity(mzk_casecareteammember.EntityLogicalName, SystemUser.EntityLogicalName, "mzk_user", "systemuserid", JoinOperator.Inner);
                careTeamMember.LinkEntities.Add(user);

                user.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname");
                user.EntityAlias = "user";

                #endregion

                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(queryUser);

                foreach (Entity entity in entitycol.Entities)
                {
                    User model = new User();

                    if (entity.Attributes.Contains("user.fullname"))
                    {
                        model.Name = ((Microsoft.Xrm.Sdk.AliasedValue)(entity.Attributes["user.fullname"])).Value.ToString();
                        model.caseId = entity.Attributes["incidentid"].ToString();
                    }

                    retList.Add(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retList;
        }

        public Dictionary<string, string> getUsersImages(List<string> userIdList)
        {
            Dictionary<string, string> userIdImage = new Dictionary<string, string>();

            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression("systemuser");

            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);

            foreach (string userId in userIdList)
            {
                
                {
                    childFilter.AddCondition("systemuserid", ConditionOperator.Equal, new Guid(userId));
                }
            }

            query.ColumnSet = new ColumnSet("entityimage", "systemuserid");

            EntityCollection entityCollection = repo.GetEntityCollection(query);

            foreach (Entity entity in entityCollection.Entities)
            {
                SystemUser user = (SystemUser)entity;

                if (user.EntityImage != null && user.SystemUserId != null)
                {
                    userIdImage.Add(user.SystemUserId.ToString(), Convert.ToBase64String(user.EntityImage));
                }
            }

            return userIdImage;
        }

    }
}
