using sample_cdn_api.Cache;
using sample_cdn_api.Models;
using System.Web.Http;

namespace sample_cdn_api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            char a = 'a';
            char A = 'A';

            for ( int ii = 0; ii < 26; ii++)
            {
                CacheHelper.Add(((char)(a + ii)).ToString(), new ValueModel() { Cached = true });
                CacheHelper.Add(((char)(A + ii)).ToString(), new ValueModel() { Cached = true });
            }

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
