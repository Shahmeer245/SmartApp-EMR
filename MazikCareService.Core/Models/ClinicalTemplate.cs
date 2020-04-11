using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class ClinicalTemplate
    {

        private const string dummyTemplateID = "98765432-9876-9876-98765432";
        public string name
        {
            get; set;
        }

        public string Id
        {
            get; set;
        }

        public int agegroup
        {
            get; set;
        }

        public bool showHeading
        {
            get; set;
        }

        public List<ClinicalTemplateSection> breadcrum
        {
            get; set;
        }

        public List<ClinicalTemplateSection> sections
        {
            get; set;
        }

        public string CPSOid
        {
            get; set;
        }

        public async Task<List<ClinicalTemplate>> getClinicalTemplatesList(string patientguid, string templateName = null, bool showDetails = false, string resourceId = null)
        {
            List<ClinicalTemplate> listModel = new List<ClinicalTemplate>();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_sonotesclinicaltemplate.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_sonotesclinicaltemplateid", "mzk_filteronage", "mzk_filterongender", "mzk_gender", "mzk_agevalidationid");
                LinkEntity EntityAgeValidation = new LinkEntity(mzk_mmtgroupcode.EntityLogicalName, mzk_agevalidation.EntityLogicalName, "mzk_agevalidationid", "mzk_agevalidationid", JoinOperator.LeftOuter);
                EntityAgeValidation.EntityAlias = "AgeValidation";
                EntityAgeValidation.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_agefromunit", "mzk_agefromvalue", "mzk_agetounit", "mzk_agetovalue");
                query.LinkEntities.Add(EntityAgeValidation);

                if (!string.IsNullOrEmpty(templateName))
                {
                    query.Criteria.AddCondition("mzk_description", ConditionOperator.Like, "%" + templateName + "%");
                }

                EntityCollection entitycollection = repo.GetEntityCollection(query);
                ClinicalTemplate model;

                User userModel = new User();

                List<Speciality> specialityList = null;

                if (resourceId != null)
                {
                    specialityList = userModel.getSpecialities(resourceId);
                }

                foreach (Entity entity in entitycollection.Entities)
                {
                    model = new ClinicalTemplate();

                    mzk_sonotesclinicaltemplate sonotesclinicaltemplate = (mzk_sonotesclinicaltemplate)entity;

                    if (!this.isValidCriteria(sonotesclinicaltemplate, patientguid, specialityList))
                    {
                        continue;
                    }

                    if (showDetails)
                    {
                        listModel.Add(await this.getTemplateDetails(sonotesclinicaltemplate.mzk_sonotesclinicaltemplateId.Value.ToString(), "", ""));
                    }
                    else
                    {
                        model.name = sonotesclinicaltemplate.mzk_Description;
                        model.Id = sonotesclinicaltemplate.mzk_sonotesclinicaltemplateId.Value.ToString();

                        listModel.Add(model);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listModel;
        }

        public async Task<bool> addClinicalTemplates(string patientguid, string encounterguid, List<ClinicalTemplate> templateList)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                mzk_patientsonotestemplate entity;

                foreach (ClinicalTemplate model in templateList)
                {
                    if (!string.IsNullOrEmpty(this.checkTemplateExist(patientguid, encounterguid, model.Id)))
                    {
                        throw new ValidationException(string.Format("Clinical template {0} already exist", model.name));
                    }

                    entity = new mzk_patientsonotestemplate();

                    entity.mzk_customerid = new EntityReference(Contact.EntityLogicalName, new Guid(patientguid));
                    entity.mzk_PatientEncounter = new EntityReference(mzk_patientencounter.EntityLogicalName, new Guid(encounterguid));
                    entity.mzk_Clinicaltemplate = new EntityReference(mzk_sonotesclinicaltemplate.EntityLogicalName, new Guid(model.Id));

                    repo.CreateEntity(entity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public async Task<bool> addClinicalTemplatesAndInsertFindings(string patientguid, string encounterguid, List<ClinicalTemplate> templateList)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                mzk_patientsonotestemplate entity;
                Guid patientsonotestemplateGUID = new Guid();
                string patientTemplateGuid = "";

                foreach (ClinicalTemplate model in templateList)
                {
                    patientTemplateGuid = "";
                    patientTemplateGuid = this.checkTemplateExist(patientguid, encounterguid, model.Id);

                    if (patientTemplateGuid == "")
                    {
                        entity = new mzk_patientsonotestemplate();
                        entity.mzk_customerid = new EntityReference(Contact.EntityLogicalName, new Guid(patientguid));
                        entity.mzk_PatientEncounter = new EntityReference(mzk_patientencounter.EntityLogicalName, new Guid(encounterguid));
                        entity.mzk_Clinicaltemplate = new EntityReference(mzk_sonotesclinicaltemplate.EntityLogicalName, new Guid(model.Id));

                        patientsonotestemplateGUID = await this.createClinicalTemplate(entity, repo);
                    }
                    else
                    {
                        patientsonotestemplateGUID = new Guid(patientTemplateGuid);
                    }

                    this.createPatientSONotesFinding(patientsonotestemplateGUID, model.CPSOid);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public async Task<bool> addAnEmptyClinicalTemplateAndMultipleFindings(string patientguid, string encounterguid, List<ClinicalTemplate> templateList)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                mzk_patientsonotestemplate entity;
                entity = new mzk_patientsonotestemplate();
                int counter = 0;
                Guid patientsonotestemplateGUID = new Guid();
                foreach (ClinicalTemplate model in templateList)
                {
                    if (counter == 0)
                    {
                        entity.mzk_customerid = new EntityReference(Contact.EntityLogicalName, new Guid(patientguid));
                        entity.mzk_PatientEncounter = new EntityReference(mzk_patientencounter.EntityLogicalName, new Guid(encounterguid));
                        patientsonotestemplateGUID = await this.createClinicalTemplate(entity, repo);
                    }

                    this.createPatientSONotesFinding(patientsonotestemplateGUID, model.CPSOid);
                    counter++;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        private async Task<Guid> createClinicalTemplate(mzk_patientsonotestemplate entity, SoapEntityRepository repo)
        {

            Guid patientsonotestemplateGUID = repo.CreateEntity(entity);
            return patientsonotestemplateGUID;
        }

        public async Task<bool> removeClinicalTemplates(string patientguid, string encounterguid, List<ClinicalTemplate> templateList)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                string patientsonotestemplateId = string.Empty;

                foreach (ClinicalTemplate model in templateList)
                {
                    patientsonotestemplateId = this.checkTemplateExist(patientguid, encounterguid, model.Id);

                    if (!string.IsNullOrEmpty(patientsonotestemplateId))
                    {
                        repo.DeleteEntity(mzk_patientsonotestemplate.EntityLogicalName, new Guid(patientsonotestemplateId));
                    }
                    else
                    {
                        throw new ValidationException(string.Format("Clinical template {0} does not exist", model.name));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        private string checkTemplateExist(string patientguid, string patientEncounter, string templateId)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            QueryExpression query = new QueryExpression(mzk_patientsonotestemplate.EntityLogicalName);

            query.ColumnSet = new ColumnSet("mzk_patientsonotestemplateid");
            query.Criteria.AddCondition("mzk_customerid", ConditionOperator.Equal, new Guid(patientguid));
            query.Criteria.AddCondition("mzk_patientencounter", ConditionOperator.Equal, new Guid(patientEncounter));

            if (!string.IsNullOrEmpty(templateId))
            {
                query.Criteria.AddCondition("mzk_clinicaltemplate", ConditionOperator.Equal, new Guid(templateId));
            }

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null)
            {
                if (entitycollection.Entities.Count == 0)
                {
                    return "";
                }
                else
                {
                    return (entitycollection.Entities[0]["mzk_patientsonotestemplateid"]).ToString();
                }
            }
            else
            {
                return "";
            }
        }

        public async Task<ClinicalTemplate> getTemplateDetails(string templateGuid, string patientguid, string encounterguid)
        {
            ClinicalTemplate model = new ClinicalTemplate();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                Entity entityTemplate = repo.GetEntity(mzk_sonotesclinicaltemplate.EntityLogicalName, new Guid(templateGuid), new ColumnSet(true));

                if (entityTemplate != null && entityTemplate.Id.ToString() != string.Empty)
                {
                    mzk_sonotesclinicaltemplate sonotesclinicaltemplate = (mzk_sonotesclinicaltemplate)entityTemplate;

                    model.Id = sonotesclinicaltemplate.mzk_sonotesclinicaltemplateId.Value.ToString();
                    model.name = sonotesclinicaltemplate.mzk_Description;
                    model.showHeading = sonotesclinicaltemplate.mzk_ShowHeading.Value;

                    List<ClinicalTemplateNarration> patientNarration = this.getPatientsClinicalTempalteNarration(patientguid, encounterguid, model.Id);

                    QueryExpression query = new QueryExpression(mzk_sonotesclinicaltemplatesection.EntityLogicalName);

                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                    query.Criteria.AddCondition("mzk_clinicaltemplate", ConditionOperator.Equal, new Guid(model.Id));
                    query.AddOrder("mzk_ordering", OrderType.Ascending);

                    EntityCollection entitycollection = repo.GetEntityCollection(query);

                    List<ClinicalTemplateSection> sections = new List<ClinicalTemplateSection>();

                    foreach (Entity entity in entitycollection.Entities)
                    {
                        mzk_sonotesclinicaltemplatesection sonotesclinicaltemplatesection = (mzk_sonotesclinicaltemplatesection)entity;

                        ClinicalTemplateSection modelSection = new ClinicalTemplateSection();

                        modelSection.Id = sonotesclinicaltemplatesection.mzk_sonotesclinicaltemplatesectionId.Value.ToString();
                        modelSection.Name = sonotesclinicaltemplatesection.mzk_sectionname;
                        modelSection.ParentId = sonotesclinicaltemplatesection.mzk_ParentSection != null ? sonotesclinicaltemplatesection.mzk_ParentSection.Id.ToString() : "";
                        modelSection.showHeading = sonotesclinicaltemplatesection.mzk_Showheading.Value;

                        query = new QueryExpression(mzk_sonotessectionfinding.EntityLogicalName);

                        query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                        query.Criteria.AddCondition("mzk_templatesection", ConditionOperator.Equal, new Guid(modelSection.Id));

                        LinkEntity queryMmt = new LinkEntity("mzk_sonotessectionfinding", "mzk_mmtcode", "mzk_measurement", "mzk_mmtcodeid", JoinOperator.Inner);
                        queryMmt.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                        queryMmt.EntityAlias = "queryMmt";

                        query.LinkEntities.Add(queryMmt);

                        EntityCollection entitycollectionChild = repo.GetEntityCollection(query);

                        List<ClinicalTemplateQuestion> questions = new List<ClinicalTemplateQuestion>();

                        foreach (Entity entityChld in entitycollectionChild.Entities)
                        {
                            mzk_sonotessectionfinding sonotessectionfinding = (mzk_sonotessectionfinding)entityChld;

                            ClinicalTemplateQuestion modelQuestion = new ClinicalTemplateQuestion();

                            modelQuestion.Id = sonotessectionfinding.mzk_sonotessectionfindingId.Value.ToString();
                            modelQuestion.ParentId = sonotessectionfinding.mzk_ParentFinding != null ? sonotessectionfinding.mzk_ParentFinding.Id.ToString() : "";
                            if (sonotessectionfinding.Attributes.Contains("queryMmt.mzk_description"))
                                modelQuestion.question = (sonotessectionfinding.Attributes["queryMmt.mzk_description"] as AliasedValue).Value.ToString();
                            modelQuestion.SecId = modelSection.Id;
                            if (sonotessectionfinding.Attributes.Contains("queryMmt.mzk_paragraph"))
                                modelQuestion.isParagraph = sonotessectionfinding.mzk_Paragraph.Value;
                            if (sonotessectionfinding.Attributes.Contains("queryMmt.mzk_datatype"))
                                modelQuestion.soapType = ((OptionSetValue)((AliasedValue)sonotessectionfinding.Attributes["queryMmt.mzk_datatype"]).Value).Value;
                            modelQuestion.templateId = model.Id;

                            if (sonotessectionfinding.Attributes.Contains("queryMmt.mzk_responsetype"))
                                modelQuestion.responseType = ((OptionSetValue)((AliasedValue)sonotessectionfinding.Attributes["queryMmt.mzk_responsetype"]).Value).Value;

                            if (sonotessectionfinding.mzk_measurement != null)
                                modelQuestion.measurementId = sonotessectionfinding.mzk_measurement.Id.ToString();

                            query = new QueryExpression(mzk_measurementcodenarration.EntityLogicalName);

                            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                            query.Criteria.AddCondition("mzk_measurementcode", ConditionOperator.Equal, new Guid(modelQuestion.measurementId));                            

                            EntityCollection entitycollectionChildNarration = repo.GetEntityCollection(query);

                            foreach (Entity entityChldNarration in entitycollectionChildNarration.Entities)
                            {
                                mzk_measurementcodenarration measurementcodenarration = (mzk_measurementcodenarration)entityChldNarration;

                                ClinicalTemplateNarration modelnarration = new ClinicalTemplateNarration();

                                modelnarration.Id = measurementcodenarration.mzk_measurementcodenarrationId.Value.ToString();
                                modelnarration.narrativeText = measurementcodenarration.mzk_name;

                                if (sonotessectionfinding.Attributes.Contains("queryMmt.mzk_inputtype"))
                                {
                                    modelnarration.type = ((OptionSetValue)((AliasedValue)sonotessectionfinding.Attributes["queryMmt.mzk_inputtype"]).Value).Value;
                                }

                                if (patientNarration.Any(item => item.ansIn == modelnarration.Id))
                                {
                                    ClinicalTemplateNarration currentNarr = patientNarration.Where(item => item.ansIn == modelnarration.Id).First();
                                    modelnarration.comments = currentNarr.comments;
                                    modelnarration.ans = currentNarr.ans;
                                    modelnarration.ansIn = modelnarration.Id;
                                    modelnarration.ansList = currentNarr.ansList;
                                    modelnarration.patientFindingId = currentNarr.patientFindingId;
                                    modelnarration.choicesName = currentNarr.choicesName;
                                }

                                switch ((mzk_narrationtype)measurementcodenarration.mzk_narrationtype.Value)
                                {
                                    case mzk_narrationtype.Positive:
                                        modelQuestion.yes = modelnarration;
                                        if (modelnarration.ansIn != null && modelnarration.ansIn != string.Empty)
                                        {
                                            modelQuestion.ansIn = "yes";
                                        }
                                        break;
                                    case mzk_narrationtype.Negative:
                                        modelQuestion.no = modelnarration;
                                        if (modelnarration.ansIn != null && modelnarration.ansIn != string.Empty)
                                        {
                                            modelQuestion.ansIn = "no";
                                        }
                                        break;
                                    case mzk_narrationtype.NoSelection:
                                        modelQuestion.noSelection = modelnarration;
                                        if (modelnarration.ansIn != null && modelnarration.ansIn != string.Empty)
                                        {
                                            modelQuestion.ansIn = "noSelection";
                                        }
                                        break;
                                }
                                                                
                                if ((mzk_narrationtype)measurementcodenarration.mzk_narrationtype.Value == mzk_narrationtype.Positive)
                                {        
                                    List<NarrationChoices> choices = new List<NarrationChoices>();

                                    if (modelnarration.type == (int)mzk_mmtcodemzk_Inputtype.Dropdown || modelnarration.type == (int)mzk_mmtcodemzk_Inputtype.Dropdown_MultiSelect)
                                    {
                                        query = new QueryExpression(mzk_measurementchoice.EntityLogicalName);

                                        query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_measurementchoiceid", "mzk_name", "entityimage");
                                        query.AddOrder("mzk_ordering", OrderType.Ascending);
                                        query.Criteria.AddCondition("mzk_measurementcode", ConditionOperator.Equal, new Guid(modelQuestion.measurementId));

                                        EntityCollection entitycollectionChildChoices = repo.GetEntityCollection(query);

                                        foreach (Entity entityChldChoices in entitycollectionChildChoices.Entities)
                                        {
                                            mzk_measurementchoice mmtChoice = (mzk_measurementchoice)entityChldChoices;
                                            NarrationChoices modelChoices = new NarrationChoices();

                                            if (entityChldChoices.Attributes.Contains("mzk_measurementchoiceid"))
                                            {
                                                modelChoices.Id = entityChldChoices.Attributes["mzk_measurementchoiceid"].ToString();
                                            }
                                            if (entityChldChoices.Attributes.Contains("mzk_name"))
                                            {
                                                modelChoices.name = entityChldChoices.Attributes["mzk_name"].ToString();
                                            }
                                            if (entityChldChoices.Attributes.Contains("entityimage"))
                                            {
                                                byte[] imageBytes = entityChldChoices.Attributes["entityimage"] as byte[];

                                                modelChoices.Image = Convert.ToBase64String(imageBytes);
                                            }

                                            choices.Add(modelChoices);
                                        }
                                        modelnarration.choices = choices;
                                    }                                    
                                }                                
                            }
                            questions.Add(modelQuestion);
                        }

                        modelSection.questions = questions;

                        sections.Add(modelSection);
                    }

                    model.sections = sections;
                    model.breadcrum = sections;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }

        public async Task<ClinicalTemplate> getTemplateDetails(string patientguid, string encounterguid)
        {
            ClinicalTemplate model = new ClinicalTemplate();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                List<ClinicalTemplateNarration> patientNarration = this.getPatientsClinicalTempalteNarration(patientguid, encounterguid, null);

                List<ClinicalTemplateSection> sections = new List<ClinicalTemplateSection>();

                QueryExpression query = new QueryExpression(mzk_patientsonotestemplate.EntityLogicalName);

                query.ColumnSet = new ColumnSet(true);

                if (!string.IsNullOrEmpty(patientguid))
                {
                    query.Criteria.AddCondition("mzk_customerid", ConditionOperator.Equal, new Guid(patientguid));
                }

                if (!string.IsNullOrEmpty(encounterguid))
                {
                    query.Criteria.AddCondition("mzk_patientencounter", ConditionOperator.Equal, new Guid(encounterguid));
                }

                query.Criteria.AddCondition("mzk_clinicaltemplate", ConditionOperator.Null);

                LinkEntity patientSONotesFinding = new LinkEntity(mzk_patientsonotestemplate.EntityLogicalName, mzk_patientsonotesfinding.EntityLogicalName, "mzk_patientsonotestemplateid", "mzk_patientsonotestemplate", JoinOperator.Inner);
                patientSONotesFinding.Columns = new ColumnSet(true);
                patientSONotesFinding.EntityAlias = "patientSONotesFinding";

                query.LinkEntities.Add(patientSONotesFinding);

                LinkEntity casePathwayStateOutcome = new LinkEntity(mzk_patientsonotesfinding.EntityLogicalName, mzk_casepathwaystateoutcome.EntityLogicalName, "mzk_casepathwaystateoutcome", "mzk_casepathwaystateoutcomeid", JoinOperator.Inner);
                casePathwayStateOutcome.Columns = new ColumnSet(true);
                casePathwayStateOutcome.EntityAlias = "casePathwayStateOutcome";

                patientSONotesFinding.LinkEntities.Add(casePathwayStateOutcome);

                LinkEntity soNotesClinicalTemplateSection = new LinkEntity(mzk_casepathwaystateoutcome.EntityLogicalName, mzk_sonotesclinicaltemplatesection.EntityLogicalName, "mzk_sonotesclinicaltemplatesection", "mzk_sonotesclinicaltemplatesectionid", JoinOperator.LeftOuter);
                soNotesClinicalTemplateSection.Columns = new ColumnSet(true);
                soNotesClinicalTemplateSection.EntityAlias = "soNotesClinicalTemplateSection";

                casePathwayStateOutcome.LinkEntities.Add(soNotesClinicalTemplateSection);

                LinkEntity measurementCode = new LinkEntity(mzk_casepathwaystateoutcome.EntityLogicalName, mzk_mmtcode.EntityLogicalName, "mzk_measurement", "mzk_mmtcodeid", JoinOperator.Inner);
                measurementCode.Columns = new ColumnSet(true);
                measurementCode.EntityAlias = "measurementCode";

                casePathwayStateOutcome.LinkEntities.Add(measurementCode);

                EntityCollection entityCollection = repo.GetEntityCollection(query);

                foreach (Entity entity in entityCollection.Entities)
                {
                    ClinicalTemplateSection modelSection = new ClinicalTemplateSection();

                    if (entity.Attributes.Contains("soNotesClinicalTemplateSection.mzk_sonotesclinicaltemplatesectionid"))
                    {
                        modelSection.Id = (entity.Attributes["soNotesClinicalTemplateSection.mzk_sonotesclinicaltemplatesectionid"] as AliasedValue).Value.ToString();

                        if (entity.Attributes.Contains("soNotesClinicalTemplateSection.mzk_sectionname"))
                            modelSection.Name = (entity.Attributes["soNotesClinicalTemplateSection.mzk_sectionname"] as AliasedValue).Value.ToString();
                        if (entity.Attributes.Contains("soNotesClinicalTemplateSection.mzk_parentsection"))
                            modelSection.ParentId = (entity.Attributes["soNotesClinicalTemplateSection.mzk_parentsection"] as AliasedValue).Value.ToString();
                        if (entity.Attributes.Contains("soNotesClinicalTemplateSection.mzk_showheading"))
                            modelSection.showHeading = Convert.ToBoolean((entity.Attributes["soNotesClinicalTemplateSection.mzk_showheading"] as AliasedValue).Value);
                    }

                    string measurementId = (entity.Attributes["measurementCode.mzk_mmtcodeid"] as AliasedValue).Value.ToString();

                    query = new QueryExpression(mzk_sonotessectionfinding.EntityLogicalName);

                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                    if (!string.IsNullOrEmpty(modelSection.Id))
                    {
                        query.Criteria.AddCondition("mzk_templatesection", ConditionOperator.Equal, new Guid(modelSection.Id));
                    }

                    query.Criteria.AddCondition("mzk_measurement", ConditionOperator.Equal, new Guid(measurementId));
                    
                    EntityCollection entitycollectionChild = repo.GetEntityCollection(query);

                        List<ClinicalTemplateQuestion> questions = new List<ClinicalTemplateQuestion>();

                        foreach (Entity entityChild in entitycollectionChild.Entities)
                        {
                            mzk_sonotessectionfinding sonotessectionfinding = (mzk_sonotessectionfinding)entityChild;

                            ClinicalTemplateQuestion modelQuestion = new ClinicalTemplateQuestion();

                            modelQuestion.Id = sonotessectionfinding.mzk_sonotessectionfindingId.Value.ToString();
                            modelQuestion.ParentId = sonotessectionfinding.mzk_ParentFinding != null ? sonotessectionfinding.mzk_ParentFinding.Id.ToString() : "";
                            if (entity.Attributes.Contains("measurementCode.mzk_description"))
                                modelQuestion.question = (entity.Attributes["measurementCode.mzk_description"] as AliasedValue).Value.ToString();
                        if (!string.IsNullOrEmpty(modelSection.Id))
                        {
                            modelQuestion.SecId = modelSection.Id;
                        }
                            if (sonotessectionfinding.Attributes.Contains("queryMmt.mzk_paragraph"))
                                modelQuestion.isParagraph = sonotessectionfinding.mzk_Paragraph.Value;
                            if (entity.Attributes.Contains("measurementCode.mzk_datatype"))
                                modelQuestion.soapType = ((OptionSetValue)((AliasedValue)entity.Attributes["measurementCode.mzk_datatype"]).Value).Value;
                            //modelQuestion.SubSecId = modelSection.Id;
                            //modelQuestion.templateId = model.Id;

                            if (sonotessectionfinding.Attributes.Contains("queryMmt.mzk_responsetype"))
                                modelQuestion.responseType = ((OptionSetValue)((AliasedValue)sonotessectionfinding.Attributes["queryMmt.mzk_responsetype"]).Value).Value;

                            if (sonotessectionfinding.mzk_measurement != null)
                                modelQuestion.measurementId = sonotessectionfinding.mzk_measurement.Id.ToString();
                            
                            query = new QueryExpression(mzk_measurementcodenarration.EntityLogicalName);

                            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                            query.Criteria.AddCondition("mzk_measurementcode", ConditionOperator.Equal, new Guid(modelQuestion.measurementId));

                            EntityCollection entityCollectionChildNarration = repo.GetEntityCollection(query);

                            foreach (Entity entityChildNarration in entityCollectionChildNarration.Entities)
                            {
                                mzk_measurementcodenarration measurementcodenarration = (mzk_measurementcodenarration)entityChildNarration;

                                ClinicalTemplateNarration modelnarration = new ClinicalTemplateNarration();

                                modelnarration.Id = measurementcodenarration.mzk_measurementcodenarrationId.Value.ToString();
                                modelnarration.narrativeText = measurementcodenarration.mzk_name;

                                if (entity.Attributes.Contains("measurementCode.mzk_inputtype"))
                                {
                                    modelnarration.type = ((OptionSetValue)((AliasedValue)entity.Attributes["measurementCode.mzk_inputtype"]).Value).Value;
                                }
                                if (patientNarration.Any(item => item.ansIn == modelnarration.Id))
                                {
                                    ClinicalTemplateNarration currentNarr = patientNarration.Where(item => item.ansIn == modelnarration.Id).First();
                                    modelnarration.comments = currentNarr.comments;
                                    modelnarration.ans = currentNarr.ans;
                                    modelnarration.ansIn = modelnarration.Id;
                                    modelnarration.ansList = currentNarr.ansList;
                                    modelnarration.patientFindingId = currentNarr.patientFindingId;
                                    modelnarration.choicesName = currentNarr.choicesName;
                                }

                                switch ((mzk_narrationtype)measurementcodenarration.mzk_narrationtype.Value)
                                {
                                    case mzk_narrationtype.Positive:
                                        modelQuestion.yes = modelnarration;
                                        if (modelnarration.ansIn != null && modelnarration.ansIn != string.Empty)
                                        {
                                            modelQuestion.ansIn = "yes";
                                        }
                                        break;
                                    case mzk_narrationtype.Negative:
                                        modelQuestion.no = modelnarration;
                                        if (modelnarration.ansIn != null && modelnarration.ansIn != string.Empty)
                                        {
                                            modelQuestion.ansIn = "no";
                                        }
                                        break;                                    
                                    case mzk_narrationtype.NoSelection:
                                        modelQuestion.noSelection = modelnarration;
                                        if (modelnarration.ansIn != null && modelnarration.ansIn != string.Empty)
                                        {
                                            modelQuestion.ansIn = "noSelection";
                                        }
                                        break;
                                }

                                if ((mzk_narrationtype)measurementcodenarration.mzk_narrationtype.Value == mzk_narrationtype.Positive)
                                {
                                    List<NarrationChoices> choices = new List<NarrationChoices>();

                                if (modelnarration.type == (int)mzk_mmtcodemzk_Inputtype.Dropdown || modelnarration.type == (int)mzk_mmtcodemzk_Inputtype.Dropdown_MultiSelect)
                                {
                                        query = new QueryExpression(mzk_measurementchoice.EntityLogicalName);

                                        query.ColumnSet = new ColumnSet("mzk_measurementchoiceid", "mzk_name", "entityimage");
                                        query.AddOrder("mzk_ordering", OrderType.Ascending);
                                        query.Criteria.AddCondition("mzk_measurementcode", ConditionOperator.Equal, new Guid(modelQuestion.measurementId));

                                        EntityCollection entityCollectionChildChoices = repo.GetEntityCollection(query);

                                        foreach (Entity entityChildChoices in entityCollectionChildChoices.Entities)
                                        {
                                            mzk_measurementchoice mmtChoice = (mzk_measurementchoice)entityChildChoices;
                                            NarrationChoices modelChoices = new NarrationChoices();

                                            if (entityChildChoices.Attributes.Contains("mzk_measurementchoiceid"))
                                            {
                                                modelChoices.Id = entityChildChoices.Attributes["mzk_measurementchoiceid"].ToString();
                                            }
                                            if (entityChildChoices.Attributes.Contains("mzk_name"))
                                            {
                                                modelChoices.name = entityChildChoices.Attributes["mzk_name"].ToString();
                                            }
                                            if (entityChildChoices.Attributes.Contains("entityimage"))
                                            {
                                                byte[] imageBytes = entityChildChoices.Attributes["entityimage"] as byte[];

                                                modelChoices.Image = Convert.ToBase64String(imageBytes);
                                            }

                                            choices.Add(modelChoices);
                                        }
                                        modelnarration.choices = choices;
                                    }
                                }
                            }
                            questions.Add(modelQuestion);
                        }

                        modelSection.questions = questions;
                        sections.Add(modelSection);
                }

                model.sections = sections;
                model.breadcrum = sections;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }

        public async Task<bool> saveTemplateDetails(List<ClinicalTemplateNarration> clinicalTemplateNarration, string patientguid, string encounterguid)
        {
            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                mzk_patientsonotesfinding entity;
                mzk_patientsonotesfindingchoice choice;
                Guid notesFindingId = new Guid();
                List<ClinicalTemplateNarration> patientNarration = new List<ClinicalTemplateNarration>();
                
                List<string> uniqueTemplateIdList = clinicalTemplateNarration.Select(id => id.templateId).Distinct().ToList();

                foreach (string templateId in uniqueTemplateIdList)
                {
                    patientNarration = this.getPatientsClinicalTempalteNarration(patientguid, encounterguid, templateId);

                    string patientTemplateGuid;

                    patientTemplateGuid = this.checkTemplateExist(patientguid, encounterguid, templateId);

                    foreach (ClinicalTemplateNarration model in clinicalTemplateNarration)
                    {
                        if (model.ansIn != null && model.ansIn != string.Empty)
                        {
                            if (patientNarration.Any(item => item.patientFindingId == model.patientFindingId))
                            {
                                ClinicalTemplateNarration currentNarr = patientNarration.Where(item => item.patientFindingId == model.patientFindingId).First();

                                entity = (mzk_patientsonotesfinding)repo.GetEntity(mzk_patientsonotesfinding.EntityLogicalName, new Guid(model.patientFindingId), new ColumnSet(true));

                                entity.mzk_Comments = model.comments;

                                if (string.IsNullOrEmpty(currentNarr.ansIn) && currentNarr.ansIn != model.ansIn)
                                {
                                    entity.mzk_measurementcodenarration = new EntityReference(mzk_measurementcodenarration.EntityLogicalName, new Guid(model.ansIn));
                                }

                                QueryExpression query = new QueryExpression();

                                query.EntityName = mzk_patientsonotesfindingchoice.EntityLogicalName;

                                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientsonotesfindingchoiceid");

                                query.Criteria.AddCondition("mzk_patientsonotesfinding", ConditionOperator.Equal, new Guid(model.patientFindingId));

                                EntityCollection entityCollection = repo.GetEntityCollection(query);

                                foreach (Entity entityChild in entityCollection.Entities)
                                {
                                    repo.DeleteEntity(mzk_patientsonotesfindingchoice.EntityLogicalName, new Guid(entityChild.Attributes["mzk_patientsonotesfindingchoiceid"].ToString()));
                                }

                                if (currentNarr.type == (int)mzk_mmtcodemzk_Inputtype.ImageUpload)
                                {
                                    if (model.type != (int)mzk_mmtcodemzk_Inputtype.ImageUpload || string.IsNullOrEmpty(model.ans) || model.ans != entity.mzk_Value)
                                    {
                                        query = new QueryExpression();

                                        query.EntityName = Annotation.EntityLogicalName;

                                        query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("annotationid");

                                        query.Criteria.AddCondition("objectid", ConditionOperator.Equal, new Guid(model.patientFindingId));
                                        query.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, mzk_patientsonotesfinding.EntityLogicalName);

                                        entityCollection = repo.GetEntityCollection(query);

                                        foreach (Entity entityChild in entityCollection.Entities)
                                        {
                                            repo.DeleteEntity(Annotation.EntityLogicalName, new Guid(entityChild.Attributes["annotationid"].ToString()));
                                        }
                                    }
                                }

                                if (model.type == (int)mzk_mmtcodemzk_Inputtype.Dropdown || (model.type == (int)mzk_mmtcodemzk_Inputtype.Dropdown_MultiSelect))
                                {
                                    entity.mzk_Value = "";

                                    repo.UpdateEntity(entity);

                                    foreach (string choiceValue in model.ansList)
                                    {
                                        choice = new mzk_patientsonotesfindingchoice();

                                        choice.mzk_PatientSONotesFinding = new EntityReference(mzk_patientsonotesfinding.EntityLogicalName, new Guid(model.patientFindingId));
                                        choice.mzk_measurementchoice = new EntityReference(mzk_measurementchoice.EntityLogicalName, new Guid(choiceValue));

                                        repo.CreateEntity(choice);
                                    }
                                }
                                else if (model.type == (int)mzk_mmtcodemzk_Inputtype.ImageUpload)
                                {
                                    entity.mzk_Value = model.ans;

                                    repo.UpdateEntity(entity);

                                    if (!MazikCareService.CRMRepository.Notes.addDocsinCRM(model.ans, Path.GetFileNameWithoutExtension(model.ans), "", model.file, model.mimeType, mzk_patientsonotesfinding.EntityLogicalName, new Guid(model.patientFindingId)))
                                    {
                                        throw new ValidationException("");
                                    }
                                }
                                else
                                {
                                    entity.mzk_Value = model.ans;

                                    repo.UpdateEntity(entity);
                                }

                                patientNarration.Remove(currentNarr);
                            }
                            else
                            {
                                entity = new mzk_patientsonotesfinding();

                               entity.mzk_PatientSONotesTemplate = new EntityReference(mzk_patientsonotestemplate.EntityLogicalName, new Guid(patientTemplateGuid));
                                entity.mzk_measurementcodenarration = new EntityReference(mzk_measurementcodenarration.EntityLogicalName, new Guid(model.ansIn));

                                entity.mzk_Comments = model.comments;

                                if (model.type == (int)mzk_mmtcodemzk_Inputtype.Dropdown || (model.type == (int)mzk_mmtcodemzk_Inputtype.Dropdown_MultiSelect))
                                {
                                    notesFindingId = repo.CreateEntity(entity);

                                    foreach (string choiceValue in model.ansList)
                                    {
                                        choice = new mzk_patientsonotesfindingchoice();

                                        choice.mzk_PatientSONotesFinding = new EntityReference(mzk_patientsonotesfinding.EntityLogicalName, notesFindingId);
                                        choice.mzk_measurementchoice = new EntityReference(mzk_measurementchoice.EntityLogicalName, new Guid(choiceValue));

                                        repo.CreateEntity(choice);
                                    }
                                }
                                else if (model.type == (int)mzk_mmtcodemzk_Inputtype.ImageUpload)
                                {
                                    entity.mzk_Value = model.ans;

                                    notesFindingId = repo.CreateEntity(entity);

                                    if (!MazikCareService.CRMRepository.Notes.addDocsinCRM(model.ans, Path.GetFileNameWithoutExtension(model.ans), "", model.file, model.mimeType, mzk_patientsonotesfinding.EntityLogicalName, notesFindingId))
                                    {
                                        throw new ValidationException("");
                                    }
                                }
                                else
                                {
                                    entity.mzk_Value = model.ans;

                                    notesFindingId = repo.CreateEntity(entity);
                                }
                            }
                        }
                    }

                    foreach (ClinicalTemplateNarration model in patientNarration)
                    {
                        repo.DeleteEntity(mzk_patientsonotesfinding.EntityLogicalName, new Guid(model.patientFindingId));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ClinicalTemplateNarration> getPatientsClinicalTempalteNarration(string patientguid, string encounterguid, string templateGuid, bool forHistory = false, int type = 0, bool forExistCheck = false, string searchFinding = "", string CasePathwayStateOutcomeStateID = "")
        {
            List<ClinicalTemplateNarration> listModel = new List<ClinicalTemplateNarration>();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(mzk_patientsonotesfinding.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                LinkEntity mzk_casepathwaystateoutcomeLinkEntity = new LinkEntity(mzk_patientsonotesfinding.EntityLogicalName, mzk_casepathwaystateoutcome.EntityLogicalName, "mzk_casepathwaystateoutcome", "mzk_casepathwaystateoutcomeid", JoinOperator.LeftOuter);

                mzk_casepathwaystateoutcomeLinkEntity.Columns = new ColumnSet(true);
                mzk_casepathwaystateoutcomeLinkEntity.EntityAlias = "mzk_casepathwaystateoutcome";

                if (CasePathwayStateOutcomeStateID != "")
                {
                    mzk_casepathwaystateoutcomeLinkEntity.LinkCriteria.AddCondition("mzk_casepathwaystate", ConditionOperator.Equal, new Guid(CasePathwayStateOutcomeStateID));                  
                }

                query.LinkEntities.Add(mzk_casepathwaystateoutcomeLinkEntity);
                

                LinkEntity patientSoNotesTemplate = new LinkEntity(mzk_patientsonotesfinding.EntityLogicalName, mzk_patientsonotestemplate.EntityLogicalName, "mzk_patientsonotestemplate", "mzk_patientsonotestemplateid", JoinOperator.Inner);
                patientSoNotesTemplate.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_clinicaltemplate");

                if (!string.IsNullOrEmpty(templateGuid))
                {
                    patientSoNotesTemplate.LinkCriteria.AddCondition("mzk_clinicaltemplate", ConditionOperator.Equal, new Guid(templateGuid));

                }

                if (!string.IsNullOrEmpty(patientguid))
                {
                    patientSoNotesTemplate.LinkCriteria.AddCondition("mzk_customerid", ConditionOperator.Equal, new Guid(patientguid));
                }

                if (encounterguid != null && encounterguid != string.Empty)
                {
                    patientSoNotesTemplate.LinkCriteria.AddCondition("mzk_patientencounter", ConditionOperator.Equal, new Guid(encounterguid));
                }

                query.LinkEntities.Add(patientSoNotesTemplate);

                LinkEntity patientSoNotesFindingChoice = new LinkEntity(mzk_patientsonotesfinding.EntityLogicalName, mzk_patientsonotesfindingchoice.EntityLogicalName, "mzk_patientsonotesfindingid", "mzk_patientsonotesfinding", JoinOperator.LeftOuter);
                patientSoNotesFindingChoice.Columns = new ColumnSet(true);
                patientSoNotesFindingChoice.EntityAlias = "patientSoNotesFindingChoice";

                query.LinkEntities.Add(patientSoNotesFindingChoice);

                LinkEntity measurementCodeNarration = new LinkEntity(mzk_patientsonotesfinding.EntityLogicalName, mzk_measurementcodenarration.EntityLogicalName, "mzk_measurementcodenarration", "mzk_measurementcodenarrationid", JoinOperator.LeftOuter);
                measurementCodeNarration.Columns = new ColumnSet(true);

                if (!string.IsNullOrEmpty(searchFinding))
                {
                    measurementCodeNarration.LinkCriteria.AddCondition("mzk_name", ConditionOperator.Like, "%" + searchFinding + "%");
                }
                measurementCodeNarration.EntityAlias = "measurementCodeNarration";

                query.LinkEntities.Add(measurementCodeNarration);

                LinkEntity measurementCode = new LinkEntity(mzk_measurementcodenarration.EntityLogicalName, mzk_mmtcode.EntityLogicalName, "mzk_measurementcode", "mzk_mmtcodeid", JoinOperator.LeftOuter);

                measurementCode.Columns = new ColumnSet(true);
                measurementCode.EntityAlias = "measurementCode";

                measurementCodeNarration.LinkEntities.Add(measurementCode);

                LinkEntity linkEntitychild = new LinkEntity(mzk_mmtcode.EntityLogicalName, mzk_sonotessectionfinding.EntityLogicalName, "mzk_mmtcodeid", "mzk_measurement", JoinOperator.LeftOuter);
                linkEntitychild.EntityAlias = mzk_sonotessectionfinding.EntityLogicalName;
                linkEntitychild.Columns = new ColumnSet(true);
                measurementCode.LinkEntities.Add(linkEntitychild);

                if (forHistory)
                { 
                    LinkEntity linkEntitychildSection = new LinkEntity(mzk_sonotessectionfinding.EntityLogicalName, mzk_sonotesclinicaltemplatesection.EntityLogicalName, "mzk_templatesection", "mzk_sonotesclinicaltemplatesectionid", JoinOperator.Inner);
                    linkEntitychildSection.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_historytype");
                    linkEntitychildSection.EntityAlias = mzk_sonotesclinicaltemplatesection.EntityLogicalName;
                    linkEntitychildSection.LinkCriteria.AddCondition("mzk_history", ConditionOperator.Equal, true);
                    linkEntitychildSection.LinkCriteria.AddCondition("mzk_historytype", ConditionOperator.Equal, type);

                    linkEntitychild.LinkEntities.Add(linkEntitychildSection);
                }

                EntityCollection entitycollectionPatient = repo.GetEntityCollection(query);

                ClinicalTemplateNarration model = null;
                string oldFindingId = "";
                List<string> choicesList = new List<string>();
                List<string> choicesName = new List<string>();

                foreach (Entity entity in entitycollectionPatient.Entities)
                {
                    mzk_patientsonotesfinding patientsonotesfinding = (mzk_patientsonotesfinding)entity;

                    if (oldFindingId != patientsonotesfinding.mzk_patientsonotesfindingId.Value.ToString())
                    {
                        if (model != null)
                        {
                            if (choicesList.Count > 0)
                            {
                                model.ansList = choicesList.ToArray();
                            }

                            if (choicesName.Count > 0)
                            {
                                model.choicesName = choicesName.ToArray();
                            }

                            listModel.Add(model);

                            if (forExistCheck)
                            {
                                break;
                            }
                        }

                        model = new ClinicalTemplateNarration();

                        if (entity.Attributes.Contains("mzk_casepathwaystateoutcome"))
                        {
                            model.mzk_casepathwaystateoutcomeStr = ((EntityReference)entity.Attributes["mzk_casepathwaystateoutcome"]).Id.ToString();

                            if (entity.Attributes.Contains("measurementCodeNarration.mzk_narrationtype"))
                            {
                                model.narrationType = ((Microsoft.Xrm.Sdk.OptionSetValue)(entity.Attributes["measurementCodeNarration.mzk_narrationtype"] as AliasedValue).Value).Value; //sonotesfindingnarration.mzk_NarrationType.Value; // from patient so notes finding.
                            }
                        }

                        if (entity.Attributes.Contains("mzk_casepathwaystateoutcome.mzk_measurement"))
                        {
                            model.measurementId = ((EntityReference)(entity.Attributes["mzk_casepathwaystateoutcome.mzk_measurement"] as AliasedValue).Value).Id.ToString();
                        }
                        else if (entity.Attributes.Contains("mzk_sonotessectionfinding.mzk_measurement"))
                        {
                            model.measurementId = ((EntityReference)(entity.Attributes["mzk_sonotessectionfinding.mzk_measurement"] as AliasedValue).Value).Id.ToString();
                        }

                        choicesList = new List<string>();
                        choicesName = new List<string>();

                        oldFindingId = patientsonotesfinding.mzk_patientsonotesfindingId.Value.ToString();

                        model.ansIn = patientsonotesfinding.mzk_measurementcodenarration != null ? patientsonotesfinding.mzk_measurementcodenarration.Id.ToString() : "";
                        model.comments = patientsonotesfinding.mzk_Comments;
                        model.ans = patientsonotesfinding.mzk_Value;
                        model.patientFindingId = patientsonotesfinding.mzk_patientsonotesfindingId.Value.ToString();
                        model.templateId = templateGuid;

                        if (entity.Attributes.Contains("measurementCode.mzk_inputtype") && (entity.Attributes["measurementCode.mzk_inputtype"] as AliasedValue) != null)
                        {
                            model.type = ((entity.Attributes["measurementCode.mzk_inputtype"] as AliasedValue).Value as OptionSetValue).Value;
                        }

                        if (forHistory)
                        {
                            if (entity.Attributes.Contains("mzk_sonotesclinicaltemplatesection.mzk_historytype") && (entity.Attributes["mzk_sonotesclinicaltemplatesection.mzk_historytype"] as AliasedValue) != null)
                            {
                                model.historyType = ((entity.Attributes["mzk_sonotesclinicaltemplatesection.mzk_historytype"] as AliasedValue).Value as OptionSetValue).Value;
                            }

                            if (entity.Attributes.Contains("measurementCodeNarration.mzk_name") && (entity.Attributes["measurementCodeNarration.mzk_name"] as AliasedValue) != null)
                            {
                                model.narrativeText = (entity.Attributes["measurementCodeNarration.mzk_name"] as AliasedValue).Value.ToString();
                            }

                            model.createdDateTime = patientsonotesfinding.CreatedOn.HasValue ? patientsonotesfinding.CreatedOn.Value : DateTime.MinValue;
                            model.userName = patientsonotesfinding.CreatedBy != null ? patientsonotesfinding.CreatedBy.Name : string.Empty;
                        }
                    }

                    if (entity.Attributes.Contains("patientSoNotesFindingChoice.mzk_measurementchoice"))
                    {
                        choicesList.Add(((EntityReference)(entity.Attributes["patientSoNotesFindingChoice.mzk_measurementchoice"] as AliasedValue).Value).Id.ToString());
                        choicesName.Add(((EntityReference)(entity.Attributes["patientSoNotesFindingChoice.mzk_measurementchoice"] as AliasedValue).Value).Name.ToString());
                    }
                }

                if (model != null)
                {
                    if (choicesList.Count > 0)
                    {
                        model.ansList = choicesList.ToArray();
                    }

                    if (choicesName.Count > 0)
                    {
                        model.choicesName = choicesName.ToArray();
                    }

                    listModel.Add(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listModel;
        }

        public async Task<List<ClinicalTemplate>> getPatientClinicalTemplates(string patientguid, string encounterguid)
        {
            List<ClinicalTemplate> listModel = new List<ClinicalTemplate>();

            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(mzk_patientsonotestemplate.EntityLogicalName);

                query.ColumnSet = new ColumnSet("mzk_clinicaltemplate");
                if (!string.IsNullOrEmpty(patientguid))
                    query.Criteria.AddCondition("mzk_customerid", ConditionOperator.Equal, new Guid(patientguid));
                if (!string.IsNullOrEmpty(encounterguid))
                    query.Criteria.AddCondition("mzk_patientencounter", ConditionOperator.Equal, new Guid(encounterguid));

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                foreach (mzk_patientsonotestemplate entity in entitycollection.Entities)
                {
                    if (entity.mzk_Clinicaltemplate != null)
                    {
                        listModel.Add(await this.getTemplateDetails(entity.mzk_Clinicaltemplate.Id.ToString(), patientguid, encounterguid));
                    }
                    else
                    {
                        listModel.Add(await this.getTemplateDetails(patientguid, encounterguid));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listModel;
        }

        public async Task<ClinicalTemplateNarration> getFile(string narrationGuid)
        {
            ClinicalTemplateNarration model = new ClinicalTemplateNarration();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(Annotation.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("documentbody", "mimetype");

                query.Criteria.AddCondition("objectid", ConditionOperator.Equal, new Guid(narrationGuid));
                query.Criteria.AddCondition("objecttypecode", ConditionOperator.Equal, mzk_patientsonotesfinding.EntityLogicalName);

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

        bool isValidCriteria(mzk_sonotesclinicaltemplate clinicalTemplate, string patientguid, List<Speciality> specialities)
        {
            bool ret = false;
            int patientGender = 0;
            DateTime patientBirthDate;
            AgeHelper ageHelper = new AgeHelper(DateTime.Now);
            Patient patient = new Patient();

            if (!string.IsNullOrEmpty(patientguid))
            {

                Patient patientData = new Patient().getPatientDetails(patientguid).Result;

                patientBirthDate = patientData.dateOfBirth;
                patientGender = patientData.genderValue;

                if (clinicalTemplate.mzk_FilteronGender.HasValue)
                {
                    if (clinicalTemplate.mzk_FilteronGender.Value)
                    {
                        if (clinicalTemplate.mzk_Gender != null)
                        {
                            if (patientGender == clinicalTemplate.mzk_Gender.Value)
                            {
                                ret = true;
                            }
                            else
                            {
                                ret = false;
                            }
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                    else
                    {
                        ret = true;
                    }
                }
                else
                {
                    ret = true;
                }

                if (ret)
                {
                    if (clinicalTemplate.mzk_FilteronAge.HasValue)
                    {
                        if (clinicalTemplate.mzk_FilteronAge.Value)
                        {
                            if (clinicalTemplate.mzk_AgeValidationId != null)
                            {
                                if (ageHelper.isAgeMatched(patientBirthDate, (Helper.Enum.DayWeekMthYr)((OptionSetValue)((AliasedValue)clinicalTemplate.Attributes["AgeValidation.mzk_agefromunit"]).Value).Value, (int)((AliasedValue)clinicalTemplate.Attributes["AgeValidation.mzk_agefromvalue"]).Value, (Helper.Enum.DayWeekMthYr)((OptionSetValue)((AliasedValue)clinicalTemplate.Attributes["AgeValidation.mzk_agetounit"]).Value).Value, (int)((AliasedValue)clinicalTemplate.Attributes["AgeValidation.mzk_agetovalue"]).Value))
                                {
                                    ret = true;
                                }
                                else
                                {
                                    ret = false;
                                }
                            }
                        }
                        else
                        {
                            ret = true;
                        }
                    }
                    else
                    {
                        ret = true;
                    }
                }
            }
            else
            {
                ret = true;
            }

            if (ret && specialities != null && specialities.Count > 0 && clinicalTemplate.mzk_speciality != null && clinicalTemplate.mzk_speciality.Id.ToString() != string.Empty)
            {
                ret = specialities.Any(item => item.SpecialityId == clinicalTemplate.mzk_speciality.Id.ToString());
            }

            return ret;
        }

        //public async Task<bool> saveClinicalTemplate2(ClinicalTemplate clinicalTemplate)
        //{
        //    ClinicalTemplate model = new ClinicalTemplate();

        //    try
        //    {
        //        SoapEntityRepository repo = SoapEntityRepository.GetService();

        //        mzk_sonotesclinicaltemplate sonotesclinicaltemplate = new mzk_sonotesclinicaltemplate();

        //        sonotesclinicaltemplate.mzk_Description = clinicalTemplate.name;
        //        sonotesclinicaltemplate.mzk_templateid = clinicalTemplate.Id;
        //        //sonotesclinicaltemplate.mzk_ResponseType = new OptionSetValue(clinicalTemplate.type);
        //        sonotesclinicaltemplate.mzk_ShowHeading = clinicalTemplate.showHeading;

        //        Guid clinicalTemplateGUID = repo.CreateEntity(sonotesclinicaltemplate);

        //        if (clinicalTemplateGUID != null && clinicalTemplateGUID != Guid.Empty)
        //        {
        //            List<ClinicalTemplateSection> sections = clinicalTemplate.sections;

        //            foreach (ClinicalTemplateSection modelSection in sections)
        //            {
        //                mzk_sonotesclinicaltemplatesection sonotesclinicaltemplatesection = new mzk_sonotesclinicaltemplatesection();

        //                sonotesclinicaltemplatesection.mzk_sectionname = modelSection.Name;
        //                sonotesclinicaltemplatesection.mzk_ParentSection = new EntityReference(mzk_sonotesclinicaltemplatesection.EntityLogicalName, new Guid(modelSection.ParentId));
        //                sonotesclinicaltemplatesection.mzk_Showheading = modelSection.showHeading;
        //                sonotesclinicaltemplatesection.mzk_ClinicalTemplate = new EntityReference(mzk_sonotesclinicaltemplate.EntityLogicalName, clinicalTemplateGUID);
        //                sonotesclinicaltemplatesection.mzk_History = modelSection.history;
        //                sonotesclinicaltemplatesection.mzk_HistoryType = new OptionSetValue(modelSection.historyType);

        //                Guid templateSectionGUID = repo.CreateEntity(sonotesclinicaltemplatesection);

        //                if (templateSectionGUID != null && templateSectionGUID != Guid.Empty)
        //                {
        //                    List<ClinicalTemplateQuestion> questions = modelSection.questions;

        //                    foreach (ClinicalTemplateQuestion modelQuestion in questions)
        //                    {
        //                        mzk_sonotessectionfinding sonotessectionfinding = new mzk_sonotessectionfinding();

        //                        sonotessectionfinding.mzk_ParentFinding = new EntityReference(mzk_sonotessectionfinding.EntityLogicalName, new Guid(modelQuestion.ParentId));
        //                        sonotessectionfinding.mzk_Description = modelQuestion.question;
        //                        sonotessectionfinding.mzk_Paragraph = modelQuestion.isParagraph;
        //                        sonotessectionfinding.mzk_TemplateSection = new EntityReference(mzk_sonotesclinicaltemplatesection.EntityLogicalName, templateSectionGUID);
        //                        sonotessectionfinding.mzk_measurement = new EntityReference(mzk_mmtcode.EntityLogicalName, new Guid(modelQuestion.measurementId));
        //                        //sonotessectionfinding.mzk_ResponseType = new OptionSetValue(modelQuestion.responseType);

        //                        Guid sectionFindingGUID = repo.CreateEntity(sonotessectionfinding);

        //                        if (sectionFindingGUID != null && sectionFindingGUID != Guid.Empty)
        //                        {
        //                            if (modelQuestion.yes != null)
        //                            {
        //                                //ClinicalTemplateNarration modelnarration = modelQuestion.yes;
        //                                //mzk_sonotesfindingnarration sonotesfindingnarration = new mzk_sonotesfindingnarration();

        //                                //sonotesfindingnarration.mzk_Narration = modelnarration.narrativeText;
        //                                //sonotesfindingnarration.mzk_ResponseType = new OptionSetValue(modelnarration.type);
        //                                //sonotesfindingnarration.mzk_NarrationType = new OptionSetValue((int)mzk_sonotesfindingnarrationmzk_NarrationType.Positive);
        //                                //sonotesfindingnarration.mzk_SectionFinding = new EntityReference(mzk_sonotessectionfinding.EntityLogicalName, sectionFindingGUID);

        //                                //Guid findingNarrationGUID = repo.CreateEntity(sonotesfindingnarration);

        //                                //if (findingNarrationGUID != null && findingNarrationGUID != Guid.Empty)
        //                                //{
        //                                //    this.createChoices(modelnarration.choices, findingNarrationGUID);
        //                                //}

        //                                ClinicalTemplateNarration modelnarration = modelQuestion.yes;

        //                                mzk_mmtcode measurementCode = new mzk_mmtcode();
        //                                measurementCode.mzk_Inputtype = new OptionSetValue(modelnarration.type);
        //                                measurementCode.Id = sonotessectionfinding.mzk_measurement.Id;
                                        
        //                                mzk_measurementcodenarration measurementCodeNarration = new mzk_measurementcodenarration();

        //                                measurementCodeNarration.mzk_narration = modelnarration.narrativeText;
        //                                measurementCodeNarration.mzk_narrationtype = new OptionSetValue((int)mzk_narrationtype.Positive);
        //                                measurementCodeNarration.mzk_measurementcode = new EntityReference(mzk_mmtcode.EntityLogicalName, measurementCode.Id);

        //                                Guid measurementcodenarrationGUID = repo.CreateEntity(measurementCodeNarration);

        //                                if (measurementcodenarrationGUID != null && measurementcodenarrationGUID != Guid.Empty)
        //                                {
        //                                    this.createChoices(modelnarration.choices, measurementcodenarrationGUID);
        //                                }
        //                            }

        //                            if (modelQuestion.no != null)
        //                            {
        //                                //ClinicalTemplateNarration modelnarration = modelQuestion.no;
        //                                //mzk_sonotesfindingnarration sonotesfindingnarration = new mzk_sonotesfindingnarration();

        //                                //sonotesfindingnarration.mzk_Narration = modelnarration.narrativeText;
        //                                //sonotesfindingnarration.mzk_ResponseType = new OptionSetValue(modelnarration.type);
        //                                //sonotesfindingnarration.mzk_NarrationType = new OptionSetValue((int)mzk_sonotesfindingnarrationmzk_NarrationType.Negative);
        //                                //sonotesfindingnarration.mzk_SectionFinding = new EntityReference(mzk_sonotessectionfinding.EntityLogicalName, sectionFindingGUID);

        //                                //Guid findingNarrationGUID = repo.CreateEntity(sonotesfindingnarration);

        //                                //if (findingNarrationGUID != null && findingNarrationGUID != Guid.Empty)
        //                                //{
        //                                //    this.createChoices(modelnarration.choices, findingNarrationGUID);
        //                                //}

        //                                ClinicalTemplateNarration modelnarration = modelQuestion.no;

        //                                mzk_mmtcode measurementCode = new mzk_mmtcode();
        //                                measurementCode.mzk_Inputtype = new OptionSetValue(modelnarration.type);
        //                                measurementCode.Id = sonotessectionfinding.mzk_measurement.Id;

        //                                mzk_measurementcodenarration measurementCodeNarration = new mzk_measurementcodenarration();

        //                                measurementCodeNarration.mzk_narration = modelnarration.narrativeText;
        //                                measurementCodeNarration.mzk_narrationtype = new OptionSetValue((int)mzk_narrationtype.Negative);
        //                                measurementCodeNarration.mzk_measurementcode = new EntityReference(mzk_mmtcode.EntityLogicalName, measurementCode.Id);

        //                                Guid measurementcodenarrationGUID = repo.CreateEntity(measurementCodeNarration);

        //                                if (measurementcodenarrationGUID != null && measurementcodenarrationGUID != Guid.Empty)
        //                                {
        //                                    this.createChoices(modelnarration.choices, measurementcodenarrationGUID);
        //                                }
        //                            }

        //                            if (modelQuestion.noSelection != null)
        //                            {
        //                                //ClinicalTemplateNarration modelnarration = modelQuestion.noSelection;
        //                                //mzk_sonotesfindingnarration sonotesfindingnarration = new mzk_sonotesfindingnarration();

        //                                //sonotesfindingnarration.mzk_Narration = modelnarration.narrativeText;
        //                                //sonotesfindingnarration.mzk_ResponseType = new OptionSetValue((int)mzk_sonotesfindingnarrationmzk_ResponseType.None);
        //                                //sonotesfindingnarration.mzk_NarrationType = new OptionSetValue((int)mzk_sonotesfindingnarrationmzk_NarrationType.NoSelection);
        //                                //sonotesfindingnarration.mzk_SectionFinding = new EntityReference(mzk_sonotessectionfinding.EntityLogicalName, sectionFindingGUID);

        //                                //Guid findingNarrationGUID = repo.CreateEntity(sonotesfindingnarration);

        //                                ClinicalTemplateNarration modelnarration = modelQuestion.noSelection;

        //                                mzk_mmtcode measurementCode = new mzk_mmtcode();
        //                                measurementCode.mzk_Inputtype = new OptionSetValue((int)mzk_inputtype.None);
        //                                measurementCode.Id = sonotessectionfinding.mzk_measurement.Id;

        //                                mzk_measurementcodenarration measurementCodeNarration = new mzk_measurementcodenarration();

        //                                measurementCodeNarration.mzk_narration = modelnarration.narrativeText;
        //                                measurementCodeNarration.mzk_narrationtype = new OptionSetValue((int)mzk_narrationtype.NoSelection);
        //                                measurementCodeNarration.mzk_measurementcode = new EntityReference(mzk_mmtcode.EntityLogicalName, measurementCode.Id);

        //                                Guid measurementcodenarrationGUID = repo.CreateEntity(measurementCodeNarration);                                        
        //                            }

        //                            if (modelQuestion.multiSelect != null)
        //                            {
        //                                //ClinicalTemplateNarration modelnarration = modelQuestion.multiSelect;
        //                                //mzk_sonotesfindingnarration sonotesfindingnarration = new mzk_sonotesfindingnarration();

        //                                //sonotesfindingnarration.mzk_Narration = modelnarration.narrativeText;
        //                                //sonotesfindingnarration.mzk_ResponseType = new OptionSetValue((int)mzk_sonotesfindingnarrationmzk_ResponseType.Dropdown);
        //                                //sonotesfindingnarration.mzk_NarrationType = new OptionSetValue((int)mzk_sonotesfindingnarrationmzk_NarrationType.MultiSelect);
        //                                //sonotesfindingnarration.mzk_SectionFinding = new EntityReference(mzk_sonotessectionfinding.EntityLogicalName, sectionFindingGUID);

        //                                //Guid findingNarrationGUID = repo.CreateEntity(sonotesfindingnarration);

        //                                //if (findingNarrationGUID != null && findingNarrationGUID != Guid.Empty)
        //                                //{
        //                                //    this.createChoices(modelnarration.choices, findingNarrationGUID);
        //                                //}

        //                                ClinicalTemplateNarration modelnarration = modelQuestion.multiSelect;

        //                                mzk_mmtcode measurementCode = new mzk_mmtcode();
        //                                measurementCode.mzk_Inputtype = new OptionSetValue((int)mzk_inputtype.Dropdown);
        //                                measurementCode.Id = sonotessectionfinding.mzk_measurement.Id;

        //                                mzk_measurementcodenarration measurementCodeNarration = new mzk_measurementcodenarration();

        //                                measurementCodeNarration.mzk_narration = modelnarration.narrativeText;
        //                                measurementCodeNarration.mzk_narrationtype = new OptionSetValue((int)mzk_narrationtype.MultiSelect);
        //                                measurementCodeNarration.mzk_measurementcode = new EntityReference(mzk_mmtcode.EntityLogicalName, measurementCode.Id);

        //                                Guid measurementcodenarrationGUID = repo.CreateEntity(measurementCodeNarration);

        //                                if (measurementcodenarrationGUID != null && measurementcodenarrationGUID != Guid.Empty)
        //                                {
        //                                    this.createChoices(modelnarration.choices, measurementcodenarrationGUID);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return true;
        //}

        //public async Task<bool> saveClinicalTemplate(ClinicalTemplate clinicalTemplate)
        //{
        //    ClinicalTemplate model = new ClinicalTemplate();

        //    try
        //    {
        //        SoapEntityRepository repo = SoapEntityRepository.GetService();

        //        mzk_sonotesclinicaltemplate sonotesclinicaltemplate = new mzk_sonotesclinicaltemplate();

        //        sonotesclinicaltemplate.mzk_Description = clinicalTemplate.name;
        //        sonotesclinicaltemplate.mzk_templateid = clinicalTemplate.Id;
        //        sonotesclinicaltemplate.mzk_ShowHeading = clinicalTemplate.showHeading;

        //        Guid clinicalTemplateGUID = repo.CreateEntity(sonotesclinicaltemplate);

        //        if (clinicalTemplateGUID != null && clinicalTemplateGUID != Guid.Empty)
        //        {
        //            List<ClinicalTemplateSection> sections = clinicalTemplate.sections;
        //            Dictionary<string, Guid> sectionsDict = new Dictionary<string, Guid>();

        //            foreach (ClinicalTemplateSection modelSection in sections)
        //            {
        //                mzk_sonotesclinicaltemplatesection sonotesclinicaltemplatesection = new mzk_sonotesclinicaltemplatesection();

        //                sonotesclinicaltemplatesection.mzk_sectionname = modelSection.Name;
        //                Guid parentSectionGUID = Guid.Empty;

        //                if (!string.IsNullOrEmpty(modelSection.ParentId) && sectionsDict.TryGetValue(modelSection.ParentId, out parentSectionGUID))
        //                {
        //                    sonotesclinicaltemplatesection.mzk_ParentSection = new EntityReference(mzk_sonotesclinicaltemplatesection.EntityLogicalName, parentSectionGUID);
        //                }

        //                sonotesclinicaltemplatesection.mzk_Showheading = modelSection.showHeading;
        //                sonotesclinicaltemplatesection.mzk_ClinicalTemplate = new EntityReference(mzk_sonotesclinicaltemplate.EntityLogicalName, clinicalTemplateGUID);
        //                sonotesclinicaltemplatesection.mzk_History = modelSection.history;
        //                if (modelSection.history)
        //                    sonotesclinicaltemplatesection.mzk_HistoryType = new OptionSetValue(modelSection.historyType);

        //                Guid templateSectionGUID = repo.CreateEntity(sonotesclinicaltemplatesection);

        //                sectionsDict.Add(modelSection.Id, templateSectionGUID);

        //                if (templateSectionGUID != null && templateSectionGUID != Guid.Empty)
        //                {
        //                    List<ClinicalTemplateQuestion> questions = modelSection.questions;
        //                    Dictionary<string, Guid> questionsDict = new Dictionary<string, Guid>();

        //                    foreach (ClinicalTemplateQuestion modelQuestion in questions)
        //                    {
        //                        mzk_sonotessectionfinding sonotessectionfinding = new mzk_sonotessectionfinding();

        //                        Guid parentQuestionGUID = Guid.Empty;

        //                        if (!string.IsNullOrEmpty(modelQuestion.ParentId) && questionsDict.TryGetValue(modelQuestion.ParentId, out parentQuestionGUID))
        //                        {
        //                            sonotessectionfinding.mzk_ParentFinding = new EntityReference(mzk_sonotessectionfinding.EntityLogicalName, parentQuestionGUID);
        //                        }

        //                        sonotessectionfinding.mzk_Description = modelQuestion.question;
        //                        sonotessectionfinding.mzk_Paragraph = modelQuestion.isParagraph;
        //                        sonotessectionfinding.mzk_TemplateSection = new EntityReference(mzk_sonotesclinicaltemplatesection.EntityLogicalName, templateSectionGUID);
        //                        sonotessectionfinding.mzk_measurement = new EntityReference(mzk_mmtcode.EntityLogicalName, new Guid (modelQuestion.measurementId));                               
                                
        //                        Guid sectionFindingGUID = repo.CreateEntity(sonotessectionfinding);

        //                        questionsDict.Add(modelQuestion.Id, sectionFindingGUID);

        //                        if (sectionFindingGUID != null && sectionFindingGUID != Guid.Empty)
        //                        {     
        //                            if (modelQuestion.yes != null)
        //                            {
        //                                ClinicalTemplateNarration modelnarration = modelQuestion.yes;

        //                                mzk_mmtcode measurementCode = new mzk_mmtcode();
        //                                measurementCode.mzk_Inputtype = new OptionSetValue(modelnarration.type);
        //                                measurementCode.Id = sonotessectionfinding.mzk_measurement.Id;
        //                                measurementCode.mzk_responsetype = new OptionSetValue(modelQuestion.responseType);

        //                                //Guid measurementCodeGuid = repo.CreateEntity(measurementCode);

        //                                mzk_measurementcodenarration measurementCodeNarration = new mzk_measurementcodenarration();

        //                                measurementCodeNarration.mzk_narration = modelnarration.narrativeText;
        //                                measurementCodeNarration.mzk_narrationtype = new OptionSetValue((int)mzk_narrationtype.Positive);
        //                                measurementCodeNarration.mzk_measurementcode = new EntityReference(mzk_mmtcode.EntityLogicalName, measurementCode.Id);

        //                                Guid measurementcodenarrationGUID = repo.CreateEntity(measurementCodeNarration);

        //                                if (measurementcodenarrationGUID != null && measurementcodenarrationGUID != Guid.Empty)
        //                                {
        //                                    this.createChoices(modelnarration.choices, measurementcodenarrationGUID);
        //                                }                                        
                                        
        //                            }

        //                            if (modelQuestion.no != null)
        //                            {
        //                                ClinicalTemplateNarration modelnarration = modelQuestion.no;

        //                                mzk_mmtcode measurementCode = new mzk_mmtcode();
        //                                measurementCode.mzk_Inputtype = new OptionSetValue(modelnarration.type);
        //                                measurementCode.Id = sonotessectionfinding.mzk_measurement.Id;
        //                                measurementCode.mzk_responsetype = new OptionSetValue(modelQuestion.responseType);

        //                                mzk_measurementcodenarration measurementCodeNarration = new mzk_measurementcodenarration();

        //                                measurementCodeNarration.mzk_narration = modelnarration.narrativeText;
        //                                measurementCodeNarration.mzk_narrationtype = new OptionSetValue((int)mzk_narrationtype.Negative);
        //                                measurementCodeNarration.mzk_measurementcode = new EntityReference(mzk_mmtcode.EntityLogicalName, measurementCode.Id);

        //                                Guid measurementcodenarrationGUID = repo.CreateEntity(measurementCodeNarration);

        //                                if (measurementcodenarrationGUID != null && measurementcodenarrationGUID != Guid.Empty)
        //                                {
        //                                    this.createChoices(modelnarration.choices, measurementcodenarrationGUID);
        //                                }
        //                            }

        //                            if (modelQuestion.noSelection != null)
        //                            {
        //                                ClinicalTemplateNarration modelnarration = modelQuestion.noSelection;

        //                                mzk_mmtcode measurementCode = new mzk_mmtcode();
        //                                measurementCode.mzk_Inputtype = new OptionSetValue((int)mzk_inputtype.None);
        //                                measurementCode.Id = sonotessectionfinding.mzk_measurement.Id;
        //                                measurementCode.mzk_responsetype = new OptionSetValue(modelQuestion.responseType);

        //                                mzk_measurementcodenarration measurementCodeNarration = new mzk_measurementcodenarration();

        //                                measurementCodeNarration.mzk_narration = modelnarration.narrativeText;
        //                                measurementCodeNarration.mzk_narrationtype = new OptionSetValue((int)mzk_narrationtype.NoSelection);
        //                                measurementCodeNarration.mzk_measurementcode = new EntityReference(mzk_mmtcode.EntityLogicalName, measurementCode.Id);

        //                                Guid measurementcodenarrationGUID = repo.CreateEntity(measurementCodeNarration);
        //                            }

        //                            if (modelQuestion.multiSelect != null)
        //                            {
        //                                ClinicalTemplateNarration modelnarration = modelQuestion.multiSelect;

        //                                mzk_mmtcode measurementCode = new mzk_mmtcode();
        //                                measurementCode.mzk_Inputtype = new OptionSetValue((int)mzk_inputtype.Dropdown);
        //                                measurementCode.Id = sonotessectionfinding.mzk_measurement.Id;
        //                                measurementCode.mzk_responsetype = new OptionSetValue(modelQuestion.responseType);

        //                                mzk_measurementcodenarration measurementCodeNarration = new mzk_measurementcodenarration();

        //                                measurementCodeNarration.mzk_narration = modelnarration.narrativeText;
        //                                measurementCodeNarration.mzk_narrationtype = new OptionSetValue((int)mzk_narrationtype.MultiSelect);
        //                                measurementCodeNarration.mzk_measurementcode = new EntityReference(mzk_mmtcode.EntityLogicalName, measurementCode.Id);

        //                                Guid measurementcodenarrationGUID = repo.CreateEntity(measurementCodeNarration);

        //                                if (measurementcodenarrationGUID != null && measurementcodenarrationGUID != Guid.Empty)
        //                                {
        //                                    this.createChoices(modelnarration.choices, measurementcodenarrationGUID);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return true;
        //}

        public void createChoices(List<NarrationChoices> choices, Guid measurementcodenarrationGUID)
        {
            //SoapEntityRepository repo = SoapEntityRepository.GetService();

            //foreach (NarrationChoices modelChoices in choices)
            //{
            //    mzk_sonotesnarrationchoice sonotesnarrationchoice = new mzk_sonotesnarrationchoice();

            //    sonotesnarrationchoice.mzk_name = modelChoices.name;
            //    sonotesnarrationchoice.mzk_FindingNarration = new EntityReference(mzk_sonotesfindingnarration.EntityLogicalName, findingNarrationGUID);

            //    if (!string.IsNullOrEmpty(modelChoices.Image))
            //    {
            //        sonotesnarrationchoice.EntityImage = Convert.FromBase64String(modelChoices.Image);
            //    }

            //    repo.CreateEntity(sonotesnarrationchoice);
            //}

            QueryExpression query = new QueryExpression(mzk_measurementcodenarration.EntityLogicalName);

            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddCondition("mzk_measurementcodenarrationid", ConditionOperator.Equal, measurementcodenarrationGUID);

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entityCollection = entityRepository.GetEntityCollection(query);            

            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
            {  
                foreach (NarrationChoices modelChoices in choices)
                {
                    Entity measurementChoice = new Entity("mzk_measurementchoice");

                    measurementChoice.Attributes["mzk_name"] = modelChoices.name;
                    measurementChoice.Attributes["mzk_measurementcode"] = new EntityReference(mzk_mmtcode.EntityLogicalName, entityCollection.Entities[0].GetAttributeValue<EntityReference>("mzk_measurementcode").Id);

                    if (!string.IsNullOrEmpty(modelChoices.Image))
                    {
                        measurementChoice.Attributes["entityimage"] = modelChoices.Image;
                    }

                    entityRepository.CreateEntity(measurementChoice);
                }                
            }
        }

        public void createPatientSONotesFinding(Guid mzk_patientsonotestemplateGUID, string CPSOid)
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            mzk_patientsonotesfinding entity = new mzk_patientsonotesfinding();
            entity.mzk_PatientSONotesTemplate = new EntityReference(mzk_patientsonotestemplate.EntityLogicalName, mzk_patientsonotestemplateGUID);
            entity.mzk_CasePathwayStateOutcome = new EntityReference(mzk_patientsonotestemplate.EntityLogicalName, new Guid(CPSOid));
            repo.CreateEntity(entity);
        }

        public bool isValidCriteriaById(string templateId, string patientguid)
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(mzk_sonotesclinicaltemplate.EntityLogicalName);
            query.Criteria.AddCondition("mzk_sonotesclinicaltemplateid", ConditionOperator.Equal, new Guid(templateId));
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_sonotesclinicaltemplateid", "mzk_filteronage", "mzk_filterongender", "mzk_gender", "mzk_agevalidationid");
            LinkEntity EntityAgeValidation = new LinkEntity(mzk_mmtgroupcode.EntityLogicalName, mzk_agevalidation.EntityLogicalName, "mzk_agevalidationid", "mzk_agevalidationid", JoinOperator.LeftOuter);
            EntityAgeValidation.EntityAlias = "AgeValidation";
            EntityAgeValidation.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_agefromunit", "mzk_agefromvalue", "mzk_agetounit", "mzk_agetovalue");
            query.LinkEntities.Add(EntityAgeValidation);
            EntityCollection entitycollection = repo.GetEntityCollection(query);

            if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {
                mzk_sonotesclinicaltemplate sonotesclinicaltemplate = (mzk_sonotesclinicaltemplate)entitycollection.Entities[0];

                return this.isValidCriteria(sonotesclinicaltemplate, patientguid, null);
            }
            else
            {
                return false;
            }
        }
    }
}
