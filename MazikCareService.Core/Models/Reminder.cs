using MazikCareService.CosmosDB;
using MazikCareService.CosmosDB.Models;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class Reminder
    {
        public string id { get; set; }
        public string patientOrderId { get; set; }
        public string doseId { get; set; }
        public DateTime startDateTime { get; set; }
        public DateTime endDateTime { get; set; }
        public string frequencyPerDay { get; set; }
        public string reminderAfterDays { get; set; }
        public List<string> schedules { get; set; }
        public string instruction { get; set; }

        public async Task<List<Reminder>> getReminders(string patientOrderId)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientOrderId))
                {
                    List<Reminder> reminders = new List<Reminder>();

                    List<DBReminder> listReminders = await new DBReminder().GetReminders(patientOrderId);

                    foreach (DBReminder item in listReminders)
                    {
                        Reminder obj = new Reminder();
                        if (item.Id != null)
                        {
                            obj.id = item.Id;
                        }
                        if (item.PatientOrderId != null)
                        {
                            obj.patientOrderId = item.PatientOrderId;
                        }
                        if (item.StartDateTime != null)
                        {
                            obj.startDateTime = Convert.ToDateTime(item.StartDateTime);
                        }
                        if (item.EndDateTime != null)
                        {
                            obj.endDateTime = Convert.ToDateTime(item.EndDateTime);
                        }
                        reminders.Add(obj);
                    }
                    return reminders;
                }
                else
                {
                    throw new ValidationException("PatientOrder Id not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> createReminders(Reminder reminder)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                bool response = false;
                DocumentDBRepository<DBReminder> documentDB = new DocumentDBRepository<DBReminder>("Items");
                if (reminder.patientOrderId != null)
                {
                    if (reminder.schedules.Count > 0)
                    {
                        Entity entity = new Entity("mzk_patientorder");
                        entity.Id = new Guid(reminder.patientOrderId);
                        entity["mzk_isreminderset"] = true;
                        repo.UpdateEntity(entity);
                        
                        int count = 0;
                        foreach (string schedule in reminder.schedules)
                        {
                            if (count != reminder.schedules.Count)
                            {
                                DBReminder reminderObj = new DBReminder();
                                reminderObj.PatientOrderId = reminder.patientOrderId;
                                reminderObj.StartDateTime = reminder.schedules[count];
                                response = await documentDB.CreateItemAsync(reminderObj);
                                count++;
                            }
                        }
                    }
                }
                
                //for (int i = 0; i < days; i++)
                //{
                    //DBReminder reminderObj = new DBReminder();
                    //if (reminder.patientOrderId != null)
                    //{
                    //    reminderObj.PatientOrderId = reminder.patientOrderId;
                    //}
                   // reminderObj.StartDateTime = reminder.startDateTime;
                  //  reminderObj.EndDateTime = reminder.endDateTime;



                    //response = await documentDB.CreateItemAsync(reminderObj);
                //}
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
