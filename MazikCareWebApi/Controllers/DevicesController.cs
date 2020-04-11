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
    public class DevicesController : ApiController
    {
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetDeviceTypes(ApiModel inputmodel)
        {
            List<Device> data = new List<Device>();
            try
            {
                ApiResponseModel<List<Device>> model = new ApiResponseModel<List<Device>>() { };
                var client = ServiceFactory.GetService(typeof(Device));
                data = await client.getDeviceTypes(inputmodel.practiceId);
                model.data.records = data;
                return Response.Success<List<Device>>(model);
            }
            catch(Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetDeviceServiceTypes()
        {
            List<DeviceServiceType> data = new List<DeviceServiceType>();
            try
            {
                ApiResponseModel<List<DeviceServiceType>> model = new ApiResponseModel<List<DeviceServiceType>>() { };
                var client = ServiceFactory.GetService(typeof(Device));
                data = await client.getDeviceServiceTypes();
                model.data.records = data;
                return Response.Success<List<DeviceServiceType>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetDeviceSerialNumbers(ApiModel inputmodel)
        {
            List<DeviceSerialNumbers> data = new List <DeviceSerialNumbers>();
            try
            {
                ApiResponseModel<List<DeviceSerialNumbers>> model = new ApiResponseModel<List<DeviceSerialNumbers>>() { };
                var client = ServiceFactory.GetService(typeof(Device));
                data = await client.getDeviceSerialNumbers(inputmodel.deviceId);
                model.data.records = data;
                return Response.Success<List<DeviceSerialNumbers>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetDeviceDeployDurations(ApiModel inputmodel)
        {
            List<DeviceDeploymentDurations> data = new List<DeviceDeploymentDurations>();
            try
            {
                ApiResponseModel<List<DeviceDeploymentDurations>> model = new ApiResponseModel<List<DeviceDeploymentDurations>>() { };
                var client = ServiceFactory.GetService(typeof(Device));
                data = await client.getDeviceDeployDurations(inputmodel.deviceId);
                model.data.records = data;
                return Response.Success<List<DeviceDeploymentDurations>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetAlternateServices(ApiModel inputmodel)
        {
            List<AlternateServices> data = new List<AlternateServices>();
            try
            {
                ApiResponseModel<List<AlternateServices>> model = new ApiResponseModel<List<AlternateServices>>() { };
                var client = ServiceFactory.GetService(typeof(Device));
                data = await client.getAlternateServices(inputmodel.deviceId);
                model.data.records = data;
                return Response.Success<List<AlternateServices>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}