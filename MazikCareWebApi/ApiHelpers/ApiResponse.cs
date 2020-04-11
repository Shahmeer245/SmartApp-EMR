using Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ModelBinding;

namespace MazikCareWebApi.ApiHelpers
{
    public class ApiResponse<T>
    {
        private ApiResponseModel<T> _model;
        private data<T> _data;
        private HttpStatusCode _statusCode;
        public List<Error> _errors;
        public List<Warning> _warnings;
        public ApiResponse(HttpStatusCode statusCode,string type = null, string errorMessage = null, Exception exc = null,
           ModelStateDictionary modelState = null, ApiResponseModel<T> model = default(ApiResponseModel<T>))
        {
           //
            this._errors = new List<Error>();
            this._warnings = new List<Warning>();
            this._model = model;
            this._statusCode = statusCode;

            if (model != null)
            {
               this._data = this._model.data;
               this._model.data.pagination.recordPerPage = (Convert.ToInt32(this._model.data.pagination.totalCount) > 0) ? AppSettings.GetByKey("PageSize") : "0";
            }

            if (HttpContext.Current.Items["warningmessage"]!=null)
            {
                this._warnings.Add(new Warning() { type = type, message = new List<string>() { HttpContext.Current.Items["warningmessage"].ToString() } });
            }


            #region Incase of Error and Unauthorization

            if (!string.IsNullOrEmpty(errorMessage))
            {
                this._errors.Add(new Error() {type = type,message = new List<string>() { errorMessage} });
            }

            #endregion

            #region Incase of Exception

            if (exc != null)
            {
               
                string innerException = exc.InnerException != null ? exc.InnerException.Message : string.Empty;
                this._errors.Add(new Error()
                {
                    type = type,
                    message = new List<string>()
                    {
                        string.Format("Message : {0}", exc.Message),
                        string.Format("Source: {0}", exc.Source),
                        string.Format("InnerException: {0}", innerException)
                    }
                });
            }

            #endregion

            #region Incase of ModelState InValid for Validation

            if (modelState != null)
            {
                if (modelState.Count() > 0)
                    foreach (var keyValue in modelState)
                        this._errors.Add(new Error() { type = type, message = new List<string>() { string.Format("{0}", keyValue.Value.Errors.FirstOrDefault().ErrorMessage),/* string.Format("{0}", keyValue.Value.Errors.FirstOrDefault().Exception)*/ } });
              
            }

            #endregion

            #region Incase of Success
            //there is no need to do anything, all set already for success
            #endregion
        }


        public HttpResponseMessage GetResponse()
        {
            try
            {
                return new HttpResponseMessage()
                {
                    StatusCode = this._statusCode,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResponseModel<T>()
                    { 
                        error = this._errors,
                        data = this._data,
                        warning = this._warnings
                    }))
                };
            }
            catch (Exception exc)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiResponseModel<List<string>>()
                    {
                        error = new List<Error>() { new Error { type = "Service", message = { "GenericResponse.GetResponse failed.Exception : " + exc.Message } } },
                        data = null
                    }))
                };
            }
        }
    }

    public class ApiResponseModel<T>
    {
        public ApiResponseModel()
        {
            data = new data<T>();
          
        }
        /// <summary>
        /// Error Flag to inform about error exists or not
        /// </summary>
        public List<Error> error { get; set; }     
        /// <summary>
        /// Data according to Api request sent
        /// </summary>
        public data<T> data { get; set; }

        public List<Warning> warning { get; set; }



    }

    public class Warning
    {
        public string type { get; set; }
        public List<string> message { get; set; }
    }

    public class Error
    {
       public  string type { get; set; } 
       public  List<string> message { get; set; }  
    }

    public class dateRange
    {
        public string startDate { get; set; }

        public string endDate { get; set; }
    }

    public class pagination
    {
        public string totalCount { get; set; }
        public string recordPerPage { get; set; }
        public string currentPage { get; set; }
        public string hasMoreRecords { get; set; }
      
    }

    public class data<T>
    {
        public data()
        {
            dateRange = new dateRange() {startDate = string.Empty, endDate = string.Empty };
            pagination = new pagination() { currentPage = "0", recordPerPage = "0", totalCount = "0" };         
        }
        public dateRange dateRange { get; set; }
       
        public pagination pagination { get; set; }
        public T records { get; set; }        
    }  
}
