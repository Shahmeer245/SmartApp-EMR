using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
    public class FTP
    {
        public string user { get; set; }
        public string password { get; set; }
        public string url { get; set; }
        public string subfolder { get; set; }
        public List<FileDetail> FileDetail { get;set;}

        public async Task<string> uploadToFTP(FTP ftp)
        {
            string response = "";
            try
            {
             
                foreach (var item in ftp.FileDetail)
                {
                    string filePath = ftp.url +(ftp.subfolder != null ? ftp.subfolder:"") + item.name;
                FtpWebRequest request =
                    (FtpWebRequest)WebRequest.Create(filePath);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                request.Credentials =
                    new NetworkCredential(ftp.user, ftp.password);
                    request.EnableSsl = true;
                    if (request.EnableSsl)
                    {
                        ServicePointManager.ServerCertificateValidationCallback =
                      (s, certificate, chain, sslPolicyErrors) => true;
                      
                    }
                    request.ContentLength = item.content.Length;
                    using (Stream request_stream = request.GetRequestStream())
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(item.content);
                        request_stream.Write(bytes, 0, item.content.Length);
                        request_stream.Close();
                    }
                }
                response = "File successfully uploaded";
            }
            catch (Exception ex)
            {
                 throw ex;
               // response = ex.Message;
            }

            return response;
        }
        public static bool ValidateServerCertificate(object sender,
       X509Certificate certificate, X509Chain chain,
       SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors ==
               SslPolicyErrors.RemoteCertificateChainErrors)
            {
                return false;
            }
            else if (sslPolicyErrors ==
             SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                System.Security.Policy.Zone z =
                   System.Security.Policy.Zone.CreateFromUrl
                   (((HttpWebRequest)sender).RequestUri.ToString());
                if (z.SecurityZone ==
                   System.Security.SecurityZone.Intranet ||
                   z.SecurityZone ==
                   System.Security.SecurityZone.MyComputer)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
    }
   
    public class FileDetail
    {
        public string name { get; set; }
        public string content { get; set; }

    }
}
