using Helper;
using MazikCareService.CRMRepository;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazikCareService.Core.Models
{
   public class PostComment
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string text { get; set; }
        public string createdby { get; set; }
        public string createdon { get; set; }
        public string image { get; set; }
        public string createdbyId { get; set; }

        public async Task<PostComment> createPostComment(PostComment comments)
        {
            

            //SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            //SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            //SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            // xrm.mzk_patientorder patientOrderEntity = new xrm.mzk_patientorder();
            //  xrm.Post postentity = new xrm.Post();
            try
            {
               


                xrm.PostComment comment = new xrm.PostComment();
                comment.PostId = new EntityReference("post", Guid.Parse(comments.PostId));   // Guid.Parse("7e73dfdc-800d-e711-942d-180373b13231")
                comment.Text = comments.text;


                //postentity.Attributes["RegardingObjectId"] = new EntityReference( "Account",Guid.Parse("a5d39fa8-d507-e611-93fd-180373b13231"));
                //postentity.Attributes["text"] = post.text;
                //postentity.Attributes["source"] = new OptionSetValue((int)PostSource.ManualPost);
                Id = Convert.ToString(entityRepository.CreateEntity(comment));
                if (!string.IsNullOrEmpty(Id))
                {
                    comments.Id = Id;
                }
            }




            catch (Exception ex)
            {
                throw ex;
            }

            return comments;

        }

        public async Task<PostComment> getPostComment(string id)
        {


            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();

            Entity entity = entityRepository.GetEntity("postcomment", new Guid(id), new ColumnSet(true));

            PostComment filledPost = entityToPostComment(entity);

            return filledPost;
        }

        public async Task<List<PostComment>> GetAllPostComments(string PostId)
        {

            List<PostComment> comments = new List<PostComment>();

            QueryExpression query = new QueryExpression("postcomment");
            query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

            SoapCredential.UserName = AppSettings.GetByKey("USERNAME");
            SoapCredential.Password = AppSettings.GetByKey("PASSWORD");
            SoapCredential.Domain = AppSettings.GetByKey("DOMAIN");

            SoapEntityRepository entityRepository = SoapEntityRepository.GetService();
            if (!string.IsNullOrEmpty(PostId))
            {
                query.Criteria.AddCondition("postid", ConditionOperator.Equal, Guid.Parse(PostId));
            }

            EntityCollection entitycollection = entityRepository.GetEntityCollection(query);

            foreach (Entity entity in entitycollection.Entities)
            {
                PostComment comment = entityToPostComment(entity);

                if (comment.text.Contains("@["))
                {
                    string text = "";
                    char[] delimiters = new char[] { '@' };

                    string[] mentions = comment.text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string item in mentions)
                    {
                        string mention = item; ;

                        if (item.Contains("[") && item.Contains("]"))
                        {
                            char[] mentiondelimiters = new char[] { '[', ',', ']' };
                            string[] mentionelemetns = item.Split(mentiondelimiters, StringSplitOptions.RemoveEmptyEntries);

                            mention = "<b>" + mentionelemetns[2].ToString().Replace("\"", "") + "</b>" + mentionelemetns[3];
                        }

                        text += mention;

                    }

                    comment.text = text;

                }

                comment.createdby = ((EntityReference)(entity.Attributes["createdby"] )).Name.ToString();
                comment.createdon = entity.Attributes["createdon"].ToString();
                comment.PostId = entity.Attributes["postid"].ToString();


                comments.Add(comment);
            }
            comments = comments.OrderByDescending(a => a.createdon).ToList<PostComment>();
            return comments;
        }

        public PostComment entityToPostComment(Entity entity)
        {
            PostComment postComment = new PostComment();
            postComment.Id = entity.Id.ToString();
            postComment.text = entity.Attributes["text"].ToString();
            return postComment;
        }


    }
}
