using sample_cdn_api.Cache;
using sample_cdn_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace sample_cdn_api.Services
{
    public class ValueServices
    {
        static Random random = new Random();

        public async Task<ValueModel> GetValue(string valueName, System.Threading.CancellationToken token)
        {
            int waitNumberOfSeconds = random.Next(4);

            for (int ii = 0; ii < waitNumberOfSeconds; ii++)
            {
                if ( token.IsCancellationRequested )
                {
                    return null;
                }
                await Task.Delay(1000);
            }

            return new ValueModel();
        }
    }
}