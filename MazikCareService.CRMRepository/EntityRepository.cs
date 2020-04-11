
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Globalization;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;

namespace MazikCareService.CRMRepository
{
    public class EntityRepository<T> : Repository where T : Crmbaseentity
    {
        public async Task<bool> Add(T entity, string EntityName)
        {
            using (HttpClient client = new HttpClient(new HttpClientHandler() { Credentials = new NetworkCredential(this.UserName, this.Password, this.Domain) }))
            {
                HttpResponseMessage response = await HttpClientExtensions.SendAsJsonAsync<T>(client, HttpMethod.Post, this.CRMAPI + EntityName, entity);

                if (response.IsSuccessStatusCode)
                    return true;
                else
                    return false;

            }
        }

        public async Task<T> Get(string EntityGuid, string EntityName)
        {
            T item = null;
            using (var client = new HttpClient(new HttpClientHandler() { Credentials = new NetworkCredential(this.UserName, this.Password, this.Domain) }))
            {
                client.BaseAddress = new Uri(this.CRMAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");

                HttpResponseMessage response = await client.GetAsync(EntityName + "(" + EntityGuid + ")");

                if (response.IsSuccessStatusCode)
                {
                    var Content = await response.Content.ReadAsStringAsync();
                    T itemres = JsonConvert.DeserializeObject<T>(Content);
                    item = itemres;
                }
                else
                {

                }
            }

            return item;
        }

        public async Task<List<T>> FilterbyContains(string EntityName,string property,string value)
        {
            List<T> list = new List<T>();
            using (var client = new HttpClient(new HttpClientHandler() { Credentials = new NetworkCredential(this.UserName, this.Password, this.Domain) }))
            {
                client.BaseAddress = new Uri(this.CRMAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");

                HttpResponseMessage response = await client.GetAsync(EntityName + string.Format("?$filter=contains({0},\'{1}\')", property,value));

                if (response.IsSuccessStatusCode)
                {
                    var Content = await response.Content.ReadAsStringAsync();
                    ODataResponse<T> itemres = JsonConvert.DeserializeObject<ODataResponse<T>>(Content);
                    list = this.CollectionFromResponseSet(itemres.Value);
                }
                else
                {

                }
            }

            return list;
        }

        public async Task<List<T>> GetAll(string EntityName)
        {
            List<T> list = new List<T>();

            using (var client = new HttpClient(new HttpClientHandler() { Credentials = new NetworkCredential(this.UserName, this.Password, this.Domain) }))
            {
                // New code:
                client.BaseAddress = new Uri(this.CRMAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");

                HttpResponseMessage retrieveResponse = await client.GetAsync(EntityName);

                if (retrieveResponse.IsSuccessStatusCode)
                {
                    var Content = await retrieveResponse.Content.ReadAsStringAsync();
                    ODataResponse<T>  odataresponse = JsonConvert.DeserializeObject<ODataResponse<T>>(Content);
                    list = this.CollectionFromResponseSet(odataresponse.Value);
                }
                else
                {

                }
            }

            return list;
        }


        public async Task<bool> Update(T entity, string EntityName, string EntityId)
        {
            using (HttpClient client = new HttpClient(new HttpClientHandler() { Credentials = new NetworkCredential(this.UserName, this.Password, this.Domain) }))
            {

                HttpResponseMessage response = await HttpClientExtensions.SendAsJsonAsync<T>(client, new HttpMethod("PATCH"), this.CRMAPI + EntityName + "(" + EntityId + ")", entity);

                if (response.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> Delete(string EntityName, string EntityId)
        {
            using (HttpClient client = new HttpClient(new HttpClientHandler() { Credentials = new NetworkCredential(this.UserName, this.Password, this.Domain) }))
            {

                HttpResponseMessage response = await client.DeleteAsync(this.CRMAPI + EntityName + "(" + EntityId + ")");

                if (response.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
        }
    }
}
