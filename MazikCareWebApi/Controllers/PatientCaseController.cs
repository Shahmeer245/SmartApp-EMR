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
    
    public class PatientCaseController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> AddPatientCase(ApiModel inputmodel)
        {
            PatientCase data = new PatientCase();

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            try
            {
                ApiResponseModel<PatientCase> model = new ApiResponseModel<PatientCase>() { };
                var client = ServiceFactory.GetService(typeof(PatientCase));
                data = await client.addPatientCase(inputmodel.patientId, inputmodel.patientRecId, inputmodel.caseType, inputmodel.clinicId);
                model.data.records = data;
                return Response.Success<PatientCase>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> markFinancialDischarge(ApiModel inputmodel)
        {
            bool data = false;

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(PatientCase));
                data = await client.markFinancialDischarge(inputmodel.CaseId);
                model.data.records = data;
                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> markClosed(ApiModel inputmodel)
        {
            bool data = false;

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(PatientCase));
                data = await client.markClosed(inputmodel.CaseId);
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
        public async Task<HttpResponseMessage> CreateCase(ApiModel inputmodel)
        {
            long data = 0;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientCase));
                data = await client.createCase(inputmodel.patientId, inputmodel.clinicId, inputmodel.caseType, inputmodel.CaseId, inputmodel.caseNumber);
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
        public async Task<HttpResponseMessage> getOrderSetup(ApiModel inputmodel)
        {
            List<OrderSetup> data = new List<OrderSetup>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<OrderSetup>> model = new ApiResponseModel<List<OrderSetup>>() { };

                var client = ServiceFactory.GetService(typeof(OrderSetup));
                data = await client.getOrderSetup(inputmodel.Type);

                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<OrderSetup>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();                 
                }

                return Response.Success<List<OrderSetup>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getConcept(ApiModel inputmodel)
        {
            List<Concept> data = new List<Concept>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<Concept>> model = new ApiResponseModel<List<Concept>>() { };

                var client = ServiceFactory.GetService(typeof(Concept));
                data = await client.getConcept(inputmodel.Type);

                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<Concept>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                    
                }

                return Response.Success<List<Concept>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientCase(ApiModel inputmodel)
        {
            List<PatientCase> data = new List<PatientCase>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientCase>> model = new ApiResponseModel<List<PatientCase>>() { };
                var client = ServiceFactory.GetService(typeof(PatientCase));
                data = await client.getPatientCase(inputmodel.patientId);

                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<PatientCase>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                    
                }

                return Response.Success<List<PatientCase>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddNotes(MazikCareService.Core.Models.Notes notes)
        {
            string data = string.Empty;
            notes.EntityType = "mzk_patientencounter";
            notes.Subject = "Progress Notes";
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>();
                var client = ServiceFactory.GetService(typeof(MazikCareService.Core.Models.Notes));
                data = await client.AddNotes(notes);
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
        public async Task<HttpResponseMessage> getNotes(MazikCareService.Core.Models.Notes notes)
        {
            List<MazikCareService.Core.Models.Notes> data = new List<MazikCareService.Core.Models.Notes>();
            int CurrentPage = 1;

                if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

                try
                {
                    ApiResponseModel<List<MazikCareService.Core.Models.Notes>> model = new ApiResponseModel<List<MazikCareService.Core.Models.Notes>>() { };
                    var client = ServiceFactory.GetService(typeof(MazikCareService.Core.Models.Notes));
                    data = await client.getNotes(notes);

                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<MazikCareService.Core.Models.Notes>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                    return Response.Success<List<MazikCareService.Core.Models.Notes>>(model);
                }
                catch (Exception ex)
                {
                    return Response.Exception(ex);
                }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getCaseNotes(ApiModel apimodel)
        {
            List<MazikCareService.Core.Models.Notes> data = new List<MazikCareService.Core.Models.Notes>();
            int CurrentPage = 1;

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<MazikCareService.Core.Models.Notes>> model = new ApiResponseModel<List<MazikCareService.Core.Models.Notes>>() { };
                var client = ServiceFactory.GetService(typeof(MazikCareService.Core.Models.Notes));
                data = await client.getCaseNotes(apimodel.CaseId);

                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<MazikCareService.Core.Models.Notes>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }    
                
                return Response.Success<List<MazikCareService.Core.Models.Notes>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updateNotes(MazikCareService.Core.Models.Notes notes)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(MazikCareService.Core.Models.Notes));
                data = await client.updateNotes(notes);
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
        public async Task<HttpResponseMessage> getPatient(ApiModel inputmodel)
        {
            string data = string.Empty;
            try
                {
                    ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                    var client = ServiceFactory.GetService(typeof(Patient));
                    data = client.getPatientIdFromRefRecId(inputmodel.patientRecId);
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
        public async Task<HttpResponseMessage> createCaseInsurance(ApiModel inputmodel)
        {
            string result = string.Empty;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>();
                var client = ServiceFactory.GetService(typeof(PatientCaseInsurance));
                result = await client.createCaseInsurance(inputmodel.CaseId);

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
        public async Task<HttpResponseMessage> createCaseTransByCase(PatientOrder inputmodel)
        {
            bool result = false;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>();
                var client = ServiceFactory.GetService(typeof(PatientOrder));
                //dictionary = await client.getWorkOrderProduct(inputmodel.Id);

                //result = await client.createCaseTransByCase(dictionary["mzk_caseid"] , "", dictionary["msdyn_product"], mzk_orderstatus.Ordered, dictionary["mzk_axclinicrefrecid"]);
                result = await client.createCaseTransByCase(inputmodel.CaseId, "", inputmodel.product, mzk_orderstatus.Ordered, inputmodel.clinicid);

                model.data.records = result;

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> createClaim(Claim claim)
        {
            bool result = false;

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            HttpContext.Current.Items["username"] = AppSettings.GetByKey("USERNAME");
            HttpContext.Current.Items["password"] = AppSettings.GetByKey("PASSWORD");

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>();
                var client = ServiceFactory.GetService(typeof(Claim));
                result = await client.createClaim(claim);

                model.data.records = result.ToString();

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

    }
}
