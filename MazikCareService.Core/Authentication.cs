using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MazikCareService.Core.Models;
using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk.Query;
using xrm;
using Microsoft.Xrm.Sdk;
using Helper;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using System.Web;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikLogger;

namespace MazikCareService.Core
{
    public static class Authentication
    {
        public static async Task<LoginModelOutput> Authenticate(string username, string password, string domain, bool fromToken = false)
        {
            LoginModelOutput output = new LoginModelOutput();

            try
            {
                HttpContext.Current.Items["username"] = username;
                HttpContext.Current.Items["password"] = password;
                HttpContext.Current.Items["domain"] = domain;

                SoapEntityRepository repo = SoapEntityRepository.GetService();

                string sysUserId = string.Empty;
                try
                {
                    sysUserId = repo.GetUserId();
                }
                catch (Exception ex)
                {
                    throw new AuthenticationException("Username/Password is incorrect");
                }

                if (sysUserId != null && sysUserId != string.Empty)
                {
                    if (fromToken)
                    {
                        output.success = true;
                        output.userId = sysUserId;
                        output.userName = username;
                    }
                    else
                    {
                        string externalIP = Helper.AppSettings.GetByKey("APPPublicIP");

                        if (externalIP == Authentication.GetUserIp())                        
                        {  
                            QueryExpression query = new QueryExpression(SystemUser.EntityLogicalName);

                            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname", "mzk_axresourcerefrecid");

                            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, new Guid(sysUserId));

                            EntityCollection entitycollection = repo.GetEntityCollection(query);

                            Entity entity = entitycollection[0];

                            SystemUser user = (SystemUser)entity;

                            long resourceRecId = user.mzk_AXResourceRefRecId.HasValue ? Convert.ToInt64(user.mzk_AXResourceRefRecId.Value) : 0;

                            if (resourceRecId == 0)
                            {
                                throw new ValidationException("Resource Id not set for the user. Please contact system administrator");
                            }

                            //ResourceRepository resourceRepo = new ResourceRepository();
                            //HMResourceLoginSMSCodeContract contract = resourceRepo.generateResourceSMSCode(resourceRecId, 0);

                            //if (contract == null)
                            //{
                            //    throw new ValidationException("Error generating SMS code. Please try again");
                            //}

                            //if (contract.parmSuccess)
                            //{
                            //    output.success = true;
                            //    output.smsCodeId = contract.parmResourceLoginSMSCodeRecId;                                
                            //    output.userId = sysUserId;
                            //}
                            //else
                            //{
                            //    throw new ValidationException(contract.parmErrorMessage);
                            //}
                        }
                        else
                        {
                            QueryExpression query = new QueryExpression(SystemUser.EntityLogicalName);

                            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname", "mzk_axresourcerefrecid", "entityimage");

                            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, new Guid(sysUserId));

                            LinkEntity position = new LinkEntity(SystemUser.EntityLogicalName, xrm.Position.EntityLogicalName, "positionid", "positionid", JoinOperator.LeftOuter);
                            position.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("name");

                            LinkEntity businessUnit = new LinkEntity(SystemUser.EntityLogicalName, BusinessUnit.EntityLogicalName, "businessunitid", "businessunitid", JoinOperator.Inner);
                            businessUnit.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("name");

                            LinkEntity bookableResource = new LinkEntity(SystemUser.EntityLogicalName, BookableResource.EntityLogicalName, "systemuserid", "userid", JoinOperator.LeftOuter);
                            bookableResource.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                            bookableResource.EntityAlias = "bookableresource";
                            query.LinkEntities.Add(bookableResource);

                            query.LinkEntities.Add(position);
                            query.LinkEntities.Add(businessUnit);

                            EntityCollection entitycollection = repo.GetEntityCollection(query);

                            Entity entity = entitycollection[0];

                            SystemUser user = (SystemUser)entity;

                            output.fullName = user.FullName;

                            
                            if (entity.Attributes.Contains("bookableresource.bookableresourceid"))
                            {
                                output.resourceId = (entity.Attributes["bookableresource.bookableresourceid"] as AliasedValue).Value.ToString();

                            }
                            //else
                            //{
                            //    throw new ValidationException("User as a Resource not configured");
                            //}

                            if (entity.Attributes.Contains("position1.name"))
                                output.designation = (entity.Attributes["position1.name"] as AliasedValue).Value.ToString();

                            if (entity.Attributes.Contains("businessunit2.name"))
                                output.organization = (entity.Attributes["businessunit2.name"] as AliasedValue).Value.ToString();

                            output.success = true;
                            output.userId = sysUserId;
                            output.userName = username;

                            if (user.EntityImage != null)
                            {
                                output.image = Convert.ToBase64String(user.EntityImage);
                            }

                            output.privileges = User.getPrivileges(sysUserId);
                        }
                    }
                }
                else
                {
                    output.success = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }   

            return output;
        }

