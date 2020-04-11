using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Helper
{
    public class WPAuthentication : IAuthentication
    {
        public TokenClass AuthenticateUser(string domain, string username, string pwd)
        {            
            WPAuthResponse wpAuth = AuthUser(GetNonceToken(), username, pwd);

            if (wpAuth.status.ToLower().Trim().Equals("ok"))
            {
                return new TokenClass()
                {
                    token = wpAuth.user.username,
                    username = wpAuth.user.username,
                    password = pwd,
                    domain = ConfigurationManager.AppSettings["axCustomerDomain"].ToString()
                };
            }
            else
            {
                return new TokenClass()
                {
                    token = string.Empty
                };
            }
        }

        private string GetNonceToken()
        {
            string endPoint = ConfigurationManager.AppSettings["wpAPIUrl"].ToString() + @"get_nonce/";
            var client = new RestClient(endPoint);
            var json = client.MakeRequest("?controller=auth&method=generate_auth_cookie");
            
            NonceAPI obj = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<NonceAPI>(json);

            return obj.nonce;
        }

        private WPAuthResponse AuthUser(string nonce, string username, string pwd)
        {
            string endPoint = ConfigurationManager.AppSettings["wpAPIUrl"].ToString() + @"auth/generate_auth_cookie/";
            var client = new RestClient(endPoint);
            var json = client.MakeRequest(string.Format("?nonce={0}&username={1}&password={2}", nonce, username, pwd));

            WPAuthResponse obj = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<WPAuthResponse>(json);
            
            return obj;
        }
    }
}
