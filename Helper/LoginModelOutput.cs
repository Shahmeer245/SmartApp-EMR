using System.Collections.Generic;
using System.Linq;

namespace Helper
{
    public class LoginModelOutput
    {
        public string token { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public string resourceRecId { get; set; }
        public bool success { get; set; }

        public List<T> CollectionFromResponseSet<T>(T[] ResponseSet)
        {
            List<T> list = new List<T>(ResponseSet.Count());
            foreach (T item in ResponseSet)
            {
                list.Add(item);
            }

            return list;
        }
    }
}