        public static async Task<LoginModelOutput> AuthenticateUser(string username, string password, string domain)
        {
            LoginModelOutput output = new LoginModelOutput();

            try
            {
                HttpContext.Current.Items["username"] = username;
                HttpContext.Current.Items["password"] = password;
                HttpContext.Current.Items["domain"] = domain;

                SoapEntityRepository repo = SoapEntityRepository.GetService();

                string sysUserId = string.Empty;
                try
                {
                    sysUserId = repo.GetUserId();
                }
                catch (Exception ex)
                {
                    throw new AuthenticationException("Username/Password is incorrect");
                }

                if (sysUserId != null && sysUserId != string.Empty)
                {
                    QueryExpression query = new QueryExpression(SystemUser.EntityLogicalName);

                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                    query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, new Guid(sysUserId));

                    EntityCollection entitycollection = repo.GetEntityCollection(query);

                    Entity entity = entitycollection[0];

                    SystemUser user = (SystemUser)entity;

                    output.fullName = user.FullName;
                    output.success = true;
                    output.userId = sysUserId;
                    output.userName = username;
                }
                else
                {
                    output.success = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return output;
        }


        public static async Task<LoginModelOutput> authenticateSMSCode(string sysUserId, long smsCodeId, string smsCode)
        {
            LoginModelOutput output = new LoginModelOutput();

            //try
            //{
            //    ResourceRepository resourceRepo = new ResourceRepository();
            //    HMResourceLoginSMSCodeContract contract = resourceRepo.verifyResourceSMSCode(smsCode, smsCodeId);

            //    if (contract == null)
            //    {
            //        throw new ValidationException("Error verifying SMS code. Please try again");
            //    }

            //    if (contract.parmSuccess)
            //    {                    
            //        output.smsCodeId = contract.parmResourceLoginSMSCodeRecId;                   

            //        SoapEntityRepository repo = SoapEntityRepository.GetService();
                    
            //        QueryExpression query = new QueryExpression(SystemUser.EntityLogicalName);

            //        query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname", "mzk_axresourcerefrecid");

            //        query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, new Guid(sysUserId));

            //        LinkEntity position = new LinkEntity(SystemUser.EntityLogicalName, xrm.Position.EntityLogicalName, "positionid", "positionid", JoinOperator.LeftOuter);
            //        position.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("name");

            //        LinkEntity businessUnit = new LinkEntity(SystemUser.EntityLogicalName, BusinessUnit.EntityLogicalName, "businessunitid", "businessunitid", JoinOperator.Inner);
            //        businessUnit.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("name");

            //        query.LinkEntities.Add(position);
            //        query.LinkEntities.Add(businessUnit);

            //        EntityCollection entitycollection = repo.GetEntityCollection(query);

            //        Entity entity = entitycollection[0];

            //        SystemUser user = (SystemUser)entity;

            //        output.fullName = user.FullName;
            //        output.resourceRecId = user.mzk_AXResourceRefRecId.HasValue ? Convert.ToInt64(user.mzk_AXResourceRefRecId.Value).ToString() : "";

            //        if (entity.Attributes.Contains("position1.name"))
            //            output.designation = (entity.Attributes["position1.name"] as AliasedValue).Value.ToString();

            //        if (entity.Attributes.Contains("businessunit2.name"))
            //            output.organization = (entity.Attributes["businessunit2.name"] as AliasedValue).Value.ToString();

            //        output.success = true;
            //        output.userId = sysUserId;                    

            //        output.privileges = User.getPrivileges(sysUserId);

            //        if (HttpContext.Current.Request.Headers["Authorization"] != null)
            //        {
            //            output.token = HttpContext.Current.Request.Headers["Authorization"].ToString();
            //        }
            //    }
            //    else
            //    {
            //        output.success = false;
            //        output.doLogout = contract.parmLogOut;
            //        output.errorMessage = contract.parmErrorMessage;
            //    }                
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            return output;
        }

        public static async Task<LoginModelOutput> resendSMSCode(string sysUserId, long smsCodeId)
        {
            LoginModelOutput output = new LoginModelOutput();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
               
                QueryExpression query = new QueryExpression(SystemUser.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname", "mzk_axresourcerefrecid");

                query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, new Guid(sysUserId));

                EntityCollection entitycollection = repo.GetEntityCollection(query);

                Entity entity = entitycollection[0];

                SystemUser user = (SystemUser)entity;

                long resourceRecId = user.mzk_AXResourceRefRecId.HasValue ? Convert.ToInt64(user.mzk_AXResourceRefRecId.Value) : 0;

                if (resourceRecId == 0)
                {
                    throw new ValidationException("Resource Id not set for the user. Please contact system administrator");
                }

                ResourceRepository resourceRepo = new ResourceRepository();
                //HMResourceLoginSMSCodeContract contract = resourceRepo.generateResourceSMSCode(resourceRecId, smsCodeId);

                //if (contract == null)
                //{
                //    throw new ValidationException("Error generating SMS code. Please try again");
                //}

                //if (contract.parmSuccess)
                //{
                //    output.success = true;
                //    output.smsCodeId = contract.parmResourceLoginSMSCodeRecId;
                //    output.userId = sysUserId;
                //    output.errorMessage = "SMS code sent successfully";
                //}               
                //else
                //{
                //    output.success = false;
                //    output.doLogout = contract.parmLogOut;
                //    output.errorMessage = contract.parmErrorMessage;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return output;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string GetUserIp()
        {
            if (HttpContext.Current.Request.Headers["Origin"] != null)
            {
                return HttpContext.Current.Request.Headers["Origin"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public async static Task<bool> ValidateToken(string authToken)
        {
            LoginModelOutput output = new LoginModelOutput();
            try
            {
                // string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));

                //Zain -- for Basic scheme 
                if (authToken.StartsWith("Basic "))
                {
                    authToken = authToken.Substring("Basic ".Length).Trim();
                }
                //Zain

                string decodedToken = Authentication.Base64Decode(authToken);
                string userName = decodedToken.Substring(0, decodedToken.IndexOf(":"));
                string password = decodedToken.Substring(decodedToken.IndexOf(":") + 1);

                // bool result = true;
                output = await Authentication.Authenticate(userName, password, AppSettings.GetByKey("DOMAIN"), true);

                if (output.success)
                {
                    SoapCredential.UserName = userName;
                    SoapCredential.Password = password;
                    SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }

            return false;
        }

    }

   
}
