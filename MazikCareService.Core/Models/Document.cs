using MazikCareService.AXRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.Core.Models
{
    public class Document
    {
        public string name
        {
            get; set;
        }

        public string type
        {
            get; set;
        }

        public string recId
        {
            get; set;
        }

        public string document
        {
            get; set;
        }

        public async Task<string> getDocumentFile(string documentRecId)
        {
            try
            {
                CommonRepository repo = new CommonRepository();

                return repo.getDocumentsBase64(Convert.ToInt64(documentRecId));
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}
