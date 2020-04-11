using MazikLogger;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;



namespace MazikCareService.CRMRepository
{
    public enum RequestType
    {
        Create,
        Update,
        Delete
    }

    //  public class SoapEntityRepository<T> : Repository where T : Microsoft.Dynamics.CRM.Crmbaseentity
    public class SoapEntityRepository : Repository
    {
        private IOrganizationService _service;        
        private ExecuteTransactionRequest executeTransactionRequest = null;

        private SoapEntityRepository()
        {
           
        }

        /*    public static SoapEntityRepository GetService()
            {
                SoapEntityRepository soapentoty = null;

                try
                {
                    if (HttpContext.Current.Items["service"] == null)
                    {
                        soapentoty = new SoapEntityRepository();
                        ClientCredentials credentials = new ClientCredentials();
                        credentials.SupportInteractive = false;
                        //credentials.Windows.ClientCredential = new System.Net.NetworkCredential(HttpContext.Current.Items["username"].ToString(), HttpContext.Current.Items["password"].ToString(), HttpContext.Current.Items["domain"].ToString());
                        credentials.UserName.UserName = HttpContext.Current.Items["username"].ToString();
                        credentials.UserName.Password = HttpContext.Current.Items["password"].ToString();
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        Uri organizationUri = new Uri(Helper.AppSettings.GetByKey("CRMService"));
                        soapentoty.proxyService = new OrganizationServiceProxy(organizationUri, null, credentials, null);
                        soapentoty.proxyService.EnableProxyTypes();
                        soapentoty._service = (IOrganizationService)soapentoty.proxyService;
                        HttpContext.Current.Items["service"] = soapentoty;
                    }
                    else
                    {
                        soapentoty = (SoapEntityRepository)HttpContext.Current.Items["service"];
                    }

                    return soapentoty;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }*/

        public static SoapEntityRepository GetService()
        {
            SoapEntityRepository soapentoty = null;

            try
            {
                if (HttpContext.Current.Items["service"] == null)
                {
                    soapentoty = new SoapEntityRepository();        

                    Uri organizationUri = new Uri(Helper.AppSettings.GetByKey("CRMService"));

                    OrganizationWebProxyClient sdkService = new OrganizationWebProxyClient(organizationUri, true);

                    sdkService.HeaderToken = HttpContext.Current.Items["token"].ToString();

                    IOrganizationService _orgService = (IOrganizationService)sdkService != null ? (IOrganizationService)sdkService : null;
                                        
                    soapentoty._service = (IOrganizationService)sdkService;

                    HttpContext.Current.Items["service"] = soapentoty;
                }
                else
                {
                    soapentoty = (SoapEntityRepository)HttpContext.Current.Items["service"];
                }

                return soapentoty;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public EntityCollection GetEntityCollection(QueryExpression query)
        {
            try
            {                
                EntityCollection mzk_EntityCollection = _service.RetrieveMultiple(query);
                return mzk_EntityCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
               

        public Guid CreateEntity(Entity _entity)
        {
            try
            {                
                return _service.Create(_entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Entity GetEntity(string entityName, Guid Id, ColumnSet column)
        {
            try
            {                
                return _service.Retrieve(entityName, Id, column);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateEntity(Entity entity)
        {
            try
            {
                _service.Update(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteEntity(string entityName, Guid Id)
        {
            try
            {
                _service.Delete(entityName, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ExecuteMultipleResponse ExecuteRetrieveMultiple(List<QueryExpression> queryExpressions)
        {
            ExecuteMultipleRequest multiReq = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };
            foreach (QueryExpression queryExpression in queryExpressions)
            {
                multiReq.Requests.Add(getRetrieveMultipleRequest(queryExpression));
            }
            return (ExecuteMultipleResponse)_service.Execute(multiReq);
        }

        private RetrieveMultipleRequest getRetrieveMultipleRequest(QueryExpression queryExpression)
        {
            return new RetrieveMultipleRequest
            {
                Query = queryExpression
            };
        }

        public OptionMetadata[] GetOptionSetItems(string optionSetName)
        {
            try
            {
                OptionMetadata[] optionList = null;
                RetrieveOptionSetRequest retrieveOptionSetRequest = new RetrieveOptionSetRequest
                {
                    Name = optionSetName
                };

                RetrieveOptionSetResponse retrieveOptionSetResponse = (RetrieveOptionSetResponse)_service.Execute(retrieveOptionSetRequest);

                OptionSetMetadata retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

                // Get the current options list for the retrieved attribute.
                optionList = retrievedOptionSetMetadata.Options.ToArray();                

                return optionList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public OptionMetadata[] GetOptionSetItems(string entityName,
                                             string optionSetAttributeName)
        {
            try
            {
                OptionMetadata[] optionList = null;
                RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityName,
                    LogicalName = optionSetAttributeName,
                    RetrieveAsIfPublished = true
                };

                RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)_service.Execute(retrieveAttributeRequest);
                if (retrieveAttributeResponse.AttributeMetadata.AttributeType == AttributeTypeCode.Picklist)
                {
                    PicklistAttributeMetadata retrievedPicklistAttributeMetadata = (PicklistAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;
                    optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
                }

                return optionList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateTransaction()
        {
            try
            {
                executeTransactionRequest = new ExecuteTransactionRequest();

                executeTransactionRequest.Requests = new OrganizationRequestCollection();
                executeTransactionRequest.ReturnResponses = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ExecuteTransactionResponse ExecuteTransaction()
        {
            try
            {
                return (ExecuteTransactionResponse)_service.Execute(executeTransactionRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            try
            {                
                return (OrganizationResponse)_service.Execute(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddTransactionRequest(Entity _entity)
        {
            try
            {
                if(executeTransactionRequest == null)
                {
                    this.CreateTransaction();
                }

                CreateRequest request = new CreateRequest
                {
                    Target = _entity
                };

                executeTransactionRequest.Requests.Add(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetUserId()
        {
            string sysUserId = string.Empty;
            try
            {
                sysUserId = ((WhoAmIResponse)_service.Execute(new WhoAmIRequest())).UserId.ToString();
            }
            catch (Exception ex)
            {
                throw new AuthenticationException("Username/Password is incorrect");
            }
            return sysUserId;
        }

        public ExecuteTransactionResponse ExecuteTransaction(Dictionary<Entity, RequestType> entityList)
        {
            try
            {                                  
                executeTransactionRequest = new ExecuteTransactionRequest();
                executeTransactionRequest.Requests = new OrganizationRequestCollection();
                executeTransactionRequest.ReturnResponses = true;                

                foreach (KeyValuePair<Entity, RequestType> entity in entityList)
                {
                    if (entity.Value == RequestType.Create)
                    {
                        CreateRequest request = new CreateRequest
                        {
                            Target = entity.Key
                        };
                        executeTransactionRequest.Requests.Add(request);
                    }
                    else if (entity.Value == RequestType.Update)
                    {
                        UpdateRequest request = new UpdateRequest
                        {
                            Target = entity.Key
                        };
                        executeTransactionRequest.Requests.Add(request);
                    }
                    else if (entity.Value == RequestType.Delete)
                    {
                        DeleteRequest request = new DeleteRequest
                        {
                            Target = new EntityReference(entity.Key.LogicalName, entity.Key.Id)
                        };
                        executeTransactionRequest.Requests.Add(request);
                    }
                }

                return (ExecuteTransactionResponse)_service.Execute(executeTransactionRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
