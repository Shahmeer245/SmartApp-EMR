using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Helper
{
    [Serializable]
    public class CustomException : Exception
    {
        public CustomException()
            : base()
        {

        }

        public CustomException(string message, string title, ExceptionType type, int status)
            : base()
        {
            this.Title = title;
            this.CustomMessage = message;
            this.Type = type;
            this.Status = status;
        }
          
        public string Title { get; set; }
          
        public string CustomMessage { get; set; }
          
        public ExceptionType Type { get; set; }
          
        public int Status { get; set; }

    }
}