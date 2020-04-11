using Helper;
using MazikCareService.CRMRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MazikCareService.Core.Models
{


    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    //public partial class pi
    //{

    //    private string[] psField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("p", IsNullable = false)]
    //    public string[] ps
    //    {
    //        get
    //        {
    //            return this.psField;
    //        }
    //        set
    //        {
    //            this.psField = value;
    //        }
    //    }
    //}

    public class Post
    {
        private OrganizationServiceProxy _serviceProxy;
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Createdby { get; set; }
        public string createdon { get; set; }
        public string modifiedon { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public string image { get; set; }
        public string source { get; set; }
        public string subject { get; set; }


        public int commentscount { get; set; }
        public List<PostComment> comments { get; set; }



        public async Task<Post> createPost(Post post)
        {


            //SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            //SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            //SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            // xrm.mzk_patientorder patientOrderEntity = new xrm.mzk_patientorder();
            //  xrm.Post postentity = new xrm.Post();
            try
            {
                //var whoAmIRequest = new WhoAmIRequest();
                //var whoAmIResponse = (WhoAmIResponse)entityRepository..Execute(whoAmIRequest);
                //var currentUserRef = new EntityReference(
                //    SystemUser.EntityLogicalName, whoAmIResponse.UserId);

                // RegardingObjectId

                xrm.Post postentity = new xrm.Post()
                {
                    Text = post.text,
                    Source = new OptionSetValue((int)PostSource.ManualPost),
                    RegardingObjectId = new EntityReference("systemuser", Guid.Parse(post.UserId))
                };


                Id = Convert.ToString(entityRepository.CreateEntity(postentity));
                if (!string.IsNullOrEmpty(Id))
                {
                    post.Id = Id;
                }

            }




            catch (Exception ex)
            {
                throw ex;
            }

            return post;

        }


        public static async Task<Post> getPost(string id)
        {


            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            Entity entity = entityRepository.GetEntity("post", new Guid(id), new ColumnSet(true));

            Post filledPost = entityToPost(entity);

            return filledPost;
        }

        public async Task<List<Post>> getPosts(int currentpage, string Type = null)
        {
            List<Post> posts = new List<Post>();
            #region user Posts
            QueryExpression query = new QueryExpression("post");
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            #endregion


            //SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            //SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            //SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            //LinkEntity EntityPostcomment = new LinkEntity("post", "postcomment", "postid", "postid", JoinOperator.LeftOuter);

            //EntityPostcomment.Columns = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            //EntityPostcomment.EntityAlias = "postcomment";
            //query.LinkEntities.Add(EntityPostcomment);
            //if (!string.IsNullOrEmpty(Type))
            //{
            //    query.Criteria.AddCondition("source", ConditionOperator.Equal, Type);
            //}
            //EntityCollection entitycollection = entityRepository.GetEntityCollection(query);



            //foreach (xrm.Post post in entitycollection.Entities)
            //{

            //    if (post.Post_Comments!=null)
            //    {
            //        foreach (xrm.PostComment comment in post.Post_Comments)
            //        {


            //        }
            //    }    
            //}


            OptionSetValue source;

            if (!string.IsNullOrEmpty(Type))
                source = new OptionSetValue(Convert.ToInt32(Type));
            else
                source = null;

            var personalWallPageReq = new RetrievePersonalWallRequest
            {
                CommentsPerPost = 10,
                PageNumber = currentpage,
                PageSize = 10,
                Source = source
            };
            //var personalWallPageRes =
            //    (RetrievePersonalWallResponse)_serviceProxy.Execute(personalWallPageReq);

            var personalWallPageRes =
                (RetrievePersonalWallResponse)entityRepository.Execute(personalWallPageReq);



            //ClientCredentials credentials = new ClientCredentials();
            //credentials.Windows.ClientCredential = new System.Net.NetworkCredential(SoapCredential.UserName, SoapCredential.Password, SoapCredential.Domain);
            //Uri organizationUri = new Uri(Helper.AppSettings.GetByKey("CRMService"));


            //using (_serviceProxy = new OrganizationServiceProxy(organizationUri, null, credentials, null))
            //{
            //    // This statement is required to enable early-bound type support.
            //    _serviceProxy.EnableProxyTypes();
            //    //_serviceContext = new ServiceContext(_serviceProxy);

            //    OptionSetValue source;

            //    if (!string.IsNullOrEmpty(Type))
            //        source = new OptionSetValue(Convert.ToInt32(Type));
            //    else
            //        source = null;

            //   var personalWallPageReq = new RetrievePersonalWallRequest
            //    {
            //        CommentsPerPost = 10,
            //        PageNumber = currentpage,
            //        PageSize = 10,
            //        Source=source
            //    };
            //    //var personalWallPageRes =
            //    //    (RetrievePersonalWallResponse)_serviceProxy.Execute(personalWallPageReq);

            //    var personalWallPageRes =
            //        (RetrievePersonalWallResponse)entityRepository.ExecuteTransaction(personalWallPageReq);

            List<string> userID = new List<string>();

            Pagination.hasMoreRecords = personalWallPageRes.EntityCollection.MoreRecords;

            foreach (xrm.Post entity in personalWallPageRes.EntityCollection.Entities)
            {
                Post model = entityToPost(entity);

                AliasedValue commentCountAttr = (AliasedValue)entity.Attributes["commentcount"];

                //xrm.SystemUser mainEntity = (xrm.SystemUser)entityRepository.GetEntity("systemuser", ((EntityReference)entity.Attributes["createdby"]).Id, new ColumnSet("entityimage"));
                //if (mainEntity.EntityImage != null)
                //{
                //    model.image = Convert.ToBase64String(mainEntity.EntityImage);
                //}
                
                int actualCount = (int)commentCountAttr.Value;

                model.text = convertPostMentionsToBold(model.text);

                model.Createdby = ((EntityReference)entity.Attributes["createdby"]).Id.ToString();

                userID.Add(model.Createdby);

                //if (model.text.Contains("@["))
                //    {
                //        string text = "";
                //       char[] delimiters = new char[] {'@'};

                //       string[] mentions = model.text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                //        foreach (string item in mentions)
                //        {
                //            string mention = item; ;

                //            if (item.Contains("[") && item.Contains("]"))
                //            {
                //                char[] mentiondelimiters = new char[] { '[', ',', ']' };
                //                string[] mentionelemetns = item.Split(mentiondelimiters, StringSplitOptions.RemoveEmptyEntries);

                //                mention = "<b>"+mentionelemetns[2].ToString().Replace("\"", "") + "</b>";
                //            }

                //            text += mention;

                //        }

                //        model.text = text;

                //    }

                if (entity.Post_Comments != null)
                {
                    List<PostComment> comments = new List<PostComment>();
                    model.commentscount = actualCount;
                    foreach (xrm.PostComment postcomment in entity.Post_Comments)
                    {
                        postcomment.Text = convertPostMentionsToBold(postcomment.Text);

                        PostComment comment = new PostComment();
                        comment.Id = postcomment.Id.ToString();
                        comment.text = postcomment.Text;
                        comment.createdby = postcomment.CreatedBy.Name;
                        comment.createdbyId = postcomment.CreatedBy.Id.ToString();
                        comment.createdon = postcomment.CreatedOn.Value.ToString();
                        comment.PostId = postcomment.PostId.Id.ToString();
                        //mainEntity = (xrm.SystemUser)entityRepository.GetEntity("systemuser", ((EntityReference)postcomment.Attributes["createdby"]).Id, new ColumnSet("entityimage"));
                        //if(mainEntity.EntityImage != null)
                        //{
                        //    comment.image = Convert.ToBase64String(mainEntity.EntityImage);

                        //}

                        userID.Add(comment.createdbyId);

                        comments.Add(comment);

                    }

                    model.comments = comments;
                }


                posts.Add(model);

                

            }

            List<string> distinctUserIdList = userID.Distinct().ToList();

            Dictionary<string, string> userIdImage = new User().getUsersImages(distinctUserIdList);

            foreach (Post post in posts)
            {
                if (userIdImage.ContainsKey(post.Createdby))
                {
                    post.image = userIdImage[post.Createdby];
                }

                if (post.comments != null)
                {
                    foreach (PostComment postComment in post.comments)
                    {
                        if (userIdImage.ContainsKey(postComment.createdbyId))
                        {
                            postComment.image = userIdImage[postComment.createdbyId];
                        }
                    }
                }
            }

            posts = posts.OrderByDescending(a=> a.createdon).ToList<Post>();
            //}



            // Previous Code
            #region old Code
            //    foreach (Entity entity in entitycollection.Entities)
            //    {
            //        Post model = new Post();
            //        model.Id =entity.Id.ToString(); 
            //        model.Createdby = ((EntityReference)entity.Attributes["createdby"]).Name;
            //        model.type = (entity.Attributes["type"] as OptionSetValue).Value.ToString();
            //        model.createdon = entity.Attributes["createdon"].ToString();
            //        model.modifiedon = entity.Attributes["modifiedon"].ToString();
            //        model.source = (entity.Attributes["source"] as OptionSetValue ).Value.ToString();
            //        model.text = entity.Attributes["text"].ToString();

            //        if (model.source.Equals("2"))
            //        {
            //            model.subject = model.Createdby;
            //            if (model.text.Contains("@"))
            //            {
            //                char[] delimiters = new char[] { '@', '[', ',', ']' };
            //                string[] elements = model.text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            //                model.text = elements[2].ToString().Replace("\"", "") + elements[3].ToString();
            //            }

            //        }


            //        else if (model.source.Equals("1"))
            //        {

            //            if (model.text.Contains("xml"))
            //            {
            //                try
            //                {
            //                    string xml = model.text;
            //                    var serializer = new XmlSerializer(typeof(pi));
            //                    pi result;

            //                    using (TextReader reader = new StringReader(xml))
            //                    {
            //                        result = (pi)serializer.Deserialize(reader);

            //                        if(result.ps.Count()>1)
            //                            for (int i = 1; i < result.ps.Count(); i++)
            //                            {
            //                                model.subject = result.ps[i];
            //                                if (!string.IsNullOrEmpty(model.subject))
            //                                {
            //                                    break;
            //                                }
            //                            }
            //                        else
            //                            model.subject = result.ps[0];


            //                    }
            //                }
            //                catch (Exception ex)
            //                {


            //                }

            //            }


            //            model.text = "Created by " + model.Createdby;

            //        }






            //        if (entity.Attributes.Contains("postcomment.postid"))
            //        {
            //            List<PostComment> comments = new List<PostComment>();

            //            PostComment comment = new PostComment();
            //            comment.Id = entity.Id.ToString();
            //            comment.text = (entity.Attributes["postcomment.text"] as AliasedValue).Value.ToString();

            //            if (comment.text.Contains("@") && comment.text.Contains("[") && comment.text.Contains("]"))
            //            {
            //                char[] delimiters = new char[] { '@', '[', ',', ']' };
            //                string[] elements = comment.text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            //                comment.text =  elements[2].ToString().Replace("\"", "") + elements[3].ToString();
            //            }


            //            comment.createdby = ((EntityReference)(entity.Attributes["postcomment.createdby"] as AliasedValue).Value).Name.ToString();
            //            comment.createdon = (entity.Attributes["postcomment.createdon"] as AliasedValue).Value.ToString();

            //            comments.Add(comment);
            //            model.comments = comments;

            //        }

            //        posts.Add(model);
            //    }

            //    // IEnumerable<IGrouping<int, smth>> groups = list.GroupBy(x => x.ID)
            //    //foreach (IEnumerable<smth> element in groups)
            //    //            {
            //    //                //do something
            //    //            }


            //    posts = posts.OrderBy(a => a.modifiedon).ToList();


            //    var Groupedposts = posts.GroupBy(u => u.Id);
            //    List<Post> groupposts = new List<Post>();
            //    foreach (var item in Groupedposts)
            //    {
            //        Post model = new Post();
            //        model.Id = item.First().Id;
            //        model.text = item.First().text;
            //        model.Createdby = item.First().Createdby;
            //        model.createdon = item.First().createdon;
            //        model.source = item.First().source;
            //        model.type = item.First().type;
            //        model.subject  = item.First().subject;

            //        model.comments = new List<PostComment>();
            //        foreach (var inneritem in item)
            //        {
            //            if (inneritem != null && inneritem.comments!=null)
            //            {
            //                PostComment comment = new PostComment();
            //                comment.Id = inneritem.comments.First().Id;
            //                comment.PostId = inneritem.comments.First().PostId;
            //                comment.createdby = inneritem.comments.First().createdby;
            //                comment.createdon = inneritem.comments.First().createdon;
            //                comment.text = inneritem.comments.First().text;


            //                model.comments.Add(comment);
            //            }
            //        }

            //        if (model.comments.Count > 0)
            //            model.commentscount = model.comments.Count;

            //        groupposts.Add(model);
            //    }

            //    return groupposts;
            //}
            #endregion

            return posts;

        }

        public static Post entityToPost(Entity entity)
        {
            Post post = new Post();
            post.Id = entity.Id.ToString();
            post.Createdby = ((EntityReference)entity.Attributes["createdby"]).Name;
            post.type = (entity.Attributes["type"] as OptionSetValue).Value.ToString();
            post.createdon = entity.Attributes["createdon"].ToString();
            post.modifiedon = entity.Attributes["modifiedon"].ToString();
            post.source = (entity.Attributes["source"] as OptionSetValue).Value.ToString();
            post.text = entity.Attributes["text"].ToString();
            post.subject = ((EntityReference)entity.Attributes["regardingobjectid"]).Name;
            return post;
        }

        public static Post retrieveFormattedAutoPost(Post post)
        {

            Post model = new Post();

            #region user Posts
            QueryExpression query = new QueryExpression("post");
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);
            #endregion

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            int postSource = 2;

            var personalWallPageReq = new RetrievePersonalWallRequest
            {
                CommentsPerPost = 0,
                PageNumber = 1,
                PageSize = 10,

            };

            if (Int32.TryParse(post.source, out postSource))
            {
                personalWallPageReq.Source = new OptionSetValue(postSource);
            }


            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            //if (DateTime.TryParse(post.createdon, out startDate))
            //{
            //    personalWallPageReq.StartDate = startDate;
            //}


            //if (DateTime.TryParse(post.createdon, out endDate))
            //{
            //    personalWallPageReq.EndDate = endDate;

            //}



            var personalWallPageRes =
                (RetrievePersonalWallResponse)entityRepository.Execute(personalWallPageReq);

            Entity entity = personalWallPageRes.EntityCollection.Entities.Where(x => x.Id.ToString().Equals(post.Id)).ToList<Entity>().FirstOrDefault<Entity>();

            if (entity.Id != null)
            {
                model = entityToPost(entity);
                AliasedValue commentCountAttr = (AliasedValue)entity.Attributes["commentcount"];

                xrm.SystemUser mainEntity = (xrm.SystemUser)entityRepository.GetEntity("systemuser", ((EntityReference)entity.Attributes["createdby"]).Id, new ColumnSet("entityimage"));
                if(mainEntity.EntityImage != null)
                {
                    model.image = Convert.ToBase64String(mainEntity.EntityImage);
                }
                int actualCount = (int)commentCountAttr.Value;

                model.text = convertPostMentionsToBold(model.text);
            }


            return model;


        }

        public static string convertPostMentionsToBold(string textToConvert)
        {
            string returnedText = textToConvert;

            if (returnedText.Contains("@["))
            {
                string text = "";
                char[] delimiters = new char[] { '@' };

                string[] mentions = returnedText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in mentions)
                {
                    string mention = item; ;

                    if (item.Contains("[") && item.Contains("]"))
                    {
                        char[] mentiondelimiters = new char[] { '[', ',', ']' };
                        string[] mentionelemetns = item.Split(mentiondelimiters, StringSplitOptions.RemoveEmptyEntries);
                        if (mentionelemetns.Length >= 4)
                        {
                            mention = "<b>" + mentionelemetns[2].ToString().Replace("\"", "") + "</b>" + mentionelemetns[3].ToString();
                        }
                        else if (mentionelemetns.Length >= 3)
                        {
                            mention = "<b>" + mentionelemetns[2].ToString().Replace("\"", "") + "</b>";
                        }
                    }

                    text += mention;

                }

                returnedText = text;

            }
            return returnedText;
        }


    }
}
