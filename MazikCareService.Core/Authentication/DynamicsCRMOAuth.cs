using Helper;
using MazikCareService.Core.Models;
using MazikCareService.CRMRepository;
using MazikLogger;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MazikCareService.Core
{
    public class DynamicsCRMOAuth
    {
        public static async Task<LoginModelOutput> AuthenticateUser(bool validateUser, string username, string password)
        {
            LoginModelOutput output = new LoginModelOutput();

            try
            {
                string crmUrl = AppSettings.GetByKey("CRMAPI");
                string clientId = AppSettings.GetByKey("CRMClientID");
                string clientSecret = AppSettings.GetByKey("CRMClientSecret");

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                AuthenticationParameters ap = AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(crmUrl)).Result;

                String authorityUrl = ap.Authority;
                String resource = ap.Resource;


                AuthenticationResult result = null;

                AuthenticationContext authContext = new AuthenticationContext(authorityUrl, false);

                if (validateUser)
                {
                    UserCredential userCredential = new UserPasswordCredential(username, password);
                    result = authContext.AcquireTokenAsync(resource, clientId, userCredential).Result;
                }
                else
                {
                    ClientCredential credential = new ClientCredential(clientId, clientSecret);
                    result = authContext.AcquireTokenAsync(resource, credential).Result;
                }

                if (result != null)
                {
                    output.token = result.AccessToken;
                    HttpContext.Current.Items["token"] = result.AccessToken;

                    if (validateUser)
                    {
                        SoapEntityRepository repo = SoapEntityRepository.GetService();

                        output.userId = repo.GetUserId();
                    }

                    output.success = true;
                }
                else
                {
                    output.success = false;

                    if (validateUser)
                    {
                        throw new AuthenticationException("Username/Password is incorrect");
                    }
                    else
                    {
                        throw new AuthenticationException("Application user is not setup");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return output;
        }
    }
}
