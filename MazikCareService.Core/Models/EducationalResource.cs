using MazikCareService.CRMRepository;
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
    public class EducationalResource
    {

        public string educationalResourceId { get; set; }
        public string patientEncounterEducationalResourceId { get; set; }
        public string name { get; set; }
        public string patientEncounterId { get; set; }
        public string patientId { get; set; }
        public Notes note { get; set; }


        public async Task<List<EducationalResource>> getAllEducationalResource(string filter)
        {
            try
            {
                List<EducationalResource> EducationalResource = new List<EducationalResource>();

                QueryExpression query = new QueryExpression("mzk_educationalresource");

                query.ColumnSet = new ColumnSet("mzk_name", "mzk_educationalresourceid");
                query.Criteria.AddCondition("mzk_name", ConditionOperator.Like, "%" + filter + "%");

                LinkEntity notes = new LinkEntity("mzk_educationalresource", "annotation", "mzk_educationalresourceid", "objectid", JoinOperator.LeftOuter);
                notes.Columns = new ColumnSet("mimetype", "filename");
                notes.LinkCriteria.AddCondition("isdocument", ConditionOperator.Equal, true);
                notes.LinkCriteria.AddCondition("objecttypecode", ConditionOperator.Equal, "mzk_educationalresource");

                notes.EntityAlias = "notes";

                query.LinkEntities.Add(notes);

                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entityCollection = entityRepository.GetEntityCollection(query);

                foreach (Entity entity in entityCollection.Entities)
                {
                    EducationalResource model = new EducationalResource();
                    mzk_educationalresource mzk_educationalresource = (mzk_educationalresource)entity;

                    if (entity.Attributes.Contains("mzk_educationalresourceid"))
                        model.educationalResourceId = mzk_educationalresource.mzk_educationalresourceId.ToString();

                    if (entity.Attributes.Contains("mzk_name"))
                        model.name = mzk_educationalresource.mzk_name;

                    Notes resourceNotes = new Notes();

                    if (entity != null)
                    {
                        resourceNotes.Id = entity.Attributes.Contains("notes.annotationid") ? (entity.Attributes["notes.annotationid"] as AliasedValue).Value.ToString() : string.Empty;
                        resourceNotes.fileName = entity.Attributes.Contains("notes.filename") ? (entity.Attributes["notes.filename"] as AliasedValue).Value.ToString() : string.Empty;
                        resourceNotes.mimeType = entity.Attributes.Contains("notes.mimetype") ? (entity.Attributes["notes.mimetype"] as AliasedValue).Value.ToString() : string.Empty;
                    }

                    model.note = resourceNotes;
                    EducationalResource.Add(model);
                }
                return EducationalResource;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<EducationalResource>> getPatientEncounterEducationalResource(string encounterGuid, string patientGuid)
        {
            try
            {
                List<EducationalResource> EducationalResource = new List<EducationalResource>();

                QueryExpression query = new QueryExpression("mzk_patientencountereducationalresource");
                query.ColumnSet = new ColumnSet("mzk_patientencountereducationalresourceid", "mzk_name", "mzk_customer", "mzk_patientencounter", "mzk_educationalresource");
                if (!string.IsNullOrEmpty(encounterGuid))
                    query.Criteria.AddCondition("mzk_patientencounter", ConditionOperator.Equal, new Guid(encounterGuid));
                if (!string.IsNullOrEmpty(patientGuid))
                    query.Criteria.AddCondition("mzk_customer", ConditionOperator.Equal, new Guid(patientGuid));

                LinkEntity educationalResourceQuery = new LinkEntity("mzk_patientencountereducationalresource", "mzk_educationalresource", "mzk_educationalresource", "mzk_educationalresourceid", JoinOperator.LeftOuter);
                educationalResourceQuery.Columns = new ColumnSet("mzk_educationalresourceid", "mzk_name");
                query.LinkEntities.Add(educationalResourceQuery);

                LinkEntity patientEncounterEducationalResourceNotes = new LinkEntity("mzk_patientencountereducationalresource", "annotation", "mzk_patientencountereducationalresourceid", "objectid", JoinOperator.LeftOuter);
                patientEncounterEducationalResourceNotes.Columns = new ColumnSet("annotationid", "mimetype", "filename");
                patientEncounterEducationalResourceNotes.LinkCriteria.AddCondition("isdocument", ConditionOperator.Equal, true);
                patientEncounterEducationalResourceNotes.LinkCriteria.AddCondition("objecttypecode", ConditionOperator.Equal, "mzk_patientencountereducationalresource");
                patientEncounterEducationalResourceNotes.EntityAlias = "patientEncounterEducationalResourceNotes";
                query.LinkEntities.Add(patientEncounterEducationalResourceNotes);

                LinkEntity educationalResourceNotes = new LinkEntity("mzk_educationalresource", "annotation", "mzk_educationalresourceid", "objectid", JoinOperator.LeftOuter);
                educationalResourceNotes.Columns = new ColumnSet("annotationid", "mimetype", "filename");
                educationalResourceNotes.LinkCriteria.AddCondition("isdocument", ConditionOperator.Equal, true);
                educationalResourceNotes.LinkCriteria.AddCondition("objecttypecode", ConditionOperator.Equal, "mzk_educationalresource");
                educationalResourceNotes.EntityAlias = "educationalResourceNotes";

                educationalResourceQuery.LinkEntities.Add(educationalResourceNotes);


                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                EntityCollection entityCollection = entityRepository.GetEntityCollection(query);

                foreach (Entity entity in entityCollection.Entities)
                {
                    EducationalResource model = new EducationalResource();
                    mzk_patientencountereducationalresource mzk_patientencountereducationalresource = (mzk_patientencountereducationalresource)entity;

                    if (entity.Attributes.Contains("mzk_patientencountereducationalresourceid"))
                        model.patientEncounterEducationalResourceId = mzk_patientencountereducationalresource.mzk_patientencountereducationalresourceId.ToString();

                    if (entity.Attributes.Contains("mzk_patientencounter"))
                        model.patientEncounterId = mzk_patientencountereducationalresource.mzk_patientencounter.Id.ToString();

                    if (entity.Attributes.Contains("mzk_customer"))
                        model.patientId = mzk_patientencountereducationalresource.mzk_customer.Id.ToString();

                    if (entity.Attributes.Contains("mzk_educationalresource"))
                    {
                        Notes resourceNotes = new Notes();

                        if (entity != null)
                        {
                            resourceNotes.Id = entity.Attributes.Contains("educationalResourceNotes.annotationid") ? (entity.Attributes["educationalResourceNotes.annotationid"] as AliasedValue).Value.ToString() : string.Empty;
                            resourceNotes.fileName = entity.Attributes.Contains("educationalResourceNotes.filename") ? (entity.Attributes["educationalResourceNotes.filename"] as AliasedValue).Value.ToString() : string.Empty;
                            resourceNotes.mimeType = entity.Attributes.Contains("educationalResourceNotes.mimetype") ? (entity.Attributes["educationalResourceNotes.mimetype"] as AliasedValue).Value.ToString() : string.Empty;
                        }
                        model.note = resourceNotes;
                    }

                    else
                    {
                        Notes resourceNotes = new Notes();

                        if (entity != null)
                        {
                            resourceNotes.Id = entity.Attributes.Contains("patientEncounterEducationalResourceNotes.annotationid") ? (entity.Attributes["patientEncounterEducationalResourceNotes.annotationid"] as AliasedValue).Value.ToString() : string.Empty;
                            resourceNotes.fileName = entity.Attributes.Contains("patientEncounterEducationalResourceNotes.filename") ? (entity.Attributes["patientEncounterEducationalResourceNotes.filename"] as AliasedValue).Value.ToString() : string.Empty;
                            resourceNotes.mimeType = entity.Attributes.Contains("patientEncounterEducationalResourceNotes.mimetype") ? (entity.Attributes["patientEncounterEducationalResourceNotes.mimetype"] as AliasedValue).Value.ToString() : string.Empty;
                        }

                        model.note = resourceNotes;
                    }
                    EducationalResource.Add(model);
                }
                return EducationalResource;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> removePatientEncounterEducationalResource(List<EducationalResource> educationalResourceList)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                foreach (EducationalResource resource in educationalResourceList)
                {
                    entityRepository.DeleteEntity("mzk_patientencountereducationalresource", new Guid(resource.patientEncounterEducationalResourceId));
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> addPatientEncounterEducationalResource(List<EducationalResource> educationalResourceList)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                foreach (EducationalResource resource in educationalResourceList)
                {
                    mzk_patientencountereducationalresource mzk_patientencountereducationalresource = new mzk_patientencountereducationalresource();

                    mzk_patientencountereducationalresource.mzk_name = resource.name;
                    if (!string.IsNullOrEmpty(resource.patientEncounterId))
                    {
                        mzk_patientencountereducationalresource.mzk_patientencounter = new EntityReference("mzk_patientencounter", new Guid(resource.patientEncounterId));
                    }
                    if (!string.IsNullOrEmpty(resource.patientId))
                    {
                        mzk_patientencountereducationalresource.mzk_customer = new EntityReference("contact", new Guid(resource.patientId));
                    }

                    if (!string.IsNullOrEmpty(resource.educationalResourceId))
                    {
                        mzk_patientencountereducationalresource.mzk_educationalresource = new EntityReference("mzk_educationalresource", new Guid(resource.educationalResourceId));
                        entityRepository.CreateEntity(mzk_patientencountereducationalresource);
                    }
                    else
                    {
                        patientEncounterEducationalResourceId = Convert.ToString(entityRepository.CreateEntity(mzk_patientencountereducationalresource));

                        resource.note.EntityType = mzk_patientencountereducationalresource.EntityLogicalName;
                        resource.note.ObjectId = patientEncounterEducationalResourceId;

                        Notes notes = new Notes();
                        string x = await notes.AddNotes(resource.note);
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
