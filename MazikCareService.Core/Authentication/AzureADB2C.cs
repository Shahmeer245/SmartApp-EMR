using Helper;
using MazikCareService.Core.Models;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core
{
    public class AzureADB2C
    {
        public static async Task<LoginModelOutput> AuthenticateUser(string username, string password)
        {
            LoginModelOutput output = new LoginModelOutput();

            try
            {
                using (var client = new HttpClient(new HttpClientHandler()))
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("username"      , username),
                        new KeyValuePair<string, string>("password"           , password),
                        new KeyValuePair<string, string>("grant_type"  ,"password"),
                        new KeyValuePair<string, string>("scope"    ,"openid " + AppSettings.GetByKey("AzureADB2CClientID")+ " offline_access"),
                        new KeyValuePair<string, string>("client_id"  ,AppSettings.GetByKey("AzureADB2CClientID")),
                        new KeyValuePair<string, string>("response_type"    ,"token id_token")
                    });
                    var result = client.PostAsync(AppSettings.GetByKey("AzureADB2CURL"), content).Result;

                    if (result != null && result.IsSuccessStatusCode && result.Content != null)
                    {
                        string resultContent = result.Content.ReadAsStringAsync().Result;

                        dynamic ret = JsonConvert.DeserializeObject(resultContent);

                        if (ret != null && ret.access_token != null)
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var jsonToken = handler.ReadToken(ret.access_token.Value);

                            List<System.Security.Claims.Claim> claimList = jsonToken.Claims;

                            System.Security.Claims.Claim oid = claimList.Where(item => item.Type == "oid").FirstOrDefault();

                            if (oid != null)
                            {
                                LoginModelOutput token = DynamicsCRMOAuth.AuthenticateUser(false, "", "").Result;

                                if (token == null || !token.success)
                                {                                    
                                    output.success = false;
                                    throw new AuthenticationException("Error while generating token");
                                }

                                SoapEntityRepository repo = SoapEntityRepository.GetService();

                                QueryExpression query = new QueryExpression("contact");
                                query.Criteria.AddCondition("mzk_appobjectid", ConditionOperator.Equal, oid.Value);

                                EntityCollection collection = repo.GetEntityCollection(query);

                                if (collection != null && collection.Entities != null && collection.Entities.Count > 0)
                                {
                                    output.userId = collection.Entities[0].Id.ToString();
                                    output.token = token.token;
                                    output.success = true;
                                }
                                else
                                {
                                    output.success = false;
                                    throw new AuthenticationException("User is not a registered user");
                                }
                            }
                            else
                            {
                                output.success = false;
                                throw new AuthenticationException("User is not a registered user");
                            }
                        }
                        else
                        {
                            output.success = false;
                            throw new AuthenticationException("Username/Password is incorrect");
                        }
                    }
                    else
                    {
                        output.success = false;
                        throw new AuthenticationException("Username/Password is incorrect");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return output;
        }
    }
}
