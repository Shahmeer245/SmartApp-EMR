using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.CRMRepository
{
    public class Repository
    {
        public virtual string CRMAPI
        {
            get
            {
                return Helper.AppSettings.GetByKey("CRMAPI");
            }
        }

        public virtual string UserName
        {
            get
            {
                return Helper.AppSettings.GetByKey("USERNAME");
            }
        }

        public virtual string Password
        {
            get
            {
                return Helper.AppSettings.GetByKey("PASSWORD");
            }
        }

        public virtual string Domain
        {
            get
            {
                return Helper.AppSettings.GetByKey("DOMAIN");
            }
        }

        public virtual string CRMService
        {
            get
            {
                return Helper.AppSettings.GetByKey("CRMService");
            }
        }

   

        protected List<T> CollectionFromResponseSet<T>(T[] ResponseSet)
        {
            List<T> list = new List<T>(ResponseSet.Count());
            foreach (T item in ResponseSet)
            {
                list.Add(item);
            }

            return list;
        }

        public static string ReplaceEscapeChars(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace("'", "''");
            }
            return str;
        }
    }

    public class ODataResponse<T>
    {
        public T[] Value { get; set; }
    }

    public class OData<T>
    {
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }
        public List<T> Value { get; set; }
    }

}
