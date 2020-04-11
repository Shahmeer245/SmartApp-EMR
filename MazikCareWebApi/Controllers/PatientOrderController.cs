using Helper;
using MazikCareService.Core.Models;
using MazikCareService.Core.Services;
using MazikCareService.CRMRepository;
using MazikCareWebApi.ApiHelpers;
using MazikCareWebApi.Filters;
using MazikCareWebApi.Models;
using MazikLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MazikCareWebApi.Controllers
{
    
    public class PatientOrderController : ApiController
    {
        #region Referral
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updatePatientReferral(PatientReferralOrder patientReferral)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientReferralOrder));
                data = await client.updatePatientOrder(patientReferral);
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
        public async Task<HttpResponseMessage> getPatientReferral(ApiModel inputmodel)
        {
            List<PatientReferralOrder> data = new List<PatientReferralOrder>();

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientReferralOrder>> model = new ApiResponseModel<List<PatientReferralOrder>>() { };
                var client = ServiceFactory.GetService(typeof(PatientReferralOrder));
                data = await client.getPatientOrder(inputmodel.patientId, inputmodel.patientEncounterId, inputmodel.SearchFilters, inputmodel.searchOrder, inputmodel.startDate, inputmodel.endDate, inputmodel.forFulfillment, inputmodel.orderId,inputmodel.CaseId, CurrentPage, inputmodel.appointmentId);
                
                if (data != null)
                {
                    //if (!string.IsNullOrEmpty(inputmodel.patientEncounterId))
                    //    model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);

                    if (data.Count > 0)
                    {
                        model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientReferralOrder>().CreatedOn);
                        model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientReferralOrder>().CreatedOn);
                    }
                    model.data.records = data;
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = Pagination.totalCount.ToString();
                }

                return Response.Success<List<PatientReferralOrder>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddReferral(PatientReferralOrder inputmodel)
        {
            string data = string.Empty;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientReferralOrder));
                data = await client.addPatientOrder(inputmodel);
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
        public async Task<HttpResponseMessage> getReferral(PatientReferralOrder inputmodel)
        {
            List<PatientReferralOrder> data = new List<PatientReferralOrder>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");
            
            try
            {
                ApiResponseModel<List<PatientReferralOrder>> model = new ApiResponseModel<List<PatientReferralOrder>>() { };
                var client = ServiceFactory.GetService(typeof(PatientReferralOrder));
                data = await client.getReferral(inputmodel);

                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    model.data.records = data;
                }
                return Response.Success<List<PatientReferralOrder>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        #endregion
        #region Medication
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientMedications(ApiModel inputmodel)
        {
            List<PatientMedication> data = new List<PatientMedication>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientMedication>> model = new ApiResponseModel<List<PatientMedication>>() { };

                var client = ServiceFactory.GetService(typeof(PatientMedication));
                data = await client.getPatientOrder(inputmodel.patientId, inputmodel.patientEncounterId, inputmodel.SearchFilters, inputmodel.searchOrder, inputmodel.startDate, inputmodel.endDate, inputmodel.forFulfillment, inputmodel.orderId, inputmodel.CaseId, CurrentPage);


                if (data != null)
                {
                    //if (!string.IsNullOrEmpty(inputmodel.patientEncounterId))
                    //    model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);
                    if (data.Count > 0)
                    {
                        model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientMedication>().CreatedOn);
                        model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientMedication>().CreatedOn);
                    }
                    model.data.records = data;
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = Pagination.totalCount.ToString();
                }

                return Response.Success<List<PatientMedication>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientMedicationOrderHistory(ApiModel inputmodel)
        {
            List<PatientMedication> data = new List<PatientMedication>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientMedication>> model = new ApiResponseModel<List<PatientMedication>>() { };

                var client = ServiceFactory.GetService(typeof(PatientMedication));
                data = await client.getPatientOrderHistory(inputmodel.patientId, inputmodel.IsActive);
                
                if (data != null)
                {                    
                    model.data.records = data;
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = Pagination.totalCount.ToString();
                }

                return Response.Success<List<PatientMedication>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddMedicationOrder(PatientMedication inputmodel)
        {
            string data = string.Empty;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientMedication));

                data = await client.addPatientOrder(inputmodel);

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
        public async Task<HttpResponseMessage> updateMedicationOrder(PatientMedication patientMedication)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientMedication));
                data = await client.updatePatientOrder(patientMedication);
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
        public async Task<HttpResponseMessage> deleteMedicationDose(PatientMedication patientMedication)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientMedication));
                data = await client.deleteMedicationDose(patientMedication.Id);
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
        public async Task<HttpResponseMessage> createPatientOrderLog(ApiModel inputmodel)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientMedication));
                data = await client.createPatientOrderLog(inputmodel.patientOrderId,inputmodel.doseId,inputmodel.date,inputmodel.skipReasonId);
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
        public async Task<HttpResponseMessage> getPatientOrderLog(ApiModel inputmodel)
        {
            //string data = string.Empty;
            List<PatientOrderAdministration> data = new List<PatientOrderAdministration>();
            try
            {
                ApiResponseModel<List<PatientOrderAdministration>> model = new ApiResponseModel<List<PatientOrderAdministration>>() { };

                var client = ServiceFactory.GetService(typeof(PatientOrderAdministration));
                data = await client.getPatientOrderLog(inputmodel.patientOrderId);
                model.data.records = data;

                return Response.Success<List<PatientOrderAdministration>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        #endregion
        #region Lab
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientLabOrder(ApiModel inputmodel)
        {
            List<PatientLabOrder> data = new List<PatientLabOrder>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientLabOrder>> model = new ApiResponseModel<List<PatientLabOrder>>() { };

                var client = ServiceFactory.GetService(typeof(PatientLabOrder));
                data = await client.getPatientOrder(inputmodel.patientId, inputmodel.patientEncounterId, inputmodel.SearchFilters, inputmodel.searchOrder, inputmodel.startDate, inputmodel.endDate, inputmodel.forFulfillment, inputmodel.orderId, inputmodel.CaseId, CurrentPage);


                if (data != null)
                {
                    //if (!string.IsNullOrEmpty(inputmodel.patientEncounterId))
                    //    model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);
                    if (data.Count > 0)
                    {
                        model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientLabOrder>().CreatedOn);
                        model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientLabOrder>().CreatedOn);
                    }
                    model.data.records = data;
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = Pagination.totalCount.ToString();
                }

                return Response.Success<List<PatientLabOrder>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddLabOrder(PatientLabOrder inputmodel)
        {
            string data = string.Empty;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientLabOrder));
                data = await client.addPatientOrder(inputmodel);
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
        public async Task<HttpResponseMessage> AddOrderList(List<PatientOrder> inputmodel)
        {
            string data = string.Empty;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientOrder));
                data = await client.addPatientOrderByList(inputmodel);
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
        public async Task<HttpResponseMessage> updatePatientLabOrder(PatientLabOrder patientLabOrder)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientLabOrder));
                data = await client.updatePatientOrder(patientLabOrder);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        #endregion
        #region Radiology
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientRadiologyOrder(ApiModel inputmodel)
        {
            List<PatientRadiologyOrder> data = new List<PatientRadiologyOrder>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientRadiologyOrder>> model = new ApiResponseModel<List<PatientRadiologyOrder>>() { };

                var client = ServiceFactory.GetService(typeof(PatientRadiologyOrder));
                data = await client.getPatientOrder(inputmodel.patientId, inputmodel.patientEncounterId, inputmodel.SearchFilters, inputmodel.searchOrder, inputmodel.startDate, inputmodel.endDate, inputmodel.forFulfillment, inputmodel.orderId, inputmodel.CaseId, false, CurrentPage);


                if (data != null)
                {
                    //if (!string.IsNullOrEmpty(inputmodel.patientEncounterId))
                    //    model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);

                    if (data.Count > 0)
                    {
                        model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientRadiologyOrder>().CreatedOn);
                        model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientRadiologyOrder>().CreatedOn);
                    }
                    model.data.records = data;
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = Pagination.totalCount.ToString();
                }

                return Response.Success<List<PatientRadiologyOrder>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddRadiologyOrder(PatientRadiologyOrder inputmodel)
        {
            string data = string.Empty;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientRadiologyOrder));
                data = await client.addPatientOrder(inputmodel);
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
        public async Task<HttpResponseMessage> updatePatientRadiologyOrder(PatientRadiologyOrder patientRadiologyOrder)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientRadiologyOrder));
                data = await client.updatePatientOrder(patientRadiologyOrder);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        #endregion
        #region Special Test
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientSpecialTest(ApiModel inputmodel)
        {
            List<PatientSpecialTest> data = new List<PatientSpecialTest>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientSpecialTest>> model = new ApiResponseModel<List<PatientSpecialTest>>() { };
                var client = ServiceFactory.GetService(typeof(PatientSpecialTest));
                data = await client.getPatientOrder(inputmodel.patientId, inputmodel.patientEncounterId, inputmodel.SearchFilters, inputmodel.searchOrder, inputmodel.startDate, inputmodel.endDate, inputmodel.forFulfillment, inputmodel.orderId, inputmodel.CaseId, CurrentPage);

                if (data != null)
                {
                    //if (!string.IsNullOrEmpty(inputmodel.patientEncounterId))
                    //    model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);
                    if (data.Count > 0)
                    {
                        model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientSpecialTest>().CreatedOn);
                        model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientSpecialTest>().CreatedOn);
                    }
                    model.data.records = data;
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = Pagination.totalCount.ToString();
                }
                return Response.Success<List<PatientSpecialTest>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddSpecialTest(PatientSpecialTest inputmodel)
        {
            string data = string.Empty;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientSpecialTest));
                data = await client.addPatientOrder(inputmodel);
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
        public async Task<HttpResponseMessage> updatePatientSpecialTest(PatientSpecialTest patientSpecialTest)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientSpecialTest));
                data = await client.updatePatientOrder(patientSpecialTest);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        #endregion
        #region Procedure
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPateintProcedure(ApiModel inputmodel)
        {
            List<PatientProcedure> data = new List<PatientProcedure>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
            return Response.Error("App", "current page is not valid");

                try
                {
                ApiResponseModel<List<PatientProcedure>> model = new ApiResponseModel<List<PatientProcedure>>() { };
                var client = ServiceFactory.GetService(typeof(PatientProcedure));
                data = await client.getPatientOrder(inputmodel.patientId, inputmodel.patientEncounterId, inputmodel.SearchFilters, inputmodel.searchOrder, inputmodel.startDate, inputmodel.endDate, inputmodel.forFulfillment, inputmodel.orderId, inputmodel.CaseId, true, CurrentPage);
                    if (data != null)
                    {
                        //if (!string.IsNullOrEmpty(inputmodel.patientEncounterId))
                        //model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);
                        if (data.Count > 0)
                        {
                            model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientProcedure>().CreatedOn);
                            model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientProcedure>().CreatedOn);
                        }

                    model.data.records = data;
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = Pagination.totalCount.ToString();
                }
                return Response.Success<List<PatientProcedure>>(model);
                }
                catch (Exception ex)
                {
                return Response.Exception(ex);
                }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddPateintProcedure(PatientProcedure inputmodel)
        {
            string data = string.Empty;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientProcedure));

                data = await client.addPatientOrder(inputmodel);

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
        public async Task<HttpResponseMessage> updatePateintProcedure(PatientProcedure patientProcedure)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientProcedure));
                data = await client.updatePatientOrder(patientProcedure);
                model.data.records = data;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        #endregion
        #region Therapies
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientTherapies(ApiModel inputmodel)
        {
            List<PatientTherapy> data = new List<PatientTherapy>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientTherapy>> model = new ApiResponseModel<List<PatientTherapy>>() { };

                var client = ServiceFactory.GetService(typeof(PatientTherapy));
                data = await client.getPatientOrder(inputmodel.patientId, inputmodel.patientEncounterId, inputmodel.SearchFilters, inputmodel.searchOrder, inputmodel.startDate, inputmodel.endDate, inputmodel.forFulfillment, inputmodel.orderId, inputmodel.CaseId);


                if (data != null)
                {
                    //if (!string.IsNullOrEmpty(inputmodel.patientEncounterId))
                    //    model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);

                    if (data.Count > 0)
                    {
                        model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientTherapy>().CreatedOn);
                        model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientTherapy>().CreatedOn);
                    }

                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    model.data.records = data.Take<PatientTherapy>(Convert.ToInt32(CurrentPage * 10)).Skip((CurrentPage - 1) * Convert.ToInt32(AppSettings.GetByKey("PageSize"))).ToList();
                }

                return Response.Success<List<PatientTherapy>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddPatientTherapies(PatientTherapy inputmodel)
        {
            string data = string.Empty;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientTherapy));
                data = await client.addPatientOrder(inputmodel);
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
        public async Task<HttpResponseMessage> updatePatientTherapy(PatientTherapy patientTherapy)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(PatientTherapy));
                data = await client.updatePatientOrder(patientTherapy);
                model.data.records = data;
                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        #endregion
        #region Order
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> executeStatusManagerAction(ApiModel apiModel)
        {
            bool data = false;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(StatusManager));
                data = client.executeAction(apiModel.statusManagerDetailsId, apiModel.orderId, apiModel.patientEncounterId, apiModel.ParamsValues);
                model.data.records = data.ToString();

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> cancelOrder(ApiModel inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientOrder));

                data = client.cancelOrder(inputmodel.orderId, inputmodel.patientEncounterId, inputmodel.ParamsValues);

                model.data.records = data.ToString();
                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> discontinueOrder(ApiModel inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientOrder));

                data = client.discontinueOrder(inputmodel.orderId, inputmodel.patientEncounterId, inputmodel.ParamsValues);

                model.data.records = data.ToString();
                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> completeOrder(ApiModel inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientOrder));

                data = client.completeOrder(inputmodel.orderId, inputmodel.patientEncounterId, inputmodel.ParamsValues);

                model.data.records = data.ToString();
                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> cancelAppointmentOrder(ApiModel inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientOrder));

                data = client.cancelAppointmentOrder(inputmodel.orderId, inputmodel.CaseId, inputmodel.ParamsValues);

                model.data.records = data.ToString();
                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> paidOrder(ApiModel inputmodel)
        {
            bool data = false;

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientOrder));

                data = client.paidOrder(inputmodel.orderId, inputmodel.CaseId, inputmodel.ParamsValues);

                model.data.records = data.ToString();
                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]

        public async Task<HttpResponseMessage> pendingOrder(ApiModel inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientOrder));

                data = client.pendingOrder(inputmodel.orderId, inputmodel.CaseId, inputmodel.ParamsValues);

                model.data.records = data.ToString();
                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> refundOrder(ApiModel inputmodel)
        {
            bool data = false;

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientOrder));

                data = client.refundOrder(inputmodel.orderId, inputmodel.CaseId, inputmodel.ParamsValues);

                model.data.records = data.ToString();
                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> sendPanicResult(ApiModel inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientOrder));

                data = client.sendPanicResult(inputmodel.orderId, inputmodel.message);

                model.data.records = data.ToString();
                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> updateAppointmentDetails(ApiModel inputmodel)
        {
            bool data = false;

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientOrder));

                data = await client.updateAppointmentDetails(inputmodel.orderId, inputmodel.date, inputmodel.appointmentId, inputmodel.patientRecId, inputmodel.registered);

                model.data.records = data.ToString();
                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        #endregion
    }
}
