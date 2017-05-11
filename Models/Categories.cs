using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SofttrendsAddon.Models
{
    public class Categories
    {
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string uuid { get; set; }

    }
}
