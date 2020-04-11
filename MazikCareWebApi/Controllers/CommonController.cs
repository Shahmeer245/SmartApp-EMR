using Helper;
using MazikCareService.Core;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using MazikCareService.CosmosDB;

namespace MazikCareWebApi.Controllers
{


    public class CommonController : ApiController
    {

        [HttpGet]
        public HttpResponseMessage UnAuthorize()
        {
            return Response.Unauthorized("Service", "Invalid Authorization Token.");
        }


        [HttpGet]
        public async Task<HttpResponseMessage> GetUserDetails(string Username)
        {
            User data = new User();

            try
            {
                ApiResponseModel<User> model = new ApiResponseModel<User>();
                var client = ServiceFactory.GetService(typeof(CrmService));
                data = await client.GetUserByDomain("domainname", "sumair.irshad");
                model.data.records = data;
                return Response.Success<User>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetUsers(string currentPage)
        {

            List<User> data = new List<User>();
            int CurrentPage = Convert.ToInt32(currentPage);
            try
            {
                ApiResponseModel<List<User>> model = new ApiResponseModel<List<User>>();
                var client = ServiceFactory.GetService(typeof(CrmService));
                data = await client.GetUsers();

                if (data != null)
                {
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.totalCount = data.Count().ToString();
                    model.data.records = data.Take<User>(Convert.ToInt32(AppSettings.GetByKey("PageSize"))).Skip((CurrentPage - 1) * Convert.ToInt32(AppSettings.GetByKey("PageSize"))).ToList();

                }

                return Response.Success<List<User>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetMentions(ApiModel inputmodel)
        {

            List<Mentions> data = new List<Mentions>();
            //int CurrentPage = Convert.ToInt32(currentPage);
            try
            {
                ApiResponseModel<List<Mentions>> model = new ApiResponseModel<List<Mentions>>();
                var client = ServiceFactory.GetService(typeof(Mentions));
                data = client.getMentions(inputmodel.Name);

                if (data != null)
                {
                    //model.data.pagination.currentPage = CurrentPage.ToString();
                    //model.data.pagination.totalCount = data.Count().ToString();
                    model.data.records = data;

                }

                return Response.Success<List<Mentions>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getUserClinicsTree(ApiModel inputmodel)
        {
            List<Clinic> data = new List<Clinic>();

            try
            {
                ApiResponseModel<List<Clinic>> model = new ApiResponseModel<List<Clinic>>() { };

                var client = ServiceFactory.GetService(typeof(Clinic));
                data = await client.getUserClinicsTree(inputmodel.resourceId);

                if (data != null)
                {
                    model.data.records = data.ToList<Clinic>();
                }

                return Response.Success<List<Clinic>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getDocumentFile(ApiModel inputmodel)
        {
            string data = string.Empty;

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(Document));
                data = await client.getDocumentFile(inputmodel.documentRecId);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPowerBIReportList(ApiModel inputmodel)
        {
            List<PowerBIReport> data = new List<PowerBIReport>();

            try
            {
                ApiResponseModel<List<PowerBIReport>> model = new ApiResponseModel<List<PowerBIReport>>() { };

                var client = ServiceFactory.GetService(typeof(User));
                data = client.getPowerBIReports(inputmodel.userId);

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<List<PowerBIReport>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPowerBIAccessToken()
        {
            try
            {
                //PowerBIReport data = new PowerBIReport();
                //var queryVals = Request.RequestUri.ParseQueryString();
                //string code = queryVals["code"];
                //string email = queryVals["email"];
                //string password = queryVals["password"];
                //string username = fdc.Get("username");
                //string password = fdc.Get("password");
                //string accesstoken = GetAccessToken(code, "4354ecb0-9a83-43b9-91af-8f959be2c2da", "M8tcQ9CFQfPAd8QWIp6wsQ1GE3Zbxn4OIVrmo6eiGQw=", "http://localhost:8000/index.html");
                PowerBIAccessToken data = new PowerBIAccessToken();
                //await accesstoken.SetAccessToken("4354ecb0-9a83-43b9-91af-8f959be2c2da", "M8tcQ9CFQfPAd8QWIp6wsQ1GE3Zbxn4OIVrmo6eiGQw=", email, password);
                //AccessToken accesstoken = await SetAccessToken("4354ecb0-9a83-43b9-91af-8f959be2c2da", "M8tcQ9CFQfPAd8QWIp6wsQ1GE3Zbxn4OIVrmo6eiGQw=", "badar.pervaiz@mazikglobal.com", "giga78$");

                //var result = new AddExternalLoginBindingModel();
                // Filling the list with data here...
                //result.AccessToken = accesstoken.access_token;

                // Then I return the list
                //return accesstoken;



                ApiResponseModel<PowerBIAccessToken> model = new ApiResponseModel<PowerBIAccessToken>() { };



                var client = ServiceFactory.GetService(typeof(PowerBIAccessToken));
                data = await client.SetAccessToken(Helper.AppSettings.GetByKey("powerBIClientID"), Helper.AppSettings.GetByKey("powerBIClientSecret"), Helper.AppSettings.GetByKey("powerBIUserName"), Helper.AppSettings.GetByKey("powerBIPassword"));

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<PowerBIAccessToken>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getCompanyLogo()
        {
            string data = string.Empty;

            HttpContext.Current.Items["username"] = AppSettings.GetByKey("USERNAME");
            HttpContext.Current.Items["password"] = AppSettings.GetByKey("PASSWORD");
            HttpContext.Current.Items["domain"] = AppSettings.GetByKey("DOMAIN");

            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>() { };

                var client = ServiceFactory.GetService(typeof(Organization));
                data = await client.getCompanyLogo();

                if (data != null)
                {
                    model.data.records = data;
                }

                return Response.Success<string>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> GetAllPostComments(ApiModel inputmodel)
        {
            List<PostComment> data = new List<PostComment>();

            try
            {
                ApiResponseModel<List<PostComment>> model = new ApiResponseModel<List<PostComment>>() { };

                var client = ServiceFactory.GetService(typeof(PostComment));
                data = await client.GetAllPostComments(inputmodel.PostId);

                if (data != null)
                {
                    model.data.records = data.ToList<PostComment>();
                }

                return Response.Success<List<PostComment>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getPosts(ApiModel inputmodel)
        {
            List<Post> data = new List<Post>();
            int CurrentPage = Convert.ToInt32(inputmodel.currentpage);

            if (CurrentPage < 0)
                return Response.Error("App", "current page is not valid");

            try
            {
                ApiResponseModel<List<Post>> model = new ApiResponseModel<List<Post>>() { };

                var client = ServiceFactory.GetService(typeof(Post));
                data = await client.getPosts(Convert.ToInt32(inputmodel.currentpage), inputmodel.Type);

                if (data != null)
                {
                    model.data.records = data.ToList<Post>();
                    model.data.pagination.currentPage = CurrentPage.ToString();
                    model.data.pagination.hasMoreRecords = Pagination.hasMoreRecords.ToString();

                }

                return Response.Success<List<Post>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> createPost(Post post)
        {
            Post data = new Post();

            try
            {
                ApiResponseModel<Post> model = new ApiResponseModel<Post>() { };
                if (!string.IsNullOrEmpty(post.text))   
                {
                    var client = ServiceFactory.GetService(typeof(Post));
                    data = await client.createPost(post);
                }
                
                model.data.records = data;
                //MessagingHub.sendPost(data);
                return Response.Success<Post>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> signalRPushPost(Post post)
        {
            Post data = post;

            try
            {
                ApiResponseModel<Post> model = new ApiResponseModel<Post>() { };
                model.data.records = data;
                MessagingHub.sendPost(data);
                return Response.Success<Post>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> signalRPushPostComment(PostComment postComment)
        {
            PostComment data = postComment;

            try
            {
                ApiResponseModel<PostComment> model = new ApiResponseModel<PostComment>() { };

                model.data.records = data;
                MessagingHub.sendPostComments(data);
                return Response.Success<PostComment>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> createPostComment(PostComment post)
        {
            PostComment data = new PostComment();

            try
            {
                ApiResponseModel<PostComment> model = new ApiResponseModel<PostComment>() { };
                var client = ServiceFactory.GetService(typeof(PostComment));
                data = await client.createPostComment(post);
                model.data.records = data;
                //MessagingHub.sendPostComments(data);

                return Response.Success<PostComment>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getNotesFile(ApiModel inputmodel)
        {
            MazikCareService.Core.Models.Notes data = new MazikCareService.Core.Models.Notes();
            try
            {
                ApiResponseModel<MazikCareService.Core.Models.Notes> model = new ApiResponseModel<MazikCareService.Core.Models.Notes>();

                var client = ServiceFactory.GetService(typeof(MazikCareService.Core.Models.Notes));
                data = await client.getFile(inputmodel.notesAnnotationId);
                model.data.records = data;

                return Response.Success<MazikCareService.Core.Models.Notes>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        //[HttpGet]
        //public async Task<HttpResponseMessage>  GetRoles()
        //{
        //    List<Role> data = new List<Role>();

        //    try
        //    {

        //        using (var client  = ServiceBusFactory.GetService())
        //        {
        //            data = await client.GetRoles();
        //        }

        //        return Response.Success<List<Role>>(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Response.Exception(ex);
        //    }
        //}

        [HttpPost]
        public async Task<HttpResponseMessage> postFileToftp(FTP ftp)
        {
            string data;
            try
            {
                ApiResponseModel<string> model = new ApiResponseModel<string>();

                var client = ServiceFactory.GetService(typeof(MazikCareService.Core.Models.FTP));
                data = await client.uploadToFTP(ftp);
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
        public async Task<HttpResponseMessage> getOrganizationDetail()
        {
            Organization data = new Organization();

            try
            {
                ApiResponseModel<Organization> model = new ApiResponseModel<Organization>() { };
                var client = ServiceFactory.GetService(typeof(Organization));
                data = await client.getOrganizationDetail();
                model.data.records = data;
                return Response.Success<Organization>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getAlerts(ApiModel inputmodel)
        {
            List<Alert> data = new List<Alert>();

            try
            {
                ApiResponseModel<List<Alert>> model = new ApiResponseModel<List<Alert>>() { };
                var client = ServiceFactory.GetService(typeof(Alert));
                data = await client.getAlerts(inputmodel.patientId);
                model.data.records = data;
                return Response.Success<List<Alert>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> saveCommunication(Communication post)
        {
            bool data = false;

            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Communication));
                data = await client.saveCommunication(post);
                model.data.records = data;
                return Response.Success(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getCommunications(ApiModel inputmodel)
        {
            List<Communication> data = new List<Communication>();

            try
            {
                ApiResponseModel<List<Communication>> model = new ApiResponseModel<List<Communication>>() { };
                var client = ServiceFactory.GetService(typeof(Communication));
                data = await client.getCommunications(inputmodel.patientId);
                model.data.records = data;
                return Response.Success<List<Communication>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreateNotification(Alert alert)
        {
            bool data = new bool();
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Alert));
                data = await client.CreateNotification(alert);
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
        public async Task<HttpResponseMessage> GetFAQS(ApiModel inputmodel)
        {
            List<FrequentlyAskedQuestions> data = new List<FrequentlyAskedQuestions>();
            try
            {
                ApiResponseModel<List<FrequentlyAskedQuestions>> model = new ApiResponseModel<List<FrequentlyAskedQuestions>>() { };
                var client = ServiceFactory.GetService(typeof(FrequentlyAskedQuestions));
                data = await client.getFAQS(inputmodel.patientId);
                model.data.records = data;
                return Response.Success<List<FrequentlyAskedQuestions>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetFAQDetails(ApiModel inputmodel)
        {
            List<QuestionAndAnswer> data = new List<QuestionAndAnswer>();
            try
            {
                ApiResponseModel<List<QuestionAndAnswer>> model = new ApiResponseModel<List<QuestionAndAnswer>>() { };
                var client = ServiceFactory.GetService(typeof(FrequentlyAskedQuestions));
                data = await client.getFAQDetails(inputmodel.FAQId);
                model.data.records = data;
                return Response.Success<List<QuestionAndAnswer>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> CreateReminders(Reminder reminder)
        {
            bool data = new bool();
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Reminder));
                data = await client.createReminders(reminder);
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
        public async Task<HttpResponseMessage> GetReminders(ApiModel inputmodel)
        {
            List<Reminder> data = new List<Reminder>();

            try
            {
                ApiResponseModel<List<Reminder>> model = new ApiResponseModel<List<Reminder>>() { };
                var client = ServiceFactory.GetService(typeof(Reminder));
                data = await client.getReminders(inputmodel.patientOrderId);
                model.data.records = data;
                return Response.Success<List<Reminder>>(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> GetTermsAndConditions()
        {
            Configuration data = new Configuration();
            try
            {
                ApiResponseModel<Configuration> model = new ApiResponseModel<Configuration>() { };
                var client = ServiceFactory.GetService(typeof(Configuration));
                data = await client.getTermsAndConditions();
                model.data.records = data;
                return Response.Success(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        [MzkAuthorize]
        public async Task<HttpResponseMessage> getConfigurationTimeZone()
        {
            bool data = new bool();
            try
            {
                ApiResponseModel<bool> model = new ApiResponseModel<bool>() { };
                var client = ServiceFactory.GetService(typeof(Configuration));
                data = await client.getConfigurationTimeZone();
                model.data.records = data;
                return Response.Success(model);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}
