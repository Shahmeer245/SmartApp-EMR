using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class OAuthHelper
    {
        /// <summary>
        /// The header to use for OAuth.
        /// </summary>
        public const string OAuthHeader = "Authorization";

        /// <summary>
        /// Retrieves an authentication header from the service.
        /// </summary>
        /// <returns>The authentication header for the Web API call.</returns>
        public static string GetAuthenticationHeader(bool useWebAppAuthentication = false)
        {
            string aadTenant = AppSettings.GetByKey("OperationsActiveDirectoryTenant");
            string aadClientAppId = AppSettings.GetByKey("OperationsActiveDirectoryClientAppId");
            string aadResource = AppSettings.GetByKey("OperationsActiveDirectoryResource");

            AuthenticationContext authenticationContext = new AuthenticationContext(aadTenant);
            AuthenticationResult authenticationResult;

            if (useWebAppAuthentication)
            {
                string aadClientAppSecret = AppSettings.GetByKey("OperationsActiveDirectoryClientAppSecret");
                var creadential = new ClientCredential(aadClientAppId, aadClientAppSecret);
                authenticationResult = authenticationContext.AcquireTokenAsync(aadResource, creadential).Result;
            }
            else
            {
                // OAuth through username and password.
                string username = AppSettings.GetByKey("OperationsUserName");
                string password = AppSettings.GetByKey("OperationsPassword");

                // Get token object
                var userCredential = new UserPasswordCredential(username, password);
                authenticationResult = authenticationContext.AcquireTokenAsync(aadResource, aadClientAppId, userCredential).Result;
            }

            // Create and get JWT token
            return authenticationResult.CreateAuthorizationHeader();
        }
    }
}
