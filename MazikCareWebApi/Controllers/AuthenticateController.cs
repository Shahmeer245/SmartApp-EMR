using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Description;
using MazikCareWebApi.Models;
using MazikCareWebApi.ApiHelpers;
using Helper;
using MazikCareService.Core.Models;
using MazikCareService.Core;
using MazikLogger;
using MazikCareService.CRMRepository;
using MazikCareWebApi.Filters;

namespace MazikCareWebApi.Controllers
{
    public class AuthenticateController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Login([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return Response.ModelStateInValid(ModelState);
            
            ApiResponseModel<LoginModelOutput> apimodel = new ApiResponseModel<LoginModelOutput>();            
            LoginModelOutput output = new LoginModelOutput();
            string token = "";
            try
            {
                output = await MazikCareService.Core.Authentication.Authenticate(model.UserName, model.Password, model.Domain);

                if (output.success)
                {
                    string tokenString = string.Format("{0}:{1}", model.UserName, model.Password);
       
                    token = MazikCareService.Core.Authentication.Base64Encode(tokenString);
                    output.token = token;
                    apimodel.data.records = output;
                }
                else
                {
                    return Response.Unauthorized("CRM", "Credentials are not Valid");
                }

                return Response.Success<LoginModelOutput>(apimodel);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [HttpPost]
        public async Task<HttpResponseMessage> LoginContact([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return Response.ModelStateInValid(ModelState);

            ApiResponseModel<LoginModelOutput> apimodel = new ApiResponseModel<LoginModelOutput>();
            LoginModelOutput output = new LoginModelOutput();
            
            try
            {
                output = await MazikCareService.Core.AzureADB2C.AuthenticateUser(model.UserName, model.Password);

                if (output.success)
                {                    
                    apimodel.data.records = output;
                }
                else
                {
                    return Response.Unauthorized("CRM", "Credentials are not Valid");
                }

                return Response.Success<LoginModelOutput>(apimodel);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> getUserToken([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return Response.ModelStateInValid(ModelState);

            ApiResponseModel<LoginModelOutput> apimodel = new ApiResponseModel<LoginModelOutput>();
            LoginModelOutput output = new LoginModelOutput();
            string token = "";
            try
            {
                output = await MazikCareService.Core.Authentication.AuthenticateUser(model.UserName, model.Password, model.Domain);

                if (output.success)
                {
                    string tokenString = string.Format("{0}:{1}", model.UserName, model.Password);

                    token = MazikCareService.Core.Authentication.Base64Encode(tokenString);
                    output.token = token;
                    apimodel.data.records = output;
                }
                else
                {
                    return Response.Unauthorized("CRM", "Credentials are not Valid");
                }

                return Response.Success<LoginModelOutput>(apimodel);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }


        [MzkAuthorize]
        [HttpPost]
        public async Task<HttpResponseMessage> resendSMSCode(LoginModel model)
        {    
            ApiResponseModel<LoginModelOutput> apimodel = new ApiResponseModel<LoginModelOutput>();
            LoginModelOutput output = new LoginModelOutput();
           
            try
            {
                output = await MazikCareService.Core.Authentication.resendSMSCode(model.sysUserId, model.smsCodeId);
                
                apimodel.data.records = output;

                return Response.Success<LoginModelOutput>(apimodel);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [MzkAuthorize]
        [HttpPost]
        public async Task<HttpResponseMessage> authenticateSMSCode(LoginModel model)
        {
            ApiResponseModel<LoginModelOutput> apimodel = new ApiResponseModel<LoginModelOutput>();
            LoginModelOutput output = new LoginModelOutput();

            try
            {
                output = await MazikCareService.Core.Authentication.authenticateSMSCode(model.sysUserId, model.smsCodeId, model.smsCode);

                apimodel.data.records = output;

                return Response.Success<LoginModelOutput>(apimodel);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> getToken()
        {
            ApiResponseModel<LoginModelOutput> apimodel = new ApiResponseModel<LoginModelOutput>();
            LoginModelOutput output = new LoginModelOutput();
            string token = "";
            try
            {
                string tokenString = string.Format("{0}:{1}", SoapCredential.UserName, SoapCredential.Password);
                token = MazikCareService.Core.Authentication.Base64Encode(tokenString);
                output.token = token;
                apimodel.data.records = output;
                return Response.Success<LoginModelOutput>(apimodel);
            }
            catch (Exception ex)
            {
                return Response.Exception(ex);
            }
        }
    }
}
