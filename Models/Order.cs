using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sample_cdn_api.Models
{
    public class Order
    {
        public DateTimeOffset OrderedOn { get; set; }
        public List<string> ItemsOrdered { get; set; }
        public double TotalAmount { get; set; }
    }
}