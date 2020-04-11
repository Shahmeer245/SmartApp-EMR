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
    
    public class PatientEncounterController : ApiController
    {
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddPatientEncounter(ApiModel inputmodel)
        {
            string data = string.Empty;
            
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                data = await client.addPatientEncounter((inputmodel.CaseId), inputmodel.encounterType, inputmodel.worklistTypeID, inputmodel.resourceRecId, inputmodel.cpsaWorkflowId, inputmodel.resourceId , inputmodel.patientId, inputmodel.appointmentId);

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
        public async Task<HttpResponseMessage> getEncounterDetails(PatientEncounter inputmodel)
        {
            List<PatientEncounter> data = new List<PatientEncounter>();
            int CurrentPage = 1;// Convert.ToInt32(inputmodel.currentpage);

            //if (CurrentPage < 0)
            //    return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientEncounter>> model = new ApiResponseModel<List<PatientEncounter>>() { };
                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                data = await client.encounterDetails(inputmodel);

                if (data != null)
                {                    
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    //int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data;//.Take<PatientEncounter>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<PatientEncounter>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getEncounterTemplate(ApiModel inputmodel)
        {
            List<UITemplate> data = new List<UITemplate>();
            
            try
            {
                ApiResponseModel<List<UITemplate>> model = new ApiResponseModel<List<UITemplate>>() { };

                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                data = await client.getEncounterTemplate(inputmodel.EncounterTemplateId, inputmodel.cpsaWorkflowId, inputmodel.patientId);

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
        public async Task<HttpResponseMessage> updateEncounter(PatientEncounter inputmodel)
        {
            bool data = false;

            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                data = await client.updateEncounter(inputmodel.EncounterId, inputmodel.EncounterStatus, inputmodel.userId ,inputmodel.isAutoSignOff);
                model.data.records = data;
                //if (data != null)
                //{
                //    model.data.records = data.ToList<string>();
                //}

                return Response.Success<bool>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> ConsultationComplete(PatientEncounter inputmodel)
        {
            bool data = false;

            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                data = await client.ConsultationComplete(inputmodel.EncounterId);
                model.data.records = data;
                //if (data != null)
                //{
                //    model.data.records = data.ToList<string>();
                //}

                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

    /*    [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getConceptData(Concept inputmodel)
        {
            List<Concept> data = new List<Concept>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");            

            try
            {
                ApiResponseModel<List<Concept>> model = new ApiResponseModel<List<Concept>>() { };
                var client = ServiceFactory.GetService(typeof(Concept));
                data = await client.getConceptData(inputmodel, inputmodel.currentpage);

                if (data != null)
                {
                    model.data.records = data;
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = Pagination.totalCount.ToString();
                }

                return Response.Success<List<Concept>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }*/
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updateVisitReason(ApiModel inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisit));
                data = await client.updateVisitReason(inputmodel.patientEncounterId, inputmodel.VisitReason);
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
        public async Task<HttpResponseMessage> getVisitReason(ApiModel inputmodel)
        {
            List<PatientVisit> data = new List<PatientVisit>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientVisit>> model = new ApiResponseModel<List<PatientVisit>>() { };

                var client = ServiceFactory.GetService(typeof(PatientVisit));
                data = await client.getVisitReason(inputmodel.patientId, inputmodel.patientEncounterId);


                if (data != null)
                {
                    //model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<PatientVisit>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }

                return Response.Success<List<PatientVisit>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updateCheifComplaint(ApiModel inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(CheifComplaint));
                data = await client.updateCheifComplaint(inputmodel.patientEncounterId, inputmodel.CheifComplaint);
                model.data.records = data;


                return Response.Success<bool>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]       
        public async Task<HttpResponseMessage> getCheifComplaint(ApiModel inputmodel)
        {
            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            List<CheifComplaint> data = new List<CheifComplaint>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<CheifComplaint>> model = new ApiResponseModel<List<CheifComplaint>>() { };

                var client = ServiceFactory.GetService(typeof(CheifComplaint));
                data = await client.getCheifComplaint(inputmodel.patientId, inputmodel.patientEncounterId, inputmodel.AxAppoitnmentRefRecId, inputmodel.CaseId);


                if (data != null)
                {
                    //model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<CheifComplaint>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }

                return Response.Success<List<CheifComplaint>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getCheifComplaintFromClinicalTemplate(ApiModel inputmodel)
        {
            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            List<CheifComplaint> data = new List<CheifComplaint>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<CheifComplaint>> model = new ApiResponseModel<List<CheifComplaint>>() { };

                var client = ServiceFactory.GetService(typeof(CheifComplaint));
                data = await client.getCheifComplaintFromClinicalTemplate(inputmodel.CaseId, inputmodel.searchOrder, inputmodel.appointmentId);

                if (data != null)
                {
                    //model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<CheifComplaint>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }

                return Response.Success<List<CheifComplaint>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetVitalsList(ApiModel inputmodel)
        {
            List<MmtGroupCode> data = new List<MmtGroupCode>();

            try
            {
                ApiResponseModel<List<MmtGroupCode>> model = new ApiResponseModel<List<MmtGroupCode>>() { };

                var client = ServiceFactory.GetService(typeof(MmtGroupCode));
                data = await client.GetVitalsList(inputmodel.patientId);

                if (data != null)
                {
                    model.data.records = data.ToList<MmtGroupCode>();
                }

                return Response.Success<List<MmtGroupCode>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddVitals(List<PatientVitals> inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(PatientVitals));
                data = await client.AddVitals(inputmodel, false, true);
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
        public async Task<HttpResponseMessage> UpdateVitalValues(List<PatientVitals> inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(PatientVitals));
                data = await client.UpdateVitalValues(inputmodel);
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
        public async Task<HttpResponseMessage> DeleteVitals(List<PatientVitals> inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(PatientVitals));
                data = await client.DeleteVitals(inputmodel);
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
        public async Task<HttpResponseMessage> updatePatientReferral(PatientReferralOrder patientReferral)
        {
            //string data = string.Empty;
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(PatientReferralOrder));
                data = await client.updatePatientReferral(patientReferral);
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
        public async Task<HttpResponseMessage> AddPatientDiagnosis(PatientDiagnosis patientDiagnosis)
        {
            string data = string.Empty;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientDiagnosis));
                data = await client.AddPatientDiagnosis(patientDiagnosis);

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
        public async Task<HttpResponseMessage> getPatientDiagnosis(ApiModel inputmodel)
        {
            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            List<PatientDiagnosis> data = new List<PatientDiagnosis>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientDiagnosis>> model = new ApiResponseModel<List<PatientDiagnosis>>() { };
                var client = ServiceFactory.GetService(typeof(PatientDiagnosis));
                data = await client.getPatientDiagnosis(inputmodel.patientEncounterId, inputmodel.SearchFilters, inputmodel.searchOrder, inputmodel.startDate, inputmodel.endDate, inputmodel.AxAppoitnmentRefRecId, inputmodel.CaseId);

                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientDiagnosis>().CreatedOn);
                        model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientDiagnosis>().CreatedOn);
                    }
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<PatientDiagnosis>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<PatientDiagnosis>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updatePatientDiagnosis(PatientDiagnosis patientDiagnosis)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(PatientDiagnosis));
                data = await client.updatePatientDiagnosis(patientDiagnosis);
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
        public async Task<HttpResponseMessage> getPatientEncounterVitals(ApiModel inputmodel)
        {
            List<PatientVitals> data = new List<PatientVitals>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientVitals>> model = new ApiResponseModel<List<PatientVitals>>() { };

                var client = ServiceFactory.GetService(typeof(PatientVitals));
                data = await client.getPatientEncounterVitals(inputmodel.patientId, inputmodel.patientEncounterId, false);


                if (data != null)
                {

                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<PatientVitals>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }

                return Response.Success<List<PatientVitals>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getLastRecordedVitals(ApiModel inputmodel)
        {
            List<PatientVitals> data = new List<PatientVitals>();

            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientVitals>> model = new ApiResponseModel<List<PatientVitals>>() { };

                var client = ServiceFactory.GetService(typeof(PatientVitals));
                data = await client.getLastRecordedVitals(inputmodel.CaseId, inputmodel.appointmentId);

                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<PatientVitals>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<PatientVitals>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getClinicalTemplatesList(ApiModel inputmodel)
        {
            List<ClinicalTemplate> data = new List<ClinicalTemplate>();

            try
            {
                ApiResponseModel<List<ClinicalTemplate>> model = new ApiResponseModel<List<ClinicalTemplate>>() { };

                var client = ServiceFactory.GetService(typeof(ClinicalTemplate));
                data = await client.getClinicalTemplatesList(inputmodel.patientId, inputmodel.Name, false, inputmodel.resourceId);
                
                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<List<ClinicalTemplate>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getClinicalTemplatesListDetail(ApiModel inputmodel)
        {
            List<ClinicalTemplate> data = new List<ClinicalTemplate>();

            try
            {
                ApiResponseModel<List<ClinicalTemplate>> model = new ApiResponseModel<List<ClinicalTemplate>>() { };

                var client = ServiceFactory.GetService(typeof(ClinicalTemplate));
                data = await client.getClinicalTemplatesList(string.Empty, string.Empty, true);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<List<ClinicalTemplate>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPatientClinicalTemplates(ApiModel inputmodel)
        {
            List<ClinicalTemplate> data = new List<ClinicalTemplate>();

            try
            {
                ApiResponseModel<List<ClinicalTemplate>> model = new ApiResponseModel<List<ClinicalTemplate>>() { };

                var client = ServiceFactory.GetService(typeof(ClinicalTemplate));
                data = await client.getPatientClinicalTemplates(inputmodel.patientId, inputmodel.patientEncounterId);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<List<ClinicalTemplate>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getClinicalNarrationFile(ApiModel inputmodel)
        {
            ClinicalTemplateNarration data = new ClinicalTemplateNarration();

            try
            {
                ApiResponseModel<ClinicalTemplateNarration> model = new ApiResponseModel<ClinicalTemplateNarration>() { };

                var client = ServiceFactory.GetService(typeof(ClinicalTemplate));
                data = await client.getFile(inputmodel.narrationGuid);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<ClinicalTemplateNarration>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getTemplateDetails(ApiModel inputmodel)
        {
            ClinicalTemplate data = new ClinicalTemplate();

            try
            {
                ApiResponseModel<ClinicalTemplate> model = new ApiResponseModel<ClinicalTemplate>() { };

                var client = ServiceFactory.GetService(typeof(ClinicalTemplate));
                data = await client.getTemplateDetails(inputmodel.templateGuid, inputmodel.patientId, inputmodel.patientEncounterId);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<ClinicalTemplate>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> saveClinicalTemplateDetails(ApiModel inputmodel)
        {
            bool data;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(ClinicalTemplate));
                data = await client.saveTemplateDetails(inputmodel.narrationList, inputmodel.patientId, inputmodel.patientEncounterId);
                                
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
        public async Task<HttpResponseMessage> addPatientClinicalTemplates(ApiModel inputmodel)
        {
            bool data;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(ClinicalTemplate));
                data = await client.addClinicalTemplates(inputmodel.patientId, inputmodel.patientEncounterId, inputmodel.templateList);

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
        public async Task<HttpResponseMessage> removePatientClinicalTemplates(ApiModel inputmodel)
        {
            bool data;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(ClinicalTemplate));
                data = await client.removeClinicalTemplates(inputmodel.patientId, inputmodel.patientEncounterId, inputmodel.templateList);

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
        public async Task<HttpResponseMessage> AddPatientDisposition(PatientDisposition patientDisposition)
        {
            string data = string.Empty;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(PatientDisposition));
                data = await client.AddPatientDisposition(patientDisposition);

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
        public async Task<HttpResponseMessage> getPatientDisposition(ApiModel inputmodel)
        {
            List<PatientDisposition> data = new List<PatientDisposition>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientDisposition>> model = new ApiResponseModel<List<PatientDisposition>>() { };
                var client = ServiceFactory.GetService(typeof(PatientDisposition));
                data = await client.getPatientDisposition(inputmodel.patientEncounterId, inputmodel.SearchFilters, inputmodel.searchOrder, inputmodel.startDate, inputmodel.endDate);
                if (data != null)
                {
                    //if (!string.IsNullOrEmpty(inputmodel.patientEncounterId))
                    //    model.data.hasDiagnosis = new PatientDiagnosis().hasDiagnosis(inputmodel.patientEncounterId);

                    if (data.Count > 0)
                    {
                        model.data.dateRange.startDate = String.Format("{0:yyyy-MM-dd}", data.Last<PatientDisposition>().CreatedOn);
                        model.data.dateRange.endDate = String.Format("{0:yyyy-MM-dd}", data.First<PatientDisposition>().CreatedOn);
                    }

                        model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    // int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data;//.Take<PatientDisposition>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<PatientDisposition>>(model);
            }
             catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updatePatientDisposition(PatientDisposition patientDisposition)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(PatientDisposition));
                data = await client.updatePatientDisposition(patientDisposition);
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
        public async Task<HttpResponseMessage> UpdateEncounterAllergies(PatientEncounter patientEncounter)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                data = await client.UpdateEncounterAllergies(patientEncounter);
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
        public async Task<HttpResponseMessage> UpdateAllergies(ApiModel patientAllergy)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(PatientAllergy));
                data = await client.updatePatientAllergies(patientAllergy.patientId, patientAllergy.AllergyReviewedBy, patientAllergy.allergyList);
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
        public async Task<HttpResponseMessage> getConsultationSummary(ApiModel inputmodel)
        {
            List<ConsultationSummary> data = new List<ConsultationSummary>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<ConsultationSummary>> model = new ApiResponseModel<List<ConsultationSummary>>() { };
                var client = ServiceFactory.GetService(typeof(ConsultationSummary));

                data = await client.getConsultationSummary(inputmodel.patientEncounterId,inputmodel.summaryUpdated);
                if (data != null)
                {
                   
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                   // int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data;//.Take<ConsultationSummary>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<ConsultationSummary>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> triageEncounter(ApiModel inputmodel)
        {
            List<ConsultationSummary> data = new List<ConsultationSummary>();
            int CurrentPage = 1;// Convert.ToInt32(inputmodel.currentpage);
            
            try
            {
                ApiResponseModel<List<ConsultationSummary>> model = new ApiResponseModel<List<ConsultationSummary>>() { };
                var client = ServiceFactory.GetService(typeof(ConsultationSummary));
                data = await client.triageConsultationSummary(inputmodel.CaseId, inputmodel.cpsaWorkflowId);
                if (data != null)
                {

                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                   // int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data;//.Take<ConsultationSummary>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<ConsultationSummary>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> assessmentEncounter(ApiModel inputmodel)
        {
            List<ConsultationSummary> data = new List<ConsultationSummary>();
            int CurrentPage = 1;// Convert.ToInt32(inputmodel.currentpage);

            try
            {
                ApiResponseModel<List<ConsultationSummary>> model = new ApiResponseModel<List<ConsultationSummary>>() { };
                var client = ServiceFactory.GetService(typeof(ConsultationSummary));
                data = await client.assessmentConsultationSummary(inputmodel.CaseId);
                if (data != null)
                {

                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    //int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data;//.Take<ConsultationSummary>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<ConsultationSummary>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> updateNurseInstruction(ApiModel patientEncounter)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                data = await client.updateNurseInstruction(patientEncounter.patientEncounterId,patientEncounter.NurseInstruction);
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
        public async Task<HttpResponseMessage> getCarePLan(ApiModel inputmodel)
        {
            List<ConsultationSummary> data = new List<ConsultationSummary>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<ConsultationSummary>> model = new ApiResponseModel<List<ConsultationSummary>>() { };
                var client = ServiceFactory.GetService(typeof(ConsultationSummary));
                data = await client.getCarePLan(inputmodel.CaseId);
                if (data != null)
                {

                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<ConsultationSummary>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<ConsultationSummary>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetOPConsultationList(ApiModel inputmodel)
        {
            List<PatientEncounter> data = new List<PatientEncounter>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientEncounter>> model = new ApiResponseModel<List<PatientEncounter>>() { };
                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                data = await client.GetEncounterList(inputmodel.patientId,inputmodel.SpecialityId ,inputmodel.caseType=(int)mzk_casetype.OutPatient,inputmodel.encounterType=(int)mzk_encountertype.Consultation);
                if (data != null)
                {

                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<PatientEncounter>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<PatientEncounter>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getOPNonConsultaiton(ApiModel inputmodel)
        {
            List<PatientEncounter> data = new List<PatientEncounter>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientEncounter>> model = new ApiResponseModel<List<PatientEncounter>>() { };
                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                data = await client.GetEncounterList(inputmodel.patientId, inputmodel.SpecialityId = null, inputmodel.caseType = (int)mzk_casetype.OutPatient, inputmodel.encounterType = 0);
                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<PatientEncounter>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<PatientEncounter>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetERPrimaryAssessmentList(ApiModel inputmodel)
        {
            List<PatientEncounter> data = new List<PatientEncounter>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<PatientEncounter>> model = new ApiResponseModel<List<PatientEncounter>>() { };
                var client = ServiceFactory.GetService(typeof(PatientEncounter));
                data = await client.GetEncounterList(inputmodel.patientId, inputmodel.SpecialityId = null, inputmodel.caseType = (int)mzk_casetype.Emergency, inputmodel.encounterType = (int) mzk_encountertype.PrimaryAssessment);
                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data.Take<PatientEncounter>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<PatientEncounter>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> AddUserFavourite(Concept inputmodel)
        {
            string data = string.Empty;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };
                var client = ServiceFactory.GetService(typeof(Concept));
                data = await client.AddUserFavourite(inputmodel);
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
        public async Task<HttpResponseMessage> removeUserFavourite(Concept inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(Concept));
                data = await client.removeUserFavourite(inputmodel);
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
        public async Task<HttpResponseMessage> getAllEducationalResource(ApiModel inputmodel)
        {
            List<EducationalResource> data = new List<EducationalResource>();
            try
            {
                ApiResponseModel<List<EducationalResource>> model = new ApiResponseModel<List<EducationalResource>>();

                var client = ServiceFactory.GetService(typeof(EducationalResource));
                data = await client.getAllEducationalResource(inputmodel.SearchFilters);
                model.data.records = data;

                return Response.Success<List<EducationalResource>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }

        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> addPatientEncounterEducationalResource(List<EducationalResource> inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(EducationalResource));
                data = await client.addPatientEncounterEducationalResource(inputmodel);
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
        public async Task<HttpResponseMessage> getPatientEncounterEducationalResource(ApiModel inputmodel)
        {
            List<EducationalResource> data = new List<EducationalResource>();
            try
            {
                ApiResponseModel<List<EducationalResource>> model = new ApiResponseModel<List<EducationalResource>>();

                var client = ServiceFactory.GetService(typeof(EducationalResource));
                data = await client.getPatientEncounterEducationalResource(inputmodel.patientEncounterId, inputmodel.patientId);
                model.data.records = data;

                return Response.Success<List<EducationalResource>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> removePatientEncounterEducationalResource(List<EducationalResource> inputmodel)
        {
            bool data = false;
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };

                var client = ServiceFactory.GetService(typeof(EducationalResource));
                data = await client.removePatientEncounterEducationalResource(inputmodel);
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
        public async Task<HttpResponseMessage> getContractProducts(ApiModel inputmodel)
        {
            List<Products> data = new List<Products>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<Products>> model = new ApiResponseModel<List<Products>>() { };
                var client = ServiceFactory.GetService(typeof(Products));
                data = await client.getContractProducts(inputmodel.workOrderId);
                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    int pageSize = Convert.ToInt32(AppSettings.GetByKey("PageSize"));
                    model.data.records = data;//.Take<Products>(Convert.ToInt32(CurrentPage * pageSize)).Skip((CurrentPage - 1) * pageSize).ToList();
                }
                return Response.Success<List<Products>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


    }
}