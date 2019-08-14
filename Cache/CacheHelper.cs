using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sample_cdn_api.Cache
{
    public static class CacheHelper
    {
        public static System.Web.Caching.Cache Cache = new System.Web.Caching.Cache();
        
        public static T Get<T>(string key) where T : class
        {
            return Cache.Get(key) as T;
        }

        public static void Add<T>(string key, T value) where T : class
        {
            Cache.Add(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
        }
    }
}