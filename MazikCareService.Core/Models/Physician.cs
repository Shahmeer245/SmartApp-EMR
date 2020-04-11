using Helper;
using MazikCareService.AXRepository;
using MazikCareService.AXRepository.AXServices;
using MazikCareService.Core.Interfaces;
using MazikCareService.CRMRepository;
using MazikCareService.CRMRepository.Microsoft.Dynamics.CRM;
using MazikLogger;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using xrm;


namespace MazikCareService.Core.Models
{
    public class Physician
    {
        public string physicianId { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }
        public string location { get; set; }
        public string NPI { get; set; }
        public string middleInitial { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string phoneType { get; set; }
        public bool sms { get; set; }

        public List<PhysicianType> physicianTypes { get; set; }

        public async Task<Physician> GetPhysician(string practiceId)
        {
            //List<Physician> Physicians = new List<Physician>();
            Physician model = new Physician();
            QueryExpression query = new QueryExpression(xrm.BookableResource.EntityLogicalName);
            query.Criteria.AddCondition("bookableresourceid", ConditionOperator.Equal, new Guid(practiceId));
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("bookableresourceid", "name", "mzk_npinumber", "msdyn_primaryemail", "mzk_phone");
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            foreach (Entity entity in entitycollection.Entities)
            {
                //Physician model = new Physician();


                if (entity.Attributes.Contains("bookableresourceid"))
                    model.physicianId = entity["bookableresourceid"].ToString();
                if (entity.Attributes.Contains("name"))
                {
                    string[] splitname = (entity["name"].ToString()).Split(new char[0]);
                    if (splitname.Length >= 3)
                    {
                        for (int i = 3; i < splitname.Length; i++)
                        {
                            splitname[2] += " " + splitname[i];
                        }
                        char[] middleinitial = splitname[1].ToCharArray();
                        model.firstName = splitname[0];
                        model.middleInitial = middleinitial[0].ToString();
                        model.lastName = splitname[2];
                    }
                    else if (splitname.Length == 2)
                    {
                        model.firstName = splitname[0];
                        model.lastName = splitname[1];
                    }
                    else
                    {
                        model.firstName = entity["name"].ToString();
                        model.lastName = entity["name"].ToString();
                    }
                }

                if (entity.Attributes.Contains("mzk_npinumber"))
                    model.NPI = entity["mzk_npinumber"].ToString();
                if (entity.Attributes.Contains("msdyn_primaryemail"))
                    model.email = entity["msdyn_primaryemail"].ToString();
                if (entity.Attributes.Contains("mzk_phone"))
                    model.phone = entity["mzk_phone"].ToString();

                // Physicians.Add(model);
            }


            return model;

            /* Physician p = new Physician();
             // p = await getPhysician(Phy_uid);
             List<PhysicianType> pTypes = new List<PhysicianType>();
             PhysicianType pType = new PhysicianType();
             pTypes.Add(pType);
             p.physicianTypes = pTypes;
             return p;*/
        }

     /*   public async Task<List<DiagnosticCode>> getDiagnosticCodes(string diagnosticCodeId, string diagnosticCodeName, string practiceId)
        {
            List<DiagnosticCode> DCode = new List<DiagnosticCode>();
            QueryExpression query = new QueryExpression(xrm.mzk_concept.EntityLogicalName);
            FilterExpression filter = query.Criteria.AddFilter(LogicalOperator.And);
            if (!string.IsNullOrEmpty(diagnosticCodeId))
            {
                filter.AddCondition("mzk_conceptid", ConditionOperator.Equal, diagnosticCodeId);
            }
            if (!string.IsNullOrEmpty(diagnosticCodeName))
            {
                filter.AddCondition("mzk_conceptname", ConditionOperator.Equal, diagnosticCodeName);
            }
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("mzk_conceptid", "mzk_conceptname");
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                DiagnosticCode code = new DiagnosticCode();
                if (entity.Attributes.Contains("mzk_conceptid"))
                {
                    code.codeId = entity["mzk_conceptid"].ToString();
                }
                if (entity.Attributes.Contains("mzk_conceptname"))
                {
                    code.codeName = entity["mzk_conceptname"].ToString();
                }
                DCode.Add(code);
            }

            return DCode;

        }*/
        public async Task<List<Physician>> getPhysicians(string practiceId)
        {
            List<Physician> Physicians = new List<Physician>();
            //Physician model = new Physician();
            //Physician model = new Physician();
            QueryExpression query = new QueryExpression(xrm.mzk_clinicphysician.EntityLogicalName);
            query.Criteria.AddCondition("mzk_clinic", ConditionOperator.Equal, new Guid(practiceId));
            //query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(false);
            LinkEntity link1 = new LinkEntity(xrm.mzk_clinicphysician.EntityLogicalName, xrm.BookableResource.EntityLogicalName, "mzk_physician", "bookableresourceid", JoinOperator.Inner)
            {
                Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet("bookableresourceid", "name", "mzk_npinumber", "msdyn_primaryemail", "mzk_phone"),
                EntityAlias = "ContactBook",
            };
            query.LinkEntities.Add(link1);
            /* LinkEntity Link1 = new LinkEntity(xrm.Account.EntityLogicalName, xrm.mzk_clinicphysician.EntityLogicalName, "territoryid", "mzk_clinicphysicianid", JoinOperator.Inner)
             {

             };*/

            //query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("bookableresourceid", "name", "mzk_npinumber", "msdyn_primaryemail", "mzk_phone");
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);
            foreach (Entity entity in entitycollection.Entities)
            {
                Physician model = new Physician();
                if (entity.Attributes.Contains("ContactBook.bookableresourceid"))
                    model.physicianId = ((AliasedValue)entity["ContactBook.bookableresourceid"]).Value.ToString();
                if (entity.Attributes.Contains("ContactBook.name"))
                {
                    string[] splitname = (((AliasedValue)entity["ContactBook.name"]).Value.ToString()).Split(new char[0]);
                    if (splitname.Length >= 3)
                    {
                        for (int i = 3; i < splitname.Length; i++)
                        {
                            splitname[2] += " " + splitname[i];
                        }
                        char[] middleinitial = splitname[1].ToCharArray();
                        model.firstName = splitname[0];//((AliasedValue)entity["ContactBook.name"]).Value.ToString(); //
                        model.middleInitial = middleinitial[0].ToString();//entity["name"].ToString();
                        model.lastName = splitname[2];
                    }
                    else if (splitname.Length == 2)
                    {
                        model.firstName = splitname[0];
                        model.lastName = splitname[1];
                    }
                    else
                    {
                        model.firstName = ((AliasedValue)entity["ContactBook.name"]).Value.ToString();
                    }

                }

                if (entity.Attributes.Contains("ContactBook.mzk_npinumber"))
                    model.NPI = ((AliasedValue)entity["ContactBook.mzk_npinumber"]).Value.ToString();
                if (entity.Attributes.Contains("ContactBook.msdyn_primaryemail"))
                    model.email = ((AliasedValue)entity["ContactBook.msdyn_primaryemail"]).Value.ToString();
                if (entity.Attributes.Contains("ContactBook.mzk_phone"))
                    model.phone = ((AliasedValue)entity["ContactBook.mzk_phone"]).Value.ToString();

                Physicians.Add(model);
            }


            return Physicians;
            /*List<Physician> Physicians = new List<Physician>();
            Physician physician = new Physician();
            List<PhysicianType> pTypes = new List<PhysicianType>();
            PhysicianType pType = new PhysicianType();
            pTypes.Add(pType);
            physician.physicianTypes = pTypes;
            Physicians.Add(physician);
            return Physicians;*/
        }
        // public enum choice { Add,Modify,Delete}
        public enum CChoice
        {
            Add,
            Modify,
            Delete
        }
        public async Task<bool> postPhysicianToPractice(string practiceId, string Choice, Physician physician,string userCreatedBy,string userModifiedBy)
        {
            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            if (Choice.Equals("add"))
            {
                //WarnType warning = new WarnType();
                //warning = await verifyNPI(physician.NPI);
                //bool result = await verifyNPI(physician.NPI);
                Entity contact = new Entity("contact");
                contact["mzk_usercreatedby"] = userCreatedBy;
                contact["mzk_usermodifiedby"] = userCreatedBy;
                if (!string.IsNullOrEmpty(physician.firstName))
                {
                    contact["firstname"] = physician.firstName;
                }
                if (!string.IsNullOrEmpty(physician.lastName))
                {
                    contact["lastname"] = physician.lastName;
                }
                if ((!string.IsNullOrEmpty(physician.firstName)) && (!string.IsNullOrEmpty(physician.lastName)))
                {
                    contact["fullname"] = physician.firstName + " " + physician.lastName;
                }
                Guid contactguid = new Guid();
                contactguid = entityRepository.CreateEntity(contact);

                //EntityReference link = new EntityReference();
                //link.Id = contactguid;

                Entity physician_new = new Entity("bookableresource");
                // physician_new.FormattedValues["resourcetype"] = "Contact";
                physician_new["mzk_usercreatedby"] = userCreatedBy;
                physician_new["mzk_usermodifiedby"] = userCreatedBy;
                physician_new["resourcetype"] = new OptionSetValue(2);
                physician_new["contactid"] = new EntityReference("contact",contactguid);
                //if(!.string.IsNullOrEmpty(physician.))
                if (!string.IsNullOrEmpty(physician.firstName))
                {
                    physician_new["name"] = physician.firstName;
                    if (!string.IsNullOrEmpty(physician.middleInitial))
                    {
                        physician_new["name"] += physician.middleInitial;
                        if (!string.IsNullOrEmpty(physician.lastName))
                        {
                            physician_new["name"] += physician.lastName;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(physician.middleInitial))
                {
                    physician_new["name"] = physician.middleInitial;
                    if (!string.IsNullOrEmpty(physician.lastName))
                    {
                        physician_new["name"] += physician.lastName;
                    }
                }
                else if (!string.IsNullOrEmpty(physician.lastName))
                {
                    physician_new["name"] = physician.lastName;
                }
                //physician_new["name"] = physician.firstName + " " + physician.middleInitial + " " + physician.lastName;
                if (!string.IsNullOrEmpty(physician.NPI))
                {
                   WarnType warning = new WarnType();
                  warning = await verifyNPI(physician.NPI);
                   if (warning.verified.Equals(false))
                    {
                        HttpContext.Current.Items["warningmessage"] = warning.message;
                    }
                    else
                    {
                       // HttpContext.Current.Items["warningmessage"] = warning.message;
                    }
                    physician_new["mzk_npinumber"] = physician.NPI;
                }
                if(!string.IsNullOrEmpty(physician.email))
                {
                    physician_new["msdyn_primaryemail"] = physician.email;
                }
                if (!string.IsNullOrEmpty(physician.phone))
                {
                    physician_new["mzk_phone"] = physician.phone;
                }
                
                physician_new["timezone"]=25;
  
                Guid id_phy = new Guid();
                id_phy = entityRepository.CreateEntity(physician_new);
                //  EntityReference lookup = new EntityReference();
                // entityRepository.CreateEntity(physician_new); //= new EntityReference("bookableresourceid", id_phy);
                //id_phy = (EntityReference)entityRepository.CreateEntity(physician_new);


                /*  QueryExpression query = new QueryExpression("account");
                  query.Criteria.AddCondition("accountid", ConditionOperator.Equal, new Guid(practiceId));
                  //LinkEntity link1 = new LinkEntity("")*/
                  
                Entity relation = new Entity(mzk_clinicphysician.EntityLogicalName);
                relation["mzk_usercreatedby"] = userCreatedBy;
                relation["mzk_usermodifiedby"] = userModifiedBy; 
                relation["mzk_physician"] = new EntityReference("bookableresource",id_phy);
                relation["mzk_clinic"] = new EntityReference("account",new Guid(practiceId));
                entityRepository.CreateEntity(relation);


                //Create Association
                return true;
            }
            else if (Choice.Equals("delete"))
            {
                QueryExpression query = new QueryExpression(mzk_clinicphysician.EntityLogicalName);
                FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                childFilter.AddCondition("mzk_clinic", ConditionOperator.Equal, new Guid(practiceId));
                childFilter.AddCondition("mzk_physician", ConditionOperator.Equal, new Guid(physician.physicianId));

                EntityCollection entity = entityRepository.GetEntityCollection(query);

                Guid guid = entity.Entities[0].Id;

               entityRepository.DeleteEntity(mzk_clinicphysician.EntityLogicalName,guid);
                return true;
            }
            else if (Choice.Equals("modify"))
            {

                QueryExpression queryb = new QueryExpression(BookableResource.EntityLogicalName);
                FilterExpression childFilter = queryb.Criteria.AddFilter(LogicalOperator.And);
                childFilter.AddCondition("bookableresourceid", ConditionOperator.Equal, new Guid(physician.physicianId));
                queryb.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("bookableresourceid","contactid" ,"name", "mzk_npinumber", "msdyn_primaryemail", "mzk_phone");
                EntityCollection entitycollection = entityRepository.GetEntityCollection(queryb);
                foreach (Entity entity in entitycollection.Entities)
                {
                    
                    if (!string.IsNullOrEmpty(physician.firstName))
                    {
                        entity["name"] = physician.firstName;

                    }
                    if (!string.IsNullOrEmpty(physician.middleInitial))
                    {
                        entity["name"] +=" "+physician.middleInitial;
                    }
                    if (!string.IsNullOrEmpty(physician.lastName))
                    {
                        entity["name"] += " "+physician.lastName;
                    }
                    if (!string.IsNullOrEmpty(physician.NPI))
                    {
                        entity["mzk_npinumber"] = physician.NPI;
                    }
                    if (!string.IsNullOrEmpty(physician.email))
                    {
                        entity["msdyn_primaryemail"] = physician.email;
                    }
                    if (!string.IsNullOrEmpty(physician.phone))
                    {
                        entity["mzk_phone"] = physician.phone;
                    }

                   

                    if (entity.Attributes.Contains("contactid"))
                    {

                        EntityReference refer = (EntityReference)entity["contactid"];
                        QueryExpression queryc = new QueryExpression(xrm.Contact.EntityLogicalName);
                        FilterExpression childfilter = queryc.Criteria.AddFilter(LogicalOperator.And);
                        childfilter.AddCondition("contactid", ConditionOperator.Equal, refer.Id);
                        queryc.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("fullname", "address1_composite", "emailaddress1", "telephone2");
                        EntityCollection ecollection = entityRepository.GetEntityCollection(queryc);
                        foreach (Entity c_entity in ecollection.Entities)
                        {
                            if (!string.IsNullOrEmpty(physician.firstName))
                            {
                                c_entity.Attributes["fullname"] = physician.firstName;

                            }
                            if (!string.IsNullOrEmpty(physician.middleInitial))
                            {
                                c_entity.Attributes["fullname"] += " " + physician.middleInitial;
                            }
                            if (!string.IsNullOrEmpty(physician.lastName))
                            {
                                c_entity.Attributes["fullname"] += " " + physician.lastName;
                            }
                            if (!string.IsNullOrEmpty(physician.location))

                            {
                                c_entity.Attributes["address1_composite"] = physician.location;
                            }
                            if (!string.IsNullOrEmpty(physician.email))
                            {
                                c_entity.Attributes["emailaddress1"] = physician.email;
                            }
                            if (!string.IsNullOrEmpty(physician.phone))
                            {
                                c_entity.Attributes["telephone2"] = physician.phone;
                            }
                            try
                            {
                                entityRepository.UpdateEntity(c_entity);
                            }
                            catch(Exception ex)
                            {
                                throw ex;
                            }
                            
                        }
                    }
                    try
                    {
                        entityRepository.UpdateEntity(entity);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }


                }
               
                return true;
            }
            else
            {
                return false;
            }
        }


        /*public async Task<bool> verifyNPI(string NPI)
        {
            string URL = "https://npiregistry.cms.hhs.gov/api/resultsDemo2/";
            string urlParameters = "/?number="+NPI;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                foreach (var d in dataObjects)
                {
                    Console.WriteLine("{0}", d.Data);
                }
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            //Make any other calls using HttpClient here.

            //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();

            return true;

        }*/
        public async Task<WarnType> verifyNPI(string NPI)
        {
            WarnType warning = new WarnType();
            string parameter = "number="+NPI;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://npiregistry.cms.hhs.gov/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //string postBody = JsonConvert.SerializeObject(parameter);
            //HttpContent content = new StringContent(postBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.GetAsync("api/resultsDemo2/?"+parameter).Result;
            if (response.IsSuccessStatusCode)
            {
                // Read the response body as string
                string json = response.Content.ReadAsStringAsync().Result;
                if (json.Contains("result_count"))
                {
                    dynamic js = JsonConvert.DeserializeObject(json);
                    var count = js["result_count"].ToString();
                    if (count.Equals("0"))
                    {
                        warning.verified = false;
                        warning.message = "Unregistered NPI";
                        return warning;
                        //return false;
                    }
                    else if (count.Equals("1"))
                    {
                        warning.verified = true;
                        warning.message = "";
                        return warning;
                    }
                }
                
                else
                {
                    JObject jo = JObject.Parse(json);
                    JToken msg = jo["Errors"].First["description"];
                    //dynamic js = JsonConvert.DeserializeObject(json);
                    //var error = js["Error"]["description"].ToString();
                    warning.message = msg.ToString();
                    warning.verified = false;
                    return warning;
                }   
                   
             }
            return warning;
                //return true;
          

            //return false;
        }
    }

    



    // public class Insurance
    // {
    /* public string insuranceId { get; set; }
     public string insuranceName { get; set; }
     public string phone { get; set; }
     public string fax { get; set; }
     public string address1 { get; set; }
     public string address2 { get; set; }
     public string city { get; set; }
     public string state { get; set; }
     public string zip { get; set; }

     public string insuranceType { get; set; }
     public string insurancePolicyNumber { get; set; }*/
    //  }
    public class DiagnosticCode
    {
        public string codeId { get; set; }
        public string codeName { get; set; }
    }

    public class PhysicianType
    {
        public bool ordering { get; set; }
        public bool? reading { get; set; }
        public bool? referring { get; set; }
    }
    public class WarnType
    {
        public string message { get; set; }
        public bool verified { get; set; }
    }
}
