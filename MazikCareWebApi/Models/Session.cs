using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareWebApi.Models
{
   public class Session
    {
    }

    public class LoginModel
    {
        /// <summary>
        /// UserName to be provided by the User
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// Password to be provided by the User
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// Domain to be provided by the User
        /// </summary>
       
        public string Domain { get; set; }

        public string smsCode { get; set; }

        public long smsCodeId { get; set; }

        public string sysUserId { get; set; }
    }
    //public class LoginModelOutput
    //{
    //    public string token { get; set; }
    //    public string userId { get; set; }
    //    public string userName { get; set; }
    //    public string resourceRecId { get; set; }
    //    public bool success { get; set; }
    //}
}
