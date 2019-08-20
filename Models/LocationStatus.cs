using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sample_cdn_api.Models
{
    public class LocationStatus
    {
        public int StoreId { get; set; }
        public List<string> Schedule { get; set; }
    }
}