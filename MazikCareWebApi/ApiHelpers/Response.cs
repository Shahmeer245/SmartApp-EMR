using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MazikCareWebApi.ApiHelpers;
using System.Net;
using System.Web.Http.ModelBinding;
using MazikCareService.Core.Models;
using MazikLogger;
using System.Web;
using System.IO;

namespace MazikCareWebApi.ApiHelpers
{
   public class Response
    {
        public static HttpResponseMessage Success<T>(ApiResponseModel<T> model)
        {
            ApiResponse<T> response = new ApiResponse<T>(HttpStatusCode.OK, null, null, null, null, model);
            return response.GetResponse();
        }

        public static HttpResponseMessage Error(string type,string errorMessage)
        {
            ApiResponse<List<string>> response = new ApiResponse<List<string>>(HttpStatusCode.BadRequest, type, errorMessage, null, null, null);
            return response.GetResponse();
        }

        public static HttpResponseMessage Unauthorized(string type, string message)
        {
            ApiResponse<List<string>> response = new ApiResponse<List<string>>(HttpStatusCode.Unauthorized, type, message, null, null, null);
            return response.GetResponse();
        }

        public static HttpResponseMessage Exception(Exception exc)
        {
            
            ApiResponse<List<string>> response = null;

            if (exc.GetType().BaseType == typeof(CustomException))
            {
                CustomException customException = (CustomException)exc;
                response = new ApiResponse<List<string>>(customException.ResponceCode, customException.ExceptionType, customException.Message, null, null, null);
            }
            else
            {
                response = new ApiResponse<List<string>>(System.Net.HttpStatusCode.InternalServerError, "Service", exc.Message, null, null, null);
            }

            HttpContext context = HttpContext.Current;
            string requestBody;
            using (Stream receiveStream = context.Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, context.Request.ContentEncoding))
                {
                    requestBody = readStream.ReadToEnd();
                }
            }
            
            bool EnableLogging = Convert.ToBoolean(Helper.AppSettings.GetByKey("EnableLogging"));
            if (EnableLogging)
            {
                ExceptionLogger exLog = new ExceptionLogger();

                exLog.Log(exc, requestBody);
            }
            return response.GetResponse();
        }

        /// <summary>
        /// Generate response of HttpStatusCode.BadRequest when ModelState Validation failed for POST and PUT request
        /// </summary>
        /// <param name="modelState">ModelState from which validation messages to be extracted</param>
        /// <returns></returns>
        public static HttpResponseMessage ModelStateInValid(ModelStateDictionary modelState)
        {
            ApiResponseModel<List<string>> model = new ApiResponseModel<List<string>>();
            ApiResponse<List<string>> response = new ApiResponse<List<string>>(HttpStatusCode.BadRequest,"APP", "model State is not valid", null, modelState, model);
            return response.GetResponse();
        }

    }
}
