using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xrm;

namespace MazikCareService.CRMRepository
{
    public class Notes
    {
        public static bool addDocsinCRM(string fileName, string subject, string description, string file, string mimeType, string objectType, Guid objectId)
        {
            try
            {
                bool ret = false;

                SoapEntityRepository repo = SoapEntityRepository.GetService();

                Annotation setupAnnotation = new Annotation()
                {
                    Subject = subject,
                    FileName = fileName,
                    DocumentBody = file,
                    MimeType = mimeType
                };

                setupAnnotation.NoteText = description;
                setupAnnotation.ObjectTypeCode = objectType;
                setupAnnotation.ObjectId = new EntityReference(objectType, objectId);

                Guid notesId = repo.CreateEntity(setupAnnotation);

                if (notesId != Guid.Empty)
                {
                    ret = true;
                }

                return ret;
            }
            catch
            {
                return false;
            }
        }
    }
}
