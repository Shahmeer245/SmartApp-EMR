using Helper;
using MazikCareService.Core.Models;
using MazikCareService.Core.Services;
using MazikCareWebApi.ApiHelpers;
using MazikCareWebApi.Filters;
using MazikCareWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MazikLogger;
using MazikCareService.CRMRepository;
using System.Web;

namespace MazikCareWebApi.Controllers
{
    //[MzkAuthorize]
    public class PatientChartController : ApiController
    {

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientAllergies(ApiModel inputmodel)
        {
            List<PatientAllergy> data = new List<PatientAllergy>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientAllergy>> model = new ApiResponseModel<List<PatientAllergy>>() { };

                var client = ServiceFactory.GetService(typeof(PatientAllergy));
                data = await client.getPatientAllergies(inputmodel.patientId, inputmodel.SearchFilters, inputmodel.searchOrder, inputmodel.startDate, inputmodel.endDate, inputmodel.IsActive);


                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientAllergy>().CreatedOn);
                        model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientAllergy>().CreatedOn);
                    }

                    //if (!string.IsNullOrEmpty(inputmodel.patientId))
                    //{

                    //    model.data.noKnownAllergy = allergyStatus;
                    //}
                    //if (!string.IsNullOrEmpty(inputmodel.patientId))
                    //{
                    //    int countActive = new PatientAllergy().getActiveAllergyCount(inputmodel.patientId, mzk_patientallergiesmzk_Status.Active);
                    //    string allergyStatus = new Patient().getPatientDetail(inputmodel.patientId);

                    //    if (countActive > 0)
                    //        model.data.noKnownAllergy = "0";

                    //    if (countActive==0 && allergyStatus == "0")
                    //        model.data.noKnownAllergy = "1";

                    //    if (allergyStatus == "1")
                    //        model.data.noKnownAllergy = "2";

                    //}

                    if (CurrentPage > 0)
                    {
                        model.data.pagination.currentPage = CurrentPage.ToString();
                        model.data.pagination.totalCount = data.Count().ToString();
                        int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                        model.data.records = data.Take<PatientAllergy>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                    }
                    else
                    {
                        model.data.records = data;
                    }
                   
                }

                return Response.Success<List<PatientAllergy>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientHistory(ApiModel inputmodel)
        {
            List<PatientHistory> data = new List<PatientHistory>();

            try
            {
                ApiResponseModel<List<PatientHistory>> model = new ApiResponseModel<List<PatientHistory>>() { };

                var client = ServiceFactory.GetService(typeof(PatientHistory));
                data = await client.getPatientHistory(inputmodel.patientId, Convert.ToInt32(inputmodel.Type));

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<List<PatientHistory>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getChartTemplate(ApiModel inputmodel)
        {
            List<UITemplate> data = new List<UITemplate>();

            try
            {
                ApiResponseModel<List<UITemplate>> model = new ApiResponseModel<List<UITemplate>>() { };

                var client = ServiceFactory.GetService(typeof(UITemplate));
                data = await client.getChartTemplate(inputmodel.userId);

                if (data != null)
                {
                    model.data.records = data.ToList<UITemplate>();
                }

                return Response.Success<List<UITemplate>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientVitals(ApiModel inputmodel)
        {
            List<PatientVitals> data = new List<PatientVitals>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientVitals>> model = new ApiResponseModel<List<PatientVitals>>() { };

                var client = ServiceFactory.GetService(typeof(PatientVitals));
                data = await client.getPatientVitals(inputmodel.patientId);
                model.data.records = data;

                //if (data != null)
                //{
                //    model.data.pagination.currentPage = CurrentPage.ToString();
                //    model.data.pagination.totalCount = data.Count().ToString();
                //    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                //    model.data.records = data.Take<PatientVitals>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                    
                //}

                return Response.Success<List<PatientVitals>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getGraphValues(ApiModel inputmodel)
        {
            List<Graph> data = new List<Graph>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<Graph>> model = new ApiResponseModel<List<Graph>>() { };

                var client = ServiceFactory.GetService(typeof(PatientVitals));
                data = await client.getGraphValues(inputmodel.patientId,inputmodel.mmtCodeId,0);
                model.data.records = data;

                //if (data != null)
                //{
                //    model.data.pagination.currentPage = CurrentPage.ToString();
                //    model.data.pagination.totalCount = data.Count().ToString();
                //    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                //    model.data.records = data.Take<PatientVitals>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();

                //}

                return Response.Success<List<Graph>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientProblems(ApiModel inputmodel)
        {
            List<PatientProblem> data = new List<PatientProblem>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientProblem>> model = new ApiResponseModel<List<PatientProblem>>() { };

                var client = ServiceFactory.GetService(typeof(PatientProblem));
                data = await client.getPatientProblems(inputmodel.patientId,inputmodel.IsActive,inputmodel.SearchFilters,inputmodel.searchOrder,inputmodel.startDate,inputmodel.endDate, false, CurrentPage);


                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientProblem>().CreatedOn);
                        model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientProblem>().CreatedOn);
                    }
                                        
                    model.data.records = data;
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = Pagination.totalCount.ToString();

                }

                return Response.Success<List<PatientProblem>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> searchPatientDetails(ApiModel inputmodel)
        {
            List<Patient> data = new List<Patient>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<Patient>> model = new ApiResponseModel<List<Patient>>() { };
                var client = ServiceFactory.GetService(typeof(Patient));
                data = await client.searchPatientDetails(inputmodel.searchOrder, inputmodel.SearchFilters);


                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<Patient>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                    
                }

                return Response.Success<List<Patient>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> createAccount(Patient patient)
        {
            bool result = false;

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>();
                var client = ServiceFactory.GetService(typeof(Patient));
                result = await client.createAccount(patient);

                model.data.records = result.ToString();

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> updatePatientRIS(ApiModel inputmodel)
        {
            bool result = false;

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>();
                var client = ServiceFactory.GetService(typeof(Patient));
                result = await client.updatePatientRIS(inputmodel.MRN);

                model.data.records = result.ToString();

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> mergePatient(ApiModel inputmodel)
        {
            bool result = false;

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>();
                var client = ServiceFactory.GetService(typeof(Patient));
                result = await client.mergePatient(inputmodel.patientId, inputmodel.MRN);

                model.data.records = result.ToString();

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> createPatient(Patient patient)
        {
            long result = 0;
            bool res = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>();
                var client = ServiceFactory.GetService(typeof(Patient));
                result = await client.createPatient(patient);

                //if (result > 0)
                // {
                // client = ServiceFactory.GetService(typeof(Patient));
                //  res = await client.updatePatientRecId(patient, result);
                //}
                //else
                //{
                //    throw new ValidationException("Patient not created on AX");
                //}

                if (result != null)
                {
                    model.data.records = result.ToString();
                }
                //if (res)
                //{
                // model.data.records = result.ToString();
                //}
                //else
                //{
                //    throw new ValidationException("Patient not updated in CRM");
                //}

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updateAccount(Patient patient)
        {
            bool result = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>();
                var client = ServiceFactory.GetService(typeof(Patient));
                result = await client.updateAccount(patient);

                model.data.records = result.ToString();

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        /********************Patietn Patient***********************/

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientDetails(ApiModel inputmodel)
        {
            Patient result;
            try
            {
                ApiResponseModel<Patient> model = new ApiResponseModel<Patient>();
                var client = ServiceFactory.GetService(typeof(Patient));
                result = await client.getPatientDetails(inputmodel.patientId, inputmodel.getDocuments, inputmodel.getAddresses, inputmodel.getRelationship);

                model.data.records = result;

                return Response.Success<Patient>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreatePatientCRM(ApiModel inputmodel)
        {
            Guid result;
            try
            {
                Patient patient = new Patient();                
                ApiResponseModel<Patient> model = new ApiResponseModel<Patient>();
                var client = ServiceFactory.GetService(typeof(Patient));
                result = await client.CreatePatientCRM(inputmodel.patient);
                patient.patientId = result.ToString();

                model.data.records = patient;
                model.data.records.patientId = result.ToString();

                return Response.Success<Patient>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        /********************Patietn Patient***********************/

        /********************Patietn Device***********************/

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientDevice(ApiModel inputmodel)
        {
            List<PatientDevice> result;
            try
            {
                ApiResponseModel<List<PatientDevice>> model = new ApiResponseModel<List<PatientDevice>>();
                var client = ServiceFactory.GetService(typeof(PatientDevice));
                result = await client.getPatientDeviceCRM(inputmodel.patientId, inputmodel.startDate, inputmodel.endDate);

                model.data.records = result;

                return Response.Success<List<PatientDevice>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreatePatientDeviceCRM(ApiModel inputmodel)
        {
            Guid result;
            try
            {
                PatientDevice condition = new PatientDevice();
                ApiResponseModel<PatientDevice> model = new ApiResponseModel<PatientDevice>();
                var client = ServiceFactory.GetService(typeof(PatientDevice));
                result = await client.CreatePatientDeviceCRM(inputmodel.patientDevice);
                model.data.records = condition;

                //model.data.records = result.ToString();

                return Response.Success<PatientDevice>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        /********************Patietn Device***********************/

        /********************Patietn Encounter***********************/

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientEncounter(ApiModel inputmodel)
        {
            List<PatientEncounter> result;
            try
            {
                ApiResponseModel<List<PatientEncounter>> model = new ApiResponseModel<List<PatientEncounter>>();
                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                result = await client.getPatientEncounterCRM(inputmodel.patientId, inputmodel.startDate, inputmodel.endDate);

                model.data.records = result;

                return Response.Success<List<PatientEncounter>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreatePatientEncounterCRM(ApiModel inputmodel)
        {
            Guid result;
            try
            {
                PatientEncounter condition = new PatientEncounter();
                ApiResponseModel<PatientEncounter> model = new ApiResponseModel<PatientEncounter>();
                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                result = await client.CreatePatientEncounterCRM(inputmodel.patientEncounter);
                model.data.records = condition;

                //model.data.records = result.ToString();

                return Response.Success<PatientEncounter>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        /********************Patietn Encounter***********************/

        /********************Patietn Procedure***********************/

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientProcedure(ApiModel inputmodel)
        {
            List<PatientProcedure> result;
            try
            {
                ApiResponseModel<List<PatientProcedure>> model = new ApiResponseModel<List<PatientProcedure>>();
                var client = ServiceFactory.GetService(typeof(PatientProcedure));
                result = await client.getPatientProcedureCRM(inputmodel.patientId, inputmodel.startDate, inputmodel.endDate);

                model.data.records = result;

                return Response.Success<List<PatientProcedure>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreatePatientProcedureCRM(ApiModel inputmodel)
        {
            Guid result;
            try
            {
                PatientProcedure condition = new PatientProcedure();
                ApiResponseModel<PatientProcedure> model = new ApiResponseModel<PatientProcedure>();
                var client = ServiceFactory.GetService(typeof(PatientProcedure));
                result = await client.CreatePatientProcedureCRM(inputmodel.patientProcedure);
                model.data.records = condition;

                //model.data.records = result.ToString();

                return Response.Success<PatientProcedure>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        /********************Patietn Procedure***********************/

        /********************Patietn Procedure Request***********************/

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientProcedureRequest(ApiModel inputmodel)
        {
            List<PatientProcedureRequest> result;
            try
            {
                ApiResponseModel<List<PatientProcedureRequest>> model = new ApiResponseModel<List<PatientProcedureRequest>>();
                var client = ServiceFactory.GetService(typeof(PatientProcedureRequest));
                result = await client.getPatientProcedureRequestCRM(inputmodel.patientId, inputmodel.startDate, inputmodel.endDate);

                model.data.records = result;

                return Response.Success<List<PatientProcedureRequest>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreatePatientProcedureRequestCRM(ApiModel inputmodel)
        {
            Guid result;
            try
            {
                PatientProcedureRequest condition = new PatientProcedureRequest();
                ApiResponseModel<PatientProcedureRequest> model = new ApiResponseModel<PatientProcedureRequest>();
                var client = ServiceFactory.GetService(typeof(PatientProcedureRequest));
                result = await client.CreatePatientProcedureRequestCRM(inputmodel.patientProcedureRequest);
                model.data.records = condition;

                //model.data.records = result.ToString();

                return Response.Success<PatientProcedureRequest>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        /********************Patietn Procedure Request***********************/

        /********************Patietn Condition***********************/
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientCondition(ApiModel inputmodel)
        {
            List<PatientCondition> result;
            try
            {
                ApiResponseModel<List<PatientCondition>> model = new ApiResponseModel<List<PatientCondition>>();
                var client = ServiceFactory.GetService(typeof(PatientCondition));
                result = await client.getPatientConditionCRM(inputmodel.patientId, inputmodel.startDate, inputmodel.endDate);

                model.data.records = result;

                return Response.Success<List<PatientCondition>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreatePatientConditionCRM(ApiModel inputmodel)
        {
            Guid result;
            try
            {
                PatientCondition condition = new PatientCondition();
                ApiResponseModel<PatientCondition> model = new ApiResponseModel<PatientCondition>();
                var client = ServiceFactory.GetService(typeof(PatientCondition));
                result = await client.CreatePatientConditionCRM(inputmodel.patientCondition);
                model.data.records = condition;

                //model.data.records = result.ToString();

                return Response.Success<PatientCondition>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        /********************Patietn Condition***********************/

        /********************Patietn Appointment***********************/
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientAppointment(ApiModel inputmodel)
        {
            List<PatientAppointment> result;
            try
            {
                ApiResponseModel<List<PatientAppointment>> model = new ApiResponseModel<List<PatientAppointment>>();
                var client = ServiceFactory.GetService(typeof(PatientAppointment));
                result = await client.getPatientConditionCRM(inputmodel.patientId, inputmodel.startDate, inputmodel.endDate);

                model.data.records = result;

                return Response.Success<List<PatientAppointment>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreatePatientAppointmentCRM(ApiModel inputmodel)
        {
            Guid result;
            try
            {
                PatientAppointment condition = new PatientAppointment();
                ApiResponseModel<PatientAppointment> model = new ApiResponseModel<PatientAppointment>();
                var client = ServiceFactory.GetService(typeof(PatientAppointment));
                result = await client.CreatePatientAppointmentCRM(inputmodel.patientAppointment);
                model.data.records = condition;

                //model.data.records = result.ToString();

                return Response.Success<PatientAppointment>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        /********************Patietn Appointment***********************/

        /********************Patietn CarePlan***********************/

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientCarePlans(ApiModel inputmodel)
        {
            List<PatientCarePlan> result;
            try
            {
                ApiResponseModel<List<PatientCarePlan>> model = new ApiResponseModel<List<PatientCarePlan>>();
                var client = ServiceFactory.GetService(typeof(PatientCarePlan));
                result = await client.getPatientCareplans(inputmodel.patientId, inputmodel.startDate, inputmodel.endDate);

                model.data.records = result;

                return Response.Success<List<PatientCarePlan>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreatePatientCarePlanCRM(ApiModel inputmodel)
        {
            Guid result;
            try
            {
                PatientCarePlan careplan = new PatientCarePlan();
                ApiResponseModel<PatientCarePlan> model = new ApiResponseModel<PatientCarePlan>();
                var client = ServiceFactory.GetService(typeof(PatientCarePlan));
                result = await client.CreatePatientCarePlanCRM(inputmodel.patientCarePlan);
                model.data.records = careplan;

                //model.data.records = result.ToString();

                return Response.Success<PatientCarePlan>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        /********************Patietn CarePlan***********************/

        /********************Patietn Allergy***********************/

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientAllergiesCRM(ApiModel inputmodel)
        {
            List<PatientAllergy> result;
            try
            {
                ApiResponseModel<List<PatientAllergy>> model = new ApiResponseModel<List<PatientAllergy>>();
                var client = ServiceFactory.GetService(typeof(PatientAllergy));
                result = await client.getPatientAllergiesCRM(inputmodel.patientId, inputmodel.startDate, inputmodel.endDate);

                model.data.records = result;

                return Response.Success<List<PatientAllergy>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreatePatientAllergyCRM(ApiModel inputmodel)
        {
            Guid result;
            try
            {
                PatientAllergy careplan = new PatientAllergy();
                ApiResponseModel<PatientAllergy> model = new ApiResponseModel<PatientAllergy>();
                var client = ServiceFactory.GetService(typeof(PatientAllergy));
                result = await client.CreatePatientAllergyCRM(inputmodel.patientAllergy);
                model.data.records = careplan;

                //model.data.records = result.ToString();

                return Response.Success<PatientAllergy>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        /********************Patietn Allergy***********************/

        /********************Patietn Observation***********************/
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientObservationCRM(ApiModel inputmodel)
        {
            List<PatientObservation> result;
            try
            {
                ApiResponseModel<List<PatientObservation>> model = new ApiResponseModel<List<PatientObservation>>();
                var client = ServiceFactory.GetService(typeof(PatientObservation));
                result = await client.getPatientObservationCRM(inputmodel.patientId, inputmodel.startDate, inputmodel.endDate);

                model.data.records = result;

                return Response.Success<List<PatientObservation>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreatePatientObservationCRM(ApiModel inputmodel)
        {
            Guid result;
            try
            {
                PatientObservation careplan = new PatientObservation();
                ApiResponseModel<PatientObservation> model = new ApiResponseModel<PatientObservation>();
                var client = ServiceFactory.GetService(typeof(PatientObservation));
                result = await client.CreatePatientObservationCRM(inputmodel.patientObservation);
                model.data.records = careplan;

                //model.data.records = result.ToString();

                return Response.Success<PatientObservation>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        
        /********************Patietn Observation***********************/


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatients(ApiModel inputmodel)
        {
            List<Patient> result = new List<Patient>();
            try
            {
                ApiResponseModel<List<Patient>> model = new ApiResponseModel<List<Patient>>();
                var client = ServiceFactory.GetService(typeof(Patient));
                result = await client.getPatients(inputmodel.patientId);

                model.data.records = result.ToList<Patient>();

                return Response.Success<List<Patient>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]       
        public async Task<HttpResponseMessage> getPatientVisitDetails(ApiModel inputmodel)
        {
            Activity result;
            try
            {
                SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
                SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
                SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

                ApiResponseModel<Activity> model = new ApiResponseModel<Activity>();
                var client = ServiceFactory.GetService(typeof(Patient));
                result = await client.getPatientVisitDetails(inputmodel.AxAppoitnmentRefRecId, inputmodel.CaseId, inputmodel.patientRecId);

                model.data.records = result;

                return Response.Success<Activity>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientBasicInfo(ApiModel inputmodel)
        {
            Activity result;
            try
            {
                ApiResponseModel<Activity> model = new ApiResponseModel<Activity>();
                var client = ServiceFactory.GetService(typeof(Patient));
                result = await client.getPatientBasicInfo(inputmodel.patientId, inputmodel.appointmentId,inputmodel.CaseId);

                model.data.records = result;

                return Response.Success<Activity>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> addPatientAllergy(PatientAllergy patientAllergy)
        {
            string data = string.Empty;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientAllergy));
                data = await client.addPatientAllergy(patientAllergy);
                model.data.records = data;

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updatePatientAllergy(PatientAllergy inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client          = ServiceFactory.GetService(typeof(PatientAllergy));
                data                = await client.updatePatientAllergy(inputmodel);
                model.data.records  = data;


                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getConceptData(Concept inputmodel)
        {
            List<Concept> data = new List<Concept>();
            try
            {
                ApiResponseModel<List<Concept>> model = new ApiResponseModel<List<Concept>>() { };
                var client = ServiceFactory.GetService(typeof(Concept));
                data = await client.getConceptData(inputmodel.ConceptType, inputmodel.name);
                model.data.records = data;
                return Response.Success<List<Concept>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> addPatientProblem(PatientProblem patientProblem)
        {
            string data = string.Empty;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientProblem));
                data = await client.addPatientProblem(patientProblem);
                model.data.records = data;

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updatePatientProblem(PatientProblem patientProblem)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>(){ };

                var client = ServiceFactory.GetService(typeof(PatientProblem));
                data = await client.updatePatientProblem(patientProblem);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updatePatientDetails(Patient patient)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(Patient));
                data = await client.updatePatientDetails(patient);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> checkPatientAppAccess(ApiModel inputmodel)
        {
            //string data = string.Empty;
            ReturnStatus data = new ReturnStatus();
            try
            {
                ApiResponseModel<ReturnStatus> model = new ApiResponseModel<ReturnStatus>() { };

                var client = ServiceFactory.GetService(typeof(Patient));
                data = await client.checkPatientAppAccess(HttpContext.Current.Request.Headers["Authorization"].ToString());
                model.data.records = data;

                return Response.Success<ReturnStatus>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPrescriptions(ApiModel inputmodel)
        {
            //string data = string.Empty;
            List<PatientPrescription> data = new List<PatientPrescription>();
            try
            {
                ApiResponseModel<List<PatientPrescription>> model = new ApiResponseModel<List<PatientPrescription>>() { };

                var client = ServiceFactory.GetService(typeof(PatientPrescription));
                data = await client.getPrescriptions(inputmodel.patientId);
                model.data.records = data;

                return Response.Success<List<PatientPrescription>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }



        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> addAddress(Address address)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(Address));
                data = await client.addAddress(address);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updateAddress(Address address)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(Address));
                data = await client.updateAddress(address);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> viewReferrals(ApiModel inputmodel)
        {
            
            List<PatientReferral> data = new List<PatientReferral>();
            try
            {
                ApiResponseModel<List<PatientReferral>> model = new ApiResponseModel<List<PatientReferral>>() { };

                var client = ServiceFactory.GetService(typeof(PatientReferral));
                data = await client.viewReferrals(inputmodel.patientId);
                model.data.records = data;
                return Response.Success<List<PatientReferral>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> viewVitals(ApiModel inputmodel)
        {

            List<PatientVitals> data = new List<PatientVitals>();
            try
            {
                ApiResponseModel<List<PatientVitals>> model = new ApiResponseModel<List<PatientVitals>>() { };

                var client = ServiceFactory.GetService(typeof(PatientVitals));
                data = await client.viewVitals(inputmodel.patientId);
                model.data.records = data;
                return Response.Success<List<PatientVitals>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> viewVitalsHistory(ApiModel inputmodel)
        {

            PatientVitals data = new PatientVitals();
            try
            {
                ApiResponseModel<PatientVitals> model = new ApiResponseModel<PatientVitals>() { };

                var client = ServiceFactory.GetService(typeof(PatientVitals));
                data = await client.viewVitalsHistory(inputmodel.patientId,inputmodel.mmtCodeId);
                model.data.records = data;
                return Response.Success<PatientVitals>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> UpdateProblems(ApiModel patientAllergy)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(PatientProblem));
                data = await client.updatePatientProblems(patientAllergy.problemList);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> createPatientInsurance(PatientInsurance inputmodel)
        {
            long result = 0;

            try
            {
                ApiResponseModel<long> model = new ApiResponseModel<long>();
                var client = ServiceFactory.GetService(typeof(PatientInsurance));
                result = await client.createPatientInsurance(inputmodel.contactAxRefrecid, inputmodel.insuranceCarrierAxRefrecid);

                model.data.records = result;

                return Response.Success<long>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getCaseActivities(ApiModel inputmodel)
        {
            //string data = string.Empty;
            List<CaseActivities> data = new List<CaseActivities>();
            try
            {
                ApiResponseModel<List<CaseActivities>> model = new ApiResponseModel<List<CaseActivities>>() { };

                var client = ServiceFactory.GetService(typeof(CaseActivities));
                data = await client.getCaseActivities(inputmodel.referralId);
                model.data.records = data;

                return Response.Success<List<CaseActivities>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updateWorkOrderProduct(PatientVisitProducts products)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisitProducts));
                data = await client.updateWorkOrderProduct(products);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> addWorkOrderProducts(ApiModel inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisitProducts));
                data = await client.addWorkOrderProducts(inputmodel.workOrderId,inputmodel.productsList);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> addWorkOrderProductValidation(ApiModel inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisitProducts));
                data = await client.addWorkOrderProductValidation(inputmodel.workOrderId);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> addRelationship(Relationship relObj)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(Relationship));
                data = await client.addRelationship(relObj);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updateRelationship(Relationship inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(Relationship));
                data = await client.updateRelationship(inputmodel.relationshipId,inputmodel.nextOfKin,inputmodel.carer);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getCarerRelations(ApiModel inputmodel)
        {
            List<Relationship> data = new List<Relationship>();
            try
            {
                ApiResponseModel<List<Relationship>> model = new ApiResponseModel<List<Relationship>>() { };

                var client = ServiceFactory.GetService(typeof(Relationship));
                data = await client.getCarerRelations(inputmodel.carerId);
                model.data.records = data;

                return Response.Success<List<Relationship>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> useHeader(ApiModel inputmodel)
        {
            AppHeader data = new AppHeader();
            try
            {
                ApiResponseModel<AppHeader> model = new ApiResponseModel<AppHeader>() { };

                var client = ServiceFactory.GetService(typeof(Relationship));
                data = await client.useHeader(inputmodel.patientId);
                model.data.records = data;

                return Response.Success<AppHeader>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}
