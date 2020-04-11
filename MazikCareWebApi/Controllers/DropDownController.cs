using Helper;
using MazikCareService.Core.Models;
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
    [MzkAuthorize]
    public class DropDownController : ApiController
    {

        [HttpPost]
        public async Task<HttpResponseMessage> getDropDownList(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();
            
            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getDropDownList(inputmodel.entityName, inputmodel.patientEncounterId, inputmodel.SpecialityId,inputmodel.DosageId,inputmodel.patientId, inputmodel.dropDownName);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getOptionSetList(ApiModel inputmodel)
        {
           DropDownList data = new DropDownList();

            try
            {
                ApiResponseModel<DropDownList> model = new ApiResponseModel<DropDownList>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data =  client.getOptionSetList(inputmodel.Type, inputmodel.entityName, inputmodel.attributename, inputmodel.defaultVal);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<DropDownList>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getOrderSetupList(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();

            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getOrderSetupList(inputmodel.Type, inputmodel.entityName, inputmodel.SpecialityId, inputmodel.DosageId);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getEncounterDiagnosisList(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();

            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getEncounterDiagnosisList(inputmodel.Type, inputmodel.entityName, inputmodel.SpecialityId, inputmodel.DosageId);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getConceptList(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();

            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getConceptList(inputmodel.Type, inputmodel.typeValue);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getLocationList(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();

            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getLocationList(inputmodel.Type, inputmodel.typeValue);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getEncounterSetupList(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();

            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getEncounterSetupList(inputmodel.Type, inputmodel.entityName, inputmodel.SpecialityId, inputmodel.DosageId);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> getSpecialityList(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();

            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getSpecialityList(inputmodel.Type, inputmodel.entityName, inputmodel.SpecialityId, inputmodel.DosageId);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getUnitList(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();

            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getUnitList(inputmodel.Type, inputmodel.entityName, inputmodel.SpecialityId, inputmodel.DosageId);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getUserList(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();

            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getUserList(inputmodel.Type, inputmodel.SpecialityId);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getReferringPhysicianList(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();

            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getReferringPhysicianList(inputmodel.Type, inputmodel.SpecialityId);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getAXDropDown(ApiModel inputmodel)
        {
            List<DropDownList> data = new List<DropDownList>();

            try
            {
                ApiResponseModel<List<DropDownList>> model = new ApiResponseModel<List<DropDownList>>() { };
                var client = ServiceFactory.GetService(typeof(DropDownList));
                data = await client.getAXDropDown(inputmodel.Type);
                if (data != null)
                {
                    model.data.records = data;
                }
                return Response.Success<List<DropDownList>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

    }

}
