using sample_cdn_api.Attributes;
using sample_cdn_api.Cache;
using sample_cdn_api.Models;
using sample_cdn_api.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using System.Data.Entity;
using System.Data.SqlClient;
using Dapper;

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

        [HttpGet]
        [Route("resilentexternalcall/{id}")]
        [Cacheability(Cache = false)]
        public async Task<ValueModel> ResilentExternalCall(string id)
        {
            ValueServices service = new ValueServices();
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Task<ValueModel> serviceTask = service.GetValue(id, cts.Token);

                if (!Task.WaitAll(new Task[] { serviceTask }, 2000))
                {
                    cts.Cancel();
                    System.Diagnostics.Trace.TraceInformation("ResilentExternalCall:  From Cache");
                    return await Task.FromResult(CacheHelper.Get<ValueModel>(id));
                }
                else
                {
                    System.Diagnostics.Trace.TraceInformation("ResilentExternalCall:  From Live");
                    return await Task.FromResult(serviceTask.Result);
                }
            }
        }

        /*
          https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-connect-msi
          
            Magic is:
            - Install NuGet:  Microsoft.Azure.Services.AppAuthentication
            - In the connection string, no username and password, just: 
            
                Authentication=Active Directory Interactive;UID=<some name>
        */

        [HttpGet]
        [Route("getvaluesfromef")]
        [Cacheability(Cache = false)]
        public async Task<IEnumerable<string>> GetValuesFromEF()
        {
            using (authtestEntities context = new authtestEntities())
            {
                return (await context.Values.ToListAsync()).Select(v => v.Value1);
            }
        }

        /*
            https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-connect-msi
          
            Magic is:
            - Install NuGet:  Microsoft.Azure.Services.AppAuthentication
            - In the connection string, no username and password, just: 
            
                Authentication=Active Directory Interactive;UID=<some name>
        */

        [HttpGet]
        [Route("getvaluesfromdapper")]
        [Cacheability(Cache = false)]
        public async Task<IEnumerable<string>> GetValuesFromDapper()
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["authtestEntitiesSQL"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                await connection.OpenAsync();
                IEnumerable<SQLValue> values = await connection.QueryAsync<SQLValue>("SELECT Id, [Value] FROM [VALUES]");
                return values.Select(v => v.Value);

            }
        }
    }
}
