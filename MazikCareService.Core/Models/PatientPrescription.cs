using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Models.HL7;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class PatientPrescription
    {
        public string prescriptionId { get; set; }
        public string prescriber { get; set; }
        public string contract { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string pharmacist { get; set; }
        public string patient { get; set; }
        public string prescriberComments { get; set; }
        public List<PatientMedication> medicationOrder { get; set; }
        public async Task<List<PatientPrescription>> getPrescriptions(string patientId)
        {
            List<PatientPrescription> prescriptions = new List<PatientPrescription>();
            QueryExpression query = new QueryExpression(xrm.mzk_prescription.EntityLogicalName);
            query.Criteria.AddCondition("mzk_patient", ConditionOperator.Equal, new Guid(patientId));
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.Or);
            //childFilter.AddCondition("mzk_prescriptionstatus", ConditionOperator.Equal, 275380000);//Received
            childFilter.AddCondition("mzk_prescriptionstatus", ConditionOperator.Equal, 275380001);//Verified
            childFilter.AddCondition("mzk_prescriptionstatus", ConditionOperator.Equal, 275380004);//Ready
            query.ColumnSet = new ColumnSet("mzk_prescriptionid", "mzk_prescriber", "mzk_contract", "mzk_startdate", "mzk_enddate", "mzk_pharmacist", "mzk_patient", "mzk_prescribercomments");
            LinkEntity link1 = new LinkEntity(xrm.mzk_prescription.EntityLogicalName,xrm.mzk_patientorder.EntityLogicalName,"mzk_prescriptionid", "mzk_prescription", JoinOperator.LeftOuter)
            {
                Columns = new ColumnSet("mzk_patientorderid","mzk_productid","mzk_dosevalue", "mzk_unitid", "mzk_routeid", "mzk_frequencyid", "mzk_medicationtype",
                "mzk_deliveredquantity", "mzk_remainingorders", "mzk_totalsupplyqty", "mzk_startdate", "mzk_enddate"),
                LinkCriteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression("mzk_patientorderid",ConditionOperator.NotNull)//None
                    }
                },
                EntityAlias="MedicationOrder"
            };
            query.LinkEntities.Add(link1);

            SoapEntityRepository repo = SoapEntityRepository.GetService();
            EntityCollection entityCollection = repo.GetEntityCollection(query);

            var groupedPrescriptions = entityCollection.Entities.GroupBy(item => (item.GetAttributeValue<Guid>("mzk_prescriptionid")));
            foreach (var groupedPrescriptionsCurrent in groupedPrescriptions)
            {
                PatientPrescription prescription = new PatientPrescription();
                prescription.medicationOrder = new List<PatientMedication>();

                

                foreach (Entity entity in groupedPrescriptionsCurrent)
                {
                    PatientMedication medication = new PatientMedication();
                    if (entity.Attributes.Contains("mzk_prescriptionid"))
                        prescription.prescriptionId = entity["mzk_prescriptionid"].ToString();
                    if (entity.Attributes.Contains("mzk_prescriber"))
                    {
                        prescription.prescriber = (entity["mzk_prescriber"] as EntityReference).Name;
                    }
                    if (entity.Attributes.Contains("mzk_contract"))
                    {
                        prescription.contract = (entity["mzk_contract"] as EntityReference).Name;
                    }
                    if (entity.Attributes.Contains("mzk_startdate"))
                    {
                        prescription.startDate = (DateTime)entity["mzk_startdate"];
                    }
                    if (entity.Attributes.Contains("mzk_enddate"))
                    {
                        prescription.endDate = (DateTime)entity["mzk_enddate"];
                    }
                    if (entity.Attributes.Contains("mzk_pharmacist"))
                    {
                        prescription.pharmacist = (entity["mzk_pharmacist"] as EntityReference).Name;
                    }
                    if (entity.Attributes.Contains("mzk_patient"))
                    {
                        prescription.patient = (entity["mzk_patient"] as EntityReference).Name;
                    }
                    if (entity.Attributes.Contains("mzk_prescribercomments"))
                    {
                        prescription.prescriberComments = entity["mzk_prescribercomments"].ToString();
                    }

                    if (entity.Attributes.Contains("MedicationOrder.mzk_patientorderid"))
                    {
                        medication.MedicationOrderId = entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_patientorderid").Value.ToString();
                    }
                    if (entity.Attributes.Contains("MedicationOrder.mzk_productid"))
                    {
                        medication.ProductId = (entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_productid").Value as EntityReference).Id.ToString();
                        medication.MedicationName = (entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_productid").Value as EntityReference).Name;
                    }

                    if (entity.Attributes.Contains("MedicationOrder.mzk_medicationtype"))
                    {
                        medication.MedicationInstructions = new List<string>();
                        if (entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_medicationtype").Value.Equals(new OptionSetValue(275380000)))
                        {
                            if (entity.Attributes.Contains("MedicationOrder.mzk_dosevalue") &&
                                 entity.Attributes.Contains("MedicationOrder.mzk_unitid") &&
                                 entity.Attributes.Contains("MedicationOrder.mzk_routeid") &&
                                 entity.Attributes.Contains("MedicationOrder.mzk_frequencyid") &&
                                 entity.Attributes.Contains("MedicationOrder.mzk_startdate") && 
                                 entity.Attributes.Contains("MedicationOrder.mzk_enddate")
                                 )
                            {
                                medication.MedicationInstructions.Add("Use " + entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_dosevalue").Value.ToString() +
                        " " + (entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_unitid").Value as EntityReference).Name +
                        " " + (entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_routeid").Value as EntityReference).Name +
                        " " + (entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_frequencyid").Value as EntityReference).Name +
                        " from " + Convert.ToDateTime(entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_startdate").Value).ToShortDateString() + " to " + Convert.ToDateTime(entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_enddate").Value).ToShortDateString());
                            }
                        }
                        if (entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_medicationtype").Value.Equals(new OptionSetValue(275380003)))
                        {
                            if (!string.IsNullOrEmpty(medication.MedicationOrderId))
                            {
                                medication.MedicationInstructions = getMedicationInstructions(medication.MedicationOrderId);
                            }
                        }
                        if (entity.Attributes.Contains("MedicationOrder.mzk_deliveredquantity"))
                        {
                            medication.deliveredQuantity = Convert.ToInt32(entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_deliveredquantity").Value);
                        }
                        if (entity.Attributes.Contains("MedicationOrder.mzk_remainingorders"))
                        {
                            medication.remainingQuantity = Convert.ToInt32(entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_remainingorders").Value);
                        }
                        if (entity.Attributes.Contains("MedicationOrder.mzk_totalsupplyqty"))
                        {
                            medication.totalQuantity = Convert.ToInt32(entity.GetAttributeValue<AliasedValue>("MedicationOrder.mzk_totalsupplyqty").Value);
                        }
                        prescription.medicationOrder.Add(medication);
                    }
                }
                prescriptions.Add(prescription);

            }

            return prescriptions;
        }
        public List<string> getMedicationInstructions(string patientOrderId)
        {
            List<string> Instructions = new List<string>();
            QueryExpression query = new QueryExpression("mzk_patientorder");
            query.Criteria.AddCondition("mzk_patientorderid", ConditionOperator.Equal, new Guid(patientOrderId));
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_routeid");
            LinkEntity EntityDose = new LinkEntity("mzk_patientorder", "mzk_dose", "mzk_patientorderid", "mzk_medicationorder", JoinOperator.Inner);
            EntityDose.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_startdate", "mzk_enddate", "mzk_medicationdose", "mzk_unitid", "mzk_frequencyid");
            EntityDose.EntityAlias = "Dose";
            query.LinkEntities.Add(EntityDose);
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entityCollection = entityRepository.GetEntityCollection(query);
            foreach (Entity entity in entityCollection.Entities)
            {
                if (entity.Attributes.Contains("mzk_routeid") && entity.Attributes.Contains("Dose.mzk_startdate") && entity.Attributes.Contains("Dose.mzk_enddate") && entity.Attributes.Contains("Dose.mzk_medicationdose")
                    && entity.Attributes.Contains("Dose.mzk_unitid") && entity.Attributes.Contains("Dose.mzk_frequencyid"))
                {
                    Instructions.Add("Use " + entity.GetAttributeValue<AliasedValue>("Dose.mzk_medicationdose").Value.ToString() +
                        " " + (entity.GetAttributeValue<AliasedValue>("Dose.mzk_unitid").Value as EntityReference).Name +
                        " " + (entity["mzk_routeid"] as EntityReference).Name +
                        " " + (entity.GetAttributeValue<AliasedValue>("Dose.mzk_frequencyid").Value as EntityReference).Name +
                        " from " + Convert.ToDateTime(entity.GetAttributeValue<AliasedValue>("Dose.mzk_startdate").Value).ToShortDateString() + " to " + Convert.ToDateTime(entity.GetAttributeValue<AliasedValue>("Dose.mzk_enddate").Value).ToShortDateString());
                }

            }
            return Instructions;
        }

    }
}
