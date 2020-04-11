using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using Microsoft.AspNet.SignalR;
using System.Web.Http.Cors;
using System.Web.Cors;
using System.Web.Http;

[assembly: OwinStartup(typeof(MazikCareWebApi.OwinStartup))]

namespace MazikCareWebApi
{
    public class OwinStartup
    {

        private static readonly Lazy<CorsOptions> SignalrCorsOptions = new Lazy<CorsOptions>(() =>
        {
            return new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context =>
                    {
                        var policy = new CorsPolicy();
                        policy.AllowAnyOrigin = true;
                        policy.AllowAnyMethod = true;
                        policy.AllowAnyHeader = true;
                        policy.SupportsCredentials = true;
                        return Task.FromResult(policy);
                    }
                }
            };
        });

        public void Configuration(IAppBuilder app)
        {
            //app.Map("/signalr", map =>
            //{
            //    map.UseCors(SignalrCorsOptions.Value);
            //    map.RunSignalR(new HubConfiguration());
            //});
            //now start the WebAPI app
            //GlobalConfiguration.Configure(MazikCareService.WebApiConfig.Register);

            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                };


                hubConfiguration.EnableJSONP = true;
                map.RunSignalR(hubConfiguration);
            });
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
