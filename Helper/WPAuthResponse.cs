using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class WPAuthResponse
    {
        public string status { get; set; }
        public string error { get; set; }
        public string cookie { get; set; }
        public string cookie_name { get; set; }
        public WPUser user { get; set; }
    }

    public class WPUser 
    {
        public int id { get; set; }
        public string username { get; set; }
        public string nicename { get; set; }
        public string email { get; set; }
        public string url { get; set; }
        public string registered { get; set; }
        public string displayname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string nickname { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string avatar { get; set; }
    }
}
