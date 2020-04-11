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
    public class SetupController : ApiController
    {
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> saveClinicalTemplate(ClinicalTemplate inputmodel)
        {
            bool data;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(ClinicalTemplate));
                data = await client.saveClinicalTemplate(inputmodel);

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
        public async Task<HttpResponseMessage> saveStatusManager(StatusManagerHeader inputmodel)
        {
            bool data;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(StatusManagerHeader));
                data = await client.saveStatusManager(inputmodel);

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
        public async Task<HttpResponseMessage> getStatusManagerList()
        {
            List<StatusManagerHeader> data;

            try
            {
                ApiResponseModel<List<StatusManagerHeader>> model = new ApiResponseModel<List<StatusManagerHeader>>() { };

                var client = ServiceFactory.GetService(typeof(StatusManagerHeader));
                data = await client.getStatusManagerList();

                model.data.records = data;

                return Response.Success<List<StatusManagerHeader>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getStatusManagerDetails(ApiModel inputmodel)
        {
            StatusManagerHeader data;

            try
            {
                ApiResponseModel<StatusManagerHeader> model = new ApiResponseModel<StatusManagerHeader>() { };

                var client = ServiceFactory.GetService(typeof(StatusManagerHeader));
                data = await client.getStatusManagerDetails(inputmodel.statusManagerId);

                model.data.records = data;

                return Response.Success<StatusManagerHeader>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getReportServer()
        {
            string data = System.Configuration.ConfigurationManager.AppSettings["ReportServer"];

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
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
        public async Task<HttpResponseMessage> getAXReportServer()
        {
            string data = System.Configuration.ConfigurationManager.AppSettings["AXReportServer"];

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                model.data.records = data;

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}
