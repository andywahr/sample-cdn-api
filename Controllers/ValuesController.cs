using sample_cdn_api.Attributes;
using System.Collections.Generic;
using System.Web.Http;

namespace sample_cdn_api.Controllers
{
    // reference https://docs.microsoft.com/en-us/azure/frontdoor/front-door-caching

    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        [HttpGet]
        [Route("cacheableget")]
        [Cacheability(Cache = true, CacheDurationInSeconds = 10 * 60)]
        public IEnumerable<string> CacheableGet()
        {
            System.Diagnostics.Trace.TraceInformation("Hit Cacheable");
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("noncacheableget")]
        [Cacheability(Cache = false)]
        public IEnumerable<string> NotCacheableGet()
        {
            System.Diagnostics.Trace.TraceInformation("Hit Not Cacheable");
            return new string[] { "value1", "value2" };
        }
    }
}
