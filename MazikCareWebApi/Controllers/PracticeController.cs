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
    public class PracticeController : ApiController
    {
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetPhysician(ApiModel inputmodel)
        { 
            Physician data = new Physician();
            try
            {
                ApiResponseModel<Physician> model = new ApiResponseModel<Physician>() { };
                var client = ServiceFactory.GetService(typeof(Physician));
                data = await client.GetPhysician(inputmodel.physicianId);
                model.data.records = data;
                return Response.Success<Physician>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetInsurance(ApiModel inputmodel)
        {
            //ask whether this will be a PatientInsurance
            List<Insurance> data = new List<Insurance>();
            try
            {
                ApiResponseModel<List<Insurance>> model = new ApiResponseModel<List<Insurance>>() { };
                var client = ServiceFactory.GetService(typeof(Insurance));
                data = await client.getInsurance(inputmodel.insuranceName, inputmodel.insuranceId);
                model.data.records = data;
                return Response.Success<List<Insurance>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetDiagnosticCodes(ApiModel inputmodel)
        {
            List<DiagnosticCode> data = new List<DiagnosticCode>();
            try
            {
                ApiResponseModel<List<DiagnosticCode>> model = new ApiResponseModel<List<DiagnosticCode>>() { };
                var client = ServiceFactory.GetService(typeof(Physician));
                data = await client.getDiagnosticCodes(inputmodel.diagnosticCodeId,inputmodel.diagnosticCodeName,inputmodel.practiceId);
                model.data.records = data;
                return Response.Success<List<DiagnosticCode>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetPhysicians(ApiModel inputmodel)
        {
            List<Physician> data = new List<Physician>();
            try
            {
                ApiResponseModel<List<Physician>> model = new ApiResponseModel<List<Physician>>() { };
                var client = ServiceFactory.GetService(typeof(Physician));
                data = await client.getPhysicians(inputmodel.practiceId);
                model.data.records = data;
                return Response.Success<List<Physician>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetLocations(ApiModel inputmodel)
        {
            List<Location> data = new List<Location>();
            try
            {
                ApiResponseModel<List<Location>> model = new ApiResponseModel<List<Location>>() { };
                var client = ServiceFactory.GetService(typeof(Location));
                data = await client.GetLocations(inputmodel.practiceId);
                model.data.records = data;
                return Response.Success<List<Location>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        //[MzkAuthorize]
        public async Task<HttpResponseMessage> PostAddPhysicianToClinic(ApiModel apiModel)
        {
            bool result = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Enrollment));
                result = await client.postAddPhysicianToClinic(apiModel.practiceId, apiModel.physician);
                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> PostRequestNewInsurance(ApiModel apiModel)
        {
            bool result = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Enrollment));
                result = await client.PostRequestNewInsurance(apiModel.practiceId, apiModel.insurance,apiModel.userCreatedBy);
                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> PostPhysicianToPractice(ApiModel inputmodel)
        {
            //Physician data = new Physician();
            //Physician result = null;
           bool result = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Physician));
                result = await client.postPhysicianToPractice(inputmodel.practiceId,inputmodel.choice,inputmodel.physician,inputmodel.userCreatedBy,inputmodel.userModifiedBy); //Choice.Add
                //model.data.records = result.sms;
                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> VerifyNPI(ApiModel inputmodel)
        {
            WarnType data = new WarnType();
            try
            {
                ApiResponseModel<WarnType> model = new ApiResponseModel<WarnType>() { };
                var client = ServiceFactory.GetService(typeof(Physician));
                data = await client.verifyNPI(inputmodel.npi);
                model.data.records = data;
                return Response.Success<WarnType>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}