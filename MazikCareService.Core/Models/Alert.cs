using Helper;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MazikCareService.CosmosDB.Models;
using MazikCareService.CosmosDB;

namespace MazikCareService.Core.Models
{
    public class Alert
    {
        public string name { get; set; }
        public string id { get; set; }
        public string patientId { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public int typeValue { get; set; }
        public string text { get; set; }
        public DateTime alertDate { get; set; }

        public async Task<List<Alert>> getAlerts(string patientId)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientId))
                {
                    List<Alert> alerts = new List<Alert>();
                   
                    List<DBAlert> listAlerts = await new DBAlert().GetAlerts(patientId);

                    foreach (DBAlert item in listAlerts)
                    {
                        Alert obj = new Alert();
                        if (item.Id != null)
                        {
                            obj.id = item.Id;
                        }
                        if (item.PatientId != null)
                        {
                            obj.patientId = item.PatientId;
                        }
                        if (item.Name != null)
                        {
                            obj.name = item.Name;
                        }
                        if (item.Description != null)
                        {
                            obj.description = item.Description;
                        }
                        alerts.Add(obj);
                    }
                    return alerts;
                }
                else
                {
                    throw new ValidationException("Patient Id not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }     

        public async Task<bool> CreateNotification(Alert alert)
        {
            try
            {
                bool response = false;
                DocumentDBRepository<DBAlert> documentDB = new DocumentDBRepository<DBAlert>("Items");

                DBAlert alertObj = new DBAlert();
                alertObj.PatientId = alert.patientId;
                alertObj.Name = alert.name;
                alertObj.Description = alert.description;
               
                response = await documentDB.CreateItemAsync(alertObj);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
