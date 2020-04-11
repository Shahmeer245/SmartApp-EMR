using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
   public static class CrmAuthentication
    {
        public static async Task<LoginModelOutput> Authenticate(string username, string password, string domain, string WebApiurl)
        {
            LoginModelOutput output = new LoginModelOutput();
            using (var client = new HttpClient(new HttpClientHandler() { Credentials = new NetworkCredential(username, password, domain) }))
            {
               
                List<SystemUser> list = new List<SystemUser>();
                // New code:
                try
                { 
                    client.BaseAddress = new Uri(WebApiurl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    client.DefaultRequestHeaders.Add("OData-Version", "4.0");

                    HttpResponseMessage retrieveResponse = await client.GetAsync("systemusers" + string.Format("?$filter=contains({0},\'{1}\')", "domainname", domain + "\\" + username));

                    if (retrieveResponse.IsSuccessStatusCode)
                    {
                        var Content = await retrieveResponse.Content.ReadAsStringAsync();
                        ODataResponse<SystemUser> itemres = JsonConvert.DeserializeObject<ODataResponse<SystemUser>>(Content);
                        list = output.CollectionFromResponseSet(itemres.Value);

                        output.resourceRecId = list[0].mzk_axresourcerefrecid;
                        output.userId = list[0].systemuserid;
                        output.success = true;
                    }
                    else if (retrieveResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        output.success = false;
                    }
                    else
                    {
                        output.success = false;
                    }
                }
                catch (Exception ex)
                {
                    output.success = false;
                }
            }

            return output;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
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
               

                string decodedToken = CrmAuthentication.Base64Decode(authToken);
                string userName = decodedToken.Substring(0, decodedToken.IndexOf(":"));
                string password = decodedToken.Substring(decodedToken.IndexOf(":") + 1);

                // bool result = true;
                output = await CrmAuthentication.Authenticate(userName, password, AppSettings.GetByKey("DOMAIN"), AppSettings.GetByKey("CRMHOST"));

                if (output.success)
                {
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
