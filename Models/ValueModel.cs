using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sample_cdn_api.Models
{
    public class ValueModel
    {
        public ValueModel()
        {
            Time = DateTime.Now;
        }

        public DateTime Time { get; set; }
        public bool Cached { get; set; }
    }
}