using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace sample_cdn_api.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CacheabilityAttribute : ActionFilterAttribute
    {
        public bool Cache { get; set; }
        public int CacheDurationInSeconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            actionExecutedContext.Response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();

            if (((int)actionExecutedContext.Response.StatusCode > 399 && (int)actionExecutedContext.Response.StatusCode != 404) ||
                  actionExecutedContext.Exception != null || !Cache)

            {
                actionExecutedContext.Response.Headers.CacheControl.NoStore = true;
                actionExecutedContext.Response.Headers.CacheControl.NoCache = true;
                actionExecutedContext.Response.Headers.CacheControl.MustRevalidate = true;
                actionExecutedContext.Response.Headers.CacheControl.MaxAge = TimeSpan.FromSeconds(0);
                actionExecutedContext.Response.Headers.CacheControl.MustRevalidate = true;
                actionExecutedContext.Response.Headers.CacheControl.ProxyRevalidate = true;
                actionExecutedContext.Response.Headers.CacheControl.MaxStale = false;
                actionExecutedContext.Response.Headers.CacheControl.Private = true;
            }
            else
            {
                actionExecutedContext.Response.Content.Headers.Expires = DateTimeOffset.UtcNow.AddSeconds(CacheDurationInSeconds);
                actionExecutedContext.Response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
                actionExecutedContext.Response.Headers.CacheControl.MaxAge = TimeSpan.FromSeconds(CacheDurationInSeconds);
                actionExecutedContext.Response.Headers.CacheControl.Public = true;
            }

            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

    }
}