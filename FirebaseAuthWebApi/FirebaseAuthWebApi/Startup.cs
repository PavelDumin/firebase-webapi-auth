using System.Web.Http;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FirebaseAuthWebApi.Startup))]
namespace FirebaseAuthWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfiguration = new HttpConfiguration();

            //Setup Firebase Authentication
            FirebaseAuthConfig.Register(app);

            WebApiConfig.Register(httpConfiguration);

            app.UseWebApi(httpConfiguration);
        }
    }
}
