using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using xrm;
using MazikLogger;

namespace MazikCareService.Core.Models
{
    public class Worklist : IWorklist
    {
        public string name
        {
            get; set;
        }

        public string worklistTypeId
        {
            get; set;
        }

        public string type
        {
            get; set;
        }

        HMDropDownContract[] activityStatusList;

        DropDownList activityStatusDropDownList;

        public List<DropDown> SearchFilters { get; set; }

        public async Task<List<Worklist>> getWorklistTypes(string userId, bool fetchByRole = false)
        {
            List<Worklist> listModel = new List<Worklist>();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                EntityCollection entitycol = repo.GetEntityCollection(this.getWorklistTypesQuery(userId, fetchByRole));

                Worklist model;

                foreach (Entity entity in entitycol.Entities)
                {
                    model = new Worklist();
                    mzk_worklisttype worklisttype = (mzk_worklisttype)entity;

                    model.name = worklisttype.mzk_name;
                    model.type = worklisttype.FormattedValues["mzk_type"].ToString();
                    model.worklistTypeId = worklisttype.mzk_worklisttypeId.Value.ToString();

                    DropDownList searchFilters = new DropDownList();

                    switch ((mzk_worklisttypemzk_Type)worklisttype.mzk_Type.Value)
                    {
                        case mzk_worklisttypemzk_Type.todaysConsultation:
                            searchFilters = searchFilters.getOptionSetList("SearchFilters", "", "mzk_consultationworklistfilter", "0");
                            break;
                        case mzk_worklisttypemzk_Type.erAssessmentWorklist:
                        case mzk_worklisttypemzk_Type.erCaseWorklist:
                            searchFilters = searchFilters.getOptionSetList("SearchFilters", "", "mzk_assessmentworklistfilter", "0");
                            break;
                        case mzk_worklisttypemzk_Type.erTriageWorklist:
                            searchFilters = searchFilters.getOptionSetList("SearchFilters", "", "mzk_ertriageworklistfilter", "0");
                            break;
                        case mzk_worklisttypemzk_Type.todaysTriage:
                            searchFilters = searchFilters.getOptionSetList("SearchFilters", "", "mzk_triageworklistfilter", "0");
                            break;
                        case mzk_worklisttypemzk_Type.todaysTreatment:
                        case mzk_worklisttypemzk_Type.specialTestWorklist:
                            searchFilters = searchFilters.getOptionSetList("SearchFilters", "", "mzk_treatmentworklistfilter", "0");
                            break;
                        case mzk_worklisttypemzk_Type.erTreatmentWorklist:
                            searchFilters = searchFilters.getOptionSetList("SearchFilters", "", "mzk_ertreatmentworklistfilter", "0");
                            break;
                    }

                    if (searchFilters != null && searchFilters.DropDownValues != null)
                    {
                        model.SearchFilters = searchFilters.DropDownValues;
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

        private QueryExpression getWorklistTypesQuery(string userGUID, bool fetchByRole = false)
        {
            if (fetchByRole == false)
            {
                QueryExpression query = new QueryExpression(mzk_worklisttype.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_name", "mzk_type", "mzk_worklisttypeid");

                LinkEntity entityTypeUser = new LinkEntity(mzk_worklisttype.EntityLogicalName, mzk_worklisttypeuser.EntityLogicalName, "mzk_worklisttypeid", "mzk_worklisttypeid", JoinOperator.Inner);
                entityTypeUser.LinkCriteria.AddCondition("mzk_userid", ConditionOperator.Equal, new Guid(userGUID));

                query.LinkEntities.Add(entityTypeUser);

                return query;
            }
            else
            {
                QueryExpression query = new QueryExpression(mzk_worklisttype.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_name", "mzk_type", "mzk_worklisttypeid");

                LinkEntity entityTypeUser = new LinkEntity(mzk_worklisttype.EntityLogicalName, mzk_worklisttypeuser.EntityLogicalName, "mzk_worklisttypeid", "mzk_worklisttypeid", JoinOperator.Inner);
                entityTypeUser.Columns = new ColumnSet("mzk_worklisttypeid", "mzk_role");
                LinkEntity entityTypeSystemUserRoles = new LinkEntity(mzk_worklisttypeuser.EntityLogicalName, SystemUserRoles.EntityLogicalName, "mzk_role", "roleid", JoinOperator.Inner);
                entityTypeSystemUserRoles.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, new Guid(userGUID));
                entityTypeSystemUserRoles.Columns = new ColumnSet("roleid", "systemuserid");
                query.LinkEntities.Add(entityTypeUser);
                entityTypeUser.LinkEntities.Add(entityTypeSystemUserRoles);

                return query;

            }

        }

        public mzk_worklisttypemzk_Type getWorklistType(string worklistTypeID)
        {
            SoapEntityRepository repo = SoapEntityRepository.GetService();
            QueryExpression query = new QueryExpression(mzk_worklisttype.EntityLogicalName);
            mzk_worklisttypemzk_Type workListType = 0;

            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_name", "mzk_type", "mzk_worklisttypeid");

            query.Criteria.AddCondition("mzk_worklisttypeid", ConditionOperator.Equal, new Guid(worklistTypeID));

            EntityCollection entitycol = repo.GetEntityCollection(query);

            if (entitycol.Entities.Count > 0)
            {
                workListType = (mzk_worklisttypemzk_Type)((OptionSetValue)entitycol.Entities[0]["mzk_type"]).Value;
            }

            return workListType;
        }

        public async Task<List<Activity>> getUserWorkListData(string worklistTypeID, string userId, string clinicId, DateTime date, string SearchFilters, string searchOrder, string timezone, string resourceId = null)
        {
            List<Activity> listModel = new List<Activity>();
            mzk_worklisttypemzk_Type worklistType = this.getWorklistType(worklistTypeID);

            DropDownList dropDown = new DropDownList();
            activityStatusDropDownList = dropDown.getOptionSetList("Activity Status", string.Empty, "mzk_activitystatus", string.Empty);

            try
            {
                switch (worklistType)
                {
                    case mzk_worklisttypemzk_Type.todaysConsultation:
                        listModel = await this.getConsultationtWorklist(clinicId, date, searchOrder, resourceId);
                        listModel.AddRange(await this.getCasePathwayStateActivityWorkflowWorklist(date, worklistTypeID, mzk_encountertype.Consultation, searchOrder, userId));

                        //Search Filter
                        if (!string.IsNullOrEmpty(SearchFilters) && Convert.ToInt32(SearchFilters) != (int)mzk_consultationworklistfilter.All)
                        {

                            mzk_activitystatus filterStatus = mzk_activitystatus.None;

                            switch (Convert.ToInt32(SearchFilters))
                            {
                                case (int)mzk_consultationworklistfilter.Open:
                                    filterStatus = mzk_activitystatus.Open;
                                    break;
                                case (int)mzk_consultationworklistfilter.ConsultationComplete:
                                    filterStatus = mzk_activitystatus.ConsultationComplete;
                                    break;
                                case (int)mzk_consultationworklistfilter.InConsultation:
                                    filterStatus = mzk_activitystatus.InConsultation;
                                    break;
                                case (int)mzk_consultationworklistfilter.PendingConsultation:
                                    filterStatus = mzk_activitystatus.PendingConsultation;
                                    break;


                            }
                            listModel = listModel.Where(item => item.Status == (int)filterStatus).ToList();
                        }

                        break;
                    case mzk_worklisttypemzk_Type.todaysTriage:
                        listModel = await this.getTriageWorklist(clinicId, date, searchOrder);
                        listModel.AddRange(await this.getCasePathwayStateActivityWorkflowWorklist(date, worklistTypeID, mzk_encountertype.Triage, searchOrder, userId));
                        //Search Filter
                        if (!string.IsNullOrEmpty(SearchFilters) && Convert.ToInt32(SearchFilters) != (int)mzk_triageworklistfilter.All)
                        {

                            mzk_activitystatus filterStatus = mzk_activitystatus.None;

                            switch (Convert.ToInt32(SearchFilters))
                            {
                                case (int)mzk_triageworklistfilter.Open:
                                    filterStatus = mzk_activitystatus.Open;
                                    break;
                                case (int)mzk_triageworklistfilter.TriageComplete:
                                    filterStatus = mzk_activitystatus.TriageComplete;
                                    break;
                                case (int)mzk_triageworklistfilter.InTriage:
                                    filterStatus = mzk_activitystatus.InTriage;
                                    break;
                                case (int)mzk_triageworklistfilter.PendingTriage:
                                    filterStatus = mzk_activitystatus.PendingTriage;
                                    break;
                            }

                            listModel = listModel.Where(item => item.Status == (int)filterStatus).ToList();
                        }
                        break;
                    case mzk_worklisttypemzk_Type.todaysTreatment:
                        listModel = await this.getTreatmentWorklist(clinicId, date, searchOrder, false);
                        listModel.AddRange(await this.getCasePathwayStateActivityWorkflowWorklist(date, worklistTypeID, mzk_encountertype.Treatment, searchOrder, userId));

                        //Search Filter
                        if (!string.IsNullOrEmpty(SearchFilters) && Convert.ToInt32(SearchFilters) != (int)mzk_treatmentworklistfilter.All)
                        {

                            mzk_activitystatus filterStatus = mzk_activitystatus.None;

                            switch (Convert.ToInt32(SearchFilters))
                            {
                                case (int)mzk_treatmentworklistfilter.Open:
                                    filterStatus = mzk_activitystatus.Open;
                                    break;
                                case (int)mzk_treatmentworklistfilter.TreatmentComplete:
                                    filterStatus = mzk_activitystatus.TreatmentComplete;
                                    break;
                                case (int)mzk_treatmentworklistfilter.InTreatment:
                                    filterStatus = mzk_activitystatus.InTreatment;
                                    break;
                                case (int)mzk_treatmentworklistfilter.PendingTreatment:
                                    filterStatus = mzk_activitystatus.PendingTreatment;
                                    break;
                            }

                            listModel = listModel.Where(item => item.Status == (int)filterStatus).ToList();
                        }
                        break;
                    case mzk_worklisttypemzk_Type.specialTestWorklist:
                        listModel = await this.getTreatmentWorklist(clinicId, date, searchOrder, true);

                        //Search Filter
                        if (!string.IsNullOrEmpty(SearchFilters) && Convert.ToInt32(SearchFilters) != (int)mzk_treatmentworklistfilter.All)
                        {

                            mzk_activitystatus filterStatus = mzk_activitystatus.None;

                            switch (Convert.ToInt32(SearchFilters))
                            {
                                case (int)mzk_treatmentworklistfilter.Open:
                                    filterStatus = mzk_activitystatus.Open;
                                    break;
                                case (int)mzk_treatmentworklistfilter.TreatmentComplete:
                                    filterStatus = mzk_activitystatus.TreatmentComplete;
                                    break;
                                case (int)mzk_treatmentworklistfilter.InTreatment:
                                    filterStatus = mzk_activitystatus.InTreatment;
                                    break;
                                case (int)mzk_treatmentworklistfilter.PendingTreatment:
                                    filterStatus = mzk_activitystatus.PendingTreatment;
                                    break;
                            }

                            listModel = listModel.Where(item => item.Status == (int)filterStatus).ToList();

                        }
                        break;
                    case mzk_worklisttypemzk_Type.erTriageWorklist:
                        listModel = await this.getERWorklist(clinicId, date, searchOrder, mzk_encountertype.Triage, userId, timezone);

                        //Search Filter
                        if (!string.IsNullOrEmpty(SearchFilters) && Convert.ToInt32(SearchFilters) != (int)mzk_ertriageworklistfilter.All)
                        {
                            mzk_activitystatus filterStatus = mzk_activitystatus.None;

                            switch (Convert.ToInt32(SearchFilters))
                            {
                                case (int)mzk_ertriageworklistfilter.Open:
                                    filterStatus = mzk_activitystatus.Open;
                                    break;
                                case (int)mzk_ertriageworklistfilter.TriageComplete:
                                    filterStatus = mzk_activitystatus.TriageComplete;
                                    break;
                                case (int)mzk_ertriageworklistfilter.InTriage:
                                    filterStatus = mzk_activitystatus.InTriage;
                                    break;
                                case (int)mzk_ertriageworklistfilter.PendingTriage:
                                    filterStatus = mzk_activitystatus.PendingTriage;
                                    break;
                                case (int)mzk_ertriageworklistfilter.Clinicaldischarge:
                                    filterStatus = mzk_activitystatus.ClinicallyDischarged;
                                    break;
                                case (int)mzk_ertriageworklistfilter.Physicaldischarge:
                                    filterStatus = mzk_activitystatus.PhysicallyDischarged;
                                    break;
                                case (int)mzk_ertriageworklistfilter.Financialdischarge:
                                    filterStatus = mzk_activitystatus.FinanciallyDischarged;
                                    break;
                            }

                            listModel = listModel.Where(item => item.Status == (int)filterStatus).ToList();
                        }
                        break;
                    case mzk_worklisttypemzk_Type.erAssessmentWorklist:
                        listModel = await this.getERWorklist(clinicId, date, searchOrder, mzk_encountertype.PrimaryAssessment, userId, timezone);

                        if (!string.IsNullOrEmpty(SearchFilters) && Convert.ToInt32(SearchFilters) != (int)mzk_assessmentworklistfilter.All)
                        {
                            mzk_activitystatus filterStatus = mzk_activitystatus.None;

                            switch (Convert.ToInt32(SearchFilters))
                            {
                                case (int)mzk_assessmentworklistfilter.Open:
                                    filterStatus = mzk_activitystatus.Open;
                                    break;
                                case (int)mzk_assessmentworklistfilter.AssessmentComplete:
                                    filterStatus = mzk_activitystatus.AssessmentComplete;
                                    break;
                                case (int)mzk_assessmentworklistfilter.InAssessment:
                                    filterStatus = mzk_activitystatus.InAssessment;
                                    break;
                                case (int)mzk_assessmentworklistfilter.PendingAssessment:
                                    filterStatus = mzk_activitystatus.PendingAssessment;
                                    break;
                                case (int)mzk_assessmentworklistfilter.Clinicaldischarge:
                                    filterStatus = mzk_activitystatus.ClinicallyDischarged;
                                    break;
                                case (int)mzk_assessmentworklistfilter.Physicaldischarge:
                                    filterStatus = mzk_activitystatus.PhysicallyDischarged;
                                    break;
                                case (int)mzk_assessmentworklistfilter.Financialdischarge:
                                    filterStatus = mzk_activitystatus.FinanciallyDischarged;
                                    break;
                            }

                            listModel = listModel.Where(item => item.Status == (int)filterStatus).ToList();

                        }
                        break;
                    case mzk_worklisttypemzk_Type.erCaseWorklist:
                        listModel = await this.getERWorklist(clinicId, date, searchOrder, mzk_encountertype.Assessment, userId, timezone);

                        //Search Filter
                        if (!string.IsNullOrEmpty(SearchFilters) && Convert.ToInt32(SearchFilters) != (int)mzk_assessmentworklistfilter.All)
                        {
                            mzk_activitystatus filterStatus = mzk_activitystatus.None;

                            switch (Convert.ToInt32(SearchFilters))
                            {
                                case (int)mzk_assessmentworklistfilter.Open:
                                    filterStatus = mzk_activitystatus.Open;
                                    break;
                                case (int)mzk_assessmentworklistfilter.AssessmentComplete:
                                    filterStatus = mzk_activitystatus.AssessmentComplete;
                                    break;
                                case (int)mzk_assessmentworklistfilter.InAssessment:
                                    filterStatus = mzk_activitystatus.InAssessment;
                                    break;
                                case (int)mzk_assessmentworklistfilter.PendingAssessment:
                                    filterStatus = mzk_activitystatus.PendingAssessment;
                                    break;
                                case (int)mzk_assessmentworklistfilter.Clinicaldischarge:
                                    filterStatus = mzk_activitystatus.ClinicallyDischarged;
                                    break;
                                case (int)mzk_assessmentworklistfilter.Physicaldischarge:
                                    filterStatus = mzk_activitystatus.PhysicallyDischarged;
                                    break;
                                case (int)mzk_assessmentworklistfilter.Financialdischarge:
                                    filterStatus = mzk_activitystatus.FinanciallyDischarged;
                                    break;
                            }

                            listModel = listModel.Where(item => item.Status == (int)filterStatus).ToList();
                        }
                        break;
                    case mzk_worklisttypemzk_Type.erTreatmentWorklist:
                        listModel = await this.getERWorklist(clinicId, date, searchOrder, mzk_encountertype.Treatment, userId, timezone);

                        //Search Filter
                        if (!string.IsNullOrEmpty(SearchFilters) && Convert.ToInt32(SearchFilters) != (int)mzk_ertreatmentworklistfilter.All)
                        {

                            mzk_activitystatus filterStatus = mzk_activitystatus.None;

                            switch (Convert.ToInt32(SearchFilters))
                            {
                                case (int)mzk_ertreatmentworklistfilter.Open:
                                    filterStatus = mzk_activitystatus.Open;
                                    break;
                                case (int)mzk_ertreatmentworklistfilter.TreatmentComplete:
                                    filterStatus = mzk_activitystatus.TreatmentComplete;
                                    break;
                                case (int)mzk_ertreatmentworklistfilter.InTreatment:
                                    filterStatus = mzk_activitystatus.InTreatment;
                                    break;
                                case (int)mzk_ertreatmentworklistfilter.PendingTreatment:
                                    filterStatus = mzk_activitystatus.PendingTreatment;
                                    break;
                                case (int)mzk_ertreatmentworklistfilter.Clinicaldischarge:
                                    filterStatus = mzk_activitystatus.ClinicallyDischarged;
                                    break;
                                case (int)mzk_ertreatmentworklistfilter.Physicaldischarge:
                                    filterStatus = mzk_activitystatus.PhysicallyDischarged;
                                    break;
                                case (int)mzk_ertreatmentworklistfilter.Financialdischarge:
                                    filterStatus = mzk_activitystatus.FinanciallyDischarged;
                                    break;
                            }

                            listModel = listModel.Where(item => item.Status == (int)filterStatus).ToList();
                        }
                        break;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listModel;
        }

        public async Task<List<Activity>> getConsultationtWorklist(string clinicId, DateTime date, string searchOrder, string resourceId = null, string timezone = null)
        {
            List<Activity> listModel = new List<Activity>();

            try
            {
                List<Appointment> appointments = Appointment.getUserAppointments(resourceId, clinicId, date, date.AddDays(1), false, searchOrder, null, null, false);

                if (appointments != null && appointments.Count() > 0)
                {
                    List<PatientAllergy> patAllergyList = new List<PatientAllergy>();
                    List<string> patientIdList = new List<string>();
                    List<string> appointmentIdList = new List<string>();
                    List<PatientProblem> patientProblemList = new List<PatientProblem>();
                    //Dictionary<decimal, string> patientList = null;
                    List<PatientEncounter> patientEncList = new List<PatientEncounter>();

                    foreach (Appointment appointment in appointments)
                    {
                        patientIdList.Add(appointment.patient.patientId);
                        appointmentIdList.Add(appointment.appointmentId);
                    }

                    //patientList = new Patient().getPatientIdListFromRefRecIdList(patientIdList);

                    patAllergyList = new PatientAllergy().getPatientAllergiesFromList(patientIdList, "Active", DateTime.MinValue, DateTime.MinValue);

                    patientProblemList = new PatientProblem().getPatientProblemsFromList(patientIdList, "Active", DateTime.MinValue, DateTime.MinValue, false);

                    patientEncList = new PatientEncounter().encounterDetailsFromList(appointmentIdList, null);

                    Activity model;

                    string patientId = string.Empty;

                    foreach (Appointment appointment in appointments)
                    {
                        model = new Activity();

                        //  if (patientList.TryGetValue(Convert.ToDecimal(contract.parmPatientRecId), out patientId))
                        if (patientIdList.Contains(appointment.patient.patientId))
                        {
                            patientId = appointment.patient.patientId;

                            model.patient = appointment.patient; //this.updatePatientDetails(model, appointment.patient, patientId);

                            model.caseId = appointment.caseId;  //.parmCaseId;

                            this.updatePatientAllergies(model, patAllergyList, patientId);

                            this.updatePatientProblems(model, patientProblemList, patientId);

                            model.appointment = appointment;    //this.updatePatientAppointment(model, contract);


                            if (model != null)
                            {
                                //Calculate TimeZone
                                double result = 0;

                                if (!string.IsNullOrEmpty(timezone))
                                {
                                    double.TryParse(timezone.Substring(0, 3), out result);
                                }

                                model.activityDateTime = Convert.ToDateTime(model.appointment.startDateTime).AddHours(result).ToString();
                                //model.activityDateTime = model.appointment.startDateTime;

                                this.updateOPStatusEnhanced(model, (msdyn_bookingsystemstatus)appointment.statusValue, mzk_encountertype.Consultation, false, patientEncList);       // Need to work on this

                                model.type = (int)mzk_encountertype.Consultation;
                                model.EncounterSigoffLabel = Activity.EncounterSigoff.SignOff.GetDescription();

                                listModel.Add(model);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listModel;
        }

        public async Task<List<Activity>> getTriageWorklist(string clinicId, DateTime date, string searchOrder, string timezone = null)
        {
            List<Activity> listModel = new List<Activity>();  
                      
            try
            {
                AppointmentRepository repo = new AppointmentRepository();

                List<Appointment> appointments = Appointment.getUserAppointments(null, clinicId, date, date, false, searchOrder, null, null, false);

                if (appointments != null && appointments.Count() > 0)
                {
                    List<PatientAllergy> patAllergyList = new List<PatientAllergy>();
                    List<string> patientIdList = new List<string>();
                    List<string> appointmentIdList = new List<string>();
                    List<PatientProblem> patientProblemList = new List<PatientProblem>();
                    //Dictionary<decimal, string> patientList = null;
                    List<PatientEncounter> patientEncList = new List<PatientEncounter>();

                    foreach (Appointment appointment in appointments)
                    {
                        patientIdList.Add(appointment.patient.patientId);
                        appointmentIdList.Add(appointment.appointmentId);
                    }

                    //patientList = new Patient().getPatientIdListFromRefRecIdList(patientRecIdList);

                    patAllergyList = new PatientAllergy().getPatientAllergiesFromList(patientIdList, "Active", DateTime.MinValue, DateTime.MinValue);

                    patientProblemList = new PatientProblem().getPatientProblemsFromList(patientIdList, "Active", DateTime.MinValue, DateTime.MinValue, false);

                    patientEncList = new PatientEncounter().encounterDetailsFromList(appointmentIdList, null);

                    Activity model;
                    string patientId = string.Empty;

                    foreach (Appointment appointment in appointments)
                    {
                        model = new Activity();

                        //  if (patientList.TryGetValue(Convert.ToDecimal(contract.parmPatientRecId), out patientId))
                        if (patientIdList.Contains(appointment.patient.patientId))
                        {
                            patientId = appointment.patient.patientId;

                            model.patient = appointment.patient; //this.updatePatientDetails(model, appointment.patient, patientId);

                            model.caseId = appointment.caseId;  //.parmCaseId;

                            this.updatePatientAllergies(model, patAllergyList, patientId);

                            this.updatePatientProblems(model, patientProblemList, patientId);

                            model.appointment = appointment;    //this.updatePatientAppointment(model, contract);

                            if (model != null)
                            {
                                //Calculate TimeZone
                                double result = 0;

                                if (!string.IsNullOrEmpty(timezone))
                                {
                                    double.TryParse(timezone.Substring(0, 3), out result);
                                }

                                model.activityDateTime = Convert.ToDateTime(model.appointment.startDateTime).AddHours(result).ToString();
                                //model.activityDateTime = model.appointment.startDateTime;

                                this.updateOPStatusEnhanced(model, (msdyn_bookingsystemstatus)appointment.statusValue, mzk_encountertype.Triage, false, patientEncList);        // Need to work on this

                                model.type = (int)mzk_encountertype.Triage;
                                model.EncounterSigoffLabel = Activity.EncounterSigoff.SignOff.GetDescription();

                                listModel.Add(model);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listModel;
        }

        public async Task<List<Activity>> getTreatmentWorklist(string clinicId, DateTime date, string searchOrder, bool forSpecialTest = false, string timezone = null)
        {
            List<Activity> listModel = new List<Activity>();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_patientorder.EntityLogicalName);

                query.ColumnSet = new ColumnSet(true);

                query.Criteria.AddCondition("mzk_treatmentlocation", ConditionOperator.Equal, new Guid(clinicId));
                query.Criteria.AddCondition("mzk_statusmanagerdetail", ConditionOperator.NotNull);
                query.Criteria.AddCondition("mzk_fulfillmentdate", ConditionOperator.Equal, date);

                if (forSpecialTest)
                {
                    query.Criteria.AddCondition("mzk_type", ConditionOperator.Equal, (int)mzk_patientordermzk_Type.SpecialTest);
                }
                else
                {
                    FilterExpression mainFilter = query.Criteria.AddFilter(LogicalOperator.Or);

                    FilterExpression filter1 = new FilterExpression(LogicalOperator.And);
                    filter1.Conditions.Add(new ConditionExpression("mzk_type", ConditionOperator.Equal, (int)mzk_patientordermzk_Type.Medication));
                    filter1.Conditions.Add(new ConditionExpression("mzk_urgency", ConditionOperator.Equal, (int)mzk_patientordermzk_Urgency.Stat));

                    FilterExpression filter2 = new FilterExpression(LogicalOperator.And);
                    filter2.Conditions.Add(new ConditionExpression("mzk_type", ConditionOperator.Equal, (int)mzk_patientordermzk_Type.Procedure));

                    mainFilter.AddFilter(filter1);
                    mainFilter.AddFilter(filter2);
                }

                OrderExpression orderby = new OrderExpression();
                orderby.AttributeName = "createdon";
                orderby.OrderType = OrderType.Ascending;
                query.Orders.Add(orderby);

                LinkEntity entityTypeProduct = new LinkEntity(mzk_patientorder.EntityLogicalName, Product.EntityLogicalName, "mzk_productid", "productid", JoinOperator.Inner);

                entityTypeProduct.LinkCriteria.AddCondition("productstructure", ConditionOperator.Equal, (int)ProductProductStructure.Product);

                query.LinkEntities.Add(entityTypeProduct);


                LinkEntity EntityEncounter = new LinkEntity(mzk_patientorder.EntityLogicalName, mzk_patientencounter.EntityLogicalName, "mzk_patientencounterid", "mzk_patientencounterid", JoinOperator.Inner);

                EntityEncounter.Columns = new ColumnSet(true);
                EntityEncounter.EntityAlias = "mzk_patientencounter";

                LinkEntity EntityCase = new LinkEntity(mzk_patientencounter.EntityLogicalName, Incident.EntityLogicalName, "mzk_caseid", "incidentid", JoinOperator.Inner);
                EntityCase.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("customerid", "incidentid");
                EntityCase.EntityAlias = "case";
                EntityCase.LinkCriteria.AddCondition("mzk_casetype", ConditionOperator.Equal, (int)mzk_casetype.OutPatient);

                query.LinkEntities.Add(EntityEncounter);
                EntityEncounter.LinkEntities.Add(EntityCase);

                LinkEntity appointments = null;
                LinkEntity entityTypeDetails = new LinkEntity(mzk_patientorder.EntityLogicalName, Contact.EntityLogicalName, "mzk_customer", "contactid", JoinOperator.Inner);
                entityTypeDetails.Columns = new ColumnSet("contactid", "entityimage", "gendercode", "birthdate",
                                                            "fullname",
                                                            "telephone1",
                                                            "fax",
                                                            "emailaddress1",
                                                            "address1_name",
                                                            "address1_city",
                                                            "address1_stateorprovince",
                                                            "address1_line1",
                                                            "address1_line2",
                                                            "address1_line3",
                                                            "address1_country",
                                                            "address1_postalcode",
                                                            "mzk_patientlanguage",
                                                            "familystatuscode",
                                                            "mzk_nationality",
                                                            "mzk_salutation",
                                                            "mzk_nationalidexpirydate",
                                                            "mzk_nationalidtype",
                                                            "preferredcontactmethodcode",
                                                            "mzk_vippatient");

                if (!string.IsNullOrEmpty(searchOrder))
                {
                    entityTypeDetails.LinkCriteria.AddCondition("fullname", ConditionOperator.Like, ("%" + searchOrder + "%"));
                }
                entityTypeDetails.EntityAlias = "contact"; 
                query.LinkEntities.Add(entityTypeDetails);

                LinkEntity account = new LinkEntity(Contact.EntityLogicalName, Account.EntityLogicalName, "contactid", "primarycontactid", JoinOperator.LeftOuter);
                account.Columns = new ColumnSet(true);
                account.EntityAlias = "account";
                entityTypeDetails.LinkEntities.Add(account);

                appointments = new LinkEntity(mzk_patientencounter.EntityLogicalName, mzk_patientappointment.EntityLogicalName, "mzk_appointment", "mzk_patientappointmentid", JoinOperator.Inner);

                appointments.Columns = new ColumnSet(true);
                appointments.EntityAlias = "appointment";

                EntityEncounter.LinkEntities.Add(appointments);
                
    
                LinkEntity bookingstatus = new LinkEntity(mzk_patientappointment.EntityLogicalName, BookingStatus.EntityLogicalName, "mzk_appointmentstatus", "bookingstatusid", JoinOperator.Inner);
                bookingstatus.Columns = new ColumnSet(true);
                bookingstatus.EntityAlias = "bookingstatus";

                appointments.LinkEntities.Add(bookingstatus);


                if (!string.IsNullOrEmpty(searchOrder))
                {
                    FilterExpression childFilter = account.LinkCriteria.AddFilter(LogicalOperator.Or);

                    childFilter.AddCondition("accountnumber", ConditionOperator.Like, ("%" + searchOrder + "%"));
                    childFilter.AddCondition("name", ConditionOperator.Like, ("%" + searchOrder + "%"));
                }

                EntityCollection entitycol = repo.GetEntityCollection(query);
                
                if (entitycol != null && entitycol.Entities != null && entitycol.Entities.Count > 0)
                {
                    Activity model;
                    string patientId = string.Empty;

                    List<string> appointmentList = new List<string>();

                    List<mzk_patientorder> orderList = new List<mzk_patientorder>();
                    List<string> patientIdList = new List<string>();
                    List<PatientAllergy> patAllergyList = new List<PatientAllergy>();
                    List<PatientProblem> patientProblemList = new List<PatientProblem>();
                    List<PatientEncounter> patientEncList = new List<PatientEncounter>();

                    List<StatusManager> statusManagerProcedure = new List<StatusManager>();
                    List<StatusManager> statusManagerMedication = new List<StatusManager>();
                    List<StatusManager> statusManagerSpecialTest = new List<StatusManager>();

                    if (forSpecialTest)
                    {
                        statusManagerSpecialTest = new StatusManager().getHierarchyByType(mzk_entitytype.SpecialTestOrder, mzk_casetype.OutPatient);

                        if (statusManagerSpecialTest == null || statusManagerSpecialTest.Count == 0)
                        {
                            throw new ValidationException("Status manager for Special Test not found. Please contact system administrator");
                        }
                    }
                    else
                    {
                        statusManagerProcedure = new StatusManager().getHierarchyByType(mzk_entitytype.ProcedureOrder, mzk_casetype.OutPatient);
                        statusManagerMedication = new StatusManager().getHierarchyByType(mzk_entitytype.MedicationOrder, mzk_casetype.OutPatient);

                        if (statusManagerMedication == null || statusManagerMedication.Count == 0)
                        {
                            throw new ValidationException("Status manager for Medication not found. Please contact system administrator");
                        }

                        if (statusManagerProcedure == null || statusManagerProcedure.Count == 0)
                        {
                            throw new ValidationException("Status manager for Procedure not found. Please contact system administrator");
                        }
                    }

                    foreach (Entity entity in entitycol.Entities)
                    {
                        mzk_patientorder order = (mzk_patientorder)entity;                        

                        StatusManager statusManagerObj = new StatusManager(order.mzk_StatusManagerDetail.Id.ToString());
                        mzk_orderstatus status = (mzk_orderstatus)order.mzk_OrderStatus.Value;
                        mzk_patientordermzk_Type orderType = (mzk_patientordermzk_Type)order.mzk_Type.Value;

                        if (statusManagerObj.filterHierarchyByType(orderType == mzk_patientordermzk_Type.Procedure ? statusManagerProcedure : (orderType == mzk_patientordermzk_Type.Medication ? statusManagerMedication : statusManagerSpecialTest)))
                        {
                            List<ActionManager> actionList = statusManagerObj.getNextActionList();

                            if (!statusManagerObj.isFulfilmentAction())
                            {
                                if (actionList == null || actionList.Count == 0)
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }

                        orderList.Add(order);
                        patientIdList.Add(order.mzk_customer.Id.ToString());

                        if (order.Attributes.Contains("mzk_fulfillmentappointment"))       // Need to work on this
                        {
                            appointmentList.Add((order.Attributes["mzk_fulfillmentappointment"] as EntityReference).Id.ToString());
                        }
                        else if (order.Attributes.Contains("mzk_orderingappointment"))
                        {
                            appointmentList.Add((order.Attributes["mzk_orderingappointment"] as EntityReference).Id.ToString());
                        }
                    }

                    if (orderList != null && orderList.Count > 0)
                    {
                        patAllergyList = new PatientAllergy().getPatientAllergiesFromList(patientIdList, "Active", DateTime.MinValue, DateTime.MinValue);

                        patientProblemList = new PatientProblem().getPatientProblemsFromList(patientIdList, "Active", DateTime.MinValue, DateTime.MinValue, false);

                        foreach (mzk_patientorder order in orderList)
                        {
                            model = new Activity();

                            Patient patient = new Patient();

                            if (order.mzk_customer != null)
                            {
                                patientId = order.mzk_customer.Id.ToString();

                                string appointmentId = "";

                                {
                                    List<PatientEncounter> listEnc = patientEncList.Where(item => item.EncounterId == order.mzk_PatientEncounterId.Id.ToString()).ToList();

                                    if (listEnc != null && listEnc.Count > 0)
                                    {
                                        appointmentId = listEnc[0].AppointmentId;
                                    }
                                }

                                if (appointmentList.Any(item => item == appointmentId))
                                {
                                    continue;
                                }
                                else
                                {
                                    appointmentList.Add(appointmentId);
                                }


                                Appointment appointment = Appointment.getAppointmentModelFilledByAppoinment(order, new Appointment(), "AppointmentBooking");

                                if (appointment != null)
                                {

                                    if (order.Attributes.Contains("bookingstatus.msdyn_fieldservicestatus"))
                                    {
                                        appointment.statusValue = ((OptionSetValue)(order.Attributes["bookingstatus.msdyn_fieldservicestatus"] as AliasedValue).Value).Value;
                                        appointment.status = (order.FormattedValues["bookingstatus.msdyn_fieldservicestatus"]).ToString();
                                    }

                                    model.patient = Patient.getPatientModelFilled(order, new Patient(), "contact", "account");    //this.updatePatientDetails(model, contract, patientId);

                                    if (order.Attributes.Contains("case.incidentid"))
                                    {
                                        model.caseId = (order.Attributes["case.incidentid"] as AliasedValue).Value.ToString();
                                        appointment.caseId = (order.Attributes["case.incidentid"] as AliasedValue).Value.ToString();
                                    }

                                    this.updatePatientAllergies(model, patAllergyList, patientId);

                                    this.updatePatientProblems(model, patientProblemList, patientId);

                                    model.appointment = appointment;    //this.updatePatientAppointment(model, contract);

                                    //Calculate TimeZone
                                    double result = 0;

                                    if (!string.IsNullOrEmpty(timezone))
                                    {
                                        double.TryParse(timezone.Substring(0, 3), out result);
                                    }

                                    model.activityDateTime = Convert.ToDateTime(model.appointment.startDateTime).AddHours(result).ToString();
                                    //model.activityDateTime = model.appointment.startDateTime;

                                    this.updateOPStatusEnhanced(model, (msdyn_bookingsystemstatus)appointment.statusValue, mzk_encountertype.Treatment, false, patientEncList);

                                    model.type = (int)mzk_encountertype.Treatment;
                                    model.EncounterSigoffLabel = Activity.EncounterSigoff.SignOff.GetDescription();

                                    listModel.Add(model);
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listModel;
        }

        public async Task<List<Activity>> getERWorklist(string clinicId, DateTime date, string searchOrder, mzk_encountertype encounterType, string userId, string timezone = null)
        {
            List<Activity> listModel = new List<Activity>();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(Incident.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                query.Criteria.AddCondition("mzk_organizationalunit", ConditionOperator.Equal, new Guid(clinicId));

                query.Criteria.AddCondition("mzk_casedate", ConditionOperator.Equal, date);
                query.Criteria.AddCondition("mzk_casetype", ConditionOperator.Equal, (int)mzk_casetype.Emergency);

                OrderExpression orderby = new OrderExpression();
                orderby.AttributeName = "createdon";
                orderby.OrderType = OrderType.Ascending;
                query.Orders.Add(orderby);

                LinkEntity contact = new LinkEntity(Incident.EntityLogicalName, Contact.EntityLogicalName, "customerid", "contactid", JoinOperator.Inner);
                contact.Columns = new ColumnSet("contactid", "entityimage", "gendercode", "birthdate",
                                                "fullname",
                                                "telephone1",
                                                "fax",
                                                "emailaddress1",
                                                "address1_name",
                                                "address1_city",
                                                "address1_stateorprovince",
                                                "address1_line1",
                                                "address1_line2",
                                                "address1_line3",
                                                "address1_country",
                                                "address1_postalcode",
                                                "mzk_patientlanguage",
                                                "familystatuscode",
                                                "mzk_nationality",
                                                "mzk_salutation",
                                                "mzk_nationalidexpirydate",
                                                "mzk_nationalidtype",
                                                "preferredcontactmethodcode",
                                                "mzk_vippatient");

                if (!string.IsNullOrEmpty(searchOrder))
                {
                    contact.LinkCriteria.AddCondition("fullname", ConditionOperator.Like, ("%" + searchOrder + "%"));
                }

                contact.EntityAlias = "contact";
                query.LinkEntities.Add(contact);

                LinkEntity account = new LinkEntity(Contact.EntityLogicalName, Account.EntityLogicalName, "contactid", "primarycontactid", JoinOperator.LeftOuter);
                account.Columns = new ColumnSet(true);

                if (!string.IsNullOrEmpty(searchOrder))
                {
                    FilterExpression childFilter = account.LinkCriteria.AddFilter(LogicalOperator.Or);

                    childFilter.AddCondition("accountnumber", ConditionOperator.Like, ("%" + searchOrder + "%"));
                    childFilter.AddCondition("name", ConditionOperator.Like, ("%" + searchOrder + "%"));
                }

                account.EntityAlias = "account";
                contact.LinkEntities.Add(account);

                EntityCollection entitycol = repo.GetEntityCollection(query);
                Activity model;
                string patientId = string.Empty;

                List<Incident> caseList = new List<Incident>();
                List<string> patientIdList = new List<string>();
                List<PatientAllergy> patAllergyList = new List<PatientAllergy>();
                List<PatientProblem> patientProblemList = new List<PatientProblem>();
                Dictionary<string, long> patientList = null;

                foreach (Entity entity in entitycol.Entities)
                {
                    Incident patientCase = (Incident)entity;

                    if (encounterType == mzk_encountertype.Assessment)
                    {
                        PatientReferralOrder referral = new PatientReferralOrder();

                        if (referral.referralExist(patientCase.IncidentId.Value.ToString(), userId) == null)
                        {
                            continue;
                        }
                    }
                    caseList.Add(patientCase);
                    patientIdList.Add(patientCase.CustomerId.Id.ToString());
                }

                patientList = new Patient().getPatientRecIdListFromGuidList(patientIdList);

                patAllergyList = new PatientAllergy().getPatientAllergiesFromList(patientIdList, "Active", DateTime.MinValue, DateTime.MinValue);

                patientProblemList = new PatientProblem().getPatientProblemsFromList(patientIdList, "Active", DateTime.MinValue, DateTime.MinValue, false);

                foreach (Incident patientCase in caseList)      // Need to work on this
                {
                    model = new Activity();

                    Patient patient = new Patient();

                    if (patientCase.CustomerId != null)
                    {
                        model.patient = Patient.getPatientModelFilled(patientCase, new Models.Patient(), contact.EntityAlias, account.EntityAlias);

                        this.updatePatientAllergies(model, patAllergyList, patientId);

                        this.updatePatientProblems(model, patientProblemList, patientId);

                        model.appointment = new Appointment();
                        model.appointment.appointmentId = "0";

                        model.appointment.careTeam = new List<User>();

                        if (model != null)
                        {
                            model.caseId = patientCase.IncidentId.Value.ToString();
                            //Calculate TimeZone
                            double result = 0;

                            if (!string.IsNullOrEmpty(timezone))
                            {
                                double.TryParse(timezone.Substring(0, 3), out result);
                            }

                            if (patientCase.CreatedOn.HasValue)
                                model.activityDateTime = patientCase.CreatedOn.Value.AddHours(result).ToString();

                            mzk_casestatus caseStatus = (mzk_casestatus)patientCase.mzk_casestatus.Value;

                            this.updateERStatus(model, encounterType, userId, caseStatus);

                            model.type = (int)encounterType;

                            if (encounterType == mzk_encountertype.PrimaryAssessment)
                            {
                                model.EncounterSigoffLabel = Activity.EncounterSigoff.ClinicallyDischarge.GetDescription();
                            }
                            else if (encounterType == mzk_encountertype.Treatment)
                            {
                                model.EncounterSigoffLabel = Activity.EncounterSigoff.PhysicallyDischarge.GetDescription();
                            }
                            else
                            {
                                model.EncounterSigoffLabel = Activity.EncounterSigoff.SignOff.GetDescription();
                            }

                            listModel.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listModel;
        }

        public async Task<List<Activity>> getCasePathwayStateActivityWorkflowWorklist(DateTime date, string worklistTypeId, mzk_encountertype encounterType, string searchOrder, string userId, string timezone = null)
        {
            List<Activity> listModel = new List<Activity>();

            try
            {
                SoapEntityRepository repo = SoapEntityRepository.GetService();
                QueryExpression query = new QueryExpression(mzk_casepathwaystateactivityworkflow.EntityLogicalName);

                query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

                FilterExpression filter = query.Criteria.AddFilter(LogicalOperator.Or);

                filter.AddCondition("mzk_status", ConditionOperator.Equal, (int)mzk_casepathwaystatus.Started);

                query.Criteria.AddCondition("mzk_date", ConditionOperator.Between, date, date.AddHours(23).AddMinutes(59).AddSeconds(59));

                LinkEntity cpsaWorkflowRole = new LinkEntity()
                {
                    Columns = new ColumnSet(true),
                    EntityAlias = "CPSAWR",
                    LinkFromEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                    LinkToEntityName = mzk_casepathwaystateactivityworkflowrole.EntityLogicalName,
                    LinkFromAttributeName = "mzk_casepathwaystateactivityworkflowid",
                    LinkToAttributeName = "mzk_casepathwaystateactivityworkflow",
                    JoinOperator = JoinOperator.Inner
                };

                LinkEntity casePathwayState = new LinkEntity
                {
                    Columns = new ColumnSet(false),
                    EntityAlias = "CPS",
                    LinkFromEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                    LinkToEntityName = mzk_casepathwaystate.EntityLogicalName,
                    LinkFromAttributeName = "mzk_casepathwaystate",
                    LinkToAttributeName = "mzk_casepathwaystateid",
                    JoinOperator = JoinOperator.Inner

                };

                LinkEntity activityWorkflow = new LinkEntity
                {
                    Columns = new ColumnSet(false),
                    EntityAlias = "AW",
                    LinkFromEntityName = mzk_casepathwaystateactivityworkflow.EntityLogicalName,
                    LinkToEntityName = mzk_activityworkflow.EntityLogicalName,
                    LinkFromAttributeName = "mzk_activityworkflow",
                    LinkToAttributeName = "mzk_activityworkflowid",
                    JoinOperator = JoinOperator.Inner

                };
                activityWorkflow.LinkCriteria.AddCondition("mzk_activityworkflowtype", ConditionOperator.Equal, (int)mzk_activityworkflowentitytype.Clinical);

                query.LinkEntities.Add(cpsaWorkflowRole);
                query.LinkEntities.Add(casePathwayState);
                query.LinkEntities.Add(activityWorkflow);

                LinkEntity caseCareTeamMember = new LinkEntity()
                {
                    Columns = new ColumnSet(false),
                    EntityAlias = "CCTM",
                    LinkFromEntityName = mzk_casepathwaystateactivityworkflowrole.EntityLogicalName,
                    LinkToEntityName = mzk_casecareteammember.EntityLogicalName,
                    LinkFromAttributeName = "mzk_casecareteammember",
                    LinkToAttributeName = "mzk_casecareteammemberid",
                    JoinOperator = JoinOperator.Inner
                };
                caseCareTeamMember.LinkCriteria.AddCondition("mzk_user", ConditionOperator.Equal, new Guid(userId));
                cpsaWorkflowRole.LinkEntities.Add(caseCareTeamMember);

                LinkEntity casePathway = new LinkEntity
                {
                    Columns = new ColumnSet(false),
                    EntityAlias = "CP",
                    LinkFromEntityName = mzk_casepathwaystate.EntityLogicalName,
                    LinkToEntityName = mzk_casepathway.EntityLogicalName,
                    LinkFromAttributeName = "mzk_casepathway",
                    LinkToAttributeName = "mzk_casepathwayid",
                    JoinOperator = JoinOperator.Inner
                };
                casePathwayState.LinkEntities.Add(casePathway);

                LinkEntity patientCase = new LinkEntity
                {
                    Columns = new ColumnSet("customerid", "incidentid", "mzk_axclinicrefrecid", "createdon", "mzk_casestatus"),
                    EntityAlias = "PC",
                    LinkFromEntityName = mzk_casepathway.EntityLogicalName,
                    LinkToEntityName = xrm.Incident.EntityLogicalName,
                    LinkFromAttributeName = "mzk_caseid",
                    LinkToAttributeName = "incidentid",
                    JoinOperator = JoinOperator.Inner
                };
                casePathway.LinkEntities.Add(patientCase);

                LinkEntity uiTemplate = new LinkEntity
                {
                    Columns = new ColumnSet(false),
                    EntityAlias = "UIT",
                    LinkFromEntityName = mzk_activityworkflow.EntityLogicalName,
                    LinkToEntityName = mzk_uitemplate.EntityLogicalName,
                    LinkFromAttributeName = "mzk_uitemplate",
                    LinkToAttributeName = "mzk_uitemplateid",
                    JoinOperator = JoinOperator.Inner

                };
                uiTemplate.LinkCriteria.AddCondition("mzk_worklisttypeid", ConditionOperator.Equal, new Guid(worklistTypeId));

                activityWorkflow.LinkEntities.Add(uiTemplate);

                EntityCollection entitycol = repo.GetEntityCollection(query);
                Activity model;
                string patientId = string.Empty;
                string caseId = string.Empty;

                List<xrm.Incident> caseList = new List<xrm.Incident>();
                List<string> patientIdList = new List<string>();
                List<string> caseIdList = new List<string>();
                List<PatientAllergy> patAllergyList = new List<PatientAllergy>();
                List<User> careTeamMembersList = new List<User>();
                List<PatientProblem> patientProblemList = new List<PatientProblem>();
                Dictionary<string, long> patientList = null;

                foreach (Entity entity in entitycol.Entities)
                {
                    if (entity.Attributes.Contains("PC.customerid"))
                    {
                        patientIdList.Add(((entity.Attributes["PC.customerid"] as AliasedValue).Value as EntityReference).Id.ToString());
                    }

                    if (entity.Attributes.Contains("PC.incidentid"))
                    {
                        caseIdList.Add(((Microsoft.Xrm.Sdk.AliasedValue)(entity.Attributes["PC.incidentid"])).Value.ToString());
                    }
                }

                patientList = new Patient().getPatientRecIdListFromGuidList(patientIdList);

                patAllergyList = new PatientAllergy().getPatientAllergiesFromList(patientIdList, "Active", DateTime.MinValue, DateTime.MinValue);

                careTeamMembersList = new User().getCareTeamUsersFromList(caseIdList);

                patientProblemList = new PatientProblem().getPatientProblemsFromList(patientIdList, "Active", DateTime.MinValue, DateTime.MinValue, false);

                foreach (Entity entity in entitycol.Entities)
                {
                    model = new Activity();

                    Patient patient = new Patient();

                    // long patientRecId = 0;                    

                    //  if (entity.Attributes.Contains("PC.mzk_accountid") && patientList.TryGetValue(((entity.Attributes["PC.mzk_accountid"] as AliasedValue).Value as EntityReference).Id.ToString(), out patientRecId))
                    {
                        patientId = ((entity.Attributes["PC.customerid"] as AliasedValue).Value as EntityReference).Id.ToString();

                        if (entity.Attributes.Contains("PC.incidentid"))
                        {
                            caseId = ((Microsoft.Xrm.Sdk.AliasedValue)(entity.Attributes["PC.incidentid"])).Value.ToString();
                        }

                        // model.patient = new Patient();

                        //PatientRepository patientRepo = new PatientRepository();
                        //  HMPatientInfoContract patientContract = patientRepo.GetPatientBasicDetails(patientRecId.ToString());

                        model.patient = new Models.Patient().getPatientDetails(patientId).Result;
                        model.patient.patientId = patientId;

                        /* model.patient.name = patientContract.parmFullName;
                         model.patient.age = patientContract.parmAge;
                         model.patient.mrn = patientContract.MRN;
                         model.patient.dateOfBirth = patientContract.DateOfBirth;
                         model.patient.gender = patientContract.Gender;
                         model.patient.registrationDate = patientContract.parmRegistrationDate;
                         model.patient.patientId = patientId;
                         model.patient.email = patientContract.parmEmail;
                         model.patient.phone = patientContract.parmPhone;*/

                        this.updatePatientAllergies(model, patAllergyList, patientId);

                        this.updatePatientProblems(model, patientProblemList, patientId);

                        model.appointment = new Appointment();
                        model.appointment.appointmentId = "0";

                        this.UpdateCaseCareTeamMembers(model, careTeamMembersList, caseId);

                        if (model != null)
                        {
                            model.caseId = (entity.Attributes["PC.incidentid"] as AliasedValue).Value.ToString();
                            //Calculate TimeZone
                            double result = 0;

                            if (!string.IsNullOrEmpty(timezone))
                            {
                                double.TryParse(timezone.Substring(0, 3), out result);
                            }

                            if (entity.Contains("mzk_date"))
                            {
                                DateTime dt;
                                if (DateTime.TryParse((entity.Attributes["mzk_date"]).ToString(), out dt))
                                {
                                    model.activityDateTime = dt.AddHours(result).ToString();
                                }
                            }

                            model.casePathwayActivityWorkflowId = entity.Attributes["mzk_casepathwaystateactivityworkflowid"].ToString();

                            this.updateCCStatus(model, encounterType);

                            model.type = (int)encounterType;

                            //if (encounterType == mzk_encountertype.PrimaryAssessment)
                            //{
                            //    model.EncounterSigoffLabel = Activity.EncounterSigoff.ClinicallyDischarge.GetDescription();
                            //}
                            //else if (encounterType == mzk_encountertype.Treatment)
                            //{
                            //    model.EncounterSigoffLabel = Activity.EncounterSigoff.PhysicallyDischarge.GetDescription();
                            //}
                            //else
                            //{
                            //    model.EncounterSigoffLabel = Activity.EncounterSigoff.SignOff.GetDescription();
                            //}

                            listModel.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listModel;
        }
        async void updateOPStatus(Activity model, HMResourceSchedulingAppointmentStatus apptStatus, mzk_encountertype encounterType, bool isTriageRequired)
        {
            Activity.EncounterCreation encounterCreation = Activity.EncounterCreation.None;
            mzk_activitystatus activityStatus = mzk_activitystatus.None; 
            bool encounterExist = false;
            bool encounterSigned = false;
            
            PatientEncounter enc = new PatientEncounter();

            enc = await enc.encounterDetails((int)encounterType, "", "", model.appointment.appointmentId);

            if (enc != null)
            {
                if (enc.EncounterId != null && enc.EncounterId != string.Empty)
                {
                    encounterExist = true;

                    if (enc.EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                    {
                        encounterSigned = true;
                    }
                }
            }

            if (apptStatus == HMResourceSchedulingAppointmentStatus.Confirmed)
            {
                encounterCreation = Activity.EncounterCreation.NoAction;
                activityStatus = mzk_activitystatus.Open;
            }
            else if (apptStatus == HMResourceSchedulingAppointmentStatus.CheckedOut)
            {
                encounterCreation = Activity.EncounterCreation.NoAction;
                //  activityStatus = HMActivityStatus.;
            }
            else if (apptStatus == HMResourceSchedulingAppointmentStatus.CheckedIn)
            {
                if (encounterExist)
                {
                    if (encounterSigned)
                    {
                        encounterCreation = Activity.EncounterCreation.NoAction;

                        switch (encounterType)
                        {
                            case mzk_encountertype.Triage:
                                activityStatus = mzk_activitystatus.TriageComplete;
                                break;
                            case mzk_encountertype.Consultation:
                                activityStatus = mzk_activitystatus.ConsultationComplete;
                                break;
                            case mzk_encountertype.Treatment:
                                activityStatus = mzk_activitystatus.TreatmentComplete;
                                break;
                        }
                    }
                    else
                    {
                        encounterCreation = Activity.EncounterCreation.ResumeEncounter;
                        model.encounterId = enc.EncounterId;

                        switch (encounterType)
                        {
                            case mzk_encountertype.Triage:
                                activityStatus = mzk_activitystatus.InTriage;
                                break;
                            case mzk_encountertype.Consultation:
                                activityStatus = mzk_activitystatus.InConsultation;
                                break;
                            case mzk_encountertype.Treatment:
                                activityStatus = mzk_activitystatus.InTreatment;
                                break;
                        }
                    }
                }
                else
                {
                    switch (encounterType)
                    {
                        case mzk_encountertype.Triage:
                            activityStatus = mzk_activitystatus.PendingTriage;
                            encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            break;
                        case mzk_encountertype.Consultation:
                            if (isTriageRequired)
                            {
                                enc = await enc.encounterDetails((int)mzk_encountertype.Triage, "", "", model.appointment.appointmentId);

                                if (enc != null && !string.IsNullOrEmpty(enc.EncounterId) && enc.EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                                {
                                    activityStatus = mzk_activitystatus.PendingConsultation;
                                    encounterCreation = Activity.EncounterCreation.CreateEncounter;
                                }
                                else
                                {
                                    activityStatus = mzk_activitystatus.Open;
                                    encounterCreation = Activity.EncounterCreation.NoAction;
                                }
                            }
                            else
                            {
                                activityStatus = mzk_activitystatus.PendingConsultation;
                                encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            }
                            break;
                        case mzk_encountertype.Treatment:
                            activityStatus = mzk_activitystatus.PendingTreatment;
                            encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            break;
                    }
                }
            }

            model.EncounterCreationStatus = encounterCreation;
            model.Status = (int)activityStatus;

            if (activityStatusList != null)
            {
                var selected = activityStatusList.Where(item => item.parmID == model.Status.ToString()).First();

                model.StatusText = selected.parmDescription;
                model.colorCode = selected.parmColorHex;
            }
        }

        async void updateOPStatusEnhanced(Activity model, HMResourceSchedulingAppointmentStatus apptStatus, mzk_encountertype encounterType, bool isTriageRequired, List<PatientEncounter> listPatEnc)
        {
            Activity.EncounterCreation encounterCreation = Activity.EncounterCreation.None;
            HMActivityStatus activityStatus = HMActivityStatus.None;
            bool encounterExist = false;
            bool encounterSigned = false;

            if (apptStatus == HMResourceSchedulingAppointmentStatus.Confirmed)
            {
                encounterCreation = Activity.EncounterCreation.NoAction;
                activityStatus = HMActivityStatus.Open;
            }
            else if (apptStatus == HMResourceSchedulingAppointmentStatus.CheckedOut)
            {
                encounterCreation = Activity.EncounterCreation.NoAction;
                //  activityStatus = HMActivityStatus.CheckedOut;
            }
            else if (apptStatus == HMResourceSchedulingAppointmentStatus.CheckedIn)
            {
                List<PatientEncounter> listPat = listPatEnc.Where(item => item.AppointmentId.ToString() == model.appointment.appointmentId).ToList();
                PatientEncounter enc = new PatientEncounter();

                if (listPat != null && listPat.Count > 0)
                {
                    enc = listPat.Where(item => item.EncounterType == ((int)encounterType).ToString()).FirstOrDefault();

                    if (enc != null)
                    {
                        if (enc.EncounterId != null && enc.EncounterId != string.Empty)
                        {
                            encounterExist = true;

                            if (enc.EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                            {
                                encounterSigned = true;
                            }
                        }
                    }
                }

                if (encounterExist)
                {
                    if (encounterSigned)
                    {
                        encounterCreation = Activity.EncounterCreation.NoAction;

                        switch (encounterType)
                        {
                            case mzk_encountertype.Triage:
                                activityStatus = HMActivityStatus.TriageComplete;
                                break;
                            case mzk_encountertype.Consultation:
                                activityStatus = HMActivityStatus.ConsultationComplete;
                                break;
                            case mzk_encountertype.Treatment:
                                activityStatus = HMActivityStatus.TreatmentComplete;
                                break;
                        }
                    }
                    else
                    {
                        encounterCreation = Activity.EncounterCreation.ResumeEncounter;
                        model.encounterId = enc.EncounterId;

                        switch (encounterType)
                        {
                            case mzk_encountertype.Triage:
                                activityStatus = HMActivityStatus.InTriage;
                                break;
                            case mzk_encountertype.Consultation:
                                activityStatus = HMActivityStatus.InConsultation;
                                break;
                            case mzk_encountertype.Treatment:
                                activityStatus = HMActivityStatus.InTreatment;
                                break;
                        }
                    }
                }
                else
                {
                    switch (encounterType)
                    {
                        case mzk_encountertype.Triage:
                            activityStatus = HMActivityStatus.PendingTriage;
                            encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            break;
                        case mzk_encountertype.Consultation:
                            if (isTriageRequired)
                            {
                                if (listPat != null && listPat.Count > 0)
                                {
                                    enc = listPat.Where(item => item.EncounterType == ((int)mzk_encountertype.Triage).ToString()).FirstOrDefault();

                                    // enc = await enc.encounterDetails(Convert.ToInt64(model.appointment.appointmentId), (int)mzk_encountertype.Triage, "");

                                    if (enc != null && !string.IsNullOrEmpty(enc.EncounterId) && enc.EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                                    {
                                        activityStatus = HMActivityStatus.PendingConsultation;
                                        encounterCreation = Activity.EncounterCreation.CreateEncounter;
                                    }
                                    else
                                    {
                                        activityStatus = HMActivityStatus.Open;
                                        encounterCreation = Activity.EncounterCreation.NoAction;
                                    }
                                }
                                else
                                {
                                    activityStatus = HMActivityStatus.Open;
                                    encounterCreation = Activity.EncounterCreation.NoAction;
                                }
                            }
                            else
                            {
                                activityStatus = HMActivityStatus.PendingConsultation;
                                encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            }
                            break;
                        case mzk_encountertype.Treatment:
                            activityStatus = HMActivityStatus.PendingTreatment;
                            encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            break;
                    }
                }
            }

            model.EncounterCreationStatus = encounterCreation;
            model.Status = (int)activityStatus;

            if (activityStatusList != null)
            {
                var selected = activityStatusList.Where(item => item.parmID == model.Status.ToString()).First();

                model.StatusText = selected.parmDescription;
                model.colorCode = selected.parmColorHex;
            }
        }

        async void updateOPStatusEnhanced(Activity model, msdyn_bookingsystemstatus apptStatus, mzk_encountertype encounterType, bool isTriageRequired, List<PatientEncounter> listPatEnc)
        {
            Activity.EncounterCreation encounterCreation = Activity.EncounterCreation.None;
            mzk_activitystatus activityStatus = mzk_activitystatus.None;
            bool encounterExist = false;
            bool encounterSigned = false;

            if (apptStatus == msdyn_bookingsystemstatus.Scheduled)
            {
                encounterCreation = Activity.EncounterCreation.NoAction;
                activityStatus = mzk_activitystatus.Open;
            }
            else if (apptStatus == msdyn_bookingsystemstatus.Completed)
            {
                encounterCreation = Activity.EncounterCreation.NoAction;
                activityStatus = mzk_activitystatus.CheckedOut;
            }
            else if (apptStatus == msdyn_bookingsystemstatus.InProgress)
            {
                List<PatientEncounter> listPat = listPatEnc.Where(item => item.AppointmentId.ToString() == model.appointment.appointmentId).ToList();
                PatientEncounter enc = new PatientEncounter();

                if (listPat != null && listPat.Count > 0)
                {
                    enc = listPat.Where(item => item.EncounterType == ((int)encounterType).ToString()).FirstOrDefault();

                    if (enc != null)
                    {
                        if (enc.EncounterId != null && enc.EncounterId != string.Empty)
                        {
                            encounterExist = true;

                            if (enc.EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                            {
                                encounterSigned = true;
                            }
                        }
                    }
                }

                if (encounterExist)
                {
                    if (encounterSigned)
                    {
                        encounterCreation = Activity.EncounterCreation.NoAction;

                        switch (encounterType)
                        {
                            case mzk_encountertype.Triage:
                                activityStatus = mzk_activitystatus.TriageComplete;
                                break;
                            case mzk_encountertype.Consultation:
                                activityStatus = mzk_activitystatus.ConsultationComplete;
                                break;
                            case mzk_encountertype.Treatment:
                                activityStatus = mzk_activitystatus.TreatmentComplete;
                                break;
                        }
                    }
                    else
                    {
                        encounterCreation = Activity.EncounterCreation.ResumeEncounter;
                        model.encounterId = enc.EncounterId;

                        switch (encounterType)
                        {
                            case mzk_encountertype.Triage:
                                activityStatus = mzk_activitystatus.InTriage;
                                break;
                            case mzk_encountertype.Consultation:
                                activityStatus = mzk_activitystatus.InConsultation;
                                break;
                            case mzk_encountertype.Treatment:
                                activityStatus = mzk_activitystatus.InTreatment;
                                break;
                        }
                    }
                }
                else
                {
                    switch (encounterType)
                    {
                        case mzk_encountertype.Triage:
                            activityStatus = mzk_activitystatus.PendingTriage;
                            encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            break;
                        case mzk_encountertype.Consultation:
                            if (isTriageRequired)
                            {
                                if (listPat != null && listPat.Count > 0)
                                {
                                    enc = listPat.Where(item => item.EncounterType == ((int)mzk_encountertype.Triage).ToString()).FirstOrDefault();

                                    // enc = await enc.encounterDetails(Convert.ToInt64(model.appointment.appointmentId), (int)mzk_encountertype.Triage, "");

                                    if (enc != null && !string.IsNullOrEmpty(enc.EncounterId) && enc.EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                                    {
                                        activityStatus = mzk_activitystatus.PendingConsultation;
                                        encounterCreation = Activity.EncounterCreation.CreateEncounter;
                                    }
                                    else
                                    {
                                        activityStatus = mzk_activitystatus.Open;
                                        encounterCreation = Activity.EncounterCreation.NoAction;
                                    }
                                }
                                else
                                {
                                    activityStatus = mzk_activitystatus.Open;
                                    encounterCreation = Activity.EncounterCreation.NoAction;
                                }
                            }
                            else
                            {
                                activityStatus = mzk_activitystatus.PendingConsultation;
                                encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            }
                            break;
                        case mzk_encountertype.Treatment:
                            activityStatus = mzk_activitystatus.PendingTreatment;
                            encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            break;
                    }
                }
            }

            model.EncounterCreationStatus = encounterCreation;
            model.Status = (int)activityStatus;
            model.StatusText = activityStatus.ToString();

            if (activityStatusList != null)
            {
                //var selected = activityStatusList.Where(item => item.parmID == model.Status.ToString()).FirstOrDefault();

                //model.StatusText = selected.parmDescription;
                // model.colorCode = selected.parmColorHex;
            }

            if (activityStatusDropDownList != null && activityStatusDropDownList.DropDownValues != null)
            {
                var selected = activityStatusDropDownList.DropDownValues.Where(item => item.value == model.Status.ToString()).First();

                model.StatusText = selected.text;
                model.colorCode = selected.color;
            }
        }

        async void updateERStatus(Activity model, mzk_encountertype encounterType, string userId, mzk_casestatus caseStatus)
        {
            Activity.EncounterCreation encounterCreation = Activity.EncounterCreation.None;
            mzk_activitystatus activityStatus = mzk_activitystatus.None;

            PatientEncounter enc = null;
            List<PatientEncounter> encList = null;

            switch (encounterType)
            {
                case mzk_encountertype.Triage:
                    enc = new PatientEncounter();

                    enc.CaseId = model.caseId;
                    enc.EncounterType = ((int)encounterType).ToString();

                    encList = await enc.encounterDetails(enc);

                    if (encList != null)
                    {
                        if (encList.Count > 0)
                        {
                            if (encList[0].EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                            {
                                activityStatus = mzk_activitystatus.TriageComplete;
                                encounterCreation = Activity.EncounterCreation.NoAction;
                            }
                            else
                            {
                                activityStatus = mzk_activitystatus.InTriage;
                                encounterCreation = Activity.EncounterCreation.ResumeEncounter;
                                model.encounterId = encList[0].EncounterId;
                            }
                        }
                        else
                        {
                            activityStatus = mzk_activitystatus.PendingTriage;
                            encounterCreation = Activity.EncounterCreation.CreateEncounter;
                        }
                    }
                    else
                    {
                        activityStatus = mzk_activitystatus.PendingTriage;
                        encounterCreation = Activity.EncounterCreation.CreateEncounter;
                    }
                    break;
                case mzk_encountertype.PrimaryAssessment:
                    enc = new PatientEncounter();

                    enc.CaseId = model.caseId;

                    encList = await enc.encounterDetails(enc);

                    if (encList != null)
                    {
                        if (encList.Count == 0)
                        {
                            activityStatus = mzk_activitystatus.Open;
                            encounterCreation = Activity.EncounterCreation.NoAction;
                        }
                        else if (encList.Count == 1)
                        {
                            if (encList[0].EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                            {
                                activityStatus = mzk_activitystatus.PendingAssessment;
                                encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            }
                            else
                            {
                                activityStatus = mzk_activitystatus.Open;
                                encounterCreation = Activity.EncounterCreation.NoAction;
                            }
                        }
                        else
                        {
                            var encStat = encList.Where(item => item.EncounterType == ((int)encounterType).ToString());

                            if (encStat != null && encStat.Count() > 0)
                            {
                                if (encStat.First().EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                                {
                                    //activityStatus = HMActivityStatus.ClinicallyDischarged;
                                    encounterCreation = Activity.EncounterCreation.NoAction;
                                }
                                else
                                {
                                    //activityStatus = HMActivityStatus.InAssessment;
                                    encounterCreation = Activity.EncounterCreation.ResumeEncounter;
                                    model.encounterId = encStat.First().EncounterId;
                                }

                                if (caseStatus == mzk_casestatus.Open)
                                {
                                    activityStatus = mzk_activitystatus.InAssessment;
                                }
                                else if (caseStatus == mzk_casestatus.ClinicallyDischarged)
                                {
                                    activityStatus = mzk_activitystatus.ClinicallyDischarged;
                                }
                                else if (caseStatus == mzk_casestatus.FinanciallyDischarged)
                                {
                                    activityStatus = mzk_activitystatus.FinanciallyDischarged;
                                }
                                else if (caseStatus == mzk_casestatus.PhysicallyDischarged)
                                {
                                    activityStatus = mzk_activitystatus.PhysicallyDischarged;
                                }
                            }
                            else
                            {
                                activityStatus = mzk_activitystatus.PendingAssessment;
                                encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            }
                        }
                    }
                    else
                    {
                        activityStatus = mzk_activitystatus.Open;
                        encounterCreation = Activity.EncounterCreation.NoAction;
                    }
                    break;
                case mzk_encountertype.Assessment:
                    enc = new PatientEncounter();

                    enc.CaseId = model.caseId;

                    encList = await enc.encounterDetails(enc);

                    if (encList != null)
                    {
                        var encPrim = encList.Where(item => item.EncounterType == ((int)mzk_encountertype.PrimaryAssessment).ToString());

                        if (encPrim != null && encPrim.Count() > 0)
                        {
                            var encFollowList = encList.Where(item => item.EncounterType == ((int)mzk_encountertype.Assessment).ToString() && item.CreatedById == userId);

                            if (encFollowList != null && encFollowList.Count() > 0)
                            {
                                var encFollowListSorted = encFollowList.OrderByDescending(item => item.CreatedOn).First();

                                if (encFollowListSorted.EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                                {
                                    activityStatus = mzk_activitystatus.AssessmentComplete;
                                    encounterCreation = Activity.EncounterCreation.NoAction;
                                }
                                else
                                {
                                    activityStatus = mzk_activitystatus.InAssessment;
                                    encounterCreation = Activity.EncounterCreation.ResumeEncounter;
                                    model.encounterId = encFollowListSorted.EncounterId;
                                }

                            }
                            else
                            {
                                activityStatus = mzk_activitystatus.PendingAssessment;
                                encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            }
                        }
                        else
                        {
                            activityStatus = mzk_activitystatus.Open;
                            encounterCreation = Activity.EncounterCreation.NoAction;
                        }
                    }
                    else
                    {
                        activityStatus = mzk_activitystatus.Open;
                        encounterCreation = Activity.EncounterCreation.NoAction;
                    }
                    break;
                case mzk_encountertype.Treatment:
                    enc = new PatientEncounter();

                    enc.CaseId = model.caseId;

                    encList = await enc.encounterDetails(enc);

                    if (encList != null)
                    {
                        var encPrim = encList.Where(item => item.EncounterType == ((int)mzk_encountertype.PrimaryAssessment).ToString());

                        if (encPrim != null && encPrim.Count() > 0)
                        {
                            var encTreatList = encList.Where(item => item.EncounterType == ((int)mzk_encountertype.Treatment).ToString());

                            if (encTreatList != null && encTreatList.Count() > 0)
                            {
                                if (encTreatList.First().EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                                {
                                    encounterCreation = Activity.EncounterCreation.NoAction;
                                }
                                else
                                {
                                    encounterCreation = Activity.EncounterCreation.ResumeEncounter;
                                    model.encounterId = encTreatList.First().EncounterId;
                                }

                                if (caseStatus == mzk_casestatus.Open)
                                {
                                    activityStatus = mzk_activitystatus.InTreatment;
                                }
                                else if (caseStatus == mzk_casestatus.ClinicallyDischarged)
                                {
                                    activityStatus = mzk_activitystatus.ClinicallyDischarged;
                                }
                                else if (caseStatus == mzk_casestatus.FinanciallyDischarged)
                                {
                                    activityStatus = mzk_activitystatus.FinanciallyDischarged;
                                }
                                else if (caseStatus == mzk_casestatus.PhysicallyDischarged)
                                {
                                    activityStatus = mzk_activitystatus.PhysicallyDischarged;
                                }
                            }
                            else
                            {
                                activityStatus = mzk_activitystatus.PendingTreatment;
                                encounterCreation = Activity.EncounterCreation.CreateEncounter;
                            }
                        }
                        else
                        {
                            activityStatus = mzk_activitystatus.Open;
                            encounterCreation = Activity.EncounterCreation.NoAction;
                        }
                    }
                    else
                    {
                        activityStatus = mzk_activitystatus.Open;
                        encounterCreation = Activity.EncounterCreation.NoAction;
                    }
                    break;
            }

            model.EncounterCreationStatus = encounterCreation;
            model.Status = (int)activityStatus;
            if (activityStatusList != null)
            {
                var selected = activityStatusList.Where(item => item.parmID == model.Status.ToString()).First();

                model.StatusText = selected.parmDescription;
                model.colorCode = selected.parmColorHex;
            }
        }

        async void updateCCStatus(Activity model, mzk_encountertype encounterType)
        {
            Activity.EncounterCreation encounterCreation = Activity.EncounterCreation.None;
            mzk_activitystatus activityStatus = mzk_activitystatus.None;
            bool encounterExist = false;
            bool encounterSigned = false;

            PatientEncounter enc = new PatientEncounter();

            enc = await enc.encounterDetails( 0, "", model.casePathwayActivityWorkflowId);

            if (enc != null)
            {
                if (enc.EncounterId != null && enc.EncounterId != string.Empty)
                {
                    encounterExist = true;

                    if (enc.EncounterStatusValue == (int)mzk_encounterstatus.Signed)
                    {
                        encounterSigned = true;
                    }
                }
            }

            if (encounterExist)
            {
                if (encounterSigned)
                {
                    encounterCreation = Activity.EncounterCreation.NoAction;

                    switch (encounterType)
                    {
                        case mzk_encountertype.Triage:
                            activityStatus = mzk_activitystatus.TriageComplete;
                            break;
                        case mzk_encountertype.Consultation:
                            activityStatus = mzk_activitystatus.ConsultationComplete;
                            break;
                        case mzk_encountertype.Treatment:
                            activityStatus = mzk_activitystatus.TreatmentComplete;
                            break;
                    }
                }
                else
                {
                    encounterCreation = Activity.EncounterCreation.ResumeEncounter;
                    model.encounterId = enc.EncounterId;

                    switch (encounterType)
                    {
                        case mzk_encountertype.Triage:
                            activityStatus = mzk_activitystatus.InTriage;
                            break;
                        case mzk_encountertype.Consultation:
                            activityStatus = mzk_activitystatus.InConsultation;
                            break;
                        case mzk_encountertype.Treatment:
                            activityStatus = mzk_activitystatus.InTreatment;
                            break;
                    }
                }
            }
            else
            {
                switch (encounterType)
                {
                    case mzk_encountertype.Triage:
                        activityStatus = mzk_activitystatus.PendingTriage;
                        encounterCreation = Activity.EncounterCreation.CreateEncounter;
                        break;
                    case mzk_encountertype.Consultation:
                        activityStatus = mzk_activitystatus.PendingConsultation;
                        encounterCreation = Activity.EncounterCreation.CreateEncounter;
                        break;
                    case mzk_encountertype.Treatment:
                        activityStatus = mzk_activitystatus.PendingTreatment;
                        encounterCreation = Activity.EncounterCreation.CreateEncounter;
                        break;
                }
            }

            model.EncounterCreationStatus = encounterCreation;
            model.Status = (int)activityStatus;

            if (activityStatusDropDownList != null && activityStatusDropDownList.DropDownValues != null)
            {
                var selected = activityStatusDropDownList.DropDownValues.Where(item => item.value == model.Status.ToString()).First();

                model.StatusText = selected.text;
                model.colorCode = selected.color;
            }
        }

        public void updatePatientAllergies(Activity model, List<PatientAllergy> userList, string patientId)
        {
            List<PatientAllergy> allergiesList = userList.Where(item => item.patient == patientId).ToList();

            foreach (PatientAllergy allergy in allergiesList)
            {
                if (allergy.allergen != null)
                {
                    if (model.ActiveAllergies == string.Empty || model.ActiveAllergies == null)
                    {
                        model.ActiveAllergies = allergy.allergen.name;
                    }
                    else
                    {
                        model.ActiveAllergies += ", " + allergy.allergen.name;
                    }
                }
            }
        }

        public void UpdateCaseCareTeamMembers(Activity model, List<User> _userList, string caseId)
        {
            List<User> userList = _userList.Where(item => item.caseId == caseId).ToList();
            model.appointment.careTeam = userList;
        }

        public void updatePatientProblems(Activity model, List<PatientProblem> patientProblemList, string patientId)
        {
            List<PatientProblem> problemsList = patientProblemList.Where(item => item.PatientId == patientId).ToList();

            foreach (PatientProblem problem in problemsList)
            {
                if (model.ActiveProblems == string.Empty || model.ActiveProblems == null)
                {
                    model.ActiveProblems = problem.problemName;
                }
                else
                {
                    model.ActiveProblems += ", " + problem.problemName;
                }
            }
        }

        public void updatePatientAppointment(Activity model, AXRepository.AXServices.HMAppointmentContract contract)
        {
            model.appointment = new Appointment();
            model.appointment.appointmentId = contract.parmApptRecId;
            model.appointment.startDateTime = contract.AppointmentDate + " " + contract.parmApptTime;
            model.appointment.appointmentNumber = contract.AppointmentNum;
            model.appointment.endDateTime = contract.parmApptEndDate + " " + contract.parmApptEndTime;
            model.appointment.statusValue = (int)contract.parmStatusValue;
            model.appointment.status = contract.parmApptStatus;
            model.appointment.caseId = contract.parmCaseId;

            User careTeamMember = new User();
            List<User> listCareTeam = new List<User>();

            careTeamMember.Name = contract.parmDoctorName;
            careTeamMember.PrimaryContact = contract.parmDoctorPrimaryContact;

            listCareTeam.Add(careTeamMember);

            model.appointment.careTeam = listCareTeam;
        }

        public void updatePatientDetails(Activity model, AXRepository.AXServices.HMAppointmentContract contract, string patientId)
        {
            model.patient = new Patient();

            model.patient.name = contract.parmPatientName;
            model.patient.age = contract.parmAge;
            model.patient.mrn = contract.parmPatientFileNumber;
            //   model.patient.dateOfBirth = contract.parmPatientDateOfBirth;
            //  model.patient.gender = contract.parmPatientGender;
            //  model.patient.registrationDate = contract.parmPatientRegistrationDate;                         
            model.patient.patientId = patientId;
            model.patient.email = contract.parmEmail;
            model.patient.phone = contract.parmPhone;
        }

    }
}
