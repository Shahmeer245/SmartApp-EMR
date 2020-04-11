using MazikCareWebApi.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MazikCareService.Core.Models;
using MazikCareWebApi.ApiHelpers;
using MazikCareService.Core;
using MazikCareWebApi.Models;

namespace MazikCareWebApi.Controllers
{
    public class CareProgramController : ApiController
    {

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getCarePrograms(ApiModel apiModel)
        {
            List<CareProgram> data = new List<CareProgram>();

            try
            {
                ApiResponseModel<List<CareProgram>> model = new ApiResponseModel<List<CareProgram>>() { };

                var client = ServiceFactory.GetService(typeof(CareProgram));
                data = await client.getCarePrograms(apiModel.Name);

                if (data != null)
                {
                    model.data.records = data.ToList<CareProgram>();
                }

                return Response.Success<List<CareProgram>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> addProgramEnrollmentRequest(ProgramEnrollmentRequest per)
        {
            //List<CareProgram> data = new List<CareProgram>();

            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(CareProgram));
                bool data = await client.addProgramEnrollmentRequest(per.patientEncounterId, per.programId, per.patientId);
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
        public async Task<HttpResponseMessage> getProgramEnrollmentRequest(ProgramEnrollmentRequest per)
        {
            List<ProgramEnrollmentRequest> data = new List<ProgramEnrollmentRequest>();

            try
            {
                ApiResponseModel<List<ProgramEnrollmentRequest>> model = new ApiResponseModel<List<ProgramEnrollmentRequest>>() { };

                var client = ServiceFactory.GetService(typeof(CareProgram));
                data = await client.getProgramEnrollmentRequest(per.patientEncounterId);

                if (data != null)
                {
                    model.data.records = data.ToList<ProgramEnrollmentRequest>();
                }

                return Response.Success<List<ProgramEnrollmentRequest>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

    }
}
