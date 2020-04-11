using Helper;
using MazikCareService.Core.Models;
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
using System.Web;
using System.Web.Http;
namespace MazikCareWebApi.Controllers
{
    public class EnrollmentController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> GetEnrollmentSettings(ApiModel apiModel)
        {
            List<EnrollmentSettings> data = new List<EnrollmentSettings>();
            try
            {
                ApiResponseModel<List<EnrollmentSettings>> model = new ApiResponseModel<List<EnrollmentSettings>>() { };
                var client = ServiceFactory.GetService(typeof(Enrollment));
                data = await client.GetEnrollmentSettings(apiModel.practiceId);
                model.data.records = data;
                return Response.Success<List<EnrollmentSettings>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> PostSaveEnrollment(ApiModel apiModel)
        {
            bool result = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Enrollment));
                result = await client.PostSaveEnrollment(apiModel.practiceId, apiModel.enrollment,apiModel.userCreatedBy);
                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetEnrollments(ApiModel apiModel)
        {
            List<Enrollment> result = new List<Enrollment>();
            try
            {
                ApiResponseModel<List<Enrollment>> model = new ApiResponseModel<List<Enrollment>>() { };
                var client = ServiceFactory.GetService(typeof(Enrollment));
                result = await client.GetEnrollments(apiModel.practiceId);
                model.data.records = result;
                return Response.Success<List<Enrollment>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> UpdateEnrollmentByCentricityId(ApiModel apiModel)
        {
            bool result = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Enrollment));
                result = await client.UpdateEnrollmentByCentricityId(apiModel.enrollmentId, apiModel.centricityId,apiModel.userModifiedBy);
                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [MzkAuthorize]
        [HttpPost]
        public async Task<HttpResponseMessage> UpdateEnrollmentDeviceDate(ApiModel apiModel)
        {
            bool result = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Enrollment));
                result = await client.UpdateEnrollmentDeviceDate(apiModel.enrollmentId, apiModel.dateOfService,apiModel.userModifiedBy);
                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreateEnrollmentOrder(ApiModel apiModel)
        {
            bool result = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Enrollment));
                result = await client.CreateEnrollmentOrder(apiModel.enrollmentId);//.externalOrderId, apiModel.externalPatientId, apiModel.providerCode, apiModel.facilityCode, apiModel.serviceType, apiModel.message, apiModel.messageType);
                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        //[HttpPost]
        // [MzkAuthorize]
        //public async Task<HttpResponseMessage> PostPrintForm(ApiModel apiModel)
        //{

        //    bool result = false;
        //    try
        //    {
        //        ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
        //        var client = ServiceFactory.GetService(typeof(Enrollment));
        //        result = await client.PostPrintForm(apiModel.practiceId);
        //        return Response.Success<bool>(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Response.Exception(ex);
        //    }
        //}

    }
}
