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
    public class PatientVitals : IPatientVitals
    {
        public double MeasurementValue { get; set; }
        public int GraphType { get; set; }
        public int order { get; set; }
        public double MeasurementValue2 { get; set; }
        public string   VitalName       { get; set; }
        public string MeasurementUnit { get; set; }
        public string MeasurementUnit2 { get; set; }
        public string ConvertedUnit { get; set; }
        public string ConvertedUnit2 { get; set; }
        public string Label1 { get; set; }
        public string Label2 { get; set; }
        public DateTime PerfomanceDate  { get; set; }
        public string EncounterId { get; set; }
        public string MMTGroupCodeId { get; set; }
        public string MMTCodeId { get; set; }
        public string PatientVitalId { get; set; }
        public string PerformedById { get; set; }
        public bool allowEdit { get; set; }
        public List<Graph> graphValues { get; set; }
        public List<MeasurementUnit> unitList { get; set; }
        public int VitalValue { get; set; }
        public string VitalText { get; set; }
        public bool valueUpdated { get; set; }

        public string CasepathwayStateOutcomeId { get; set; }
        

        public async Task<List<PatientVitals>> getPatientVitals(string patientguid, bool getGraphValues = false)
        {        
            List<PatientVitals> PatientVitals = new List<PatientVitals>();
            //string mmtGroupMmtCodeId = "";
            string MMTCodeId = "";
            
            QueryExpression query = this.getPatientVitalsQuery( patientguid, "", "");

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            

            var grouped = entitycollection.Entities.GroupBy(item => ((EntityReference)item.Attributes["mzk_mmtcodeid"]).Id);
            var shortest = grouped.Select(grp => grp.OrderBy(item => Convert.ToDateTime(item.Attributes["mzk_performeddate"].ToString())).Last());

            foreach (Entity entity in shortest)
            {
                PatientVitals model = new PatientVitals();

                if (entity.Attributes.Contains("mzk_measurementvalue"))
                    model.MeasurementValue = Convert.ToDouble(entity.Attributes["mzk_measurementvalue"].ToString());
                if (entity.Attributes.Contains("mzk_measurementvalue2"))
                    model.MeasurementValue2 = Convert.ToDouble(entity.Attributes["mzk_measurementvalue2"].ToString());
                if (entity.Attributes.Contains("mzk_unit"))
                {
                    model.MeasurementUnit = ((EntityReference)(entity["mzk_unit"])).Name;
                }
                if (entity.Attributes.Contains("mzk_mmtcodeid"))
                {
                    MMTCodeId = ((EntityReference)entity["mzk_mmtcodeid"]).Id.ToString();
                    model.MMTCodeId = ((EntityReference)entity["mzk_mmtcodeid"]).Id.ToString();
                    model.VitalName = ((EntityReference)entity["mzk_mmtcodeid"]).Name.ToString();
                }

                //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_vitals"))
                //    model.VitalName = (entity.Attributes["mzk_mmtcode4.mzk_description"] as AliasedValue).Value.ToString();


                if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_vitals"))
                {
                    model.VitalValue = ((OptionSetValue)(entity.Attributes["PatientMMTCodeDirect.mzk_vitals"] as AliasedValue).Value).Value;
                    model.VitalText = entity.FormattedValues["PatientMMTCodeDirect.mzk_vitals"].ToString();
                }
                //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_vitals"))
                //{
                //    model.VitalValue = ((OptionSetValue)(entity.Attributes["mzk_mmtcode4.mzk_vitals"] as AliasedValue).Value).Value;
                //    model.VitalText = entity.FormattedValues["mzk_mmtcode4.mzk_vitals"].ToString();
                //}

                //Check to Exclude CTAS
                if (model.VitalValue == (int)mzk_vital.CTAS)
                {
                    continue;
                }

                if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_label1"))
                    model.Label1 = (entity.Attributes["PatientMMTCodeDirect.mzk_label1"] as AliasedValue).Value.ToString();
                if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_label2"))
                    model.Label2 = (entity.Attributes["PatientMMTCodeDirect.mzk_label2"] as AliasedValue).Value.ToString();
                if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_type"))
                    model.GraphType = ((OptionSetValue)((entity.Attributes["PatientMMTCodeDirect.mzk_type"] as AliasedValue).Value)).Value;
                if (entity.Attributes.Contains("mzk_performeddate"))
                    model.PerfomanceDate = Convert.ToDateTime(entity.Attributes["mzk_performeddate"].ToString());
                if (entity.Attributes.Contains("mzk_patientencountervitalsignid"))
                    model.PatientVitalId = (entity.Attributes["mzk_patientencountervitalsignid"].ToString());

                //if (entity.Attributes.Contains("mzk_mmtgroupmmtcodeid"))
                //    mmtGroupMmtCodeId = ((EntityReference)(entity.Attributes["mzk_mmtgroupmmtcodeid"] as AliasedValue).Value).Id.ToString();

                if (getGraphValues)
                {
                    List<Graph> listgraph = new List<Graph>();

                    var selected = entitycollection.Entities.Where(item => ((EntityReference)(item.Attributes["mzk_mmtcodeid"])).Id.ToString() == MMTCodeId).OrderBy(item => Convert.ToDateTime((item.Attributes["mzk_performeddate"]).ToString()));

                    foreach (Entity entityGraph in selected)
                    {
                        Graph modelGraph = new Graph();

                        if (entityGraph.Attributes.Contains("mzk_measurementvalue"))
                            modelGraph.value1 = Convert.ToDouble(entityGraph.Attributes["mzk_measurementvalue"].ToString());

                        if (entityGraph.Attributes.Contains("mzk_measurementvalue2"))
                            modelGraph.value2 = Convert.ToDouble(entityGraph.Attributes["mzk_measurementvalue2"].ToString());

                        if (entityGraph.Attributes.Contains("mzk_performeddate"))
                            modelGraph.dateTime = (entityGraph.Attributes["mzk_performeddate"].ToString());

                        listgraph.Add(modelGraph);
                    }

                    model.graphValues = listgraph;
                }

                //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_mmtcodeid"))
                //    model.unitList = this.getUnitValues(((entity.Attributes["mzk_mmtcode4.mzk_mmtcodeid"] as AliasedValue).Value).ToString());

                //if (entity.Attributes.Contains("mzk_mmtgroupmmtcode3.mzk_order"))
                //    model.order = Convert.ToInt32((entity.Attributes["mzk_mmtgroupmmtcode3.mzk_order"] as AliasedValue).Value);

                PatientVitals.Add(model);
            }

            return PatientVitals;
        }

        public async Task<List<Graph>> getGraphValues(string patientguid, string mmtCodeId, int graphType)
        {
            List<Graph> listgraph = new List<Graph>();
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            QueryExpression query = this.getPatientVitalsQuery(patientguid, "", mmtCodeId);

            EntityCollection entitycollectionGraph = entityRepository.GetEntityCollection(query);

            foreach (Entity entityGraph in entitycollectionGraph.Entities)
            {
                Graph modelGraph = new Graph();

                if (entityGraph.Attributes.Contains("mzk_measurementvalue"))
                    modelGraph.value1 = Convert.ToDouble(entityGraph.Attributes["mzk_measurementvalue"].ToString());

                if (entityGraph.Attributes.Contains("mzk_measurementvalue2"))
                    modelGraph.value2 = Convert.ToDouble(entityGraph.Attributes["mzk_measurementvalue2"].ToString());
                
                if (entityGraph.Attributes.Contains("mzk_performeddate"))
                    modelGraph.dateTime = entityGraph.Attributes["mzk_performeddate"].ToString();
                
                listgraph.Add(modelGraph);
            }

            return listgraph;
        }
        

        private List<MeasurementUnit> getUnitValues(string mmtCodeId)
        {
            List<MeasurementUnit> listModel = new List<MeasurementUnit>();
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            QueryExpression query = new QueryExpression(mzk_mmtcode.EntityLogicalName);

            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddCondition("mzk_mmtcodeid", ConditionOperator.Equal, new Guid(mmtCodeId));

            LinkEntity linkUnits = new LinkEntity(mzk_mmtcode.EntityLogicalName, mzk_unit.EntityLogicalName, "mzk_unitgroup", "mzk_unitgroup", JoinOperator.Inner);
            linkUnits.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_code", "mzk_description", "mzk_baseunit");

            query.LinkEntities.Add(linkUnits);

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                MeasurementUnit model = new MeasurementUnit();
                if (entity.Attributes.Contains("mzk_baseunit"))
                {
                    model.isDefault = false;
                }
                else
                {
                    model.isDefault = true;
                }
                if (entity.Attributes.Contains("mzk_unit1.mzk_code"))
                    model.unitId = (entity.Attributes["mzk_unit1.mzk_code"] as AliasedValue).Value.ToString();

                    model.Id = entity.Id.ToString();
                                
                listModel.Add(model);
            }

            return listModel;
        }

        public async Task<List<PatientVitals>> getPatientEncounterVitals(string patientguid, string patientEncounter,bool graph=true,bool getDefault=true, string casePathwayStateID = "", bool getUnitList = true)
        {
            List<PatientVitals> PatientVitals = new List<PatientVitals>();
            string mmtGroupMmtCodeId = "";

            QueryExpression query = this.getPatientVitalsQuery("", patientEncounter, "", casePathwayStateID);

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                if (getDefault == false)
                {
                    if (Convert.ToDateTime((entity.Attributes["mzk_patientencountervitalsign2.createdon"] as AliasedValue).Value) != Convert.ToDateTime((entity.Attributes["mzk_patientencountervitalsign2.modifiedon"] as AliasedValue).Value))
                    {
                        PatientVitals model = new PatientVitals();

                        if (entity.Attributes.Contains("mzk_patientencountervitalsign2.mzk_measurementvalue"))
                            model.MeasurementValue = Convert.ToDouble((entity.Attributes["mzk_patientencountervitalsign2.mzk_measurementvalue"] as AliasedValue).Value.ToString());
                        if (entity.Attributes.Contains("mzk_patientencountervitalsign2.mzk_measurementvalue2"))
                            model.MeasurementValue2 = Convert.ToDouble((entity.Attributes["mzk_patientencountervitalsign2.mzk_measurementvalue2"] as AliasedValue).Value.ToString());
                        //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_description"))
                        //    model.VitalName = (entity.Attributes["mzk_mmtcode4.mzk_description"] as AliasedValue).Value.ToString();
                        if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_description"))
                            model.VitalName = (entity.Attributes["PatientMMTCodeDirect.mzk_description"] as AliasedValue).Value.ToString();

                        //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_vitals"))
                        //{
                        //    model.VitalValue = ((OptionSetValue)(entity.Attributes["mzk_mmtcode4.mzk_vitals"] as AliasedValue).Value).Value;
                        //    model.VitalText = entity.FormattedValues["mzk_mmtcode4.mzk_vitals"].ToString();
                        //}
                        if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_vitals"))
                        {
                            model.VitalValue = ((OptionSetValue)(entity.Attributes["PatientMMTCodeDirect.mzk_vitals"] as AliasedValue).Value).Value;
                            model.VitalText = entity.FormattedValues["PatientMMTCodeDirect.mzk_vitals"].ToString();
                        }

                        //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_label1"))
                        //    model.Label1 = (entity.Attributes["mzk_mmtcode4.mzk_label1"] as AliasedValue).Value.ToString();
                        if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_label1"))
                            model.Label1 = (entity.Attributes["PatientMMTCodeDirect.mzk_label1"] as AliasedValue).Value.ToString();
                        //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_label2"))
                        //    model.Label2 = (entity.Attributes["mzk_mmtcode4.mzk_label2"] as AliasedValue).Value.ToString();
                        if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_label2"))
                            model.Label2 = (entity.Attributes["PatientMMTCodeDirect.mzk_label2"] as AliasedValue).Value.ToString();
                        //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_type"))
                        //    model.GraphType = ((OptionSetValue)((entity.Attributes["mzk_mmtcode4.mzk_type"] as AliasedValue).Value)).Value;
                        if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_type"))
                            model.GraphType = ((OptionSetValue)((entity.Attributes["PatientMMTCodeDirect.mzk_type"] as AliasedValue).Value)).Value;
                        if (entity.Attributes.Contains("mzk_patientencountervitalsign2.createdon"))
                            model.PerfomanceDate = Convert.ToDateTime((entity.Attributes["mzk_patientencountervitalsign2.createdon"] as AliasedValue).Value.ToString());
                        if (entity.Attributes.Contains("mzk_patientencountervitalsign2.mzk_patientencountervitalsignid"))
                            model.PatientVitalId = (entity.Attributes["mzk_patientencountervitalsign2.mzk_patientencountervitalsignid"] as AliasedValue).Value.ToString();

                        if (entity.Attributes.Contains("mzk_patientencountervitalsign2.mzk_mmtgroupmmtcodeid"))
                            mmtGroupMmtCodeId = ((EntityReference)(entity.Attributes["mzk_patientencountervitalsign2.mzk_mmtgroupmmtcodeid"] as AliasedValue).Value).Id.ToString();
                        if (graph == true)
                            model.graphValues = await this.getGraphValues(patientguid, mmtGroupMmtCodeId, model.GraphType);

                        //if (entity.Attributes.Contains("mzk_mmtgroupmmtcode3.mzk_order"))
                        //    model.order = Convert.ToInt32((entity.Attributes["mzk_mmtgroupmmtcode3.mzk_order"] as AliasedValue).Value);

                        if (getUnitList)
                        {
                            //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_mmtcodeid"))
                            //    model.unitList = this.getUnitValues(((entity.Attributes["mzk_mmtcode4.mzk_mmtcodeid"] as AliasedValue).Value).ToString());
                            if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_mmtcodeid"))
                                model.unitList = this.getUnitValues(((entity.Attributes["PatientMMTCodeDirect.mzk_mmtcodeid"] as AliasedValue).Value).ToString());
                        }
                        if (entity.Attributes.Contains("mzk_unit"))
                            model.MeasurementUnit = ((EntityReference)(entity["mzk_unit"])).Name;

                        if (entity.Attributes.Contains("mzk_patientencountervitalsign2.mzk_casepathwaystateoutcome"))
                            model.CasepathwayStateOutcomeId = ((EntityReference)(entity.Attributes["mzk_patientencountervitalsign2.mzk_casepathwaystateoutcome"] as AliasedValue).Value).Id.ToString();

                        model.allowEdit = model.VitalValue != (int)mzk_vital.BMI;

                        PatientVitals.Add(model);
                    }

                }
                else
                {
                    PatientVitals model = new PatientVitals();

                    if (entity.Attributes.Contains("mzk_patientencountervitalsign2.mzk_measurementvalue"))
                        model.MeasurementValue = Convert.ToDouble((entity.Attributes["mzk_patientencountervitalsign2.mzk_measurementvalue"] as AliasedValue).Value.ToString());
                    if (entity.Attributes.Contains("mzk_patientencountervitalsign2.mzk_measurementvalue2"))
                        model.MeasurementValue2 = Convert.ToDouble((entity.Attributes["mzk_patientencountervitalsign2.mzk_measurementvalue2"] as AliasedValue).Value.ToString());
                    //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_description"))
                    //    model.VitalName = (entity.Attributes["mzk_mmtcode4.mzk_description"] as AliasedValue).Value.ToString();
                    if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_description"))
                        model.VitalName = (entity.Attributes["PatientMMTCodeDirect.mzk_description"] as AliasedValue).Value.ToString();
                    //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_vitals"))
                    //{
                    //    model.VitalValue = ((OptionSetValue)(entity.Attributes["mzk_mmtcode4.mzk_vitals"] as AliasedValue).Value).Value;
                    //    model.VitalText = entity.FormattedValues["mzk_mmtcode4.mzk_vitals"].ToString();
                    //}
                    if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_vitals"))
                    {
                        model.VitalValue = ((OptionSetValue)(entity.Attributes["PatientMMTCodeDirect.mzk_vitals"] as AliasedValue).Value).Value;
                        model.VitalText = entity.FormattedValues["PatientMMTCodeDirect.mzk_vitals"].ToString();
                    }

                    //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_label1"))
                    //    model.Label1 = (entity.Attributes["mzk_mmtcode4.mzk_label1"] as AliasedValue).Value.ToString();
                    if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_label1"))
                        model.Label1 = (entity.Attributes["PatientMMTCodeDirect.mzk_label1"] as AliasedValue).Value.ToString();
                    //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_label2"))
                    //    model.Label2 = (entity.Attributes["mzk_mmtcode4.mzk_label2"] as AliasedValue).Value.ToString();
                    if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_label2"))
                        model.Label2 = (entity.Attributes["PatientMMTCodeDirect.mzk_label2"] as AliasedValue).Value.ToString();
                    //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_type"))
                    //    model.GraphType = ((OptionSetValue)((entity.Attributes["mzk_mmtcode4.mzk_type"] as AliasedValue).Value)).Value;
                    if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_type"))
                        model.GraphType = ((OptionSetValue)((entity.Attributes["PatientMMTCodeDirect.mzk_type"] as AliasedValue).Value)).Value;
                    if (entity.Attributes.Contains("mzk_patientencountervitalsign2.createdon"))
                        model.PerfomanceDate = Convert.ToDateTime((entity.Attributes["mzk_patientencountervitalsign2.createdon"] as AliasedValue).Value.ToString());
                    if (entity.Attributes.Contains("mzk_patientencountervitalsign2.mzk_patientencountervitalsignid"))
                        model.PatientVitalId = (entity.Attributes["mzk_patientencountervitalsign2.mzk_patientencountervitalsignid"] as AliasedValue).Value.ToString();

                    if (entity.Attributes.Contains("mzk_patientencountervitalsign2.mzk_mmtgroupmmtcodeid"))
                        mmtGroupMmtCodeId = ((EntityReference)(entity.Attributes["mzk_patientencountervitalsign2.mzk_mmtgroupmmtcodeid"] as AliasedValue).Value).Id.ToString();
                    if (graph == true)
                        model.graphValues =await this.getGraphValues(patientguid, mmtGroupMmtCodeId, model.GraphType);

                    //if (entity.Attributes.Contains("mzk_mmtgroupmmtcode3.mzk_order"))
                    //    model.order = Convert.ToInt32((entity.Attributes["mzk_mmtgroupmmtcode3.mzk_order"] as AliasedValue).Value);

                    if (getUnitList)
                    {
                        //if (entity.Attributes.Contains("mzk_mmtcode4.mzk_mmtcodeid"))
                        //    model.unitList = this.getUnitValues(((entity.Attributes["mzk_mmtcode4.mzk_mmtcodeid"] as AliasedValue).Value).ToString());
                        if (entity.Attributes.Contains("PatientMMTCodeDirect.mzk_mmtcodeid"))
                            model.unitList = this.getUnitValues(((entity.Attributes["PatientMMTCodeDirect.mzk_mmtcodeid"] as AliasedValue).Value).ToString());
                    }

                    if (entity.Attributes.Contains("mzk_unit"))
                        model.MeasurementUnit = ((EntityReference)(entity["mzk_unit"])).Name;

                    if (entity.Attributes.Contains("mzk_patientencountervitalsign2.mzk_casepathwaystateoutcome"))
                        model.CasepathwayStateOutcomeId = ((EntityReference)(entity.Attributes["mzk_patientencountervitalsign2.mzk_casepathwaystateoutcome"] as AliasedValue).Value).Id.ToString();

                    model.allowEdit = model.VitalValue != (int)mzk_vital.BMI;

                    if (Convert.ToDateTime((entity.Attributes["mzk_patientencountervitalsign2.createdon"] as AliasedValue).Value) != Convert.ToDateTime((entity.Attributes["mzk_patientencountervitalsign2.modifiedon"] as AliasedValue).Value))
                    {
                        model.valueUpdated = true;
                    }

                    PatientVitals.Add(model);
                }
            }

            return PatientVitals;
        }

        public async Task<List<PatientVitals>> getLastRecordedVitals(string caseId, string appointmentId)
        {
            PatientEncounter patEnc = new PatientEncounter();
            List<PatientVitals> patientVitals = new List<PatientVitals>();

            if (!string.IsNullOrEmpty(appointmentId))
            {
                patEnc = patEnc.encounterDetails((int)mzk_encountertype.Consultation, "", "", appointmentId).Result;

                if (patEnc != null && !string.IsNullOrEmpty(patEnc.EncounterId))
                {
                    patientVitals = new PatientVitals().getPatientEncounterVitals(null, patEnc.EncounterId, false, false).Result.ToList();
                    if (patientVitals.Count == 0)
                    {
                        patEnc = new PatientEncounter();
                        patEnc = patEnc.encounterDetails((int)mzk_encountertype.Triage, "", "", appointmentId).Result;
                        if (patEnc != null && !string.IsNullOrEmpty(patEnc.EncounterId))
                        {
                            patientVitals = new PatientVitals().getPatientEncounterVitals(null, patEnc.EncounterId, false, false).Result.ToList();
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(caseId))
            {
                patEnc.CaseId = caseId;
                patEnc.EncounterType = ((int)mzk_encountertype.PrimaryAssessment).ToString();
                List<PatientEncounter> listEnc = null;

                listEnc = patEnc.getEncounterDetails(patEnc).Result;

                if (listEnc != null && listEnc.FirstOrDefault() != null && !string.IsNullOrEmpty(listEnc.First().EncounterId))
                {
                    patientVitals = new PatientVitals().getPatientEncounterVitals(null, listEnc.First().EncounterId, false, false).Result.ToList();
                    if (patientVitals.Count == 0)
                    {
                        patEnc = new PatientEncounter();
                        patEnc.CaseId = caseId;
                        patEnc.EncounterType = ((int)mzk_encountertype.Triage).ToString();
                        listEnc = patEnc.getEncounterDetails(patEnc).Result;

                        if (listEnc != null && listEnc.First() != null && !string.IsNullOrEmpty(listEnc.First().EncounterId))
                        {
                            patientVitals = new PatientVitals().getPatientEncounterVitals(null, listEnc.First().EncounterId, false, false).Result.ToList();
                        }
                    }
                }
            }

            return patientVitals;
        }

        

        private QueryExpression getPatientVitalsQuery(string patientguid, string patientEncounter, string mmtCodeId, string casePathwayStateID = "")
        {
            QueryExpression query = new QueryExpression("mzk_patientencountervitalsign");
            FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
            if (!string.IsNullOrEmpty(patientguid))
            {
                childFilter.AddCondition("mzk_patientid", ConditionOperator.Equal, new Guid(patientguid));
            }
            if (!string.IsNullOrEmpty(mmtCodeId))
            {
                childFilter.AddCondition("mzk_mmtcodeid", ConditionOperator.Equal, new Guid(mmtCodeId));
            }
            if (!string.IsNullOrEmpty(patientEncounter))
            {
                childFilter.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(patientEncounter));
            }
            childFilter.AddCondition("mzk_performeddate", ConditionOperator.NotNull);
            query.ColumnSet = new ColumnSet("mzk_mmtcodeid", "mzk_performeddate", "mzk_measurementvalue", "mzk_measurementvalue2", "mzk_patientencountervitalsignid", "mzk_unit");

            //LinkEntity PatientMMTCodeMMTgroup = new LinkEntity("mzk_patientencountervitalsign", "mzk_mmtgroupmmtcode", "mzk_mmtgroupmmtcodeid", "mzk_mmtgroupmmtcodeid", JoinOperator.LeftOuter);
            //PatientMMTCodeMMTgroup.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_order");
            //PatientMMTCodeMMTgroup.EntityAlias = "mzk_mmtgroupmmtcode3";
            //LinkEntity PatientMMTCode = new LinkEntity("mzk_patientencountervitalsign", "mzk_mmtcode", "mzk_mmtcodeid", "mzk_mmtcodeid", JoinOperator.LeftOuter);
            //PatientMMTCode.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_label1", "mzk_label2", "mzk_type", "mzk_mmtcodeid", "mzk_vitals");
            //PatientMMTCode.EntityAlias = "mzk_mmtcode4";

            LinkEntity PatientMMTCodeDirect = new LinkEntity("mzk_patientencountervitalsign", "mzk_mmtcode", "mzk_mmtcodeid", "mzk_mmtcodeid", JoinOperator.LeftOuter);
            PatientMMTCodeDirect.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_description", "mzk_label1", "mzk_label2", "mzk_type", "mzk_mmtcodeid", "mzk_vitals");
            PatientMMTCodeDirect.EntityAlias = "PatientMMTCodeDirect";
            //LinkEntity MMTUnit = new LinkEntity("mzk_patientencountervitalsign", "mzk_mmtcodeunit", "mzk_measurementunitid", "mzk_mmtcodeunitid", JoinOperator.LeftOuter);
            //MMTUnit.EntityAlias = "MMTUnit";
            //MMTUnit.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_unit");

            //LinkEntity LinkUnit = new LinkEntity("mzk_mmtgroupmmtcode", "mzk_unit", "mzk_unit", "mzk_unitid", JoinOperator.LeftOuter);
            //LinkUnit.EntityAlias = "UnitEntty";
            //LinkUnit.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            if (!string.IsNullOrEmpty(casePathwayStateID))
            {
                LinkEntity mzk_casepathwaystateoutcomeLinkEntity = new LinkEntity(mzk_patientencountervitalsign.EntityLogicalName, mzk_casepathwaystateoutcome.EntityLogicalName, "mzk_casepathwaystateoutcome", "mzk_casepathwaystateoutcomeid", JoinOperator.Inner);
                mzk_casepathwaystateoutcomeLinkEntity.LinkCriteria.AddCondition("mzk_casepathwaystate", ConditionOperator.Equal, new Guid(casePathwayStateID));
                mzk_casepathwaystateoutcomeLinkEntity.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
                query.LinkEntities.Add(mzk_casepathwaystateoutcomeLinkEntity);
            }

            //query.LinkEntities.Add(PatientEncounter);
            //query.LinkEntities.Add(PatientMMTCodeMMTgroup);
            //query.LinkEntities.Add(PatientMMTCode);
            //query.LinkEntities.Add(MMTUnit);
            query.LinkEntities.Add(PatientMMTCodeDirect);
            //Order by Descending
            query.AddOrder("createdon", OrderType.Descending);

            return query;
        }

        public async Task<bool> addDefaultVitals(string patientEncounterId, string mmtGroupId, bool isGroup, string cpsOutcomeId)
        {
            try
            {
                List<PatientVitals> patientVitals = new List<PatientVitals>();

                SoapEntityRepository repo = SoapEntityRepository.GetService();

                if (isGroup)
                {
                    QueryExpression query = new QueryExpression(mzk_mmtgroupmmtcode.EntityLogicalName);

                    query.Criteria.AddCondition("mzk_mmtgroupcodeid", ConditionOperator.Equal, new Guid(mmtGroupId));

                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_mmtgroupmmtcodeid");

                    EntityCollection entitycollection = repo.GetEntityCollection(query);
                    PatientVitals vital;

                    foreach (Entity entity in entitycollection.Entities)
                    {
                        mzk_mmtgroupmmtcode mmtgroupmmtcode = (mzk_mmtgroupmmtcode)entity;
                        vital = new PatientVitals();

                        vital.EncounterId = patientEncounterId;
                        vital.MMTGroupCodeId = mmtgroupmmtcode.mzk_mmtgroupmmtcodeId.Value.ToString();

                        patientVitals.Add(vital);
                    }
                }
                else
                {
                    PatientVitals vital = new PatientVitals();
                    vital.EncounterId = patientEncounterId;
                    vital.MMTCodeId = mmtGroupId;
                    vital.CasepathwayStateOutcomeId = cpsOutcomeId;

                    patientVitals.Add(vital);
                }                            

                return await this.AddVitals(patientVitals, true, isGroup); 
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public async Task<bool> AddVitals(List<PatientVitals> patientVitals, bool ignoreDuplicate, bool isGroup)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                foreach (PatientVitals vital in patientVitals)
                {
                    if(this.checkVitalExist(vital.EncounterId, vital.MMTGroupCodeId, vital.MMTCodeId))
                    {
                        if(ignoreDuplicate)
                        {
                            continue;
                        }

                        throw new ValidationException(string.Format("Vital sign {0} already exist", vital.VitalName));
                    }
                    
                    Entity entity = new Entity(mzk_patientencountervitalsign.EntityLogicalName);

                    if (isGroup)
                    {
                        entity.Attributes["mzk_mmtgroupmmtcodeid"] = new EntityReference(mzk_mmtgroupmmtcode.EntityLogicalName, new Guid(vital.MMTGroupCodeId));
                    }
                    else
                    {
                        entity.Attributes["mzk_mmtcodeid"] = new EntityReference(mzk_mmtgroupmmtcode.EntityLogicalName, new Guid(vital.MMTCodeId));
                    }
                    entity.Attributes["mzk_patientencounterid"] = new EntityReference(mzk_patientencounter.EntityLogicalName, new Guid(vital.EncounterId));
                    if(!string.IsNullOrEmpty(vital.MeasurementUnit))
                    entity.Attributes["mzk_unit"] = new EntityReference(mzk_unit.EntityLogicalName, new Guid(vital.MeasurementUnit));
                    if (!string.IsNullOrEmpty(vital.CasepathwayStateOutcomeId))
                        entity.Attributes["mzk_casepathwaystateoutcome"] = new EntityReference(mzk_casepathwaystateoutcome.EntityLogicalName, new Guid(vital.CasepathwayStateOutcomeId));


                    entityRepository.CreateEntity(entity);
                }

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateVitalValues(List<PatientVitals> patientVitals)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                Dictionary<string, string> bmiValues = new Dictionary<string, string>();
                PatientVitals vitalBmi = null;

                foreach (PatientVitals vital in patientVitals)
                {
                    Entity entity = entityRepository.GetEntity(mzk_patientencountervitalsign.EntityLogicalName, new Guid(vital.PatientVitalId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientencountervitalsignid"));

                    if (entity.Attributes["mzk_patientencountervitalsignid"].ToString() == string.Empty)
                    {
                        continue;
                    }

                    entity.Attributes["mzk_measurementvalue"] = vital.MeasurementValue;
                    entity.Attributes["mzk_measurementvalue2"] = vital.MeasurementValue2;
                    entity.Attributes["mzk_performeddate"] = vital.PerfomanceDate;
                    entity.Attributes["mzk_userid"] = new EntityReference(SystemUser.EntityLogicalName, new Guid(vital.PerformedById));
                    if (!string.IsNullOrEmpty(vital.MeasurementUnit))
                        entity.Attributes["mzk_unit"] = new EntityReference(mzk_unit.EntityLogicalName, new Guid(vital.MeasurementUnit));

                    entityRepository.UpdateEntity(entity);

                    //BMI temp work
                    if(vital.VitalName.ToLower() == "weight")
                    {
                        bmiValues.Add("weight", vital.MeasurementValue.ToString());
                    }
                    else if (vital.VitalName.ToLower() == "height")
                    {
                        bmiValues.Add("height", vital.MeasurementValue.ToString());
                    }
                    else if (vital.VitalName.ToLower() == "bmi")
                    {
                        bmiValues.Add("bmi", vital.PatientVitalId);
                        vitalBmi = vital;
                    }
                }

                //BMI temp work
                if (bmiValues.Count == 3 && vitalBmi != null)
                {
                    mzk_patientencountervitalsign entity = (mzk_patientencountervitalsign)entityRepository.GetEntity(mzk_patientencountervitalsign.EntityLogicalName, new Guid(vitalBmi.PatientVitalId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientencountervitalsignid"));
                    
                    string weight, height = string.Empty;
                    double weightDec = 0;
                    double heightDec = 0;

                    bmiValues.TryGetValue("height", out height);
                    bmiValues.TryGetValue("weight", out weight);

                    double.TryParse(height, out heightDec);
                    double.TryParse(weight, out weightDec);

                    heightDec = heightDec / 100;

                    if (entity.Attributes["mzk_patientencountervitalsignid"].ToString() != string.Empty)
                    {
                        double bmi = 0;

                        if (heightDec > 0)
                        {
                            bmi = (weightDec / (heightDec * heightDec));
                        }

                        entity.mzk_MeasurementValue = bmi;
                        entity.mzk_MeasurementValue2 = 0;
                        entity.Attributes["mzk_performeddate"] = vitalBmi.PerfomanceDate;
                        entity.Attributes["mzk_userid"] = new EntityReference(SystemUser.EntityLogicalName, new Guid(vitalBmi.PerformedById));

                        entityRepository.UpdateEntity(entity);
                    }
                }              

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteVitals(List<PatientVitals> patientVitals)
        {
            try
            {
                SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

                foreach (PatientVitals vital in patientVitals)
                {
                    Entity entity = entityRepository.GetEntity(mzk_patientencountervitalsign.EntityLogicalName, new Guid(vital.PatientVitalId), new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_patientencountervitalsignid"));

                    if (entity.Attributes["mzk_patientencountervitalsignid"].ToString() == string.Empty)
                    {
                        continue;
                    }                    

                    entityRepository.DeleteEntity(mzk_patientencountervitalsign.EntityLogicalName, new Guid(vital.PatientVitalId));
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PatientVitals> viewVitalsHistory(string patientId, string mmtCodeId)
        {
            try
            {
                if (!string.IsNullOrEmpty(patientId))
                {
                    //List<PatientVitals> patientVitals = new List<PatientVitals>();
                    QueryExpression query = new QueryExpression(xrm.mzk_patientencountervitalsign.EntityLogicalName);
                    FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                    childFilter.AddCondition("mzk_patientidid", ConditionOperator.Equal, new Guid(patientId));
                    if (!string.IsNullOrEmpty(mmtCodeId))
                    {
                        childFilter.AddCondition("mzk_mmtcodeidid", ConditionOperator.Equal, new Guid(mmtCodeId));
                        query.ColumnSet = new ColumnSet("mzk_measurementvalue", "mzk_measurementvalue2", "mzk_performeddate");
                        SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
                        EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
                        PatientVitals patientVital = new PatientVitals();
                        patientVital.graphValues = new List<Graph>();
                        foreach (Entity entity in entitycollection.Entities)
                        {
                            Graph values = new Graph();
                            if (entity.Attributes.Contains("mzk_measurementvalue"))
                            {
                                values.value1 = (Double)entity["mzk_measurementvalue"];
                            }
                            if (entity.Attributes.Contains("mzk_measurementvalue2"))
                            {
                                values.value2 = (Double)entity["mzk_measurementvalue2"];
                            }
                            if (entity.Attributes.Contains("mzk_performeddate"))
                            {
                                values.dateTime = entity["mzk_performeddate"].ToString();
                            }
                            patientVital.graphValues.Add(values);
                        }
                        return patientVital;

                    }
                    else
                    {
                        throw new ValidationException("MMT Code Id not found");
                    }
                }
                else
                {
                    throw new ValidationException("Patient Id not found");
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        private bool checkVitalExist(string patientEncounter, string mmtGroupMmtCodeId, string mmtCodeId)
        {            
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            if (!string.IsNullOrEmpty(mmtGroupMmtCodeId))
            {
                mzk_mmtgroupmmtcode mmtgroupmmtcode = (mzk_mmtgroupmmtcode)entityRepository.GetEntity(mzk_mmtgroupmmtcode.EntityLogicalName, new Guid(mmtGroupMmtCodeId), new ColumnSet("mzk_mmtcodeid"));

                if (mmtgroupmmtcode != null)
                {
                    QueryExpression query = new QueryExpression(mzk_patientencountervitalsign.EntityLogicalName);

                    query.ColumnSet = new ColumnSet("mzk_patientencountervitalsignid");
                    query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(patientEncounter));

                    LinkEntity PatientMMTCodeMMTgroup = new LinkEntity("mzk_patientencountervitalsign", "mzk_mmtgroupmmtcode", "mzk_mmtgroupmmtcodeid", "mzk_mmtgroupmmtcodeid", JoinOperator.Inner);
                    PatientMMTCodeMMTgroup.EntityAlias = "PatientMMTCodeMMTgroup";
                    PatientMMTCodeMMTgroup.Columns = new ColumnSet("mzk_mmtcodeid");

                    query.LinkEntities.Add(PatientMMTCodeMMTgroup);

                    EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                    if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                    {
                        if (entitycollection.Entities.Any(item => ((EntityReference)(item.Attributes["PatientMMTCodeMMTgroup.mzk_mmtcodeid"] as AliasedValue).Value).Id.ToString() == mmtgroupmmtcode.mzk_MmtCodeId.Id.ToString()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (!string.IsNullOrEmpty(mmtCodeId))
            {
                QueryExpression query = new QueryExpression(mzk_patientencountervitalsign.EntityLogicalName);

                query.ColumnSet = new ColumnSet("mzk_patientencountervitalsignid");
                query.Criteria.AddCondition("mzk_patientencounterid", ConditionOperator.Equal, new Guid(patientEncounter));
                query.Criteria.AddCondition("mzk_mmtcodeid", ConditionOperator.Equal, new Guid(mmtCodeId));

                EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

                if (entitycollection != null && entitycollection.Entities != null && entitycollection.Entities.Count > 0)
                { 
                    return true;                    
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public class Graph
    {
        public string dateTime { get; set; }
        public double value1 { get; set; }
        public double value2 { get; set; }       
    }

    public class MeasurementUnit
    {
        public string unitId { get; set; }
        public string Id { get; set; }
        public bool isDefault { get; set; }
    }

}

 