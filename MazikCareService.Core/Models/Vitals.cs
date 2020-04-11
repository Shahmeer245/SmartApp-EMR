using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
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
    public class MmtCode
    {
        public string Name { get; set; }
        public string MmtCodeId { get; set; }
        public string MmtGroupMmtCodeId { get; set; }
    }

    public class MmtGroupCode
    {
        public string Name { get; set; }
        public string MmtGroupCodeId { get; set; }

        public List<MmtCode> MmtCodeList
        {
            get; set;
        }

        public async Task<List<MmtGroupCode>> GetVitalsList(string patientguid)
        {
            List<MmtGroupCode> listModel = new List<MmtGroupCode>();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();

                QueryExpression query = new QueryExpression(mzk_mmtgroupcode.EntityLogicalName);
                               
                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_mmtgroupcodeid", "mzk_filteronage", "mzk_filterongender", "mzk_gender", "mzk_agevalidationid");

                LinkEntity EntityAgeValidation = new LinkEntity(mzk_mmtgroupcode.EntityLogicalName, mzk_agevalidation.EntityLogicalName, "mzk_agevalidationid", "mzk_agevalidationid", JoinOperator.LeftOuter);
                EntityAgeValidation.EntityAlias = "AgeValidation";
                EntityAgeValidation.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_agefromunit", "mzk_agefromvalue", "mzk_agetounit", "mzk_agetovalue");
                query.LinkEntities.Add(EntityAgeValidation);
                EntityCollection entitycollection = repo.GetEntityCollection(query);
                MmtGroupCode model;

                foreach (Entity entity in entitycollection.Entities)
                {                
                    model = new MmtGroupCode();

                    mzk_mmtgroupcode mmtgroupcode = (mzk_mmtgroupcode)entity;
                    
                    if(!this.isValidGroup(mmtgroupcode, patientguid))
                    {
                        continue;
                    }

                    model.MmtGroupCodeId = mmtgroupcode.mzk_mmtgroupcodeId.Value.ToString();
                    model.Name = mmtgroupcode.mzk_Description;

                    query = new QueryExpression(mzk_mmtcode.EntityLogicalName);

                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_mmtcodeid");

                    LinkEntity entityTypeMmtGroup = new LinkEntity(mzk_mmtcode.EntityLogicalName, mzk_mmtgroupmmtcode.EntityLogicalName, "mzk_mmtcodeid", "mzk_mmtcodeid", JoinOperator.Inner);
                    entityTypeMmtGroup.LinkCriteria.AddCondition("mzk_mmtgroupcodeid", ConditionOperator.Equal, mmtgroupcode.mzk_mmtgroupcodeId.Value);

                    entityTypeMmtGroup.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_mmtgroupmmtcodeid");

                    query.LinkEntities.Add(entityTypeMmtGroup);

                    EntityCollection entitycollectionChild = repo.GetEntityCollection(query);
                    List<MmtCode> modelChild = new List<MmtCode>();
                    MmtCode mmtCode;

                    foreach (Entity entityChild in entitycollectionChild.Entities)
                    {
                        mmtCode = new MmtCode();
                        mzk_mmtcode mmtcode = (mzk_mmtcode)entityChild;

                        mmtCode.MmtCodeId = mmtcode.mzk_mmtcodeId.Value.ToString();
                        mmtCode.Name = mmtcode.mzk_Description;
                        if (entityChild.Attributes.Contains("mzk_mmtgroupmmtcode1.mzk_mmtgroupmmtcodeid"))
                            mmtCode.MmtGroupMmtCodeId = (entityChild.Attributes["mzk_mmtgroupmmtcode1.mzk_mmtgroupmmtcodeid"] as AliasedValue).Value.ToString();
                        

                        modelChild.Add(mmtCode);
                    }

                    model.MmtCodeList = modelChild;

                    listModel.Add(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listModel;
        }

        public bool isValidGroupById(string mmtgroupcodeId, string patientguid)
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();

            QueryExpression query = new QueryExpression(mzk_mmtgroupcode.EntityLogicalName);

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_mmtgroupcodeid", "mzk_filteronage", "mzk_filterongender", "mzk_gender", "mzk_agevalidationid");

            query.Criteria.AddCondition("mzk_mmtgroupcodeid", ConditionOperator.Equal , new Guid(mmtgroupcodeId));

            LinkEntity EntityAgeValidation = new LinkEntity(mzk_mmtgroupcode.EntityLogicalName, mzk_agevalidation.EntityLogicalName, "mzk_agevalidationid", "mzk_agevalidationid", JoinOperator.LeftOuter);
            EntityAgeValidation.EntityAlias = "AgeValidation";
            EntityAgeValidation.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_agefromunit", "mzk_agefromvalue", "mzk_agetounit", "mzk_agetovalue");
            query.LinkEntities.Add(EntityAgeValidation);
            EntityCollection entitycollection = repo.GetEntityCollection(query);
           
            if(entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
            {             
                mzk_mmtgroupcode mmtgroupcode = (mzk_mmtgroupcode)entitycollection.Entities[0];

                return this.isValidGroup(mmtgroupcode, patientguid);
            }
            else
            {
                return false;
            }
        }

        bool isValidGroup(mzk_mmtgroupcode mmtgroupcode, string patientguid)
        {
            bool ret = false;
            int patientGender = 0;
            DateTime patientBirthDate;
            AgeHelper ageHelper = new AgeHelper(DateTime.Now);
            Patient patient = new Patient();
            PatientRepository patientRepo = new PatientRepository();
            
            patient = new Patient().getPatientDetails(patientguid).Result;
            patientBirthDate = patient.dateOfBirth;
            patientGender = patient.genderValue;                

            if (mmtgroupcode.mzk_FilteronGender.HasValue)      
            {
                if (mmtgroupcode.mzk_FilteronGender.Value)
                {
                    if (patientGender == mmtgroupcode.mzk_Gender.Value)
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
                    ret = true;
                }
            }
            else
            {
                ret = true;
            }

            if(ret)
            {
                if (mmtgroupcode.mzk_FilteronAge.HasValue)
                {
                    if (mmtgroupcode.mzk_FilteronAge.Value)
                    {
                        if (mmtgroupcode.mzk_AgeValidationId != null)
                        {
                            if (ageHelper.isAgeMatched(patientBirthDate, (Helper.Enum.DayWeekMthYr)((OptionSetValue)((AliasedValue)mmtgroupcode.Attributes["AgeValidation.mzk_agefromunit"]).Value).Value, (int)((AliasedValue)mmtgroupcode.Attributes["AgeValidation.mzk_agefromvalue"]).Value, (Helper.Enum.DayWeekMthYr)((OptionSetValue)((AliasedValue)mmtgroupcode.Attributes["AgeValidation.mzk_agetounit"]).Value).Value, (int)((AliasedValue)mmtgroupcode.Attributes["AgeValidation.mzk_agetovalue"]).Value))
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

            return ret;
        }
    }
}
