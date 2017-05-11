using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SofttrendsAddon.Models
{
    public class Applications
    {
        public string appName { get; set; }
        public string appId { get; set; }
        public string appAlaisName { get; set; }
        public string app_json { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string appAccess { get; set; }
        public string categoryId { get; set; }

    }
}
