using MazikCareWebApi.ApiHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Routing;
using System.Web.Http.Filters;
using Helper;
using System.Web;
using MazikCareService.Core;
using MazikCareService.Core.Models;
using System.Security.Authentication;

namespace MazikCareWebApi.Filters
{
  
    public class MzkAuthorizeAttribute : ActionFilterAttribute
    {
        public async  override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (Session.ifSessionExists)
            {
                //if (actionContext.ModelState.IsValid)
                //{
                bool result = await MzkAuthorizeAttribute.ValidateToken(HttpContext.Current.Request.Headers["Authorization"].ToString());                

                if (result)
                      base.OnActionExecuting(actionContext);
                  else
                      actionContext.Response = Response.Unauthorized("APP", "Auth Token is Invalid");
                //}
                //else
                //    actionContext.Response = Response.ModelStateInValid(actionContext.ModelState);
             }
            else
                actionContext.Response = Response.Unauthorized("APP", "Auth Token not found.");

        }

        public async static Task<bool> ValidateToken(string authToken)
        {
            try
            {
                LoginModelOutput token = DynamicsCRMOAuth.AuthenticateUser(false, "", "").Result;

                if (token == null || !token.success)
                {                    
                    throw new AuthenticationException("Error while validating token");
                }

                HttpContext.Current.Items["token"] = token.token;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
