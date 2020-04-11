using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Helper
{
    class Authentication
    {
        public virtual IAuthentication GetAuthenticationSource(string user, string password) //, string defaultDomain
        {
            IAuthentication source = null;
            Enum.AuthType authType = Enum.AuthType.SQLServer;

            string defaultdomain = ConfigurationManager.AppSettings["axDomain"].ToString().ToLower().Trim();

            if (Common.getDomain(user).ToLower().Trim().Equals(defaultdomain))
            {
                authType = Enum.AuthType.ActiveDirectory;
            }
            else
                authType = Enum.AuthType.Wordpress;

            switch (authType)
            {
                case Enum.AuthType.ActiveDirectory:
                    source = new AdAuthentication();
                    break;
                case Enum.AuthType.Wordpress:
                    source = new WPAuthentication();
                    break;
                case Enum.AuthType.SQLServer:
                    source = new SQLAuthentication();
                    break;
                default:
                    source = new AdAuthentication();
                    break;
            }

            return source;
        }
    }
}
