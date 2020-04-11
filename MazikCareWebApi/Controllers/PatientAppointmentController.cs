using Helper;
using MazikCareService.Core.Models;
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

namespace MazikCareWebApi.Controllers
{
    public class PatientAppointmentController : ApiController
    {
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getVisitAppointments(ApiModel inputmodel)
        {
            List<PatientVisitAppointment> data = new List<PatientVisitAppointment>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {

                ApiResponseModel<List<PatientVisitAppointment>> model = new ApiResponseModel<List<PatientVisitAppointment>>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisitAppointment));
                data = await client.getVisitAppointments(inputmodel.patientId, inputmodel.visitAppointmentFilterBy);

                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    //model.data.records = data.Take<PatientVisitAppointment>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                    model.data.records = data;
                }

                return Response.Success<List<PatientVisitAppointment>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> createRescheduleRequest(ApiModel inputmodel)
        {
            bool data = false;
                     
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisitAppointment));
                data = await client.createRescheduleRequest(inputmodel.workOrderId, inputmodel.startDate, inputmodel.endDate);
                                
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
        public async Task<HttpResponseMessage> getVisitTrackingDetails(ApiModel inputmodel)
        {
            PatientVisitTracking data = null;

            try
            {
                ApiResponseModel<PatientVisitTracking> model = new ApiResponseModel<PatientVisitTracking>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisitTracking));
                data = await client.getVisitTrackingDetails(inputmodel.workOrderId);

                model.data.records = data;

                return Response.Success<PatientVisitTracking>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getRescheduleVisitDates(ApiModel inputmodel)
        {
            List<AppointmentTimeSlot> data = new List<AppointmentTimeSlot>();
                     
            try
            {
                ApiResponseModel<List<AppointmentTimeSlot>> model = new ApiResponseModel<List<AppointmentTimeSlot>>() { };

                var client = ServiceFactory.GetService(typeof(AppointmentTimeSlot));
                data = await client.getRescheduleVisitDates(inputmodel.userId, inputmodel.date,inputmodel.workOrderId,inputmodel.startDate,inputmodel.endDate);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<List<AppointmentTimeSlot>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getActiveVisits(ApiModel inputmodel)
        {
            List<PatientVisitProducts> data = new List<PatientVisitProducts>();

            try
            {
                ApiResponseModel<List<PatientVisitProducts>> model = new ApiResponseModel<List<PatientVisitProducts>>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisitAppointment));
                data = await client.getActiveVisits(inputmodel.patientId);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<List<PatientVisitProducts>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getActiveVisitProducts(ApiModel inputmodel)
        {
            List<PatientVisitProducts> data = new List<PatientVisitProducts>();

            try
            {
                ApiResponseModel<List<PatientVisitProducts>> model = new ApiResponseModel<List<PatientVisitProducts>>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisitProducts));
                data = await client.getActiveVisitProducts(inputmodel.patientId);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<List<PatientVisitProducts>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> cancelVisit(ApiModel inputmodel)
        {
            bool data = false;

            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisitAppointment));
                data = await client.cancelVisit(inputmodel.workOrderId,inputmodel.cancellationReason,inputmodel.userId);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getVisitDetails(ApiModel inputmodel)
        {
            PatientVisitAppointment data = new PatientVisitAppointment();

            try
            {
                ApiResponseModel<PatientVisitAppointment> model = new ApiResponseModel<PatientVisitAppointment>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisitAppointment));
                data = await client.getVisitDetails(inputmodel.workOrderId);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}
