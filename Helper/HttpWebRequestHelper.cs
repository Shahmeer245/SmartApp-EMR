using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class HttpWebRequestHelper
    {
        public static int ExceptionConsecutiveCount = 0;
        public static int TotalExceptionCount = 0;
        bool useDirectConnection = false;
        bool useProxy = false;
        public HttpWebRequestHelper(bool _useDirectConnection, bool _useProxy)
        {
            useDirectConnection = _useDirectConnection;
            useProxy = _useProxy;
        }

        public HttpWebRequestHelper()
        {

        }


        public Response PostRequest<Response, Request>(string url, Request entity, Dictionary<string, string> parameters, bool isJson = true)
        {
            HttpWebResponse response = MakeRequest(url, "POST", entity, parameters, isJson);
            using (TextReader treader = new StreamReader(response.GetResponseStream()))
            {
                string str = treader.ReadToEnd();
                if (response != null)
                    response.Close();
                return JsonConvert.DeserializeObject<Response>(str);
            }
        }


        public R PostRequest<R>(string url, object entity, Dictionary<string, string> parameters, bool isJson = true)
        {
            HttpWebResponse response = MakeRequest(url, "POST", entity, parameters, isJson);
            using (TextReader treader = new StreamReader(response.GetResponseStream()))
            {
                string str = treader.ReadToEnd();
                if (response != null)
                    response.Close();
                return JsonConvert.DeserializeObject<R>(str);
            }
        }

        public R DeleteRequest<R>(string url, object entity, Dictionary<string, string> parameters, bool isJson = true)
        {
            HttpWebResponse response = MakeRequest(url, "DELETE", entity, parameters, isJson);
            using (TextReader treader = new StreamReader(response.GetResponseStream()))
            {
                string str = treader.ReadToEnd();
                if (response != null)
                    response.Close();
                return JsonConvert.DeserializeObject<R>(str);
            }
        }

        private HttpWebResponse MakeRequest(string url, string method, object request, Dictionary<string, string> parameters, bool isJson = true, byte[] data = null, string mimeType = null)
        {
            if (parameters != null && parameters.Count > 0)
            {
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i == 0)
                    {
                        url += string.Format("?{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        url += string.Format("&{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
            }

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            if (useDirectConnection)
            {
                req.Proxy = new System.Net.WebProxy();
            }
            else if (useProxy)
            {
                // req.Proxy = new System.Net.WebProxy(RegistryOppsLib.Settings.GetProxyValue(RegistryOppsLib.Settings.Connection.HTTPProxyIP), Convert.ToInt32(RegistryOppsLib.Settings.GetProxyValue(RegistryOppsLib.Settings.Connection.HTTPProxyPort)));
                // req.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }
            else
            {
                //   req.Proxy = System.Net.WebRequest.GetSystemWebProxy();
                //   var credentials = System.Net.CredentialCache.DefaultCredentials;
            }
            if (isJson)
            {
                req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                req.Accept = "application/json";
                req.ContentType = "text/json";
            }
            req.Method = method;
            req.Timeout = 60000;
            HttpWebResponse resp = null;
            if (request != null)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(request);
                    byte[] jsonReq = Encoding.UTF8.GetBytes(json);
                    req.GetRequestStream().Write(jsonReq, 0, jsonReq.Length);
                    req.GetRequestStream().Close();
                    req.GetRequestStream().Dispose();
                }
                catch
                {
                    resp = null;
                    throw;
                }
            }
            if (data != null)
            {
                req.ContentLength = data.Length;
                req.ContentType = mimeType;
                Stream stream = (Stream)req.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                stream.Dispose();
            }

            try
            {
                resp = (HttpWebResponse)req.GetResponse();
                ExceptionConsecutiveCount = 0;
            }
            catch (WebException webException)
            {
                ExceptionConsecutiveCount++;
                TotalExceptionCount++;
                if (resp != null)
                    resp.Close();
                throw;
            }
            catch (ProtocolViolationException webException)
            {
                ExceptionConsecutiveCount++;
                TotalExceptionCount++;
                if (resp != null)
                    resp.Close();
                throw;
            }
            catch (Exception exp)
            {
                TotalExceptionCount++;
                if (resp != null)
                    resp.Close();
                resp = null;
            }
            return resp;
        }


        public HttpWebResponse PostRequest(string url, object entity, Dictionary<string, string> parameters, bool isJson = true)
        {
            return MakeRequest(url, "POST", entity, parameters, isJson);
        }

        public HttpWebResponse PostRequest(string url, byte[] data, string mimeType)
        {
            return MakeRequest(url, "POST", null, null, false, data, mimeType);
        }

        public HttpWebResponse PutRequest(string url, object entity, Dictionary<string, string> parameters, bool isJson = true)
        {
            return MakeRequest(url, "PUT", entity, parameters, isJson);
        }

        public HttpWebResponse GetRequest(string url, Dictionary<string, string> parameters, bool isJson = true)
        {
            return MakeRequest(url, "GET", null, parameters, isJson);
        }

        public R GetRequest<R>(string url, Dictionary<string, string> parameters, bool isJson = true)
        {
            HttpWebResponse response = MakeRequest(url, "GET", null, parameters, isJson);
            using (TextReader treader = new StreamReader(response.GetResponseStream()))
            {
                string str = treader.ReadToEnd();
                if (response != null)
                    response.Close();
                return JsonConvert.DeserializeObject<R>(str);
            }
        }

        public HttpStatusCode HttpUploadFile(string url, string file, byte[] data, string paramName, string contentType, NameValueCollection nvc)
        {
            string boundary = "---------------------------" + DateTime.UtcNow.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            rs.Write(data, 0, data.Length);

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            HttpStatusCode httpstatus = HttpStatusCode.OK;
            try
            {
                wresp = wr.GetResponse();

                httpstatus = ((HttpWebResponse)wresp).StatusCode;
                //Stream stream2 = wresp.GetResponseStream();
                //StreamReader reader2 = new StreamReader(stream2);
                wresp.Close();
                return httpstatus;
            }
            catch (Exception ex)
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
                throw;
                //return HttpStatusCode.InternalServerError;
            }

        }
    }
}
