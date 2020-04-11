using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;


namespace Helper
{
    public class AdAuthentication : IAuthentication
    {
        public static string _path;
        public static string _filterAttribute;

        public TokenClass AuthenticateUser(string domain, string username, string pwd)
        {
            string token = IsAuthenticated(domain, Common.getUser(username), pwd);
            if (string.IsNullOrEmpty(token))
                return null;
            return new TokenClass()
            {
                token = token,
                username = Common.getUser(username),
                password = pwd,
                domain = domain
            };
        }

        public static string GetGroups()
        {
            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(cn=" + _filterAttribute + ")";
            search.PropertiesToLoad.Add("memberOf");
            StringBuilder groupNames = new StringBuilder();

            try
            {
                SearchResult result = search.FindOne();
                int propertyCount = result.Properties["memberOf"].Count;
                string dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    dn = (string)result.Properties["memberOf"][propertyCounter];
                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }
                    groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                    groupNames.Append("|");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }
            return groupNames.ToString();
        }





        public static string IsAuthenticated(string domain, string username, string pwd)
        {
            bool isValid;
           
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
            {
                // validate the credentials
                    isValid = pc.ValidateCredentials(username, pwd);
            }
            
            try
            {
                if (isValid == true)
                {
                string domainAndUsername = domain + @"\" + username;
                DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);

                //Bind to the native AdsObject to force authentication.
                object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    return "";
                }

                //Update the new path to the user in the directory.
                return result.Path;
                }else{
                  return "";
                }
                //_filterAttribute = (string)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user. " + ex.Message);
            }

            //return true;
        }

        public static string verifyToken(string token)
        {
            string path;
            string username;
            string distinguishedName;
            try
            {
                DirectoryEntry usr = new DirectoryEntry(token);
                // Retrieve and write the Path for the object.
                path                 = usr.Path;
                // Retrieve and write the Guid for the object.
                var guid                    = usr.Guid.ToString();
                // Retrieve and write the Name for the object.
                username             = usr.Name;
                // Retrieve and write the DN for the object.
                distinguishedName    = usr.Properties["distinguishedName"].Value.ToString();
           
            }
            catch (Exception ex)
            {
                throw new Exception("Error verifying Token . " + ex.Message);
            }
            return path.ToString();
        }


    }
}
