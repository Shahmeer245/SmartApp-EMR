using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class Notes : INotes
    {
        public string ObjectId { get; set; }
        public string Id { get; set; }
        public string NoteText { get; set; }
        public string Subject { get; set; }
        public string EntityType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string fileName { get; set; }
        public string mimeType { get; set; }
        public string file { get; set; }
        public string objectTypeCode { get; set; }


        public async Task<string> AddNotes(Notes notes)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            xrm.Annotation annotationNotes = new xrm.Annotation();

            if (!string.IsNullOrEmpty(notes.NoteText))
                annotationNotes.NoteText = notes.NoteText;

            if (!string.IsNullOrEmpty(notes.Subject))
                annotationNotes.Subject = notes.Subject;

            if (!string.IsNullOrEmpty(notes.ObjectId))
                annotationNotes.ObjectId = new Microsoft.Xrm.Sdk.EntityReference(notes.EntityType, new Guid(notes.ObjectId));

            if (!string.IsNullOrEmpty(notes.fileName))
                annotationNotes.FileName = notes.fileName;

            if (!string.IsNullOrEmpty(notes.mimeType))
                annotationNotes.MimeType = notes.mimeType;

            if (!string.IsNullOrEmpty(notes.file))
                annotationNotes.DocumentBody = notes.file;


            Id = Convert.ToString(entityRepository.CreateEntity(annotationNotes));

            return Id.ToString();
        }

        public async Task<List<Notes>> getNotes(Notes notes)
        {
            List<Notes> PatientNotes = new List<Notes>();
            xrm.Annotation patientOrderEntity = new xrm.Annotation();

            QueryExpression query = new QueryExpression(Annotation.EntityLogicalName);
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            //check if it is case
            PatientCase pcase = new PatientCase();
            if (!string.IsNullOrEmpty(notes.ObjectId))
                if (pcase.getCaseDetails(notes.ObjectId).Result.CaseNumber != null)
                {
                    #region Case
                    PatientEncounter encounter = new PatientEncounter();
                    encounter.CaseId = notes.ObjectId;
                    List<PatientEncounter> listEncounterId = encounter.getEncounterDetails(encounter).Result;
                    if (listEncounterId.Count > 0)
                    {
                        for (int entityCount = 0; entityCount < listEncounterId.Count; entityCount++)
                        {
                            #region Notes
                            QueryExpression querycase = new QueryExpression(Annotation.EntityLogicalName);
                            FilterExpression childFiltercase = querycase.Criteria.AddFilter(LogicalOperator.And);
                            childFiltercase.AddCondition("objectid", ConditionOperator.Equal, new Guid(listEncounterId[entityCount].EncounterId));
                            //Entity :: Notes
                            querycase.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("objectid",
                                                                                    "subject",
                                                                                    "notetext",
                                                                                    "createdby", "createdon", "modifiedon", "modifiedby");
                            OrderExpression orderby = new OrderExpression();
                            orderby.AttributeName = "createdon";
                            orderby.OrderType = OrderType.Descending;
                            querycase.Orders.Add(orderby);

                            #endregion
                            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                            EntityCollection entitycollection = entityRepository.GetEntityCollection(querycase);

                            foreach (Entity entity in entitycollection.Entities)
                            {
                                Notes model = new Notes();

                                if (entity.Attributes.Contains("objectid"))
                                    model.ObjectId = ((EntityReference)entity.Attributes["objectid"]).Id.ToString();

                                if (entity.Attributes.Contains("subject"))
                                    model.Subject = entity.Attributes["subject"].ToString();

                                if (entity.Attributes.Contains("notetext"))
                                    model.NoteText = entity.Attributes["notetext"].ToString();

                                if (entity.Attributes.Contains("createdon"))
                                    model.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);

                                if (entity.Attributes.Contains("createdby"))
                                    model.CreatedBy = ((EntityReference)entity.Attributes["createdby"]).Name;

                                model.Id = entity.Id.ToString();

                                PatientNotes.Add(model);

                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Encounter
                    if (!string.IsNullOrEmpty(notes.Id))
                        childFilter.AddCondition("annotationid", ConditionOperator.Equal, new Guid(notes.Id));

                    if (!string.IsNullOrEmpty(notes.ObjectId))
                        childFilter.AddCondition("objectid", ConditionOperator.Equal, new Guid(notes.ObjectId));

                    //Entity :: Notes
                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("objectid",
                                                                            "subject",
                                                                            "notetext",
                                                                            "createdby", "createdon", "modifiedon", "modifiedby");

                    OrderExpression orderby = new OrderExpression();
                    orderby.AttributeName = "createdon";
                    orderby.OrderType = OrderType.Descending;

                    query.Orders.Add(orderby);

                    if (string.IsNullOrEmpty(notes.ObjectId) && string.IsNullOrEmpty(notes.Id))
                        throw new ValidationException("Parameter missing");


                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                    foreach (Entity entity in entitycollection.Entities)
                    {

                        Notes model = new Notes();

                        if (entity.Attributes.Contains("objectid"))
                            model.ObjectId = ((EntityReference)entity.Attributes["objectid"]).Id.ToString();

                        if (entity.Attributes.Contains("subject"))
                            model.Subject = entity.Attributes["subject"].ToString();

                        if (entity.Attributes.Contains("notetext"))
                            model.NoteText = entity.Attributes["notetext"].ToString();

                        if (entity.Attributes.Contains("createdon"))
                            model.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);

                        if (entity.Attributes.Contains("createdby"))
                            model.CreatedBy = ((EntityReference)entity.Attributes["createdby"]).Name;

                        model.Id = entity.Id.ToString();

                        PatientNotes.Add(model);

                    }
                    #endregion
                }




            return PatientNotes;
        }

        public async Task<bool> updateNotes(Notes notes)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(Annotation.EntityLogicalName);
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                query.Criteria.AddCondition("annotationid", ConditionOperator.Equal, notes.Id);
                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                if (entitycollection.Entities.Count > 0)
                {
                    Annotation annotationNotes = (Annotation)entitycollection.Entities[0];

                    if (!string.IsNullOrEmpty(notes.NoteText))
                        annotationNotes.NoteText = notes.NoteText;

                    entityRepository.UpdateEntity(annotationNotes);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<Notes>> getCaseNotesOLD(string caseId)
        {
            List<Notes> PatientNotes = new List<Notes>();
            xrm.Annotation patientOrderEntity = new xrm.Annotation();
            PatientEncounter encounter = new PatientEncounter();
            encounter.CaseId = caseId;
            List<PatientEncounter> listEncounterId = encounter.getEncounterDetails(encounter).Result;
            if (listEncounterId.Count > 0)
            {
                for (int entityCount = 0; entityCount < listEncounterId.Count; entityCount++)
                {
                    #region Notes
                    QueryExpression query = new QueryExpression(Annotation.EntityLogicalName);
                    FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                    childFilter.AddCondition("objectid", ConditionOperator.Equal, new Guid(listEncounterId[entityCount].EncounterId));
                    //Entity :: Notes
                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("objectid",
                                                                            "subject",
                                                                            "notetext",
                                                                            "createdby", "createdon", "modifiedon", "modifiedby");
                    OrderExpression orderby = new OrderExpression();
                    orderby.AttributeName = "createdon";
                    orderby.OrderType = OrderType.Descending;
                    query.Orders.Add(orderby);

                    #endregion
                    SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                    EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                    foreach (Entity entity in entitycollection.Entities)
                    {
                        Notes model = new Notes();

                        if (entity.Attributes.Contains("objectid"))
                            model.ObjectId = ((EntityReference)entity.Attributes["objectid"]).Id.ToString();

                        if (entity.Attributes.Contains("subject"))
                            model.Subject = entity.Attributes["subject"].ToString();

                        if (entity.Attributes.Contains("notetext"))
                            model.NoteText = entity.Attributes["notetext"].ToString();

                        if (entity.Attributes.Contains("createdon"))
                            model.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);

                        if (entity.Attributes.Contains("createdby"))
                            model.CreatedBy = ((EntityReference)entity.Attributes["createdby"]).Name;

                        model.Id = entity.Id.ToString();

                        PatientNotes.Add(model);

                    }
                }
            }
            return PatientNotes;
        }

        public async Task<List<Notes>> getCaseNotes(string caseId)
        {
            List<Notes> PatientNotes = new List<Notes>();
            xrm.Annotation patientOrderEntity = new xrm.Annotation();

            QueryExpression query = new QueryExpression(Annotation.EntityLogicalName);           
            
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("objectid",
                                                                    "subject",
                                                                    "notetext",
                                                                    "createdby", "createdon", "modifiedon", "modifiedby");
            OrderExpression orderby = new OrderExpression();
            orderby.AttributeName = "createdon";
            orderby.OrderType = OrderType.Descending;
            query.Orders.Add(orderby);

            LinkEntity ptEnc = new LinkEntity(Annotation.EntityLogicalName, mzk_patientencounter.EntityLogicalName, "objectid", "mzk_patientencounterid", JoinOperator.Inner);

            ptEnc.LinkCriteria.AddCondition("mzk_caseid", ConditionOperator.Equal, new Guid(caseId));

            query.LinkEntities.Add(ptEnc);

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                Notes model = new Notes();

                if (entity.Attributes.Contains("objectid"))
                    model.ObjectId = ((EntityReference)entity.Attributes["objectid"]).Id.ToString();

                if (entity.Attributes.Contains("subject"))
                    model.Subject = entity.Attributes["subject"].ToString();

                if (entity.Attributes.Contains("notetext"))
                    model.NoteText = entity.Attributes["notetext"].ToString();

                if (entity.Attributes.Contains("createdon"))
                    model.CreatedOn = Convert.ToDateTime(entity.Attributes["createdon"]);

                if (entity.Attributes.Contains("createdby"))
                    model.CreatedBy = ((EntityReference)entity.Attributes["createdby"]).Name;

                model.Id = entity.Id.ToString();

                PatientNotes.Add(model);

            }
                        
            return PatientNotes;
        }

        public async Task<Notes> getFile(string annotationId)
        {
            Notes model = new Notes();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression("annotation");

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("documentbody", "mimetype");

                query.Criteria.AddCondition("annotationid", ConditionOperator.Equal, new Guid(annotationId));

                EntityCollection entityCollection = repo.GetEntityCollection(query);

                if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                {
                    model.file = entityCollection.Entities[0].Attributes.Contains("documentbody") ? entityCollection.Entities[0].Attributes["documentbody"].ToString() : string.Empty;
                    model.mimeType = entityCollection.Entities[0].Attributes.Contains("mimetype") ? entityCollection.Entities[0].Attributes["mimetype"].ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }


    }
}
