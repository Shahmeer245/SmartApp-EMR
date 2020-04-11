using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class Common
    {
        public static string getDomain(string user)
        {

            string[] splitForDomainAndUserName = user.Split('@');

            if (splitForDomainAndUserName.Length > 1)
                return splitForDomainAndUserName[1].Split('.')[0];

            return user;
        }

        public static string getUser(string user)
        {
            string[] splitForDomainAndUserName = user.Split('@');

            if (splitForDomainAndUserName.Length > 0)
                return splitForDomainAndUserName[0];

            return user;
        }
    }
}
