using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Web.Http.Cors;
using MazikCareService.Core.Models;
using MazikCareWebApi.ApiHelpers;
using System.Threading.Tasks;

namespace MazikCareWebApi
{

    [HubName("messaginghub")]
    public class MessagingHub : Hub
    {
        private static Dictionary<string,Dictionary<string,string>> clients = new Dictionary<string, Dictionary<string, string>>();

        [HubMethodName("Hello")]
        public void Hello(string message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MessagingHub>();
            var tuple = Tuple.Create(message, "hi");
            Clients.All.foo(tuple);
        }

        public override Task OnConnected()
        {
            var userId = Context.QueryString["id"];
            var token = Context.QueryString["token"];
            var connectionId = Context.ConnectionId.ToString();

            if (clients.ContainsKey(userId))
            {
                clients[userId]["connectionId"] = connectionId;
                clients[userId]["token"] = token;

            }
            else
            {
                clients.Add(userId, new Dictionary<string, string>());
                clients[userId]["connectionId"] = connectionId;
                clients[userId]["token"] = token;

            }
            return base.OnConnected();
        }

        //public static async void sendPostAfterGettingPostFromId(Post data)
        //{
        //    var hubContext = GlobalHost.ConnectionManager.GetHubContext<MessagingHub>();
        //    //var tuple = Tuple.Create(data);
        //    try
        //    {
        //        ApiResponseModel<Post> model = new ApiResponseModel<Post>() { };
        //        var client = ServiceFactory.GetService(typeof(Post));
        //        data = await client.getPost(data.Id);

        //        model.data.records = data;

        //        hubContext.Clients.All.newPost(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        hubContext.Clients.All.newPost(ex);
        //    }
        //}

        public static async void sendPost(Post data)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MessagingHub>();
            //var tuple = Tuple.Create(data);
            try
            {
                ApiResponseModel<Post> model = new ApiResponseModel<Post>() { };
                
                if(data.source == "2")
                {
                    User userData = new User();
                    var client = ServiceFactory.GetService(typeof(User));
                    userData = await client.getUserEntity(data.Createdby);
                    data.Createdby = userData.Name;
                    data.subject = userData.Name;
                    data.image = userData.image;
                    data.text = Post.convertPostMentionsToBold(data.text);
                    //}
                    model.data.records = data;
                    if (data != null)
                    {
                        hubContext.Clients.All.newPost(model);
                    }

                }
                else if (data.source == "1")
                {
                    if (clients.ContainsKey(data.UserId))
                    {
                        bool result = await MazikCareService.Core.Authentication.ValidateToken(clients[data.UserId]["token"]);
                        if (result)
                        {
                            data = Post.retrieveFormattedAutoPost(data);
                            model.data.records = data;
                        }
                        else
                        {
                            List<string> listErrors = new List<string>();
                            listErrors.Add("User not Authorized");
                            Error error = new Error();
                            error.message = listErrors;
                            List<Error> list = new List<Error>();
                            list.Add(error);
                            model.error = list;
                        }
                        if (data != null)
                        {
                            hubContext.Clients.All.newPost(model);
                        }

                    }
                    else
                    {
                        // User not registered in Signal R dictionary
                    }

                }
                
                /*if(userId.Length > 0)
                {
                    hubContext.Clients.Client(connectionIdAndUserId[userId]).newPost(model);

                }
                else
                {*/
                //hubContext.Clients.All.newPost(model);

                //}
            }
            catch (Exception ex)
            {
                hubContext.Clients.All.newPost(ex);
            }
        }

        public static async void sendPostComments(PostComment data)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MessagingHub>();
            //var tuple = Tuple.Create(data);
            try
            {
                ApiResponseModel<PostComment> model = new ApiResponseModel<PostComment>() { };

                User userData = new User();
                var client = ServiceFactory.GetService(typeof(User));
                userData = await client.getUserEntity(data.createdby);
                data.createdby = userData.Name;
                data.image = userData.image;
                data.text = Post.convertPostMentionsToBold(data.text);


                model.data.records = data;

                hubContext.Clients.All.newPostComment(model);
            }
            catch (Exception ex)
            {
                hubContext.Clients.All.newPostComment(ex);
            }
        }

        public static string getUserIdfromText(string textPosted)
        {
            //Post convertedPost = post;

            if (textPosted.Contains("@["))
            {
                string text = "";
                char[] delimiters = new char[] { '@' };

                string[] mentions = textPosted.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in mentions)
                {
                    string mention = item; ;

                    if (item.Contains("[") && item.Contains("]"))
                    {
                        char[] mentiondelimiters = new char[] { '[', ',', ']' };
                        string[] mentionelemetns = item.Split(mentiondelimiters, StringSplitOptions.RemoveEmptyEntries);
                        if(mentionelemetns.Length > 1)
                        {
                            mention = mentionelemetns[1].ToString().Replace("\"", "");
                        }
                    }

                    text += mention;

                }

                textPosted = text;

            }
            return textPosted;
        }
    }
}