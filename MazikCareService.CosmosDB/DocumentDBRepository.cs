using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Configuration;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Helper;
using MazikCareService.CosmosDB.Models;

namespace MazikCareService.CosmosDB
{
    public class DocumentDBRepository<T> where T : class
    {
        private readonly string DatabaseId = AppSettings.GetByKey("database");
        private string CollectionId;
        private DocumentClient client;
        Uri uri;

        public DocumentDBRepository(string collectionId)
        {
            client = new DocumentClient(new Uri(AppSettings.GetByKey("endpoint")), AppSettings.GetByKey("authKey"));
            CollectionId = collectionId;
        }

        public async Task<IOrderedQueryable<T>> getDocumentQuery(FeedOptions feedOptions)
        {
            return client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), feedOptions);
        }

        public async Task<T> GetRecordAsync(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> CreateItemAsync(T item)
        {
            try
            {
                await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
        }

        public async Task DeleteItemAsync(string id)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }
    }
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "isComplete")]
        public bool Completed { get; set; }

        [JsonProperty(PropertyName = "patientId")]
        public string PatientId { get; set; }
    }
}
