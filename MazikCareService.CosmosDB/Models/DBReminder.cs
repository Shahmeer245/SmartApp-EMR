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
    public class DBReminder
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "startDateTime")]
        public string StartDateTime { get; set; }

        [JsonProperty(PropertyName = "endDateTime")]
        public string EndDateTime { get; set; }

        [JsonProperty(PropertyName = "patientOrderId")]
        public string PatientOrderId { get; set; }

        public async Task<List<DBReminder>> GetReminders(string patientOrderId)
        {
            DocumentDBRepository<DBReminder> documentDB = new DocumentDBRepository<DBReminder>("Items");
            IOrderedQueryable<DBReminder> query = documentDB.getDocumentQuery(new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true }).Result;

            IDocumentQuery<DBReminder> docQuery = query.Where(col => col.PatientOrderId == patientOrderId).AsDocumentQuery();

            List<DBReminder> results = new List<DBReminder>();

            while (docQuery.HasMoreResults)
            {
                results.AddRange(await docQuery.ExecuteNextAsync<DBReminder>());
            }

            return results;
        }
    }
}
