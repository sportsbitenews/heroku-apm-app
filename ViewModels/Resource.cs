using System;

namespace SofttrendsAddon.ViewModels
{
    public class Resource
    {
        public Guid? uuid { get; set; }
        public string heroku_id { get; set; }
        public string app { get; set; }
        public string email { get; set; }
        public string plan { get; set; }
        public string region { get; set; }
        public string callback_url { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
