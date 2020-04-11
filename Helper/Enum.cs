using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{    
    public class Enum
    {
        public enum AuthType
        {
            ActiveDirectory = 0,
            Wordpress = 1,
            SQLServer = 2
        }

        public enum DayWeekMthYr { Days = 1, Weeks = 2, Months = 3, Years = 4 };
    }
}
