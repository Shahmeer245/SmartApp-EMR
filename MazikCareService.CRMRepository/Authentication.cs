using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;
using Helper;

namespace MazikCareService.CRMRepository
{
    public static class Authentication
    {
        public static async Task<string> CRMAuthenticate(string username, string password, string domain)
        {
            string userId;
            string WebApiurl = AppSettings.GetByKey("CRMAPI");

            using (var client = new HttpClient(new HttpClientHandler() { Credentials = new NetworkCredential(username, password, domain) }))
            {
               
                List<Systemuser> list = new List<Systemuser>();
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
                        ODataResponse<Systemuser> itemres = JsonConvert.DeserializeObject<ODataResponse<Systemuser>>(Content);
                        list = Authentication.CollectionFromResponseSet(itemres.Value);

                        userId = list[0].Systemuserid.Value.ToString();
                    }
                    else if (retrieveResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        userId = string.Empty;
                    }
                    else
                    {
                        userId = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    userId = string.Empty;
                }
            }

            return userId;
        }

        public static List<T> CollectionFromResponseSet<T>(T[] ResponseSet)
        {
            List<T> list = new List<T>(ResponseSet.Count());
            foreach (T item in ResponseSet)
            {
                list.Add(item);
            }

            return list;
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
            try
            {
                // string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));
               

                string decodedToken = Authentication.Base64Decode(authToken);
                string userName = decodedToken.Substring(0, decodedToken.IndexOf(":"));
                string password = decodedToken.Substring(decodedToken.IndexOf(":") + 1);

                // bool result = true;
                string output = await Authentication.CRMAuthenticate(userName, password, AppSettings.GetByKey("DOMAIN"));

                if (output != null && output != string.Empty)
                {
                    SoapCredential.UserName = userName;
                    SoapCredential.Password = password;
                    SoapCredential.Password = AppSettings.GetByKey("DOMAIN");
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
