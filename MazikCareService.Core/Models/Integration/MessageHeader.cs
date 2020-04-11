using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MazikCareService.Core.Models.Integration
{
    public enum EnumTriggerEvent
    {
        CreatePatient = 1
    }

    public class MessageHeader
    {
        public Guid Id { get; set; }
        public float HL7Version { get; set; }
        public string MessageName { get; set; }
        public Dictionary<string, string> fieldValues { get; set; }
        public int TriggerEvent { get; set; }

        public MessageConnection MessageConnection { get; set; }

        public List<MessageHeaderQueries> Queries { get; set; }

        public async Task<ExpandoObject> getMessage(EnumTriggerEvent TriggerEvent, Dictionary<string, string> fieldValue)
        {
            //MessageHeader messageHeader = new MessageHeader();
            ExpandoObject expandoObject = new ExpandoObject();

            using (var client = new HttpClient(new HttpClientHandler()))
            {
                client.BaseAddress = new Uri("http://localhost:52516");
                
                //KeyValuePair<string, object>[] keyValuePair = new KeyValuePair<string, object>[2];

                fieldValue.Add("TriggerEvent", TriggerEvent.ToString());

                //keyValuePair[0] = new KeyValuePair<string, object>("TriggerEvent", TriggerEvent.ToString());

                //keyValuePair[1] = new Dictionary<string, object>("fieldValue", fieldValue);

                //var content = new FormUrlEncodedContent(keyValuePair);

                var result = await client.PostAsJsonAsync("/api/Integration/getMessageDetails", fieldValue);// client.PostAsync("/api/Integration/getMessageDetails", content).Result;
                result.EnsureSuccessStatusCode();

                MessageHeader messageHeader = JsonConvert.DeserializeObject<MessageHeader>(result.Content.ReadAsStringAsync().Result);
                
                XmlDocument  xmlDoc = new XmlDocument();
                
                //(1) the xml declaration is recommended, but not mandatory
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = xmlDoc.DocumentElement;
                xmlDoc.InsertBefore(xmlDeclaration, root);

                //(2) string.Empty makes cleaner code
                XmlElement element1 = xmlDoc.CreateElement(string.Empty, messageHeader.MessageName.Replace(" ", "-"), string.Empty);
                element1.SetAttribute("xmlns", "http://schemas.datacontract.org/2004/07/MazikCareService.Core.Models.HL7");
                xmlDoc.AppendChild(element1);

                /*XmlElement element2 = doc.CreateElement(string.Empty, "level1", string.Empty);
                element1.AppendChild(element2);

                XmlElement element3 = doc.CreateElement(string.Empty, "level2", string.Empty);
                XmlText text1 = doc.CreateTextNode("text");
                element3.AppendChild(text1);
                element2.AppendChild(element3);

                XmlElement element4 = doc.CreateElement(string.Empty, "level2", string.Empty);
                XmlText text2 = doc.CreateTextNode("other text");
                element4.AppendChild(text2);
                element2.AppendChild(element4);

                doc.Save("D:\\document.xml");*/

                foreach (var item in messageHeader.Queries)
                {                    
                    XmlElement element2 = xmlDoc.CreateElement(string.Empty, item.queryName.Replace(" ", "-"), string.Empty);
                    element1.AppendChild(element2);

                    JObject jobject = new JObject();
                    JArray jArray = new JArray();

                    if (item.messageData.GetType().Name == "JArray")
                    {
                        jArray = item.messageData;
                        foreach(JObject jObject in item.messageData)
                        {
                            XmlDocument doc = JsonConvert.DeserializeXmlNode(jObject.ToString(), messageHeader.MessageName.Replace(" ", ""));
                            //XmlDocument doc = JsonConvert.DeserializeXmlNode("{\"Row\":" + item.messageData.ToString() + "}", messageHeader.MessageName.Replace(" ", ""));

                            foreach (XmlNode xmlNodes in doc.ChildNodes[0].ChildNodes)
                            {
                                XmlElement element3 = xmlDoc.CreateElement(string.Empty, xmlNodes.Name, string.Empty);
                                XmlText text1 = xmlDoc.CreateTextNode(xmlNodes.InnerText);
                                element3.AppendChild(text1);
                                element2.AppendChild(element3);
                            }
                        }

                    }
                    else
                    {
                        jobject = item.messageData;
                        XmlDocument doc = JsonConvert.DeserializeXmlNode(item.messageData.ToString(), messageHeader.MessageName.Replace(" ", ""));
                        //XmlDocument doc = JsonConvert.DeserializeXmlNode("{\"Row\":" + item.messageData.ToString() + "}", messageHeader.MessageName.Replace(" ", ""));

                        foreach (XmlNode xmlNodes in doc.ChildNodes[0].ChildNodes)
                        {
                            XmlElement element3 = xmlDoc.CreateElement(string.Empty, xmlNodes.Name, string.Empty);
                            XmlText text1 = xmlDoc.CreateTextNode(xmlNodes.InnerText);
                            element3.AppendChild(text1);
                            element2.AppendChild(element3);
                        }
                    }

                                       
                }

                

                string path = messageHeader.MessageConnection.ConnectionDetails;

                if (!File.Exists(path))
                {
                    File.WriteAllText(path + messageHeader.Id + ".xml", xmlDoc.InnerXml);
                }
                
            }

            return expandoObject;

        }

        public static async Task<ExpandoObject> getMessage(int TriggerEventValue, Dictionary<string, string> list)
        {
            ExpandoObject expandoObject = new ExpandoObject();

            MessageHeader messageHeader = new MessageHeader();

            expandoObject = await messageHeader.getMessage(EnumTriggerEvent.CreatePatient, list);

            return expandoObject;
        }

        public async Task<ExpandoObject> receiveMessage(MessageHeader messageHeader)
        {
            //MessageHeader messageHeader = new MessageHeader();
            ExpandoObject expandoObject = new ExpandoObject();

            using (var client = new HttpClient(new HttpClientHandler()))
            {
                client.BaseAddress = new Uri("http://localhost:52516");

                //KeyValuePair<string, object>[] keyValuePair = new KeyValuePair<string, object>[2];

                //fieldValue.Add("messsageHeader", TriggerEvent.ToString());

                //keyValuePair[0] = new KeyValuePair<string, object>("TriggerEvent", TriggerEvent.ToString());

                //keyValuePair[1] = new Dictionary<string, object>("fieldValue", fieldValue);

                //var content = new FormUrlEncodedContent(keyValuePair);

                var result = await client.PostAsJsonAsync("/api/Integration/getMessageDetails", messageHeader);// client.PostAsync("/api/Integration/getMessageDetails", content).Result;
                result.EnsureSuccessStatusCode();

                messageHeader = JsonConvert.DeserializeObject<MessageHeader>(result.Content.ReadAsStringAsync().Result);

                XmlDocument xmlDoc = new XmlDocument();

                //(1) the xml declaration is recommended, but not mandatory
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = xmlDoc.DocumentElement;
                xmlDoc.InsertBefore(xmlDeclaration, root);

                //(2) string.Empty makes cleaner code
                XmlElement element1 = xmlDoc.CreateElement(string.Empty, messageHeader.MessageName.Replace(" ", "-"), string.Empty);
                element1.SetAttribute("xmlns", "http://schemas.datacontract.org/2004/07/MazikCareService.Core.Models.HL7");
                xmlDoc.AppendChild(element1);

                /*XmlElement element2 = doc.CreateElement(string.Empty, "level1", string.Empty);
                element1.AppendChild(element2);

                XmlElement element3 = doc.CreateElement(string.Empty, "level2", string.Empty);
                XmlText text1 = doc.CreateTextNode("text");
                element3.AppendChild(text1);
                element2.AppendChild(element3);

                XmlElement element4 = doc.CreateElement(string.Empty, "level2", string.Empty);
                XmlText text2 = doc.CreateTextNode("other text");
                element4.AppendChild(text2);
                element2.AppendChild(element4);

                doc.Save("D:\\document.xml");*/

                foreach (var item in messageHeader.Queries)
                {
                    XmlElement element2 = xmlDoc.CreateElement(string.Empty, item.queryName.Replace(" ", "-"), string.Empty);
                    element1.AppendChild(element2);

                    JObject jobject = new JObject();
                    JArray jArray = new JArray();

                    if (item.messageData.GetType().Name == "JArray")
                    {
                        jArray = item.messageData;
                        foreach (JObject jObject in item.messageData)
                        {
                            XmlDocument doc = JsonConvert.DeserializeXmlNode(jObject.ToString(), messageHeader.MessageName.Replace(" ", ""));
                            //XmlDocument doc = JsonConvert.DeserializeXmlNode("{\"Row\":" + item.messageData.ToString() + "}", messageHeader.MessageName.Replace(" ", ""));

                            foreach (XmlNode xmlNodes in doc.ChildNodes[0].ChildNodes)
                            {
                                XmlElement element3 = xmlDoc.CreateElement(string.Empty, xmlNodes.Name, string.Empty);
                                XmlText text1 = xmlDoc.CreateTextNode(xmlNodes.InnerText);
                                element3.AppendChild(text1);
                                element2.AppendChild(element3);
                            }
                        }

                    }
                    else
                    {
                        jobject = item.messageData;
                        XmlDocument doc = JsonConvert.DeserializeXmlNode(item.messageData.ToString(), messageHeader.MessageName.Replace(" ", ""));
                        //XmlDocument doc = JsonConvert.DeserializeXmlNode("{\"Row\":" + item.messageData.ToString() + "}", messageHeader.MessageName.Replace(" ", ""));

                        foreach (XmlNode xmlNodes in doc.ChildNodes[0].ChildNodes)
                        {
                            XmlElement element3 = xmlDoc.CreateElement(string.Empty, xmlNodes.Name, string.Empty);
                            XmlText text1 = xmlDoc.CreateTextNode(xmlNodes.InnerText);
                            element3.AppendChild(text1);
                            element2.AppendChild(element3);
                        }
                    }


                }



                string path = messageHeader.MessageConnection.ConnectionDetails;

                if (!File.Exists(path))
                {
                    File.WriteAllText(path + messageHeader.Id + ".xml", xmlDoc.InnerXml);
                }

            }

            return expandoObject;

        }
    }
}
