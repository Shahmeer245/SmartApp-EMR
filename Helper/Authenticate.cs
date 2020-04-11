using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class Authenticate
    {
        //public virtual IAuthenticate GetAuthenticationSource(string user, string password) //, string defaultDomain
        //{
        //    IAuthenticate source = null;


        //    return source;
        //}
        IAuthentication iAuth = null;

        public TokenClass AuthenticateUser(string user, string password, string defaultDomain)
        {

            TokenClass token = new TokenClass();
            string strtoken = string.Empty;
            //Ad Authentication
            string domain = Common.getDomain(user);
            string domainuser = Common.getUser(user);

            Authentication auth = new Authentication();
            this.iAuth = auth.GetAuthenticationSource(user, password);

            token = this.iAuth.AuthenticateUser(domain, user, password);

            //if (domain.ToLower().Trim().Equals(defaultDomain.ToLower().Trim())) //
            //{
            //    strtoken = AdAuthentication.IsAuthenticated(domain, domainuser, password);
            //    token.token = strtoken;
            //    token.username = domainuser;
            //    token.password = password;
            //    token.domain = domain;
            //}
            //else
            //{
            //    token.token = user;
            //    token.username = user;
            //    token.password = password;
            //}

            //if (string.IsNullOrEmpty(strtoken))
            //{
            //    token.token = user;
            //    token.username = user;
            //    token.password = password;
            //}
            //else
            //{
                
            //}

            return token;
        }

        
    }
}
