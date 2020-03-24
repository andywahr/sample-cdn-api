using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sample_cdn_api_dotnetcore
{
    public class ValueServices
    {
        static Random random = new Random();

        public async Task<int> GetValue(System.Threading.CancellationToken token)
        {
            int waitNumberOfSeconds = random.Next(4);

            for (int ii = 0; ii < waitNumberOfSeconds; ii++)
            {
                if (token.IsCancellationRequested)
                {
                    return -1;
                }
                await Task.Delay(1000);
            }

            return waitNumberOfSeconds;
        }
    }
}
