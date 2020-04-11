using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class PowerBIAccessToken
    {
        public string token_type;
        public string scope { get; set; }
        public string expires_in { get; set; }
        public string expires_on { get; set; }
        public string not_before { get; set; }
        public string resource { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }

        public async Task<PowerBIAccessToken> SetAccessToken(string clientId, string clientSecret, string username, string password)
        {
            List<KeyValuePair<string, string>> vals = new List<KeyValuePair<string, string>>();
            vals.Add(new KeyValuePair<string, string>("grant_type", "password"));
            vals.Add(new KeyValuePair<string, string>("scope", "openid"));
            vals.Add(new KeyValuePair<string, string>("resource", "https://analysis.windows.net/powerbi/api"));
            vals.Add(new KeyValuePair<string, string>("client_id", clientId));
            vals.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            vals.Add(new KeyValuePair<string, string>("username", username));
            vals.Add(new KeyValuePair<string, string>("password", password));
            string TenantId = "common";
            string url = string.Format("https://login.windows.net/{0}/oauth2/token", TenantId);
            HttpClient hc = new HttpClient();
            HttpContent content = new FormUrlEncodedContent(vals);
            HttpResponseMessage hrm = hc.PostAsync(url, content).Result;
            string responseData = "";
            if (hrm.IsSuccessStatusCode)
            {
                Stream data = await hrm.Content.ReadAsStreamAsync();
                using (StreamReader reader = new StreamReader(data, Encoding.UTF8))
                {
                    responseData = reader.ReadToEnd();
                }
            }
            return JsonConvert.DeserializeObject<PowerBIAccessToken>(responseData);
        }
    }

    
}
