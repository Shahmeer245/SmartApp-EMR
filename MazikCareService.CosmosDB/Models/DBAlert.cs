using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.CosmosDB.Models
{
    public class DBAlert
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

        public async Task<List<DBAlert>> GetAlerts(string patientId)
        {
            DocumentDBRepository<DBAlert> documentDB = new DocumentDBRepository<DBAlert>("Items");
            IOrderedQueryable<DBAlert> query = documentDB.getDocumentQuery(new FeedOptions { MaxItemCount = -1 , EnableCrossPartitionQuery = true}).Result;

            IDocumentQuery<DBAlert> docQuery = query.Where(col => col.PatientId == patientId).AsDocumentQuery();

            List<DBAlert> results = new List<DBAlert>();

            while (docQuery.HasMoreResults)
            {
                results.AddRange(await docQuery.ExecuteNextAsync<DBAlert>());
            }

            return results;
        }
    }
}
